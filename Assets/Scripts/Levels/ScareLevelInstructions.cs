using Godot;
using System;

public partial class ScareLevelInstructions : WorldBehaviour
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play(4f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private protected override void GiveInstructions()
	{
		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Hola viajero!", 2))).SetDelay(4f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("En este nivel nos gustaría probar como generar miedo a través de la música :0"))).SetDelay(4);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Como ya habrás notado, has perdido todas tus skills."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Busca los 10 signos en el suelo y pasa sobre ellos en orden para poder continuar."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Estos signos están distribuidos en todo el mapa."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Están en un idioma desconocido, pero debería ser sencillo una vez te familiarices."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Pero espera!"))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Hay muchos enemigos en el mapa."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Por suerte ellos se asustan por la luz que emites."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Pero ten cuidado, si tu luz se apaga ellos no dudarán en atacarte."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Hay recargas para el poder de tu luz distribuidas en el mapa."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("Solo no abuses, que la luz cuenta con desgaste por recarga."))).SetDelay(6f);
		tween.TweenCallback(Callable.From(() => _ui.PopNotification("¡Suerte!", 2))).SetDelay(6f);
		tween.Connect(Tween.SignalName.Finished, Callable.From(() => tween.Dispose()));
	}
}
