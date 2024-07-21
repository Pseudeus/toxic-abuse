using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class AxeAdapter : Area2D
{
	//Return the player node
	public new Node GetParent()
	{
		return base.GetParent().GetParent().GetParent();
	}
}
