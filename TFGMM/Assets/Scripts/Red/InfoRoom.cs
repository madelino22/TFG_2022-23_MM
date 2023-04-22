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
    private struct infoMatchmaking
    {
        public infoMatchmaking(int e, string role, BoltConnection connection)
        {
            elo = e;
            playerRole = role;
            playerConnection = connection;
        }

        public int elo { get; }
        public RolEnum GetPlayerRolEnum()
        {

            RolEnum rol = RolEnum.NONE;

            switch (playerRole)
            {
                case "None":
                    rol = RolEnum.NONE;
                    break;
                case "Sniper":
                    rol = RolEnum.SNIPER;
                    break;
                case "Duelist":
                    rol = RolEnum.DUELIST;
                    break;
                case "Healer":
                    rol = RolEnum.HEALER;
                    break;
            }
            return rol;
        }
        public string playerRole { get; }

        public BoltConnection playerConnection { get; }
    }

    const int PLAYEROOM = 4; //TIENE QUE VALER LO MISMO QUE EN PLAYERSETUPCONTROLLER
    const int ELODIFF = 300;

    [SerializeField]
    TextMesh textoTotal;

    private string sessionID = "NoRoom";

    private float timer = 0;
    private bool startTimer = false;

    private int connections = 0;

    //private List<BoltConnection> playersConnections = new List<BoltConnection>();
    private List<KeyValuePair<int, infoMatchmaking>> dataOfAllUsers = new List<KeyValuePair<int, infoMatchmaking>>();

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        PlayerSetupController.setPLAYEROOM(PLAYEROOM);

        if (!BoltNetwork.IsServer)
        {
            UserHistory user = ComInfo.getPlayerData();

            JoinPlayerEvent evnt = JoinPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt.name = user.userName;
            evnt.elo = (int)user.eloRanking;
            evnt.playerRol = user.playerRole;
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
        int numPlayers = dataOfAllUsers.Count;
        int n = numPlayers;
        BoltLog.Warn("Hay " + numPlayers + "players.");

        //Crear partida si hay jugadores        
        int map = 0;
        var sortedList = dataOfAllUsers.OrderBy(x => x.Value.elo).ToList(); //ordena el elo en orden ascendente

        while (numPlayers >= PLAYEROOM)
        {
            int contador = 0;

            //UwU descomentar esto
            List<int> blueELOS = new();
            List<int> redELOS = new();

            List<GoGameEvent> evnts = new List<GoGameEvent>();

            //Comprobamos diferencia de elo entre jugadores
            int a = sortedList[contador].Value.elo - sortedList[contador + PLAYEROOM - 1].Value.elo;
            a = Math.Abs(a);
            while (a > ELODIFF && numPlayers > PLAYEROOM) //Si es mayor al limite o ya no hay otra combinacion posible
            {
                contador++;
                numPlayers--;
                a = sortedList[contador].Value.elo - sortedList[contador + PLAYEROOM - 1].Value.elo;
                a = Math.Abs(a);
            }

            if (numPlayers < PLAYEROOM || a > ELODIFF) //No hay jugadores suficientes o ultima combinacion de elo no es posible.
            {
                break;
            }
            else
            {
                int jugadoresJoin = 0;
                int first = contador;

                bool rolMM = (a <= 50);
                (int, RolEnum)[] roles = new (int, RolEnum)[PLAYEROOM];


                ((int, RolEnum), (int, RolEnum))[] bestPairs = new ((int, RolEnum), (int, RolEnum))[2]; ;


                if (rolMM)
                {
                    //si la diferencia entre todos los jugadores que se han seleccionado para esta partida es relativamente grande,
                    //entonces se organizand para que la media de ranking de cada equipo sea lo m´´as pareja posible, así los equipos
                    //tendrán jugadores parejos dentro de la "diferencia de ranking". De esta manera el mejor de un equipo será muy parecido al 
                    //mejor del otro
                    for (int i = 0; i < PLAYEROOM; i++)
                    {
                        roles[i].Item1 = jugadoresJoin + i;
                        roles[i].Item2 = sortedList[i].Value.GetPlayerRolEnum();
                    }
                    bestPairs = ROLES.FindBestRolePairs(roles);




                    //Crear los eventos
                    for (int i = 0; i < PLAYEROOM; i++)
                    {
                        evnts.Add(GoGameEvent.Create(sortedList[i + contador].Value.playerConnection));
                    }

                    //Team REd
                    //Player1
                    evnts[bestPairs[0].Item1.Item1].isRed = false;
                    redELOS.Add(sortedList[contador].Value.elo);
                    jugadoresJoin++;
                    contador++;

                    evnts[bestPairs[0].Item1.Item1].ID = (map == 0) ? "0" : "1";

                    //Player2
                    evnts[bestPairs[0].Item2.Item1].isRed = false;
                    redELOS.Add(sortedList[contador].Value.elo);
                    jugadoresJoin++;
                    contador++;

                    evnts[bestPairs[0].Item2.Item1].ID = (map == 0) ? "0" : "1";




                    //Team Blue
                    //Player1
                    evnts.Add(GoGameEvent.Create(sortedList[contador].Value.playerConnection));
                    evnts[bestPairs[1].Item1.Item1].isRed = true;
                    blueELOS.Add(sortedList[contador].Value.elo);
                    jugadoresJoin++;
                    contador++;

                    evnts[bestPairs[1].Item1.Item1].ID = (map == 0) ? "0" : "1";

                    //Player2
                    evnts[bestPairs[1].Item2.Item1].isRed = true;
                    blueELOS.Add(sortedList[contador].Value.elo);
                    jugadoresJoin++;
                    contador++;

                    evnts[bestPairs[1].Item2.Item1].ID = (map == 0) ? "0" : "1";


                }
                else
                {
                    //si la diferencia de ranking entre todos los jugadores es ínfima, entonces se organizan los jugadores en cuanto a rol favorito
                    //para que así la satisfacción de la partida mejore
                    while (jugadoresJoin < PLAYEROOM)
                    {
                        //GoGameEvent evnt = GoGameEvent.Create(playersConnections[sortedList[0].Key]);
                        evnts.Add(GoGameEvent.Create(sortedList[contador].Value.playerConnection));

                        // HACEMOS NUESTRO MATCHMAKING Y DETERMINAMOS COMO SE FORMAN LOS EQUIPOS

                        //evnts[jugadoresJoin].isRed = (jugadoresJoin < PLAYEROOM / 2);

                        evnts[jugadoresJoin].isRed = (jugadoresJoin < PLAYEROOM / 2);

                        if (evnts[jugadoresJoin].isRed)
                            redELOS.Add(sortedList[contador].Value.elo);
                        else
                            blueELOS.Add(sortedList[contador].Value.elo);

                        if (map == 0)
                            evnts[jugadoresJoin].ID = "0";
                        else
                            evnts[jugadoresJoin].ID = "1";
                        //playersConnections.RemoveAt(sortedList[0].Key);
                        //sortedList.RemoveAt(0);
                        //numPlayers--;
                        BoltLog.Warn("Jugador " + jugadoresJoin + ", va a " + evnts[jugadoresJoin].ID);
                        jugadoresJoin++;
                        contador++;
                    }
                }


                int difRed;
                difRed = redELOS[0] - redELOS[1];
                difRed = Math.Abs(difRed);

                int difBlue;
                difBlue = blueELOS[0] - blueELOS[1];
                difBlue = Math.Abs(difBlue);

                float predRed = 0;
                float predBlue = 0;

                if (difRed != 0)
                    predRed = (float)Math.Pow(1.015, difRed);

                if (difBlue != 0)
                    predBlue = (float)Math.Pow(1.015, difBlue);


                //UwU calcular media de los dos equipos
                //llamar al m�todo de c�lculo de winning chances de ELO
                //UwU descomentar esto
                Tuple<float, float> winningChances = ELO.CalculateWinningChances(redELOS, blueELOS);
                //pasar la variable de probabilidad de victoria a cada cliente por evento

                for (int i = 0; i < PLAYEROOM; i++)
                {
                    if (evnts[i].isRed)
                    {
                        if (i == 0 || i == 1) //Los primeros jugadores de la partida son los de menor ELO
                            evnts[i].ExpectedContribution = 50 - (predRed / 2);
                        else evnts[i].ExpectedContribution = 50 + (predRed / 2);
                        evnts[i].winningChances = winningChances.Item2;
                    }
                    else
                    {
                        if (i == 0 || i == 1) //Los primeros jugadores de la partida son los de menor ELO
                            evnts[i].ExpectedContribution = 50 - (predBlue / 2);
                        else evnts[i].ExpectedContribution = 50 + (predBlue / 2);
                        evnts[i].winningChances = winningChances.Item1;
                    }

                    evnts[i].Send();

                    sortedList.RemoveAt(first);
                }

                numPlayers -= PLAYEROOM;
                map++;
            }
        }

        //Devolver al menu a los jugadores que no hacen falta
        foreach (KeyValuePair<int, infoMatchmaking> conection in sortedList)
        {
            NoGameFoundEvent evnt = NoGameFoundEvent.Create(conection.Value.playerConnection);
            evnt.Send();
        }
        sortedList.Clear();
        dataOfAllUsers.Clear();

        //SceneManager.LoadScene("BOLTMapa");
    }

    public override void Disconnected(BoltConnection connection)
    {
        BoltLauncher.StartClient();

        BoltMatchmaking.JoinSession(sessionID);
        BoltLog.Warn("Session joined");

        base.Disconnected(connection);
    }


    // ----------------------------------  EVENTOS SERVER  --------------------------------------------

    public override void OnEvent(JoinPlayerEvent evnt) //LO RECIBE EL SERVER
    {
        // DATA PLAYER ==> MATCHMAKING
        infoMatchmaking playerData = new infoMatchmaking(evnt.elo, evnt.playerRol, evnt.RaisedBy);

        dataOfAllUsers.Add(new KeyValuePair<int, infoMatchmaking>(connections, playerData));

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
        RoundData.expectedContribution = evnt.ExpectedContribution;

        if (evnt.isRed)
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
