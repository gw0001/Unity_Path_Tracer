// ************************************************************************** //
//                              OBJECT PROPERTIES                             //
//                                GRAEME B. WHITE                             //
// ************************************************************************** //
// ObjectProperties.cs                                                        //
//                                                                            //
// Script contains methods for obtain values from materials applied to the    //
// scene object and setting the appropriate variables. Some variables can be  //
// set in the Unity editor.                                                   //
//                                                                            //
// Script also contains methods for other scripts to obtain the values of     //
// variable.                                                                  //
//                                                                            //
// Script is applied to a Game Object within the Unity scene editor. Once     //
// applied, the material type must be selected from a drop down menu. Default //
// is set to none.                                                            //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Object Material Enumerator 
    private enum objectMaterial
    {
        // No material
        None = 0,

        // Lambertian Material
        Lambertian = 1,

        // Metal
        Metal = 2,

        // Dielectric material
        Dielectric = 3,

        // Emissive material
        Emissive = 4,
    }

    // Material
    [SerializeField] private objectMaterial _materialType;

    // Game Object
    [SerializeField] private GameObject _object;

    // Colour multiplier
    [SerializeField, Range(0.0f, 2.0f)] private float _colourMultiplier = 1.0f;

    // Index of Refraction
    [SerializeField] private float _indexOfRefraction = 1.5f;

    // Emissive Power
    [SerializeField] private float _emissionPower = 1.0f;

    // Albedo Colour
    private Vector3 _albedo;

    // Metallic Property
    private Vector3 _metallic;

    // Smoothness
    private float _smoothness;

    // Emissive Colour
    private Vector3 _emission;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    /*
     * SET MATERIAL PROPERTIES METHOD
     * 
     * Method is used to set the material properties
     * based on the material attached to the game object
     */
    private void SetMaterialProperties()
    {
        // Obtain the material attached to the game object
        Material objectMat = _object.GetComponent<MeshRenderer>().material;

        // Albedo colour from material
        Color albedo = objectMat.color.linear;

        // Multiply
        albedo *= _colourMultiplier;

        // Set the metallic value of the sphere
        float metallic = objectMat.GetFloat("_Metallic");

        // Obtain the smoothness value
        float smoothness = objectMat.GetFloat("_Glossiness");

        // Set the albedo colour of the object
        _albedo = new Vector3(albedo.r, albedo.g, albedo.b);

        // Set the metallic value of the object
        _metallic = new Vector3(metallic, metallic, metallic);

        // Set the smoothness value of the object
        _smoothness = smoothness;

        // Check if the material is emissive
        if(_materialType == objectMaterial.Emissive)
        {
            // Obtain the emissive colour from the material
            Color emission = objectMat.GetVector("_EmissionColor");

            // Set the emissive colour
            _emission = new Vector3(emission.r, emission.g, emission.b);
        }
        // Else, set other materials to non-emissive values
        else
        {
            // Set the emissive colour
            _emission = new Vector3(0.0f, 0.0f, 0.0f);

            // Set the emissive power
            _emissionPower = 0.0f;
        }
    }

    /*
     * RETURN MATERIAL TYPE METHOD
     * 
     * Method returns the value held by the 
     * material type variable as an integer
     */
    public int ReturnMaterialType()
    {
        return (int)_materialType;
    }

    /*
     * RETURN ALBEDO METHOD
     * 
     * Method returns the albedo colour
     * of the material attached to the 
     * game object
     */
    public Vector3 ReturnAlbedo
    {
        get { return _albedo; }
    }

    /*
     * RETURN METALLIC METHOD
     * 
     * Method returns the metallic value
     * of the material attached to the 
     * game object
     */
    public Vector3 ReturnMetallic
    {
        get { return _metallic; }
    }

    /*
     * RETURN SMOOTHNESS METHOD
     * 
     * Method returns the smoothness value
     * of the material attached to the 
     * game object
     */
    public float ReturnSmoothness
    {
        get { return _smoothness;  }
    }

    /*
     * RETURN INDEX OF REFRACTION METHOD
     * 
     * Method returns the index of refraction
     * set for the material
     */
    public float ReturnIdxOfRef
    {
        get { return _indexOfRefraction; }
    }

    /*
     * RETURN EMISSION METHOD
     * 
     * Method returns the emissive
     * colour based on the material
     * attached to the object
     */
    public Vector3 ReturnEmission
    {
        get { return _emission; }
    }

    /*
     * RETURN EMISSION POWER METHOD
     * 
     * Method returns the emissive
     * power based on the material
     * attached to the object
     */
    public float ReturnEmissivePower
    {
        get { return _emissionPower; }
    }

    /*
     * AWAKE METHOD
     * 
     * Method is invoked when the script
     * is awoken
     */
    private void Awake()
    {
        // Obtain the game object
        _object = this.gameObject;

        // Set the material properties
        SetMaterialProperties();
    }
}

