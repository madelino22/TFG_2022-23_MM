using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroInfo : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public heroes HeroName;

    public GameObject heroInfoCanvas;

    public GameObject changeHeroCanvas;

    bool pressed = false;

    float timePressed = 0;

    // Update is called once per frame
    void Update()
    {
        
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
            heroInfoCanvas.SetActive(true);
            LoadInfo.HeroExp = (int) HeroName;
            changeHeroCanvas.SetActive(false);
        }

        timePressed = 0;
    }
}
