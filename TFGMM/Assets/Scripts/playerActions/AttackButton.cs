using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    public GameObject player;

    float reloadTime;

    private float timer = 0;

    private bool pressing = false;

    private bool alreadyPressing = false;

    private bool useAttack = false;

    private bool isAttacking = false; //For this player just

    private float speedChange = 1f; //For this player just

    private Vector3 normalScale;

    private Vector3 minorScale;

    private Vector3 color;

    private Image image;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (timer >= reloadTime && !isAttacking)
        {
            pressing = true;
            alreadyPressing = true;
            Debug.Log("Pulsado");
        }
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
        if (pressing && alreadyPressing) useAttack = true;
        pressing = false;
        alreadyPressing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        reloadTime = 7f;

        normalScale = this.gameObject.transform.localScale;

        minorScale = normalScale * 0.9f;

        image = this.gameObject.GetComponent<Image>();
        color = new Vector3(image.color.r, image.color.g, image.color.b);
        image.color = new Vector4(color.x, color.y, color.z, 0); //0 trasparent - 1 full
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (image.color.a != 1 && !isAttacking) //if its not full yet
        {
            float alpha = timer / reloadTime;
            if (alpha > 1) alpha = 1;
            image.color = new Vector4(color.x, color.y, color.z, alpha); //0 trasparent - 1 full
        }

        if (useAttack) //Use the attack and start the cooldown of the button
        {
            player.GetComponent<PlayerMov>().BuffSpeed(speedChange);
            isAttacking = true;

            //Reset button
            timer = 0;
            image.color = new Vector4(color.x, color.y, color.z, 0); //0 trasparent - 1 full
            this.gameObject.transform.localScale = normalScale;
            useAttack = false;
        }
        else if (isAttacking && timer >= 5f)
        {
            player.GetComponent<PlayerMov>().DeBuffSpeed(speedChange);

            //Reset attack
            isAttacking = false;
            timer = 0;
        }
    }
}
