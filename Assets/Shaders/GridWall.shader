Shader "Unlit/Grid"
{
	Properties
	{
		_LineColor("Line Color", Color) = (1,1,1,1)
		_CellColor("Cell Color", Color) = (0,0,0,0)
		_SelectedColor("Selected Color", Color) = (1,0,0,1)
		[PerRendererData] _MainTex("Albedo (RGB)", 2D) = "white" {}
		[IntRange] _GridSize("Grid Size", Range(1,250)) = 10
		_LineSize("Line Size", Range(0,1)) = 0.15


	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				half _Glossiness = 0.0;
				half _Metallic = 0.0;
				float4 _LineColor;
				float4 _CellColor;
				sampler2D _MainTex;

				float _GridSize;
				float _LineSize;

				float _SelectCell;
				float _SelectedCellX;
				float _SelectedCellY;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

    			    float waveFreq=0;
					float waveAmp=0.1;
                    float waveOffset = sin(i.uv.y * waveFreq + _Time.y * waveAmp);
	
    				// modify the y-coordinate of the grid cell to incorporate the wave displacement
    				float2 uv = float2(i.uv.x , i.uv.y + waveOffset);

	

					fixed4 c = float4(0.0,0.0,0.0,0.0);

					float brightness = 1.;

					float gsize = floor(_GridSize);



					gsize += _LineSize;

					float2 id;

					id.x = floor(uv.x / (1.0 / gsize));
					id.y = floor(uv.y / (1.0 / gsize));

					float4 color = _CellColor;
					brightness = _CellColor.w;

				

					if (frac(uv.x*gsize) <= _LineSize || frac(uv.y*gsize*3) <= _LineSize)
					{
						brightness = _LineColor.w;
						color = _LineColor;
					}


					//Clip transparent spots using alpha cutout
					if (brightness == 0.0) {
						clip(c.a - 1.0);
					}
  
			        float waveColorFreq=1;
					float waveColorAmp=1;
                    float waveColorOffset = sin(i.uv.y * waveColorFreq + _Time.y * waveColorAmp);

					c = fixed4(color.x * waveColorOffset, color.y - waveColorOffset, color.z - waveColorOffset,brightness);
					return c;
				}
				ENDCG
			}
		}
}