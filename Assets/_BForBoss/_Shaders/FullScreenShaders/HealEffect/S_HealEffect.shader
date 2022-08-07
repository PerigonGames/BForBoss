Shader "FullScreen/S_HealEffect"
{
    Properties{
        _MainTex ("-", 2D) = "" {}
        [HDR] _Color("Color", Color) = (1,1,1,1)
//        _EmissionStrength("EmissionStrength", float) = 1.0
//        _Flash("Flash", float) = 1.0
        _EmissionStrength("Emission Strength", float) = 1.0
        }

    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float2 _Aspect;
    float _Falloff;
    float _TextureEmissionStrength;
    float _Flash;
    float _EmissionStrength;
    float4 _Color;

    half3 AdjustContrast(half3 color, half contrast) {
        return saturate(lerp(half3(0.5, 0.5, 0.5), color, contrast));
    }
    
    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
        float4 color = float4(0.0, 0.0, 0.0, 0.0);

        // Load the camera color buffer at the mip 0 if we're not at the before rendering injection point
        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
            color = float4(CustomPassLoadCameraColor(varyings.positionCS.xy, 0), 1);
        
        _Aspect = float2(1.0,1.0);
        _Falloff = 0.5;
        
        float2 coord = (posInput.positionNDC.xy - 0.5) * _Aspect * 2;
        float rf = sqrt(dot(coord, coord)) * _Falloff;
        float rf2_1 = rf * rf + 1.0;
        float e = 1.0 / (rf2_1 * rf2_1);
        e = pow(e, 1.0);
        e = AdjustContrast(e, 3.0);
        e = saturate(e);
        
        
        half4 src = tex2D(_MainTex, posInput.positionNDC.xy * _MainTex_ST);
        
        src = src + ((1.0-e)*1);
        src = src * ((1.0-e)*1);
        
        float3 src_color = src * _Color;

        if (_EmissionStrength > 1)
        {
            _EmissionStrength = 1;
        }
        if (_EmissionStrength < 0)
        {
            _EmissionStrength = 0;
        }
        
        //float x = cos(_AnimTime * (PI/2) );
        float x = (1 / (9 * (_EmissionStrength + 0.1) )) - 0.1;
        _TextureEmissionStrength = x;
        _Flash = x;
        
        return half4( src_color.rgb * _TextureEmissionStrength + _Flash, src.r * _TextureEmissionStrength + _Flash);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Custom Pass 1"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
