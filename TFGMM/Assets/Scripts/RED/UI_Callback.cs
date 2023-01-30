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
    private int time = 30; //seconds

    // Start is called before the first frame update
    private void Awake()
    {
        BoltLog.Warn("A");
        _matchManager.UpdateUI(1, 1, 1);
        timer = 0;
    }

    public override void OnEvent(MatchInfoEvent evnt)
    {
        if(evnt.Time <= 0)
        {
            Debug.Log("SAlir Juego");
            foreach (var connection in BoltNetwork.Connections)
            {
                connection.Disconnect();
                _matchManager.endGameScene();
            }
        }
        else _matchManager.UpdateUI(evnt.BlueScore, evnt.RedScore, evnt.Time);
    }

    // Update is called once per frame
    void Update()
    {
        if (BoltNetwork.IsServer)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                blue++;

                MatchInfoEvent evnt = MatchInfoEvent.Create(GlobalTargets.AllClients);
                evnt.BlueScore = blue;
                evnt.RedScore = red;
                evnt.Time = time;
                evnt.Send();

                _matchManager.UpdateUI(blue, red, time);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                red++;

                MatchInfoEvent evnt = MatchInfoEvent.Create(GlobalTargets.AllClients);
                evnt.BlueScore = blue;
                evnt.RedScore = red;
                evnt.Time = time;
                evnt.Send();

                _matchManager.UpdateUI(blue, red, time);
            }

            timer += Time.deltaTime;
            if (seg < timer)
            {
                time--;

                if (time > 0)
                {
                    MatchInfoEvent evnt = MatchInfoEvent.Create(GlobalTargets.AllClients);
                    evnt.BlueScore = blue;
                    evnt.RedScore = red;
                    evnt.Time = time;
                    evnt.Send();

                    _matchManager.UpdateUI(blue, red, time);
                }
                else
                {
                    Debug.Log("BORRAR SERVER");
                    deletePlayersEvent del = deletePlayersEvent.Create(GlobalTargets.OnlyServer);
                    Debug.Log("ENVIAR SERVER");
                    del.Send();
                    blue = 0;
                    red = 0;
                    time = -10;
                    Debug.Log("RESET SERVER");
                }

                seg++;
            }
        }
    }

}
