using Godot;
using System;

[GlobalClass]
public partial class LightMovementSettings : Resource
{
    [Signal] public delegate void OutOfFuelEventHandler();
    [Signal] public delegate void RemainingFuelUpdatedEventHandler(float remain);
    [Signal] public delegate void FuelConsumeUpdatedEventHandler(float consume);

    [Export] public Vector2 PossitionOffset { get; set; } = new Vector2(2, 2);
	[Export] public float ScaleOffset { get; set; } = 0.4f;
	[Export] public float IntensityOffset { get; set; } = 0.6f;
    [Export] public float FuelConsumedPerSecond { get => _consume; set { _consume = Mathf.Clamp(value, 0.005f, 0.08f); EmitSignal(SignalName.FuelConsumeUpdated, _consume); } }

    public float initialScale;
	public Vector2 initialPosition;
	public float initialIntensity;

	public float targetIntensity { get; private set; }
	public float targetScale { get; private set; }
	public Vector2 targetPosition { get; private set; }
	public Timer updateTimer { get; private set; }
    public float FuelStatus { get { return _fuelStatus; } private set { _fuelStatus = Mathf.Clamp(value, 0f, 1f);  EmitSignal(SignalName.RemainingFuelUpdated, _fuelStatus); } }


    private float _fuelStatus = 1f;
    private float _consume = 0.01f;
	private Random _random;

    public LightMovementSettings()
    {
        _random = new Random();

        updateTimer = new Timer
        {
            WaitTime = 0.2f,
			Autostart = true
        };

        //You need explicitly add the timer node to the scene the resourse has been implemented
        //updateTimer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimerTimeout));
        updateTimer.Timeout += OnTimerTimeout;

        OutOfFuel += () => {
            updateTimer.Stop();
            targetIntensity = 0f;
        };

        // RemainingFuelUpdated += rem => GD.Print(rem); 
    }

    public void OnTimerTimeout()
	{
		targetPosition = initialPosition + GetRandomRange(-PossitionOffset, PossitionOffset);

		float inten = GetRandomRange(-IntensityOffset, IntensityOffset);
		targetIntensity = initialIntensity + inten;

		targetScale = initialScale + IntensityToScaleRemap(inten);

        if (FuelStatus > float.Epsilon)
        {
            FuelStatus -= FuelConsumedPerSecond / (1f / (float)updateTimer.WaitTime);
        }
        else
        {
            FuelStatus = 0;
            EmitSignal(SignalName.OutOfFuel);
        }
	}

    public void ReloadFuel(float reload)
    {
        FuelStatus += reload;
        updateTimer.Start();
    }

    private Vector2 GetRandomRange(Vector2 from, Vector2 to)
	{
		Vector2 range = to - from;

		return new Vector2(_random.NextSingle() * range.X, _random.NextSingle() * range.Y) + from;
	}

	private float GetRandomRange(float from, float to)
	{
		float range = to - from;

		return _random.NextSingle() * range + from;
	}

	private float IntensityToScaleRemap(float input)
	{
		//intensity range : -0.7 -- 0.7
		//scale 	range : -0.5 -- 0.5

		// 0.7 = 0.5
		// 0.2 = ?.?

		return input * ScaleOffset / IntensityOffset;
	}
}
