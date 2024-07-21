using Godot;
using System;

//This class generates the behaviour of each selectable item in the initial hub, when clicked, the item calls the main method LoadScene giving the child index as parameter
public partial class HubItem : Panel
{
	//public Action<CanvasLayer> RemoveUs;

	private Tween _tween;
	private Func<bool> ValidateTween;

	public HubItem()
	{
		Connect(Control.SignalName.MouseEntered, Callable.From(OnMouseEntered));
		Connect(Control.SignalName.MouseExited, Callable.From(OnMouseExited));
		Connect(Control.SignalName.GuiInput, Callable.From<InputEvent>(e => OnGuiInput(e)));

		ValidateTween = () => { return IsInstanceValid(_tween) && _tween.IsValid() && _tween.IsRunning(); };
	}
	
	private void OnMouseEntered()
	{
		if (ValidateTween()) _tween.Kill();

		_tween = CreateTween();
		_tween.TweenMethod(Callable.From<float>(scale => Scale = Vector2.One * scale), 1f, 1.05f, 0.1f);
	}

	private void OnMouseExited()
	{
		if (ValidateTween()) _tween.Kill();

		_tween = CreateTween();
		_tween.TweenMethod(Callable.From<float>(scale => Scale = Vector2.One * scale), 1.05f, 1f, 0.1f);
	}

	private void OnGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && !mouseButton.IsEcho() && mouseButton.ButtonIndex == MouseButton.Left)
		{
			GetTree().Root.GetChild<Main>(0).LoadScene((byte)GetIndex());
			//RemoveUs(this);
		}
	}
}
