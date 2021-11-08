Shader "Unlit/CoordinateTests"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha("Alpha",Range(0,1)) = 0
        _LambertPosition("Lambert Position",Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
                float3 lambert : TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Alpha;
            float4 _LambertPosition;

            v2f vert (appdata v)
            {
                v2f o;


                float alpha = _Alpha * _Time.w;

                float4x4 scaleMatrix = float4x4(
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                0,0,0,1
                );

                float4x4 rotationMatrix = float4x4(
                cos(alpha),-sin(alpha),0,0,
                sin(alpha),cos(alpha),0,0,
                0,0,1,0,
                0,0,0,1
                );

                float4x4 translationMatrix = float4x4(
                1,0,0,0,
                0,1,0,1,
                0,0,1,0,
                0,0,0,1
                );

                float4x4 SRmatrix = mul(scaleMatrix,rotationMatrix);
                float4x4 RTmatrix = mul(rotationMatrix,translationMatrix);
                float4x4 TRmatrix = mul(translationMatrix,rotationMatrix);

                float4x4 SRTmatrix = mul(SRmatrix,translationMatrix);
                float4x4 RTSmatrix = mul(RTmatrix,scaleMatrix);
                float4x4 TRSmatrix = mul(TRmatrix,scaleMatrix);
                

                //  v.vertex = mul(TRSmatrix,v.vertex);


                // o.vertex = UnityObjectToClipPos(v.vertex);
                // v.vertex = _WorldPositionOverride;

                // v.vertex = float4(v.vertex.xyz,0);

                o.vertex = UnityObjectToClipPos (v.vertex);

                o.vertex = float4(o.vertex.xyzw);

                float3 worldSpaceLambertPos = mul(UNITY_MATRIX_M,_LambertPosition);
                float3 worldSpaceVertexPos = mul(UNITY_MATRIX_M,v.vertex);

                float4 screenPos = ComputeScreenPos(o.vertex);
                o.lambert =  _Alpha*(1-(dot(  v.normal, normalize(worldSpaceVertexPos - worldSpaceLambertPos))))/distance(worldSpaceLambertPos,worldSpaceVertexPos);
                

                o.screenPos = screenPos / screenPos.w;

                // o.vertex += move;
                // o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // return float4(i.screenPos.xy,0,0);
                // return float4(i.uv,0,1);
                return float4(i.lambert.xyz,1);
            }
            ENDCG
        }
    }
}
