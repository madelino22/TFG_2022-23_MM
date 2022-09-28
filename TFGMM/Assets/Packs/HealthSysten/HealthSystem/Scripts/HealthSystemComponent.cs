using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    /// <summary>
    /// Adds a HealthSystem to a Game Object
    /// </summary>
    public class HealthSystemComponent : MonoBehaviour {

        [Tooltip("Maximum Health amount")]
        [SerializeField] private float healthAmountMax = 100f;

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
        [SerializeField] private GameObject lifeText;

        private void Awake() {
            // Create Health System
            health = startingHealthAmount;
        }

        private void Update()
        {
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
                if(alreadyhealing && timer >= 1f) //Heal not first time
                {
                    health += (healthAmountMax / 100) * 7;
                    timer = 0;
                    if (health >= healthAmountMax)
                    {
                        health = startingHealthAmount;
                        cured = true;
                    }
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

        //if (health <= 0) this.gameObject.GetComponent<Respawn>().enabled = false;
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

