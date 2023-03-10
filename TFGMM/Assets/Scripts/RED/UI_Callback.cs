using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

public class UI_Callback : GlobalEventListener
{
    [SerializeField]
    private MatchManager _matchManager;

    private int seg = 1;
    private float timer;

    private int blue = 0;
    private int red = 0;

    const int START_TIME = 15; //Segundos de una partida
    private int time = START_TIME; //seconds

    // Start is called before the first frame update
    private void Awake()
    {
        //BoltLog.Warn("A");
        _matchManager.UpdateUI(1, 1, 1);
        timer = 0;
    }
   
    public override void OnEvent(MatchInfoEvent evnt)// 
    {
        blue = evnt.BlueScore;
        red = evnt.RedScore;
        _matchManager.UpdateUI(evnt.BlueScore, evnt.RedScore, evnt.Time);
    }

    public override void OnEvent(PlayerDiedEvent evnt)
    {
        if (evnt.isRed) //el que ha muerto es rojo
        {
            blue+=1;

            MatchInfoEvent event2 = MatchInfoEvent.Create(GlobalTargets.AllClients);
            event2.BlueScore = blue;
            event2.RedScore = red;
            event2.Time = time;
            event2.Send();
            _matchManager.UpdateUI(blue, red, time);
        }
        else// if (!evnt.isRed)
        {
            red+=1;

            MatchInfoEvent event2 = MatchInfoEvent.Create(GlobalTargets.AllClients);
            event2.BlueScore = blue;
            event2.RedScore = red;
            event2.Time = time;
            event2.Send();

            _matchManager.UpdateUI(blue, red, time);
        }
    }

    //UNA VEZ SE HA ACTUALIZADO FIREBASE CON LOS DATOS NOS CAMBIAMOS DE ESCENA
    public override void OnEvent(SendPlayersToFinalScene evnt)
    {
        _matchManager.endGameScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (BoltNetwork.IsServer)
        {
            timer += Time.deltaTime;

            if (seg < timer)
            {
                time--;

                MatchInfoEvent evnt = MatchInfoEvent.Create(GlobalTargets.AllClients);
                evnt.BlueScore = blue;
                evnt.RedScore = red;
                evnt.Time = time;
                evnt.Send();

                _matchManager.UpdateUI(blue, red, time);

                //Si el tiempo se ha acabao el server manda mensaje de guardar estado de partida
                if (time <= 0)
                {
                    saveGameEvent evnt2 = saveGameEvent.Create(GlobalTargets.OnlyServer);
                    evnt2.Send();
                    Debug.Log("FINISH");
                }
                Debug.Log("TIEMPO: " + time);
                seg++;
            }
        }
    }

}
