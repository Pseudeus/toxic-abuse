using Godot;
using System;

public partial class EnemiesAudioSetter : Node2D
{
	[Export] private PackedScene audioScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var children = GetChildren();

		foreach (Node child in children)
		{
			var audio = audioScene.Instantiate<AudioStreamPlayer2D>();
			child.AddChild(audio);
			audio.Owner = child;
		}
	}
}
