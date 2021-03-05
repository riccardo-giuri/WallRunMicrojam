﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Camera View Sensitivity
    /// </summary>
    public float ViewSensitivity;
    /// <summary>
    /// camera pivot reference
    /// </summary>
    public Transform cameraPivot;
    /// <summary>
    /// player movement reference
    /// </summary>
    public PlayerMovement playerMovement;
    /// <summary>
    /// the camera X axis rotation
    /// </summary>
    private float cameraXrotation;
    /// <summary>
    /// vector2 that stores the inputs
    /// </summary>
    private Vector2 mouseInputs;

    // Start is called before the first frame update
    void Start()
    {
        //lock the cursor in the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //get the mouse inputs
        GetMouseInputs();

        //apply camera rotation
        ApplyCameraRotation(mouseInputs.x, mouseInputs.y, cameraPivot);
    }

    /// <summary>
    /// get the mouse inputs 
    /// </summary>
    public void GetMouseInputs()
    {
        float mouseX = Input.GetAxis("Mouse X") * ViewSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ViewSensitivity * Time.deltaTime;

        mouseInputs.x = mouseX;
        mouseInputs.y = mouseY;
    }

    /// <summary>
    /// Apply Camera rotation based on inputs
    /// </summary>
    /// <param name="mouseX"></param>
    /// <param name="mouseY"></param>
    public void ApplyCameraRotation(float mouseX, float mouseY, Transform rotationTransform)
    {
        //player rotation
        rotationTransform.Rotate(Vector3.up * mouseX);

        cameraXrotation -= mouseY;
        cameraXrotation = Mathf.Clamp(cameraXrotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(cameraXrotation, 0f, 0f);
    }

}
