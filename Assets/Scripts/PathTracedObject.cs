// ************************************************************************** //
//                               PLAYER CAMERA                                //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// PathTracedObject.cs                                                        //
//                                                                            //
// Script contains the relevant methods for registering a game object as a    //
// path traced item.                                                          //
//                                                                            //
// Method is attached to a game object within the unity editor and an object  //
// type must be selected from a dropdown menu. By default, the object type is //
// set to sphere.                                                             //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component requirements
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent(typeof(GameObject))]
public class PathTracedObject : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Object type ennumberator
    private enum objectType
    {
        // No material
        Sphere = 0,

        // Lambertian Material
        Mesh = 1,
    }

    // Object Type
    [SerializeField] private objectType _objectType;

    // ************************************************************************** //
    //                              SCRIPT METHODS                                //
    // ************************************************************************** //

    /*
     * ON ENABLE METHOD
     * 
     * Method is invoked when the script
     * is enabled
     */
    private void OnEnable()
    {
        // Check the object type
        if(_objectType == objectType.Sphere)
        {
            // Sphere object, invoke Register Sphere function in path tracer
            PathTracer.RegisterSphere(this.gameObject);
        }
        else if(_objectType == objectType.Mesh)
        {
            // Mesh object, invoke Register Object method in path tracer
            PathTracer.RegisterObject(this);
        }
    }

    /*
     * ON DISABLE METHOD
     * 
     * Method is invoked when the script
     * is disabled
     */
    private void OnDisable()
    {
        // Check object tyoe
        if (_objectType == objectType.Sphere)
        {
            // Sphere object, invoke Unregister Sphere function in path tracer
            PathTracer.UnregisterSphere(this.gameObject);
        }
        else if (_objectType == objectType.Mesh)
        {
            // Mesh object, invoke Unregister Object method in path tracer
            PathTracer.UnregisterObject(this);
        }

    }
}
