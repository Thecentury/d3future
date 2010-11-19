float4x4 wvp : WorldViewProjection;

struct VSIn {
	float3 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

struct VSOut {
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

VSOut VS(VSIn input) {
	VSOut output;
	output.Position = mul(float4(input.Position, 1), wvp);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PS(VSOut input) : COLOR {
	return float4(1,1,0,0);
}

technique Main 
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}