Shader "Custom/sample" 
{
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		//#pragma surface surf Standard 
		#pragma surface surf Standard alpha:fade 
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float dist = distance(fixed3(0, 0, 0), localPos);
			float val = abs(sin(dist * 3.0 - _Time * 100));
			if (val > 0.98) 
			{
				o.Albedo = fixed4(1, 1, 0, 1);
				o.Alpha = 1.0;
			}
            else 
			{
                o.Albedo = fixed4(110 / 255.0, 87 / 255.0, 139 / 255.0, 1);
				o.Alpha = 0.0;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}