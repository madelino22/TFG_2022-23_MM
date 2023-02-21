using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Photon.Bolt.Matchmaking;

public class InfoRoom : GlobalEventListener
{
    const int PLAYEROOM = 2;

    [SerializeField]
    TextMesh textoTotal;

    private int connections = 0;

    private List<BoltConnection> playersConnections = new List<BoltConnection>();

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

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
            BoltLog.Warn("EMPEZAMOS MATCHMAKING");
            Matchmaking();
        }
    }

    public void Matchmaking()
    {
        int n_jugadores = 0;
        //Crear partida si hay jugadores
        while(n_jugadores < PLAYEROOM)
        {
            GoGameEvent evnt = GoGameEvent.Create(playersConnections[0]);
            playersConnections.RemoveAt(0);
            evnt.Send();
            n_jugadores++;
        }

        //Devolver al menu a lo sjugadores que no hacen falta
        foreach(BoltConnection conection in playersConnections)
        {
            NoGameFoundEvent evnt = NoGameFoundEvent.Create(conection);
            evnt.Send();
            BoltLog.Warn("BORRAMOS UN JUGADOR");

        }
        playersConnections.Clear();

        SceneManager.LoadScene("BOLTMapa");
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
        BoltLog.Warn("JUGADOR DESCONECTADO");
    }

    // ----------------------------------  EVENTOS CLIENTES  --------------------------------------------

    public override void OnEvent(GoGameEvent evnt)
    {
        SceneManager.LoadScene("BOLTMapa");

        SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt2.Send();
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
