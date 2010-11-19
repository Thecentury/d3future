cbuffer cbPerObject : register( b0 )
{
	matrix	g_mWorldViewProjection	: packoffset( c0 );
};

cbuffer cbPerFrame : register ( b0 )
{
	float4 lineColor : packoffset( c0 );
}

struct VS_OUTPUT
{
    float4 Pos : SV_POSITION;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VertShader( float4 Pos : POSITION ) 
{
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.Pos = mul(Pos, g_mWorldViewProjection);
    return output;
}

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PixShader( VS_OUTPUT input ) : SV_TARGET
{
    return lineColor;
}
