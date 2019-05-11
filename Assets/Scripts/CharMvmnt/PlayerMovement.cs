using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


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

    public float wallSlideSpeed = -5f;
    private float jumpTimer = 0;

    private float stdGravity = 0;
    private float curGravity;
    private int wallJump = 0;

    private int[] dashDir; //0 - horizontal, 1 - vertical
    private float dashTime = 0;
    private bool canDash = true;
    public float dashDuration = 0.3f;
    public float dashSpeed = 30f;


    private ChMovAl controller;
    private Animator anim;

    public GameObject spirit;

    private void Awake()
    {
        controller = GetComponent<ChMovAl>();
        anim = GetComponent<Animator>();

        //runSpeed = onGroundRunSpeed;
        runSpeed = topLatSpeed;

        //set gravity
        stdGravity = (-2f * topHeight ) / (jumpTopDuration * jumpTopDuration);
        curGravity = stdGravity;

        //create dash direction array
        dashDir = new int[2];
    }

    private void Update()
    {
        //grab our current velocity to use as a base for all calculations
        var velocity = controller.velocity;

        anim.SetFloat("VerticalMov", velocity.y);

        if (controller.isGrounded)
            velocity.y = 0;

        if(controller.collisionState.hasCollisionToJump())
        {
            jumpTimer = 0;
            curGravity = stdGravity;
        }

        //horizontal input
        horizontalMove = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("HorizontalMov", Mathf.Abs(horizontalMove));

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

        //Check for wallslide
        if(!controller.collisionState.below && controller.collisionState.hasWall())
        {
            anim.SetBool("OnWall", true);
        }
        else
        {
            anim.SetBool("OnWall", false);
        }


        if (Input.GetButtonDown("Jump") && (controller.collisionState.hasCollisionToJump()))
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

        if (Input.GetButtonDown("Dash") && dashTime <= 0 && canDash)
        {
            createDash();
            spirit.GetComponent<Animator>().SetInteger("dash", 1);
        }

        if(Input.GetButton("Jump"))
        {
            jumpTimer = Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") && jumpTimer < jumpTopDuration)
            curGravity = stdGravity * 2;

        //apply gravity before moving
        velocity.y += curGravity * Time.deltaTime;

        //make wallslide slow down fall
        if (controller.collisionState.hasWall() && velocity.y < wallSlideSpeed)
            velocity.y = wallSlideSpeed;

        if (velocity.y <= 0)
        {
            //curGravity = stdGravity;
            wallJump = 0;
        }


        //Overwrite velocity in case of dash
        if(dashTime > 0)
        {
            velocity = new Vector3(dashDir[0] * dashSpeed, dashDir[1] * dashSpeed, 0);
            dashTime -= Time.deltaTime;
            

            //If dash ended this frame reset direction
            if(dashTime < 0)
            {
                velocity = new Vector3(dashDir[0] * dashSpeed/10, dashDir[1] * dashSpeed/10, 0);

                dashDir[0] = 0;
                dashDir[1] = 0;
            }
        }

        //Reset dash (for now touching ground resets dash)
        if(dashTime < 0 && !canDash && controller.isGrounded)
        {
            canDash = true;
            spirit.GetComponent<Animator>().SetInteger("dash", 0);
        }

        controller.move(velocity * Time.deltaTime);
    }

    private void goLeft()
    {
        if (transform.localScale.x > 0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            spirit.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
            
    }

    private void goRight()
    {
        if (transform.localScale.x < 0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            spirit.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); ;

        }
    }
    private float GiveInitVelocity()
    {
        float initVelocity;

        initVelocity = 2 * topHeight / jumpTopDuration;

        return initVelocity;
    }

    private void createDash()
    {
        //Set dash direction according to arrows/ASDW pressed
        if (horizontalMove > 0)
            dashDir[0] = 1;
        else if (horizontalMove < 0)
            dashDir[0] = -1;

        if (Input.GetAxisRaw("Vertical") > 0)
            dashDir[1] = 1;
        else if(Input.GetAxisRaw("Vertical") < 0)
            dashDir[1] = -1;

        //Debug.Log("horizontal  " + horizontalMove + ", verical    " + Input.GetAxisRaw("Vertical"));

        //If no direction selected dash horizontally
        if (dashDir[0] == 0 && dashDir[1] == 0)
        {
            if (transform.localScale.x > 0f)
                dashDir[0] = 1;
            else
                dashDir[0] = -1;
        }
        
        dashTime = dashDuration;

        //Make diagonal move equal
        if (Mathf.Abs(dashDir[0]) == 1 && Mathf.Abs(dashDir[1]) == 1)
            dashSpeed = 10.7f;
        else
            dashSpeed = 15f;

        canDash = false;
    }
}
