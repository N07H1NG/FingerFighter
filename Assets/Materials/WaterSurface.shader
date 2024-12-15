Shader "Custom/WaterSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amplitude ("Amplitude", Float) = 0.5
        _Frequency ("Frequency", Float) = 0.5
        _Speed ("Speed", Float) = 0.5
        _AmplitudeFalloff ("Amplitude Falloff", Float) = 0.5
        _FrequencyFalloff ("Frequency Falloff", Float) = 0.5
        _SpeedFalloff ("Speed Falloff", Float) = 0.5
        _Depth("Depth",Float) =10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows addshadow vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 normal :NORMAL;
        };

        struct disp
        {
            float height;
            float3 normal;
            float2 derivative;
        };

        half _Glossiness;
        half _Metallic;
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

        disp displace(float2 uv, float2 dir, float a, float f, float s){
            dir /= length(dir);
            //float h = a*sin((dot(dir,uv)) * f + _Time.w * s) ;
            float h = a*exp(sin((dot(dir,uv)) * f + _Time.w * s)-1.0) ;
            float c =  cos((dot(dir,uv)) * f + _Time.w * s);
            c = c*h;
            float3 t = float3(1.0, 0.0,dir.x*c);
            float3 bin = float3(0.0, 1.0, dir.y*c);
            float3 n = cross(t,bin);
            disp res;
            res.height = h;
            res.normal = n;
            res.derivative = float2(dir.x*c,dir.y*c);
            return res;
        }

        float myhash(float2 p) {
            return 0.4*dot(p, float2(1, 12.7));
        }
          
        float2x2 make_rotator(float angle){
            return float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
        }


        void vert (inout appdata_full v) {
            float a = _Amplitude;
	        float f = _Frequency;
	        float s = _Speed;
            
            
            int depth = _Depth;
            float3 norm = float3(0,0,0);
            float2 dis = float2(0,0);
            float2 dir = float2(0,1);
            float total_disp = 0;
            for(int i=0;i<depth;i++){
                disp d = displace(v.texcoord.xy+dis,dir,a,f,s);
                total_disp+= d.height;
                dis += d.derivative;
                norm += d.normal;
                a *= _AmplitudeFalloff;
		        f *= _FrequencyFalloff;
		        s *= _SpeedFalloff;
                dir = mul(make_rotator(myhash(dir)),dir);
            }
            float3 t = float3(1.0, 0.0,dis.x);
            float3 bin = float3(0.0, 1.0, dis.y);
            float3 n = cross(t,bin);
            v.vertex.z = total_disp;
            v.normal = n;

        }
        
        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c;
            o.Specular = _Metallic;
              // 0=rough, 1=smooth
            o.Occlusion = 1;
            //o.Gloss = 100000000;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
