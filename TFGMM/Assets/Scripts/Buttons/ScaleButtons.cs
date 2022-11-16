using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{

    private bool pressing = false;

    private bool alreadyPressing = false;

    private Vector3 normalScale;

    private Vector3 minorScale;

    private Vector3 color;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        normalScale = this.gameObject.transform.localScale;

        minorScale = normalScale * 0.9f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        pressing = true;
        alreadyPressing = true;
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pressing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pressing = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressing = false;
        alreadyPressing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pressing && alreadyPressing && this.gameObject.transform.localScale != minorScale) //scale the button a little
        {

            this.gameObject.transform.localScale -= normalScale * Time.deltaTime * 1.5f;

            if (this.gameObject.transform.localScale.x < minorScale.x) this.gameObject.transform.localScale = minorScale;
        }
        else if (!alreadyPressing && this.gameObject.transform.localScale != normalScale) //scale the button if not normal
        {
            this.gameObject.transform.localScale += normalScale * Time.deltaTime * 1.5f;

            if (this.gameObject.transform.localScale.x > normalScale.x) this.gameObject.transform.localScale = normalScale;
        }
    }
}
