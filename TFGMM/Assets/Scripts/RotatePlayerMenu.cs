using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RotatePlayerMenu : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    Touch touch;

    bool pressed = false;

    float timePressed = 0;

    [SerializeField]
    Transform heroHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("Me muevo");
                    if (touch.deltaPosition.x > 0)
                    {
                        heroHolder.Rotate(0, 5f, 0);
                    }
                    else
                    {
                        heroHolder.Rotate(0, -5f, 0);
                    }
                }
            }

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

        if(timePressed < 0.1f)
        {
            SceneManager.LoadScene("ChangeHero", LoadSceneMode.Single);
        }

        timePressed = 0;
    }
}
