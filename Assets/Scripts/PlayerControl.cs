using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum PlayerStateType
{
    Run,
    Jump,
    Down,
}
public class PlayerControl : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float aSpeed;//加速度
    public float jetTimes;
    public float jetSpeed;
    public GameObject kingCrimson;
    private Animator playerAnimator;
    private Transform playerTransform;
    
    private bool isGround;
    private int jumpNum = 0;//跳跃次数
    private Rigidbody playerRigdbody;
    private RaycastHit hit;

    private float jumpTime;
    private float timing = 2.0f;
    private float bestTiming = 0.5f;
    private float lastPosY = -1;
    private float jetValue;

    private bool isJump;
    private delegate void PlayerMove();
    private PlayerMove playerMove;
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        playerRigdbody = GetComponent<Rigidbody>();
        playerMove = PlayerMove01;
    }
    private void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.05f, -Vector3.up, out hit, 0.1f) && hit.transform.tag == "Path")
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        //委托
        playerMove();
    }
    private void FixedUpdate()
    {
        if (!isJump && playerRigdbody.velocity.z < speed)
        {
            playerRigdbody.AddForce(Vector3.forward * speed);
        }
        playerAnimator.SetFloat("Blend", playerRigdbody.velocity.z / speed);
    }
    private void PlayerMove01()
    {
        switch (jumpNum)
        {
            case 0:
                if (isGround)
                {
                    SetPlayerAnima(PlayerStateType.Run);
                    isJump = false;
                    if (Input.GetMouseButtonDown(0) && playerTransform.position.z < 18.0f)
                    {
                        if (playerTransform.position.z > 14.5f && playerTransform.position.z < 15.5f)//最佳
                        {
                            jumpSpeed *= 2;
                        }
                        Jump();
                    }
                    if (playerTransform.position.z >= 18.0f)
                    {
                        playerMove = PlayerDie;
                    }
                }
                break;
            case 1:
                if (isGround)
                {
                    isJump = false;
                    jumpTime += Time.deltaTime;
                    SetPlayerAnima(PlayerStateType.Run);
                    if (jumpTime > timing)
                    {
                        playerMove = PlayerDie;
                    }
                    else if (Input.GetMouseButtonDown(0) && jumpTime <= timing)
                    {
                        if (jumpTime <= bestTiming)
                        {
                            jumpSpeed *= 2;
                        }
                        Jump();
                    }
                }
                else
                {
                    if (lastPosY > playerTransform.position.y && playerTransform.position.y < 2)
                    {
                        SetPlayerAnima(PlayerStateType.Down);
                    }
                    lastPosY = playerTransform.position.y;
                }
                break;
            case 2:
                //延时委托赋值，爆发力*2
                if (isGround)
                {
                    isJump = false;
                    jumpTime += Time.deltaTime;
                    SetPlayerAnima(PlayerStateType.Run);
                    if (jumpTime > timing)
                    {
                        playerMove = PlayerDie;
                    }
                    else if (Input.GetMouseButtonDown(0) && jumpTime <= timing)
                    {
                        if (jumpTime <= bestTiming)
                        {
                            jumpSpeed *= 2;
                        }
                        Jump();
                        StartCoroutine(Jet());
                    }
                }
                else
                {
                    if (lastPosY > playerTransform.position.y && playerTransform.position.y < 2)
                    {
                        SetPlayerAnima(PlayerStateType.Down);
                    }
                    lastPosY = playerTransform.position.y;
                }
                break;
            case 3:

                break;
            default:
                Debug.Log("次数不对哦");
                break;
        }
    }
    private void PlayerMove02()
    {
        if (isGround)
        {
            playerAnimator.speed = 0;
            kingCrimson.SetActive(false);
        }
        else if (Input.GetMouseButton(0) && jetValue > 0)
        {
            --jetValue;
            GameManager.instance.SetJetSlider(jetValue / jetTimes);
            playerRigdbody.AddForce(Vector3.forward * jetSpeed, ForceMode.Impulse);
        }
    }
    private void PlayerDie()
    {
        //Debug.Log("pd");
    }
    private void Jump()
    {
        SetPlayerAnima(PlayerStateType.Jump);
        playerRigdbody.AddForce((Vector3.up) * jumpSpeed, ForceMode.Impulse);
        jumpNum++;
        jumpTime = 0;
        isJump = true;
    }
    private void SetPlayerAnima(PlayerStateType playerStateType)
    {
        switch (playerStateType)
        {
            case PlayerStateType.Run:
                playerAnimator.SetBool("run", true);
                playerAnimator.SetBool("jump", false);
                playerAnimator.SetBool("down", false);
                break;
            case PlayerStateType.Jump:
                playerAnimator.SetBool("run", false);
                playerAnimator.SetBool("jump", true);
                playerAnimator.SetBool("down", false);
                break;
            case PlayerStateType.Down:
                playerAnimator.SetBool("run", false);
                playerAnimator.SetBool("jump", false);
                playerAnimator.SetBool("down", true);
                break;
        }
    }
    IEnumerator Jet()
    {
        yield return new WaitForSeconds(1);
        playerRigdbody.freezeRotation = false;
        GameManager.instance.jetSlider.SetActive(true);
        jetValue = jetTimes;
        playerMove = PlayerMove02;
    }
}

