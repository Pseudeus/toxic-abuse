using Godot;
using System;

public partial class InputComponent : Node
{
	public Vector2 InputCoordinatesVector { get; private set; }
	public Vector2 LastInputCoordinates { get; private set; }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && !eventKey.IsEcho() && eventKey.IsPressed())
		{
			switch (eventKey.Keycode)
			{
				case Key.Right | Key.Left:
					//Input.GetAxis();
					
					break;
				
				case Key.Left:
					InputCoordinatesVector = Vector2.Left;
		
					break;

				case Key.Up:
					InputCoordinatesVector = Vector2.Up;
					break;
				
				case Key.Down:
					InputCoordinatesVector = Vector2.Down;
					break;
			}

			InputCoordinatesVector = InputCoordinatesVector.Normalized();
			//GD.Print(InputCoordinatesVector);
		}
    }
}
