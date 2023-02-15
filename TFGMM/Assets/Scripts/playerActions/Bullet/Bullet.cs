using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Bullet : MonoBehaviour
{
    //[SerializeField]
    PlayerAttackTrail playerAttacking;

    [SerializeField]
    public float speed = 1;

    [SerializeField]
    public int damage = 300;
    Vector3 bulletEndDist;

    [SerializeField]
    bool wasFiredByRed = true;

    // Start is called before the first frame update
    void Start()
    {
        playerAttacking = GameObject.Find("AttackModule").GetComponent<PlayerAttackTrail>();
        bulletEndDist = transform.position + transform.forward * playerAttacking.getTrailDistance();
        //GetComponent<Rigidbody>().velocity = Vector3.forward * speed;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    { 
        if (BoltNetwork.IsClient)
        {
            Debug.Log("Bullet Collision");
            GameObject target = collision.gameObject;

            //COMPROBAR COLISION
            if (target.CompareTag("Muro")) 
                BoltNetwork.Destroy(this.gameObject);

            else if (wasFiredByRed && target.CompareTag("Blue") //Rojo le da a azul
                || !wasFiredByRed && target.CompareTag("Red")) // Azul le da a rojo
            {
                target.GetComponent<PlayerCallback>().loseLife();
                BoltNetwork.Destroy(this.gameObject);
            }






            /*//Enemy team
            if ((collision.gameObject.CompareTag("Red") && ComInfo.getTeam() == team.blue) ||
                (collision.gameObject.CompareTag("Blue") && ComInfo.getTeam() == team.red))
            {
                Debug.Log("Soy enemigo");
                HealthSystemComponent live = collision.gameObject.GetComponent<HealthSystemComponent>();

                damage = 300;

                //if() //Hacer a mano aqui los daños si hacemos varios personajes

                live.receiveDamage(damage);


            }
            //DESTROY BULLET
            Destroy(this.gameObject);
            Debug.Log("Me destruyo?");*/
        }

    }
}
