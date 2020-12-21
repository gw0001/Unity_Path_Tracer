using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Path Tracer
    [SerializeField] private PathTracer _pt;

    // UI
    [SerializeField] private DebugInfo _debugInfo;

    // Scene Management
    [SerializeField] private GameManager _theGM;

    // Player Camera
    [SerializeField] private PlayerCamera _theCam;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //
    private void Awake()
    {
        _theGM = gameObject.GetComponent<GameManager>();

        // Update the ray bounce limit on the ui
        _debugInfo.UpdateRay(_pt.GetCurrentBounceLimit);

        // Update the samples per pixel on the UI
        _debugInfo.UpdateSamplesPP(_pt.GetCurrentSample);

        // Disable the ui
        _debugInfo.DisableUI();
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        // SCENE MANAGEMENT

        // User presses 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[0]);
        }

        // User presses 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[1]);
        }

        // User presses 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[2]);

        }

        // User presses 4
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[3]);

        }

        // User presses 5
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[4]);
        }

        // User presses 6
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // Change scene
            _theGM.sceneSelection(_theGM.scenesToLoad[5]);

        }

        // FPS INFO
        // Check if user has pressed the "U" key on the keyboard 
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Enable the UI
            _debugInfo.EnableUI();

            // Invoke disable denoising shader method
            //_pt.DisableDenoisingShader();
        }

        // Check if the user has pressed the "I" key on the keyboard
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Disable the UI
            _debugInfo.DisableUI();

            // Invoke enable denoising shader method
            //_pt.EnableDenoisingShader();
        }

        // Check if the user has pressed the "J" key on the keyboard
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Check that increasing the sampling per pixel value doesn't exceed the limit
            if (!(_pt.GetCurrentSample + 1 > _pt.GetMaxSampleLimit))
            {
                // Increment the samples per pixel
                _pt.IncrementSamplesPerPixel();

                // Update the samples per pixel text on the UI
                _debugInfo.UpdateSamplesPP(_pt.GetCurrentSample);

                // Reset the current sample
                _pt.ResetCurrentSample();
            }
        }

        // Check if the user has pressed the "H" key on the key board
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Check that decrementing the samples per pixel doesn't = 0
            if (_pt.GetCurrentSample - 1 != 0)
            {
                // Decrement the samples per pixel
                _pt.DecrementSamplesPerPixel();

                // Update the samples per pixels text on the UI
                _debugInfo.UpdateSamplesPP(_pt.GetCurrentSample);

                // Reset the current sample
                _pt.ResetCurrentSample();
            }
        }

        // Check if the user has pressed the "L" key on the key board
        if (Input.GetKeyDown(KeyCode.L))
        {
            // CHeck that incrementing the ray bounce doesn't exceed the limit
            if (!(_pt.GetCurrentBounceLimit + 1 > _pt.GetMaxBounceLimit))
            {
                // Increment the bounce limit by 1
                _pt.IncrementRayBounceLimit();

                // Update the ray bounce text on the UI
                _debugInfo.UpdateRay(_pt.GetCurrentBounceLimit);

                // Reset the current sample
                _pt.ResetCurrentSample();
            }
        }

        // Check if the user has pressed the "K" key on the key board
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Check that decrementing the ray bounce doesn't = 0
            if (_pt.GetCurrentBounceLimit - 1 != 0)
            {
                // Decrement the bounce limit by 1
                _pt.DecrementRayBounceLimit();

                // Update the ray bounce text on the UI
                _debugInfo.UpdateRay(_pt.GetCurrentBounceLimit);

                // Reset the current sample
                _pt.ResetCurrentSample();
            }
        }

        // CAMERA CONTROL

        // Check if user has pressed "o" key on the keyboard
        if (Input.GetKey(KeyCode.O))
        {
            // Unlock the mouse cursor
            _theCam.UnlockMouseCursor();
        }

        // Check if user has pressed the "p" key on the keyboard
        if (Input.GetKey(KeyCode.P))
        {
            // Lock the mouse cursor
            _theCam.LockMouseCursor();
        }

        // OTHER CONTROLS

        // Check if user has pressed "F1" key on keyboard
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Enable the standard skybox
            _pt.EnableStandardSkybox();
        }

        // Check if user has pressed "F2" key on keyboard
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Enable the vaporwave skybox
            _pt.EnableNightSkybox();
        }

        // Check if user has pressed "F3" key on keyboard
        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Enable the textured skybox
            _pt.EnableVaporwaveSkybox();
        }

        // Check if user has pressed "F4" key on keyboard
        if (Input.GetKeyDown(KeyCode.F4))
        {
            // Enable the textured skybox
            _pt.EnableTexturedSkybox();
        }

        // Check if user has pressed the "Escape" key on the keyboard
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application
            Application.Quit();
        }

        // Check if user has pressed the "N" key on the keyboard
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Invoke enable denoising shader method
            _pt.EnableDenoisingShader();
        }

        // Check if user has pressed the "M" key on the keyboard
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Invoke disable denoising shader method
            _pt.DisableDenoisingShader();
        }
    }
}
