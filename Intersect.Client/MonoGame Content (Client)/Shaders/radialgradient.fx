float Expand;
float4 LightColor;
float4 PS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{
	float t=distance(coords,float2(0.5f, 0.5f))*2;
	if (t < 1.0) {
		return lerp(LightColor,float4(0,0,0,0), (t-Expand)/(1-Expand));
	}
	else {
		return float4(0,0,0,0);
	}
}

technique Technique1
{
    pass Pass1
    {
		#if SM4
				PixelShader = compile ps_4_0_level_9_1 PS();
		#elif SM3
				PixelShader = compile ps_3_0 PS();
		#else
				PixelShader = compile ps_2_0 PS();
		#endif
    }
}