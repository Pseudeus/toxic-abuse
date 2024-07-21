using Godot;
using System;

public partial class PlayerDetectionZone : Area2D
{
	[Export] public bool PlayerHaveLight;
	[Export] public bool GlobalDetectionZone;

	private Node2D _player;
	public Node2D Player => _player;

	public PointLight2D HolyLight { get; private set; }

	private CollisionShape2D _zone;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!GlobalDetectionZone)
		{
			_zone = GetNode<CollisionShape2D>("CollisionShape2D");
			Connect(Area2D.SignalName.BodyEntered, Callable.From<CharacterBody2D>(body => OnPlayerDetectionZoneBodyEntered(body)));
			Connect(Area2D.SignalName.BodyExited, Callable.From<CharacterBody2D>(b => {
				_player = null;
				_zone.Shape.SetDeferred("radius", 80f);
			}));
		}
		else 
		{
			_player = GetTree().GetFirstNodeInGroup("Player") as Node2D;
			Monitoring = false;
		}
	}

	private void OnPlayerDetectionZoneBodyEntered(CharacterBody2D body)
	{
		if (body is PlayerMovementController)
		{
			_player = body;
			_zone.Shape.SetDeferred("radius", 120f);

			if (PlayerHaveLight && !IsInstanceValid(HolyLight))
			{
				HolyLight = _player.GetNode<PointLight2D>("PointLight2D");
			}
		}
	}
}
