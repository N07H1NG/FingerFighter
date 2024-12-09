

Shader "Hidden/MyPostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Grid Scale",Float) = 1.0
        _Angle1("Grid Rotation",Float) = 0.0
        _Angle2("Grid Rotation2",Float) = 0.0
        _Angle3("Grid Rotation3",Float) = 0.0
        _Angle4("Grid Rotation4",Float) = 0.0
        _BG("Background",2D) = "white" {}
        _Cyan("Cyan Ink", Color) = (0,1,1,1)
        _Magenta("Magenta Ink", Color) = (1,0,1,1)
        _Yellow("Yellow Ink", Color) = (1,1,0,1)
        _Black("Black Ink", Color) = (0,0,0,1)
        _Noise("Noise Texture", 2D) = "white" {}
        _NoiseBlack("Noise Texture Black", 2D) = "white" {}
        _GridShift("Grid shift",Float) = 0.0
        
        _NoiseScale("Noise Scale",Float) = 1.0
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

            static const float PI = 3.14159265f;
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 dir : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.dir = mul(unity_CameraToWorld,float4(0,0,1,0)).xyz;
                o.uv = v.uv; 
                
                return o;
            }

            float4 RGBtoCMYK (float3 rgb) {
                float r = rgb.r;
                float g = rgb.g;
                float b = rgb.b;
                float k = min(1.0 - r, min(1.0 - g, 1.0 - b));
                float3 cmy = 0.0;
                float invK = 1.0 - k;
                if (invK != 0.0) {
                    cmy.x = (1.0 - r - k) / invK;
                    cmy.y = (1.0 - g - k) / invK;
                    cmy.z = (1.0 - b - k) / invK;
                }
                return clamp(float4(cmy, k), 0.0, 1.0);
            }

            sampler2D _MainTex;
            sampler2D _Noise;
            sampler2D _NoiseBlack;
            float _Scale;
            float _Angle1;
            float _Angle2;
            float _Angle3;
            float _Angle4;
            float _NoiseScale;
            sampler2D _BG;
            float4 _Cyan;
            float4 _Magenta;
            float4 _Yellow;
            float4 _Black;
            float _GridShift;

            float remapdist(float r){
                r/=2;
                if (r <= 0.5){
                    //return 0;
                    return PI * r*r;
                }
                if (r > sqrt(2)/2){
                    return 1;
                }
                //return 0;
                float cosa = 0.5/r;
                float angle = 2 * acos(cosa);
                float segm = (1.0/2) * (r*r) * (angle - sin(angle));
                float area = PI * r*r - 4 * segm;
                return area;
            }

            float4 frag (v2f i) : COLOR
            {
                i.dir = normalize(i.dir);
                float3 flatdir = normalize(float3(i.dir.x,0,i.dir.z));
                float s = (sign(cross(flatdir,float3(1,0,0)).y)+1)/2.0;
                float d1 = acos(dot(flatdir,float3(1,0,0)))/PI;
                d1 = s*d1 + (1-s)*(2-d1);
                d1/=2;

                

                
                
                
                
                
                float4 noise = tex2D(_Noise,i.vertex.xy/(32*_Scale)-float2(d1*_Scale,0));
                
                
                float2 scaler = float2(ddx(i.uv.x),ddy(i.uv.y));
                
                

                float4 col = tex2D(_MainTex,i.uv+scaler*float2(-3,-3));
                float4 col2 = tex2D(_MainTex,i.uv+scaler*float2(-2,0));
                float4 col3 = tex2D(_MainTex,i.uv+scaler*float2(3,2));
                float4 col4 = tex2D(_MainTex,i.uv);
                float4 bg = tex2D(_BG,i.vertex.xy/512);
                
                
                float2x2 rot1 = {cos(_Angle1),sin(_Angle1),-1*sin(_Angle1),cos(_Angle1)};
                float2x2 rot2 = {cos(_Angle2),sin(_Angle2),-1*sin(_Angle2),cos(_Angle2)};
                float2x2 rot3 = {cos(_Angle3),sin(_Angle3),-1*sin(_Angle3),cos(_Angle3)};
                float2x2 rot4 = {cos(_Angle4),sin(_Angle4),-1*sin(_Angle4),cos(_Angle4)};
                float2 prikol1 = mul(rot1,i.vertex.xy);
                float2 prikol2 = mul(rot2,i.vertex.xy);
                float2 prikol3 = mul(rot3,i.vertex.xy);
                float2 prikol4 = mul(rot4,i.vertex.xy);

                

                prikol1.x+=_GridShift*_Scale*floor(prikol1.y/_Scale);
                prikol2.x+=_GridShift*_Scale*floor(prikol2.y/_Scale);
                prikol3.x+=_GridShift*_Scale*floor(prikol3.y/_Scale);
                prikol4.x+=_GridShift*_Scale*floor(prikol4.y/_Scale);

                
                float2 squarenum1 = floor(prikol1/_Scale);
                float2 squarenum2 = floor(prikol2/_Scale);
                float2 squarenum3 = floor(prikol3/_Scale);
                float2 squarenum4 = floor(prikol4/_Scale);
                
                

                prikol1 = frac(prikol1/_Scale)*2-1;
                prikol2 = frac(prikol2/_Scale)*2-1;
                prikol3 = frac(prikol3/_Scale)*2-1;
                prikol4 = frac(prikol4/_Scale)*2-1;
                
                float dist1 = length(prikol1);
                float dist2 = length(prikol2);
                float dist3 = length(prikol3);
                float dist4 = length(prikol4);
                
                
                dist1 = remapdist(dist1);
                dist2 = remapdist(dist2);
                dist3 = remapdist(dist3);
                dist4 = remapdist(dist4);
                //return float4((prikol1+1)/2,0,0);
                float n1 = (1-max(abs(prikol1.x),abs(prikol1.y)))*tex2D(_Noise,squarenum1*0.796+prikol1/100+mul(rot1,float2(d1,0)));
                float n2 = (1-max(abs(prikol2.x),abs(prikol2.y)))*tex2D(_Noise,squarenum2*0.796+prikol2/100+mul(rot2,float2(d1,0)));
                float n3 = (1-max(abs(prikol3.x),abs(prikol3.y)))*tex2D(_Noise,squarenum3*0.796+prikol3/100+mul(rot3,float2(d1,0)));
                float n4 = (1-max(abs(prikol4.x),abs(prikol4.y)))*tex2D(_Noise,squarenum4*0.796+prikol4/100+mul(rot4,float2(d1,0)));
                //return n1;
                dist1 += n1;
                dist2 += n2;
                dist3 += n3;
                dist4 += n4; 
                

                
                float4 cmyk = RGBtoCMYK(col.xyz);
                float4 cmyk2 = RGBtoCMYK(col2.xyz);
                float4 cmyk3 = RGBtoCMYK(col3.xyz);
                
                float4 cmyk4 = RGBtoCMYK(col4.xyz);
                
                cmyk = float4(cmyk.x,cmyk2.y,cmyk3.z,cmyk4.w);
                //return cmyk.z;
                //noise = float4(0,0,0,0);
                //return cmyk.x;
                cmyk = saturate(float4(cmyk.x>dist1,cmyk.y>dist2,cmyk.z>dist3,cmyk.w>dist4)); //IMPORTASNT!!
                
                
                
                float4 removal = 1-(cmyk.w*(1-_Black)+cmyk.x*(1-_Cyan) + cmyk.y*(1-_Magenta) + cmyk.z*(1-_Yellow));
                float ink = (0.99+(3*cmyk.w+cmyk.x+cmyk.y+cmyk.z)/600)*saturate((cmyk.w+cmyk.x+cmyk.y+cmyk.z));
                return bg*(1-ink)+ink*removal*bg;
                
                
                
                
                
                
            }
            ENDCG
        }

        
    }
}
