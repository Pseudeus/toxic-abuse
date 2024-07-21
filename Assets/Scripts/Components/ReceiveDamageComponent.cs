using Godot;
using Godot.Collections;
using System;

public partial class ReceiveDamageComponent : Node
{
	[Export] private CharacterType cType;
	[Export] private PackedScene hurtSound;


	public void ProcessAudioPetition()
	{
		var child = GetChildren();

		if (child.Count == 0) 
		{
			CreateAudioStreamPlayerInstance();	
		}
		else
		{
			for (byte i = 0; i < child.Count; i++)
			{
				if (child[i] is AudioStreamPlayer sp)
				{
					if (sp.Playing)
					{
						if (i + 1 < child.Count) continue;

						CreateAudioStreamPlayerInstance();
						break;
					}
					else
					{
						sp.Play();
					}
				}
			}
		}
	}

	private void CreateAudioStreamPlayerInstance()
	{
		var sound = hurtSound.Instantiate<AudioStreamPlayer>();
		AddChild(sound);
		sound.Owner = this;
		sound.Play();
	}
}

public enum CharacterType
{
	Player,
	Enemy
}
