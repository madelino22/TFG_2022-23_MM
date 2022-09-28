using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{

    public GameObject HealthbarPrefab;




    // Start is called before the first frame update
    IReadOnlyList<GameObject> playersList;
    List<GameObject> playersHealthBars;



    void Start()
    {
        
    }

    public void CreateHealthbarNewPlayer()
    {
        //instanciar healthbar
    }

    public void SetPlayersList(ref List<GameObject> playersLst)
    {
        playersList = playersLst;
    }


    void Update()
    {
        for(int x = 0; x < playersList.Count; x++)
        {
            if(x < playersHealthBars.Count)
            {
                playersHealthBars[x].transform.position = playersList[x].transform.position;
            }
        }        
    }
}
