using Godot;
using System;

public partial class SadLevelInstructions : WorldBehaviour
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private protected override void GiveInstructions()
	{
		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Hola viajero!", 2))).SetDelay(4f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("En este nivel nos gustaría probar como funciona la música para traerte tristeza :("))).SetDelay(4);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Como ya habrás notado, has perdido todas tus skills."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Busca los 10 signos en el suelo y pasa sobre ellos en orden para poder continuar."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Estos signos están distribuidos en todo el mapa."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Están en un idioma desconocido, pero debería ser sencillo una vez te familiarices."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Suerte!", 2))).SetDelay(6f);
		tween.Connect(Tween.SignalName.Finished, Callable.From(() => tween.Dispose()));
	}
}
