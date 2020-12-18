// ************************************************************************** //
//                                   SHADING                                  //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// Shading.cginc                                                              //
//                                                                            //
// CGINC file containing methods to determine the shading of scattered rays.  //
// ************************************************************************** //

// ************************************************************************** //
//                  SHADER DEFINITIONS AND INPUT VARIABLES                    //
// ************************************************************************** //

// Material Definitions
#define matNone 0
#define matLambertian 1
#define matMetallic 2
#define matDielectric 3
#define matEmissive 4

// CGINC Files
#include "DataStructures.cginc"
#include "Sampling.cginc"

// Use Skybox Boolean
bool _useSkyboxTexture;

// Use vapourwave skybox
bool _useVaporwaveSkybox;

// Use night skybox
bool _useNightSkybox;

// Skybox texture
Texture2D<float4> _SkyboxTexture;

// Skybox sampler
SamplerState sampler_SkyboxTexture;

// ************************************************************************** //
//                      SCATTERED RAY SHADING FUNCTIONS                       //
// ************************************************************************** //

/*
 * SKY BOX FUNCTION
 *
 * Function is used to return the colour
 * of the skybox, based on the direction of
 * a ray and the status of a few sky box
 * booleans
 */
float3 SkyBox(Ray aRay)
{
	// Sky colour
	float3 skyColour;

	// Obtain the unit direction based on the ray
	float3 unitDirection = normalize(aRay.direction);
	
	// Check the value of the skybox boolean
	if (_useSkyboxTexture == true)
	{
		// Check is true, use skybox texture
			
		// **** SKYBOX TEXTURE **** //
		
		// Determine theta
		float theta = acos(unitDirection.y) / -PI;
	
		// Determine phi
		float phi = atan2(unitDirection.x, -unitDirection.z) / -PI * 0.5f;
	
		// Sample the skybox as radial values
		skyColour = _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0).xyz;
	}
	else
	{
		// Skybox check is false, use one of the following colour gradients

		// Determine the distance of the ray based on the unit direction
		float dist = 0.5f * (unitDirection.y + 1.0f);
			
		// Check if the Use V A P O R W A V E skybox is enabled
		if (_useVaporwaveSkybox == true)
		{
			// **** V A P O R W A V E SKY GRADIENT **** //

			// Set to a vapourwave sky (blend between magenta and light cyan, based on the distance of the ray)
			skyColour = (1.0f - dist) * float3(0.0f, 1.0f, 1.0f) + dist * float3(1.0f, 0.0f, 1.0f);
		}
		else if (_useNightSkybox == true)
		{
			// **** NIGHT SKYBOX GRADIENT **** //
				
			// Set to a night sky (blend between a light purple to a dark purle)
			skyColour = (1.0f - dist) * float3(170.0f / 255.0f, 104.0f / 255.0f, 191.0f / 255.0f) + dist * float3(11.0f / 255.0f, 20.0f / 255.0f, 53.0f / 255.0f);
		}
		else
		{
			// **** DEFUALT SKY GRADIENT **** //
				
			// Set to a regular sky (blend between white and light blue, based on the distance of the ray)
			skyColour = (1.0f - dist) * float3(1.0f, 1.0f, 1.0f) + dist * float3(0.5f, 0.7f, 1.0f);
		}
	}
	
	// Return Sky Colour
	return skyColour;
}

/*
 * LAMBERTIAN SCATTER FUNCTION
 *
 * Function is used to determine if a ray can be
 * scattered if the material is lambertain.
 *
 * Function also modifies the ray and the attenuation
 */
bool LambertianScatter(inout Ray aRay, inout HitRecord aRecord, inout float3 attenuation)
{
	// Ray scattered boolean initialised to false
	bool rayScattered = false;
	
	// Check if the material is lambertian
	if(aRecord.hitMaterialType == matLambertian)
	{
		// Change the origin of the ray to the point of impact and add a small value to alter the direction of the ray
		aRay.origin = aRecord.hitPoint + (EPSILON * aRecord.hitNormal);
		
		// Set the direction of the scattered ray
		aRay.direction = aRecord.hitNormal + RandomInHemisphere(aRecord.hitNormal);
		
		// Set attenuation to the hit record albedo
		attenuation = aRecord.hitAlbedo;

		// Set ray scatter boolean to true
		rayScattered = true;
	}
	
	// Return the value of the scatter lambertian boolean
	return rayScattered;
}

/*
 * METALLIC SCATTER FUNCTION
 *
 * Function is used to determine if a ray can be
 * scattered if the material is metallic.
 *
 * Function also modifies the ray and the attenuation
 */
