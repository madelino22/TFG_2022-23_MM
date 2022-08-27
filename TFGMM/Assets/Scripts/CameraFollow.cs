using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    private float z;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        z = player.transform.position.z;
        cam = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (z != player.transform.position.z) //Se ha movido el player
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + player.transform.position.z - z);
        }

        //transform.LookAt(player.transform.position);

        z = player.transform.position.z;
    }
}
