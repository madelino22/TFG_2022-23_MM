using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class InfoRoom : GlobalEventListener
{
    [SerializeField]
    TextMesh textoTotal;

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
            BoltLog.Warn("EMPEZAMOS MATCHMAKING");
            Matchmaking();
        }
    }

    public void Matchmaking()
    {
        BoltLog.Warn("BUSCAMOS MATCHMAKING");
        //Enviar mensaje GoGame
        GoGameEvent evnt = GoGameEvent.Create(playersConnections[0]);
        playersConnections.RemoveAt(0);
        evnt.Send();
        
        GoGameEvent evnt2 = GoGameEvent.Create(playersConnections[0]);
        playersConnections.RemoveAt(0);
        evnt2.Send();

        SceneManager.LoadScene("BOLTMapa");


    }

    // ----------------------------------  EVENTOS SERVER  --------------------------------------------

    public override void OnEvent(JoinPlayerEvent evnt)
    {
        //Guardamos conexion del player
        BoltLog.Warn("SE conecta 1");
        playersConnections.Add(evnt.RaisedBy);

        connections++;

        BoltLog.Warn("SE conecta 2");
        //Mandamos evento a clientes para mostrar el numero de players conectados
        NumberPlayersEvent evnt2 = NumberPlayersEvent.Create(GlobalTargets.AllClients);
        evnt2.numPlayersConnected = connections;
        evnt2.Send();
    }

    // ----------------------------------  EVENTOS CLIENTES  --------------------------------------------

    public override void OnEvent(GoGameEvent evnt)
    {
        SceneManager.LoadScene("BOLTMapa");

        SpawnPlayerEvent evnt2 = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt2.Send();
    }

    public override void OnEvent(NumberPlayersEvent evnt)
    {
        textoTotal.text = evnt.numPlayersConnected.ToString();
    }


}
