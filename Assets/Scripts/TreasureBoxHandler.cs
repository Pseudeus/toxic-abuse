using Godot;
using System;
using System.Collections.Generic;

public partial class TreasureBoxHandler : StaticBody2D
{
	[Export] private Item itemInside;
	[Export] private Skills skill;

	private bool _openActionAvalaiable = false;
	private bool _opened = false;
	private ISkillComponent _playerSkillsComp;
	private Action<StringName, float, bool> _openChestAnimAction;
	private INotificable _playerNotification;
	public static Dictionary<Skills, string> _notificationsPerSkill = new ();

	public override void _Ready()
	{
		var area = GetNode<Area2D>("OpeningArea");
		area.Connect(Area2D.SignalName.BodyEntered, Callable.From<Node2D>(n => OnPlayerDetectionAreaBodyEntered(n)));
		area.Connect(Area2D.SignalName.BodyExited, Callable.From<Node2D>(n => OnPlayerDetectionAreaBodyExited(n)));

		var chest = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (_notificationsPerSkill.Count == 0)
		{
			FillDictionary();
		}

		_playerSkillsComp = GetTree().GetFirstNodeInGroup("PlayerSkillsComp") as ISkillComponent;
		_playerNotification = GetTree().GetFirstNodeInGroup("UI") as INotificable;

		_openChestAnimAction = chest.Play;
		chest.Connect(AnimatedSprite2D.SignalName.AnimationFinished, Callable.From(() => OnAnimatedSprite2DAnimationFinished()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_openActionAvalaiable && !_opened && Input.IsActionJustPressed("open_chest"))
		{
			_opened = true;
			_openChestAnimAction("open", 1, false);
		}
	}

	private static void FillDictionary()
	{
		_notificationsPerSkill.Add(Skills.Sword, "You gotted the sword skill, use space key to attack.");
		_notificationsPerSkill.Add(Skills.FlashTP, "You gotted the flashTP skill, use the 'F' key to activate and the 'S' key to teleport.");
		_notificationsPerSkill.Add(Skills.Dash, "You gotted the dash skill, use the 'D' key to dash.");
	}

	public void OnAnimatedSprite2DAnimationFinished()
	{
		_playerNotification.PopNotification(_notificationsPerSkill[skill]);
		_playerSkillsComp.SetSkillBit(skill);
	}

	public void OnPlayerDetectionAreaBodyEntered(Node2D body)
	{
		if (body is PlayerMovementController player)
		{
			_openActionAvalaiable = true;
			if (!player.FirstInteraction)
			{
				player.FirstInteraction = true;
				_playerNotification.PopNotification("Press 'E' to open the chest.");
			}
		}
	}

	public void OnPlayerDetectionAreaBodyExited(Node2D body)
	{
		if (body is PlayerMovementController)
		{
			_openActionAvalaiable = false;
		}
	}
}

public enum Item
{
	FlashTP,
	Sword,
	Coin,
	Key,
	Dash
}