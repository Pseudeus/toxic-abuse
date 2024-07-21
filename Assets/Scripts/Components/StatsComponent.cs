using Godot;
using System;

public partial class StatsComponent : Node
{
	[Signal] public delegate void NoHealthEventHandler();
	[Signal] public delegate void HealthChangedEventHandler(byte health);
	[Signal] public delegate void MaxHealthChangedEventHandler(byte maxHealth);

	[Export] private byte maxHealthPoints = 1;
	[Export] private byte hitPoints = 1;
	[Export] public bool Invincible { get; set; }

	private byte _healthPoints;
	public byte HitPoints 
	{
		get => hitPoints;
		set => hitPoints = value;
	}
	public byte HealthPoints
	{
		get => _healthPoints;
		set 
		{
			_healthPoints = value;
			EmitSignal(SignalName.HealthChanged, _healthPoints);
		}
	}

	public byte MaxHealthPoints
	{
		get => maxHealthPoints;
		set
		{
			maxHealthPoints = value;
			EmitSignal(SignalName.MaxHealthChanged, maxHealthPoints);
		}
	}

    public override void _Ready()
    {
        _healthPoints = maxHealthPoints;
    }

    public void ProcessHit(byte damage)
	{
		if (Invincible) return;

		if (_healthPoints > 0 && _healthPoints - damage > 0) HealthPoints -= damage;
        else 
        {
            _healthPoints = 0;
            EmitSignal(SignalName.HealthChanged, _healthPoints); // Is this necesary?
            EmitSignal(SignalName.NoHealth);
        }
	}
}
