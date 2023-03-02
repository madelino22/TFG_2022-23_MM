using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

public class PlayerSetupController : GlobalEventListener
{
    const int PLAYEROOM = 2; //NUM MAX JUGADORES?

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

    private BoltEntity[] entities = new BoltEntity[6];

    private BoltEntity entityCanvas;

    private BoltConnection[] entityConnection = new BoltConnection[6];

    //private int id=0;

    //public int getId()
    //{
    //    return id;
    //}

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt2.Send();
        }
    }
    public override void OnEvent(RespawnEvent evnt)
    {
        int id = evnt.id;
        if (entities[id].GetComponentInChildren<PlayerMotor>())
            entities[id].GetComponentInChildren<PlayerMotor>().gameObject.transform.position= spawners[id].transform.position;
            //if (id % 2 == 0)
            //{
            //    entity[id] = BoltNetwork.Instantiate(BoltPrefabs.Player2, spawners[id].transform.position, Quaternion.identity);
            //    entity[id].AssignControl(evnt.RaisedBy);
            //    entity[id].transform.Rotate(new Vector3(0, 180, 0));
            //}
            //else
            //{
            //    entity[id] = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[id].transform.position, Quaternion.identity);
            //    entity[id].AssignControl(evnt.RaisedBy);
            //}
        
    }
    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        if (contador % 2 == 0) //RED 0,2,4
        {
            entities[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player2, spawners[contador].transform.position, Quaternion.identity);
            entities[contador].AssignControl(evnt.RaisedBy);
            entities[contador].transform.Rotate(new Vector3(0, 180, 0));
            //entity[contador].GetComponent<PlayerCallback>().enabled = true;
        }
        else //BLUE 1,3,5
        {
            entities[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[contador].transform.position, Quaternion.identity);
            entities[contador].AssignControl(evnt.RaisedBy);
        }
        entities[contador].GetComponentInChildren<PlayerMotor>().setID(contador);
        entityConnection[contador] = evnt.RaisedBy;
        //id = contador;
        //Establecemos el numero del jugador en la sala
        setPlayerEvent evnts = setPlayerEvent.Create(evnt.RaisedBy, ReliabilityModes.ReliableOrdered);
        evnts.nPlayer = contador;
        evnts.Send();
        BoltLog.Warn("ENVIO EVENT PLAYER: " + contador);
        contador++;

        BoltLog.Warn("CHECK EMPEZAR PARTIDA");
        if (contador == PLAYEROOM)
        {
            BoltLog.Warn("EMPEZAR PARTIDA");
            StartMatchEvent evnt2 = StartMatchEvent.Create(GlobalTargets.OnlyServer);
            evnt2.Send();
        }
        else BoltLog.Warn("HAN ENTRADO " + contador + "/" + PLAYEROOM);
    }

    public override void OnEvent(StartMatchEvent evnt)
    {
        entityCanvas = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0,0,0), Quaternion.identity);
    }

    public override void OnEvent(deletePlayersEvent evnt)
    {
        BoltLog.Warn("SE destruye el jugador " + (int)evnt.numPlayer);

        BoltNetwork.Destroy(entities[(int)evnt.numPlayer].gameObject);

        entityConnection[(int)evnt.numPlayer].Disconnect();


        contador--;
        BoltLog.Warn("Contador: " + contador);
        if (contador == 0) BoltNetwork.Destroy(entityCanvas);
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt.Send();
    }
}