using Godot;
using System;

public partial class Camera2DShakeHandler : Camera2D
{
	[Export] private Vector2I intensity;
	[Export] private float shakeFriction = 5f;

	private float _intensityFactor;
	private bool _isShaking;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isShaking)
		{
			Offset = new Vector2(GD.RandRange(-intensity.X, intensity.X) * _intensityFactor, GD.RandRange(-intensity.Y, intensity.Y) * _intensityFactor);
			_intensityFactor = Mathf.Lerp(_intensityFactor, 0f, (float)delta * 2f * shakeFriction);

			if (_intensityFactor <= float.Epsilon) _isShaking = false;
		}
	}

	public void Shake()
	{
		_intensityFactor = 1f;
		_isShaking = true;
	}

	// #region Debug

	// public override void _Input(InputEvent @event) {
	// 	if (@event is InputEventKey eventKey && eventKey.Pressed && !eventKey.IsEcho() && eventKey.Keycode == Key.Q)
	// 	{
	// 		Shake();
	// 	}
	// }

	// #endregion
}
