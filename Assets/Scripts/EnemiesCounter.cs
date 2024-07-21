using Godot;
using System;

public partial class EnemiesCounter : Node2D
{
	[Signal] public delegate void AllEnemiesKilledEventHandler();
	[Signal] public delegate void EnemyKilledEventHandler();

	private int _enemiesCounter;

	public override void _Ready()
	{
		_enemiesCounter = GetChildCount();
	}

	public void OnEnemyKilled()
	{
		if (_enemiesCounter > 1)
		{
			_enemiesCounter--;
			EmitSignal(SignalName.EnemyKilled);
			GD.Print($"Enemy killed!, remaining: {_enemiesCounter} enemies");
		}
		else
		{
			_enemiesCounter = 0;
			EmitSignal(SignalName.AllEnemiesKilled);
			var ui = GetTree().GetFirstNodeInGroup("UI") as INotificable;

			var tween = CreateTween();
			tween.TweenCallback(Callable.From(() => ui.PopNotification("¡Felicidades! has obtenido todas las skills y utilizado sus beneficios para bien")));
			tween.TweenCallback(Callable.From(() => ui.PopNotification("Es hora de continuar con el siguiente nivel."))).SetDelay(6f);

			var fade = ui as IFader;
			fade.FadeOut();
		}
	}
}