bool MetallicScatter(inout Ray aRay, inout HitRecord aRecord, inout float3 attenuation)
{
	// Scatter metal boolean initialised to false
	bool rayScattered = false;
	
	// Check if the material is metallic
	if (aRecord.hitMaterialType == matMetallic)
	{
		// Material is metallic, carry out caluclations 
		
		// Determine the roughness based on the smoothness of the hit record
		float roughness = 1.0 - aRecord.hitSmoothness;
		
		// Determine the reflected vector
		float3 reflected = reflect(normalize(aRay.direction), aRecord.hitNormal);
		
		// Set the direction of the scattered ray
		aRay.direction = reflected + roughness * RandomInHemisphere(aRecord.hitNormal);
		
		// Set the origin of the ray
		aRay.origin = aRecord.hitPoint + (EPSILON * aRay.direction);
		
		// Set the attenuation
		attenuation = aRecord.hitAlbedo;
		
		// Check if the dot product of the ray direction and hit normal are greater than 0
		if (dot(aRay.direction, aRecord.hitNormal) > 0)
		{
			// Greater than 0, set ray scattered boolean to true
			rayScattered = true;
		}
		else
		{
			// Less than or equal to 0, set ray scattered boolean to false
			rayScattered = false;
		}
	}
	
	// Return the value of scatter
	return rayScattered;
}

/*
 * DIELECTRIC SCATTER FUNCTION
 *
 * Function is used to determine if a ray can be
 * scattered if the material is dielectric.
 *
 * Function also modifies the ray and the attenuation.
 */
bool DielectricScatter(inout Ray aRay, inout HitRecord aRecord, inout float3 attenuation)
{
	// Ray scattered boolean initialised to false
	bool rayScattered = false;
	
	// Check if the material is dielectric
	if (aRecord.hitMaterialType == matDielectric)
	{
		// Set attenuation value
		attenuation = aRecord.hitAlbedo;
		
		// Index of refraction
		float idxOfRef = aRecord.hitIdxOfRef;
		
		// Refraction ratio
		float refractionRatio;
		
		// Check the value of front face from the hit record
		if (aRecord.frontFace == true)
		{
			// Using front face, calculate the refraction ratio
			refractionRatio = 1.0 / idxOfRef;
		}
		else
		{
			// Using back face, set the refraction ratio to the index of refraction
			refractionRatio = idxOfRef;
		}
		
		// Determine the unit direction 
		float3 unitDirection = normalize(aRay.direction);
		
		// Determine cos theta
		float cosTheta = min(dot(-unitDirection, aRecord.hitNormal), 1.0);
		
		// Determine sin theta
		float sinTheta = sqrt(1.0 - (cosTheta * cosTheta));
		
		// Cannot refract boolean
		bool cannotRefract;
		
		// Check if the refraction ratio multiplied by sin theta is greater than 1
		if (refractionRatio * sinTheta > 1.0)
		{
			// Greater than 1, set cannot refract to true
			cannotRefract = true;
		}
		else
		{
			// Less than one, set cannot refract to false
			cannotRefract = false;
		}
		
		// Direction vector
		float3 direction;
		
		// Check if cannot refract is true, or if schlick reflectance approximation is greater than a random float
		if (cannotRefract == true || reflectance(cosTheta, refractionRatio) > Rand())
		{
			// Cannot refract, set as a reflected ray
			direction = reflect(unitDirection, aRecord.hitNormal);
		}
		else
		{
			// Can refract, set the direction as a refracted ray
			direction = refract(unitDirection, aRecord.hitNormal, refractionRatio);
		}
		
		// Set the origin of the ray to the point of collision
		aRay.origin = aRecord.hitPoint;
				
		// Determine the roughness
		float roughness = 1.0f - aRecord.hitSmoothness;
		
		// Set the direction of the ray
		aRay.direction = direction + roughness * RandomInHemisphere(aRecord.hitNormal);
		
		// Set ray scattered boolean to true
		rayScattered = true;
	}
	
	// Return the value of ray scattered
	return rayScattered;
}

/*
 * EMISSIVE SCATTER FUNCTION
 *
 * Function will always return false if
 * the material is emissive
 */
bool EmissiveScatter(inout Ray aRay, inout HitRecord aRecord, inout float3 attenuation)
{
	// Ray scattered boolean initialised to false
	bool rayScattered = false;
	
	if (aRecord.hitMaterialType != matEmissive)
	{
		// The ray can be scattered
		rayScattered = true;
	}
	
	// Return the value of ray scatterd boolean
	return rayScattered;
}

