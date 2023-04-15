Shader "Unlit/RayMarcher"

{
    Properties
    {
        //_MainSDF ("Texture", 3D) = "white" {}
        _DETAILS ("Details", Range(0, 16)) = 0
        _GLOW_COLOR ("Glow color", Color) = (1, 1, 1)

        _COLOR_0 ("Color 0", Color) = (1, 1, 1)
        _COLOR_1 ("Color 1", Color) = (1, 1, 1)
        _COLOR_2 ("Color 2", Color) = (1, 1, 1)
        _COLOR_3 ("Color 3", Color) = (1, 1, 1)
        _COLOR_4 ("Color 4", Color) = (1, 1, 1)
        _COLOR_5 ("Color 5", Color) = (1, 1, 1)
        _COLOR_DEFAULT ("Default Color", Color) = (1, 1, 1)

        /*-------------Other properties-------------*/
        //_MyColor("Some Color", Color) = (1,1,1,1)
        //_MyVector("Some Vector", Vector) = (0,0,0,0)
        //_MyFloat("My float", Float) = 0.5
        //_MyTexture("Texture", 2D) = "white" {}
        //_MyCubemap("Cubemap", CUBE) = "" {}
    }

    SubShader
    {
        // More variables here https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
        Tags { "Queue" = "Transparent" } // Draw after all opaque geometry

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            //"_CameraDepthNormalsTexture"
            "_BackgroundTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            /*-------MAIN INCLUDES-------*/
            #include "RayMarchHelpers/mainMarcher.hlsl"
            /*---------------------------*/

            // Main SDF
            //sampler3D _MainSDF;

            float4 _MainTex_ST;
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); //Insert NOTE: this is a declaration of _MainTex
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthTexture);
            //UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthNormalsTexture);
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_BackgroundTexture);

            // Uniforms
            uniform float3 _LightPos;
            //uniform float3 _LightCol;
            uniform float3 _PlayerPos;

            // Material Uniforms
            float _DETAILS;
            float3 _GLOW_COLOR;
            float3 _COLOR_0;
            float3 _COLOR_1;
            float3 _COLOR_2;
            float3 _COLOR_3;
            float3 _COLOR_4;
            float3 _COLOR_5;
            float3 _COLOR_DEFAULT;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD1;
                float3 ro : TEXCOORD2;
                float3 rd : TEXCOORD3;
                float4 screenSpace : TEXCOORD4;

                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenSpace = ComputeScreenPos(o.vertex);

                o.ro = _WorldSpaceCameraPos;
                float3 vertexHit = mul(unity_ObjectToWorld, v.vertex);

                o.rd = vertexHit - o.ro;

                return o;
            }

            float3 GetAlbedo(int id) {
                switch (id) {
                case 0:
                    return _COLOR_0;
                case 1:
                    return _COLOR_1;
                case 2:
                    return _COLOR_2;
                case 3:
                    return _COLOR_3;
                case 4:
                    return _COLOR_4;
                case 5:
                    return _COLOR_5;
                default:
                    return _COLOR_DEFAULT;
                }
            }

            float3 OrbitTrapColoring(float3 p) {
                float c = 50 / length(p);
                float f = OrbitTrapValue(p, c);
                float3 v = lerp(GetAlbedo(4), GetAlbedo(5), gt(f,0.45));

                return v;
            }

            float3 GetPixel(float3 ro, float3 rd, float depth, float4 grabPos) {
                int details = int(_DETAILS);
                March march = RayMarch(ro, rd, depth, details, _PlayerPos);
                float dE = march.disMarched;
                float3 hp = ro + dE * rd;

                // Ambient
                float Apower = 0.1;
                float A = 1.;

                // Diffuse
                float Dpower = 0.75;
                float D = 1.;
                
                // Specular
                float Spower = 0.25;
                float S = 0.;

                // Glow
                float Gpower = 0.25;
                float3 G = 0.;

                float3 n, l, albedo; 
                float shadow;
                if (dE >= MAX_DIS || dE >= depth) {
                    albedo = tex2Dproj(_BackgroundTexture, grabPos).rgb;
                }
                else
                {
                    albedo = GetAlbedo(march.hit.id);
                }

                if (dE >= depth || dE < MAX_DIS) { // There was a hit
                    n = GetNormal(hp, details, _PlayerPos);
                    l = normalize(_LightPos-hp);

                    if (dE < depth) { // Only calculate Diffuse if no polygonal hit
                        D = clamp(dot(n,l), 0., 1.);

                        float Shardness = 8.;
                        S = pow(clamp(dot(n,l), 0., 1.), Shardness);

                        float glow = float(march.disMarched)/float(MAX_STEPS);
                        G = glow * _GLOW_COLOR;
                    }
                    
                    A *= Apower;
                    D *= Dpower;
                    S *= Spower;

                    G *= Gpower;

                    // Shadow
                    //D *= LightMarch(hp, n, l, _LightPos, 16, details);

                    return G + albedo * (A + D + S);
                } else {
                    if (dE > MAX_DIS) { 
                        float glow = float(march.disMarched) / float(MAX_STEPS);
                        G = glow * _GLOW_COLOR * Gpower;
                        return G; 
                    }
                    else return albedo;
                }

            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;
                //if (screenSpaceUV.y < 0.25) return 0;
                //if (screenSpaceUV.y > 0.725) return 0;

                float3 ro = i.ro;
                float3 rd = normalize(i.rd);

                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV));
                //return depth/100;

                float3 forwardVector = -UNITY_MATRIX_V[2].xyz;
                depth *= length(rd / dot(rd, forwardVector));
                depth += HIT_EPS*20; // remove z fighting

                fixed4 col = 0;
                col.rgb = GetPixel(ro, rd, depth, i.grabPos);
                //col.rgb = tex2Dproj(_BackgroundTexture, i.grabPos).rgb;
                
                return col;
            }
            ENDCG
        }
    }
}
