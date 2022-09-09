using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersManager : MonoBehaviour
{
    [SerializeField]
    Joystick mov;

    [SerializeField]
    Joystick att;

    [SerializeField]
    GameObject[] spawns;

    [SerializeField]
    GameObject mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("mapiar");

            spawns = GameObject.FindGameObjectsWithTag("Spawn");
            for(int x=0; x < spawns.Length; x++)
            {
                Debug.Log("Hola" + spawns[x].name);
            }

        Debug.Log(spawns.Length);
        GameObject player;
        Debug.Log("ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        player = PhotonNetwork.Instantiate("Character1", new Vector3(spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.x,
            spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.y+1, spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.z), Quaternion.identity);
        //GameManager.Instance.AddNewPlayerToList(player);


        Debug.Log("CAMARA ASIGNADA TARGET");

        if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
        {
            Camera.main.transform.Rotate(new Vector3(90, 180, Camera.main.transform.rotation.z));
            Camera.main.GetComponent<CameraFollow>().offset.z = 15;
        }
        Camera.main.GetComponent<CameraFollow>().target = player.GetComponentInChildren<PlayerMov>().gameObject.transform;
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
