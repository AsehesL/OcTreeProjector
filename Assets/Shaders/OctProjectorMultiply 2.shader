// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/OT_Multiply2" {
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
				UNITY_FOG_COORDS(0)
				float4 pos : SV_POSITION;
			};
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			sampler2D _ShadowTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS = fixed4(0,0,0,0.5);

				UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
				return texS;
			}
			ENDCG
		}
	}
}
