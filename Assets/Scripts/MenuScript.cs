// ************************************************************************** //
//                                  PATH TRACER                               //
//                                GRAEME B. WHITE                             //
// ************************************************************************** //
// MenuScript.cs                                                              //
//                                                                            //
// Menu script that is used to allow user to select scenes, change            //
// resolution, view info about the application, and quit the app.             //
// ************************************************************************** //

// Libraries
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    // Scene Button
    [SerializeField] private Button _sceneButton;

    // Options Button
    [SerializeField] private Button _optionsButton;

    // About button
    [SerializeField] private Button _aboutButton;

    // Quit button
    [SerializeField] private Button _quitButton;

    // Intro canvas
    [SerializeField] private Canvas _introCanvas;

    // Scene Canvas
    [SerializeField] private Canvas _sceneCanvas;

    // Options Canvas
    [SerializeField] private Canvas _optionsCanvas;

    // About Canvas
    [SerializeField] private Canvas _aboutCanvas;

    // Game manager
    [SerializeField] private GameManager _theGM;

    // Resolution Dropdown
    [SerializeField] private Dropdown _resolutionDropdown;

    // Fullscreen Toggle
    [SerializeField] private Toggle _fullscreenToggle;

    // Resolutions array
    private Resolution[] _resolutions;

    // Current resolution index
    private int _currentResolutionIndex;

    // Temporary resolution index
    private int _temporaryResolutionIndex;

    // Default screen height (1080)
    private int _defaultHeight = 1080;

    // Default screen width (1280)
    private int _defaultWidth = 1920;

    // Start is called before the first frame update
    void Start()
    {
        // Hide the scene canvas
        HideSceneCanvas();

        // Hide the options canvas
        HideOptionsCanvas();

        // Hide the about canvas
        HideAboutCanvas();

        // Obtain viable screen resolutions
        _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        // Clear the options currently in the drop down menu
        _resolutionDropdown.ClearOptions();

        // Create a list of options
        List<string> options = new List<string>();

        // Current resolution index set to 0
        int currentResolutionIndex = 0;

        // Iterate over all of the resolutions
        for (int i = 0; i < _resolutions.Length; i++)
        {
            // Option string made from resolution width and height at index i
            string option = _resolutions[i].width + " x " + _resolutions[i].height;

            // Add the option to the options list
            options.Add(option);

            // Check the current width and height of the current screen resolution
            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                // Set the current resolution index to i
                currentResolutionIndex = i;

                // Set the private current resolution index to i
                _currentResolutionIndex = i;
            }
        }

        // Add the options to the dropdown options
        _resolutionDropdown.AddOptions(options);

        // Set the selected resolution to the current resolution index
        _resolutionDropdown.value = currentResolutionIndex;

        // Refresh dropdown to show the current resolution
        _resolutionDropdown.RefreshShownValue();

        // Update the value of the fullscreen toggle to show if the game is running in fullscreen
        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    /*
     * HIDE INTRO CANVAS
     * 
     * Method is used to hide the about
     * canvas from view.
     */
    public void HideIntroCanvas()
    {
        // Disable the options canvas
        _introCanvas.enabled = false;
    }

    /*
     * HIDE SCENE CANVAS
     * 
     * Method is used to hide the scene
     * canvas from view and disable 
     * buttons.
     */
    public void HideSceneCanvas()
    {
        // Disable the options canvas
        _sceneCanvas.enabled = false;
    }

    /*
     * SHOW SCENE CANVAS
     * 
     * Method is used to show the scene
     * canvas and enable buttons
     */
    public void ShowSceneCanvas()
    {
        // Enable the scene canvas
        _sceneCanvas.enabled = true;
    }

    /*
     * HIDE OPTIONS CANVAS
     * 
     * Method is used to hide the scene
     * canvas from view and disable 
     * buttons.
     */
    public void HideOptionsCanvas()
    {
        // Disable the options canvas
        _optionsCanvas.enabled = false;
    }

    /*
     * SHOW OPTIONS CANVAS
     * 
     * Method is used to show the options
     * canvas and enable relevant buttons.
     */
    public void ShowOptionsCanvas()
    {
        // Enable the options canvas
        _optionsCanvas.enabled = true;
    }

    /*
     * HIDE ABOUT CANVAS
     * 
     * Method is used to hide the about
     * canvas from view.
     */
    public void HideAboutCanvas()
    {
        // Disable the options canvas
        _aboutCanvas.enabled = false;
    }

    /*
     * SHOW ABOUT CANVAS
     * 
     * Method is used to show the about
     * canvas.
     */
    public void ShowAboutCanvas()
    {
        // Enable the options canvas
        _aboutCanvas.enabled = true;
    }

    /*
     * LOAD SCENE 1 METHOD
     * 
     * Method is used to instruct the
     * game manager to load the 1st
     * scene.
     * 
     * Scene is currently set to "SkyscrapeAndOrbs"
     */
    public void loadScene1()
    {
        // Instruct the game manager to load the scene with the skyscraper and orbs
        _theGM.sceneSelection("SkyscrapeAndOrbs");
    }

    /*
     * LOAD SCENE 2 METHOD
     * 
     * Method is used to instruct the
     * game manager to load the 2nd
     * scene.
     * 
     * Scene is currently set to "Glass"
     */
    public void loadScene2()
    {
        // Instruct the game manager to load the scene with the glass
        _theGM.sceneSelection("Glass");
    }

    /*
     * LOAD SCENE 3 METHOD
     * 
     * Method is used to instruct the 
     * game manager to load the 3rd
     * scene.
     * 
     * Scene is currently set to "Enclosed"
     */
    public void loadScene3()
    {
        // Instruct the game manager to load the enclosed scene
        _theGM.sceneSelection("Enclosed");
    }

    /*
     * LOAD SCENE 4 METHOD
     * 
     * Method is used to instruct the
     * game manager to load the 4th
     * scene.
     * 
     * Scene is currently set to "NiceDrink"
     */
    public void loadScene4()
    {
        // Intstruct the game manager to load the scene with the drink
        _theGM.sceneSelection("NiceDrink");
    }

    /*
     * LOAD SCENE 5 METHOD
     * 
     * Method is used to instruct the
     * game manager to load the 5th 
     * scene.
     */
    public void loadScene5()
    {
        _theGM.sceneSelection("");
    }

    /*
     * QUIT APPLICATION METHOD
     * 
     * Method quits the application when
     * invoked by the user.
     */
    public void QuitApplication()
    {
        // Quit application
        Application.Quit();
    }

    /*
     * UPDATE TEMPORARY RESOLUTION INDEX METHOD
     * 
     * Method updates the temporary resolution index
     * value, based on the selected option from the
     * dropdown menu.
     */
    public void UpdateTemporaryResolutionIndex()
    {
        // Set the temporary resolution index to the value from the dropdown menu
        _temporaryResolutionIndex = _resolutionDropdown.value;
    }

    /*
     * CHANGE RESOLUTION METHOD
     * 
     * Method is used to change the resolution
     * of the application, based on the value selected
     * by the dropdown menu.
     */
    public void ChangeResolution()
    {
        // Set the current resolution index to the value held by the temporary
        _currentResolutionIndex = _temporaryResolutionIndex;

        // Obtain the new resolution from the resolution array
        Resolution newResolution = _resolutions[_currentResolutionIndex];

        // Set the screen resolution
        Screen.SetResolution(newResolution.width, newResolution.height, _fullscreenToggle.isOn);
    }

    /*
     * SET DEFAULT RESOLUTION METHOD
     * 
     * Method sets the resolution of the application
     * to the default 1280x720 with fullscreen 
     * disabled.
     */
    public void SetDefaultResolution()
    {
        // Iterate over all resolutions to find the default resolution in the dropdown list
        for(int i = 0; i < _resolutions.Length; i++)
        {
            // Check that the resolution width and height at index i match the default width and heights
            if (_resolutions[i].width == _defaultWidth && _resolutions[i].height == _defaultHeight)
            {
                // Set the private current resolution index to i
                _currentResolutionIndex = i;
            }
        }

        // Set the value of the dropdown menu to the value of the current resolution index 
        _resolutionDropdown.value = _currentResolutionIndex;

        // Set the toggle to false
        _fullscreenToggle.isOn = false;

        // Change the resolution
        ChangeResolution();
    }
}