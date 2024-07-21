using Godot;
using Godot.Collections;

public partial class SoftCollision : Area2D
{
	private bool IsColliding(Array<Node2D> areas)
	{
		return areas.Count > 0;
	}

	public Vector2 GetPushVector()
	{
		var areas = GetOverlappingBodies();
		
		if (IsColliding(areas))
		{
			Vector2 pushVector;

			var area = areas[0];

			pushVector = area.GlobalPosition.DirectionTo(GlobalPosition);
			return pushVector;
		}
		return Vector2.Zero;
	}
}
