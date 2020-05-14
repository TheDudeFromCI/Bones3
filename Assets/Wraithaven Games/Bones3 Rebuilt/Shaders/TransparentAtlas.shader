Shader "Bones3/Transparent Voxel Atlas"
{
    Properties
    {
        _MainTex ("Texture", 2DArray) = "" {}
        [Toggle(SMOOTH_PIXELS)] _SmoothPixels ("Smooth Pixels?", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows vertex:vert alpha
        #pragma require 2darray
        #pragma target 3.5

        #pragma shader_feature SMOOTH_PIXELS

        UNITY_DECLARE_TEX2DARRAY(_MainTex);
        float4 _MainTex_TexelSize;

        struct Input
        {
            float2 uv_MainTex;
            float texIndex;
        };

        void vert(inout appdata_full v, out Input o)
        {
            o.uv_MainTex = v.texcoord.xy;
            o.texIndex = v.texcoord.z;
        }

        float mipLevel(float2 uv)
        {
            float2 dx = ddx(uv * _MainTex_TexelSize.z);
            float2 dy = ddy(uv * _MainTex_TexelSize.w);
            float d = max(dot(dx, dx), dot(dy, dy));

            float levels = log2(_MainTex_TexelSize.z) - 1;
            const float rangeClamp = pow(2, levels * 2);
            d = clamp(d, 1.0, rangeClamp);

            return 0.5 * log2(d);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            float3 uv = float3(IN.uv_MainTex, IN.texIndex);

#ifdef SMOOTH_PIXELS
            float mipmap = mipLevel(uv.xy);
            fixed4 col1 = UNITY_SAMPLE_TEX2DARRAY_LOD(_MainTex, uv, floor(mipmap));
            fixed4 col2 = UNITY_SAMPLE_TEX2DARRAY_LOD(_MainTex, uv, ceil(mipmap));
            fixed4 col = lerp(col1, col2, frac(mipmap));
#else
            fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uv);
#endif

            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
