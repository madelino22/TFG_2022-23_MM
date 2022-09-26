using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Image
using Photon.Pun;

public class PlayersManager : MonoBehaviour
{
    [SerializeField]
    Joystick mov;

    [SerializeField]
    Joystick att;


    [SerializeField]
    GameObject[] spawns;


    //SPECIAL ATTACK
    [SerializeField]
    Joystick speAtt;
    [SerializeField]
    Image joystickImage;

    [SerializeField]
    Image handleImage;

    List<GameObject> playersList;



    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        playersList = new List<GameObject>();
        view = GetComponent<PhotonView>();

        GameObject player;
        Debug.Log("ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        //player = PhotonNetwork.Instantiate("Character1", new Vector3(spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.x,
        //    spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.y+1, spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1 + 2].transform.position.z), Quaternion.identity);
        //GameManager.Instance.AddNewPlayerToList(player);
        //playersList.Add(player);

        Debug.Log("CAMARA ASIGNADA TARGET");

        if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
        {
            Debug.Log("Jugador contrario");
            Camera.main.transform.Rotate(new Vector3(140, 180, Camera.main.transform.rotation.z));
            Camera.main.GetComponent<CameraFollow>().offset.z = 5;
            player = PhotonNetwork.Instantiate("Character1", new Vector3(spawns[1].transform.position.x,
           spawns[1].transform.position.y + 1, spawns[1].transform.position.z),
           new Quaternion(transform.rotation.x,transform.rotation.y,transform.rotation.z,1));
        }
        else
            player = PhotonNetwork.Instantiate("Character1", new Vector3(spawns[0].transform.position.x,
           spawns[0].transform.position.y + 1, spawns[0].transform.position.z), Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().target = player.GetComponentInChildren<PlayerMov>().gameObject.transform;
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    player = PhotonNetwork.Instantiate("Character1", new Vector3(-8f, 1, 0), Quaternion.identity);
        //}
        //else player = PhotonNetwork.Instantiate("Character1", new Vector3(8f, 1, 0), Quaternion.identity);

        player.GetComponentInChildren<PlayerMov>().GetJoystick(mov);
        player.GetComponentInChildren<PlayerAttackTrail>().GetJoystick(att);
        
        //SPECIAL ATTACK
        player.GetComponentInChildren<SpecialShooter>().GetJoystick(speAtt);
        ActivateSpecialModule ASM = player.GetComponentInChildren<ActivateSpecialModule>();
        ASM.GetJoystick(speAtt);
        ASM.setImage(joystickImage);
        ASM.setHandleImage(handleImage);
        //NORMAL ATTACK
        BasicShooter bS = player.GetComponentInChildren<BasicShooter>();
        bS.GetJoystick(att);
        bS.setSpecialModule(ASM);
    }

    // Update is called once per frame
    void Update()
    {
    }
   

    public void AddNewPlayerToList(GameObject nPlayer)
    {
        playersList.Add(nPlayer);
    }


    public void PlayerHit(object[] arr)
    {
        Debug.Log("Envia mensaje");
        view.RPC("PlayerHitCallable", RpcTarget.Others, arr);
    }

    [PunRPC]
    private void PlayerHitCallable(object[] arr)
    {
        //Two options
        //First to pass the gameObject and the hit points and see if the gameObject value is the same on every PC
        //Second to pass the index of the playerList arrray and the hit point, but coution with cohesion of indexes on every PC

        //First
        ((GameObject)arr[0]).SetActive(false);
    }

}
