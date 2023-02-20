using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    Camera cam;

    [SerializeField]
    Camera cam2;


    // Start is called before the first frame update
    void Start()
    {
        //if (cam2 == null)
        //    cam = Camera.main;
        //else
        //    cam = cam2;

        Invoke("GetCurrentCamera", 0.5f);

    }

    private void GetCurrentCamera()
    {
        cam = Camera.current;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
        //transform.LookAt(2 * transform.position - cam.transform.position);

    }
}
