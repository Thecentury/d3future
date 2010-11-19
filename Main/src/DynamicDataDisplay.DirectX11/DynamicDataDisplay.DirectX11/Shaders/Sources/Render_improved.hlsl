cbuffer cbPerObject : register( b0 )
{
	matrix		g_mWorldViewProjection	: packoffset( c0 );
};

struct VS_OUTPUT
{
    //float4 SVPos : SV_POSITION;
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
};

cbuffer cbImmutable : register( b1 )
{
    float3 g_positions[4] =
    {
        float3( -0.1, 0.1, 0 ),
        float3( 0.1, 0.1, 0 ),
        float3( -0.1, -0.1, 0 ),
        float3( 0.1, -0.1, 0 ),
    };
};


//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VertShader( float4 Pos : POSITION, float4 Color : COLOR ) 
{
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.Pos = Pos;
    //output.SVPos = Pos;//mul(Pos, g_mWorldViewProjection);
    output.Color = Color;
    return output;
}

//--------------------------------------------------------------------------------------
// Geometry Shader
//--------------------------------------------------------------------------------------
[maxvertexcount(4)]
void GeomShader(point VS_OUTPUT input[1], inout TriangleStream<VS_OUTPUT> SpriteStream)
{
	VS_OUTPUT output;
    
    float3 g_positions1[4] =
    {
        float3( -0.1, 0.1, 0 ),
        float3( 0.1, 0.1, 0 ),
        float3( -0.1, -0.1, 0 ),
        float3( 0.1, -0.1, 0 ),
    };
    
    //
    // Emit two new triangles
    //
    for(int i=0; i<4; i++)
    {
		float3 position = g_positions1[i];
		
        position = position + input[0].Pos.xyz;
        output.Pos = mul( float4(position,1.0), g_mWorldViewProjection );
        
        output.Color = input[0].Color;
        SpriteStream.Append(output);
    }
    
    SpriteStream.RestartStrip();
}


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PixShader( VS_OUTPUT input ) : SV_TARGET
{
    return input.Color;
}
