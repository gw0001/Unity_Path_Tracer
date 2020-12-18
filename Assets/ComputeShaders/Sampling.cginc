// ************************************************************************** //
//                                 SAMPLING                                   //
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

// Pixel
float2 _pixel;

// Random value seed
float _randomNumber;

// Pi value
static const float PI = 3.14159265f;

// ************************************************************************** //
//                             SAMPLING FUNCTIONS                             //
// ************************************************************************** //

/*
 * RANDOM FUNCTION
 *
 * Random function is used to generate
 * a random float, based on a random 
 * seed number from the C# script and
 * the pixel coordinates
 *
 * Modified version of solution found from
 * https://www.gamedev.net/forums/topic/548512-hlsl-random-numbers/
 */
float Rand()
{
	// Determine a random result
	float result = frac(sin(_randomNumber / 100.0f * dot(_pixel, float2(12.9898f, 78.233f))) * 43758.5453f);
	
	// Add one to the random seed to make it slightly different
	_randomNumber += 1.0f;
	
	// Output the result
	return result;
}

/* 
 * GET TANGENT MATRIX
 *
 * Function to obtain the tangential matrix
 * based on a normal
 */
float3x3 GetTangentMatrix(float3 aNormal)
{
    // Choose a helper vector for the cross product
	float3 helper = float3(0, 0, 1);

    // Determine tangent vector
	float3 tangent = normalize(cross(aNormal, helper));
	
	// Determine binormal vector
	float3 binormal = normalize(cross(aNormal, tangent));
	
	// Create tangential matrix from tangent, binormal and normal vectors
	float3x3 tangentialMatrix = float3x3(tangent, binormal, aNormal);
	
	// Return tangential matrix
	return tangentialMatrix;
}

/*
 * RANDOM IN UNIT SPHERE FUNCTION
 *
 * Function obtains a Random unit sphere based
 * on a normal
 */
float3 RandomInUnitSphere(float3 aNormal)
{
    // Obtain a random angle, theta
	float cosTheta = 2 * Rand() - 1.0f;
	
	// Determine sine of theta
	float sinTheta = sqrt(1.0f - cosTheta * cosTheta);
	
	// Obtain a random phi value
	float phi = 2 * PI * Rand();
	
	// Determine tangential direction in local space
	float3 tangentDirectionLocal = float3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
	
    // Transform direction to world space
	float3 tangentialDirectionWorld = mul(tangentDirectionLocal, GetTangentMatrix(aNormal));

	// Return the tangential direction in world space
	return tangentialDirectionWorld;
}

/*
 * RANDOM IN HEMISPHERE
 *
 * Used to determine the hemisphere that a unit
 * vector is in.
 */
float3 RandomInHemisphere(float3 aNormal)
{
	// Random in hemisphere
	float3 randomInHemisphere;
	
	// Obtain the random in unit sphere
	float3 randomInUnitSphere = RandomInUnitSphere(aNormal);
	
	// Check if the dot product
	if (dot(randomInUnitSphere, aNormal) > 0.0f)
	{
		// In the same hemisphere, return the random in unit sphere
		randomInHemisphere = randomInUnitSphere;
	}
	else
	{
		// Not in the same hemisphere, return the inverse of the random in unit sphere
		randomInHemisphere = -randomInUnitSphere;
	}

	// Return value of random hemisphere
	return randomInHemisphere;
}

/*
 * REFLECTANCE FUNCTION
 * 
 * Function uses schlick's approximation 
 * to determine the relfectance of a ray
 */
float reflectance(float cosine, float refractanceIndex)
{
	// Determine the value for r0
	float r0 = (1.0f - refractanceIndex) / (1.0f + refractanceIndex);
	
	// Determine the value of r0 squared
	float r0Squared = r0 * r0;
	
	// Determine Schlick approximation of reflectance
	float schlickReflectance = r0Squared + (1.0f - r0Squared) * pow((1.0f - cosine), 5);
	
	// Return Schlick approximation
	return schlickReflectance;
}