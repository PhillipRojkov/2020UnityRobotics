using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerWeapon playerWeapon = player.GetComponent<PlayerWeapon>();
            if (playerWeapon.shotgunAmmo < 8 && playerWeapon.hasShotgun)
            {
                playerWeapon.shotgunAmmo++; //add to ammo if it is less than 8

                if (playerWeapon.selectedWeapon == 1)
                {
                    playerWeapon.ShotgunAmmoCheck(); //update visuals if the shotgun is equipped
                }

                Destroy(this.gameObject);
            }
        }
    }
}
