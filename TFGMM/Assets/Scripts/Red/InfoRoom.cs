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

public class InfoRoom : GlobalEventListener
{
    const int PLAYEROOM = 2;

    [SerializeField]
    TextMesh textoTotal;

    private string sessionID = "NoRoom";

    private float timer = 0;
    private bool startTimer = false;

    private int connections = 0;

    private List<BoltConnection> playersConnections = new List<BoltConnection>();

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            JoinPlayerEvent evnt = JoinPlayerEvent.Create(GlobalTargets.OnlyServer);
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
        int contador = 0;
        int numPlayers = playersConnections.Count;
        BoltLog.Warn("Hay " + numPlayers);
        //Crear partida si hay jugadores
        while (numPlayers >= PLAYEROOM)
        {
            GoGameEvent evnt = GoGameEvent.Create(playersConnections[0]);
            if (contador < PLAYEROOM)
                evnt.ID = "0";
            else
                evnt.ID = "1";
            evnt.Send();

            playersConnections.RemoveAt(0);
            contador++;
            //numPlayers--;
            BoltLog.Warn("Jugador " + contador + ", va a " + evnt.ID);
        }

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

    public override void OnEvent(JoinPlayerEvent evnt)
    {
        //Guardamos conexion del player
        playersConnections.Add(evnt.RaisedBy);

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
