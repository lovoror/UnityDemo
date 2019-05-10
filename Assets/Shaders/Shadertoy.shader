Shader "Shadertoy/Template" 
{
    Properties
    {
        iMouse ("Mouse Pos", Vector) = (100, 100, 0, 0)
        iChannel0("iChannel0", 2D) = "white" {}  
        iChannelResolution0 ("iChannelResolution0", Vector) = (100, 100, 0, 0)
    }
 
    CGINCLUDE    
    #include "UnityCG.cginc"   
    #pragma target 3.0      
 
    #define vec2 float2
    #define vec3 float3
    #define vec4 float4
    #define mat2 float2x2
    #define mat3 float3x3
    #define mat4 float4x4
    #define iTime _Time.y
    #define mod fmod
    #define mix lerp
    #define fract frac
    #define texture2D tex2D
    #define iResolution _ScreenParams
    #define gl_FragCoord ((_iParam.scrPos.xy/_iParam.scrPos.w) * _ScreenParams.xy)
 
    #define PI2 6.28318530718
    #define pi 3.14159265358979
    #define halfpi (pi * 0.5)
    #define oneoverpi (1.0 / pi)
 
    fixed4 iMouse;
    sampler2D iChannel0;
    fixed4 iChannelResolution0;
 
    struct v2f 
    {
        float4 pos : SV_POSITION;    
        float4 scrPos : TEXCOORD0;   
    };              
 
    v2f vert(appdata_base v) 
    { 
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex);
        o.scrPos = ComputeScreenPos(o.pos);
        return o;
    }  
 
    vec4 main(vec2 fragCoord);
 
    fixed4 frag(v2f _iParam) : COLOR0 
    {
        vec2 fragCoord = gl_FragCoord;
        return main(gl_FragCoord);
    }  
 
    vec3 hash3(vec2 p)
    {
        vec3 q = vec3(dot(p, vec2(127.1,311.7)),dot(p,vec2(269.5,183.3)),dot(p,vec2(419.2,317.9)));
        return fract(sin(q)*43758.5453);
    }

    float iqnoise(in vec2 x, float u, float v)
    {
        vec2 p = floor(x);
        vec2 f = fract(x);

        float k = 1.0+63.0*pow(1.0-v,4.0);

        float va = 0.0;
        float wt = 0.0;
        for (int j = 0; j < 3; ++j)
        {
            for (int i = 0; i < 3; ++i)
            {
                vec2 g = vec2(float(i),float(j));
                vec3 o = hash3(p + g)*vec3(u,u,1.0);
                vec2 r = g - f + o.xy;
                float d = dot(r,r);
                float ww = pow(1.0-smoothstep(0.0,1.414,sqrt(d)),k);
                va += o.z*ww;
                wt += ww;
            }
        }
        return va/wt;
    }

    vec4 main(vec2 fragCoord) 
    {
        vec2 uv = fragCoord.xy / iResolution.xx;
        vec2 p = 0.5 - 0.5*sin(iTime*vec2(1.01,1.71));
        if (iMouse.w > 0.001) p = vec2(0.0, 1.0) + vec2(1.0, -1.0)*iMouse.xy/iResolution.xy;
        p = p*p*(3.0-2.0*p);
        p = p*p*(3.0-2.0*p);
        p = p*p*(3.0-2.0*p);
        float f = iqnoise(24.0*uv,p.x,p.y);
        return vec4(f, f, f, 1);
    }
    ENDCG    
    SubShader 
    {    
        Pass 
        {    
            CGPROGRAM    
            #pragma vertex vert    
            #pragma fragment frag    
            #pragma fragmentoption ARB_precision_hint_fastest     
            ENDCG    
        }    
    }     
    FallBack Off    
}