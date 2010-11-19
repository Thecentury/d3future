/// <description>A simple color blending shader for WPF.</description>
/// <target>WPF</target>
/// <profile>ps_3_0</profile>

//-----------------------------------------------------------------------------
// Constants
//-----------------------------------------------------------------------------

/// <summary>The brightness offset.</summary>
/// <type>Color</type>
/// <defaultValue>0,0,0,0</defaultValue>
//float4 BlendColor : register(c0);

/// <type>Float</type>
float MinX : register(c0);
/// <type>Float</type>
float MinY : register(c1);
/// <type>Float</type>
float MaxX : register(c2);
/// <type>Float</type>
float MaxY : register(c3);
/// <type>Float</type>
float Width : register(c4);
/// <type>Float</type>
float Height : register(c6);

/// <type>Float</type>
float MinLength : register(c7);
/// <type>Float</type>
float MaxLength : register(c8);



//-----------------------------------------------------------------------------
// Samplers
//-----------------------------------------------------------------------------

/// <summary>The implicit input sampler passed into the pixel shader by WPF.</summary>
/// <samplingMode>Auto</samplingMode>
sampler2D Input : register(s0);
/// <summary></summary>
/// <samplingMode>Linear</samplingMode>
sampler2D Noize : register(s1);
/// <summary></summary>
/// <samplingMode>Linear</samplingMode>
sampler2D Field : register(s2);
/// <samplingMode>Linear</samplingMode>
sampler2D Palette : register(s3);

//-----------------------------------------------------------------------------
// Pixel Shader
//-----------------------------------------------------------------------------

float2 GetVector(float4 pos)
{
	float deltaX = MaxX - MinX;
	float deltaY = MaxY - MinY;
	float x = pos.g * deltaX + MinX;
	float y = pos.b * deltaY + MinY;

	float2 result = float2(y, x);
	//result = normalize(result);
	//result.x /= Width;
	//result.y /= -Height;

	return result;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float2 pos = uv;
	float4 value = tex2D(Noize, uv);

	//float4 tex = tex2D(Field, uv);
	//tex.r = tex.b = 0;
	//return tex;

	int stepNum = 20;
	int steps = 1;
	
	for (int i = 0; i < stepNum; i++) {
		float4 vect = tex2D(Field, pos);
		float2 dir = GetVector(vect);
		dir = normalize(dir);
		dir.x /= Height;
		dir.y /= Width;

		pos += dir;
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1) {
			steps++;
			value += tex2D(Noize, pos);
		}
	}	
	
	pos = uv;
	for (int i = 0; i < stepNum; i++) {
		float4 vect = tex2D(Field, pos);
		float2 dir = GetVector(vect);
		dir = normalize(dir);
		dir.x /= Height;
		dir.y /= Width;
		
		pos -= dir;
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1) {
			steps++;
			value += tex2D(Noize, pos);
		}
	}
	
	value /= steps;
	
	float2 inCenter = GetVector(tex2D(Field, uv));
	float len = length(inCenter);
	float ratio = (len - MinLength) / (MaxLength - MinLength);
	float4 color = tex2D(Palette, float2(ratio, 0.5));

	value *= color;

	return value;
}