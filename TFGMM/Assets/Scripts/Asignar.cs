using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Asignar : MonoBehaviour
{
    [SerializeField]
    Joystick mov;

    [SerializeField]
    Joystick att;

    [SerializeField]
    GameObject[] spawns;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player;
        Debug.Log("ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        player = PhotonNetwork.Instantiate("Character1", spawns[PhotonNetwork.LocalPlayer.ActorNumber].transform.position, Quaternion.identity);
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    player = PhotonNetwork.Instantiate("Character1", new Vector3(-8f, 1, 0), Quaternion.identity);
        //}
        //else player = PhotonNetwork.Instantiate("Character1", new Vector3(8f, 1, 0), Quaternion.identity);

        player.GetComponentInChildren<PlayerMov>().GetJoystick(mov);
        player.GetComponentInChildren<PlayerAttackTrail>().GetJoystick(att);
        BasicShooter bS = player.GetComponentInChildren<BasicShooter>();
        bS.GetJoystick(att);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
