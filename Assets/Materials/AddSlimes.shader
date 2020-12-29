Shader "Unlit/AddSlimes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AddedTex ("Added Texture", 2D) = "white" {}
        _Speed ("Scroll Speed", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AddedTex;
            float4 _Speed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 additiveCol = tex2D(_AddedTex, (i.uv + _Time.yy * _Speed.xy) % float2(1, 1));
                return col + fixed4(additiveCol.rgb * additiveCol.a, 0);
            }
            ENDCG
        }
    }
}