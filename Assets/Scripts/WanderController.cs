using Godot;
using static Godot.GD;

public partial class WanderController : Marker2D
{
	[Signal] public delegate void PositionChangedEventHandler(Vector2 newPosition);

	[Export] private byte wanderOffset = 32;

	private Vector2 _startPosition;
	private Vector2 _wanderPositionOffset;
	private Timer _timer;
	//private Random _random = new ();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startPosition = GlobalPosition;
		_timer = GetNode<Timer>("Timer");

		_timer.Connect(Timer.SignalName.Timeout, Callable.From(SetRandomPositionOffset));
	}

	public void StartTimer(byte duration)
	{
		_timer.Start(duration);
	}

	public void StopTimer()
	{
		_timer.Stop();
	}

	private void SetRandomPositionOffset()
	{
		_wanderPositionOffset = new Vector2I(RandRange(-wanderOffset, wanderOffset), RandRange(-wanderOffset, wanderOffset));
		EmitSignal(SignalName.PositionChanged, _startPosition + _wanderPositionOffset);
	}
}
