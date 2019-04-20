using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //Animation States
    //private int idleState = Animator.StringToMash("Idle");
    //private int runState = Animator.StringToMash("Run");
    //private int jumpState = Animator.StringToMash("Jump");

    private float horizontalMove = 0f;
    //public float onGroundRunSpeed = 8f;
    private float runSpeed;
    //public float jumpForce = 2f;
    //public float gravity = -15f;

    public float topLatSpeed = 6f;
    public float topHeight = 1.6f;
    public float wallJumpSpeedBonus = 2f; 
    //public float topLatMovmt = 2.6f;
    public float jumpTopDuration = 0.25f;
    private float jumpTimer = 0;

    private float stdGravity = 0;
    private float curGravity;
    private int wallJump = 0;

    private ChMovAl controller;
    private Animator anim;

    private void Awake()
    {
        controller = GetComponent<ChMovAl>();
        //animator = GetComponent<Animator>();

        //runSpeed = onGroundRunSpeed;
        runSpeed = topLatSpeed;

        //set gravity
        stdGravity = (-2f * topHeight ) / (jumpTopDuration * jumpTopDuration);
        curGravity = stdGravity;
    }

    private void Update()
    {
        //grab our current velocity to use as a base for all calculations
        var velocity = controller.velocity;

        if (controller.isGrounded)
            velocity.y = 0;

        if(controller.collisionState.hasCollisionToJump())
        {
            jumpTimer = 0;
            curGravity = stdGravity;
        }

        //horizontal input
        horizontalMove = Input.GetAxisRaw("Horizontal");

        velocity.x = runSpeed * horizontalMove;

        //Force the player to move opposite to the wall
        if (wallJump != 0)
            velocity.x = wallJump * (runSpeed + wallJumpSpeedBonus);

        //Left
        if(horizontalMove < 0)
        {
            goLeft();
        }
        //Right
        else if(horizontalMove > 0)
        {
            goRight();
        }
        //Idle
        else
        {
            if(controller.isGrounded)
                velocity.x = 0;               
        }
        

        if(Input.GetButtonDown("Jump") && (controller.collisionState.hasCollisionToJump()))
        {
            //velocity.y = Mathf.Sqrt(2f * jumpForce * - gravity);
            velocity.y = GiveInitVelocity();

            //Check if the jump was or not a walljump
            if (!controller.collisionState.below)
            {
                if (controller.collisionState.left)
                    wallJump = 1;
                else
                    wallJump = -1;
            }
        }

        if(Input.GetButton("Jump"))
        {
            jumpTimer = Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") && jumpTimer < jumpTopDuration)
            curGravity = stdGravity * 2;

        //Debug.Log("Collision to Jump  " + controller.collisionState.hasCollisionToJump());

        //apply gravity before moving
        velocity.y += curGravity * Time.deltaTime;

        if (velocity.y <= 0)
        {
            //curGravity = stdGravity;
            wallJump = 0;
        }


        controller.move(velocity * Time.deltaTime);
    }

    private void goLeft()
    {
        if (transform.localScale.x > 0f)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void goRight()
    {
        if (transform.localScale.x < 0f)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private float GiveInitVelocity()
    {
        float initVelocity;

        initVelocity = 2 * topHeight / jumpTopDuration;

        return initVelocity;
    }
}
