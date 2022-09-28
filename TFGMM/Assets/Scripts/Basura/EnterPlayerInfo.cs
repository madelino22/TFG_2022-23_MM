using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnterPlayerInfo : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    bool pressed = false;

    float timePressed = 0;
    // Update is called once per frame
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
        Debug.Log("Pulsado");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Debug.Log("Levantado");

        if (timePressed < 0.1f)
        {
            SceneManager.LoadScene("PlayerInfo", LoadSceneMode.Single);
        }

        timePressed = 0;
    }
}
