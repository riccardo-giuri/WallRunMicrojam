using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Player controller reference
    /// </summary>
    public CharacterController controller;
    /// <summary>
    /// Player speed while on the Ground
    /// </summary>
    public float GroundSpeed;
    /// <summary>
    /// Player speed while in air
    /// </summary>
    public float AirSpeed;
    /// <summary>
    /// Gravity value 
    /// </summary>
    public float GravityValue;
    /// <summary>
    /// velocity vector 
    /// </summary>
    Vector3 velocity;
    /// <summary>
    /// ground layer for the raycast
    /// </summary>
    public LayerMask groundlayer;
    /// <summary>
    /// bool value that represents if the player is on ground
    /// </summary>
    public bool isGrounded;
    /// <summary>
    /// jump height value
    /// </summary>
    public float JumpHeight;
    /// <summary>
    /// wall layer for the raycast
    /// </summary>
    public LayerMask WallLayer;
    /// <summary>
    /// bools for check if thw wall is at your right or left
    /// </summary>
    public bool IsWallRight, IsWallLeft;
    /// <summary>
    /// Gravity on the wall value
    /// </summary>
    public float WallGravity;
    /// <summary>
    /// Wall jump height value
    /// </summary>
    public float WallJumpHeight;
    /// <summary>
    /// camera pivot transform reference
    /// </summary>
    public Transform CameraPivot;
    /// <summary>
    /// camera rotation pivot reference
    /// </summary>
    public Transform CameraRotationPivot;

    // Start is called before the first frame update
    void Start()
    {
        //get the character controller reference
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //check for controller
        if(controller.enabled == false)
        {
            controller.enabled = true;
        }
        //raycast calculations
        RaycastGroundCalculation();
        WallCheckRaycast();
        CheckForRespawn();

        //things to do when the player is on the ground
        if (isGrounded == true)
        {
            velocity.x = 0f;

            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            CalculateMovement(GravityValue, GroundSpeed);
        }
        //things to do if he is not on the ground
        if (isGrounded == false)
        {
            //things to do on the wall
            if(IsWallLeft == true || IsWallRight == true)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    WallJump();
                }

                WallMovement(WallGravity, GroundSpeed);
            }
            else
            {
                CalculateMovement(GravityValue, AirSpeed);
            }
            
        }
    }

    /// <summary>
    /// calculate the movement of the player
    /// </summary>
    /// <param name="gravity"></param>
    /// <param name="Movespeed"></param>
    public void CalculateMovement(float gravity, float Movespeed)
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //control movement
        Vector3 move = (CameraPivot.transform.right * x) + (CameraPivot.transform.forward * z);
        move.Normalize();
        controller.Move(move * Movespeed * Time.deltaTime);


        //control gravity
        velocity.y += (-gravity * 0.5f) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// calculate the movement while on the wall
    /// </summary>
    /// <param name="gravity"></param>
    /// <param name="Movespeed"></param>
    public void WallMovement(float gravity, float Movespeed)
    {
        float z = Input.GetAxis("Vertical");

        if(z > 0)
        {
            Vector3 move = (transform.forward * z);
            controller.Move(move * Movespeed * Time.deltaTime);
        }

        //control gravity
        velocity.y += (-gravity * 0.5f) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// jump action while is on the wall
    /// </summary>
    public void WallJump()
    {
        if (IsWallLeft == true)
        {
            velocity.x = Mathf.Sqrt(WallJumpHeight * -2f * -WallGravity);
            velocity.y = Mathf.Sqrt(WallJumpHeight * -2f * -WallGravity);
        }
        if(IsWallRight == true)
        {
            velocity.x = -Mathf.Sqrt(WallJumpHeight * -2f * -WallGravity);
            velocity.y = Mathf.Sqrt(WallJumpHeight * -2f * -WallGravity);
        }
    }

    /// <summary>
    /// jump method
    /// </summary>
    public void Jump()
    {
        velocity.y = Mathf.Sqrt(JumpHeight * -2f * -GravityValue); 
    }

    /// <summary>
    /// raycast calculation for the ground 
    /// </summary>
    public void RaycastGroundCalculation()
    {
        float raydistance = (this.transform.localScale.y) * ((this.transform.localScale.y / 100) * 10);
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, (this.transform.localScale.y) + raydistance, groundlayer);
        Vector3 offsetray = new Vector3(0f, (this.transform.localScale.y) + raydistance, 0);

        Debug.DrawRay(transform.position, -offsetray, Color.red);
    }

    /// <summary>
    /// check raycast for the wall and perform action when he hit or leave a wall
    /// </summary>
    public void WallCheckRaycast()
    {
        bool CurrentcheckRight = IsWallRight;
        bool CurrecheckLeft = IsWallLeft;

        IsWallRight = Physics.Raycast(transform.position, Vector3.right, 1f, WallLayer);
        IsWallLeft = Physics.Raycast(transform.position, -Vector3.right, 1f, WallLayer);

        //when you attach to wall right
        if(CurrentcheckRight == false && IsWallRight == true)
        {
            ResetVelocity();
            TiltCamera(CameraRotationPivot, 15f);
        }
        //when you leave wall right
        if (CurrentcheckRight == true && IsWallRight == false)
        {
            TiltCamera(CameraRotationPivot, -15f);
        }
        //when you attach to wall left
        if (CurrecheckLeft == false && IsWallLeft == true)
        {
            ResetVelocity();
            TiltCamera(CameraRotationPivot, -15f);
        }
        //when you leave wall left
        if (CurrecheckLeft == true && IsWallLeft == false)
        {
            TiltCamera(CameraRotationPivot, 15f);
        }

        Debug.DrawRay(transform.position, Vector3.right/0.8f);
        Debug.DrawRay(transform.position, -Vector3.right/0.8f);
    }

    /// <summary>
    /// reset velocity to zero
    /// </summary>
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    /// <summary>
    /// tilt the camera some amount 
    /// </summary>
    /// <param name="CameraRotationtransform"></param>
    /// <param name="tiltAmount"></param>
    public void TiltCamera(Transform CameraRotationtransform, float tiltAmount)
    {
        CameraRotationtransform.Rotate(new Vector3(0, 0, tiltAmount));
    }
    /// <summary>
    /// check for the respwan condition
    /// </summary>
    public void CheckForRespawn()
    {
        if (transform.position.y < -7)
        {
            controller.enabled = false;
            this.transform.position = new Vector3(0f, 1f, 0f);
        }
    }

}
