using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private MenuManagerScript menuManagerScript;
    [SerializeField] private Slider healthBar;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            PlayerDeath();
        }

        healthBar.value = health;
    }

    void PlayerDeath()
    {
        gameOverText.SetActive(true);
        PlayerMove playerMove = GetComponent<PlayerMove>();
        playerMove.enabled = false;
        PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();
        playerWeapon.enabled = false;

        menuManagerScript.externalPause = true;
    }
}
