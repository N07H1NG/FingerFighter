Shader "Custom/Staand"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Outline ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size",Float) = 1
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

    }

    
    SubShader
    {
        Stencil{
            ref 1
            comp always
            pass replace
        }
        Pass
        {

            
            Cull Front
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
                float4 oldvertex : TEXCOORD1;
                float4 newvertex : TEXCOORD2;
            };

            

            v2f vert (appdata v)
            {
                v2f o;
                o.oldvertex = UnityObjectToClipPos(v.vertex);
                v.vertex.xyz+=_OutlineSize*v.normal/900;
                o.vertex = UnityObjectToClipPos(v.vertex); 
                o.newvertex = o.vertex;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Outline;
                clip((i.oldvertex-i.newvertex).z+20);
                return col;
            }
            ENDCG
        }

        Stencil{
            ref 1
            comp equal
            pass replace
        }
        
        Tags { "RenderType"="Opaque" }
        LOD 200

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
            //clip(-1);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    
}
    
    FallBack "Diffuse"
}
