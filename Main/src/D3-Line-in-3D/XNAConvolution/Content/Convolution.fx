float4x4 wvp : WorldViewProjection;
texture noizeTexture;
texture xTexture;
texture yTexture;
float width;
float height;

sampler2D noizeSampler = 
sampler_state
{
	Texture = <noizeTexture>;
	MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};

sampler2D xSampler = 
sampler_state
{
	Texture = <xTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};

sampler2D ySampler = 
sampler_state
{
	Texture = <yTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};

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

float2 GetVector(float2 position) {
	float2 x = tex2D(xSampler, position);// * 0xFFFFFFFF + packed.r;
	float2 y = tex2D(ySampler, position);// 0xFFFF + packed.b;
	
	return float2(x.x, y.x);
}

float4 PS(VSOut input) : COLOR {
	float2 pos = input.TexCoord;
	float4 value = tex2D(noizeSampler, pos);
	
	int stepNum = 20;
	int steps = 1;
	
	for (int i = 0; i < stepNum; i++) {
		float2 dir = GetVector(pos);
		dir = normalize(dir);
		dir.x /= width;
		dir.y /= -height;

		pos += dir;
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1) {
			steps++;
			value += tex2D(noizeSampler, pos);
		}
	}	
	
	pos = input.TexCoord;
	for (int i = 0; i < stepNum; i++) {
		float2 dir = GetVector(pos);
		dir = normalize(dir);
		dir.x /= width;
		dir.y /= -height;
		
		pos -= dir;
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1) {
			steps++;
			value += tex2D(noizeSampler, pos);
		}
	}
	
	value /= steps;
	
	return value;
}

technique Main 
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}