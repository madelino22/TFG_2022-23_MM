using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    AudioSource audioData;


    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void returnToMenu()
    {
        audioData.Play(0);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void returnFromInfo()
    {
        audioData.Play(0);
        SceneManager.LoadScene("ChangeHero", LoadSceneMode.Single);
    }

    public void heroChosen()
    {
        audioData.Play(0);
        ComInfo.setHero((heroes)LoadInfo.HeroExp);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void openFriends()
    {
        audioData.Play(0);
        SceneManager.LoadScene("addFriends", LoadSceneMode.Additive);
    }

    public void goSearch()
    {
        audioData.Play(0);
        SceneManager.LoadScene("Searching", LoadSceneMode.Single);
    }
}
