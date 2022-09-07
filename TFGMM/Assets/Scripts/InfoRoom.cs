using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InfoRoom : MonoBehaviourPunCallbacks
{

    [SerializeField]
    TextMesh texto;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conectando...");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JUGADOR NUMERO: " + PhotonNetwork.CountOfPlayers.ToString());
        int a = (PhotonNetwork.CountOfPlayers - 1) / 6;
        PhotonNetwork.JoinOrCreateRoom(a.ToString(), new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);
        Debug.Log("Sala creada numero: " + a.ToString());
    }

    public override void OnJoinedRoom()
    {
        texto.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player");
        texto.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        Debug.Log("PLAYERS: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount % 6 == 0)
        {
            Debug.Log("5 Player");
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Carga Lvl");
                PhotonNetwork.LoadLevel("PruebaRed");
            }
        }
    }
}
