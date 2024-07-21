using Godot;
using System;

public partial class FuelReload : Area2D
{
	private AudioStreamPlayer ap;

	public FuelReload()
	{
		BodyEntered += body => {
			if (body is PlayerMovementController playerNode)
			{
				Visible = false;
				SetDeferred(Area2D.PropertyName.Monitoring, false);
				CandleLightingHandler light = playerNode.GetNode<CandleLightingHandler>("PointLight2D");
				light?.ReloadLightFuelBy(0.5f);
				light.IncreaseLightConsume();
				ap.PitchScale += GD.Randf() * 0.6f;
				ap.Play();
			}
		};
	}

    public override void _Ready()
    {
       	ap = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		ap.Finished += QueueFree;
    }
}
