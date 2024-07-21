using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class RageLevelInstuctions : WorldBehaviour
{
	private StatsComponent _playerStats;
	private ISkillComponent _playerSkills;

	public override void _Ready()
	{
		//Engine.TimeScale = 2f;

		_playerSkills = GetTree().GetFirstNodeInGroup("PlayerSkillsComp") as ISkillComponent;
		_playerStats = GetTree().GetFirstNodeInGroup("PlayerStats") as StatsComponent;

		var enemies = GetTree().GetNodesInGroup("EnemiesStats").Cast<StatsComponent>();

		foreach (var en in enemies)
		{
			en.Invincible = false;
			en.MaxHealthPoints = (byte)((GD.Randi() % 3) + 2);
			en.HitPoints = (byte)((GD.Randi() % 2) + 1);

			//buff the movement specs too
			var movement = en.GetParent().GetParent() as BatController;

			movement.Acceleration += GD.Randf() * 50f;
			movement.MaxSpeed += GD.Randf() * 40f;
			movement.Friction += GD.Randf() * 30f;
			movement.KnockBackSpeed += GD.Randf() * 60f;
		}
		base._Ready();
	}

	private protected override void GiveInstructions()
	{
		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Hola viajero!", 2))).SetDelay(4f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("En este nivel nos gustaría probar como funciona la música para generarte ira >:("))).SetDelay(4);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Esto debe venir con un gameplay de la talla de dicho enunciado."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Preparate para esta nueva etapa, hordas de enemigos se avecinan."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Y como si eso fuera poco, los enemigos han recibido un pequeño buff..."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Pero estás de suerte, toma esta vida extra y todas las sikills disponibles."))).SetDelay(6f);

		tween.Parallel().TweenCallback(Callable.From(() => _playerSkills.SetSkillBit(Skills.Sword | Skills.FlashTP | Skills.Dash, false))).SetDelay(6f);
		tween.Parallel().TweenMethod(Callable.From<byte>(healt => {
			_playerStats.MaxHealthPoints = healt;
			_playerStats.HealthPoints = healt;
		}), _playerStats.MaxHealthPoints, 20, 4f).SetDelay(6f);

		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Ahora asegurate de sobrevivir y destruir a todas esas molestias."))).SetDelay(2f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Suerte!", 2))).SetDelay(6f);
		tween.Parallel().TweenCallback(Callable.From(() => {
			GetNode<EnemySpawner>("EnemySpawner").StartSpawning(-1);
		})).SetDelay(6f);
		tween.Connect(Tween.SignalName.Finished, Callable.From(() => tween.Dispose()));
	}
}
