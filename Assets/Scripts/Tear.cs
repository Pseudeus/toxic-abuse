using Godot;
using System;

public partial class Tear : Area2D, ITear
{
	

	[Export] private TearList tearNum;
	[Export] private AudioStream numAS;
	[Export] public bool FirstTear { get; set; }
	[Export] public bool LastTear { get; set; }

    public ITear PrevTear { get; set; }
    public ITear NextTear { get; set; }

    public bool IsChecked { get; private set; }

	private AudioStreamPlayer _audioStreamPlayer;

	public Tear()
	{
		Connect(Area2D.SignalName.BodyEntered, Callable.From<Node>(n => OnArea2dBodyEntered(n)));
	}

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_audioStreamPlayer.Stream = numAS;
	}

	public void OnArea2dBodyEntered(Node body)
	{
		if (body is PlayerMovementController && (FirstTear || PrevTear.IsChecked) && !IsChecked)
		{
			IsChecked = true;
			// GD.Print(tearNum, " madafaka");
			_audioStreamPlayer.Play();

			var parent = GetParent();

			parent.EmitSignal(TearsCounterHandler.SignalName.TearCaptured, (byte)tearNum);

			if (LastTear)
			{
				parent.EmitSignal(TearsCounterHandler.SignalName.AllTearsCaptured);
			}
		}
	}
}
