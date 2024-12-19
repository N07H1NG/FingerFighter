Shader "Custom/OutlineCombined"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Outline ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size",Float) = 1
    }
    SubShader
    {


        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 200
        Stencil{
            ref 1
            comp always
            pass replace
        }
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

        Pass
        {
            Tags {}
            Stencil{
                ref 1
                comp notequal
                pass zero
                
            }
            //ZTest Always
            //ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"
            
            float4 _Outline;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f vert (appdata v)
            {
                
                v2f o;
                
                float4 clip = UnityObjectToClipPos(v.vertex);
                float4 vec = UnityObjectToClipPos(float4(_OutlineSize*v.normal.xyz,0));
                o.vertex = clip+float4(vec.xyz/1000,0); 
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = _Outline;
                return float4(col);
            }
            ENDCG
        }
        Pass
        {
            Tags {}
            Stencil{
                ref 1
                comp always
                pass zero
                
            }
            //ZTest Always
            ColorMask 0
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"
            
            float4 _Outline;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f vert (appdata v)
            {
                
                v2f o;
                
                float4 clip = UnityObjectToClipPos(v.vertex);
                float4 vec = UnityObjectToClipPos(float4(_OutlineSize*v.normal.xyz,0));
                o.vertex = clip+float4(vec.xyz/1000,0); 
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                return float4(0,0,0,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
