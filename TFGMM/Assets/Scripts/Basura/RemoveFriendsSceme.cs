using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RemoveFriendsSceme : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    bool pressed = false;

    float timePressed = 0;

    void Update()
    {
        if (pressed)
        {
            timePressed += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (timePressed < 0.1f)
        {
            SceneManager.UnloadSceneAsync("addFriends");

            SceneManager.UnloadSceneAsync("Lobby");
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
    }
}
