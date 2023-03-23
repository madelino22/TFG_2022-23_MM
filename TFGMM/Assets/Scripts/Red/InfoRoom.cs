using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Photon.Bolt.Matchmaking;
using UdpKit;

using System.Linq; //List sort

public class InfoRoom : GlobalEventListener
{
    const int PLAYEROOM = 2; //TIENE QUE VALER LO MISMO QUE EN PLAYERSETUPCONTROLLER

    [SerializeField]
    TextMesh textoTotal;

    private string sessionID = "NoRoom";

    private float timer = 0;
    private bool startTimer = false;

    private int connections = 0;

    private List<BoltConnection> playersConnections = new List<BoltConnection>();
    private List<KeyValuePair<int, int>> dataOfAllUsers = new List<KeyValuePair<int, int>>(); //esto sera UserHistory mas tarde

    //private Use
    //public override void BoltStartBegin()
    //{
    //    BoltNetwork.RegisterTokenClass<UserHistory>();
    //}
    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        PlayerSetupController.setPLAYEROOM(PLAYEROOM);

        if (!BoltNetwork.IsServer)
        {
            UserHistory user = ComInfo.getPlayerData();

            JoinPlayerEvent evnt = JoinPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt.name = user.userName;
            evnt.elo = user.eloRanking;
            evnt.Send();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Matchmaking();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PhotonRoomProperties roomProperties = new PhotonRoomProperties();

            roomProperties.AddRoomProperty("m", "BOLTMapa"); // ex: map id

            roomProperties.IsOpen = true;
            roomProperties.IsVisible = true;

            BoltMatchmaking.CreateSession(
                    sessionID: "0",
                    token: roomProperties,
                    sceneToLoad: "BOLTMapa"
                );

            BoltLog.Warn("Otra room");
        }
        //if (startTimer)
        //    timer += Time.deltaTime;
        //if(timer >= 5000)
        //{

