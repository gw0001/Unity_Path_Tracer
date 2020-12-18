// ************************************************************************** //
//                                GAME MANAGER                                //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// GameManager.cs                                                             //
//                                                                            //
// Script is used to load scenes when user decides to change to another one.  //
// When a scene is changed, the game manager and scene manager are carried    //
// over from the old scene to the new scene.                                  //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * GameManager.cs
 * 
 * GAME MANAGER
 * 
 * Script for managing the application.
 * 
 * Game manager keeps track of which scenes to load
 * and the loading of these scenes
 */
public class GameManager : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Null instance of GameManager
    public static GameManager instance = null;

    // Scene object
    Scene scene;

    // Scene Management Header for Unity editor
    [Header("Scene Management")]
    // Array of scenes to load
    public string[] scenesToLoad;

    // Active scene
    [Header("Current scene")]
    public string activeScene;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    /*
     * AWAKE METHOD
     * 
     * Method is invoked when the scene has
     * loaded.
     * 
     * Method ensures that we only have a 
     * single instance of the Game Manager 
     * (singleton design pattern)
     */
    private void Awake()
    {
        // Check if instance is null
        if (instance == null)
        {
            //Don't destroy the current game manager
            DontDestroyOnLoad(gameObject);

            //Set game manager instance to this
            instance = this;
        }
        // Check if current instance of game manager is equal to this game manager
        else if (instance != this)
        {
            //Destroy the game manager that is not the current game manager
            Destroy(gameObject);
        }
    }

    /*
     * START METHOD
     * 
     * Method is invoked before the first frame
     * of the scene.
     * 
     * Method sets the initial active scene
     */
    void Start()
    {
        // Set scene to the currently active scene
        scene = SceneManager.GetActiveScene();

        // Set active scene to build index and scene name
        activeScene = scene.buildIndex + " - " + scene.name;
    }

    /*
     * SCENE SELECTION METHOD
     * 
     * Method for selecting a scene, based on the
     * name of the scene.
     * 
     */
    public void sceneSelection(string selectedScene)
    {
        // Load selected scene
        SceneManager.LoadScene(selectedScene);

        // Set active scene to the selected scene
        activeScene = selectedScene;
    }
}