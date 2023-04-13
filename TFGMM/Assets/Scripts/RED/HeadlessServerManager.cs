using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Bolt.Matchmaking;
using Photon.Bolt;
using Photon.Bolt.Utils;


enum ServerMode {DEFAULT, ROOM0, ROOM1};
public class HeadlessServerManager : GlobalEventListener
{
    [SerializeField] ServerMode serverMode = ServerMode.DEFAULT;

    public static int roomNumber = -1;

    private string _map = "";
    private static string s_map;

    private string _roomID = "Test";
    private static string s_roomID;

    [SerializeField]
    private bool _isServer = false;

    public bool IsServer { get => _isServer; set => _isServer = value; }

    public static string RoomID()
    {
        return s_roomID;
    }

    public static string Map()
    {
        return s_map;
    }

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            PhotonRoomProperties roomProperties = new PhotonRoomProperties();

            roomProperties.AddRoomProperty("m", _map); // ex: map id

            roomProperties.IsOpen = true;
            roomProperties.IsVisible = true;

            if (s_roomID.Length == 0)
            {
                s_roomID = Guid.NewGuid().ToString();
            }

            BoltMatchmaking.CreateSession(
                sessionID: s_roomID,
                token: roomProperties,
                sceneToLoad: _map
            );
        }
    }

    private void Awake()
    {

        switch (serverMode)
        {
            case ServerMode.DEFAULT:
                _map = "Searching";
                _roomID = "Test";
                break;
            case ServerMode.ROOM0:
                _map = "BOLTMapa";
                _roomID = "0";
                roomNumber = 0;
                break;
            case ServerMode.ROOM1:
                _map = "BOLTMapa";
                _roomID = "1";
                roomNumber = 1;
                break;
        }


        _isServer = "true" == (GetArg("-s", "-isServer") ?? (_isServer ? "true" : "false"));
        s_map = GetArg("-m", "-map") ?? _map;
        s_roomID = GetArg("-r", "-room") ?? _roomID;

        if (IsServer)
        {
            var validMap = false;

            foreach (string value in BoltScenes.AllScenes)
            {
                if (SceneManager.GetActiveScene().name != value)
                {
                    if (s_map == value)
                    {
                        validMap = true;
                        break;
                    }
                }
            }

            if (!validMap)
            {
                BoltLog.Error("Invalid configuration: please verify level name");
                Application.Quit();
            }

            BoltLauncher.StartServer();
            DontDestroyOnLoad(this);
        }
    }

    static string GetArg(params string[] names)
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            foreach (var name in names)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return null;
    }
}