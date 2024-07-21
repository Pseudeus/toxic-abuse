using Godot;

[GlobalClass]
public partial class Stats : Resource
{
    [Signal] public delegate void NoHealthEventHandler();
	[Signal] public delegate void HealthChangedEventHandler(byte health);
	[Signal] public delegate void MaxHealthChangedEventHandler(byte maxHealth);

    [Export] public byte MaxHealthPoints
        {
            get => _maxHealthPoints;
            set
            {
                _maxHealthPoints = value;
                EmitSignal(SignalName.MaxHealthChanged, _maxHealthPoints);
            }
        }
	[Export] public byte HitPoints { get; set; } = 1;
	[Export] public bool Invincible { get; set; }

    public byte HealthPoints
	{
		get => _healthPoints;
		set 
		{
			_healthPoints = value;
			EmitSignal(SignalName.HealthChanged, _healthPoints);
		}
	}

    private byte _healthPoints;
    private byte _maxHealthPoints = 1;

    public Stats()
    {
        // _healthPoints = _maxHealthPoints;
        // GD.Print("Health: ", _healthPoints);

        // Connect(Stats.SignalName.HealthChanged, Callable.From<byte>(h => { GD.Print($"Health Changed to: { h }!"); }));
    }

    public override void _SetupLocalToScene()
    {
        HealthPoints = _maxHealthPoints;
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
