using Godot;
using System;

public partial class CandleLightingHandler : PointLight2D
{
	[Export] private LightMovementSettings lights { get; set; }

	public Action<float> ReloadLightFuelBy;
	public Action IncreaseLightConsume;

	public CandleLightingHandler()
	{
		ReloadLightFuelBy += fuel => {
			lights.ReloadFuel(fuel);
			Enabled = true;
		};

		IncreaseLightConsume += () => {
			lights.FuelConsumedPerSecond += 0.005f;
			//GD.Print(lights.FuelConsumedPerSecond);
		};
	}

    public override void _EnterTree()
    {
        lights = new LightMovementSettings
        {
            initialPosition = Position,
            initialIntensity = Energy,
            initialScale = TextureScale
        };

        lights.OutOfFuel += () => {
			Tween t = CreateTween();
			t.TweenCallback(Callable.From(() => {  
				if (lights.targetIntensity == 0)
				{
					Enabled = false;
				}
			})).SetDelay(1f);
			t.Finished += () => t.Dispose();
		};

		var ui = GetTree().Root.GetNode<FuelBarHandler>("Main/PlayerUI/Control/FuelBar");

		lights.RemainingFuelUpdated += ui.OnRemainingFuelUpdated;
		lights.FuelConsumeUpdated += ui.OnFuelConsumeUpdated;
    }

    public override void _Ready()
	{
		AddChild(lights.updateTimer);
		lights.updateTimer.Owner = this;

		lights.EmitSignal(LightMovementSettings.SignalName.FuelConsumeUpdated, lights.FuelConsumedPerSecond);
	}

    public override void _Process(double delta)
    {
		if (Enabled)
		{
			Position = Position.Lerp(lights.targetPosition, (float)delta * 7f);
			Energy = Mathf.Lerp(Energy, lights.targetIntensity, (float)delta * 3f);
			TextureScale = Mathf.Lerp(TextureScale, lights.targetScale, (float)delta * 3f);
		}
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey && inputEventKey.Pressed && !inputEventKey.IsEcho() && inputEventKey.Keycode == Key.P)
		{
			ReloadLightFuelBy(0.5f);
		}
    }
}
