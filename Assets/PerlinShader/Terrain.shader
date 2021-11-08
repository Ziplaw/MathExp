Shader "Unlit/Terrain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Octaves ("Octaves",Range(0,10)) = 8
        _Amplitude ("Amplitude",Range(0,10)) = 1
        _Frequency ("Frequency",float) = 15
        _Persistence ("Persistence",Range(0,1)) = .234
        _Lacunarity ("Lacunarity",Range(1,10)) = 3.5
        //        _HeightSamples ("HeightSamples",float[]) = {1}
    }



    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100



        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "Assets/noiseSimplex.cginc"

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

            float _Octaves;
            float _Amplitude;
            float _Frequency;
            float _Persistence;
            float _Lacunarity;

            int heightNumberOfSamples;
            float height[100];

            void Remap(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }


            v2f vert(appdata v)
            {
                v2f o;

                _Frequency *= .01;
                float noise;
                float maxHeight = _Amplitude;


                for (int i = 0; i < _Octaves; i++)
                {
                    float noiseTemp = snoise(
                            float2(mul(v.vertex,UNITY_MATRIX_M).x, mul(v.vertex,UNITY_MATRIX_M).z) * _Frequency) *
                        _Amplitude;

                    _Amplitude *= _Persistence;
                    _Frequency *= _Lacunarity;

                    noise += noiseTemp;
                }
                float remapped = 0;

                Remap(noise, float2(0, maxHeight * 2 * .61), float2(50, 100), remapped);

                v.vertex.y = noise * height[remapped];

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                return float4(.1, .1, .1, 1);
            }
            ENDCG
        }
    }
}