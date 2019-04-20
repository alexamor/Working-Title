using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlMovAl : MonoBehaviour
{

    public CharCont controller;

    public float runSpeed = 40f;
    public float jumpLimit = 0.5f;

    float horizontalMove = 0f;
    float jumpTimer = 0;
    bool jump = true; //If player can jump


    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        ////On ground/wall allow for another jump
        //if (controller.CanJump() && (!Input.GetButton("Jump")))
        //{
        //    jumpTimer = 0;
        //    jump = true;
        //}

        //if (Input.GetButton("Jump") && (jumpTimer <= jumpLimit) && jump == true)
        //{
        //    controller.Jump(jumpTimer);

        //    jumpTimer += Time.deltaTime;
        //}

        //if(jumpTimer > jumpLimit || Input.GetButtonUp("Jump"))
        //{
        //    jump = false;

        //    //Debug.Log("jump time " + jumpTimer);
        //}
                
        //controller.Move(horizontalMove * Time.deltaTime, false, jump);
    }

    void FixedUpdate()
    {
        //On ground/wall allow for another jump
        if (controller.CanJump() && (!Input.GetButton("Jump")))
        {
            jumpTimer = 0;
            jump = true;
        }

        if (Input.GetButton("Jump") && (jumpTimer <= jumpLimit) && jump == true)
        {
            controller.Jump(jumpTimer);

            jumpTimer += Time.deltaTime;
        }

        if (jumpTimer > jumpLimit || Input.GetButtonUp("Jump"))
        {
            jump = false;

            if(controller.goingUp)
                controller.StopJump();
        }

        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);

        controller.UpdateDirections();
    }
}
