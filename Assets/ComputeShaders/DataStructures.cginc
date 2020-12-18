// ************************************************************************** //
//                               DATA STRUCTURES                              //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// DataStructures.cginc                                                       //
//                                                                            //
// CGINC file containing structs and methods for rays, hit records, spheres,  //
// and mesh objects used in path tracer.                                      //
// ************************************************************************** //

// ************************************************************************** //
//                  SHADER DEFINITIONS AND INPUT VARIABLES                    //
// ************************************************************************** //

// Infinity
static const float INFINITY = 1.#INF;

// Epsilon value
static const float EPSILON = 0.001f;

// Vertices buffer
StructuredBuffer<float3> _vertices;

// Indicies buffer
StructuredBuffer<int> _indices;

// Rays per pixel
int _rayBounceLimit;

// ************************************************************************** //
//                                    RAY                                     //
// ************************************************************************** //

/*
 * RAY STRUCT
 *
 * Struct for rays containing variables
 * for ray origin and direction
 */
struct Ray
{
	// Ray origin
	float3 origin;
	
	// Ray direction
	float3 direction;

	 // At point function declaration
	float3 AtPoint(float t);
};

/*
 * AT POINT FUNCTION
 *
 * Function is used to determine
 * the point where a ray has
 * collided with an object
 */
float3 Ray::AtPoint(float t)
{
	// At point vector
	float3 atPoint;
		
	// Determine the point of impact
	atPoint = origin + t * direction;
	
	// Return the point of impact
	return atPoint;
}

/*
 * CREATE RAY
 *
 * Method creates a ray and sets it origin
 * and direction
 */
Ray CreateRay(float3 anOrigin, float3 aDireciton)
{
	// New ray
	Ray aRay;
	
	// Set ray origin
	aRay.origin = anOrigin;

	// Set direciton
	aRay.direction = aDireciton;
	
	// Return ray
	return aRay;
}

// ************************************************************************** //
//                                HIT RECORD                                  //
// ************************************************************************** //

/*
 * HIT RECORD STRUCT
 *
 * Struct for hit record containing variables
 * for point of hit, normal at hit and 
 * the distance of the hit
 */
struct HitRecord
{
	// Hit point
	float3 hitPoint;
	
	// Hit Normal
	float3 hitNormal;
	
	// Hit Distance
	float hitDistance;
	
	// Front face boolean
	bool frontFace;
	
	// Material
	int hitMaterialType;
	
	// Albedo
	float3 hitAlbedo;
	
	// Metallic
	float3 hitMetallic;
	
	// Smoothness
	float hitSmoothness;
	
	// Index of Refraction
	float hitIdxOfRef;
	
	// Emissive colour
	float3 hitEmission;
	
	// Emissive Power
	float hitEmissivePower;
};

/*
 * INITIALISE HIT RECORD FUNCTION
 *
 * Function is used to initialise a hit
 * record before path tracing is carried
 * out
 */
HitRecord InitialiseHitRecord()
{
	// Create a hit record
	HitRecord aHit;
	
	// Set the hitpoint to 0.0f, 0.0f, 0.0f
	aHit.hitPoint = float3(0.0f, 0.0f, 0.0f);
	
	// Set distance to a stupidly high value
	aHit.hitDistance = 1.#INF;
	
	// Set Hit Normal
	aHit.hitNormal = float3(0.0f, 0.0f, 0.0f);
	
	// Set hit face
	aHit.frontFace = false;
	
	// Set material type (Lambertian by default)
	aHit.hitMaterialType = 0;
	
	// Set the albedo
	aHit.hitAlbedo = float3(1.0f, 0.0f, 0.0f);
	
	// Set the metallic value
	aHit.hitMetallic = float3(1.0f, 0.0f, 0.0f);
	
	// Set the smoothness value
	aHit.hitSmoothness = 0.0f;
	
	// Set the index of refraction
	aHit.hitIdxOfRef = 0.0f;
	
	// Set the emission
	aHit.hitEmission = float3(0.0f, 0.0f, 0.0f);
	
	// Set the emissive power
	aHit.hitEmissivePower = 0.0f;
	
	// Return hit
	return aHit;
};

// ************************************************************************** //
//                                  SPHERE                                    //
// ************************************************************************** //

/*
 * SPHERE STRUCT
 *
 * Struct for sphere containing variables
 * for sphere position and radius
 */
struct Sphere
{
	// Sphere position
	float3 centre;
	
	// Sphere radius
	float radius;
	
	// Material Type
	int materialType;
	
	// Sphere albedo
	float3 albedo;
	
	// Sphere metallic
	float3 metallic;
	
	// Sphere smoothness
	float smoothness;
	
	// Index of refraction
	float idxOfRef;
	
