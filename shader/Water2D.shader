Shader "Unlit/Water2D"
{
    Properties
    {
        // 贴图（你的水纹 PNG）
        _MainTex ("Texture", 2D) = "white" {}
        _Color   ("Tint Color", Color) = (1,1,1,1)

        // 顶点波浪参数（控制水面的起伏）
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1   // 波高
        _WaveFrequency ("Wave Frequency", Float) = 2.0   // 频率(一屏多少个波)
        _WaveSpeed     ("Wave Speed",     Float) = 1.0   // 波动速度
        _Phase         ("Phase Offset",   Float) = 0.0   // 相位偏移（可不管）

        // 里面的水纹流动/扭曲
        _DistortionStrength ("Distortion Strength", Float) = 0.03
        _DistortionSpeed    ("Distortion Speed",    Float) = 0.8
        _DistortionScale    ("Distortion Scale",    Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        // 透明 2D Sprite 常用设置
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _Phase;

            float _DistortionStrength;
            float _DistortionSpeed;
            float _DistortionScale;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // ---- 顶点波浪：只让上边当“水面”左右起伏 ----
                float t = _Time.y * _WaveSpeed + _Phase;

                // 基础波形（X 方向）
                float waveMain   = sin(v.vertex.x * _WaveFrequency + t);
                float waveDetail = 0.5 * sin(v.vertex.x * _WaveFrequency * 1.7 + t * 1.3);
                float wave       = waveMain + waveDetail;

                // v.uv.y 越接近 1 越靠近上边；越接近 0 越靠近底部
                // 0.6 以下几乎不动，0.6~1 区间从 0 过渡到 1
                float surfaceFactor = saturate( (v.uv.y - 0.6) / 0.4 );

                // 只抬/压上半部分的顶点 -> 形成波浪水面
                v.vertex.y += wave * _WaveAmplitude * surfaceFactor;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ---- 水体内部的纹理轻微扭曲，让它“流动” ----
                float t = _Time.y * _DistortionSpeed;

                // 简单 2D 噪声（用 sin/cos 代替）
                float2 noiseUV = i.uv * _DistortionScale;
                float n = sin(noiseUV.x * 10.0 + t) * cos(noiseUV.y * 10.0 - t);

                float2 distortedUV = i.uv;
                distortedUV.x += n * _DistortionStrength;
                distortedUV.y += n * _DistortionStrength;

                fixed4 col = tex2D(_MainTex, distortedUV) * _Color;

                // 保留 alpha，方便你用半透明 PNG 只画下面的水色，上半截透明当水面
                return col;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
