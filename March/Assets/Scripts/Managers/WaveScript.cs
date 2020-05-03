using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    public int deathsForNextWave = 1; //Deaths from this wave that must occur for the next wave to start
    public int enemyDeaths;
    public float timeBetweenWaves = 2;

    [SerializeField] private GameObject nextWave;
    [SerializeField] private bool isFinalWave = false;
    [SerializeField] private DoorScript doorScript;
    [SerializeField] private bool setNextAreaWave = false; //Spawn enemies in the next area
    [SerializeField] private GameObject nextAreaWave;

    private float t = 0; //Time for time between waves logic

    // Update is called once per frame
    void Update()
    {
        if (enemyDeaths >= deathsForNextWave) //Start setting the next wave
        {
            if (isFinalWave)
            {
                doorScript.triggerTouched = true; //Open door at end of level
                if (setNextAreaWave)
                {
                    nextAreaWave.SetActive(true); //Activate the next area
                }
                this.enabled = false; //Disable script
            }
            else
            {
                t += Time.deltaTime;
                if (t >= timeBetweenWaves) //Create a delay between waves
                {
                    nextWave.SetActive(true); //Activate next wave
                    this.enabled = false; //Disable script
                }
            }
        }
    }
}