	// Emission
	float3 emission;
	
	// Emissive power
	float emissivePower;
	
	// Hit sphere function declaration
	bool Hit(Ray aRay, float tMin, float tMax, out HitRecord aRecord);
};

/*
 * HIT SPHERE FUNCTION
 *
 * Method is used to determine if a sphere has been hit.
 * Returns false if no sphere has been hit by the ray, or returns 
 * true value if the sphere has been 
 */
bool Sphere::Hit(Ray aRay, float tMin, float tMax, out HitRecord aRecord)
{
	// Initialise Hit Record
	aRecord = InitialiseHitRecord();
	
	// Set material type
	aRecord.hitMaterialType = materialType;
				
	// Set the albedo of the hit
	aRecord.hitAlbedo = albedo;
				
	// Set the metallic value
	aRecord.hitMetallic = metallic;
				
	// Set the smoothness value
	aRecord.hitSmoothness = smoothness;
	
	// Set the index of refraction
	aRecord.hitIdxOfRef = idxOfRef;
	
	// Set emission
	aRecord.hitEmission = emission;
	
	// Set emissive power
	aRecord.hitEmissivePower = emissivePower;
	
	// Determine vector from ray origin to sphere centre
	float3 rayToSphere = aRay.origin - centre;
	
	// ***** b^2 - 4 * a * c ***** //
	
	// 'a' component - ray direction * ray direction
	float a = length(aRay.direction) * length(aRay.direction);
	
	// 'b' component - angle between rayToSphere vector and ray direction
	float bHalf = dot(rayToSphere, aRay.direction);
	
	// 'c' component
	float c = (length(rayToSphere) * length(rayToSphere)) - (radius * radius);
	
	// Determine the value of the discriminant
	float discriminant = (bHalf * bHalf) - (a * c);

	// Check if discriminant is greater than 0
	if (discriminant > 0)
	{
		// Sphere has been hit, determine the square root of the determinant
		float root = sqrt(discriminant);
		
		// Determine temporary distance with negative root value
		float temp = (-bHalf - root) / a;
		
		// Check if the temporary distance is smaller than the maximum distance
		if (temp < tMax && temp > tMin)
		{
			// Set hit record distance
			aRecord.hitDistance = temp;
			
			// Set point of collision of the hit record
			aRecord.hitPoint = aRay.AtPoint(temp);
			
			// Obtain the outward normal
			float3 outwardNormal = ((aRecord.hitPoint - centre) / radius);
			
			// Front face boolean
			bool frontFace;
			
			// Check if the dot product between ray direction and outward normal
			if (dot(aRay.direction, outwardNormal) < 0.0f)
			{
				// Set front face check to false (Outward)
				frontFace = true;
			}
			else
			{
				// Set front face check to true (Inward)
				frontFace = false;
			}
			
			// Set the front face of the hit record
			aRecord.frontFace = frontFace;
			
			// Check the value of front face
			if (aRecord.frontFace == true)
			{
				// Front face, set normal to the outward normal
				aRecord.hitNormal = outwardNormal;
			}
			else
			{
				// Back face, set the normal to the inverse of the outward normal
				aRecord.hitNormal = -outwardNormal;
			}
			
			// Set sphere hit to true
			return true;
		}

		// Determine temporary distance with positive root value
		temp = (-bHalf + root) / a;
		
		// Check if the temporary distance is smaller than the maximum distance
		if (temp < tMax && temp > tMin)
		{
			// Set hit record distance
			aRecord.hitDistance = temp;
			
			// Set hit recrod point
			aRecord.hitPoint = aRay.AtPoint(temp);
			
			// Obtain the outward normal
			float3 outwardNormal = (aRecord.hitPoint - centre) / radius;

			// Front face boolean
			bool frontFace;
			
			// Check if the dot product between ray direction and outward normal
			if (dot(aRay.direction, outwardNormal) < 0.0f)
			{
				// Set front face check to false (Outward)
				frontFace = true;
			}
			else
			{
				// Set front face check to true (Inward)
				frontFace = false;
			}
			
			// Set the front face of the hit record
			aRecord.frontFace = frontFace;
			
			// Check the value of front face
			if (aRecord.frontFace == true)
			{
				// Front face, set normal to the outward normal
				aRecord.hitNormal = outwardNormal;
			}
			else
			{
				// Back face, set the normal to the inverse of the outward normal
				aRecord.hitNormal = -outwardNormal;
			}
			
			// Set sphere hit to true
			return true;
		}
	}
	
	// Return the value of sphere hit
	return false;
}

// ************************************************************************** //
//                                 MESH OBJECT                                //
// ************************************************************************** //

/*
 * MESH OBJECT STRUCT
 *
 * Struct for sphere containing variables
 * for sphere position and radius
 */
