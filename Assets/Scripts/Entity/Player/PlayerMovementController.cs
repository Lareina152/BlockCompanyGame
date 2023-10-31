﻿using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Input Mapping

    [ShowInInspector]
    private string horizontalMovementInputMappingID;

    [ShowInInspector]
    private string jumpInputMappingID;

    private bool inputMappingInitialized = false;

    public void SetInputMapping(string horizontalMovement, string jump)
    {
        horizontalMovementInputMappingID = horizontalMovement;
        jumpInputMappingID = jump;

        inputMappingInitialized = true;
    }

    #endregion

    [ShowInInspector]
    public bool enableMovement = true;

    public float runSpeed;
    public float jumpSpeed;
    public float doubleJumpSpeed;
    public float fallGravityScale = 2;
    public float maxFallVelocity = 20;

    private Rigidbody2D myRigidbody;
    private Animator myAnim;
    private BoxCollider2D myFeet;

    [ShowInInspector]
    public bool isGround { get; private set; }

    [ShowInInspector]
    private bool canDoubleJump;

    [ShowInInspector]
    private float initialGravityScale;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();//获取player脚部的碰撞盒对象

        GlobalEventManager.AddEvent(jumpInputMappingID, Jump);

        initialGravityScale = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputMappingInitialized == false)
        {
            return;
        }

        if (enableMovement == false)
        {
            return;
        }

        Run();
        Flip();
        //Jump();
        Fall();
        CheckGrounded();
        SwitchAnimation();
    }

    //检测是否接触到地面
    void CheckGrounded()
    {
        //脚碰到地面返回true
        isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground", "Player",
            "GameProperty"));
    }

    //玩家运动时翻转脸的朝向
    void Flip()
    {
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasXAxisSpeed)//有速度才翻转
        {
            if(myRigidbody.velocity.x>0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (myRigidbody.velocity.x <- 0.1f)//往左要翻转
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void Run()
    {
        //人物水平移动
        float moveDir = GlobalEventManager.GetFloatValue(horizontalMovementInputMappingID);
        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVel;

        //人物动画切换
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("Run", playerHasXAxisSpeed);//如果水平轴有速度，在跑，动画跑设为true
    }

    void Fall()
    {
        if (myRigidbody.velocity.y < 0.0f)
        {
            myRigidbody.gravityScale = fallGravityScale;

            if (myRigidbody.velocity.y < -maxFallVelocity)
            {
                myRigidbody.velocity = myRigidbody.velocity.ReplaceY(-maxFallVelocity);
            }
        }
        else
        {
            myRigidbody.gravityScale = initialGravityScale;
        }
    }

    void Jump()
    {
        if (enableMovement == false)
        {
            return;
        }

        if (isGround)//在地面才可跳跃
        {
            myAnim.SetBool("Jump", true);
            Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
            myRigidbody.velocity = Vector2.up * jumpVel;
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump)//可以二段跳跃
            {
                //myAnim.SetBool("DoubleJump", true);
                Vector2 doubleJumpVel = new Vector2(0.0f, doubleJumpSpeed);
                myRigidbody.velocity = doubleJumpVel * Vector2.up;
                canDoubleJump = false;
            }
        }
    }

    //动画转换
    void SwitchAnimation()
    {
        myAnim.SetBool("Idle", false);
        if (myAnim.GetBool("Jump"))//如果在跳跃
        {
            if (myRigidbody.velocity.y < 0.0f)//下落时
            {
                myAnim.SetBool("Jump", false);
                myAnim.SetBool("Fall", true);
            }
        }
        else if (isGround)//接触地面时
        {
            myAnim.SetBool("Fall", false);
            myAnim.SetBool("Idle", true);
        }

        //还没有二段跳跃的动画
        /*if (myAnim.GetBool("DoubleJump"))
        {
            if (myRigidbody.velocity.y < 0.0f)
            {
                myAnim.SetBool("DoubleJump", false);
                myAnim.SetBool("DoubleFall", true);
            }
        }
        else if (isGround)
        {
            myAnim.SetBool("DoubleFall", false);
            myAnim.SetBool("Idle", true);
        }*/


    }

}
