using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideHeroes : MonoBehaviour, IPointerMoveHandler
{
    public GameObject Heroes;

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2 slide = eventData.scrollDelta;
        Debug.Log("Me muevo Posicion X: " + slide.x);
        if (slide.x > 0)
        {
            Heroes.transform.Translate(5f, 0, 0);
        }
        else if(slide.x < 0)
        {
            Heroes.transform.Translate(-5f, 0, 0);
        }
    }
}
