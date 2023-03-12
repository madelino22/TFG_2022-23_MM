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
            RoundData.ResetData();

            SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            string name = ComInfo.getPlayerData().userName;
            evnt2.playerName = name;
            evnt2.isRed = RoundData.isRed; //MATCH MAKING YA DETERMINO A QUE EQUIPO PERTENECE
            evnt2.Send();
        }
    }

    //public void SpawnPlayer()
    //{
    //    SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
    //    evnt.Send();
    //}

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        if (evnt.isRed) //RED 0,2,4
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

    public override void OnEvent(ShootEvent evnt)
    {
        Vector3 e = new Vector3(0, 1.7f, 0);
        BoltEntity entity;
        //DEPENDIENDO DEL EQUIPO DISPARA UNA BALA U OTRA
        if (evnt.redTeam)
            entity = BoltNetwork.Instantiate(BoltPrefabs.Bullet, evnt.Position + e, evnt.Rotation);
        else
            entity = BoltNetwork.Instantiate(BoltPrefabs.BlueBullet, evnt.Position + e, evnt.Rotation);

        //BasicShooter esta en AttackModule.
        //PlayerMotor esta en IceElemental
        //AttackModule y IceElemental son hijos de Player
        int id = evnt.id;
        entity.gameObject.GetComponent<Bullet>().setCreatorName(id);

        updatePlayerShots evnt2 = updatePlayerShots.Create(GlobalTargets.OnlyServer);
        evnt2.shooterName = ComInfo.getPlayerName();
        evnt2.Send();

        updatePlayerShots evnt3 = updatePlayerShots.Create(GlobalTargets.OnlySelf);
        evnt3.shooterName = ComInfo.getPlayerName();
        evnt3.Send();
    }

    public override void OnEvent(StartMatchEvent evnt)
    {
        partida = new Match(PLAYEROOM); //Creamos donde se va a guardar toda la info
        for (int i = 0; i < PLAYEROOM; i++)
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

    //------------------------------------UPDATE INFO FIREBASE MATCH PART------------------------------------------------------

    public override void OnEvent(RespawnEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            //FIREBASE
            partida.killed(namePlayers[evnt.nameKilled], namePlayers[evnt.killedBy]);

            //CHANGE POS IN SCENE
            PlayerMotor playerMotor = entities[evnt.nameKilled].GetComponentInChildren<PlayerMotor>();
            if (playerMotor)
                playerMotor.gameObject.transform.position = spawners[evnt.nameKilled].transform.position;

            killerEvent evn = killerEvent.Create(entityConnection[evnt.killedBy]);
            evn.Send();

            killedEvent evn2 = killedEvent.Create(entityConnection[evnt.nameKilled]);
            evn2.Send();
        }
    }

    public override void OnEvent(takeDamageEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            partida.damaged(namePlayers[evnt.nameDamaged], namePlayers[evnt.damagedBy]);

            damageDoneEvent evn = damageDoneEvent.Create(entityConnection[evnt.damagedBy]);
            evn.Send();
            
            damageReceivedEvent evn2 = damageReceivedEvent.Create(entityConnection[evnt.nameDamaged]);
            evn2.Send();
        }
    }

    public override void OnEvent(killedEvent evnt)
    {
        RoundData.deaths++;
    }

    public override void OnEvent(killerEvent evnt)
    {
        RoundData.kills++;
    }

    public override void OnEvent(damageDoneEvent evnt) //Lo recibe el jugador que ha hecho daño
    {
        RoundData.damage += 500; //SE SUPONE QUE EL DAÑO ES 500 SIEMPRE
    }

    public override void OnEvent(damageReceivedEvent evnt) //Lo recibe el jugador que ha hecho daño
    {
        RoundData.damageReceived += 500; //SE SUPONE QUE EL DAÑO ES 500 SIEMPRE
    }

    public override void OnEvent(updatePlayerShots evnt)
    {
        if (BoltNetwork.IsServer)
            partida.shoot(evnt.shooterName);
        else //El propio jugador actualiza sus stats
        {
             RoundData.totalShots++;
        }
    }

    //------------------------------------UPDATE INFO FIREBASE PLAYERSTATS PART------------------------------------------------------

    public override void OnEvent(updatePlayerStatsEvent evnt) //Se llama un vez por player al acabar partida
    {
        //ENVIAR INFO FIREBASE DEL JUGADOR------------------------
        if (BoltNetwork.IsClient)
        {
            //Cogemos la info del jugador
            UserHistory userHistory = ComInfo.getPlayerData();

            //Actualizamos con lo hecho en la partida
            userHistory.UpdateUserHistory((team)evnt.winnerTeam); 

            //Guardamos la info con la partida actualizada
            ComInfo.setPlayerData(userHistory);

            saveData(userHistory);
        }

    }

    //------------------------------------SEND MATCH INFO-------------------------------------------------------

    public override void OnEvent(saveGameEvent evnt)
    {
        if(BoltNetwork.IsServer)
        {
            partida.pointsRed = (int)evnt.redPoints;
            partida.pointsBlue = (int)evnt.bluePoints;
            if (partida.pointsRed == partida.pointsBlue)
            {
                partida.winner = team.none;
            }
            else if (partida.pointsRed > partida.pointsBlue)
            {
                partida.winner = team.red;
            }
            else partida.winner = team.blue;

            saveMatch();

            // MANDAMOS A LOS PLAYERS A OTRA ESCENA
            SendPlayersToFinalScene ev = SendPlayersToFinalScene.Create(GlobalTargets.AllClients);
            ev.Send();

            updatePlayerStatsEvent ev2 = updatePlayerStatsEvent.Create(GlobalTargets.AllClients);
            ev2.winnerTeam = (int)partida.winner;
            ev2.Send();
        }
        //else //Los clientes actualizan su resultado
        //{
        //    if (partida.pointsRed > partida.pointsBlue)
        //    {
        //        partida.winner = team.none;
        //    }
        //    else if (partida.pointsRed > partida.pointsBlue)
        //    {
        //        partida.winner = team.red;
        //    }
        //}
    }

    public void saveMatch()
    {
        string json2 = JsonUtility.ToJson(partida); //Cambiar el new Match por los datos reales de la partida

        //Saber que numero de partida es la siguiente
        int nMatches = 0;
        reference.Child("Matches").Child("nMatches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                nMatches = int.Parse(snapshot.Value.ToString());
                //Debug.Log("n matches: "+nMatches);
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
                int x = 0;
                x++;
                //Debug.Log("saved the match");
            }
            else
            {
                int c = 1000;
                c = 5;
                //Debug.Log("No se han enviado los datos");
            }
            int a = 8;
            a = 7;
        }
       );

        //Guardar que ha hecho cada jugador en la partida

        for (int j = 0; j < PLAYEROOM; j++)
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

    private void saveData(UserHistory userHistory) 
    {
        string json = JsonUtility.ToJson(userHistory);

        Debug.Log(json);

        //Save base data
        reference.Child("User").Child(userHistory.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //  Debug.Log("saved Data Profile");
            }
            else
            {
                //   Debug.Log("No se han enviado los datos");
            }
        }
        );

        int nMatches = 0;
        reference.Child("Matches").Child("nMatches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                nMatches = int.Parse(snapshot.Value.ToString());
                //Debug.Log("n matches: "+nMatches);
            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });

        json = userHistory.lastGameNotSaved("Partida "+ nMatches);
        //Debug.Log(i);
        Debug.Log(json);

        for (int i = 0; i < 5; i++)
        {
            json = userHistory.saveGames(i);
            Debug.Log(i);
            Debug.Log(json);

            reference.Child("User").Child(userHistory.userName).Child("zzzLastGames").Child("Partida" + i).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("saved games");
                }
                else
                {
                    Debug.Log("No se ha guardado la partida");
                }
            }
            );

          
        }

    }

}
