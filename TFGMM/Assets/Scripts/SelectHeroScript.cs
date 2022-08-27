using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHeroScript : MonoBehaviour
{
    public GameObject[] heroes; //List of characters

    // Start is called before the first frame update
    void Start()
    {

        InstantiateHero();
    }

    public void InstantiateHero()
    {
        Instantiate(heroes[(int)ComInfo.getPlayerHero()], transform.position, transform.rotation, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
