using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Security.Cryptography;

public interface ISkillComponent 
{
	Skills PlayerSkills { get; set; }

	void SetSkillBit(Skills skill, bool countSkills = true);
}

public partial class SkillsComponent : Node, ISkillComponent
{
	[Signal] public delegate void SkillsUpdatedEventHandler();

	[Export] public Skills PlayerSkills { get { return _skills; } set  { _skills = value; EmitSignal(SignalName.SkillsUpdated); } }

	[Export] private PlayerMovementController _player;
	[Export] private NodePath _flashSkillNP;

	private Skills _skills;
	private IFlashTPSkillComponent _flashSkill;
	private INotificable _ui;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ui = GetTree().GetFirstNodeInGroup("UI") as INotificable;
		_flashSkill = GetNode<IFlashTPSkillComponent>(_flashSkillNP);
		SetSkillBit(PlayerSkills);
	}

	public void SetSkillBit(Skills skill, bool countSkills = true)
	{
		if (skill == 0b0) PlayerSkills &= skill;
		else PlayerSkills |= skill;
		UpdateSkills();

		if (PlayerSkills == (Skills)7 && countSkills)
		{
			var eStats = GetTree().GetNodesInGroup("EnemiesStats").Cast<StatsComponent>();

			var tween = CreateTween();
			tween.TweenCallback(Callable.From(() => _ui.PopNotification("!Bien hecho¡, has conseguido todas las skills"))).SetDelay(6f);
			tween.TweenCallback(Callable.From(() => _ui.PopNotification($"Ahora puedes derrotar al total de {eStats.Count()} enemigos distribuidos en el mapa."))).SetDelay(6f);


			foreach (var es in eStats)
			{
				es.Invincible = false;
			}
		}
		// 0000
		// 0001

		// 0000
		// 0010

		// 0000
		// 0100
	}

	private void UpdateSkills()
	{
		if ((PlayerSkills & 0b0) == 0b0)
		{
			// GD.Print("None");
			_player.CanAttack = false;
			_player.CanDash = false;
			_flashSkill.CanFlash = false;
		}

		if ((PlayerSkills & Skills.Sword) == Skills.Sword)
		{
			// GD.Print("Sword");
			_player.CanAttack = true;
		}

		if ((PlayerSkills & Skills.FlashTP) == Skills.FlashTP)
		{
			// GD.Print("FlashTP");
			_flashSkill.CanFlash = true;
		}

		if ((PlayerSkills & Skills.Dash) == Skills.Dash)
		{
			// GD.Print("Dash");
			_player.CanDash = true;
		}
	}
}

[Flags]
public enum Skills : byte
{
	Sword = 	0b1,
	FlashTP = 	0b1 << 1,
	Dash =  	0b1 << 2
}	
