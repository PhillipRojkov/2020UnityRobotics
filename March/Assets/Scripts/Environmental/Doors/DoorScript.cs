using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private float speed = 4;
    [SerializeField] private float distance = 10;
    private float newDistance;
    public bool triggerTouched = false;
    public bool closeTriggerTouched = false;
    private float opening;

    // Start is called before the first frame update
    void Start()
    {
        newDistance = distance + transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerTouched == true && opening == 0)
        {
            opening = 1;
            closeTriggerTouched = false;
        }

        if (opening == 1 && transform.position.y < newDistance)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        if (closeTriggerTouched && opening == 1)
        {
            opening = -1;
            triggerTouched = false;
        }

        if (opening == -1 && transform.position.y > newDistance - distance)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }
}
