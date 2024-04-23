Shader "Custom/TransparentCoverShader" 
{
	Properties
	{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent+1" }
		Offset 0, -1
		ColorMask 0
		ZWrite On

		Pass
		{
			AlphaTest Greater[_Cutoff]

			SetTexture[_MainTex]
			{
				combine texture * primary, texture
			}
		}
	}
}
