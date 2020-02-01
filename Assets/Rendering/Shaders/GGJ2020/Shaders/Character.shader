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
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Specular)]
		[TCP2ColorNoAlpha] _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_SpecularToonSize ("Toon Size", Range(0,1)) = 0.25
		_SpecularToonSmoothness ("Toon Smoothness", Range(0.001,0.5)) = 0.05
		[TCP2Separator]
		
		[TCP2HeaderHelp(Rim Lighting)]
		[TCP2ColorNoAlpha] _RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.5)
		_RimMin ("Rim Min", Range(0,2)) = 0.5
		_RimMax ("Rim Max", Range(0,2)) = 1
		//Rim Direction
		_RimDir ("Rim Direction", Vector) = (0,0,1,1)
		[TCP2Separator]

		[TCP2HeaderHelp(Reflections)]
		[TCP2ColorNoAlpha] _ReflectionColor ("Color", Color) = (1,1,1,1)
		_ReflectionSmoothness ("Smoothness", Range(0,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Subsurface Scattering)]
		_SubsurfaceDistortion ("Distortion", Range(0,2)) = 0.2
		_SubsurfacePower ("Power", Range(0.1,16)) = 3
		_SubsurfaceScale ("Scale", Float) = 1
		[TCP2ColorNoAlpha] _SubsurfaceColor ("Color", Color) = (0.5,0.5,0.5,1)
		[TCP2ColorNoAlpha] _SubsurfaceAmbientColor ("Ambient Color", Color) = (0.5,0.5,0.5,1)
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

			float3 objSpaceLight = normalize(mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz);
			float3 normal = objSpaceLight.xyz;
			output.pack2.xyz = normalize(mul(unity_ObjectToWorld, v.normal).xyz);
		
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
		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _Color;
		float _RampThreshold;
		fixed3 _HColor;
		fixed3 _SColor;
		float _SpecularToonSize;
		float _SpecularToonSmoothness;
		fixed3 _SpecularColor;
		float3 _RimDir;
		float _RimMin;
		float _RimMax;
		fixed3 _RimColor;
		float _SubsurfaceDistortion;
		float _SubsurfacePower;
		float _SubsurfaceScale;
		fixed3 _SubsurfaceColor;
		fixed3 _SubsurfaceAmbientColor;
		fixed3 _ReflectionColor;
		float _ReflectionSmoothness;

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
			float3 worldPos;
			float4 screenPosition;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

			float4 clipPos = UnityObjectToClipPos(v.vertex);

			//Screen Position
			float4 screenPos = ComputeScreenPos(clipPos);
			output.screenPosition = screenPos;

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
			float __rampThreshold;
			float3 __highlightColor;
			float3 __shadowColor;
			float __occlusion;
			float __ambientIntensity;
			float __specularToonSize;
			float __specularToonSmoothness;
			float3 __specularColor;
			float3 __rimDir;
			float __rimMin;
			float __rimMax;
			float3 __rimColor;
			float __rimStrength;
			float __subsurfaceDistortion;
			float __subsurfacePower;
			float __subsurfaceScale;
			float3 __subsurfaceColor;
			float3 __subsurfaceAmbientColor;
			float __subsurfaceThickness;
			float3 __reflectionColor;
			float __reflectionSmoothness;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			// Shader Properties Sampling
			float4 __albedo = ( tex2D(_MainTex, input.texcoord0.xy).rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			output.__rampThreshold = ( _RampThreshold );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__occlusion = ( __albedo.a );
			output.__ambientIntensity = ( 1.0 );
			output.__specularToonSize = ( _SpecularToonSize );
			output.__specularToonSmoothness = ( _SpecularToonSmoothness );
			output.__specularColor = ( _SpecularColor.rgb );
			output.__rimDir = ( _RimDir.xyz );
			output.__rimMin = ( _RimMin );
			output.__rimMax = ( _RimMax );
			output.__rimColor = ( _RimColor.rgb );
			output.__rimStrength = ( 1.0 );
			output.__subsurfaceDistortion = ( _SubsurfaceDistortion );
			output.__subsurfacePower = ( _SubsurfacePower );
			output.__subsurfaceScale = ( _SubsurfaceScale );
			output.__subsurfaceColor = ( _SubsurfaceColor.rgb );
			output.__subsurfaceAmbientColor = ( _SubsurfaceAmbientColor.rgb );
			output.__subsurfaceThickness = ( 1.0 );
			output.__reflectionColor = ( _ReflectionColor.rgb );
			output.__reflectionSmoothness = ( _ReflectionSmoothness );

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
			half3 ramp;
			#define		RAMP_THRESHOLD	surface.__rampThreshold
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
			
			//Blinn-Phong Specular
			half3 h = normalize(lightDir + viewDir);
			float ndh = max(0, dot (normal, h));
			float spec = smoothstep(surface.__specularToonSize + surface.__specularToonSmoothness, surface.__specularToonSize - surface.__specularToonSmoothness,1 - (ndh / (1+surface.__specularToonSmoothness)));
			spec *= ndl;
			
			//Apply specular
			color.rgb += spec * lightColor.rgb * surface.__specularColor;
			// Rim Lighting
			half3 rViewDir = viewDir;
			half3 rimDir = surface.__rimDir;
			half3 screenPosOffset = (surface.input.screenPosition.xyz / surface.input.screenPosition.w) - 0.5;
			rimDir.xyz -= screenPosOffset.xyz;
			rViewDir = normalize(UNITY_MATRIX_V[0].xyz * rimDir.x + UNITY_MATRIX_V[1].xyz * rimDir.y + UNITY_MATRIX_V[2].xyz * rimDir.z);
			half rim = 1.0f - saturate(dot(rViewDir, normal));
			half rimMin = surface.__rimMin;
			half rimMax = surface.__rimMax;
			rim = smoothstep(rimMin, rimMax, rim);
			half3 rimColor = surface.__rimColor;
			half rimStrength = surface.__rimStrength;
			//Rim light mask
			color.rgb += ndl * atten * rim * rimColor * rimStrength;
			
				//Subsurface Scattering
			#if (POINT || SPOT)
				half3 ssLight = lightDir + normal * surface.__subsurfaceDistortion;
				half ssDot = pow(saturate(dot(viewDir, -ssLight)), surface.__subsurfacePower) * surface.__subsurfaceScale;
				half3 ssColor = ((ssDot * surface.__subsurfaceColor) + surface.__subsurfaceAmbientColor) * surface.__subsurfaceThickness;
			#if !defined(UNITY_PASS_FORWARDBASE)
				ssColor *= atten;
			#endif
				ssColor *= lightColor;
				color.rgb *= surface.Albedo * ssColor;
			#endif
			// ForwardBase pass only
			#if defined(UNITY_PASS_FORWARDBASE)

			//Reflection probes/skybox
			half3 reflections = gi.indirect.specular * occlusion * surface.__reflectionColor;
			color.rgb += reflections;
			#endif
			return color;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
		{
			half3 normal = surface.Normal;

			//GI with reflection probes support
			half smoothness = surface.__reflectionSmoothness;
			Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(smoothness, data.worldViewDir, normal, half3(0,0,0));	// last parameter is actually unused
			gi = UnityGlobalIllumination(data, 1.0, normal, g); // occlusion is applied in the lighting function, if necessary

			surface.atten = data.atten; // transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb; // remove attenuation
		}

		ENDCG

	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(ver:"2.4.2";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","SPECULAR","SPECULAR_NO_ATTEN","RIM","RIM_DIR_PERSP_CORRECTION","RIM_LIGHTMASK","MATCAP_PERSPECTIVE_CORRECTION","OCCLUSION","OUTLINE","OUTLINE_BEHIND_DEPTH","OUTLINE_LIGHTING_FRAG","OUTLINE_LIGHTING","SKETCH_AMBIENT","ENABLE_LIGHTMAPS","CRISP_RAMP_NO_AA","SPEC_LEGACY","SPECULAR_TOON","OUTLINE_FAKE_RIM_DIRLIGHT","TT_SHADER_FEATURE","RIM_DIR","GLOSSY_REFLECTIONS","SUBSURFACE_SCATTERING","SS_MULTIPLICATIVE","SUBSURFACE_AMB_COLOR"];flags:list["addshadow"];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.5",RIM_LABEL="Rim Lighting"];shaderProperties:list[];customTextures:list[]) */
/* TCP_HASH 153000ceba818fc2808638370da7134a */