struct MeshObject
{
	// Local to World Matrix
	float4x4 localToWorldMatrix;
	
	// Indices offset
	int indicesOffset;
	
	// Indices count
	int indicesCount;
	
	// Material Type
	int materialType;
	
	// Sphere albedo
	float3 albedo;
	
	// Sphere metallic
	float3 metallic;
	
	// Sphere smoothness
	float smoothness;
	
	// Index of refraction
	float idxOfRef;
	
	// Emission
	float3 emission;
	
	// Emissive power
	float emissivePower;
	
	// Hit mesh function declaration
	bool Hit(Ray ray, float tMin, float tMax, out HitRecord bestHit);
};

/*
 * HIT TRIANGLE FUNCTION
 *
 * Function is used to determine if a triangle
 * has been hit by a ray
 */
bool HitTriangle(Ray aRay, float3 vert1, float3 vert2, float3 vert3, inout float t, inout float u, inout float v)
{
	// First Edge
	float3 edge1 = vert2 - vert1;
	
	// Second Edge
	float3 edge2 = vert3 - vert1;
	
	// Obtain the vector between the ray and the second edge
	float3 pVec = cross(aRay.direction, edge2);
	
	// Determine the determinant
	float determinant = dot(edge1, pVec);
	
	// Check if the determinant is less than Epsilon
	if (determinant < EPSILON)
	{
		// Return false
		return false;
	}

	// Inverse the determinant
	float inverseDet = 1.0f / determinant;
	
	// Distance from the ray origin to vertex 1
	float3 tVec = aRay.origin - vert1;
	
	// Determine the U parameter
	u = dot(tVec, pVec) * inverseDet;
	
	// Check if U parameter is outwith the range
	if (u < 0 || u > 1.0f)
	{
		// Outwith range, return false
		return false;
	}
	
	// Obtain vector between tVec and edge 1
	float3 qVec = cross(tVec, edge1);
	
	// Determine the V parameter
	v = dot(aRay.direction, qVec) * inverseDet;
	
	// Check if v parameter is outside the range
	if (v < 0 || u + v > 1.0f)
	{
		// Outwith range, return false
		return false;
	}
	
	// Determine the distance of the ray that has hit the triangle
	t = dot(edge2, qVec) * inverseDet;
	
	// Return true
	return true;
}

/*
 * HIT MESH FUNCTION
 *
 * Function is used to determine if a 
 */
bool MeshObject::Hit(Ray ray, float tMin, float tMax, out HitRecord aRecord)
{
	// Initialise best hit
	aRecord = InitialiseHitRecord();
	
	// Set material type
	aRecord.hitMaterialType = materialType;
				
	// Set the albedo of the hit
	aRecord.hitAlbedo = albedo;
				
	// Set the metallic value
	aRecord.hitMetallic = metallic;
				
	// Set the smoothness value
	aRecord.hitSmoothness = smoothness;
				
	// Set front face to true
	aRecord.frontFace = true;
	
	// Set the index of refraction
	aRecord.hitIdxOfRef = idxOfRef;
	
	// Set the hit emission
	aRecord.hitEmission = emission;
	
	// Set
	aRecord.hitEmissivePower = emissivePower;

	// Obtain the offset
	uint offset = indicesOffset;
	
	// Obtain the count
	uint count = offset + indicesCount;
	
	// Iterate over all vertices in the object
	for (uint i = offset; i < count; i += 3)
	{
		// Determine Vertex 1
		float3 v1 = (mul(localToWorldMatrix, float4(_vertices[_indices[i]], 1))).xyz;
		
		// Determine Vertex 2
		float3 v2 = (mul(localToWorldMatrix, float4(_vertices[_indices[i + 1]], 1))).xyz;
		
		// Determine Vertex 3
		float3 v3 = (mul(localToWorldMatrix, float4(_vertices[_indices[i + 2]], 1))).xyz;
		
		// Distance
		float t;
		
		// U Parameter
		float u;
		
		// V Parameter
		float v;
		
		// Check if the triangle of the vertices have been hit
		if (HitTriangle(ray, v1, v2, v3, t, u, v) == true)
		{
			// Check if the value of t is greater than 0, but smaller than the best hit
			if (t < tMax && t > tMin)
			{
				// Set the distance of the best hit
				aRecord.hitDistance = t;
				
				// Set the point of the hit
				aRecord.hitPoint = ray.origin + t * ray.direction;
				
				// Set the normal of the hit
				aRecord.hitNormal = normalize(cross(v2 - v1, v3 - v1));
				
				// Mesh has been hit, return true
				return true;
			}
		}
	}
	
	// Mesh has been hit, return false
	return false;
}