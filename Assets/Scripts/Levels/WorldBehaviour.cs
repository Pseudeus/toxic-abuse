using Godot;
using System;

public partial class WorldBehaviour : Node
{
	[Export] private float instructionsShowingTime = 2f;

	private protected INotificable _ui;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ui = GetTree().GetFirstNodeInGroup("UI") as INotificable;
		GiveInstructions();
	}

	//Base method for initial instructions in any level, this class should be used as base class for every level, calling the base _Ready and overriding the GiveInstructionsMethod
	private protected virtual void GiveInstructions()
	{
		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Hola viajero!", instructionsShowingTime))).SetDelay(2f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("En este nivel nos gustaría probar como funciona la música para traerte alegría :)"))).SetDelay(instructionsShowingTime + 2f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Nos gustaría seguir conversando, pero ahora no cuentas con skill alguna."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Busca y encuentra los 3 cofres que contienen estas muy útiles skills."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Puedes moverte utilizando las flechas del teclado."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Pero ten cuidado!, siempre ten un ojo en tu estado de vida actual."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Suerte!", instructionsShowingTime))).SetDelay(6f);
		tween.Connect(Tween.SignalName.Finished, Callable.From(() => tween.Dispose()));
	}
}
