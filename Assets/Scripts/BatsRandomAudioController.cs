using Godot;
using Godot.Collections;
using System;

public partial class BatsRandomAudioController : AudioStreamPlayer2D
{
	[Export] private Array<AudioStreamOggVorbis> randomAudioList = new ();
	[Export] private float timeBetweenPlaying = 5f;

	private Timer _timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetChild<Timer>(0);
		_timer.Timeout += OnTimerTimeout;
	}

	public void OnTimerTimeout()
	{
		_timer.WaitTime = timeBetweenPlaying + (GD.Randf() * timeBetweenPlaying);
		PlayRandomSound();
		_timer.Start();
	}

	public void PlayRandomSound()
	{
		PitchScale = 0.9f + (GD.Randf() * 0.3f);
		Stream = randomAudioList[(int)(GD.Randi() % randomAudioList.Count)];
		Play();
	}
}
