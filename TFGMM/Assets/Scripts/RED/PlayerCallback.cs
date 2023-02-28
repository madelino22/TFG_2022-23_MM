using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

public class PlayerCallback : EntityEventListener<IPlayerState>
{
    private PlayerMotor _playerMotor;
    private GUI_Controller _guiController;
    private MatchManager _matchManager;

    private int seg = 1;
    private float timer;

    private int blue = 0;
    private int red = 0;
    private int time = 2000;
    bool isDead = false;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _guiController = GetComponentInChildren<GUI_Controller>();
        _matchManager = GetComponentInChildren<MatchManager>();
        //_matchManager.UpdateUI(1, 1, 1);
        timer = 0;
        //BoltLog.Warn("A");
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
        if (BoltNetwork.IsClient)
        {
        }

    }

    public void Respawn()
    {
        

    }

    public void loseLife(bool redWasHit)
    {
        this._playerMotor.ActualLife -= 500;
        if (this._playerMotor.ActualLife <= 0)
        {

            _playerMotor.Respawn();

            if (entity.IsControllerOrOwner) //si soy yo el que se ha muerto
            {
                //RESPAWN
                //VA IGUAL SI ESTA DENTRO DEL IF O NO
 //SOLO SE ESTA LLAMANDO EN EL IsControllerOrOwner

                //ACTUALIZAR PUNTUACION
                PlayerDiedEvent evnt1 = PlayerDiedEvent.Create(GlobalTargets.OnlyServer);
                evnt1.isRed = redWasHit;
                evnt1.Send();
            }
            //RespawnEvent evento = RespawnEvent.Create(GlobalTargets.OnlyServer);
            //evento.id = gameObject.GetComponent<PlayerSetupController>().getId();
            //evento.Send();

            this._playerMotor.ActualLife = this._playerMotor.TotalLife;

            //VA IGUAL SI ESTA COMENTADO O NO
            //this._playerMotor.gameObject.transform.position = this._playerMotor.SpawnPos;
        }

        HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.Everyone);
        evnt.ActualLife = _playerMotor.ActualLife;
        evnt.TotalLife = _playerMotor.TotalLife;
        evnt.Send();
    }
    public override void OnEvent(HealthEvent evnt)
    {
        if (_guiController != null)
        {
            _guiController.UpdateLife(evnt.ActualLife, evnt.TotalLife);

            BoltLog.Warn("Menos Vida");
            Debug.Log("Menos Vida");
        }
    }

    //public override void OnEvent(MatchInfoEvent evnt)
    //{
    //    if (_matchManager != null)
    //    {
    //        _matchManager.UpdateUI(evnt.BlueScore, evnt.RedScore, evnt.Time);
    //        BoltLog.Warn("Menos Vida");
    //    }
    //}

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