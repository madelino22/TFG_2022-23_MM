using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using Firebase.Database;

public class PlayerSetupController : GlobalEventListener
{
    private static int PLAYEROOM = 6; //TIENE QUE VALER LO MISMO QUE EN INFOROOM
    private int contador = 0; 
    private static int redIntSpawn = 0; //Team lejos (0,2)
    private static int blueIntSpawn = PLAYEROOM / 2; //Team cerca (3,5)

    [SerializeField]
    private Camera _sceneCamera;

    [SerializeField]
    private GameObject _setupPanel;

    [SerializeField]
    private Canvas canvas;

    public Camera SceneCamera { get => _sceneCamera; }
    private BoltEntity entityCanvas;

    [SerializeField]
    private GameObject[] spawners;
    private BoltEntity[] entities = new BoltEntity[PLAYEROOM];
    private BoltConnection[] entityConnection = new BoltConnection[PLAYEROOM];
    private string[] namePlayers = new string[PLAYEROOM];

    //Referencia a Firebase
    DatabaseReference reference;

    Match partida;
    nMatches Nmatches = new nMatches();

    public static void setPLAYEROOM(int n)
    {
        PLAYEROOM = n;
        redIntSpawn = 0;
        blueIntSpawn = PLAYEROOM / 2;
    }

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
        if (evnt.isRed) //RED 0,1,2
        {
            entities[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player2, spawners[redIntSpawn].transform.position, Quaternion.identity);
            entities[contador].AssignControl(evnt.RaisedBy);
            entities[contador].transform.Rotate(new Vector3(0, 180, 0));
            redIntSpawn++;
            //entity[contador].GetComponent<PlayerCallback>().enabled = true;
        }
        else //BLUE 3,4,5
        {
            entities[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[blueIntSpawn].transform.position, Quaternion.identity);
            entities[contador].AssignControl(evnt.RaisedBy);
            blueIntSpawn++;
        }
        PlayerMotor motor = entities[contador].GetComponentInChildren<PlayerMotor>();
        motor.setID(contador);
        motor.SetTeam((evnt.isRed) ? 1 : 0);
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

