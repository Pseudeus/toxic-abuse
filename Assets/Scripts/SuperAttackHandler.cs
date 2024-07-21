using Godot;
using System;

public partial class SuperAttackHandler : Node2D
{
	private AnimationPlayer _anim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Input(InputEvent @event) 
	{
		if (@event is InputEventKey eventKey && eventKey.IsPressed() && !eventKey.Echo && eventKey.Keycode == Key.E)
		{
			if (!_anim.IsPlaying())
			{
				_anim.Play("Attack");
			}
		}
	}
}
