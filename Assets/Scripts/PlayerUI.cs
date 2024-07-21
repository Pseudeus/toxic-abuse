using Godot;
using System;

public interface INotificable
{
	void PopNotification(string msg, float showingTime = 4);
}
public interface IFader
{
	void FadeIn(bool keepVignette = false, float instensity = 0.4f);
	void FadeOut();
}

public partial class PlayerUI : CanvasLayer, INotificable, IFader
{
	[Export] private byte heartXSize = 15;

	private StatsComponent _playerStats;
	private Control _maxHeartsTextureRect;
	private Control _currentHeartsTextureRect;
	private Label _notify;
	private PanelContainer _panelCont;
	private Tween _tween;
	private AudioStreamPlayer _audioPlayer;
	private ColorRect _vignette;
	private ColorRect _solidColor;
	private ColorRect _blurs;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_notify = GetNode<Label>("Control/Panel/MarginContainer/Label");
		_panelCont = GetNode<PanelContainer>("Control/Panel");

		_maxHeartsTextureRect = GetNode<Control>("Control/MaxHeartsCont");
		_currentHeartsTextureRect = GetNode<Control>("Control/CurrentHeartsCont");

		_audioPlayer = _panelCont.GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		_vignette = GetNode<ColorRect>("Control/ColorRect");
		_solidColor = GetNode<ColorRect>("Control/ColorRect2");
		_blurs = GetNode<ColorRect>("Control/ColorRect3");
	}

	public void OnPlayerReady()
	{
		_playerStats = GetTree().GetFirstNodeInGroup("PlayerStats") as StatsComponent;
		_playerStats.Connect(StatsComponent.SignalName.HealthChanged, Callable.From<byte>(h => OnPlayerStatsCurrentHealthChanged(h)));
		_playerStats.Connect(StatsComponent.SignalName.NoHealth, Callable.From(OnPlayerStatsNoHealth));
		_playerStats.MaxHealthChanged += OnPlayerStatsMaxHealthChanged;

		Vector2 tSize = HealthToHearts(_playerStats.MaxHealthPoints);
		_maxHeartsTextureRect.Size = tSize;
		_currentHeartsTextureRect.Size = tSize;
		_currentHeartsTextureRect.Visible = true;
	}

	public void DisconnectPlayer()
	{
		_playerStats.NoHealth -= OnPlayerStatsNoHealth;
		_playerStats.HealthChanged -= OnPlayerStatsCurrentHealthChanged;
		_playerStats.MaxHealthChanged -= OnPlayerStatsMaxHealthChanged;
	}

	public void OnPlayerStatsCurrentHealthChanged(byte health)
	{
		_currentHeartsTextureRect.Size = HealthToHearts(health);
	}

	public void OnPlayerStatsMaxHealthChanged(byte maxHealth)
	{
		_maxHeartsTextureRect.Size = HealthToHearts(maxHealth);
	}

	public void OnPlayerStatsNoHealth()
	{
		_currentHeartsTextureRect.Visible = false;
	}

	private Vector2 HealthToHearts(byte health)
	{
		return new Vector2(20 + ((health -1) * heartXSize), 16);
	}

    public override void _Notification(int what)
    {
        if (IsInstanceValid(_tween) && what == NotificationExitTree)
		{
			_tween.Kill();
			_tween.Dispose();
			
			_audioPlayer.Stop();
			_panelCont.Visible = false;
		}
    }

    public void PopNotification(string msg, float showingTime = 4)
	{
		if (IsInstanceValid(_tween) &&  _tween.IsRunning())
		{
			_tween.Kill();
			_tween.Dispose();
		}

		_notify.Text = msg;
		_panelCont.Size = Vector2.One * 19;
		_panelCont.Visible = true;

		ushort panelXsize = (ushort)(_panelCont.Size.X + 5);
		ushort screenXSize = (ushort)(GetTree().Root.Size.X / ProjectSettings.GetSetting("display/window/stretch/scale").As<byte>());

		Vector2 start = new (screenXSize, 155);
		Vector2 finish = new (screenXSize - panelXsize, 155);

		_panelCont.Position = start;

		_audioPlayer.Play();

		_tween = CreateTween();
		_tween.TweenMethod(Callable.From<Vector2>(pos => _panelCont.Position = pos), start, finish, 1f).SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.Out);
		_tween.TweenMethod(Callable.From<Vector2>(pos => _panelCont.Position = pos), finish, start, 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In).SetDelay(showingTime);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(() => { _notify.Text = string.Empty;	_panelCont.Visible = false; _tween.Dispose(); }), 4);
	}

	public void FadeOut()
	{
		GetTree().CreateTimer(2f).Timeout += () => GetTree().Paused = true;

		_solidColor.Color = Color.Color8(0, 0, 0, 0);
		var mat = _vignette.Material as ShaderMaterial;
		mat.SetShaderParameter("alpha", 0);

		var blur = _blurs.Material as ShaderMaterial;
		blur.SetShaderParameter("radial_blur_lod", 0f);
		blur.SetShaderParameter("blur_lod", 0f);
		blur.SetShaderParameter("radial_iterations", 5);

		_solidColor.Visible = true;
		_vignette.Visible = true;
		_blurs.Visible = true;

		var tween = CreateTween();
		tween.SetParallel();
		tween.TweenMethod(Callable.From<float>(rbl => blur.SetShaderParameter("radial_blur_lod", rbl)), 0f, 0.08f, 2f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.In);
		tween.TweenMethod(Callable.From<float>(bl => blur.SetShaderParameter("blur_lod", bl)), 0, 5f, 2f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenMethod(Callable.From<byte>(radit => blur.SetShaderParameter("radial_iterations", radit)), 5, 10, 2).SetTrans(Tween.TransitionType.Linear);
		tween.TweenMethod(Callable.From<float>(value => mat.SetShaderParameter("alpha", value)), 0f, 1f, 4f).SetEase(Tween.EaseType.In);
		tween.TweenMethod(Callable.From<byte>(value => _solidColor.Color = Color.Color8(0, 0, 0, value)), 0, 255, 4f).SetDelay(2f).SetEase(Tween.EaseType.In);

		tween.Connect(Tween.SignalName.Finished, Callable.From(() => {
			GetParent<Main>().ReturnToHub();
			//FadeIn();
		}));
	}

	public void FadeIn(bool keepVignette = false, float intensity = 0.4f)
	{
		GetTree().Paused = false;

		_solidColor.Color = Color.Color8(0, 0, 0, 255);
		var mat = _vignette.Material as ShaderMaterial;
		mat.SetShaderParameter("alpha", 1);

		_solidColor.Visible = true;
		_vignette.Visible = true;
		_blurs.Visible = false;

		var tween = CreateTween();
		tween.SetParallel();
		
		if (keepVignette) tween.TweenMethod(Callable.From<float>(value => mat.SetShaderParameter("alpha", value)), 1f, intensity, 4f).SetEase(Tween.EaseType.In);
		else tween.TweenMethod(Callable.From<float>(value => mat.SetShaderParameter("alpha", value)), 1f, 0f, 4f).SetEase(Tween.EaseType.In);
		

		tween.TweenMethod(Callable.From<byte>(value => _solidColor.Color = Color.Color8(0, 0, 0, value)), 255, 0, 4f).SetDelay(2f).SetEase(Tween.EaseType.In);

		Connect(Node.SignalName.TreeExiting, Callable.From(() => tween.Kill()), 4);

		tween.Connect(Tween.SignalName.Finished, Callable.From(() => {
			_solidColor.Visible = false;
			_vignette.Visible = keepVignette;

			if (!keepVignette)
			{
				mat.SetShaderParameter("alpha", 0.6f);
			}
		}));
	}

	// #region Debug

	// public override void _Input(InputEvent @event) {
	// 	if (@event is InputEventKey eventKey && eventKey.Pressed && !eventKey.IsEcho() && eventKey.Keycode == Key.Q)
	// 	{
	// 		FadeOut();
	// 	}
	// }

	// #endregion
}
