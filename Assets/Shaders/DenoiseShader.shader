// ************************************************************************** //
//                              DENOISING SHADER                              //
// ************************************************************************** //
//                         ADVANCED GAMES ENGINEERING                         //
//                         GRAEME B. WHITE - 40415739                         //
// ************************************************************************** //
// DenoiseShader.shader                                                       //
//                                                                            //
// Shader file for denoising the image in realtime. A very simple approach.   //
// Within the fragment shader, the colour of the pixel is divided by the      //
// current sample number + 1.                                                 //
//                                                                            //
// When the camera is stationary, the image converges into one with less      //
// noise. However, when the user move the camera, current sample will be 0    //
// and will only denoise the image when the camera is stationary.             //
// ************************************************************************** //

// Shader declaration
Shader "DenoisingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // Enable alpha blending
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Main texture
            sampler2D _MainTex;

            // Sample
            float _currentSample;

            // Fragment shader for denoising image
            float4 frag(v2f i) : SV_Target
            {
                return float4(tex2D(_MainTex, i.uv).rgb, 1.0f / (_currentSample + 1.0f));
            }
            ENDCG
        }
    }
}