        //ACTUALIZAR LOS INDICES DE LOS RESPAWN
        if (redIntSpawn >= PLAYEROOM / 2) // 0, 1, 2
            redIntSpawn = 0;
        if (blueIntSpawn >= PLAYEROOM) // 3, 4, 5
            redIntSpawn = PLAYEROOM / 2;
    }

    //Solo lo ejecuta el server
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
        int i = 0;
        while (i < PLAYEROOM && namePlayers[i] != evnt.nameShooter)
        {
            i++;
        }

        int id = i;
        entity.gameObject.GetComponent<Bullet>().setCreatorID(id);

        //Este evento es para actualizar match
        updatePlayerShots evnt2 = updatePlayerShots.Create(GlobalTargets.OnlyServer);
        evnt2.shooterName = evnt.nameShooter;
        evnt2.Send();
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
        entityCanvas = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public override void OnEvent(deletePlayersEvent evnt)
    {
        BoltLog.Warn("SE destruye el jugador " + (int)evnt.numPlayer);

        BoltNetwork.Destroy(entities[(int)evnt.numPlayer].gameObject);

        entityConnection[(int)evnt.numPlayer].Disconnect();

        contador--;
        BoltLog.Warn("Contador: " + contador);
        if (contador == 0) 
            BoltNetwork.Destroy(entityCanvas);
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
            if (playerMotor && playerMotor.GetTeam() == 1) //ROJO
            {
                playerMotor.gameObject.transform.position = spawners[redIntSpawn].transform.position;
                redIntSpawn++;
            }
            else if (playerMotor && playerMotor.GetTeam() == 0) //AZUL
            {
                playerMotor.gameObject.transform.position = spawners[blueIntSpawn].transform.position;
                blueIntSpawn++;
            }

            //ACTUALIZAR LOS INDICES DE LOS RESPAWN
            if (redIntSpawn >= PLAYEROOM / 2) // 0, 1, 2
                redIntSpawn = 0;
            if (blueIntSpawn >= PLAYEROOM) // 3, 4, 5
                redIntSpawn = PLAYEROOM / 2;

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
            //esta llamada funciona bien
            partida.damaged(namePlayers[evnt.nameDamaged], namePlayers[evnt.damagedBy]);

            //Esto no funciona bien
            //damageDoneEvent evn = damageDoneEvent.Create(entityConnection[evnt.damagedBy]);
            damageDoneEvent evn = damageDoneEvent.Create(GlobalTargets.AllClients);
            evn.namePlayer = namePlayers[evnt.damagedBy];
            evn.Send();

            //damageReceivedEvent evn2 = damageReceivedEvent.Create(entityConnection[evnt.nameDamaged]);
            damageReceivedEvent evn2 = damageReceivedEvent.Create(GlobalTargets.AllClients);
            evn2.namePlayer = namePlayers[evnt.nameDamaged];
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
        Debug.Log("Disparó: hizo daño");
        if (evnt.namePlayer == ComInfo.getPlayerName())
        {
            RoundData.damageInflicted += 500; //SE SUPONE QUE EL DAÑO ES 500 SIEMPRE
            Debug.Log("Disparó: daño inflingido actual: " + RoundData.damageInflicted);
        }
    }

    public override void OnEvent(damageReceivedEvent evnt) //Lo recibe el jugador que ha hecho daño
    {
        Debug.Log("Disparó: recibe daño");

        if (evnt.namePlayer == ComInfo.getPlayerName())
        {
            RoundData.damageReceived += 500; //SE SUPONE QUE EL DAÑO ES 500 SIEMPRE
            Debug.Log("Disparó: daño recibido actual: " + RoundData.damageReceived);

        }
    }

    public override void OnEvent(updatePlayerShots evnt)
    {
        if (BoltNetwork.IsServer)
            partida.shoot(evnt.shooterName);
        else //OnlySelf ==> El propio jugador actualiza sus stats
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
        if (BoltNetwork.IsServer)
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

        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");

        //Nmatches.setTotalGames(7);

        //Saber que numero de partida es la siguiente
        //int num = 0;
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                BoltLog.Warn("COGIENDO NMATCHES---------------------");
                DataSnapshot snapshot = task.Result;

                Nmatches.LoadInfo(snapshot);

                int num = Nmatches.getTotalGames();
                //Debug.Log("n matches: "+nMatches);
                BoltLog.Warn("NMATCHES COGIDO " + num);

                //Crear la partida en la base de datos
                reference.Child("Matches").Child("Partida " + num.ToString()).SetRawJsonValueAsync(json2).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        BoltLog.Warn("PARTIDA GUARDADA " + num);
                        //Guardar que ha hecho cada jugador en la partida
                        for (int j = 0; j < PLAYEROOM; j++)
                        {
                            BoltLog.Warn("VAMOS A GUARDAR AL JUGADOR " + j);
                            string json = partida.playerJSON(j);

                            Debug.Log(json);

                            reference.Child("Matches").Child("Partida " + num.ToString()).Child(partida.players[j].name).SetRawJsonValueAsync(json).ContinueWith(task =>
                            {
                                if (task.IsCompleted && j + 1 == PLAYEROOM)
                                {
                                    BoltLog.Warn("VAMOS A ACTUALIZAR AL JUGADOR " + j);
                                    num = Nmatches.getTotalGames();
                                    num++;
                                    BoltLog.Warn("NMATCHES ACTUALIZADO " + num);
                                    //nMatches aux = new nMatches();
                                    Nmatches.setTotalGames(num);

                                    string json3 = JsonUtility.ToJson(Nmatches); //JSON VACIO
                                    BoltLog.Warn("JSON3  " + json3);
                                    //string json3 = "{\"totalGames\":" + num + "}";

                                    reference.SetRawJsonValueAsync(json3).ContinueWith(task =>
                                    {
                                        if (task.IsCompleted)
                                        {
                                            BoltLog.Warn("YA SE HA GUARDADO jSON3  " + json3);

                                            //Debug.Log("saved the match");
                                        }
                                        else
                                        {
                                            //Debug.Log("No se han enviado los datos");
                                        }
                                    });
                                }
                                else if (task.IsCompleted)
                                {
                                    BoltLog.Warn("GUARDADO JUGADOR " + j);
                                }
                                else
                                {
                                    //Debug.Log("No se ha guardado al jugador");
                                }
                            }
                            );
                        }
                        //--------------------------------------------------------------------------------


                    }
                    else
                    {

                    }

                }
               );


            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });




        //Guardar que se ha jugado una partida mas



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

        //int nMatches = 0;
        //reference.Child("Matches").Child("nMatches").GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;

        //        nMatches = int.Parse(snapshot.Value.ToString());
        //        //Debug.Log("n matches: "+nMatches);
        //    }
        //    else
        //    {
        //        Debug.Log("No se han encontrado el numero de partidas totales");
        //    }
        //});

        //json = userHistory.lastGameNotSaved("Partida "+ nMatches);
        ////Debug.Log(i);
        //Debug.Log(json);

        //for (int i = 0; i < 5; i++)
        //{
        //    json = userHistory.saveGames(i);
        //    Debug.Log(i);
        //    Debug.Log(json);

        //    reference.Child("User").Child(userHistory.userName).Child("zzzLastGames").Child("Partida" + i).SetRawJsonValueAsync(json).ContinueWith(task =>
        //    {
        //        if (task.IsCompleted)
        //        {
        //            Debug.Log("saved games");
        //        }
        //        else
        //        {
        //            Debug.Log("No se ha guardado la partida");
        //        }
        //    }
        //    );
        //}

    }

}


public class nMatches
{
    private int totalGames = 0;

    public void LoadInfo(DataSnapshot snapshot)
    {
        string value = snapshot.Child("totalGames").Value.ToString();
        totalGames = int.Parse(value);
    }

    public void setTotalGames(int n)
    {
        totalGames = n;
    }

    public int getTotalGames()
    {
        return totalGames;
    }
}