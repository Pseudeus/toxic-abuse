using Godot;
using System;
using System.Collections.Generic;

public interface ITear 
{
	ITear PrevTear { get; set; }
	ITear NextTear { get; set; }
	bool IsChecked { get; } 
	bool FirstTear { get; set; }
	bool LastTear { get; set; }
}

public partial class TearsCounterHandler : Node2D
{
	[Signal] public delegate void AllTearsCapturedEventHandler();
	[Signal] public delegate void TearCapturedEventHandler(TearList tear);
	private List<ITear> _tears = new ();

	private AudioStreamPlayer _voicePlayer;
	private AudioStreamPlayer _interferencePlayer;

	private Action _PlayAudioRecord;

	public TearsCounterHandler()
	{
		Connect(SignalName.AllTearsCaptured, Callable.From(AllCaptured));
		TearCaptured += OnTearCaptured;

		_PlayAudioRecord += delegate {
			_voicePlayer.Finished += () => {
				var t = CreateTween();
				t.SetParallel();
				t.TweenMethod(Callable.From<int>(volume => _interferencePlayer.VolumeDb = volume), -12, 0, 1);
				t.TweenCallback(Callable.From(_interferencePlayer.Stop)).SetDelay(2f);
				t.Finished += () => t.Dispose();
			};

			Tween _tween = CreateTween();
			_tween.TweenCallback(Callable.From(() => _interferencePlayer.Play())).SetDelay(2f);
			_tween.TweenCallback(Callable.From(() => _voicePlayer.Play())).SetDelay(2f);
			_tween.TweenMethod(Callable.From<int>(volume => _interferencePlayer.VolumeDb = volume), 0, -14, 2).SetDelay(4f);
			_tween.Parallel().TweenCallback(Callable.From(() => _voicePlayer.VolumeDb += 14)).SetDelay(109f);
			_tween.Finished += () => _tween.Dispose();
		};
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_voicePlayer = GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
		_interferencePlayer = GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer2");

		var tears = GetChildren();

		foreach (var t in tears)
		{
			if (t is ITear tear) _tears.Add(tear);
		}

		for (byte i = 0; i < _tears.Count - 1; i++)
		{
			_tears[i].NextTear = _tears[i + 1];
			_tears[i + 1].PrevTear = _tears[i];
		}
	}

	private void AllCaptured()
	{
		var fade = GetTree().GetFirstNodeInGroup("UI") as IFader;

		fade.FadeOut();
	}

	private void OnTearCaptured(TearList tear)
	{
		if (tear == TearList.Ichi && _voicePlayer != null)
		{
			AudioStreamPlayer mainPlayer = GetParent().GetNode<AudioStreamPlayer>("AudioStreamPlayer");

			if (mainPlayer.Playing)
			{
				mainPlayer.Finished += _PlayAudioRecord;
			}
			else _PlayAudioRecord();
		}
	}
}

public enum TearList : byte
{
	Ichi,
	Ni,
	San,
	Yon,
	Go,
	Roku,
	Nana,
	Hachi,
	Kyu,
	Juu,
	None
}
