using Godot;
using System;
using System.Collections.Generic;

//This class controls everything that's present in game, if you create a new level set up it in here
//Be aware!, if you change the project route of any of the preloaded scenes, change it here too, otherwise it would give an error
public partial class Main : Node
{
	private readonly List<PackedScene> _packedScenes;
	private readonly Action _deleteCurrentSceneLevel;
	private byte _sceneIndex;
	private Node _instantiatedLevel;

	private CanvasLayer _hubUI;
	private PlayerUI _playerUI;
	private Node _scareSpecialNode;

	public Main()
	{
		_packedScenes = new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Assets/Scenes/world.tscn"),
			GD.Load<PackedScene>("res://Assets/Scenes/sad_level.tscn"),
			GD.Load<PackedScene>("res://Assets/Scenes/scare_level.tscn"),
			GD.Load<PackedScene>("res://Assets/Scenes/rage_level.tscn")
        };


		_hubUI = GD.Load<PackedScene>("res://Assets/Scenes/UI/hub.tscn").Instantiate<CanvasLayer>();
		_playerUI = GD.Load<PackedScene>("res://Assets/Scenes/UI/player_ui.tscn").Instantiate<PlayerUI>();

		AddChild(_hubUI);
		_hubUI.Owner = this;

		_deleteCurrentSceneLevel = () => { 
			if (IsInstanceValid(_instantiatedLevel))
			{
				_instantiatedLevel.Free();
			}
		};
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && !eventKey.IsEcho())
		{
			switch (eventKey.Keycode)
			{
				case Key.Escape:
					ReturnToHub();
					break;
			}
		}
    }

	public void ReturnToHub()
	{
		if (!_hubUI.IsInsideTree())
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			_deleteCurrentSceneLevel();
			RemoveChild(_playerUI);
			AddChild(_hubUI);
			_hubUI.Owner = this;
		}
	}

	public void LoadScene(byte index)
	{
		if (_packedScenes.Count <= index) return;

		Input.MouseMode = Input.MouseModeEnum.Hidden;

		_deleteCurrentSceneLevel();

		if (_hubUI.IsInsideTree()) RemoveChild(_hubUI);

		//Setting up special initial UI settings for each scene, remember that all the UI is never deleted, just removed from the game tree
		if (!_playerUI.IsInsideTree())
		{
			AddChild(_playerUI);
			_playerUI.Owner = this;

			switch (index)
			{
				case 0:
					_playerUI.GetNode<Control>("Control/FuelBar").Hide();
					_playerUI.FadeIn();
					break;

				case 1:
					_playerUI.GetNode<Control>("Control/FuelBar").Hide();
					_playerUI.FadeIn(true);
					break;
				
				case 2:
					_playerUI.GetNode<Control>("Control/FuelBar").Show();
					_playerUI.FadeIn(true, 1.3f);
					break;
				
				case 3:
					_playerUI.GetNode<Control>("Control/FuelBar").Hide();
					_playerUI.FadeIn();
					break;

				default:
					//if (_scareSpecialNode.IsInsideTree()) _playerUI.RemoveChild(_scareSpecialNode);
					_playerUI.FadeIn();
					break;
			}
		}

		_instantiatedLevel = _packedScenes[index].Instantiate<Node>();
		AddChild(_instantiatedLevel);
		_instantiatedLevel.Owner = this;
	}
}
