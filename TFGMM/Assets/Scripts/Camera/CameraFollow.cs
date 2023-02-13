using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CameraFollow : MonoBehaviour
{

	public Transform target;

	public float smoothSpeed = 0.125f;
	Vector3 offset;


    private void Start()
    {
		offset =  transform.position - target.position;
    }
    void FixedUpdate()
	{
		if (!target) return;


		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = new Vector3(smoothedPosition.x, desiredPosition.y, smoothedPosition.z);
	}

}
//public class CameraFollow : MonoBehaviour
//{
//    [SerializeField]
//    GameObject player;

//    private float z;
//    private Camera cam;

//    // Start is called before the first frame update
//    void Start()
//    {
//        z = player.transform.position.z;
//        cam = this.gameObject.GetComponent<Camera>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (z != player.transform.position.z) //Se ha movido el player
//        {
//            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + player.transform.position.z - z);
//        }

//        //transform.LookAt(player.transform.position);

//        z = player.transform.position.z;
//    }
//}
