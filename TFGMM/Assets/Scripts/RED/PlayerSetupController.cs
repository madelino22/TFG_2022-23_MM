using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using Firebase.Database;

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

    private BoltEntity[] entities = new BoltEntity[PLAYEROOM];

    private BoltEntity entityCanvas;

    private BoltConnection[] entityConnection = new BoltConnection[PLAYEROOM];

    private string[] namePlayers = new string[PLAYEROOM];

    //Referencia a Firebase
    DatabaseReference reference;

    Match partida;

    public void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            string name = ComInfo.getPlayerData().name;
            evnt2.playerName = name;
            evnt2.Send();
        }
    }
    public override void OnEvent(RespawnEvent evnt)
    {
        int id = evnt.id;
        string killedBy = evnt.killedBy;
        string nameKilled = evnt.nameKilled;

        //FIREBASE
        partida.killed(nameKilled, killedBy);

        //CHANGE POS IN SCENE
        PlayerMotor playerMotor = entities[id].GetComponentInChildren<PlayerMotor>();
        if (playerMotor)
            playerMotor.gameObject.transform.position = spawners[id].transform.position; 
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
        //FIREBASE
        namePlayers[contador] = evnt.playerName;
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
        partida = new Match(PLAYEROOM); //Creamos donde se va a guardar toda la info
        for (int i = 0; i< PLAYEROOM; i++)
        {
            team equipo = team.blue;
            if (entities[i].gameObject.transform.CompareTag("Red"))
                equipo = team.red;
            partida.addPlayer(namePlayers[i], equipo, i);
        }
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

    //------------------------------------SEND MATCH INFO-------------------------------------------------------

    public override void OnEvent(saveGameEvent evnt)
    {
        saveMatch();
    }

    public void saveMatch()
    {
        string json2 = JsonUtility.ToJson(partida); //Cambiar el new Match por los datos reales de la partida


        //Saber que numero de partida es la siguiente
        int nMatches = 0;
        reference.Child("Matches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                nMatches = int.Parse(snapshot.Child("nMatches").Value.ToString());
            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });

        //Crear la partida en la base de datos
        reference.Child("Matches").Child("Partida " + nMatches.ToString()).SetRawJsonValueAsync(json2).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //Debug.Log("saved the match");
            }
            else
            {
                //Debug.Log("No se han enviado los datos");
            }
        }
       );

        //Guardar que ha hecho cada jugador en la partida

        for (int j = 0; j < 6; j++)
        {
            string json = partida.playerJSON(j);

            Debug.Log(json);

            reference.Child("Matches").Child("Partida " + nMatches.ToString()).Child(partida.players[j].name).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    //Debug.Log("saved players of game");
                }
                else
                {
                    //Debug.Log("No se ha guardado al jugador");
                }
            }
            );
        }

        //Guardar que se ha jugado una partida mas

        nMatches++;

        string json3 = JsonUtility.ToJson(nMatches);

        reference.Child("Matches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                reference.Child("nMatches").SetRawJsonValueAsync(json3).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        //Debug.Log("saved the match");
                    }
                    else
                    {
                        //Debug.Log("No se han enviado los datos");
                    }
                });
            }
            else
            {
                Debug.Log("No se ha encontrado matches");
            }
        });

    }
}
