using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateSpecialModule : MonoBehaviour
{
    //Hasta que se pueda disparar, estaran desactivados los siguientes componentes:
    //- El Script FixedJoystick esta desactivado hasta que se pueda disparar
    //- Hay que ir cambiando el alpha cada vez que se dispare

    [SerializeField]
    bool active = false;

    [SerializeField]
    Joystick specialJoystick;

    [SerializeField]
    Color joystickColor;

    [SerializeField]
    Image joystickImage;

    [SerializeField]
    Image handleImage;

    private bool shoot = false;

    // Start is called before the first frame update
    void Start()
    {
        Activate(active);
    }

    // Update is called once per frame
    void Update()
    {
        if (specialJoystick != null && active)
        {
            //PREPARE SHOOTER
            if (Mathf.Abs(specialJoystick.Horizontal) > 0.1f || Mathf.Abs(specialJoystick.Vertical) > 0.1f)
            {
                if (!shoot) shoot = true;
            }
            //JOYSTICK WAS RELEASED
            else if (shoot && Input.GetMouseButtonUp(0))
            {
                shoot = false;
                Activate(false);
            }
        }
    }

    public void IncreaseAplha(float numBullets)
    {
        //CALCULATE ALPHA
        Debug.Log(joystickColor.a + "+" + 1 / numBullets);
        joystickColor.a = joystickColor.a + 1 / numBullets;
        if (joystickColor.a > 1) joystickColor.a = 1;
        else if (joystickColor.a < 0) joystickColor.a = 0;

        //CHANGE COLOR
        joystickImage.color = joystickColor;
        handleImage.color = joystickColor;
    }

    public void Activate(bool a)
    {
        active = a;

        if (!active)
        {
            joystickColor.a = 0;
            specialJoystick.enabled = false;
        }
        else
        {
            joystickColor.a = 1;
            specialJoystick.enabled = true;
            Debug.Log("Special Attack Activated");
        }

        joystickImage.color = joystickColor;
        handleImage.color = joystickColor;
    }
}
