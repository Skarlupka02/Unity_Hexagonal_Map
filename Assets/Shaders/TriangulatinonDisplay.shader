Shader "Custom/TriangulationDisplay"
{
    Properties
    {
        _Color ("Face Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        _EdgeThickness ("Edge Thickness", Range(0, 0.1)) = 0.01
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
            };
            
            struct geom_out
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float edge : TEXCOORD1;
            };
            
            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeThickness;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            [maxvertexcount(3)]
            void geom(triangle v2f input[3], inout TriangleStream<geom_out> triStream)
            {
                geom_out o;
                
                for (int i = 0; i < 3; i++)
                {
                    o.vertex = input[i].vertex;
                    o.normal = input[i].normal;
                    o.worldPos = input[i].worldPos;
                    
                    // Вычисляем расстояние до соседних вершин
                    float3 v1 = input[(i + 1) % 3].worldPos - input[i].worldPos;
                    float3 v2 = input[(i + 2) % 3].worldPos - input[i].worldPos;
                    
                    // Если близко к ребру, устанавливаем флаг
                    o.edge = length(v1) < _EdgeThickness || length(v2) < _EdgeThickness ? 1 : 0;
                    
                    triStream.Append(o);
                }
                triStream.RestartStrip();
            }
            
            fixed4 frag(geom_out i) : SV_Target
            {
                return i.edge > 0 ? _EdgeColor : _Color;
            }
            ENDCG
        }
    }
}
