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

    [SerializeField]
    public int damage = 300;
    Vector3 bulletEndDist;

    [SerializeField]
    bool wasFiredByRed = true;

    int creatorName = 0;

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

    public void setCreatorName(int n)
    {
        creatorName = n;
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
            int wasHitName = collision.gameObject.GetComponent<PlayerMotor>().getID();
            bool redWasHit = !wasFiredByRed; //rojo es golpeado si la bala la disparo azul
            if (BoltNetwork.IsServer)
                target.GetComponent<PlayerCallback>().loseLife(redWasHit, creatorName, wasHitName);
            BoltNetwork.Destroy(this.gameObject);
        }
    }
}
