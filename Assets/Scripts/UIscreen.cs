using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIscreen : MonoBehaviour
{
    // Average frame rate
    private int _fpsAvg;

    // FPS text
    [SerializeField] private Text _fpsText;

    // FPS text back
    [SerializeField] private Text _fpsTextBack;

    // Current Sample text
    [SerializeField] private Text _curSampleText;

    // Current Sample text back
    [SerializeField] private Text _curSampleTextBack;

    // Ray bounce limit text
    [SerializeField] private Text _rayBounceText;

    // Ray bounce limit text back
    [SerializeField] private Text _rayBounceTextBack;

    // Samples per pixel text
    [SerializeField] private Text _sppText;

    // Samples per pixel text back
    [SerializeField] private Text _sppTextBack;

    // Path tracer
    [SerializeField] private PathTracer _pathTracer;

    // Enable UI boolean
    private bool _enableUI;

    private void Update()
    {
        // Check if the UI is enabled
        if(_enableUI == true)
        {
            // Update the FPS
            UpdateFPS();

            // Update the current sample
            UpdateCurrentSample();
        }
        
        
        
        

    }


    private void UpdateFPS()
    {
        // Current frame
        float current = 0;

        // Determine the frame rate
        current = (int)(1f / Time.unscaledDeltaTime);

        // Set average frame rate
        _fpsAvg = (int)current;

        // Update FPS text
        _fpsText.text = "FPS: " + _fpsAvg.ToString();

        // Update FPS back text
        _fpsTextBack.text = "FPS: " + _fpsAvg.ToString();
    }

    private void UpdateCurrentSample()
    {
        // Obtain the current sample
        int currentSample = _pathTracer.GetCurrentSample();

        // Update current sample text
        _curSampleText.text = "Current sample: " + currentSample.ToString();

        // Update the current sample back text
        _curSampleTextBack.text = "Current sample: " + currentSample.ToString();
    }

    public void UpdateRay(int anInteger)
    {
        // Update the ray bounce text
        _rayBounceText.text = "Ray bounce limit: " + anInteger.ToString();

        // Update the ray bounce back text
        _rayBounceTextBack.text = "Ray bounce limit: " + anInteger.ToString();
    }

    public void UpdateSamplesPP(int anInteger)
    {
        // Update the samples per pixel text
        _sppText.text = "Samples per pixel: " + anInteger.ToString();

        // Update the samples per pixel back text
        _sppTextBack.text = "Samples per pixel: " + anInteger.ToString();
    }

    /*
     * ENABLE UI METHOD
     * 
     * Method shows the UI and sets enable
     * UI to true
     */
    public void EnableUI()
    {
        // Show FPS text
        _fpsText.gameObject.SetActive(true);

        // Show FPS back text
        _fpsTextBack.gameObject.SetActive(true);

        // Show the current sample text
        _curSampleText.gameObject.SetActive(true);

        // Show the current sample back text
        _curSampleTextBack.gameObject.SetActive(true);

        // Show the ray bounce limit text
        _rayBounceText.gameObject.SetActive(true);

        // Show the ray bounce limit back text
        _rayBounceTextBack.gameObject.SetActive(true);

        // Show the samples per pixel text
        _sppText.gameObject.SetActive(true);

        // Show the samples per pixel back text
        _sppTextBack.gameObject.SetActive(true);

        // Set enable UI to true
        _enableUI = true;
    }

    /*
     * DISABLE UI METHOD
     * 
     * Method hides the UI and sets enable
     * UI to false
     */
    public void DisableUI()
    {
        // Hide FPS text
        _fpsText.gameObject.SetActive(false);

        // Hide FPS back text
        _fpsTextBack.gameObject.SetActive(false);

        // Hide current sample text
        _curSampleText.gameObject.SetActive(false);

        // Hide current sample back text
        _curSampleTextBack.gameObject.SetActive(false);

        // Hide ray bounce limit text
        _rayBounceText.gameObject.SetActive(false);

        // Hide ray bounce limit back text
        _rayBounceTextBack.gameObject.SetActive(false);

        // Hide samples per pixel text
        _sppText.gameObject.SetActive(false);

        // Hide samples per pixel back text
        _sppTextBack.gameObject.SetActive(false);

        // Set enable UI to false
        _enableUI = false;
    }
}
