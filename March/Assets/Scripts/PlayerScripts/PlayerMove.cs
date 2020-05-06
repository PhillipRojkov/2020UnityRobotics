using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] public float baseSpeed = 10;
    private float speed;
    [SerializeField] private float airSpeedMultiplier = 0.6f;
    [SerializeField] private float turnSensitivity = 10;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private GameObject camera;
    private float verticalLook;
    private float horizontalLook;
    public bool isGrounded = false;
    [SerializeField] private CapsuleCollider playerCollider;
    public bool canMove = true;

    public bool inPuzzle = false;

    private float t;

    private void Start()
    {
        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveVertical = transform.forward * vertical; //Create movement vectors using local rotation
        Vector3 moveHorizontal = transform.right * horizontal;

        Vector3 move = moveVertical + moveHorizontal;
        move.Normalize(); //Normalize movement so that diagonal isn't faster than forward or horizontal movement

        Vector3 velocity = rb.velocity; //Directly modify player velocity
        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        //GROUND CHECK
        //Grounded raycast check with 5 raycasts, one central, 4 on outer bounds of player. For more forgiving jumping
        bool groundCheckCentre = Physics.Raycast(playerCollider.bounds.center, Vector3.down, playerCollider.bounds.extents.y + 0.1f);

        Vector3 frontRay = playerCollider.bounds.center + transform.forward * playerCollider.radius; //Creates a vector3 position at the front edge of the player collider
        bool groundCheckFront = Physics.Raycast(frontRay, Vector3.down, playerCollider.bounds.extents.y);

        Vector3 backRay = playerCollider.bounds.center + transform.forward * -playerCollider.radius; //Creates a vector3 position at the back edge of the player collider
        bool groundCheckBack = Physics.Raycast(backRay, Vector3.down, playerCollider.bounds.extents.y);

        Vector3 leftRay = playerCollider.bounds.center + transform.right * -playerCollider.radius; //Creates a vector3 position at the left edge of the player collider
        bool groundCheckLeft = Physics.Raycast(leftRay, Vector3.down, playerCollider.bounds.extents.y);

        Vector3 rightRay = playerCollider.bounds.center + transform.right * playerCollider.radius; //Creates a vector3 position at the right edge of the player collider
        bool groundCheckRight = Physics.Raycast(rightRay, Vector3.down, playerCollider.bounds.extents.y);

        if (groundCheckCentre || groundCheckFront || groundCheckBack || groundCheckLeft || groundCheckRight)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //JUMPING
        //Apply jump force if not grounded (only applied when jump > 0, ie. when jump is pressed)
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Apply jump force
                velocity.y = jumpForce;
            }
            if (speed != baseSpeed)
            {
                speed = baseSpeed; //Change speed back to base speed when grounded
            }
        }
        else
        {
            speed = baseSpeed * airSpeedMultiplier; //Decrease speed by airspeed multiplier when airborne
        }

        //APPLY VELOCITY
        if (canMove && !inPuzzle) //Allows for disablement of movement
        {
            rb.velocity = velocity; //Apply the new velocity
        } else if (t == 0)
        {
            t = Time.time;
            t += .8f;
        } else if (Time.time > t)
        {
            canMove = true;
            t = 0;
        }

        //LOOKING
        verticalLook += -Input.GetAxis("Mouse Y") * turnSensitivity;
        horizontalLook += Input.GetAxis("Mouse X") * turnSensitivity;
        verticalLook = Mathf.Clamp(verticalLook, -90, 90);
        //Rotate camera according to verticalLook
        camera.transform.localRotation = Quaternion.Euler(verticalLook, 0, 0);
        //Rotate player on y axis
        transform.rotation = Quaternion.Euler(0, horizontalLook, 0);
    }
}
