using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueCardGunPickup : MonoBehaviour
{
    public TriggerScript triggerScript;
    public GameObject gun;
    public PlayerWeapon playerWeapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerScript.canOpen = true;
            gun.SetActive(true);
            playerWeapon.enabled = true;

            this.gameObject.SetActive(false);
        }
    }
}
