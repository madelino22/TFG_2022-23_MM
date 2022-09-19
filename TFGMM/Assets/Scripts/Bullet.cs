using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField]
    PlayerAttackTrail playerAttacking;

    [SerializeField]
    public float speed = 1;

    [SerializeField]
    public int damage = 300;
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

        Destroyable other = collision.gameObject.GetComponent<Destroyable>();
      
        
        
        object[] array = { other.gameObject };
        GameManager.Instance.playersManager.PlayerHit(array);




        //Enemy team
        if ((collision.gameObject.CompareTag("Red Team") && ComInfo.getTeam() == team.blue) || 
            (collision.gameObject.CompareTag("Blue Team") && ComInfo.getTeam() == team.red))
        {
            Debug.Log("Soy enemigo");
            HealthSystemComponent live = collision.gameObject.GetComponent<HealthSystemComponent>();

            damage = 300;

            //if() //Hacer a mano aqui los daños si hacemos varios personajes

            live.receiveDamage(damage);

            
        }
        else if(other != null) //Destroyable wall
        {
            Destroy(collision.gameObject);
        }
        //DESTROY BULLET
        Destroy(this.gameObject);        
    }
}
