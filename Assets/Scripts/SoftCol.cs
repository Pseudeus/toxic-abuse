using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public partial class SoftCol : Node2D
{
	public Vector2 SoftCollisionVector { get; private set; }
	public Action<int, bool> SetCollisionMask { get; private set; }

	private IEnumerable<RayCast2D> _rays = new List<RayCast2D>();
	private bool _isColliding;
	private Vector2 _resultingForce;

	public SoftCol()
	{
		SetCollisionMask = (layer, value) => Parallel.ForEach(_rays, ray => ray.SetCollisionMaskValue(layer, value));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rays = GetChildren().Cast<RayCast2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		_isColliding = false;
		_resultingForce = Vector2.Zero;

		foreach (var ray in _rays)
		{
			if (ray.IsColliding())
			{
				_resultingForce += GlobalPosition - ray.GetCollisionPoint();
				_isColliding = true;
			}
		}
		
		if (!_isColliding)
		{
			SoftCollisionVector = Vector2.Zero;
		}
		else
		{
			SoftCollisionVector = _resultingForce.Normalized();
		}
	}
}
