using Godot;
using System;

public partial class EnemiesContainer : Node2D
{
	[Export] private int enemiesLimit = 100;

	private int _enemiesCurrentNumber;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node n in GetChildren())
		{
			n.QueueFree();
		}

		_enemiesCurrentNumber = GetChildCount();
	}

	public void AttachEnemy(ref BatController enemy, out bool full, out float deltaTime, float upLimit, int enemiesInQueue, float botLimit = 0.2f)
	{
		_enemiesCurrentNumber = GetChildCount() + enemiesInQueue;
		deltaTime = Mathf.Lerp(botLimit, upLimit, (float)_enemiesCurrentNumber / (float)enemiesLimit) - Mathf.Lerp(0f, upLimit - botLimit, (float)enemiesInQueue / ((float)_enemiesCurrentNumber + 1));

		GD.Print(_enemiesCurrentNumber);
		GD.Print("Time reduction: ", Mathf.Lerp(0f, upLimit - botLimit, (float)enemiesInQueue / ((float)_enemiesCurrentNumber + 1)));

		if (_enemiesCurrentNumber < enemiesLimit)
		{
			AddChild(enemy);
			enemy.Owner = this;
			full = false;
		}
		else 
		{
			full = true;
			GD.Print("Enemies Limit Reached");
			enemy.QueueFree();
		}
	}
}
