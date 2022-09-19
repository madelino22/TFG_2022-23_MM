using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    [SerializeField]
    public Transform respawnPosition;

    [SerializeField]
    public GameObject emptyPlayer;

    [SerializeField]
    public GameObject canvasRespawn;

    [SerializeField]
    public GameObject playerUI;

    [SerializeField]
    public Text countdown;

    public float respawnTime = 5;

    private float timer = 0;



    private void OnEnable()
    {
        emptyPlayer.SetActive(false);
        playerUI.SetActive(false);

        timer = respawnTime;

        canvasRespawn.SetActive(true);
    }

    private void OnDisable()
    {
        this.gameObject.GetComponent<HealthSystemComponent>().recoverMaxLife();

        emptyPlayer.transform.position = respawnPosition.position;
        emptyPlayer.SetActive(true);
        playerUI.SetActive(true);

        timer = 0;

        canvasRespawn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            this.enabled = false;
        }
        else
        {
            int aux = (int)timer + 1;
            countdown.text = aux.ToString();
        }

    }
}
