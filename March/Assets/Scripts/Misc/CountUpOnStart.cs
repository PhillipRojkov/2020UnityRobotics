using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountUpOnStart : MonoBehaviour
{ //Used for the shotgun cone fire effect to scroll the material's texture up the object
    private float timeOnStart;
    private float customTime; //Counts up from 0 on start/enabling/instantiating of the object.

    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        timeOnStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        customTime = Time.time - timeOnStart;

        material.SetFloat("Vector1_1AB241C5", customTime); //Scroll the texture up the object by adding to the CustomTime float
    }
}
