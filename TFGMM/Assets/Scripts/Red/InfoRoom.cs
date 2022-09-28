using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMesh texto;

    [SerializeField]
    TextMesh textoTotal;

    [SerializeField]
    int nPersonasMax;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conectando...");
        PhotonNetwork.AutomaticallySyncScene = true;
        textoTotal.text = "/ " + nPersonasMax.ToString();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JUGADOR NUMERO: " + PhotonNetwork.CountOfPlayers.ToString());
        int a = (PhotonNetwork.CountOfPlayers - 1) / nPersonasMax;
        PhotonNetwork.JoinOrCreateRoom(a.ToString(), new RoomOptions { MaxPlayers = (byte) nPersonasMax }, TypedLobby.Default);
        Debug.Log("Sala creada numero: " + a.ToString());
    }

    public void ButtonPress()
    {
        Debug.Log("Saliendo Room");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Se fue");
        texto.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
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
        if (PhotonNetwork.CurrentRoom.PlayerCount % nPersonasMax == 0)
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
