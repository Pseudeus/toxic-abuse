using Godot;
using System;
using System.Collections.Generic;

//This node must be atached as a child of the current camera2D
public partial class EnemySpawner : Node2D
{
	[Export] private PackedScene enemy;
	[Export] private float spawningTime = 1.5f;
	[Export] private EnemiesContainer spawningNode;
	[Export] private Camera2D cameraView;

	public Action<double> StartSpawning { get; private set; }

	private Queue<BatController> _ringBCController;
	private Timer _spawnTimer;
	private PathFollow2D _spawnCoords;
	private Area2D _coordChecker;
	private bool _spawnable;

	public EnemySpawner()
	{
		_ringBCController = new Queue<BatController>();
	}

	public override void _Ready()
	{
		_spawnTimer = GetNode<Timer>("SpawnTimer");
		_spawnCoords = GetNode<PathFollow2D>("SpawnCoords/SpawnLocation");
		_coordChecker = GetNode<Area2D>("Area2D");

		StartSpawning += _spawnTimer.Start;
		_spawnTimer.Timeout += OnTimerTimeout;
		_spawnTimer.WaitTime = spawningTime;
	}

	public void EnqueueBat(BatController bat)
	{
		_ringBCController.Enqueue(bat);
	}

    private async void OnTimerTimeout()
    {

        _spawnCoords.ProgressRatio = GD.Randf();
        _coordChecker.GlobalPosition = (_spawnCoords.Position / 4) + cameraView.GlobalPosition - (GetViewportRect().Size / 2);

		await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);

		//GD.Print(!_coordChecker.HasOverlappingBodies());

		if (!_coordChecker.HasOverlappingBodies())
		{
        	InstantiateCustomBat(out BatController bat, out bool newInstance);
			bat.GlobalPosition = _coordChecker.GlobalPosition;
			spawningNode.AttachEnemy(ref bat, out bool full, out float deltaWT, spawningTime, _ringBCController.Count);

			_spawnTimer.WaitTime = deltaWT;

			if (newInstance)
			{
				var softCol = bat.GetNode<SoftCol>("SoftCol");
				softCol.SetCollisionMask(5, true);
			}

			if (full) _spawnTimer.Stop();
		}
    }

    private void InstantiateCustomBat(out BatController bat, out bool newInstance)
    {
        if (_ringBCController.Count > 0)
		{
			bat = _ringBCController.Dequeue();
			newInstance = false;
		}
		else 
		{
			bat = enemy.Instantiate<BatController>();
			var en = bat.GetNode<StatsComponent>("Components/StatsComponent");

			en.Invincible = false;
			en.MaxHealthPoints = (byte)((GD.Randi() % 3) + 2);
			en.HitPoints = (byte)((GD.Randi() % 2) + 1);

			//buff the movement specs too

			bat.Acceleration += GD.Randf() * 50f;
			bat.MaxSpeed += GD.Randf() * 45f;
			bat.Friction += GD.Randf() * 30f;
			bat.KnockBackSpeed += GD.Randf() * 80f;
			bat.SetCollisionMaskValue(5, true);

			var detection = bat.GetNode<PlayerDetectionZone>("PlayerDetectionZone");
			detection.GlobalDetectionZone = true;

			var screenEnabler = bat.GetNode<VisibleOnScreenEnabler2DHandler>("VisibleOnScreenEnabler2D");

			screenEnabler.EnableQueueFreeing = true;
			screenEnabler.Spawner = this;
			newInstance = true;
		}
    }
}
