Shader "Custom/CRT_URP"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque" 
        }
        
        Pass
        {

            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Properties for the shader
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float2 _InputSize;
            float2 _OutputSize;
            float2 _TextureSize;
            float2 _One;
            float2 _Texcoord;
            float _Factor;
            float _Distortion = 0.1f;
            float _Gamma = 1.0f;
            float _curvatureSet1 = 0.5f;
            float _curvatureSet2 = 0.5f;
            float _YExtra = 0.5f;
            float _rgb1R = 1.0f;
            float _rgb1G = 1.0f;
            float _rgb1B = 1.0f;
            float _rgb2R = 1.0f;
            float _rgb2G = 1.0f;
            float _rgb2B = 1.0f;
            float _dotWeight = 2.0f;

            // Radial Distortion Function
            float2 RadialDistortion(float2 coord)
            {
                coord *= _TextureSize / _InputSize;
                float2 cc = coord - _curvatureSet1;
                float dist = dot(cc, cc) * _Distortion;
                return (coord + cc * (_curvatureSet2 + dist) * dist) * _InputSize / _TextureSize;
            }

            // Scanline Weights Function
            float4 ScanlineWeights(float distance, float4 color)
            {
                float4 width = 2.0f + 2.0f * pow(color, float4(4.0f, 4.0f, 4.0f, 4.0f));
                float4 weights = float4(distance / 0.5f, distance / 0.5f, distance / 0.5f, distance / 0.5f);
                return 1.4f * exp(-pow(weights * rsqrt(0.5f * width), width)) / (0.3f + 0.2f * width);
            }

            // Main Fragment Shader
            half4 frag(Varyings i) : SV_Target
            {
                _Texcoord = i.uv;
                _One = 1.0f / _TextureSize;
                _OutputSize = _TextureSize;
                _InputSize = _TextureSize;
                _Factor = _Texcoord.x * _TextureSize.x * _OutputSize.x / _InputSize.x;

                #ifdef CURVATURE
                float2 xy = RadialDistortion(_Texcoord);
                #else
                float2 xy = _Texcoord;
                #endif

                float2 ratio = xy * _TextureSize - float2(0.5f, 0.5f);
                float2 uvratio = frac(ratio);

                xy.y = (floor(ratio.y) + _YExtra) / _TextureSize;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, xy);
                half4 col2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, xy + float2(0.0f, _One.y));

                half4 weights = ScanlineWeights(uvratio.y, col);
                half4 weights2 = ScanlineWeights(1.0f - uvratio.y, col2);
                half3 res = (col.rgb * weights.rgb + col2.rgb * weights2.rgb);

                half3 rgb1 = half3(_rgb1R, _rgb1G, _rgb1B);
                half3 rgb2 = half3(_rgb2R, _rgb2G, _rgb2B);

                half3 dotMaskWeights = lerp(rgb1, rgb2, floor(fmod(_Factor, _dotWeight)));
                res *= dotMaskWeights;

                // Gamma Correction
                return half4(pow(res, half3(1.0f / _Gamma, 1.0f / _Gamma, 1.0f / _Gamma)), 1.0f);
            }

            ENDHLSL
        }
    }
    // Fallback or Default Shader Settings
    //Fallback "Universal Forward"
}
