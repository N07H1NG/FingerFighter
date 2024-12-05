Shader "Unlit/Outline"
{
    Properties
    {
        _Outline ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        LOD 100
        
        //Cull Front
        Stencil{
            ref 1
            comp always
            pass replace
        }
        Pass
        {
            //ZTest Always
            
            Blend  SrcAlpha OneMinusSrcAlpha
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
                float2 to_edge : TEXCOORD1;
            };

            

            v2f vert (appdata v)
            {
                
                v2f o;
                
                float4 clip = UnityObjectToClipPos(v.vertex);
                float4 vec = UnityObjectToClipPos(float4(_OutlineSize*v.normal.xyz,0));
                float4 offset = UnityObjectToClipPos(v.vertex +_OutlineSize*v.normal/900);
                o.vertex = clip+float4(vec.xyz/1000,0); 
                //o.vertex = offset; 
                o.to_edge = vec.xy/_OutlineSize;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //clip(-1);
                // sample the texture
                fixed4 col = _Outline;
                float somth = length(i.to_edge)/200;
                return float4(somth,0,0,1);
                //return float4(somth-360,0,0,0);
                somth = somth/fwidth(12*somth);
                return float4(col.xyz,saturate(somth*col.w));
            }
            ENDCG
        }
    }
}
