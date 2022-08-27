using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField]
    PlayerAttackTrail playerAttacking;

    [SerializeField]
    public float speed = 1;

    Vector3 bulletEndDist;

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
        transform.Translate(Vector3.forward * speed*Time.fixedDeltaTime);
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Bullet Collision");

        ////CAN BE DESTROYED
        Destroyable other = collision.gameObject.GetComponent<Destroyable>();
        //ENEMY
        if (other != null) //BETTER THAN USING TAGS
        {
            //DECREASE ENEMY HEALTH
            Destroy(collision.gameObject);
        }
        //DESTROY BULLET
        Destroy(this.gameObject);        
    }
}
