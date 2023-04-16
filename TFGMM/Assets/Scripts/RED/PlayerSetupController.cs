using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using Firebase.Database;

public class PlayerSetupController : GlobalEventListener
{
    private static int PLAYEROOM = 4; //TIENE QUE VALER LO MISMO QUE EN INFOROOM
    private int contador = 0; 
    private static int redIntSpawn = 0; //Team lejos (0,2)
    private static int blueIntSpawn = 3; //Team cerca (3,5)

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
    private int numPartida = -1;

    public static void setPLAYEROOM(int n)
    {
        PLAYEROOM = n;
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
            evnt2.winningChances = ELO.blueChances;
            evnt2.winningChances = ELO.blueChances;
            evnt2.playerRole = ComInfo.getPlayerData().playerRole;
            evnt2.Send();

            Debug.Log("CHANCES PSC: las chances de ganar del red son: " + ELO.redChances);

        }
    }

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

        Debug.Log("AUX: Se HA SPAWNEADO EL JUGADOR " + evnt.playerName + " con ID: " + motor.getID());
        //id = contador;
        //Establecemos el numero del jugador en la sala
        setPlayerEvent evnts = setPlayerEvent.Create(evnt.RaisedBy, ReliabilityModes.ReliableOrdered);
        evnts.nPlayer = contador;
        evnts.teamRed = evnt.isRed;
        evnts.Send();
        BoltLog.Warn("ENVIO EVENT PLAYER: " + contador);
        contador++;

        BoltLog.Warn("CHECK EMPEZAR PARTIDA");
        if (contador == PLAYEROOM)
        {
            BoltLog.Warn("EMPEZAR PARTIDA");
            StartMatchEvent evnt2 = StartMatchEvent.Create(GlobalTargets.OnlyServer);
            evnt2.Send();

            //Siempre le pasamos la probaibilidadad de blue
            partida.winningChancesBlue = evnt.winningChances;
            partida.winningChancesRed = 1 - evnt.winningChances;
        }
        else BoltLog.Warn("HAN ENTRADO " + contador + "/" + PLAYEROOM);

        //ACTUALIZAR LOS INDICES DE LOS RESPAWN
        if (redIntSpawn >= 3) // 0, 1, 2
            redIntSpawn = 0;
        if (blueIntSpawn >= 6) // 3, 4, 5
            blueIntSpawn = 3;

        //Guardar el tipo de jugador que es 

        if(contador == 1) //Cuando se vuelve a llenar al empezar reseteamos los datos
        {
            partida = new Match(PLAYEROOM); //Creamos donde se va a guardar toda la info
        }

        team equipo = team.blue;
        if (entities[contador - 1].gameObject.transform.CompareTag("Red"))
            equipo = team.red;
        partida.addPlayer(namePlayers[contador - 1], equipo, contador - 1, evnt.playerRole);
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
        entityCanvas = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0, 0, 0), Quaternion.identity);

        //Saber que partida es

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                BoltLog.Warn("COGIENDO NMATCHES---------------------");
                DataSnapshot snapshot = task.Result;

                Nmatches.LoadInfo(snapshot);

                int firstGame  = Nmatches.getTotalGames();

                numPartida = firstGame;

                //Cuando empieza la partida avisar al jugador de uqe partida esta jugando
                if (HeadlessServerManager.roomNumber == 0) // Si es room0 se coge el valor por defecto
                {
                    lastGamePlayedEvent ev2 = lastGamePlayedEvent.Create(GlobalTargets.AllClients);
                    ev2.gameName = "Partida " + numPartida.ToString();
                    ev2.Send();
                }
                else if(HeadlessServerManager.roomNumber == 1) // Si es room1 se le añade uno para diferenciar ambas salas
                {
                    numPartida++; //Una partida mas
                    lastGamePlayedEvent ev2 = lastGamePlayedEvent.Create(GlobalTargets.AllClients);
                    ev2.gameName = "Partida " + numPartida.ToString();
                    ev2.Send();
                }


                reference.Child("totalGames").SetValueAsync(firstGame + 2).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("actualizado valor nmatches");
                    }
                    else
                    {
                        Debug.Log("No se han enviado los datos");
                    }
                });
            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });
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
            if (redIntSpawn >= 3) // 0, 1, 2
                redIntSpawn = 0;
            if (blueIntSpawn >= 6) // 3, 4, 5
                blueIntSpawn = 3;

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
            partida.damaged(namePlayers[evnt.nameDamaged], namePlayers[evnt.damagedBy], evnt.distance);

            if (partida.GetPlayerTeam(evnt.damagedBy.ToString()) == team.red) partida.damageDealtRed += 500;
            else if (partida.GetPlayerTeam(evnt.damagedBy.ToString()) == team.blue) partida.damageDealtBlue += 500;

            //damageDoneEvent evn = damageDoneEvent.Create(entityConnection[evnt.damagedBy]);
            damageDoneEvent evn = damageDoneEvent.Create(entityConnection[evnt.damagedBy]);
            evn.namePlayer = namePlayers[evnt.damagedBy];
            evn.distance = evnt.distance;
            evn.Send();

            //damageReceivedEvent evn2 = damageReceivedEvent.Create(entityConnection[evnt.nameDamaged]);
            damageReceivedEvent evn2 = damageReceivedEvent.Create(entityConnection[evnt.nameDamaged]);
            evn2.namePlayer = namePlayers[evnt.nameDamaged];
            evn2.Send();

            Debug.Log("Ell jugador -" + namePlayers[evnt.damagedBy] + "- ha hecho daño al jugador -" + namePlayers[evnt.nameDamaged]);
        }
    }


    public override void OnEvent(healPlayerEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            //esta llamada funciona bien
            partida.healed(namePlayers[evnt.nameHealed], namePlayers[evnt.healedBy]);

            //Esto no funciona bien
            //damageDoneEvent evn = damageDoneEvent.Create(entityConnection[evnt.damagedBy]);
            healingDoneEvent evn = healingDoneEvent.Create(entityConnection[evnt.healedBy]);
            evn.Send();

            //damageReceivedEvent evn2 = damageReceivedEvent.Create(entityConnection[evnt.nameDamaged]);
            healingReceivedEvent evn2 = healingReceivedEvent.Create(entityConnection[evnt.nameHealed]);
            evn2.Send();

            Debug.Log("Ell jugador -" + namePlayers[evnt.healedBy] + "- ha curado al jugador -" + namePlayers[evnt.nameHealed]);
        }
    }

    public override void OnEvent(healingReceivedEvent evnt)
    {
        RoundData.healedMyLife+= 400;
    }

    public override void OnEvent(healingDoneEvent evnt)
    {
        RoundData.healedPlayers += 400;
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
            RoundData.hitEnemyShoots++;

            RoundData.totalDistance += evnt.distance;
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
            userHistory.UpdateUserHistory((team)evnt.winnerTeam, (int)evnt.DamageRed, (int)evnt.DamageBlue, evnt.partnerRole);

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

            //GUARDAMOS LA PARTIDA EN FIREBASE
            saveMatch(); //actualiza el nombre de partida en user history

            // MANDAMOS A LOS PLAYERS A OTRA ESCENA
            SendPlayersToFinalScene ev = SendPlayersToFinalScene.Create(GlobalTargets.AllClients);
            ev.Send();

            for (int i = 0; i < PLAYEROOM; i++)
            {
                updatePlayerStatsEvent ev2 = updatePlayerStatsEvent.Create(entityConnection[i]);
                ev2.winnerTeam = (int)partida.winner;
                ev2.partnerRole = partida.getPartnerRole(partida.players[i].name);
                ev2.DamageRed = partida.damageDealtRed;
                ev2.DamageBlue = partida.damageDealtBlue;
                ev2.Send();
            }
        }
    }

    public override void OnEvent(lastGamePlayedEvent evnt)
    {
        UserHistory userHistory = ComInfo.getPlayerData();
        Debug.Log("ESKERRRRRRRRRRRRRRRRRRR");

        userHistory.nameLastGamePlayed = evnt.gameName;
  
        ComInfo.setPlayerData(userHistory);
    }

    public void saveMatch()
    {
        string json2 = JsonUtility.ToJson(partida); //Cambiar el new Match por los datos reales de la partida

        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");
        BoltLog.Warn("---------------------");

                //Crear la partida en la base de datos
                reference.Child("Matches").Child("Partida " + numPartida.ToString()).SetRawJsonValueAsync(json2).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        BoltLog.Warn("PARTIDA GUARDADA " + numPartida);
                        //Guardar que ha hecho cada jugador en la partida
                        for (int j = 0; j < PLAYEROOM; j++)
                        {
                            BoltLog.Warn("VAMOS A GUARDAR AL JUGADOR " + j);
                            string json = partida.playerJSON(j);

                            Debug.Log(json);

                            reference.Child("Matches").Child("Partida " + numPartida.ToString()).Child(partida.players[j].name).SetRawJsonValueAsync(json).ContinueWith(task =>
                            {
                                if (task.IsCompleted && j + 1 == PLAYEROOM)
                                {
                                    BoltLog.Warn("VAMOS A ACTUALIZAR AL JUGADOR " + j);
                                    numPartida = Nmatches.getTotalGames();
                                    BoltLog.Warn("NMATCHES ACTUALIZADO " + numPartida);
                                    //nMatches aux = new nMatches();
                                    Nmatches.setTotalGames(numPartida);

                                    //string json3 = JsonUtility.ToJson(Nmatches); //JSON VACIO
                                    //BoltLog.Warn("JSON3  " + json3);
                                    ////string json3 = "{\"totalGames\":" + num + "}";

                                    //reference.SetRawJsonValueAsync(json3).ContinueWith(task =>
                                    //{
                                    //    if (task.IsCompleted)
                                    //    {
                                    //        BoltLog.Warn("YA SE HA GUARDADO jSON3  " + json3);

                                    //        //Debug.Log("saved the match");
                                    //    }
                                    //    else
                                    //    {
                                    //        //Debug.Log("No se han enviado los datos");
                                    //    }
                                    //});
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