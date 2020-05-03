using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickup : MonoBehaviour
{
    public TriggerScript triggerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerScript.canOpen = true;
            this.gameObject.SetActive(false);
        }
    }
}
