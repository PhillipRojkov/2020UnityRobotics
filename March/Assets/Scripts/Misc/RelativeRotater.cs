using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeRotater : MonoBehaviour
{
    public float rotateSpeed = 1;
    public bool up;
    public bool right;
    public bool forward;
    private Vector3 rotateAxis;

    // Start is called before the first frame update
    void Start()
    {
        if (up)
        {
            rotateAxis = transform.up;
        }
        if (right)
        {
            rotateAxis = transform.right;
        }
        if (forward)
        {
            rotateAxis = transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, rotateAxis, rotateSpeed); ;        
    }
}
