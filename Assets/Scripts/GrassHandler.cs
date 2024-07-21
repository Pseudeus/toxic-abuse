using Godot;

public partial class GrassHandler : Node2D
{
	private AnimatedSprite2D _destroyEffect;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Area2D>("Hurtbox").Connect(Area2D.SignalName.AreaEntered, Callable.From<Area2D>(a => OnHurtboxAreaEntered(a)));

		_destroyEffect = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_destroyEffect.Connect(AnimatedSprite2D.SignalName.AnimationFinished, Callable.From(OnAnimatedSpriteAnimationFinished));
	}

	private void OnAnimatedSpriteAnimationFinished()
	{
		QueueFree();
	}

	private void OnHurtboxAreaEntered(Area2D area)
	{
		_destroyEffect.Play("Destroy");
	}
}
