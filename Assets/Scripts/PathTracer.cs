// ************************************************************************** //
//                                  PATH TRACER                               //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// PathTracer.cs                                                              //
//                                                                            //
// Main script that is used to carry out the path tracing function.           //
//                                                                            //
// In the Unity editor, the script has the denoising shader and a skybox      //
// texture automatically assigned. However, these can be replaced in each     //
// scene.                                                                     //
//                                                                            //
// In order to work, the script must be attached to a camera with a scene.    //
//                                                                            //
// Script first adds path traced spheres and mesh objects to the relevant     //
// lists, along with their material properties.                               //
//                                                                            //
// Once materials set, the compute shader is used to carry out path tracing   //
// calculations on the GPU.                                                   //
//                                                                            //
// If an object in the scene changes (removed or moved in the scene), the     //
// script automatically rebuilds the buffers.                                 //
//                                                                            //
// When the script is disabled, the buffers are erased.                       //
// ************************************************************************** //

// Libraries
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathTracer : MonoBehaviour
{
    // ************************************************************************** //
    //                                 VARIABLES                                  //
    // ************************************************************************** //

    // Intance
    private static PathTracer instance = null;

    // Camera
    private Camera _cam;

    // List of Sphere Game Objects
    private static List<GameObject> _sphereGameObjects = new List<GameObject>();

    // Sphere objects list
    private static List<Sphere> _sphereObjects = new List<Sphere>();

    // Objects need rebuilding boolean
    private static bool _spheresNeedRebuilding = false;

    // Mesh Object Buffer
    private ComputeBuffer _objectBuffer;

    // Vertex Buffer
    private ComputeBuffer _vertexBuffer;

    // Index Buffer
    private ComputeBuffer _indexBuffer;

    // Target Render Texture
    private RenderTexture _targetRenderTexture;

    // Render texture width
    [SerializeField] private int _rtWidth = 1280;

    // Render texture height
    [SerializeField] private int _rtHeight = 720;

    // Path Tracer ComputeShader
    public ComputeShader PathTracerShader;

    // Skybox texture
    public Texture _skyboxTexture;

    // Sphere buffer
    private ComputeBuffer _sphereBuffer;

    // Anti-Alliasing samples
    private uint _currentSample = 0;

    // Denoising shader
    [SerializeField] private Shader _denoisingShader;

    // Blending material
    private Material _denoisingMaterial;

    // Use Skybox Texture boolean boolean
    [SerializeField] private bool _useSkyboxTexture = false;

    // Use night skybox boolean
    [SerializeField] private bool _useNightSkybox = false;

    // Use vapourwave skybox boolean
    [SerializeField] private bool _useVaporwaveSkybox = false;

    // Use denoising shader
    private bool _useDenoisingShader = true;

    // Mesh object list
    private static List<MeshObject> _objects = new List<MeshObject>();

    // Verticies
    private static List<Vector3> _vertices = new List<Vector3>();

    // Indicies
    private static List<int> _indices = new List<int>();

    // Objects need rebuilding boolean
    private static bool _objectsNeedRebuilding = false;

    // Path Traced Object list
    private static List<PathTracedObject> _pathTracedObjects = new List<PathTracedObject>();

    // Random Number Generator
    private System.Random _randomNumberGenerator = new System.Random();

    // Max samples per pixel
    private const int _MAX_SPP = 20;

    // Max Ray Bounce Limit
    private const int _MAX_BOUNCE = 20;

    // ***** EDITOR - RAY SETTINGS ***** //
    [Header("Path Tracer Settings")]

    // Samples Per Pixel
    [SerializeField, Range(1, _MAX_SPP)] private int _samplesPerPixel = 8;

    // Ray Bounce Limit
    [SerializeField, Range(1, _MAX_BOUNCE)] private int _rayBounceLimit = 8;

    // ************************************************************************** //
    //                              DATA STRUCTURES                               //
    // ************************************************************************** //

    // Size of a float
    private int floatSize = 4;

    // Size of an integer
    private int intSize = 4;

    /*
     * SPHERE STRUCT
     * 
     * Struct containing variables for a 
     * sphere object.
     */
    public struct Sphere
    {
        // Sphere Origin (3 floats)
        public Vector3 centre;

        // Radius (1 float)
        public float radius;

        // Material Type (1 int)
        public int materialType;

        // Albedo (3 floats)
        public Vector3 albedo;

        // Metallic (3 floats)
        public Vector3 metallic;

        // Smoothness (1 float)
        public float smoothness;

        // Index of refraction (1 float)
        public float idxOfRef;

        // Emission colour (3 floats)
        public Vector3 emission;

        // Emissive Power (1 float)
        public float emissivePower;

        // Number of floats in struct = 16

        // Number of ints in struct = 1
    }

    // Number of floats in the sphere struct
    private int numFloatsSphereStruct = 16;

    // Number of ints in the sphere struct
    private int numIntsSphereStruct = 1;

    /*
     * MESH STRUCT
     * 
     * Struct containing variables for a 
     * mesh object.
     */
    struct MeshObject
    {
        // Mesh local to world matrix - (16 floats)
        public Matrix4x4 localToWorldMatrix;
	
	    // Indicies offset (1 int)
        public int indicesOffset;

        // Indicies count (1 int)
        public int indicesCount;

        // Material Type (1 int)
        public int materialType;

        // Albedo (3 floats)
        public Vector3 albedo;

        // Metallic (3 floats)
        public Vector3 metallic;

        // Smoothness (1 floats)
        public float smoothness;

        // Index of refraction (1 float)
        public float idxOfRef;

        // Emission colour (3 floats)
        public Vector3 emission;

        // Emissive Power (1 float)
        public float emissivePower;

        // Total number of floats = 28

        // Total number of ints = 3
    }

    // Number of floats in mesh struct
    private int numFloatsMeshStruct = 28;

    // Number of ints in mesh struct
    private int numIntsMeshStruct = 3;

    // ************************************************************************** //
    //                             SCRIPT METHODS                                 //
    // ************************************************************************** //

    /*
     * REGISTER OBJECT METHOD
     * 
     * Method is used to register a path traced object
     */
    public static void RegisterObject(PathTracedObject obj)
    {
        // Add object to the object list
        _pathTracedObjects.Add(obj);

        // Set objects need rebuilding boolean to true
        _objectsNeedRebuilding = true;
    }

    /*
     * UNREGISTER OBJECT METHOD
     * 
     * Method is used to unregister a path traced object
     */
    public static void UnregisterObject(PathTracedObject obj)
    {
        // Remove the object from the list
        _pathTracedObjects.Remove(obj);

        // Set objects need rebuilding boolean to true
        _objectsNeedRebuilding = true;
    }

    /*
     * REGISTER SPHERE METHOD
     * 
     * Method is used to register a path traced sphere
     */
    public static void RegisterSphere(GameObject aSphere)
    {
        // Add sphere to the list
        _sphereGameObjects.Add(aSphere);

        // Spheres need rebuilding
        _spheresNeedRebuilding = true;
    }

    /*
     * UNREGISTER SPHERE METHOD
     * 
     * Method is used to unregister a path traced sphere
     */
    public static void UnregisterSphere(GameObject aSphere)
    {
        // Remove sphere from the list
        _sphereGameObjects.Remove(aSphere);

        // Spheres need rebuilding
        _spheresNeedRebuilding = true;
    }

    /*
     * UPDATE METHOD
     * 
     * Method invokes on each frame update
     */
    private void Update()
    {
        // Check if the camera has changed position
        // or rotation
        if (_cam.transform.hasChanged == true)
        {
            // Reset current sample to 0
            ResetCurrentSample();

            // Change the camera transform to false
            _cam.transform.hasChanged = false;
        }

        // Check each sphere in the sphere list
        foreach (PathTracedObject anObject in _pathTracedObjects)
        {
            // Check the sphere has changed position, rotation, etc
            if(anObject.gameObject.transform.hasChanged == true)
            {
                // Change the value of "has changed" to false
                anObject.gameObject.transform.hasChanged = false;

                // Set Objects Need Rebuilding boolean to true
                _objectsNeedRebuilding = true;
            }
        }

        // Check each sphere in the game object array
        foreach (GameObject aSphere in _sphereGameObjects)
        {
            // Check the sphere has changed position, rotation, etc
            if (aSphere.transform.hasChanged == true)
            {
                // Change the value of "has changed" to false
                aSphere.transform.hasChanged = false;

                // Set spheres need rebuilding to true
                _spheresNeedRebuilding = true;
            }
        }
    }

    /*
     * ON DISABLE METHOD
     * 
     * Method invokes when script is disabled
     */
    private void OnDisable()
    {
        // Check if the sphere buffer exists
        if (_sphereBuffer != null)
        {
            // Release the buffer
            _sphereBuffer.Release();
        }

        // Check if object buffer exists
        if (_objectBuffer != null)
        {
            // Release the buffer
            _objectBuffer.Release();
        }

        // Check if vertex buffer exists
        if (_vertexBuffer != null)
        {
            // Release the buffer
            _vertexBuffer.Release();
        }

        // Check if index buffer exists
        if (_indexBuffer != null)
        {
            // Release the buffer
            _indexBuffer.Release();
        }
    }

    /*
     * AWAKE METHOD
     * 
     * Method is invoked when the game object the script
     * is attached to is woken up
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

        // Obtain the camera component
        _cam = gameObject.GetComponent<Camera>();
    }

    /*
     * ON RENDER IMAGE METHOD
     * 
     * Standard Unity method. Called after rendering an image is complete
     */
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Rebuild Sphere buffer
        RebuildSphereBuffer();

        // Rebuild object buffers
        RebuildObjectBuffers();

        // Set shader parameters
        SetShaderParameters();

        // Render the scene
        RenderScene(destination);
    }

    /*
     * INITIALISE RENDER TEXTURE METHOD
     * 
     * Method initialises the target render texture if one doesn't
     * exist, or if the width of height of a 
     */
    private void InitialiseRenderTexture()
    {
        // Check if the target render target doesn't exist or if the dimensions of an existing render texture
        // do not match the dimensions of the screen
        if((_targetRenderTexture == null) || (_targetRenderTexture.width != _rtWidth) || (_targetRenderTexture.height != _rtHeight) )
        {
            // Check if a render texture exists
            if(_targetRenderTexture != null)
            {
                // Release the render texture, a new one will be made
                _targetRenderTexture.Release();
            }

            // Set the target render texture to a new render texture
            _targetRenderTexture = new RenderTexture(_rtWidth, _rtHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

            // Set render texture width to match the width of the screen
            _targetRenderTexture.width = _rtWidth;

            // Set render texture height to match the height of the screen
            _targetRenderTexture.height = _rtHeight;

            // Enable random write on the target render texture to ensure it can be written to
            _targetRenderTexture.enableRandomWrite = true;

            // Create the target render texture
            _targetRenderTexture.Create();

            // Set current samples to 0
            ResetCurrentSample();
        }
    }

    /*
     * RENDER SCENE METHOD
     * 
     * Method for rendering the scene
     */
    private void RenderScene(RenderTexture aRenderTexture)
    {
        // Initialise the render texture
        InitialiseRenderTexture();

        // Set the texture of the path tracer to the target render texture
        PathTracerShader.SetTexture(0, "_result", _targetRenderTexture);

        // Determine the number of threads to process over the image width
        int threadsX = Mathf.CeilToInt(_rtWidth / 8);

        // Determine the number of threads to process over the image height
        int threadsY = Mathf.CeilToInt(_rtHeight / 8);

        // Dispatch the threads in X and Y to the PathTracer Shader
        PathTracerShader.Dispatch(0, threadsX, threadsY, 1);

        // Check if the denoising material doesn't exist
        if (_denoisingMaterial == null)
        {
            // Doesn't exist, create a new material from the shader
            _denoisingMaterial = new Material(_denoisingShader);
        }

        // Pass sample to the denoising shader
        _denoisingMaterial.SetFloat("_currentSample", _currentSample);

        // Check if the blending shader boolean is true
        if(_useDenoisingShader == true)
        {
            // Boolean is true, Graphics Blit the target render texture, 
            // destination render texture and result of the denoising shader
            Graphics.Blit(_targetRenderTexture, aRenderTexture, _denoisingMaterial);
        }
        else 
        {
            // Boolean is false, Graphics Blit the target render texture 
            // and the destination render texture
            Graphics.Blit(_targetRenderTexture, aRenderTexture);
        }

        // Increment the current sample by 1
        _currentSample++;
    }

    /*
     * SET SHADER PARAMETERS METHOD
     * 
     * Method is used to set shader parameters, based on the camera
     * and objects in the virtual environment
     */
    private void SetShaderParameters()
    {
        // Pass camera world matrix to the shader
        PathTracerShader.SetMatrix("_camWorldMatrix", _cam.cameraToWorldMatrix);

        // Pass the inverse of the camera projection matrix to the path shader
        PathTracerShader.SetMatrix("_camInverseProjection", _cam.projectionMatrix.inverse);

        // Pass the skybox to the shader
        PathTracerShader.SetTexture(0, "_SkyboxTexture", _skyboxTexture);

        // Set sphere buffer
        SetComputeBuffer("_spheres", _sphereBuffer);

        // Set object buffer
        SetComputeBuffer("_objects", _objectBuffer);

        // Set vertices buffer
        SetComputeBuffer("_vertices", _vertexBuffer);

        // Set Indices buffer
        SetComputeBuffer("_indices", _indexBuffer);

        // Pass sphere list to the shader
        PathTracerShader.SetBool("_useSkyboxTexture", _useSkyboxTexture);

        // Pass sphere list to the shader
        PathTracerShader.SetBool("_useNightSkybox", _useNightSkybox);
        
        // Pass sphere list to the shader
        PathTracerShader.SetBool("_useVaporwaveSkybox", _useVaporwaveSkybox);

        // Pass the samples per pixel to the shader
        PathTracerShader.SetInt("_samplesPerPixel", _samplesPerPixel);

        // Pass the ray bounce limit to the shader
        PathTracerShader.SetInt("_rayBounceLimit", _rayBounceLimit);

        // Pass random value to the shader
        PathTracerShader.SetFloat("_randomNumber", (float)_randomNumberGenerator.NextDouble());
    }

    /*
     * REBUILD SPHERE BUFFER METHOD
     * 
     * When invoked, due to mesh moving in someway,
     * the method rebuilds the list and buffer 
     * related to sphere objects
     */
    private void RebuildSphereBuffer()
    {
        // Check if spheres need rebuilding
        if(_spheresNeedRebuilding == false)
        {
            // Spheres do not need rebuilding
            return;
        }

        // Set spheres need rebuilding to false
        _spheresNeedRebuilding = false;

        // Set current sample to false
        ResetCurrentSample();

        // Clear the sphere object list
        _sphereObjects.Clear();

        // Iterate over all of the game objects with the tag "Sphere" attached
        for (int i = 0; i < _sphereGameObjects.Count; i++)
        {
            // Obtain the object properties of the sphere game object
            ObjectProperties sphereProperties = _sphereGameObjects[i].GetComponent<ObjectProperties>();

            // Add the sphere object to the list and set each property
            _sphereObjects.Add(new Sphere()
            {
                // Set local to world matrix
                centre = _sphereGameObjects[i].transform.position,

                // Set the first index
                radius = _sphereGameObjects[i].transform.localScale.x / 2.0f,

                // Set the material type
                materialType = sphereProperties.ReturnMaterialType(),

                // Set the albedo colour
                albedo = sphereProperties.ReturnAlbedo,

                // Set the metallic value
                metallic = sphereProperties.ReturnMetallic,

                // Set the smoothness value
                smoothness = sphereProperties.ReturnSmoothness,

                // Set the index of refraction
                idxOfRef = sphereProperties.ReturnIdxOfRef,

                // Set the emissive colour
                emission = sphereProperties.ReturnEmission,

                // Set the emissive power
                emissivePower = sphereProperties.ReturnEmissivePower,

            });
        }

        // Sphere struct data size
        int _sphereStructDataSize = (numFloatsSphereStruct * floatSize) + (numIntsSphereStruct * intSize);

        // Create Sphere buffer
        CreateComputeBuffer(ref _sphereBuffer, _sphereObjects, _sphereStructDataSize);
    }

    /*
     * REBUILD OBJECT BUFFERS METHOD
     * 
     * When invoked, due to mesh moving in someway,
     * the method rebuilds the relevant lists and
     * buffers related to mesh objects 
     */
    private void RebuildObjectBuffers()
    {
        // Check if the objects need rebuilding
        if (_objectsNeedRebuilding == false)
        {
            // Value is false, return
            return;
        }

        // Set Objects need rebuilding to false
        _objectsNeedRebuilding = false;

        // Set current sample to false
        ResetCurrentSample();

        // Clear the object list
        _objects.Clear();

        // Clear the vertices list
        _vertices.Clear();

        // Clear the indices list
        _indices.Clear();

        // Loop over all objects and gather their data
        foreach (PathTracedObject anObject in _pathTracedObjects)
        {
            // Obtain the mesh from the object
            Mesh mesh = anObject.GetComponent<MeshFilter>().sharedMesh;

            // Add vertex data
            int firstVertex = _vertices.Count;

            // Add the vertices from the mesh to the vertices list
            _vertices.AddRange(mesh.vertices);

            // Add index data
            int firstIndex = _indices.Count;

            // Obtain the indices from the mesh
            var indices = mesh.GetIndices(0);

            // Add indices to the indices list
            _indices.AddRange(indices.Select(index => index + firstVertex));

            // Obtain the object properties of the sphere game object
            ObjectProperties objectProperties = anObject.GetComponent<ObjectProperties>();

            // Add the object itself
            _objects.Add(new MeshObject()
            {
                // Set local to world matrix
                localToWorldMatrix = anObject.transform.localToWorldMatrix,

                // Set the first index
                indicesOffset = firstIndex,

                // Set the indices count
                indicesCount = indices.Length,

                // Set the material type
                materialType = objectProperties.ReturnMaterialType(),

                // Set the albedo colour
                albedo = objectProperties.ReturnAlbedo,

                // Set the metallic value
                metallic = objectProperties.ReturnMetallic,

                // Set the smoothness value
                smoothness = objectProperties.ReturnSmoothness,

                // Set the index of refraction
                idxOfRef = objectProperties.ReturnIdxOfRef,

                // Set the emissive colour
                emission = objectProperties.ReturnEmission,

                // Set the emissive power
                emissivePower = objectProperties.ReturnEmissivePower,
            });
        }

        // Mesh object struct
        int _meshStructDataSize = (numFloatsMeshStruct * floatSize) + (numIntsMeshStruct * intSize);

        // Create Object buffer
        CreateComputeBuffer(ref _objectBuffer, _objects, _meshStructDataSize);

        // Create vertex buffer
        CreateComputeBuffer(ref _vertexBuffer, _vertices, 12);

        // Create index buffer
        CreateComputeBuffer(ref _indexBuffer, _indices, 4);
    }

    /*
     * CREATE COMPUTE BUFFER TEMPLATE
     * 
     * Template is used to create a compute buffer from 
     * 3 arguments: the buffer, the data, and the data stride
     */
    private static void CreateComputeBuffer<T>(ref ComputeBuffer buffer, List<T> data, int stride)
    where T : struct
    {
        // Check if a compute buffer already exists
        if (buffer != null)
        {
            // Compute buffer exists, check if the data count is 0, the buffer count doesn't match the data count
            // or if the data stride does not match the buffer stride
            if (data.Count == 0 || buffer.count != data.Count || buffer.stride != stride)
            {
                // Release the buffer
                buffer.Release();

                // Set the buffer to null
                buffer = null;
            }
        }

        // Check if the data count does not equal 0
        if (data.Count != 0)
        {
            // Check if the buffer is null
            if (buffer == null)
            {
                // Create a new compute buffer with the data count and stride
                buffer = new ComputeBuffer(data.Count, stride);
            }

            // Set the data to the buffer
            buffer.SetData(data);
        }
    }

    /*
     * SET COMPUTE BUFFER
     */
    private void SetComputeBuffer(string name, ComputeBuffer buffer)
    {
        if (buffer != null)
        {
            PathTracerShader.SetBuffer(0, name, buffer);
        }
    }

    /*
     * STANDARD SKYBOX METHOD
     * 
     * Method is used to enable the standard skybox (blend
     * from white to blue).
     * 
     * Method sets vaporwave skybox boolean, night skybox
     * boolean and the texture boolean to false
     */
    public void EnableStandardSkybox()
    {
        // Set use vaporwave skybox to false
        _useVaporwaveSkybox = false;

        // Set use skybox boolean to false
        _useSkyboxTexture = false;

        // Set use night sky boolean to false
        _useNightSkybox = false;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * V A P O R W A V E SKYBOX METHOD
     * 
     * Method is used to enable the v a p o... 
     * I'm ending this joke from now on.
     * 
     * Method is used to enable the Vaporwave 
     * skybox (blend from cyan to magenta).
     * 
     * Method sets vaporwave skybox boolean to true,
     * night skybox boolean to true,
     * and skybox texture boolean to false
     */
    public void EnableVaporwaveSkybox()
    {
        // Set use vaporwave skybox to false
        _useVaporwaveSkybox = true;

        // Set use skybox boolean to false
        _useSkyboxTexture = false;

        // Set use night sky boolean to false
        _useNightSkybox = false;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * TEXTURED SKYBOX METHOD
     * 
     * Method is used to enable the textured skybox 
     * (a PNG texture that wraps around the scene
     * pretty well).
     * 
     * Method sets vaporwave skybox boolean to true,
     * night skybox boolean to false,
     * and skybox texture boolean to false
     */
    public void EnableTexturedSkybox()
    {
        // Set use skybox boolean to false
        _useSkyboxTexture = true;

        // Set use vaporwave skybox to false
        _useVaporwaveSkybox = false;

        // Set use night sky boolean to false
        _useNightSkybox = false;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * TEXTURED SKYBOX METHOD
     * 
     * Method is used to enable the night skybox 
     * (a blend from light purple to dark purple).
     * 
     * Method sets vaporwave skybox boolean to true,
     * night skybox boolean to false
     * and skybox texture boolean to false
     */
    public void EnableNightSkybox()
    {
        // Set use night sky boolean to false
        _useNightSkybox = true;

        // Set use vaporwave skybox to false
        _useVaporwaveSkybox = false;

        // Set use skybox boolean to false
        _useSkyboxTexture = false;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * ENABLE DENOISING SHADER METHOD
     * 
     * Method is used to enable the denoising
     * shader
     */
    public void EnableDenoisingShader()
    {
        // Set use denoising shader to true
        _useDenoisingShader = true;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * DISABLE DENOISING SHADER METHOD
     * 
     * Method is used to disable the denoising
     * shader
     */
    public void DisableDenoisingShader()
    {
        // Set use denoising shader to true
        _useDenoisingShader = false;

        // Set current sample to 0
        ResetCurrentSample();
    }

    /*
     * GET CURRENT SAMPLE METHOD
     * 
     * Returns the value held by 
     * current sample
     */
    public int GetCurrentSample
    {
        get { return _samplesPerPixel;  }
    }

    public int GetCurrentBounceLimit
    {
        get { return _rayBounceLimit;  }
    }



    public void ResetCurrentSample()
    {
        _currentSample = 0;
    }

    public void IncrementRayBounceLimit()
    {
        _rayBounceLimit++;
    }

    public void DecrementRayBounceLimit()
    {
        _rayBounceLimit--;
    }

    public void IncrementSamplesPerPixel()
    {
        _samplesPerPixel++;
    }

    public void DecrementSamplesPerPixel()
    {
        _samplesPerPixel--;
    }

    public int GetMaxSampleLimit
    {
        get
        {
            return _MAX_SPP;
        }

    }

    public int GetMaxBounceLimit
    {
        get
        {
            return _MAX_BOUNCE;
        }
    }
}






















































// You're not supposed to be here. Go away.