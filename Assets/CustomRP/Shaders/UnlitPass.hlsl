#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED
#include "../ShaderLibrary/Common.hlsl"

CBUFFER_START(UnityPerDraw)
float4 _BaseColor;
CBUFFER_END

float4 UnlitPassVertex(float3 posOS : POSITION) : SV_POSITION
{
    float3 posWS = TransformObjectToWorld(posOS);
    float4 posCS = TransformWorldToHClip(posWS);
    return posCS;
}

float4 UnlitPassFragment() : SV_TARGET
{
    return _BaseColor;
}

#endif