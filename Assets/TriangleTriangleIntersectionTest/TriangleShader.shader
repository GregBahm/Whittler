Shader "Unlit/TriangleShader"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			struct TriangleIntersection
			{
				int Present;
				float3 Start;
				float3 End;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 basePos : TEXCOORD0;
				float3 testPosA : TEXCOORD1;
				float3 testPosB : TEXCOORD2;
			};

			float3 _Color;
			StructuredBuffer<float3> _PointsBuffer;
			StructuredBuffer<TriangleIntersection> _IntersectionBuffer;

			v2f vert(uint meshId : SV_VertexID, uint instanceId : SV_InstanceID)
			{
				v2f o;
				float3 vertPos = _PointsBuffer[meshId];
				
				TriangleIntersection intersection = _IntersectionBuffer[0];
				o.basePos = vertPos;
				o.testPosA = intersection.Start;
				o.testPosB = intersection.End;

				o.vertex = UnityObjectToClipPos(vertPos);
				return o;
			}

			float GetDistanceToLine(float3 pointOnLine, float3 normal, float3 basePoint)
			{
				float3 crossProduct = cross(normal, basePoint - pointOnLine);
				return length(crossProduct);
			} 
			
			fixed4 frag (v2f i) : SV_Target
			{
				//float ret = GetDistanceToLine(i.testPosA, i.testPosB, i.basePos);
				//return ret;

				float lengthA = length(i.testPosA - i.basePos);
				float lengthB = length(i.testPosB - i.basePos);
				float minLength = min(lengthA, lengthB);
				return float4(_Color, 1) * minLength;
			}
			ENDCG
		}
	}
}
