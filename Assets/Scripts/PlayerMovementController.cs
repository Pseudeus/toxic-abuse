using Godot;

public partial class PlayerMovementController : CharacterBody2D
{
	[Export][ExportGroup("Movement Settings")] private const float speed = 80f;
	[Export] private const float acceleration = 700f;
	[Export] private const float friction = 700f;
	[Export] private const float rollSpeed = 120f;
	[Export] private Camera2DShakeHandler shakeHandler;


	[Export][ExportGroup("Components")] private Stats stats;

	private AnimationTree _animationTree;
	private AnimationNodeStateMachinePlayback _animationState;
	private Vector2 _inputCoordinatesVector;
	private Vector2 _lastInputCoordinates;
	private Vector2 _velocity = Vector2.Zero;
	private MovementState _state = MovementState.Move;
	private StatsComponent _stats;
	private Area2D _hurtbox;
	private bool _ignoreInputs;
	private BlinkComponent _blinkComp;
	private ReceiveDamageComponent _dmgSoundComp;
	private Timer _safeRollAnimState;

	public enum MovementState
	{
		Move,
		Roll,
		Attack
	}
	
	public Vector2 FacingDirection => _lastInputCoordinates.Normalized();
	public byte Damage => _stats.HitPoints;
	public bool CanAttack { get; set; }
	public bool CanDash { get; set; }
	public bool FirstInteraction { get; set; }

	public override void _EnterTree() 
	{
		var ui = GetTree().GetFirstNodeInGroup("UI") as PlayerUI;

		Ready += ui.OnPlayerReady;
		TreeExiting += ui.DisconnectPlayer;
	}

	public override void _Ready()
	{

		_safeRollAnimState = new Timer();
		AddChild(_safeRollAnimState);
		_safeRollAnimState.Owner = this;
		_safeRollAnimState.OneShot = true;
		_safeRollAnimState.WaitTime = 0.45f;

		_safeRollAnimState.Connect(Timer.SignalName.Timeout, Callable.From(OnSafeRollAnimStateTimeout));

		_blinkComp = GetNode<BlinkComponent>("Components/BlinkComponent");
		_dmgSoundComp = GetNode<ReceiveDamageComponent>("Components/ReceiveDamageComponent");

		_stats = GetNode<StatsComponent>("Components/StatsComponent");
		_stats.Connect(StatsComponent.SignalName.NoHealth, Callable.From(ProcessDestroyNode));

		_hurtbox = GetNode<Area2D>("Hurtbox");
		_hurtbox.Connect(Area2D.SignalName.AreaEntered, Callable.From<Area2D>(a => OnHurtboxAreaEntered(a)));

		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationState = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
		{
			if (_stats.IsInGroup("PlayerStats"))
			{
				_stats.RemoveFromGroup("PlayerStats");
			}
		}
    }

    public void OnHurtboxAreaEntered(Area2D area)
	{
		if (!IsInstanceValid(area)) return;

		var bat = area.GetParent<BatController>();
		if (bat.CurrentMovementState == BatController.MovementState.RunningAway) return;
		

		var eDamage = bat.GetNode<StatsComponent>("Components/StatsComponent").HitPoints;
		_stats.ProcessHit(eDamage);

		if (_stats.HealthPoints > 0) _blinkComp.StartBlinking();
		_dmgSoundComp.ProcessAudioPetition();
		shakeHandler?.Shake();
	}

	public void ProcessDestroyNode()
	{
		QueueFree();
	}

	public void LockMovement()
	{
		_ignoreInputs = true;
		_inputCoordinatesVector = Vector2.Zero;
	}

	public void FreeMovement()
	{
		_ignoreInputs = false;
	}

	private void OnSafeRollAnimStateTimeout()
	{
		if (_ignoreInputs) 
		{
			_animationState.Travel("Walk");
			_inputCoordinatesVector = Vector2.Zero;
			OnRollAnimationFinished();
		}
	}

	private void MoveState(float delta)
	{
		if (!_ignoreInputs)
		_inputCoordinatesVector = new Vector2(Input.GetAxis("ui_left", "ui_right"), Input.GetAxis("ui_up", "ui_down"));

		if (_inputCoordinatesVector != Vector2.Zero)
		{
			_lastInputCoordinates = _inputCoordinatesVector;
			SetBlendTrees(_inputCoordinatesVector);
			_animationState.Travel("Walk");
			_velocity = _velocity.MoveToward(_inputCoordinatesVector.Normalized() * speed, acceleration * delta);
		}
		else
		{
			_animationState.Travel("Idle");
			_velocity = _velocity.MoveToward(Vector2.Zero, friction * delta);
		}

		if (Input.IsActionJustPressed("ui_select") && !_ignoreInputs)
		{
			if (CanAttack)
			{
				_state = MovementState.Attack;
			}
		}
		if (Input.IsActionJustPressed("game_dash") && !_ignoreInputs)
		{
			if (CanDash)
			{
				_safeRollAnimState.Start();
				_state = MovementState.Roll;
			}
		}
	}

	private void SetBlendTrees(Vector2 coord)
	{
		_animationTree.Set("parameters/Idle/blend_position", coord);
		_animationTree.Set("parameters/Walk/blend_position", coord);
		_animationTree.Set("parameters/Attack/blend_position", coord);
		_animationTree.Set("parameters/Roll/blend_position", coord);
	}

	private void AttackState()
	{
		if (_lastInputCoordinates == Vector2.Zero) SetBlendTrees(Vector2.Right);

		_velocity = Vector2.Zero;
		_animationState.Travel("Attack");
		_ignoreInputs = true;
	}

	private void RollState()
	{
		if (_lastInputCoordinates == Vector2.Zero)
		{
			_lastInputCoordinates = Vector2.Right;
			SetBlendTrees(_lastInputCoordinates);
		}

		_velocity = _lastInputCoordinates.Normalized() * rollSpeed;
		_animationState.Travel("Roll");
		_ignoreInputs = true;
	}

	public void OnAttackAnimationFinished()
	{
		_state = MovementState.Move;
		_ignoreInputs = false;
	}

	public void OnRollAnimationFinished()
	{
		_ignoreInputs = false;
		_state = MovementState.Move;
		Velocity /= 3f;
		//SetDeferred(PropertyName._state, 0);
	}

    public override void _Process(double delta)
    {
        MoveState((float)delta);
		switch (_state)
		{
			case MovementState.Move:
				MoveState((float)delta);
				break;
			
			case MovementState.Roll:
				RollState();
				break;

			case MovementState.Attack:
				AttackState();
				break;
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		if (_velocity != Vector2.Zero)
		{
			Velocity = _velocity;
			MoveAndSlide();
		}
	}
}

