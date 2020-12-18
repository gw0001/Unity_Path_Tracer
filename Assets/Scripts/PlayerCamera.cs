// ************************************************************************** //
//                               PLAYER CAMERA                                //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// PlayerCamera.cs                                                            //
//                                                                            //
// Script allows the user to move the camera with the mouse and explore the   //
// 3D scene with the W, A, S, D keys.                                         //
//                                                                            //
// Script is attached to the camera game object in unity                      //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Mouse sensitivity
    [SerializeField] private float _mouseSensitivity = 5.0f;

    // Player speed
    [SerializeField] private float _playerSpeed = 10.0f;

    // Maximum and Minimum angles of rotation - to prevent over rotation
    // Minium rotation angle in Y
    [SerializeField] public float _minimumY = -90.0f;

    // Maximum rotation angle in Y
    [SerializeField] public float _maximumY = 90.0f;

    // Rotation in X
    private float _rotationX = 0.0f;

    // Rotation in Y
    private float _rotationY = 0.0f;

    // Camera component
    private Camera _theCamera;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    /*
     * AWAKE METHOD
     * 
     * Method is called when the active game object
     * is set to active. Method sets the cursor lockstate,
     * hides the cursor and obtains the camera in the scene
     */
    void Awake()
    {
        // Lock the mouse cursor to the camera of the application
        LockMouseCursor();

        // Determine the camera object that the script is assigned to
        _theCamera = Camera.main;
    }

    /*
     * UPDATE METHOD
     * 
     * Method is called before the first frame of the 
     * application has loaded
     */
    void Update()
    {

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Determine the rotation in X from mouse input
            _rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _mouseSensitivity;

            // Determine the rotation in Y from the mouse input
            _rotationY += Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Use clamp method to determine the value for the Y Rotation
            _rotationY = Mathf.Clamp(_rotationY, _minimumY, _maximumY);

            // Transform the camera
            _theCamera.transform.localEulerAngles = new Vector3(-_rotationY, _rotationX, 0);

            // Camera movement
            CameraMovement();
        }

        // Check if user has pressed "o" key on the keyboard
        if (Input.GetKey(KeyCode.O))
        {
            // Unlock the mouse cursor
            UnlockMouseCursor();
        }

        // Check if user has pressed the "p" key on the keyboard
        if (Input.GetKey(KeyCode.P))
        {
            // Lock the mouse cursor
            LockMouseCursor();
        }
    }

    /*
     * CAMERA MOVEMENT METHOD
     * 
     * Method for moving the camera around the virtual
     * environment.
     */
    void CameraMovement()
    {
        // Obtain the current velocity of the camera based on the axis (horizontal and vertical)
        Vector3 cameraVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Determine the player velocity based on the player speed and the current time slice
        cameraVelocity = cameraVelocity * _playerSpeed * Time.deltaTime;

        // Move the camera
        _theCamera.transform.position = (_theCamera.transform.position + (_theCamera.transform.forward * cameraVelocity.z) + (_theCamera.transform.right * cameraVelocity.x));
    }

    /*
     * UNLOCK CURSOR METHOD
     * 
     * Method is used to unlock the cursor from
     * the application and make the cursor visible
     */
    void UnlockMouseCursor()
    {
        // Unlock the cursor to the window of the application
        Cursor.lockState = CursorLockMode.None;

        // Hide cursor from the main view
        Cursor.visible = true;
    }

    /*
     * LOCK CURSOR METHOD
     * 
     * Method is used to lock the cursor to
     * the application and hide the cursor
     */
    void LockMouseCursor()
    {
        // Lock the cursor to the window of the application
        Cursor.lockState = CursorLockMode.Locked;

        // Hide cursor from the main view
        Cursor.visible = false;
    }
}