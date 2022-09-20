using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using TMPro;


/// <summary>
/// Adds a HealthSystem to a Game Object
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Tooltip("Part of the player wont be disabled")]
    [SerializeField] private GameObject playerTop;

    [Tooltip("Maximum Health amount")]
    [SerializeField] private float healthAmountMax = 100f;

    [SerializeField] Image healthBar;
    float initialHBFill;

    private float health;
    [Tooltip("Starting Health amount, leave at 0 to start at full health.")]
    [SerializeField] private float startingHealthAmount;

    private bool receivingDamage = false;

    private bool alreadyhealing = false;

    private float timer = 0;

    private bool cured = true;

    [Tooltip("Time Before Reovering Life")]
    [SerializeField] private float recoverLifeTime;

    [Tooltip("Starting Health amount, leave at 0 to start at full health.")]
    [SerializeField] private TMP_Text lifeText;
    

    private void Awake()
    {
        // Create Health System
        health = startingHealthAmount;
        initialHBFill = healthBar.fillAmount;

    }
    private void Start()
    {
        UpdateHealthbarUI();

    }


    private void UpdateHealthbarUI()
    {
        if (healthBar)
        {
            lifeText.text = health.ToString();
            healthBar.fillAmount = health / healthAmountMax * initialHBFill;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) receiveDamage(20);

        if (receivingDamage)
        {
            cured = false;
            alreadyhealing = false;
            timer = 0;
            receivingDamage = false;
        }

        //Timer for recover life
        if (!cured && !receivingDamage && health >= healthAmountMax)
        {

            timer += Time.deltaTime;
            if (alreadyhealing && timer >= 1f) //Heal not first time
            {
                health += (healthAmountMax / 100) * 7;
                timer = 0;
                if (health >= healthAmountMax)
                {
                    health = startingHealthAmount;
                    cured = true;
                }
                UpdateHealthbarUI();
            }
            else if (!alreadyhealing && timer >= recoverLifeTime) //Heal first time
            {
                health += (healthAmountMax / 100) * 7;
                timer = 0;
                if (health >= healthAmountMax)
                {
                    health = startingHealthAmount;
                    cured = true;
                }
                else alreadyhealing = true;

                UpdateHealthbarUI();

            }
        }


       
    }

    /// <summary>
    /// Get the Health System created by this Component
    /// </summary>

    public void receiveDamage(int damage)
    {
        health -= damage;
        receivingDamage = true;
        UpdateHealthbarUI();
        if (health <= 0) playerTop.gameObject.GetComponent<Respawn>().enabled = true;
    }

    public void recoverMaxLife()
    {
        health = startingHealthAmount;
        cured = true;
        alreadyhealing = false;
        timer = 0;
        receivingDamage = false;
    }
}

