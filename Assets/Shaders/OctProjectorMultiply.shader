
Shader "Projector/OT_Multiply" {
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float2 uvShadow : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
			};
			
			v2f vert (float4 vertex : POSITION, float2 uv:TEXCOORD0)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = uv;
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			sampler2D _ShadowTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS = tex2D (_ShadowTex, i.uvShadow);
				texS.a = 1.0-texS.a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
				return texS;
			}
			ENDCG
		}
	}
}
