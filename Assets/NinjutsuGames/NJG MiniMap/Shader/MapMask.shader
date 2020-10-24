Shader "NinjutsuGames/MapMask"
{
	Properties {
		_Color ("_Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Culling Mask", 2D) = "white" {}
	}
	SubShader { 

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }	
		ColorMask RGB
		
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMaterial AmbientAndDiffuse		

		Pass {
			SetTexture [_Mask] { 
				combine texture 
			}

			SetTexture [_MainTex] { 
         		matrix [_Matrix] combine texture * Primary, previous * Primary 
			}
		}
	}
}
