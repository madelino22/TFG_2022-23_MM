using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void returnFromHeros()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void returnFromInfo()
    {
        SceneManager.LoadScene("ChangeHero", LoadSceneMode.Single);
    }

    public void heroChosen()
    {
        ComInfo.setHero((heroes)LoadInfo.HeroExp);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void openFriends()
    {
        SceneManager.LoadScene("addFriends", LoadSceneMode.Additive);
    }
}
