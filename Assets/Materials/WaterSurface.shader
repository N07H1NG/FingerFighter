Shader "Custom/WaterSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Glossiness", Float) = 0.5
        _Specular ("Specular", Color) = (0.0,0.0,0.0)
        _Amplitude ("Amplitude", Float) = 0.5
        _Frequency ("Frequency", Float) = 0.5
        _Speed ("Speed", Float) = 0.5
        _AmplitudeFalloff ("Amplitude Falloff", Float) = 0.5
        _FrequencyFalloff ("Frequency Falloff", Float) = 0.5
        _SpeedFalloff ("Speed Falloff", Float) = 0.5
        _Depth("Depth",Float) =10
        _Tess ("Tessellation", Range(1,32)) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows vertex:vert tessellate:tessDistance

        #include "UnityPBSLighting.cginc"
        #include "Tessellation.cginc"
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        struct disp
        {
            float height;
            float2 derivative;
        };

        float _Glossiness;
        float3 _Specular;
        fixed4 _Color;
        int _Depth;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        float _Amplitude;
        float _Frequency;
        float _Speed;
        float _AmplitudeFalloff;
        float _FrequencyFalloff;
        float _SpeedFalloff;
        float _Tess;

        disp displace(float2 uv, float2 dir, float a, float f, float s){
            dir /= length(dir);
            //float h = a*sin((dot(dir,uv)) * f + _Time.w * s) ;
            float h = a*exp(sin((dot(dir,uv)) * f + f*_Time.w * s)-1.0) ;
            float c =  cos((dot(dir,uv)) * f +f* _Time.w * s);
            c = c*h;
            
            disp res;
            res.height = h;
            
            res.derivative = f*float2(dir.x*c,dir.y*c);
            return res;
        }

        float4 tessDistance (appdata v0, appdata v1, appdata v2) {
            float minDist = 200.0;
            float maxDist = 505.0;
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }
          
        float2x2 make_rotator(float angle){
            return float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
        }

        //float4 LightingWater (SurfaceOutputStandard s, half3 viewDir,UnityGI gi)
        //{
//
        //    float4 c;
        //    c.rgb =s.Albedo;
        //    c.a = s.Alpha;
        //    return float4(s.Normal,1);  
        //}
//
        //inline void LightingWater_GI(
        //    SurfaceOutputStandard s,
        //    UnityGIInput data,
        //    inout UnityGI gi)
        //{
        //    LightingStandard_GI(s, data, gi);
        //}


        void vert (inout appdata v) {
            float a = _Amplitude;
	        float f = _Frequency;
	        float s = _Speed;
            
            
            int depth = _Depth;
            float2 dis = float2(0,0);
            float2 dir = float2(0,1);
            float2 off = float2(0,0);
            float total_disp = 0;
            for(int i=0;i<depth;i++){
                disp d = displace(v.vertex.xy+off,dir,a,f,s);
                total_disp += d.height;
                dis += d.derivative;
                off = d.derivative;
                a *= _AmplitudeFalloff;
		        f *= _FrequencyFalloff;
		        s *= _SpeedFalloff;
                dir = mul(make_rotator(17.42),dir);
                
            }
            float3 t = float3(1, 0,dis.x);
            float3 bin = float3(0, 1, dis.y);
            float3 n = cross(t,bin);
            v.vertex.z = total_disp;
            v.normal = n;
            
        }
        
        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
            o.Smoothness = _Glossiness;
            o.Specular = _Specular;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
