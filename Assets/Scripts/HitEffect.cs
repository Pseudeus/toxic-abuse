using Godot;

public partial class HitEffect : AnimatedSprite2D
{
	public override void _Ready()
	{
		Connect(AnimatedSprite2D.SignalName.AnimationFinished, Callable.From(() => { QueueFree(); }));
	}
}
