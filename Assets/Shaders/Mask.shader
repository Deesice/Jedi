Shader "Hidden/Mask"
{
    Properties
    {
        _MainTex("Main texture", 2D) = "white" {}
        _Scale ("Scale", Float) = 0
        _Aspect ("Aspect", Float) = 1
        _OffsetY ("OffsetY", Float) = 0
        _OffsetX ("OffsetX", Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite On ZTest Always

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
            float _Scale;
            float _Aspect;
            float _OffsetX;
            float _OffsetY;

            fixed4 frag(v2f i) : SV_Target
            {
                if (_Scale == 0)
                    return fixed4(0, 0, 0, 1);
                float2 coords = (i.uv - float2(0.5, 0.5));
                if (((coords.x - _OffsetX) * (coords.x - _OffsetX) * _Aspect * _Aspect + (coords.y - _OffsetY) * (coords.y - _OffsetY)) < _Scale)
                    return tex2D(_MainTex, i.uv);
                else
                    return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
