Shader "Unlit/PortalShader"
{
    Properties
    {

    }

        SubShader
    {
        // More variables here https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
        Tags { "RenderType" = "Transparent"}
        //Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _MainTex_ST;
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); //Insert NOTE: this is a declaration of _MainTex

            float3 _COLOR_0;
            float3 _COLOR_1;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                float4 vert = UnityObjectToClipPos(v.vertex);

                float _Radius = 10;

                o.vertex = float4(vert.x, vert.y, vert.z, vert.w);

                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            //// Comparison functions
            float gt(float v1, float v2)
            {
                return step(v2, v1);
            }

            float lt(float v1, float v2)
            {
                return step(v1, v2);
            }

            float between(float val, float start, float end)
            {
                return gt(val, start) * lt(val, end);
            }

            float eq(float v1, float v2, float e)
            {
                return between(v1, v2 - e, v2 + e);
            }

            float s_gt(float v1, float v2, float e)
            {
                return smoothstep(v2 - e, v2 + e, v1);
            }

            float s_lt(float v1, float v2, float e)
            {
                return smoothstep(v1 - e, v1 + e, v2);
            }

            float s_between(float val, float start, float end, float epsilon)
            {
                return s_gt(val, start, epsilon) * s_lt(val, end, epsilon);
            }

            float s_eq(float v1, float v2, float e, float s_e)
            {
                return s_between(v1, v2 - e, v2 + e, s_e);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - .5;

                uv *= 50;
                
                float x = uv.x;
                float y = uv.y;

                float r = sqrt(x * x + y * y);
                float a = atan(y/x);

                fixed4 col = fixed4(0.0125, 0.025, 0.05, 1);

                float time = _Time.y;

                col.rg += s_eq(cos(r - a + time), sin(a - r / 2. + time * 2.), 0.5, 0.2);
                col.r += s_eq(cos(r - a + time), sin(a - r / 2. + time * 5.), 0.25, 0.75) / 5.;
                col.gb += s_eq(sin(r - a + time / 2.), cos(a - r / 2. + time * 5.), 0.5, 0.2);

                return col;
            }
        ENDCG
        }
    }
    Fallback "Diffuse"
}
