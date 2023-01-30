
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Este ya no se usa

public class PlayerMov : MonoBehaviour
{
    [SerializeField]
    Joystick joystick;

    [SerializeField]
    Transform playerBall;

    [SerializeField]
    float movementLimit = 0.05f;

    [SerializeField]
    float speed = 1;


    public GameObject healtSystemBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GetJoystick(Joystick a)
    {
        joystick = a;
    }

    public Joystick GetJoystickMov()
    {
        return joystick;
    }

    public void BuffSpeed(float buff)
    {
        Debug.Log("Speed buffed");
        speed += buff;
    }
    public void DeBuffSpeed(float buff)
    {
        Debug.Log("Speed Debuffed");
        speed -= buff;
    }


    public void DisableJoysticks()
    {
        joystick.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update");
        if (joystick != null)
        {
            //Debug.Log("Que ase");   
            //Debug.Log("Moviendose...");
            //Mover bola inferior
            //if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
            //{
            //    playerBall.position = new Vector3(-joystick.Horizontal + transform.position.x, transform.position.y, -joystick.Vertical + transform.position.z);

            //    //Mirar hacia adelante
            //    transform.LookAt(new Vector3(playerBall.position.x, 0, playerBall.position.z));
            //    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //    Debug.Log("Mi posicion: " + playerBall.position);               
            //}
            //else
            //{
            //    playerBall.position = new Vector3(joystick.Horizontal + transform.position.x, transform.position.y, joystick.Vertical + transform.position.z);

            //    //Mirar hacia adelante
            //    transform.LookAt(new Vector3(playerBall.position.x, 0, playerBall.position.z));
            //    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //    Debug.Log("Rotar");
            //}


            //Mover jugador
            //if (joystick.Horizontal > movementLimit || -movementLimit > joystick.Horizontal || joystick.Vertical > movementLimit || -movementLimit > joystick.Vertical)
            //{
            //    Debug.Log("Mover");

            //    float value = 1; //En funcion de la distancia mas o menos velocidad

            //    float movAux = transform.position.x;

            //    //if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
            //    //    transform.Translate(Vector3.back * Time.deltaTime * speed * value);
            //    //else
            //        transform.Translate(Vector3.forward * Time.deltaTime * speed * value);

            //    movAux -= transform.position.x;

            //    //healtSystemBar.transform.position = new Vector3(healtSystemBar.transform.position.x + movAux, healtSystemBar.transform.position.y, healtSystemBar.transform.position.z);
            //}
            //else
            //{
            //    Debug.Log("MovementLimit " + movementLimit);
            //    Debug.Log("Joystick " + joystick.Horizontal + " " + joystick.Vertical);
            //}
        }
    }
}