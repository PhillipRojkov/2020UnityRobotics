using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    public float healthIncrease = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth.health < 100)
            {
                if (playerHealth.health >= 100 - healthIncrease) //This is so that player health doesn't exceed 100
                {
                    playerHealth.health = 100;
                } else if (playerHealth.health < 70)
                {
                    playerHealth.health += healthIncrease;
                }

                Destroy(this.transform.parent.gameObject);
            }
        }
    }
}
