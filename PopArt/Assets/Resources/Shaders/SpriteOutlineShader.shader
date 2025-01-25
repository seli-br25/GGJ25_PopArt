Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,0,1) // Gold color
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texSize = float2(_OutlineThickness, _OutlineThickness);
                float alpha = tex2D(_MainTex, i.texcoord).a;

                if (alpha > 0.0)
                {
                    // Check neighboring pixels for outline
                    float left = tex2D(_MainTex, i.texcoord - float2(texSize.x, 0)).a;
                    float right = tex2D(_MainTex, i.texcoord + float2(texSize.x, 0)).a;
                    float up = tex2D(_MainTex, i.texcoord + float2(0, texSize.y)).a;
                    float down = tex2D(_MainTex, i.texcoord - float2(0, texSize.y)).a;

                    if (left < 0.1 || right < 0.1 || up < 0.1 || down < 0.1)
                    {
                        return _OutlineColor; // Return outline color
                    }
                }

                return tex2D(_MainTex, i.texcoord); // Return original texture
            }
            ENDCG
        }
    }
}
