using Godot;
using System;
using System.Reflection;

public partial class VisibleOnScreenEnabler2DHandler : VisibleOnScreenEnabler2D
{
	[Export] public bool EnableQueueFreeing;
	public EnemySpawner Spawner;

	private Timer _timer;

	public override void _EnterTree() 
	{
		if (EnableQueueFreeing && _timer == null)
		{
			
			ProcessMode = ProcessModeEnum.Always;

            _timer = new()
            {
                WaitTime = 0.6f,
                OneShot = true,
                Autostart = true
            };

			AddChild(_timer);
			_timer.Owner = this;

            _timer.Timeout += () => {
				Spawner.EnqueueBat(GetParent() as BatController);
				_timer.Autostart = true;
				GetParent().GetParent().RemoveChild(GetParent());
			};

			ScreenExited += () => _timer.Start();
			ScreenEntered += () => _timer.Stop();
		}	
	}
}
