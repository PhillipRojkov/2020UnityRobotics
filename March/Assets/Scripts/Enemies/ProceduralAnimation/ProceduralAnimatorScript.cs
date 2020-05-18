using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimatorScript : MonoBehaviour
{
    [SerializeField] private GameObject target1;
    [SerializeField] private GameObject target2;
    [SerializeField] private GameObject target3;
    [SerializeField] private GameObject target4;
    private GameObject[] targets = new GameObject[4];

    [SerializeField] private GameObject movingTarget1;
    [SerializeField] private GameObject movingTarget2;
    [SerializeField] private GameObject movingTarget3;
    [SerializeField] private GameObject movingTarget4;
    private GameObject[] movingTargets = new GameObject[4];

    private float[] distance = new float[4]; //Distance from target to movingTarget
    [SerializeField] private float maxDistance = 1f; //Max distance after which the target will move to moving target

    private Vector3[] currentTargetPos = new Vector3[4]; //Used for linearly sliding between two target positions
    private bool[] legisMoving = new bool[4];

    [SerializeField] private int groundLayer = 9; //layer mask so the legs don't try to stand on top of eachother. Layer 9 is ground

    [SerializeField] private float legMoveSpeed = 0.5f;

    private IEnumerator[] moveLegsCoroutines = new IEnumerator[4];

    public bool slowGate = false;
    public bool fastGate = true;


    // Start is called before the first frame update
    void Start()
    {
        targets[0] = target1; //Define targets in targets array
        targets[1] = target2;
        targets[2] = target3;
        targets[3] = target4;

        movingTargets[0] = movingTarget1; //Define moving targets in movingTargets array
        movingTargets[1] = movingTarget2;
        movingTargets[2] = movingTarget3;
        movingTargets[3] = movingTarget4;

        for (int i = 0; i < targets.Length; i++)
        {
            RaycastHit hit; //Raycasts down and up by 10 units to place targets on the ground
            if (Physics.Raycast(targets[i].transform.position, Vector3.down, out hit, 10, ~groundLayer)) //10 is raycast distance, 9 is layermask ground
            {
                targets[i].transform.position = hit.point; //Move target object to the ground
            }

            currentTargetPos[i] = targets[i].transform.position; //Move currentTargetPos vector3 to the ground
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < targets.Length; i++) //Iterates on all 4 legs
        {
            RaycastHit hit; //Raycasts down and up by 5 units to determine where to put the moving Targets
            Vector3 raycastStart = new Vector3(movingTargets[i].transform.position.x, transform.position.y, movingTargets[i].transform.position.z); //Puts the start of the raycast at the height of the body
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 10f, ~groundLayer))
            {
                movingTargets[i].transform.position = hit.point; //Moves targets to hit position
            }
            
            distance[i] = Vector3.Distance(targets[i].transform.position, movingTargets[i].transform.position); //Distance from the current target point (target) to the moving target point

            if (fastGate) //Move legs on diagonals
            {
                if (distance[i] > maxDistance && !legisMoving[i]) //Moves the target to the moving target's position if the distance between them is too large and if the current leg is not moving
                {
                    if (i == 0 && !legisMoving[1] && !legisMoving[2]) //Leg 1 cannot move if legs 2 and 3 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 1 && !legisMoving[0] && !legisMoving[3]) //Leg 2 cannot move if leg 1 and 4 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 2 && !legisMoving[3] && !legisMoving[0]) //Leg 3 cannot move if leg 4 and 1 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 3 && !legisMoving[2] && !legisMoving[1]) //Leg 4 cannot move if leg 3 and 2 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                }
            }

            if (slowGate) //Move legs one by one
            {
                if (distance[i] > maxDistance && !legisMoving[i]) //Moves the target to the moving target's position if the distance between them is too large and if the current leg is not moving
                {
                    if (i == 0 && !legisMoving[1] && !legisMoving[2] && !legisMoving[3]) //Leg 1 cannot move if legs 2, 3 and 4 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 1 && !legisMoving[0] && !legisMoving[3]) //Leg 2 cannot move if leg 1, 3 and 4 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 2 && !legisMoving[3] && !legisMoving[0] && !legisMoving[1]) //Leg 3 cannot move if leg 4, 1 and 2 are moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                    else if (i == 3 && !legisMoving[2] && !legisMoving[1] && !legisMoving[0]) //Leg 4 cannot move if leg 3, 2 and 1 is moving
                    {
                        moveLegsCoroutines[i] = MoveLeg(i);
                        StartCoroutine(moveLegsCoroutines[i]); //Move the leg
                        legisMoving[i] = true; //The leg is moving
                    }
                }
            }

            targets[i].transform.position = currentTargetPos[i]; //Place the target object onto the currentTargetPosition
        }
    }

    private IEnumerator MoveLeg (int i) //i is the current leg
    {
        float progress = 0;
        Vector3 endPoint = movingTargets[i].transform.position;
        Vector3 startPoint = currentTargetPos[i];

        while (progress < 1.0f)
        {
            currentTargetPos[i] = Vector3.Lerp(startPoint, endPoint, progress); //Linearly move the current target position towards the endPoint
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime * legMoveSpeed;
        }

        currentTargetPos[i] = endPoint;
        legisMoving[i] = false; //The leg is no longer moving
    }
}
