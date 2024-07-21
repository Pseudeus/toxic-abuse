using Godot;

public partial class BlinkComponent : Node
{
	[Export] private CanvasItem blinkingNode;
	[Export] private float blinkingTotalTime = 1f;
	[Export] private byte blinkingTimes = 4;

	private ShaderMaterial _blinkShader;
	private bool _active = true;

    public override void _Ready()
    {
		_blinkShader = blinkingNode.Material as ShaderMaterial;
    }

	public void StartBlinking()
	{
		var tween = CreateTween(); 

		tween.SetLoops(blinkingTimes);
		tween.TweenCallback(Callable.From(() => {
			_blinkShader.SetShaderParameter("active", _active);
			_active = !_active;
		})).SetDelay(blinkingTotalTime / blinkingTimes / 2);
		
		tween.TweenCallback(Callable.From(() =>{
			_blinkShader.SetShaderParameter("active", _active);
			_active = !_active;
		})).SetDelay(blinkingTotalTime / blinkingTimes / 2);
	}
}
