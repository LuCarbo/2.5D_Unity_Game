Shader "Custom/PixelatedRetroFireURP"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        [HDR] _ColorInner ("Inner Core Color", Color) = (1, 0.9, 0, 1)
        [HDR] _ColorOuter ("Outer Body Color", Color) = (1, 0.3, 0, 1)
        _Speed ("Scroll Speed", Float) = 1.5
        _PixelSize ("Pixel Resolution", Float) = 32.0
        _CoreThreshold ("Inner Core Size", Range(0.0, 1.0)) = 0.6
        _OuterThreshold ("Outer Body Size", Range(0.0, 1.0)) = 0.3
    }
    SubShader
    {
        // URP specific tags
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "IgnoreProjector"="True" 
        }
        LOD 100

        Pass
        {
            Name "Unlit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off // Renders on both sides for 2.5D

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Includes core URP shader library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // URP Texture declaration
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            // CBUFFER ensures this shader is compatible with the SRP Batcher for performance
            CBUFFER_START(UnityPerMaterial)
                float4 _NoiseTex_ST;
                float4 _ColorInner;
                float4 _ColorOuter;
                float _Speed;
                float _PixelSize;
                float _CoreThreshold;
                float _OuterThreshold;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                // URP specific space transformation
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // Apply texture tiling and offset
                output.uv = input.uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Pixelate the UV coordinates
                float2 pixelUV = floor(input.uv * _PixelSize) / _PixelSize;

                // 2. Scroll the UVs upward over time
                float2 scrollUV = pixelUV;
                scrollUV.y -= _Time.y * _Speed;

                // 3. Sample the Noise texture (URP method)
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, scrollUV).r;

                // 4. Create a base teardrop shape mask
                float heightMask = 1.0 - pixelUV.y; 
                float widthMask = 1.0 - abs(pixelUV.x - 0.5) * 2.0; 
                float baseShape = heightMask * widthMask * (1.2 - pixelUV.y); 

                // 5. Combine the noise with our shape
                float fireValue = baseShape * noise;

                // 6. Quantize into hard color bands
                half4 finalColor = half4(0, 0, 0, 0);

                if (fireValue > _CoreThreshold) 
                {
                    finalColor = _ColorInner;
                } 
                else if (fireValue > _OuterThreshold) 
                {
                    finalColor = _ColorOuter;
                } 
                else 
                {
                    discard; // Hide pixels outside the flame area
                }

                return finalColor;
            }
            ENDHLSL
        }
    }
}