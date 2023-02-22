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
            /*if (Input.GetKeyDown(KeyCode.F))
            {
                _playerMotor.ActualLife += 200;
                Debug.Log(state.LifePoints);
                HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.OnlySelf);
                evnt.ActualLife = _playerMotor.ActualLife;
                evnt.TotalLife = _playerMotor.TotalLife;
                evnt.Send();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                _playerMotor.ActualLife -= 200;
                Debug.Log(state.LifePoints);
                HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.OnlySelf);
                evnt.ActualLife = _playerMotor.ActualLife;
                evnt.TotalLife = _playerMotor.TotalLife;
                evnt.Send();
            }*/
        }
        //HealthEvent evnt = HealthEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
        //evnt.ActualLife = _playerMotor.ActualLife;
        //evnt.TotalLife = _playerMotor.TotalLife;
        //evnt.Send();


        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    _playerMotor.BlueScore = _playerMotor.BlueScore + 1;
        //    MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        //    evnts.BlueScore = _playerMotor.BlueScore;
        //    evnts.RedScore = _playerMotor.RedScore;
        //    evnts.Time = _playerMotor.TimeMatch;
        //    evnts.Send();
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    _playerMotor.RedScore = _playerMotor.RedScore + 1;
        //    MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        //    evnts.BlueScore = _playerMotor.BlueScore;
        //    evnts.RedScore = _playerMotor.RedScore;
        //    evnts.Time = _playerMotor.TimeMatch;
        //    evnts.Send();
        //}
        //timer += Time.deltaTime;
        //if (seg < timer)
        //{
        //    _playerMotor.TimeMatch = _playerMotor.TimeMatch - 1;
        //    MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        //    evnts.BlueScore = _playerMotor.BlueScore;
        //    evnts.RedScore = _playerMotor.RedScore;
        //    evnts.Time = _playerMotor.TimeMatch;
        //    evnts.Send();
        //    seg++;
        //}
        //timer += Time.deltaTime;
        //if (seg < timer)
        //{
        //    time--;

        //    MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.Everyone);
        //    evnts.BlueScore = blue;
        //    evnts.RedScore = red;
        //    evnts.Time = time;
        //    evnts.Send();
        //    _matchManager.UpdateUI(evnts.BlueScore, evnts.RedScore, evnts.Time);
        //    seg++;
        //}

        //if (entity.IsOwner)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    {
        //        blue++;
        //        //MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.Everyone);
        //        //evnts.BlueScore = blue;
        //        //evnts.RedScore = red;
        //        //evnts.Time = time;
        //        //evnts.Send();
        //        _matchManager.UpdateUI(evnts.BlueScore, evnts.RedScore, evnts.Time);
        //    }
        //    if (Input.GetKeyDown(KeyCode.RightArrow))
        //    {
        //        red++;
        //        //MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.Everyone);
        //        //evnts.BlueScore = blue;
        //        //evnts.RedScore = red;
        //        //evnts.Time = time;
        //        //evnts.Send();
        //        _matchManager.UpdateUI(evnts.BlueScore, evnts.RedScore, evnts.Time);
        //    }

        //    timer += Time.deltaTime;
        //    if (seg < timer)
        //    {
        //        time--;

        //        MatchInfoEvent evnts = MatchInfoEvent.Create(entity, EntityTargets.Everyone);
        //        evnts.BlueScore = blue;
        //        evnts.RedScore = red;
        //        evnts.Time = time;
        //        evnts.Send();
        //        _matchManager.UpdateUI(evnts.BlueScore, evnts.RedScore, evnts.Time);

        //        seg++;
        //    }
        //}

    }

    //NO SE USA
    public void Respawn()
    {
        _playerMotor.Respawn();

    }

    public void loseLife(bool redWasHit)
    {
        this._playerMotor.ActualLife -= 500;
        if (this._playerMotor.ActualLife <= 0)
        {
            if (entity.IsControllerOrOwner)
            {
                Respawn();
                PlayerDiedEvent evnt1 = PlayerDiedEvent.Create(GlobalTargets.OnlyServer);
                evnt1.isRed = redWasHit;
                evnt1.Send();
            }
            this._playerMotor.ActualLife = this._playerMotor.TotalLife;
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