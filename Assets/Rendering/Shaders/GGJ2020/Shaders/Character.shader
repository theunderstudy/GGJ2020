// Toony Colors Pro+Mobile 2
// (c) 2014-2019 Jean Moreno

Shader "GGJ2020/Shaders/Character"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_Color ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_MainTex ("Albedo", 2D) = "white" {}
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		[Header(Main Directional Light)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		[Header(Other Lights)]
		_RampThresholdOtherLights ("Threshold", Range(0.01,1)) = 0.5
		[Space]
		[TCP2Separator]
		
		[TCP2HeaderHelp(Specular)]
		[TCP2ColorNoAlpha] _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_SpecularSmoothnessPBR ("Smoothness", Range(0,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Rim Lighting)]
		[TCP2ColorNoAlpha] _RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.5)
		_RimMinVert ("Rim Min", Range(0,2)) = 0.5
		_RimMaxVert ("Rim Max", Range(0,2)) = 1
		//Rim Direction
		_RimDirVert ("Rim Direction", Vector) = (0,0,1,1)
		[TCP2Separator]
		
		_StylizedThreshold ("Stylized Threshold", 2D) = "gray" {}
		[TCP2Separator]
		
		[TCP2HeaderHelp(Outline)]
		_OutlineWidth ("Width", Range(0.1,4)) = 1
		_OutlineColorVertex ("Color", Color) = (0,0,0,1)
		//This property will be ignored and will draw the custom normals GUI instead
		[TCP2OutlineNormalsGUI] __outline_gui_dummy__ ("_unused_", Float) = 0
		[TCP2Separator]

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="AlphaTest+25"
		}

		// Outline Include
		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Shader Properties
		float _OutlineWidth;
		fixed4 _OutlineColorVertex;

		struct appdata_outline
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		#if TCP2_COLORS_AS_NORMALS
			float4 vertexColor : COLOR;
		#endif
		// TODO: need a way to know if texcoord1 is used in the Shader Properties
		#if TCP2_UV2_AS_NORMALS
			float2 uv2 : TEXCOORD1;
		#endif
		#if TCP2_TANGENT_AS_NORMALS
			float4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f_outline
		{
			float4 vertex : SV_POSITION;
			float4 screenPosition : TEXCOORD0;
			float4 vcolor : TEXCOORD1;
			float3 pack2 : TEXCOORD2; /* pack2.xyz = normal */
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f_outline vertex_outline (appdata_outline v)
		{
			v2f_outline output;
			UNITY_INITIALIZE_OUTPUT(v2f_outline, output);
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			// Shader Properties Sampling
			float __outlineWidth = ( _OutlineWidth );
			float4 __outlineColorVertex = ( _OutlineColorVertex.rgba );

			output.pack2.xyz = normalize(mul(unity_ObjectToWorld, v.normal).xyz);
		
		#ifdef TCP2_COLORS_AS_NORMALS
			//Vertex Color for Normals
			float3 normal = (v.vertexColor.xyz*2) - 1;
		#elif TCP2_TANGENT_AS_NORMALS
			//Tangent for Normals
			float3 normal = v.tangent.xyz;
		#elif TCP2_UV2_AS_NORMALS
			//UV2 for Normals
			float3 n;
			//unpack uv2
			v.uv2.x = v.uv2.x * 255.0/16.0;
			n.x = floor(v.uv2.x) / 15.0;
			n.y = frac(v.uv2.x) * 16.0 / 15.0;
			//- get z
			n.z = v.uv2.y;
			//- transform
			n = n*2 - 1;
			float3 normal = n;
		#else
			float3 normal = v.normal;
		#endif
			float size = 1;
		
		#if !defined(SHADOWCASTER_PASS)
			output.vertex = UnityObjectToClipPos(v.vertex + float4(normal,0) * __outlineWidth * size * 0.01);
		#else
			v.vertex = v.vertex + float4(normal,0) * __outlineWidth * size * 0.01;
		#endif
		
			output.vcolor.xyzw = __outlineColorVertex;
			float4 clipPos = output.vertex;

			//Screen Position
			float4 screenPos = ComputeScreenPos(clipPos);
			output.screenPosition = screenPos;
			return output;
		}

		float4 fragment_outline (v2f_outline input) : SV_Target
		{
			// Shader Properties Sampling
			float4 __outlineColor = ( float4(1,1,1,1) );

			half4 outlineColor = __outlineColor * input.vcolor.xyzw;
			half ndl = max(0, dot(input.pack2.xyz, _WorldSpaceLightPos0));
			outlineColor *= ndl;
			return outlineColor;
		}

		ENDCG
		// Outline Include End

		//Outline
		Pass
		{
			Name "Outline"
			Tags { "LightMode"="ForwardBase" }
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vertex_outline
			#pragma fragment fragment_outline
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile_instancing
			#pragma target 3.5
			ENDCG
		}
		// Main Surface Shader

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha addshadow nofog nolppv
		#pragma target 3.5

		//================================================================
		// VARIABLES

		// Shader Properties
		float3 _RimDirVert;
		float _RimMinVert;
		float _RimMaxVert;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _Color;
		sampler2D _StylizedThreshold;
		float4 _StylizedThreshold_ST;
		float _RampThreshold;
		float _RampThresholdOtherLights;
		fixed3 _HColor;
		fixed3 _SColor;
		float _SpecularSmoothnessPBR;
		fixed3 _SpecularColor;
		fixed3 _RimColor;
		
		//Specular help functions (from UnityStandardBRDF.cginc)
		inline half3 SafeNormalize(half3 inVec)
		{
			half dp3 = max(0.001f, dot(inVec, inVec));
			return inVec * rsqrt(dp3);
		}
		
		//GGX
		#define TCP2_PI 3.14159265359
		#define TCP2_INV_PI        0.31830988618f
		#if defined(SHADER_API_MOBILE)
			#define EPSILON 1e-4f
		#else
			#define EPSILON 1e-7f
		#endif
		inline half GGX(half NdotH, half roughness)
		{
			half a2 = roughness * roughness;
			half d = (NdotH * a2 - NdotH) * NdotH + 1.0f;
			return TCP2_INV_PI * a2 / (d * d + EPSILON);
		}

		//Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord0 : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			half4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			half3 viewDir;
			float4 screenPosition;
			half rim;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			// Shader Properties Sampling
			float3 __rimDirVert = ( _RimDirVert.xyz );
			float __rimMinVert = ( _RimMinVert );
			float __rimMaxVert = ( _RimMaxVert );

			float4 clipPos = UnityObjectToClipPos(v.vertex);

			//Screen Position
			float4 screenPos = ComputeScreenPos(clipPos);
			output.screenPosition = screenPos;
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			half3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

			half3 rViewDir = viewDir;
			half3 rimDir = __rimDirVert;
			rViewDir = normalize(UNITY_MATRIX_V[0].xyz * rimDir.x + UNITY_MATRIX_V[1].xyz * rimDir.y + UNITY_MATRIX_V[2].xyz * rimDir.z);
			half rim = 1.0f - saturate(dot(rViewDir, v.normal.xyz));
			rim = smoothstep(__rimMinVert, __rimMaxVert, rim);
			output.rim = rim;
		}

		//================================================================

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;

			Input input;
			
			// Shader Properties
			float __stylizedThreshold;
			float __stylizedThresholdScale;
			float __rampThreshold;
			float __rampThresholdOtherLights;
			float3 __highlightColor;
			float3 __shadowColor;
			float __occlusion;
			float __ambientIntensity;
			float __specularSmoothnessPbr;
			float3 __specularColor;
			float3 __rimColor;
			float __rimStrength;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			// Shader Properties Sampling
			float4 __albedo = ( tex2D(_MainTex, input.texcoord0.xy).rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			output.__stylizedThreshold = ( tex2D(_StylizedThreshold, input.texcoord0.xy * _StylizedThreshold_ST.xy + _StylizedThreshold_ST.zw).a );
			output.__stylizedThresholdScale = ( 1.0 );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampThresholdOtherLights = ( _RampThresholdOtherLights );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__occlusion = ( __albedo.a );
			output.__ambientIntensity = ( 1.0 );
			output.__specularSmoothnessPbr = ( _SpecularSmoothnessPBR );
			output.__specularColor = ( _SpecularColor.rgb );
			output.__rimColor = ( _RimColor.rgb );
			output.__rimStrength = ( 1.0 );

			output.input = input;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			output.Albedo *= __mainColor.rgb;
		}

		//================================================================
		// LIGHTING FUNCTION

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, half3 viewDir, UnityGI gi)
		{
			half3 lightDir = gi.light.dir;
			#if defined(UNITY_PASS_FORWARDBASE)
				half3 lightColor = _LightColor0.rgb;
				half atten = surface.atten;
			#else
				//extract attenuation from point/spot lights
				half3 lightColor = _LightColor0.rgb;
				half atten = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b)) / max(_LightColor0.r, max(_LightColor0.g, _LightColor0.b));
			#endif

			half3 normal = normalize(surface.Normal);
			half ndl = dot(normal, lightDir);
			float stylizedThreshold = surface.__stylizedThreshold;
			stylizedThreshold -= 0.5;
			stylizedThreshold *= surface.__stylizedThresholdScale;
			ndl += stylizedThreshold;
			half3 ramp;
			#if defined(UNITY_PASS_FORWARDBASE)
				#define		RAMP_THRESHOLD	surface.__rampThreshold
			#else
				#define		RAMP_THRESHOLD	surface.__rampThresholdOtherLights
			#endif
			ndl = saturate(ndl);
			ramp = step(RAMP_THRESHOLD, ndl);
			half3 rampGrayscale = ramp;

			//Apply attenuation (shadowmaps & point/spot lights attenuation)
			ramp *= atten;

			//Highlight/Shadow Colors
			#if !defined(UNITY_PASS_FORWARDBASE)
				ramp = lerp(half3(0,0,0), surface.__highlightColor, ramp);
			#else
				ramp = lerp(surface.__shadowColor, surface.__highlightColor, ramp);
			#endif

			//Output color
			half4 color;
			color.rgb = surface.Albedo * lightColor.rgb * ramp;
			color.a = surface.Alpha;

			// Apply indirect lighting (ambient)
			half occlusion = surface.__occlusion;
			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				half3 ambient = gi.indirect.diffuse;
				ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;

				color.rgb += ambient;
			#endif
			
			//Specular: GGX
			half3 halfDir = SafeNormalize(lightDir + viewDir);
			half roughness = surface.__specularSmoothnessPbr*surface.__specularSmoothnessPbr;
			half nh = saturate(dot(normal, halfDir));
			half spec = GGX(nh, saturate(roughness));
			spec *= TCP2_PI * 0.05;
			#ifdef UNITY_COLORSPACE_GAMMA
				spec = max(0, sqrt(max(1e-4h, spec)));
				half surfaceReduction = 1.0 - 0.28 * roughness * surface.__specularSmoothnessPbr;
			#else
				half surfaceReduction = 1.0 / (roughness*roughness + 1.0);
			#endif
			spec = max(0, spec * ndl);
			spec *= surfaceReduction;
			
			//Apply specular
			color.rgb += spec * lightColor.rgb * surface.__specularColor;
			// Rim Lighting
			half rim = surface.input.rim;
			half3 rimColor = surface.__rimColor;
			half rimStrength = surface.__rimStrength;
			//Rim light mask
			color.rgb += ndl * atten * rim * rimColor * rimStrength;
			return color;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
		{
			half3 normal = surface.Normal;

			//GI without reflection probes
			gi = UnityGlobalIllumination(data, 1.0, normal); // occlusion is applied in the lighting function, if necessary

			surface.atten = data.atten; // transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb; // remove attenuation
		}

		ENDCG

	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(ver:"2.4.2";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","RAMP_MAIN_OTHER","RAMP_SEPARATED","SPECULAR","SPECULAR_NO_ATTEN","SPEC_PBR_GGX","RIM","RIM_DIR","RIM_DIR_PERSP_CORRECTION","RIM_VERTEX","RIM_LIGHTMASK","MATCAP_PERSPECTIVE_CORRECTION","OCCLUSION","OUTLINE","OUTLINE_BEHIND_DEPTH","OUTLINE_LIGHTING_FRAG","OUTLINE_LIGHTING","SKETCH_AMBIENT","TEXTURED_THRESHOLD","ENABLE_LIGHTMAPS","CRISP_RAMP_NO_AA"];flags:list["addshadow"];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.5",RIM_LABEL="Rim Lighting"];shaderProperties:list[];customTextures:list[]) */
/* TCP_HASH 8a280424651cbb70439e7cdb11b906c5 */
