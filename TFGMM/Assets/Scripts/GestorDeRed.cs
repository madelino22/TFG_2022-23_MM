using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GestorDeRed : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void Button()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conectando...");
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JUGADOR NUMERO: " + PhotonNetwork.CountOfPlayers.ToString());
        int a = (PhotonNetwork.CountOfPlayers - 1) / 2;
        PhotonNetwork.JoinOrCreateRoom(a.ToString(), new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
        Debug.Log("Sala creada numero: " + a.ToString());
    }

    public override void OnJoinedRoom()
    {
        //if (PhotonNetwork.CountOfPlayers % 2 == 1)
        //{
        //    PhotonNetwork.Instantiate("Pala1", new Vector2(-8f, 0), Quaternion.identity);
        //}
        //else
        //{
        //    PhotonNetwork.Instantiate("Pala1", new Vector2(8f, 0), Quaternion.identity);
        //    PhotonNetwork.Instantiate("Bola", new Vector2(0, 0), Quaternion.identity);

        //}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player");
        Debug.Log("PLAYERS: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount % 2 != 1)
        {
            Debug.Log("2 Player");
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Carga Lvl");
                PhotonNetwork.LoadLevel(1);

            }
        }
    }
}
