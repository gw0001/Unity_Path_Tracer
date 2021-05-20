// ************************************************************************** //
//                                SCENE MOVEMENT                              //
//                                GRAEME B. WHITE                             //
// ************************************************************************** //
// SceneMovement.cs                                                           //
//                                                                            //
// Script manages how the user can change scene.                              //
//                                                                            //
// User can press 1, 2, 3, 4, 5, or 6 keys on the keyboard to change scene    //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMovement : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Game Manager game object
    private GameManager _theGM;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    /*
     * AWAKE METHOD
     * 
     * Method is invoked before when the scene
     * is loaded
     * 
     * Method obtains the game manager and
     * checks for user input at each frame.
     * 
     */
    private void Awake()
    {
        // Obtain the game manager component
        _theGM = GetComponent<GameManager>();
    }

    /*
     * UPDATE METHOD
     * 
     * Method is invoked once per frame
     * 
     * Method checks for user input, then moves
     * camera depending on the input
     */
    void Update()
    {
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
    }
}