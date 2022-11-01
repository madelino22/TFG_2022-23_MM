using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

public class PlayerCallback : EntityEventListener<IPlayerState>
{
    private PlayerMotor _playerMotor;
    private GUI_Controller _guiController;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _guiController = GetComponentInChildren<GUI_Controller>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);

        if (entity.IsOwner)
        {
            state.LifePoints = _playerMotor.TotalLife;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            state.LifePoints += 10;

            HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
            evnt.ActualLife = state.LifePoints;
            evnt.TotalLife = _playerMotor.TotalLife;
            evnt.Send();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            state.LifePoints -= 10;

            HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
            evnt.ActualLife = state.LifePoints;
            evnt.TotalLife = _playerMotor.TotalLife;
            evnt.Send();
        }
    }

    public override void OnEvent(HealthEvent evnt)
    {
        if(_guiController!=null)
        {
            _guiController.UpdateLife(evnt.ActualLife, evnt.TotalLife);
            BoltLog.Warn("Menos Vida");
        }
    }

    public void UpdatePlayerLife()
    {
        if (entity.HasControl)
        {
            //if (state.LifePoints <= 0)
            //    BoltNetwork.Destroy(this.gameObject);
            GUI_Controller.Current.UpdateLife(state.LifePoints, _playerMotor.TotalLife);
        }
    }
}