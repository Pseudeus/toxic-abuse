using Godot;
using Godot.Collections;
using System;

public partial class Components : Node
{
	[Export] public Array<Resource> Resources { get; set; } = new Array<Resource>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
