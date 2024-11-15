Shader "Hidden/MyPostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Scale",Float) = 1.0
        _DepthThreshold("Threshold", Float) = 1.0
        _NormalThreshold("Normal Threshold", Float) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _CameraDepthNormalsTexture;
            float _Scale;
            float _DepthThreshold;
            float _NormalThreshold;

            fixed4 frag (v2f i) : COLOR
            {
                float halfScaleFloor = floor(_Scale * 0.5);
                float halfScaleCeil = ceil(_Scale * 0.5);
                
                float2 bottomLeftUV = i.uv - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 topRightUV = i.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;  
                float2 bottomRightUV = i.uv + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 topLeftUV = i.uv + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);
                
                float4 center = tex2D(_CameraDepthNormalsTexture, i.uv);
                float4 bl = tex2D(_CameraDepthNormalsTexture, bottomLeftUV);
                float4 tr = tex2D(_CameraDepthNormalsTexture, topRightUV);
                float4 br = tex2D(_CameraDepthNormalsTexture, bottomRightUV);
                float4 tl = tex2D(_CameraDepthNormalsTexture, topLeftUV);


                float depth;
                float3 normal;
                float4 col = tex2D(_MainTex,i.uv);
                float3 normalValuesBL;
                float depthValueBL;
                float3 normalValuesTR;
                float depthValueTR;
                float3 normalValuesBR;
                float depthValueBR;
                float3 normalValuesTL;
                float depthValueTL;
                
                DecodeDepthNormal(center, depth, normal);
                DecodeDepthNormal(bl, depthValueBL, normalValuesBL);
                DecodeDepthNormal(tr, depthValueTR, normalValuesTR);
                DecodeDepthNormal(br, depthValueBR, normalValuesBR);
                DecodeDepthNormal(tl, depthValueTL, normalValuesTL);


                float3 normalFiniteDifference0 = normalValuesTL - normalValuesBR;
                float3 normalFiniteDifference1 = normalValuesTR - normalValuesBL;

                float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
                edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;
                return edgeNormal;
                float depthFiniteDifference0 = depthValueBL - depthValueTR;
                float depthFiniteDifference1 = depthValueBR - depthValueTL;
                float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
                
                
                
                return edgeDepth>_DepthThreshold*depth;
                
            }
            ENDCG
        }
    }
}