        //}
        if (startTimer)
        {
            BoltLauncher.StartClient();

            BoltMatchmaking.JoinSession(sessionID);
            BoltLog.Warn("AAAAA");

            ComInfo.setTeam(team.red);

            if (BoltMatchmaking.CurrentSession.HostName != "Test")
            {
                SceneManager.LoadScene("BOLTMapa");
                BoltLog.Warn("BBBB");

                //SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
                //evnt2.Send();
            }

            // PROBAR SI ESTO FUNCIONA ----------------------------------------------------

            //else
            //{
            //    BoltLauncher.Shutdown();
            //}
        }
    }



    public void Matchmaking()
    {
        int numPlayers = playersConnections.Count;
        int n = numPlayers;
        BoltLog.Warn("Hay " + numPlayers + "players.");

        //Crear partida si hay jugadores        
        int map = 0;
        var sortedList = dataOfAllUsers.OrderBy(x => x.Value).ToList(); //ordena el elo en orden ascendente

        while (numPlayers >= PLAYEROOM)
        {           
            int contador = 0;

            //UwU descomentar esto
            List<int> blueELOS = new();
            List<int> redELOS = new();

            List<GoGameEvent> evnts = new List<GoGameEvent>();
            while (contador < PLAYEROOM)
            {
                //GoGameEvent evnt = GoGameEvent.Create(playersConnections[sortedList[0].Key]);
                evnts.Add(GoGameEvent.Create(playersConnections[contador]));

                // HACEMOS NUESTRO MATCHMAKING Y DETERMINAMOS COMO SE FORMAN LOS EQUIPOS

                //evnt.isRed = (contador % 2) == 0;  //PARA QUE SPAWN EVENT SEPA A QUE EQUIPO VA
                evnts[contador].isRed = (contador < PLAYEROOM / 2);

                if (evnts[contador].isRed)
                    redELOS.Add(sortedList[contador].Value);
                else
                    blueELOS.Add(sortedList[contador].Value);

                if (map == 0)
                    evnts[contador].ID = "0";
                else
                    evnts[contador].ID = "1";
                //playersConnections.RemoveAt(sortedList[0].Key);
                //sortedList.RemoveAt(0);
                //numPlayers--;
                BoltLog.Warn("Jugador " + contador + ", va a " + evnts[contador].ID);
                contador++;
            }

            //UwU calcular media de los dos equipos
            //llamar al m�todo de c�lculo de winning chances de ELO
            //UwU descomentar esto
            Tuple<float, float> winningChances = ELO.CalculateWinningChances(redELOS, blueELOS);
            //pasar la variable de probabilidad de victoria a cada cliente por evento

            for(int i = 0; i < PLAYEROOM;i++)
            {
                if (evnts[i].isRed)
                {
                    evnts[i].winningChances = winningChances.Item2;
                }
                else evnts[i].winningChances = winningChances.Item1;

                evnts[i].Send();

                playersConnections.RemoveAt(0);
            }



            numPlayers -= PLAYEROOM;
            map++;
        }

        //while (numPlayers >= PLAYEROOM && ((numPlayers - contador) >= PLAYEROOM))
        //{
        //    GoGameEvent evnt = GoGameEvent.Create(playersConnections[0]);
        //    if (contador < PLAYEROOM)
        //        evnt.ID = "0";
        //    else
        //        evnt.ID = "1";
        //    evnt.Send();

        //    playersConnections.RemoveAt(0);
        //    contador++;
        //    //numPlayers--;
        //    BoltLog.Warn("Jugador " + contador + ", va a " + evnt.ID);
        //}

        //Devolver al menu a los jugadores que no hacen falta
        foreach (BoltConnection conection in playersConnections)
        {
            NoGameFoundEvent evnt = NoGameFoundEvent.Create(conection);
            evnt.Send();
        }
        playersConnections.Clear();

        //SceneManager.LoadScene("BOLTMapa");
    }

    public override void Disconnected(BoltConnection connection)
    {
        //BoltLauncher.Shutdown();
        //BoltNetwork.Shutdown();
        //BoltLog.Warn("Me desconecte");
        //if (sessionID == "0")
        //{
        //    var session = BoltMatchmaking.CurrentSession;
        //    BoltLog.Warn(session.HostName);

        //    BoltLauncher.StartClient();      
        //}
        //else if (sessionID == "1")
        //{
        //    BoltLog.Warn("Esperando sala 1");
        //    BoltLauncher.StartClient();

        //}
        BoltLauncher.StartClient();

        BoltMatchmaking.JoinSession(sessionID);
        BoltLog.Warn("Session joined");

        base.Disconnected(connection);
    }


    // ----------------------------------  EVENTOS SERVER  --------------------------------------------

    public override void OnEvent(JoinPlayerEvent evnt) //LO RECIBE EL SERVER
    {
        //Guardamos conexion del player
        playersConnections.Add(evnt.RaisedBy);

        // DATA PLAYER ==> MATCHMAKING
        int elo = evnt.elo;
        dataOfAllUsers.Add(new KeyValuePair<int,int>(connections, elo));

        connections++;

        //Mandamos evento a clientes para mostrar el numero de players conectados
        NumberPlayersEvent evnt2 = NumberPlayersEvent.Create(GlobalTargets.AllClients);
        evnt2.numPlayersConnected = connections;
        evnt2.Send();
    }

    public override void OnEvent(DisconectPlayerEvent evnt)
    {
        BoltLog.Warn("JUGADOR ANTES DE DESCONECTADO");
        evnt.RaisedBy.Disconnect();
        connections--;
        BoltLog.Warn("JUGADOR DESCONECTADO");
    }
    // ----------------------------------  EVENTOS CLIENTES  --------------------------------------------

    public override void OnEvent(GoGameEvent evnt)
    {
        RoundData.isRed = evnt.isRed; //PARA QUE SPAWN EVENT SEPA A QUE EQUIPO VA

        if(evnt.isRed)
        {
            ELO.redChances = evnt.winningChances;
            ELO.blueChances = 1 - evnt.winningChances;
        }
        else
        {
            ELO.redChances = 1 - evnt.winningChances;
            ELO.blueChances = evnt.winningChances;
        }

            Debug.Log("CHANCES IR: las chances de ganar del red son: " + ELO.redChances); 

        //SceneManager.LoadScene("BOLTMapa");
        sessionID = evnt.ID;
        BoltLog.Warn("Guardo ID: " + sessionID);

        startTimer = true;

        DisconectPlayerEvent evnt2 = DisconectPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt2.Send();
        BoltLog.Warn("Cambiar jugador servidor/sala " + sessionID);
        //SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        //evnt2.Send();
    }
    public override void OnEvent(NoGameFoundEvent evnt)
    {
        SceneManager.LoadScene("Lobby");
        //Le decimos al server que desconecte al jugador
        DisconectPlayerEvent evnt2 = DisconectPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt2.Send();

        BoltLog.Warn("Enviado mensaje borrar al jugador");
    }

    public override void OnEvent(NumberPlayersEvent evnt)
    {
        textoTotal.text = evnt.numPlayersConnected.ToString();
    }



}
