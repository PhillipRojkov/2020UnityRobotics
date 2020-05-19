using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstShotgunPickupScript : MonoBehaviour
{ //Used in Level2 when the player recieves the shotgun upgrade for their blaster
    public PlayerWeapon playerWeaponScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerWeaponScript.hasShotgun = true; //Enable the shotgun
            playerWeaponScript.shotgunAmmo = 8; //Set shotgun ammo to the maximum (8)

            playerWeaponScript.SwapWeapon(); //Swap the weapon to the shotgun

            this.gameObject.SetActive(false); //Deactivate this object
        }
    }
}
