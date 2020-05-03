using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private DoorScript doorScript;
    public bool canOpen = true;
    public bool isCloseTrigger = false;
    public bool hasText = false;
    public GameObject text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canOpen)
        {
            if (!isCloseTrigger)
            {
                doorScript.triggerTouched = true;
            }
            else if (isCloseTrigger)
            {
                doorScript.closeTriggerTouched = true;
            }
            this.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Player") && canOpen == false)
        {
            if (hasText)
            {
                text.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasText)
        {
            text.SetActive(false);
        }
    }
}
