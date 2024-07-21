using Godot;
using Godot.Collections;

public interface IFlashTPSkillComponent
{
    bool CanFlash { get; set; }
}

public partial class FlashTPSkillComponent : Node, IFlashTPSkillComponent
{
    [Export] private RayCast2D flashMechanism;
    [Export] private PlayerMovementController playerMovement;

    public bool CanFlash { get; set; }

    private Area2D _targetFlash;
    private CanvasItem _tpSprite;
    private AnimationPlayer _animPlayer;
    private Vector2 _originPosition;
    private bool _backToOrigin;
    private bool _isCloseEnough;
    private bool _isValidDestination;
    private bool _isActive;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _targetFlash = flashMechanism.GetNode<Area2D>("Area2D");
        _tpSprite = _targetFlash.GetNode<CanvasItem>("Sprite2D");
        _animPlayer = _tpSprite.GetNode<AnimationPlayer>("AnimationPlayer");
        _tpSprite.Visible = false;
    }

    public override void _Notification(int what)
    {
        // switch (what)
        // {
        //     case NotificationCrash:
        //     break;
        // }
    }

    private void LockPlayerMovement()
    {
        playerMovement.LockMovement();
    }

    private void FreePlayerMovement()
    {
        playerMovement.FreeMovement();
    }

    public async override void _Input(InputEvent @event)
    {
        if (CanFlash && @event is InputEventKey eventKey && !eventKey.IsEcho() && eventKey.Pressed)
        {
            switch (eventKey.Keycode)
            {
                case Key.F when _backToOrigin:
                    playerMovement.GlobalPosition = _originPosition;
                    _backToOrigin = false;
                    break;

                case Key.F:
                    if (_isActive)
                    {
                        FreePlayerMovement();
                        TurnOnOffMechanism(false);
                        _isActive = false;
                        _animPlayer.Stop();
                    }
                    else
                    {
                        _isValidDestination = true;
                        LockPlayerMovement();
                        TurnOnOffMechanism(true);
                        _isActive = true;
                    }
                    break;

                case Key.Up when _isActive:
                    TurnOnOffMechanism(false);
                    flashMechanism.RotationDegrees = 180;

                    await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                    TurnOnOffMechanism(true);
                    break;

                case Key.Down when _isActive:
                    TurnOnOffMechanism(false);
                    flashMechanism.RotationDegrees = 0;

                    await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                    TurnOnOffMechanism(true);
                    break;

                case Key.Right when _isActive:
                    TurnOnOffMechanism(false);
                    flashMechanism.RotationDegrees = -90;

                    await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                    TurnOnOffMechanism(true);
                    break;

                case Key.Left when _isActive:
                    TurnOnOffMechanism(false);
                    flashMechanism.RotationDegrees = 90;

                    await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                    TurnOnOffMechanism(true);
                    break;

                case Key.S when _isActive:

                    if (_isCloseEnough && _isValidDestination)
                    {
                        TurnOnOffMechanism(false);
                        FreePlayerMovement();
                        _originPosition = playerMovement.GlobalPosition;
                        playerMovement.GlobalTranslate(_targetFlash.GlobalPosition - playerMovement.GlobalPosition);
                        _backToOrigin = true;
                        _isActive = false;
                    }
                    // else
                    // {
                    //     GD.Print($"Is not posible to teleport. {_isCloseEnough} {_isValidDestination}");
                    // }
                    break;
            }

        }
    }

    private void ValidateTP()
    {
        _isCloseEnough = flashMechanism.IsColliding();

        if (_targetFlash.GetOverlappingBodies().Count == 0)
        {
            _isValidDestination = true;
        }
        else
        {
            _isValidDestination = false;
        }
    }

    private void TurnOnOffMechanism(bool state)
    {
        //flashMechanism.SetDeferred(RayCast2D.PropertyName.Enabled, state);
        //flashMechanism.Enabled = true;
        //_targetFlash.SetDeferred(Area2D.PropertyName.Monitoring, state);
        _tpSprite.Visible = state;


        if (state)
        {
            CallDeferred(MethodName.ValidateTP);
            CallDeferred(MethodName.UpdateVisibleTPAnimation);
        }
        else
        {
            _animPlayer.Stop();
        }
    }

    private async void UpdateVisibleTPAnimation()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        if (_isCloseEnough && _isValidDestination)
        {
            _animPlayer.Play("blink_normal");
        }
        else
        {
            _animPlayer.Play("blink_wrong");
        }
    }
}
