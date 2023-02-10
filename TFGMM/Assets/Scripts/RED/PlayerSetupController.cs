using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

public class PlayerSetupController : GlobalEventListener
{
    [SerializeField]
    private Camera _sceneCamera;

    [SerializeField]
    private GameObject _setupPanel;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject[] spawners;

    public Camera SceneCamera { get => _sceneCamera; }

    private int contador = 0; // Team lejos (0,2) || Team cerca (3,5)

    private BoltEntity[] entity = new BoltEntity[6];

    private BoltConnection[] entityConnection = new BoltConnection[6];

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt.Send();

            StartMatchEvent evnt2 = StartMatchEvent.Create(GlobalTargets.OnlyServer);
            evnt2.Send();
        }
    }
     
    public override void OnEvent(SpawnPlayerEvent evnt)
    {

        BoltLog.Warn("En evento crear jugador");
        if (contador<=2) //RED
        {
            entity[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player2, spawners[contador].transform.position, Quaternion.identity);
            BoltLog.Warn("Creado objeto");
            entity[contador].AssignControl(evnt.RaisedBy);
            BoltLog.Warn("Creado asignar control");
            entity[contador].transform.Rotate(new Vector3(0, 180, 0));
            BoltLog.Warn("rotacion");
        }
        else //BLUE
        {
            entity[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[contador].transform.position, Quaternion.identity);
            entity[contador].AssignControl(evnt.RaisedBy);
        }
        entityConnection[contador] = evnt.RaisedBy;

        //Establecemos el numero del jugador en la sala
        setPlayerEvent evnts = setPlayerEvent.Create(evnt.RaisedBy, ReliabilityModes.ReliableOrdered);
        evnts.nPlayer = contador;
        evnts.Send();

        contador++;
    }

    public override void OnEvent(StartMatchEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0,0,0), Quaternion.identity);
    }

    public override void OnEvent(deletePlayersEvent evnt)
    {
        Destroy(entity[(int)evnt.numPlayer].gameObject);

        entityConnection[(int)evnt.numPlayer].Disconnect();


        contador--;
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt.Send();
    }
}