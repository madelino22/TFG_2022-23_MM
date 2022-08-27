using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CodeMonkey.HealthSystemCM {

    /// <summary>
    /// Adds a HealthSystem to a Game Object
    /// </summary>
    public class HealthSystemComponent : MonoBehaviour, IGetHealthSystem {

        [Tooltip("Maximum Health amount")]
        [SerializeField] private float healthAmountMax = 100f;

        [Tooltip("Starting Health amount, leave at 0 to start at full health.")]
        [SerializeField] private float startingHealthAmount;

        private HealthSystem healthSystem;

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
            healthSystem = new HealthSystem(healthAmountMax);

            if (startingHealthAmount != 0) {
                healthSystem.SetHealth(startingHealthAmount);
            }

            //Testing Bar
            healthSystem.Damage(50);
            cured = false;
        }

        private void Update()
        {
            //Timer for recover life
            if(!cured && !receivingDamage && healthSystem.GetHealth() != healthAmountMax)
            {
               
                timer += Time.deltaTime;
                if(alreadyhealing && timer >= 1f) //Heal not first time
                {
                    healthSystem.Heal((healthAmountMax / 100) * 7);
                    timer = 0;
                    if (healthSystem.GetHealth() == healthAmountMax) cured = true;
                }
                else if (!alreadyhealing && timer >= recoverLifeTime) //Heal first time
                {
                    healthSystem.Heal((healthAmountMax / 100) * 7);
                    timer = 0;
                    if (healthSystem.GetHealth() == healthAmountMax)
                    {
                        cured = true;
                    }
                    else alreadyhealing = true;
                }
            }

            if(receivingDamage)
            {
                cured = false;
                alreadyhealing = false;
                timer = 0;
                receivingDamage = false;
            }
        }

        /// <summary>
        /// Get the Health System created by this Component
        /// </summary>
        public HealthSystem GetHealthSystem() {
            return healthSystem;
        }


    }

}