// ************************************************************************** //
//                                  PATH TRACER                               //
//                                GRAEME B. WHITE                             //
// ************************************************************************** //
// PlayerControl.cs                                                           //
//                                                                            //
// Script  that provides the player with some application functionality.      //
// ************************************************************************** //

// Libraries
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Path Tracer
    [SerializeField] private PathTracer _pt;

    // Scene Management
    [SerializeField] private GameManager _theGM;

    // Player Camera
    [SerializeField] private PlayerCamera _theCam;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    private void Update()
    {
        // Check if the user has pressed the "L" key on the key board
        if (Input.GetKeyDown(KeyCode.L))
        {
            // CHeck that incrementing the ray bounce doesn't exceed the limit
            if (!(_pt.GetCurrentBounceLimit + 1 > _pt.GetMaxBounceLimit))
            {
                // Increment the bounce limit by 1
                _pt.IncrementRayBounceLimit();

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

                // Reset the current sample
                _pt.ResetCurrentSample();
            }
        }

        // SKY CONTROLS

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

        // OTHER CONTROLS

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

        // Check if user has pressed the "Escape" key on the keyboard
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Unlock the mouse cursor
            _theCam.UnlockMouseCursor();

            // Quit the application
            _theGM.sceneSelection("MainMenu");
        }
    }
}
