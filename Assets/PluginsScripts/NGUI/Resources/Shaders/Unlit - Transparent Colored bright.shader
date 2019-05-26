// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Colored (Bright)"  
{  
    Properties  
    {  
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}  
    }  
      
    SubShader  
    {  
        LOD 200  
  
        Tags  
        {  
            "Queue" = "Transparent"  
            "IgnoreProjector" = "True"  
            "RenderType" = "Transparent"  
        }  
          
        Pass  
        {  
	
	        Cull Off  
	        Lighting Off  
	        ZWrite Off  
	        Fog { Mode Off }  
	        Offset -1, -1  
	        Blend SrcAlpha OneMinusSrcAlpha  
  

            	CGPROGRAM  
                #pragma vertex vert  
                #pragma fragment frag  
                 
                #include "UnityCG.cginc"  
      
                sampler2D _MainTex;  
                float4 _MainTex_ST;  
                struct appdata_t  
                {  
                    float4 vertex : POSITION;  
                    float2 texcoord : TEXCOORD0;  
                    fixed4 color : COLOR;  
                };  
      
                struct v2f  
                {  
                    float4 vertex : SV_POSITION;  
                    half2 texcoord : TEXCOORD0;  
                    fixed4 color : COLOR;  
                };  
      
		v2f o;  
                  
                v2f vert (appdata_t v)  
                {  
                    o.vertex = UnityObjectToClipPos(v.vertex);  
                    o.texcoord = v.texcoord;
                    o.color = v.color;  
                    return o;  
                }  
                  
                fixed4 frag (v2f IN) : COLOR  
                {  
                	//if(IN.color.r < 1.0f)
                	//{
                	//	_Bright = 1.5f;
                	//	IN.color.r = 1.0f;
                	//}
					//return tex2D(_MainTex, IN.texcoord) * IN.color * _Bright; 
					 
					fixed4 col = tex2D(_MainTex, IN.texcoord);
					fixed4 vColor = col * IN.color;

					half3 vGrey = dot(col.rgb, float3(0.299, 0.587, 0.114));
					half3 vNormal = vColor.rgb;
					//half3 vHighLight = (1.4 * i.color.g + 0.4)* col.rgb
					half3 vHighLight = vColor.rgb * 1.5f;

					col.rgb = lerp( vNormal, vHighLight, IN.color.r < 1.0f );
					col.rgb = lerp( col.rgb, vGrey, IN.color.b <= 0.001f );

					col.a = vColor.a;

					return col;
                }  
            ENDCG  
        }  
    }  
  
    SubShader  
    {  
        LOD 100  
  
        Tags  
        {  
            "Queue" = "Transparent"  
            "IgnoreProjector" = "True"  
            "RenderType" = "Transparent"  
        }  
          
        Pass  
        {  
            Cull Off  
            Lighting Off  
            ZWrite Off  
            Fog { Mode Off }  
            Offset -1, -1  
            ColorMask RGB  
            Blend SrcAlpha OneMinusSrcAlpha  
            ColorMaterial AmbientAndDiffuse  
              
            SetTexture [_MainTex]  
            {  
                Combine Texture * Primary  
            }  
        }  
    }  
}  