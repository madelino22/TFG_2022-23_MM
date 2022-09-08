using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;


    public PlayersManager playersManager;

    List<GameObject> playersList;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();

            }
            return _instance;
        }
    }


    private void Start()
    {
        playersList = new List<GameObject>();
    }

    public void AddNewPlayerToList(GameObject nPlayer)
    {
        playersList.Add(nPlayer);
    }

}