using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Bullet : MonoBehaviour
{
    //[SerializeField]
    PlayerAttackTrail playerAttacking;

    [SerializeField]
    public float speed = 10f;

    AudioSource shootSound;

    [SerializeField]
    public int damage = 300;
    Vector3 bulletEndDist;

    [SerializeField]
    bool wasFiredByRed = true;

    int creatorID = 0;

    // Start is called before the first frame update
    void Start()
    {
        shootSound = GetComponent<AudioSource>();
        playerAttacking = GameObject.Find("AttackModule").GetComponent<PlayerAttackTrail>();
        bulletEndDist = transform.position + transform.forward * playerAttacking.getTrailDistance();
        if (!BoltNetwork.IsServer)
            shootSound.Play();
        //GetComponent<Rigidbody>().velocity = Vector3.forward * speed;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }

    public void setCreatorID(int n)
    {
        creatorID = n;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Bullet Collision");
        GameObject target = collision.gameObject;

        //COMPROBAR COLISION
        if (target.CompareTag("Muro"))
            BoltNetwork.Destroy(this.gameObject);

        else if (wasFiredByRed && target.CompareTag("Blue") //Rojo le da a azul
            || !wasFiredByRed && target.CompareTag("Red")) // Azul le da a rojo
        {
            PlayerMotor pMotor = collision.gameObject.GetComponent<PlayerMotor>();
            if (BoltNetwork.IsServer)
            {
                int wasHitID = pMotor.getID();
                bool redWasHit = !wasFiredByRed; //rojo es golpeado si la bala la disparo azul

                Debug.Log("AUX: Spawneo la bala" + creatorID + " se le dio a " + wasHitID);
                target.GetComponent<PlayerCallback>().loseLife(redWasHit, creatorID, wasHitID);
            }
            else
            {
                pMotor.Hurt();
            }
            BoltNetwork.Destroy(this.gameObject);
        }
        else if (wasFiredByRed && target.CompareTag("Red") //Rojo le da a azul
           || !wasFiredByRed && target.CompareTag("Blue")) // Azul le da a rojo
        {
            PlayerMotor pMotor = collision.gameObject.GetComponent<PlayerMotor>();
            int wasHitID = pMotor.getID();

            if (BoltNetwork.IsServer && creatorID != wasHitID) //no me curo a mi mismo
            {
                bool redWasHit = !wasFiredByRed; //rojo es golpeado si la bala la disparo azul

                Debug.Log("AUX: Spawneo la bala" + creatorID + " curo a " + wasHitID);
                target.GetComponent<PlayerCallback>().addLife(redWasHit, creatorID, wasHitID);
                BoltNetwork.Destroy(this.gameObject);   //no se destruye al colisionar con el que la spawnea
            }

        }
    }
}
