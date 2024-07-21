using Godot;
using System;

public partial class FuelBarHandler : Panel
{
	private ProgressBar _fuelProgressBar;
	private StyleBox _background;
	private StyleBox _fill;

	private Color _currentColor;

	private float _remainTarget;
	private Tween _blinkingTween;
	private AudioStreamPlayer _alertAudio;
	private float _consume;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fuelProgressBar = GetNode<ProgressBar>("HBoxContainer/ProgressBar");

		_background = _fuelProgressBar.GetThemeStylebox("background");
		_fill = _fuelProgressBar.GetThemeStylebox("fill");

		_alertAudio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	public void OnRemainingFuelUpdated(float remain)
	{
		_remainTarget = remain * 100f;

		_currentColor = PercentageColor(remain);

		if (remain <= _consume * 5f)
		{
			StartBlinking();
		}
		else
		{
			StopBlinking();
		}
	}

	public void OnFuelConsumeUpdated(float consume)
	{
		_consume = consume;
	}

    public override void _Process(double delta)
    {
        if (Visible)
		{
			_fuelProgressBar.Value = Mathf.Lerp(_fuelProgressBar.Value, _remainTarget, (float)delta * 3f);

			_fill.Set("bg_color", _currentColor);
			_background.Set("bg_color", Color.FromHsv(_currentColor.H, _currentColor.S, 0.1f));
		}
    }

	private Color PercentageColor(float percentage)
	{
		// 100 = fullcolor
		//	0  = emptycolor

		return Color.Color8((byte)((1f - percentage) * 255), (byte)(percentage * 255), 0);
	}

	private void StartBlinking()
	{
		if (IsInstanceValid(_blinkingTween) && _blinkingTween.IsValid() && _blinkingTween.IsRunning()) return;

		_blinkingTween = CreateTween();
		_blinkingTween.SetLoops();

		_blinkingTween.TweenCallback(Callable.From(() => _fuelProgressBar.Visible = false)).SetDelay(0.5f);
		_blinkingTween.Parallel();
		_blinkingTween.TweenCallback(Callable.From(() => _alertAudio.Play()));

		_blinkingTween.TweenCallback(Callable.From(() => _fuelProgressBar.Visible = true)).SetDelay(0.5f);
		_blinkingTween.Parallel();
		_blinkingTween.TweenCallback(Callable.From(() => _alertAudio.Stop()));
	}

	private void StopBlinking()
	{
		if (_blinkingTween != null && _blinkingTween.IsRunning()) 
		{
			_blinkingTween.Kill();
			//_blinkingTween.Dispose();
			_fuelProgressBar.Visible = true;
		}
	}
}
