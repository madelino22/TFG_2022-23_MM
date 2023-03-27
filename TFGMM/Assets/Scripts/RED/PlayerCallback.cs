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

    // SOLO SE EJECUTA EN EL SERVER (OnlyServer)
    public void loseLife(bool redWasHit, int shooterName, int wasHitName)
    {
        this._playerMotor.ActualLife -= 500;

        //MANDAR EVENTO DE QUE SE HA HECHO DANYO------------------
        takeDamageEvent evn = takeDamageEvent.Create(GlobalTargets.OnlyServer);
        evn.damagedBy = shooterName;
        evn.nameDamaged = wasHitName;
        evn.Send();

        if (this._playerMotor.ActualLife <= 0)
        {
            //RESPAWN ==> MOVER JUGADOR
            _playerMotor.Respawn(shooterName); //REVISAR ESTA MIERDA

            //ACTUALIZAR PUNTUACION
            PlayerDiedEvent evnt1 = PlayerDiedEvent.Create(GlobalTargets.OnlyServer);
            evnt1.isRed = redWasHit;
            evnt1.Send();

            //RESET VIDA
            this._playerMotor.ActualLife = this._playerMotor.TotalLife;
        }

        // ACTUALIZAR BARRA VIDA
        HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.Everyone);
        evnt.ActualLife = _playerMotor.ActualLife;
        evnt.TotalLife = _playerMotor.TotalLife;
        evnt.Send();
    }

    public void addLife(bool redWasHit, int healerName, int wasHitName)
    {
        this._playerMotor.ActualLife += 250;
        if (this._playerMotor.ActualLife > 2500)
            this._playerMotor.ActualLife = 2500;

        else
        {
            //MANDAR EVENTO DE QUE SE HA HECHO DANYO------------------
            healPlayerEvent evn = healPlayerEvent.Create(GlobalTargets.OnlyServer);
            evn.healedBy = healerName;
            evn.nameHealed = wasHitName;
            evn.Send();

            // ACTUALIZAR BARRA VIDA
            HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.Everyone);
            evnt.ActualLife = _playerMotor.ActualLife;
            evnt.TotalLife = _playerMotor.TotalLife;
            evnt.Send();
        }
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