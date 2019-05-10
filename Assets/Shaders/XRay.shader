Shader "Custome/XRay"
{
    Properties
    {
        _Color("Color", Color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _AfterColor("After Color", Color)=(0.435,0.851,1,0.419)
    }
    SubShader
    {
        Tags{"Queue"="Geometry+1" "RenderType"="Opaque"}
        LOD  300
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            ZTest GEqual // 深度测试，大于等于当前最小中的值就会显示
            ZWrite  Off 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 _AfterColor;
            fixed4 frag (v2f i) : SV_Target
            {
                return _AfterColor;
            }
            ENDCG
        }
        Pass
        {
            ZTest LEqual
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
                float4 vertex: SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            v2f vert(appdata i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            }
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) + _Color;
                return c;
            }
            ENDCG
        }
    }
}
