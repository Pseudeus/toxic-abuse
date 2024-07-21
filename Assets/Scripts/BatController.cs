using Godot;
using System;

public partial class BatController : CharacterBody2D
{
	[Export] public float Acceleration { get; set; } = 250f;
	[Export] public float MaxSpeed { get; set; } = 60f;
	[Export] public float Friction { get; set; } = 200f;
	[Export] public float KnockBackSpeed { get; set; } = 150f;

	public MovementState CurrentMovementState => _movementState;

	public enum MovementState { Idle, Wander, Chase, RunningAway }

	private Hurtbox _hurtbox;
	private Area2D _hitbox;
	//private SoftCollision _softCollision;
	private SoftCol _newSoftCollision;
	private CollisionShape2D _hitBoxCollisionShape;
	private CollisionShape2D _hurtBoxCollisionShape;
	private Sprite2D _shadow;
	private AnimatedSprite2D _animatedSprite;
	private Vector2 _knockVelocity;
	private Vector2 _velocity;
	private Vector2 _wanderPosition;
	private StatsComponent _stats;
	private MovementState _movementState;
	private PlayerDetectionZone _detectionZone;
	private WanderController _wanderController;
	private BlinkComponent _blinkComp;
	private bool _alive = true;

	public override void _Ready()
	{
		_stats = GetNode<StatsComponent>("Components/StatsComponent");
		_shadow = GetNode<Sprite2D>("Sprite2D");
		_hurtbox = GetNode<Hurtbox>("Hurtbox");
		//_softCollision = GetNode<SoftCollision>("SoftCollision");
		_newSoftCollision = GetNode<SoftCol>("SoftCol");
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_wanderController = GetNode<WanderController>("WanderController");
		_hitbox = GetNode<Area2D>("Hitbox");
		_detectionZone = GetNode<PlayerDetectionZone>("PlayerDetectionZone");
		_blinkComp = GetNode<BlinkComponent>("Components/BlinkComponent");

		_hurtbox.Connect(Area2D.SignalName.AreaEntered, Callable.From<Area2D>(a => OnHurtBoxAreaEntered(a)));
		_stats.Connect(StatsComponent.SignalName.NoHealth, Callable.From(() => ProcessDestroyNode()));
		

		if (GetParent() is EnemiesCounter ec)
		{
			_stats.Connect(StatsComponent.SignalName.NoHealth, Callable.From(ec.OnEnemyKilled));
		}

		_wanderController.Connect(WanderController.SignalName.PositionChanged, Callable.From<Vector2>(p => OnWanderControllerPositionChanged(p)));

		_movementState = SetRandomState(MovementState.Idle, MovementState.Wander);
	}

    public override void _PhysicsProcess(double delta)
    {  
		_velocity = Velocity;
		_knockVelocity = _knockVelocity.MoveToward(Vector2.Zero, Friction * (float)delta);
		Velocity = _knockVelocity;
		MoveAndSlide();

		switch (_movementState)
		{
			case MovementState.Idle:
				_velocity = _velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
				SeekPlayer();
				break;

			case MovementState.Wander:
				SeekPlayer();
				MoveTowardsPoint(_wanderPosition, (float)delta);

				if (GlobalPosition.DistanceSquaredTo(_wanderPosition) <= Mathf.Pow(MaxSpeed * (float)delta, 2))
				{
					_movementState = MovementState.Idle;
				}
				break;

			case MovementState.Chase:
				if (IsInstanceValid(_detectionZone.Player) && _alive)
				{
					if (_detectionZone.PlayerHaveLight && _detectionZone.HolyLight.Enabled)
					{
						_movementState = MovementState.RunningAway;
					}
					else
					{
						MoveTowardsPoint(_detectionZone.Player.GlobalPosition, (float)delta);
					}
				}
				else
				{
					_movementState = MovementState.Idle;
				}
				break;

			case MovementState.RunningAway:
				if (IsInstanceValid(_detectionZone.Player) && _alive && _detectionZone.HolyLight.Enabled)
				{
					MoveAgainstPoint(_detectionZone.Player.GlobalPosition, (float)delta);
				}
				else
				{
					_movementState = MovementState.Idle;
				}
				break;
		}

		_velocity += _newSoftCollision.SoftCollisionVector * (float)delta * 400f;
		Velocity = _velocity;
		MoveAndSlide();
    }

	private void MoveTowardsPoint(Vector2 targetPosition, float delta)
	{
		var direction = (targetPosition - GlobalPosition).Normalized();
		_velocity = _velocity.MoveToward(direction * MaxSpeed, Acceleration * delta);
		_animatedSprite.FlipH = _velocity.X < float.Epsilon;
	}

	private void MoveAgainstPoint(Vector2 targetPosition, float delta)
	{
		var direction = (GlobalPosition - targetPosition).Normalized();
		_velocity = _velocity.MoveToward(direction * MaxSpeed, Acceleration * delta);
		_animatedSprite.FlipH = _velocity.X < float.Epsilon;
	}

	private void SeekPlayer()
	{
		if (_detectionZone.Player != null)
		{
			if (!_detectionZone.PlayerHaveLight)
			{
				_movementState = MovementState.Chase;
			}
			else 
			{
				if (_detectionZone.HolyLight.Enabled)
				{
					_movementState = MovementState.RunningAway;
				}
				else
				{
					_movementState = MovementState.Chase;
				}
			}
		}
	}

	private void OnWanderControllerPositionChanged(Vector2 newPosition)
	{
		_movementState = SetRandomState(MovementState.Idle, MovementState.Wander);
		_wanderPosition = newPosition;
	}

	private MovementState SetRandomState(params MovementState[] states)
	{
		_wanderController.StartTimer((byte)GD.RandRange(1, 3));
		return states[GD.RandRange(0, states.Length - 1)];
	}

	public void OnHurtBoxAreaEntered(Area2D area)
	{
		var player = area.GetParent() as PlayerMovementController;

		if (player == null && area is AxeAdapter axeArea)
		{
			player = axeArea.GetParent() as PlayerMovementController;
			_knockVelocity = (GlobalPosition - player.GlobalPosition).Normalized() * KnockBackSpeed;
		}

		else
		{
			_knockVelocity = player.FacingDirection * KnockBackSpeed;
		}


		if (_stats.Invincible) return;

		_stats.ProcessHit(player.Damage);
		_hurtbox.CreateHitEffect();

		if (_stats.HealthPoints > 0) _blinkComp.StartBlinking();
	}

	public void ProcessDestroyNode()
	{
		_alive = false;
		_wanderController.StopTimer();
		_hurtbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
		_hitbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
		_shadow.Visible = false;
		_knockVelocity= Vector2.Zero;
		_velocity = Vector2.Zero;
		_animatedSprite.Play("Death");

		GetNode<AudioStreamPlayer>("AnimatedSprite2D/AudioStreamPlayer").Play();
		_animatedSprite.Connect(AnimatedSprite2D.SignalName.AnimationFinished, Callable.From(OnAnimationSpriteAnimationFinished));
	}

	private void OnAnimationSpriteAnimationFinished()
	{
		QueueFree();
	}
}
