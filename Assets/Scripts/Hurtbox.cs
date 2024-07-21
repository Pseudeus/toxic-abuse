using Godot;
using System;

public partial class Hurtbox : Area2D
{
	[Signal] public delegate void InvincivilityStartedEventHandler();
	[Signal] public delegate void InvincivilityEndedEventHandler();

	[Export] private PackedScene hitEffect;

	private Timer _timer;
	private bool Invincible
	{
		set
		{
			if (value)
			{
				EmitSignal(SignalName.InvincivilityStarted);
			}
			else
			{
				EmitSignal(SignalName.InvincivilityEnded);
			}
		}
	}

	public Hurtbox()
	{
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimerTimeout));
	}

	public void StartInvincibility(float duration)
	{
		Invincible = true;
		_timer.Start(duration);
	}

	public void CreateHitEffect()
	{
		var effect = (AnimatedSprite2D)hitEffect.Instantiate();
		GetTree().CurrentScene.AddChild(effect);
		effect.GlobalPosition = GlobalPosition;
	}

	private void OnTimerTimeout()
	{
		Invincible = false;
	}

	private void OnHurtBoxInvincibilityStarted()
	{
		SetDeferred(Area2D.PropertyName.Monitoring, false);
	}

	private void OnHurtBoxInvincibilityEnded()
	{
		SetDeferred(Area2D.PropertyName.Monitoring, true);
	}
}
