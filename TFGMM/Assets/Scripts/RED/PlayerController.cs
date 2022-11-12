using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;


public class PlayerController : EntityBehaviour<IPhysicState>
{
    private PlayerMotor _playerMotor;
    private PlayerMov _playerMov;
    private float _horizontal;
    private float _vertical;

    private bool _hasControl = false;

    public void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerMov = GetComponent<PlayerMov>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        if (entity.HasControl)
        {
            _hasControl = true;
            GUI_Controller.Current.Show(true);
        }

        BoltLog.Warn("isMine: " + entity.HasControl);
        Init(entity.HasControl);
        _playerMotor.Init(entity.HasControl);
    }

    public void Init(bool isMine)
    {
        if (isMine)
        {
            FindObjectOfType<PlayerSetupController>().SceneCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_hasControl)
            PollKeys();
    }

    private void PollKeys()
    {
        _horizontal = _playerMov.GetJoystickMov().Horizontal;
        _vertical = _playerMov.GetJoystickMov().Vertical;
    }

    public override void SimulateController()
    {
        IPlayerCommandInput input = PlayerCommand.Create();
        input.Horizontal = _horizontal;
        input.Vertical = _vertical;

        entity.QueueInput(input);

        _playerMotor.ExecuteCommand(_horizontal, _vertical);
    }


    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            _playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            PlayerMotor.State motorState = new PlayerMotor.State();

            if (!entity.HasControl)
            {
                motorState = _playerMotor.ExecuteCommand(
                cmd.Input.Horizontal,
                cmd.Input.Vertical);
            }

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = motorState.rotation;
        }
    }
}