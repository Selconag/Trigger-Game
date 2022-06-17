// Some part of this code protected author listen, while you cannot copy modify oir resale some parts of this shader, you have no limits to use this shader in any your project


// Upgrade NOTE: replaced 'defined NEED_DTHR' with 'defined (NEED_DTHR)'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Hidden/Fast PostProcessing Shader Source" {

	Properties
	{
		_MainTex("_MainTex", any) = "" {}

		_DetailTexture("_DetailTexture", any) = "defaulttexture" {}

		_LUT1("_LUT1", 2D) = "defaulttexture" {}
		_LUT1_GRAD("_LUT1_GRAD", 2D) = "defaulttexture" {}
		//_LUT1_params("_LUT1_params", Vector) = (0,0,0,0)
		_LUT1_amount("_LUT1_amount", Float) = 1.0

		_LUT1_gradient_smooth("_LUT1_gradient_smooth", Float) = 0.2

		_LUT2("_LUT2", 2D) = "defaulttexture" {}
		//_LUT2_params("_LUT2_params", Vector) = (0,0,0,0)
		_LUT2_amount("_LUT2_amount", Float) = 0.5

		_lutsLevelClipMaxValue("_lutsLevelClipMaxValue", Float) = 1

		_b_brightAmount("_b_brightAmount", Float) = 1.0
		_b_colorsAmount("_b_colorsAmount", Vector) = (1, 1, 1, 1)
		_b_saturate01("_b_saturate01", Float) = 1.0
		_b_contrastAmount("_b_contrastAmount", Float) = 1.0


		
		_posterizationStepsAmount("_posterizationStepsAmount", Vector) = (5, 5, 5, 1)
		_posterizationDither("_posterizationDither", Vector) = (1, 1, 1, 0)
		_posterizationOffsetZeroPoint("_posterizationOffsetZeroPoint", Float) = 0

			
		_FinalGradientTexture("_FinalGradientTexture", 2D) = "defaulttexture" {}
		//_FinalGradientTest("_FinalGradientTest", 2D) = "defaulttexture" {}
		_DitherTexture("_DitherTexture", 2D) = "defaulttexture" {}
		_1_GradiendBrightness("_1_GradiendBrightness", Float) = 1.0
		_1_GradiendOffset("_1_GradiendOffset", Float) = 3.0
		_M_GradiendBrightness("_M_GradiendBrightness", Float) = 0.5
		_M_GradiendOffset("_M_GradiendOffset", Float) = 1.5
		// 1 bit
		_BayerMultyply("_BayerMultyply", Float) = 1.0
		// calc
		//_GradiendDitherSize("_GradiendDitherSize", Float) = 0.5
		// tex
		// calc + tex
		_DitherTextureSize("_DitherTextureSize", Float) = 1.5
		
			

		_MotionBlurMoveAmount("_MotionBlurMoveAmount", Float) = 2.2
		_MotionBlurRotateAmount("_MotionBlurRotateAmount", Float) = 2.2
		_MotionBlurPhaseShift("_MotionBlurPhaseShift", Float) = 0.0
			

		_glowContrast("_glowContrast", Float) = 2.2
		_glowBrightness("_glowBrightness", Float) = 1.0
		_glowTreshold("_glowTreshold", Float) = 0.67

		_outlineStrokesColor("_outlineStrokesColor", Color) = (0.3, 0.6, 0.9,1)
		_outlineStrokesScale("_outlineStrokesScale", Float) = 0.3
		_outlineStrokesBlend("_outlineStrokesBlend", Float) = 1.0
		_outlineStrokesDetection("_outlineStrokesDetection", Float) = 15.0


		_aoRadius("_aoRadius", Float) = 1.0
		_aoAmount("_aoAmount", Float) = 1.0
		_aoBlend("_aoBlend", Float) = 1.0
		_aoThreshold("_aoThreshold", Float) = 0.005//0.0001 0.5
		_aoGroundBias("_aoGroundBias", Float) = 1//0.0001 0.5
		_aoStrokesSpread("_aoStrokesSpread", Float) = 2.0
		_aoDistanceFixAmount("_aoDistanceFixAmount", Float) = 90
		_aoNormalsBias("_aoNormalsBias", Float) = 0.1

		
		_patTintAffect("_patTintAffect", Float) = -0.2
		_patUvAffect("_patUvAffect", Float) = 0.2
		_patSize("_patSize", Float) = 2
		_patPixelization("_patPixelization", Float) = 0.2
		_PatternTexture("_PatternTexture", 2D) = "defaulttexture" {}


		_FogColor("_FogColor", Color) = (0.5748665, 0.7498095, 0.8301887, 1)
		_StartZ("_StartZ", Float) = 10
		_EndZ("_EndZ", Float) = 40
		_StartY("_StartY", Float) = 5.0
		_EndY("_EndY", Float) = -1.0
		_ZFogDensity("_ZFogDensity", Float) = 1.0
		_HeightFogDensity("_HeightFogDensity", Float) = 1.0
		_FogParams("_FogParams", Vector) = (0, 0, 0, 0)
		_NoiseTexture("_NoiseTexture", any) = "defaulttexture" {}
		_FogTexAnimateSpeed("_FogTexAnimateSpeed", Float) = 6.0
		_FogTexTile("_FogTexTile", Vector) = (40, 40, 0, 0)
		_SKIP_FOG_FOR_BACKGROUND("_SKIP_FOG_FOR_BACKGROUND", Float) = 0.0
		_USE_HDR_FOR_FOG("_USE_HDR_FOR_FOG", Float) = 1.0
			

		//_darkBorders("_darkBorders", Float) = 1.0
		_sharpenAmounta("_sharpenAmounta", Float) = 6.0
		_sharpenAmountb("_sharpenAmountb", Float) = 6.0
		_sharpenSizea("_sharpenSizea", Float) = 2.0
		_sharpenSizeb("_sharpenSizeb", Float) = 1.0
		_sharpenDarkPoint_Fast("_sharpenDarkPoint_Fast", Float) = 0.5
		_sharpenDarkPoint("_sharpenDarkPoint", Float) = 0.7
		//_sharpenBottomBright_Fast("_sharpenBottomBright_Fast", Float) = 1.0
		//_sharpenBottomBright_Improved("_sharpenBottomBright_Improved", Float) = 1.0
		_sharpenLerp_Improved("_sharpenLerp_Improved", Float) = 0.7
			

	

		_glowSamples_BlurRadius("_glowSamples_BlurRadius", Vector) = (5, 1, 5, 1)
		_blurSamples_BlurRadius("_blurSamples_BlurRadius", Vector) = (50, 10, 5, 1)
		_glowSamples_DitherSize("_glowSamples_DitherSize", Float) = 0.5
		_blurSamples_DitherSize("_blurSamples_DitherSize", Float) = 0.5


		_dof_zoff_borders("_dof_zoff_borders", Vector) = (0.12, -0.3, 0.45 , -0.3)
		_dof_zoff_feather("_dof_zoff_feather", Float) = 0.5
		_dof_z_distance("_dof_z_distance", Float) = 2.5
		_dof_z_aperture("_dof_z_aperture", Float) = 2.5


		_spread_AnimateSpeedMulti("_spread_AnimateSpeedMulti", Float) = 1.0
		_spread_Variatnt("_spread_Variatnt", Float) =  20.58// 20.58

		
	}



		SubShader
		{
			Pass
			{
				ZTest Always Cull Off ZWrite Off
				CGPROGRAM
			//COLORS
				#pragma shader_feature_local __ USE_LUT0 USE_LUT1 USE_LUT2
				#pragma shader_feature_local __ SKIP_LUTS_FOR_BRIGHT_AREAS
				#pragma shader_feature_local __ USE_HDR_COLORS
				#pragma shader_feature_local __ USE_BRIGHTNESS_FUNCTIONS
				#pragma shader_feature_local __ USE_POSTERIZE_LUTS USE_POSTERIZE_IMPROVED USE_POSTERIZE_SIMPLE 
				#pragma shader_feature_local __ USE_ULTRA_FAST_SHARPEN USE_IMPROVED_SHARPEN
				#pragma shader_feature_local __ USE_POSTERIZE_DITHER
				#pragma shader_feature_local __ USE_WRAPS
				#pragma shader_feature_local __ APPLY_CUSTOM_GRADIENT_FOR_LUT0



			//GRADIENTS
				#pragma shader_feature_local __ USE_FINAL_GRADIENT_DITHER_ONE_BIT USE_FINAL_GRADIENT_DITHER_MATH USE_FINAL_GRADIENT_DITHER_TEX USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX
				#pragma shader_feature_local __ GRADIENT_RAMP 
				#pragma shader_feature_local __ GRADIENT_COLOR_ADD GRADIENT_COLOR_MULTY 
				#pragma shader_feature_local __ LUTS_AMONT_MORE_THAN_1 
			
			
			  
			
	
			//OTHER
				#pragma shader_feature_local __ USE_OUTLINE_STROKES USE_OUTLINE_COLORED_STROKES_NORMAL  USE_OUTLINE_COLORED_STROKES_ADD USE_OUTLINE_STROKES_INVERTED
				#pragma shader_feature_local __ APPLY_DITHER_TO_STROKE
				#pragma shader_feature_local __ APPLY_DEPTH_CORRECTION_TO_STROKE

				#pragma shader_feature_local __ USE_SIMPLE_SSAO_3 USE_SIMPLE_SSAO_4 USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION
				#pragma shader_feature_local __ APPLY_SSAO_AFTER_EFFECTS

			//FOG
				#pragma shader_feature_local __ USE_SHADER_DISTANCE_FOG USE_SHADER_DISTANCE_FOG_WITH_TEX USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
				#pragma shader_feature_local __ USE_GLOBAL_HEIGHT_FOG USE_GLOBAL_HEIGHT_FOG_WITH_TEX USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
				#pragma shader_feature_local __ USE_FOG_ADDITIVE_BLENDING
				#pragma shader_feature_local __ FOG_BEFORE_LUT
			
			
				#pragma shader_feature_local __ APPLY_STROKES_AFTER_FOG

				
			//GLOW
				//#pragma shader_feature_local __ USE_GLOW_1 USE_GLOW_4 USE_GLOW_8 USE_GLOW_16 
				//#pragma shader_feature_local __ USE_GLOW_BLEACH_STYLE
				//#pragma shader_feature_local __ USE_GLOW_SAMPLE_DITHER
				//#pragma shader_feature_local __ USE_Z_FOR_BLURING

				#pragma shader_feature_local __ USE_GLOW_MOD
				#pragma shader_feature_local __ GLOW_SAMPLES_SIZE_ON_Z //GLOW_SAMPLES_BASE_BLUR_IN_FRAG_SIZE_ON_Z
				#pragma shader_feature_local __ GLOW_SAMPLES_4 GLOW_SAMPLES_8 GLOW_SAMPLES_16 //GLOW_SAMPLES_1
				//#pragma shader_feature_local __ USE_GLOW_SAMPLE_DITHER

				#pragma shader_feature_local __ GLOW_DIRECTION_BI GLOW_DIRECTION_X GLOW_DIRECTION_Y
				#pragma shader_feature_local __ USE_GLOW_BLEACH_STYLE

			//BLUR
				//#pragma shader_feature_local __ USE_VINJETE_BLUR USE_DEPTH_OF_FIELD_SIMPLE USE_DEPTH_OF_FIELD_COMPLETE_4 USE_DEPTH_OF_FIELD_COMPLETE_8 USE_DEPTH_OF_FIELD_COMPLETE_16
				#pragma shader_feature_local __ USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z
				#pragma shader_feature_local __ BLUR_SAMPLES_4 BLUR_SAMPLES_8 BLUR_SAMPLES_16
				

				#pragma shader_feature_local __ F_MOTION_BLUR_FAST F_MOTION_BLUR_TEMPERARY



				#pragma shader_feature_local __ USE_ALPHA_OUTPUT
				

				#pragma target 2.0
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				#pragma fragmentoption ARB_precision_hint_fastest


#if USE_SIMPLE_SSAO_3 || USE_SIMPLE_SSAO_4  || USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION  || USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION
#define USE_SSAO 1
#endif

#if USE_OUTLINE_STROKES || USE_OUTLINE_COLORED_STROKES_NORMAL || USE_OUTLINE_COLORED_STROKES_ADD|| USE_OUTLINE_STROKES_INVERTED
#define USE_STOKE_DEFINED 1
#endif


#define MY_HALF4 half4
#define MY_HALF3 half3
#define MY_HALF2 half2
#define MY_HALF half
/*
#define MY_FIXED4 fixed4
#define MY_FIXED3 fixed3
#define MY_FIXED2 fixed2
#define MY_FIXED fixed
*/
#define MY_FIXED4 float4
#define MY_FIXED3 float3
#define MY_FIXED2 float2
#define MY_FIXED float


		UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
		//UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex);
		//UNITY_DECLARE_SCREENSPACE_TEXTURE(_DetailTexture);
		uniform MY_FIXED4 _MainTex_ST;
		uniform MY_FIXED4 _MainTex_TexelSize;
		uniform MY_FIXED4 _MainTex_AbsTexelSize;
		uniform MY_HALF4 _Camera_Size;
		uniform MY_FIXED4 _Camera_Forward;
		uniform MY_FIXED4 _Camera_Converter;
		uniform MY_FIXED4 _DepthCorrection;
		uniform MY_FIXED4 _Texel_Size;
		uniform MY_HALF4 _Fixed_Time;
		uniform MY_HALF4 _Fog_Time;

		


		//UNITY_DECLARE_TEX2D_HALF(_LUT1);
		//TEXTURE2D(_LUT1);
#if !USE_LUT0
#if APPLY_CUSTOM_GRADIENT_FOR_LUT0
			UNITY_DECLARE_TEX2D(_LUT1_GRAD);
#else
			UNITY_DECLARE_TEX2D(_LUT1);
#endif
		uniform MY_FIXED4 _LUT1_params;
		uniform MY_FIXED _LUT1_amount;
#endif
#if USE_LUT2
		UNITY_DECLARE_TEX2D_NOSAMPLER(_LUT2);
		uniform MY_FIXED4 _LUT2_params;
		uniform MY_FIXED _LUT2_amount;
#endif
#if SKIP_LUTS_FOR_BRIGHT_AREAS
		uniform MY_FIXED _lutsLevelClipMaxValue;
#endif

#if USE_BRIGHTNESS_FUNCTIONS
		uniform MY_FIXED3 _b_brightAmountVector;
		uniform MY_FIXED _b_saturate01Value;
		uniform MY_FIXED _b_saturate01;
		uniform MY_FIXED _b_contrastAmount;
		static const MY_FIXED3 LuminaceCoeff = MY_FIXED3(0.2125, 0.7153, 0.1721);
		static const MY_FIXED3 AvgLimin = MY_FIXED3(0.5, 0.5, 0.5);
#endif

#if USE_POSTERIZE_LUTS || USE_POSTERIZE_IMPROVED || USE_POSTERIZE_SIMPLE
		uniform MY_FIXED4 _posterizationStepsAmount;
		uniform MY_FIXED4 _posterizationDither;
		uniform MY_FIXED _posterizationOffsetZeroPoint;
#endif

#if USE_FINAL_GRADIENT_DITHER_ONE_BIT || USE_FINAL_GRADIENT_DITHER_MATH || USE_FINAL_GRADIENT_DITHER_TEX || USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX
#define USE_FINAL_GRADIENT 1
#endif
#if defined(USE_FINAL_GRADIENT)
		UNITY_DECLARE_TEX2D(_FinalGradientTexture);
		uniform MY_FIXED4 _FinalGradientTexture_params;
		
		uniform MY_FIXED _1_GradiendBrightness;
		uniform MY_FIXED _1_GradiendOffset;
		uniform MY_FIXED _M_GradiendBrightness;
		uniform MY_FIXED _M_GradiendOffset;
		uniform MY_FIXED _BayerMultyply;
		uniform MY_FIXED _DitherTextureSize;


		uniform fixed4x4 bayer_matrix;


		
#if USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX || USE_FINAL_GRADIENT_DITHER_MATH
#if !defined(NEED_DTHR)
#define NEED_DTHR 1
#endif
#endif


#if USE_FINAL_GRADIENT_DITHER_TEX || USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX
		UNITY_DECLARE_TEX2D(_DitherTexture);
#endif

#endif




#if defined(USE_STOKE_DEFINED)
		uniform MY_FIXED _outlineStrokesBlend;
		uniform MY_FIXED _outlineStrokesDetection;
		uniform MY_FIXED _outlineStrokesScale;
		uniform MY_FIXED4 _outlineStrokesColor;
#endif



#if USE_WRAPS
		uniform MY_FIXED _patTintAffect = 0;// = 0.5;
		uniform MY_FIXED _patUvAffect = 1;// = 0.5;
		uniform MY_FIXED _patSize = 1;// = 0.5;
		uniform MY_FIXED _patPixelization = 0;// = 0.5;
		UNITY_DECLARE_TEX2D(_PatternTexture);
#endif
		//UNITY_DECLARE_TEX2D(_PatternTexture);
		//UNITY_DECLARE_TEX2D_NOSAMPLER(_NoiseTexture);



#if defined(USE_SSAO)
		uniform MY_FIXED _aoRadius;// = 0.5;
		uniform MY_FIXED _aoAmount;// = 2;
		uniform MY_FIXED _aoBlend;// = 2;
		uniform MY_FIXED _aoThreshold;// = 0.1; //0.0001 0.5
		uniform MY_FIXED _aoGroundBias;// = 1; //0.0001 0.5
		uniform MY_FIXED _aoNormalsBias;// = 1; //0.0001 0.5
		uniform MY_FIXED _aoStrokesSpread;// = 1;
		uniform MY_FIXED _aoDistanceFixAmount;// = 1;
#endif

#if USE_SHADER_DISTANCE_FOG || USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
		//UNITY_DECLARE_SCREENSPACE_TEXTURE(_NoiseTexture);
		//UNITY_DECLARE_TEX2D_NOSAMPLER(_NoiseTexture);
		UNITY_DECLARE_TEX2D(_NoiseTexture);
		
#endif
		uniform MY_FIXED2 _FogTexTile;
		uniform MY_FIXED _FogTexAnimateSpeed;
		
		uniform MY_FIXED _USE_HDR_FOR_FOG;
		uniform MY_FIXED _SKIP_FOG_FOR_BACKGROUND;
		uniform MY_FIXED4 _FogParams;

		
		uniform MY_FIXED _HeightFogDensity;
		uniform MY_FIXED _ZFogDensity;
		uniform MY_FIXED4 _FogColor;
		uniform MY_FIXED _StartZ;
		uniform MY_FIXED _EndZ;
		uniform MY_FIXED _StartY;
		uniform MY_FIXED _EndY;
#endif


#if USE_ULTRA_FAST_SHARPEN || USE_IMPROVED_SHARPEN
		//uniform MY_FIXED _darkBorders;// = 1.0;
		uniform MY_FIXED _sharpenAmounta;// = 6.0;
		uniform MY_FIXED _sharpenSizea;// = 2.0;
		uniform MY_FIXED _sharpenAmountb;// = 6.0;
		uniform MY_FIXED _sharpenSizeb;// = 2.0;
		uniform MY_FIXED _sharpenDarkPoint_Fast;// = 1.0;
		uniform MY_FIXED _sharpenDarkPoint;// = 1.0;
		//uniform MY_FIXED _sharpenBottomBright_Fast;// = 0.5;
		//uniform MY_FIXED _sharpenBottomBright_Improved;// = 0.0;
		uniform MY_FIXED _sharpenLerp_Improved;// = 0.0;
#endif

#if F_MOTION_BLUR_FAST || F_MOTION_BLUR_TEMPERARY
		uniform MY_FIXED _MotionBlurMoveAmount;
		uniform MY_FIXED _MotionBlurPhaseShift;
		uniform MY_FIXED _MotionBlurRotateAmount;
		uniform MY_FIXED4 _CameraVelocity;
		uniform MY_FIXED4 _CameraDeepVel;
		uniform MY_FIXED4 _CameraStrafeVel;
		#define USE_MOTION_BLUR 1
#endif

		
/////////FIXED FOR Off


#if !GLOW_SAMPLES_4 && !GLOW_SAMPLES_8 && !GLOW_SAMPLES_16
#define GLOW_SAMPLES_1 1
#endif

//#if !GLOW_DIRECTION_X && !GLOW_DIRECTION_Y
//#define GLOW_DIRECTION_XY 1
//#endif

#if !BLUR_SAMPLES_4 && !BLUR_SAMPLES_8 && !BLUR_SAMPLES_16
#define BLUR_SAMPLES_1 1
#endif
  
/////////



//#if GLOW_SAMPLES_BASE_BLUR_IN_VERT_SIZE_OFF_Z || GLOW_SAMPLES_BASE_BLUR_IN_FRAG_SIZE_ON_Z 
//GLOW_DIRECTION_XY GLOW_DIRECTION_X GLOW_DIRECTION_Y
//GLOW_SAMPLES_1 GLOW_SAMPLES_4 GLOW_SAMPLES_8 GLOW_SAMPLES_16
#if USE_GLOW_MOD || USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z || USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z




#if !GLOW_SAMPLES_SIZE_ON_Z // if
#define USE_GLOW_SAMPLES_OFF_Z 1
#else // else
#define USE_GLOW_SAMPLES_ON_Z 1
#if !defined(NEED_MAIN_Z)
#define NEED_MAIN_Z 1
#endif
#endif // endif

uniform MY_FIXED _glowContrast;
uniform MY_FIXED _glowBrightness;
uniform MY_FIXED _glowTreshold;
#endif



//USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z
//BLUR_SAMPLES_1 BLUR_SAMPLES_4 BLUR_SAMPLES_8 BLUR_SAMPLES_16
#if USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z
	uniform MY_FIXED4 _dof_zoff_borders;
	uniform MY_FIXED _dof_zoff_feather;
#endif
#if USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z || USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z
#if !defined(NEED_MAIN_Z)
#define NEED_MAIN_Z 1
#endif
	uniform MY_FIXED _dof_z_distance;
	uniform MY_FIXED _dof_z_aperture;
#endif
#if USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z
#define USE_BLUR_SAMPLE 1
#endif





#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) 
	uniform MY_FIXED2 _glowSamples_BlurRadius;
	//#if USE_GLOW_SAMPLE_DITHER
	//#if !defined(NEED_DTHR)
	//#define NEED_DTHR 1
	//#endif
	uniform MY_FIXED _glowSamples_DitherSize;
	//#endif
	#if !defined(NEED_DTHR)
	#define NEED_DTHR 1
	#endif
#endif

#if defined(USE_BLUR_SAMPLE)
	uniform MY_FIXED2 _blurSamples_BlurRadius;
//#if USE_BLUR_SAMPLE_DITHER
//	#if !defined(NEED_DTHR)
//	#define NEED_DTHR 1
//#endif
	uniform MY_FIXED _blurSamples_DitherSize;
	//#endif
	#if !defined(NEED_DTHR)
	#define NEED_DTHR 1
	#endif
#endif





#if defined(USE_MOTION_BLUR)
	#if !defined(NEED_DTHR)
	#define NEED_DTHR 1
	#endif
#endif






#if USE_POSTERIZE_LUTS || USE_POSTERIZE_IMPROVED || USE_POSTERIZE_SIMPLE
#define USE_POSTERIZE 1
#endif


#if USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG
#define NEED_WP 1
#endif

#if defined(NEED_WP) || defined(USE_SSAO) || defined(USE_STOKE_DEFINED)
#if !defined(NEED_MAIN_Z)
#define NEED_MAIN_Z 1
#endif
#endif

#if defined(USE_SSAO) || USE_SIMPLE_SSAO_DISTANCE_CORRECTION || defined(USE_POSTERIZE) && USE_POSTERIZE_DITHER  || defined(USE_STOKE_DEFINED) && APPLY_DITHER_TO_STROKE
#if !defined(NEED_DTHR)
#define NEED_DTHR 1
#endif
#endif






#if defined (NEED_DTHR)
		uniform MY_FIXED _spread_AnimateSpeedMulti;// = 1;
		uniform MY_FIXED _spread_Variatnt;// = 1;
		
#endif
#if defined(NEED_MAIN_Z) // || USE_OUTLINE_STROKES || USE_OUTLINE_COLORED_STROKES
		UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
#endif





		struct appdata_t {
			MY_FIXED4 vertex : POSITION;
			MY_FIXED2 uv : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};//appdata_t

		struct v2f {
			MY_FIXED4 vertex : SV_POSITION;
			MY_FIXED4 uv : TEXCOORD0;
			MY_FIXED3 cameraDir : TEXCOORD1;

#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) || defined(USE_BLUR_SAMPLE)
			MY_FIXED4 blur_a : TEXCOORD2;
#endif
			MY_FIXED4 utils_a : TEXCOORD3;
#if USE_ULTRA_FAST_SHARPEN || USE_IMPROVED_SHARPEN
			MY_FIXED4 sharp_uv : TEXCOORD4;
#endif

//#if defined(NEED_DTHR)
//			float4 _dthr_ut : TEXCOORD5;
//#endif



			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
		};//v2f


		//fixed4x4 _LeftEyeToWorld;
		//fixed4x4 _RightEyeToWorld;
		//fixed4x4 _LeftEyeProjection;
		//fixed4x4 _RightEyeProjection;
		//fixed4x4 _ClipToWorld;
		fixed4x4 _CameraDirLeft;
		fixed4x4 _CameraDirRight;
		fixed4x4 _Rotate2World;

		#define EM_TEXEL_SIZE 0.004

		MY_FIXED2 s_offset;
		v2f vert(appdata_t v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			//if (_ProjectionParams.x < 0.0) {
			//	v.uv.y = 1.0 - v.uv.y;
			//}


			_MainTex_AbsTexelSize = _MainTex_TexelSize;
			_MainTex_AbsTexelSize.y = abs(_MainTex_AbsTexelSize.y);

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv.y = 1 - o.uv.y;
#endif
			o.uv.zw = o.uv.xy * _MainTex_AbsTexelSize.zw;

			
#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) || defined(USE_BLUR_SAMPLE)
#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) 
			MY_FIXED2 _offset = _glowSamples_BlurRadius * _Texel_Size.xy * EM_TEXEL_SIZE;
#if defined(USE_GLOW_SAMPLES_ON_Z) 
			_offset /= 5;
#endif
			o.blur_a.xy = _offset.xy;


//#if defined(GLOW_SAMPLES_1)
//#elif GLOW_SAMPLES_4
//			o.blur_a.xy = offset.xy;
//#elif GLOW_SAMPLES_8
//#elif GLOW_SAMPLES_16
//#endif
	#else
			o.blur_a.xy = MY_FIXED2(0.0,0.0);
#endif
#if defined(USE_BLUR_SAMPLE)
			MY_FIXED2 _offsetb = _blurSamples_BlurRadius * _Texel_Size.xy * EM_TEXEL_SIZE;
			o.blur_a.zw = _offsetb.xy;
			#else
			o.blur_a.zw = MY_FIXED2(0.0,0.0);
#endif
#endif
			o.utils_a = MY_FIXED4(0.0,0.0,0.0,0.0);
#if defined(NEED_DTHR)
			o.utils_a.x = _Fixed_Time.z * _spread_AnimateSpeedMulti;
#endif


			o.cameraDir = _CameraDirLeft[o.uv.x + o.uv.y * 2].xyz;



			#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
//#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			//o.utils_a.zw = (_WorldSpaceCameraPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile;
//#else
			o.utils_a.zw = (_WorldSpaceCameraPos.xz) / _FogTexTile;
//#endif

			//MY_FIXED currentPos = 1 - UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture,  currentPos_uv,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			//MY_FIXED2 fog_height_d_uv = (worldPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile;
			//MY_FIXED fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, fog_height_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			//MY_FIXED fogDetailTexture2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture,-fog_height_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			//fogDetailTexture = max(fogDetailTexture, fogDetailTexture2);
			//MY_FIXED2 fog_height_d_uv2 = (_WorldSpaceCameraPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile;
					//	MY_FIXED2 fog_height_d_uv2 = i.utils_a.zw;

			//MY_FIXED2 fog_height_d_uv1 = (o.utils_a.zw - _Fog_Time.xy) ;
			//MY_FIXED2 fog_height_d_uv2 = (o.utils_a.zw + _Fog_Time.xy) ;
			//MY_FIXED currentPos = UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture,  fog_height_d_uv1,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			//MY_FIXED currentPos2 = UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture,  fog_height_d_uv2,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			//currentPos = 1 - max(currentPos, currentPos2); 
			MY_FIXED2 start_fog_uv = o.utils_a.zw;
			MY_FIXED2 fog_d_uv = (start_fog_uv +  _Fog_Time.xy) ;
			MY_FIXED2 fog_d_uv2 = (start_fog_uv - _Fog_Time.xy) ;
			MY_FIXED2 start_fog_uv_2 = start_fog_uv / 2;
			MY_FIXED2 fog_d_uv3 = (start_fog_uv_2 + _Fog_Time.zw) ;
			MY_FIXED2 fog_d_uv4 = (start_fog_uv_2 - _Fog_Time.zw) ;
			//MY_FIXED _fogDetailTexture = UNITY_SAMPLE_TEX2D_SAMPLER(_NoiseTexture, _LUT1, fog_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED _fogDetailTexture = UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture, fog_d_uv3,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED _fogDetailTexture2 = UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture,fog_d_uv4 ,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED fogDetailTexture_a = max(_fogDetailTexture, _fogDetailTexture2);
			//fogDetailTexture_a *= UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture, fog_d_uv ,0).r*1.5; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			//fogDetailTexture_a *= UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture, fog_d_uv2 ,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2

			MY_FIXED	foddgDetailTexture_a = UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture, fog_d_uv ,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			foddgDetailTexture_a += UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture, fog_d_uv2,0 ).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			fogDetailTexture_a *= foddgDetailTexture_a;
			fogDetailTexture_a = saturate(fogDetailTexture_a);

			MY_FIXED currentPos = 1 - fogDetailTexture_a; 

#else
			//MY_FIXED fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (worldPos.xz ) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			//MY_FIXED currentPos_uv = (_WorldSpaceCameraPos.xz) / _FogTexTile;
			MY_FIXED2 currentPos_uv = o.utils_a.zw;
			MY_FIXED currentPos = 1 - UNITY_SAMPLE_TEX2DARRAY_LOD(_NoiseTexture,  currentPos_uv,0).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
#endif

			o.utils_a.w =saturate( _FogParams.y * currentPos * _HeightFogDensity) ;
			
#endif

#if USE_ULTRA_FAST_SHARPEN || USE_IMPROVED_SHARPEN
#if USE_ULTRA_FAST_SHARPEN
			MY_FIXED2 sharpenSize = _sharpenSizea * _MainTex_AbsTexelSize.xy;
#endif
#if USE_IMPROVED_SHARPEN
			MY_FIXED2 sharpenSize = _sharpenSizeb * _MainTex_AbsTexelSize.xy;
#endif
			o.sharp_uv.xy =  o.uv.xy + sharpenSize;
			o.sharp_uv.zw =  o.uv.xy - sharpenSize;
#endif


//#if defined(NEED_DTHR)
//			MY_FIXED2 dr_t_uv = o.uv.zw; //1357.0
//			MY_FIXED dr_texel =  _spread_Variatnt  ;
//			MY_FIXED dr_uv_x = dr_t_uv.x * (3849.345 / 2220.0);
//			MY_FIXED dr_uv_y = dr_t_uv.y * (6486.345 / 1327.0);
//			MY_FIXED dr_sum1 = dr_uv_x + dr_uv_y;
//			MY_FIXED dthr_z =  frac((dr_sum1) * dr_texel  + o.utils_a.x); //dottt *
//			o._dthr_ut = float4(dthr_z, dr_uv_y, dr_t_uv.y, 50506.345/ 1827.0);
//#endif


			return o;
		}//vert


#if USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z || USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z
#define USE_BLUR_AND_GLOW_PASS_FOR_BLUR 1
#endif

#if defined(USE_BLUR_AND_GLOW_PASS_FOR_BLUR)
#define AT_ALPHA 0.08333
#define DT_ALPHA 0
#else
#define AT_ALPHA 0
#define DT_ALPHA 1
#endif




#if GLOW_SAMPLES_8
		static const MY_FIXED4 at = MY_FIXED4(0.08333, 0.08333, 0.08333, AT_ALPHA);
#elif GLOW_SAMPLES_16
		static const MY_FIXED4 at = MY_FIXED4(0.08333, 0.08333, 0.08333, AT_ALPHA) / 1;
#else
		static const MY_FIXED4 at = MY_FIXED4(0.33333, 0.33333, 0.33333, AT_ALPHA) / 2.5;
#endif
		static const MY_FIXED4 ft = MY_FIXED4(0.16666, 0.16666, 0.16666, AT_ALPHA) / 6;
#if GLOW_SAMPLES_16
		static const MY_FIXED4 bt = MY_FIXED4(0.16666, 0.16666, 0.16666, AT_ALPHA) / 3;
#else 
		static const MY_FIXED4 bt = MY_FIXED4(0.16666, 0.16666, 0.16666, AT_ALPHA) / 3;
#endif
		static const MY_FIXED3 lum_const = MY_FIXED3(0.3f, 0.6f, 0.1f);
		static const MY_FIXED3 d0 = MY_FIXED3(0.33333, 0.33333, 0.33333);
		static const MY_FIXED3 d1 = MY_FIXED3(1, 1, 1);
		static const MY_FIXED4 dt = MY_FIXED4(1.0, 1.0, 1.0, DT_ALPHA);
		static const MY_FIXED4 f1 = MY_FIXED4(1.0, 1.0, 1.0, 1.0);

		static const MY_FIXED2 blend_05 = MY_FIXED2(0.5, 0.5);
		static const MY_FIXED3 blend_433 = MY_FIXED3(0.4, 0.3, 0.3);
		static const MY_FIXED4 blend_3322 = MY_FIXED4(0.3, 0.3, 0.2, 0.2);
		static const MY_FIXED4 blend_33211 = MY_FIXED4(0.3, 0.3, 0.2, 0.1);

		//MY_FIXED d = SAMPLE_RAW_DEPTH_TEXTURE(_CameraDepthTexture, uvv);
	//MY_FIXED d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv).r;
//#define TTT MY_FIXED
//#define WWW (0.5
//#define QQQ  + 0.5);
//#define QQWE 1
//#define ASD()\
//return TTT WWW QQQ;

			//ASD()

		MY_FIXED4 frag(v2f i) : SV_Target
		{

			//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            //return lerp(1, 0, unity_StereoEyeIndex);
		
			UNITY_SETUP_INSTANCE_ID(i);
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

#if USE_WRAPS
			MY_FIXED2 _patUv = i.uv.xy / _patSize * 10;
			//MY_FIXED2 po =( _patPixelization - 1)* _MainTex_AbsTexelSize.zw;
			i.uv.xy = (floor((i.uv.xy * _MainTex_AbsTexelSize.zw ) * _patPixelization )  ) / _patPixelization / _MainTex_AbsTexelSize.zw;
			MY_FIXED _patTex = UNITY_SAMPLE_TEX2DARRAY_LOD(_PatternTexture, _patUv, 0).r;
			MY_FIXED _patTint = _patTex * _patTintAffect;
			i.uv.xy += _patTex * _patUvAffect / 100;
#endif

			MY_FIXED2 uvv = i.uv.xy;
			MY_FIXED4 MainTex_TexelSize = _MainTex_AbsTexelSize;

			//MY_FIXED4 asdasd = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
			//return asdasd;
#if defined(NEED_DTHR) 
			//MY_FIXED dthr = frac((uvv.x * 256.345 + uvv.y * 156.345) / MainTex_TexelSize.x + _Fixed_Time.z * _spread_AnimateSpeedMulti);

			//MY_FIXED p1 = floor(uvv.x);
			//MY_FIXED f1 = frac(uvv.y);
			//MY_FIXED p2 = floor(uvv.y)/2;
			//MY_FIXED f2 = frac(uvv.x) / 2;
			//MY_FIXED d1 = dot(MY_FIXED2(p1, f1), MY_FIXED2(11.9898, 58.233)) * 43758.5453;
			//MY_FIXED d2 =  dot(MY_FIXED2(p2, f2) / 2, MY_FIXED2(12.9898, 78.233))* 438.5453;
			//MY_FIXED asd =( frac(d1 )+ frac( d2)) ;
			//return asd;
			//MY_FIXED texel = 1 / floor(MainTex_TexelSize.z / 355);
			//MY_FIXED texel = MainTex_TexelSize.z;
			// * MainTex_TexelSize.w / MainTex_TexelSize.z
	
			//MY_FIXED dr_texel =  244.511f  ;
			//MY_FIXED dr_texel =  0.2220f  ;
			//20.58f 244.511f  90.58f 0.2220f
              //              new ADDITIONAL_PARAM( new float[]{ 90.58f, 20.58f,  /*0.4f*/  143.511f, 0.2220f, }, PID._spread_Variatnt, new[]{ "v1", "v2", "v3", "v4" } ,
			//MY_FIXED dr_texel =  244.511f  ;
			//dr_texel = 143.511f;
			//MY_FIXED dr_uv_x = dr_t_uv.x * (2056.345 / 2220.0);
			//MY_FIXED dr_uv_y = dr_t_uv.y * (1406.345 / 1327.0);
			
			//MY_FIXED2 dr_uv2 = MY_FIXED2((uvv.x  * 4056.345 ), (uvv.y  * 3506.345));

			MY_FIXED2 dr_t_uv = i.uv.zw; //1357.0
			MY_FIXED dr_texel =  _spread_Variatnt  ;
			MY_FIXED dr_uv_x = dr_t_uv.x * (3849.345 / 2220.0);
			MY_FIXED dr_uv_y = dr_t_uv.y * (6486.345 / 1327.0);
			MY_FIXED dr_sum1 = dr_uv_x + dr_uv_y;
			MY_FIXED dthr_z =  frac((dr_sum1) * dr_texel  + i.utils_a.x); //dottt *
			MY_FIXED dr_t_uv_y = dr_t_uv.y;
			MY_FIXED dr_dif = 50506.345/ 1827.0;
			//i._dthr_ut = float4(dthr_z, dr_uv_y, dr_t_uv.y, 50506.345/ 1827.0);
			//
			// dthr_z = i._dthr_ut.x;
			// dr_uv_y = i._dthr_ut.y;
			// dr_t_uv_y = i._dthr_ut.z;
			// dr_dif = i._dthr_ut.w;

			//MY_FIXED dr_sum2 = dr_uv2.x + dr_uv2.y;
			//MY_FIXED dr_sum_lerp = MainTex_TexelSize.z / MainTex_TexelSize.w;
			// lerp(dr_sum1, dr_sum2, dr_sum_lerp)
			//MY_FIXED dottt =  frac(dot(dr_uv/2, MY_FIXED2(12.9898, 78.233)));
//			MY_FIXED dthr =  frac((dr_sum1) * dr_texel  + i.utils_a.x); //dottt *
//			MY_FIXED dthr_y = frac(dthr + dr_uv_y) - 0.5;
////#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) 
////#if GLOW_SAMPLES_4 || GLOW_SAMPLES_8 || GLOW_SAMPLES_16
//			//MY_FIXED2 glow_dthr0_yx = MY_FIXED2(dthr_y, dthr_z) * glow_offset_glowSamples_DitherSize.xy ;///6
//			MY_FIXED dr_uv_z = dr_t_uv.y * (50506.345/ 1827.0);
//			MY_FIXED dthr_z = frac(dthr + dr_uv_z) - 0.5;

			MY_FIXED dthr_y = frac(dthr_z + dr_uv_y) - 0.5;
//#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) 
//#if GLOW_SAMPLES_4 || GLOW_SAMPLES_8 || GLOW_SAMPLES_16
			//MY_FIXED2 glow_dthr0_yx = MY_FIXED2(dthr_y, dthr_z) * glow_offset_glowSamples_DitherSize.xy ;///6
			MY_FIXED dr_uv_z = dr_t_uv_y * dr_dif;
			MY_FIXED dthr = frac(dthr_z + dr_uv_z) - 0.5;
		
//#endif
//#endif
//dthr = lerp (dthr_z, dthr, uvv.x);
//			dthr = (dthr - 0.5);

			//if (uvv.x < 0.33) return dthr;
			//if (uvv.x < 0.66) return dthr_y;
			//return dthr_z;

			//return dthr_y;
			//return dthr;
			//MY_FIXED dthr_alpha = 1 - abs(dthr)/4; //falloff ssao
			//MY_FIXED i_dthr = step(0.5, dthr) - dthr;
			//if (i_dthr < 0) return 0.5;
			//dthr = i_dthr * 1.5 + 0,5;


//			MY_FIXED _Radius = 0.3;
//			MY_FIXED _Columns = 400;
//			MY_FIXED _Rows = 400;
//			float2 center = float2(0.5, 0.5);
//float2 newUV = frac(i.uv * float2(_Columns, _Rows)); // number of columns and rows of circles
//float ht = distance(newUV, center ) * dthr * 20;
//ht = step(ht, _Radius);
//dthr = dthr_y= dthr_z =ht;

#endif

	//MY_FIXED4 asdad = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv );
	//	asdad.rgb *= _CameraVelocity.w * 2 ;
	//	return asdad;

	//if (dthr_y > 0 ) return float4(1,0,0,1);
	//return float4(1,1,0,1);
	#if defined(USE_MOTION_BLUR)

	#if F_MOTION_BLUR_FAST 

		MY_FIXED _camSpeedStep = _MotionBlurPhaseShift;
		MY_FIXED camSpeedDthr = (dthr + _camSpeedStep)*2;
		MY_FIXED camSpeedRot=  _CameraVelocity.w * _MotionBlurRotateAmount;
		MY_FIXED2 camVelocityRot = MY_FIXED2( _CameraVelocity.x * camSpeedRot, _CameraVelocity.y * camSpeedRot) ;
		//return camVelocityRot.xyxy*5;

		//MY_FIXED camSpeedStrf=  _CameraStrafeVel.w * camSpeedDthr;
		//MY_FIXED2 camVelocityStrf = MY_FIXED2( _CameraStrafeVel.x * camSpeedStrf, _CameraStrafeVel.y * camSpeedStrf) ;
		//MY_FIXED4 asdad = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv );
		//return abs(_CameraDeepVel.x - uvv.x) *abs(_CameraDeepVel.y - uvv.y) + asdad/2;

		//MY_FIXED camSpeedDeep=  _CameraDeepVel.w * camSpeedDthr;
		//MY_FIXED2 cstep = saturate(_CameraDeepVel.xy - uvv.xy); //normalize;
		//MY_FIXED2 camVelocityDeep = MY_FIXED2( cstep.x * camSpeedDeep, cstep.y * camSpeedDeep) ;
		MY_FIXED camSpeedDeep=  _CameraDeepVel.w * _MotionBlurMoveAmount;
		MY_FIXED2 cstep = saturate((_CameraDeepVel.xy - uvv.xy/2))-0.5; //normalize;
		MY_FIXED2 camVelocityDeep = MY_FIXED2( cstep.x * camSpeedDeep, cstep.y * camSpeedDeep) ;




		//lol distortion
	//	MY_FIXED2 ss = camVelocityDeep;
	//camVelocityDeep.x = camVelocityDeep.x * abs(ss.y +1.5 - 3*abs(uvv.y-0.5));
	//camVelocityDeep.y = camVelocityDeep.y * abs(ss.x +1.5 - 3*abs(uvv.x-0.5));
		MY_FIXED2 camVelocitySum = (camVelocityDeep + camVelocityRot)* camSpeedDthr;//
		uvv += camVelocitySum;
		#endif

	#if F_MOTION_BLUR_TEMPERARY 

		#endif


		#endif


			//MY_FIXED d_sign = 1 - 2 * step(0.5, dq);
			//dthr = step(dthr, 1) != 1 ? dthr : -dthr;

#define GET_Z_01(_coord)\
			Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, _coord).r) 
			//* Z_Proj
#define GET_Z_EYE(_coord)\
			LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, _coord).r) 
			//* Z_Proj

			// CALC Z AND UNIT SIZE
#if defined(NEED_MAIN_Z)
			//MY_FIXED Z_Proj = length(i.cameraDir.xyz) / _Camera_Size.y; //#TODO REMOVE
			MY_HALF sampled_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv).r;
			MY_HALF raw_d = Linear01Depth(sampled_depth);
			MY_FIXED d = raw_d * _Camera_Size.y; //* Z_Proj
			//MY_FIXED2  raw_unit_size;
			//raw_unit_size.y = _DepthCorrection.w / raw_d;
			//raw_unit_size.x = raw_unit_size.y / _Camera_Converter.x;
			//MY_FIXED2 unit_size =  raw_unit_size / _Camera_Size.y;
			//MY_FIXED2  unit_size;
			//unit_size.y = _DepthCorrection.w / d / _Camera_Converter.y;
			//unit_size.x = unit_size.y ;
			MY_FIXED unit_size = _DepthCorrection.w / d / _Camera_Converter.y;
			//return raw_d*100;

			//return  Linear01Depth(SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, MY_FIXED4(uvv.xy, 0.1,0.1)))*20;
#endif


			// TODO MODULE PIXELIZATION
			/// MY_FIXED _pixelizationAmount = 1.0;
			/// MY_FIXED pixelization = _MainTex_AbsTexelSize.w / _pixelizationAmount;
			/// uvv = floor(uvv * pixelization) / pixelization;
			/// MainTex_TexelSize.xy *= _pixelizationAmount;
			/// MainTex_TexelSize.zw /= _pixelizationAmount;


			// CALC WP
#if defined(NEED_WP)
			//MY_FIXED3 worldPos;
			//MY_FIXED3 dir = i.cameraDir.xyz;
			//dir.x *= Z_Proj;
			//dir.y *= Z_Proj;
			//dir.z /= 1.11111;
			//dir.z *= _Camera_Converter.x;
			//dir -= (length(i.cameraDir.xyz) - _Camera_Size.y ) /2;
			//dir = normalize(dir) * _Camera_Size.y * Z_Proj;
			//dir = mul(_Rotate2World, dir);
			MY_FIXED3 worldPos = raw_d * i.cameraDir.xyz + _WorldSpaceCameraPos;
#endif



			MY_FIXED4 raw_color = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv );


#define EM_UFPP_LICENSE Note That. This SSAO is right reviedes it is the EM alghorithm and you can use it only with this asset and while cannot separate copy change or publish it
#if defined(USE_SSAO)
			// 
			// 
			//MY_FIXED x = abs(frac(uvv.z / 4 ) - 0.5);
			//MY_FIXED y = abs(frac(uvv.w / 4 ) - 0.5);
			//MY_FIXED _dq = x + y; 
			//_dq = frac(uvv.x * 256.345 / MainTex_TexelSize.x + uvv.y * 156.345 / MainTex_TexelSize.y);
			//MY_FIXED _dq = frac(uvv.x * 256.345 / MainTex_TexelSize.x + uvv.y * 156.345 / MainTex_TexelSize.y);
		

			//MY_FIXED _dq = frac((i.cameraDir.z + i.cameraDir.y + i.cameraDir.x) / 100 * MainTex_TexelSize.z / _WPos.w);
			//MY_FIXED _dq = frac((uvv.x + uvv.y) * 26.345 / MainTex_TexelSize.x );
			//MY_FIXED _dq = frac( i.cameraDir.z * 1 + (uvv.y+0.5));
		


			//dthr *= 2;
			MY_FIXED2 aoRadius = unit_size * _aoRadius / 10;
			//aoRadius.xy = step(MainTex_TexelSize.xy*10, aoRadius.xy) * aoRadius.xy;

			MY_FIXED2 TX_1 = aoRadius * ( MY_FIXED2( dthr, dthr_y) * _aoStrokesSpread + 1);
		
					MY_FIXED2 ao_uvv = uvv;
			// MY_FIXED FADE = saturate((ao_uvv.y/TX_1 - 1/TX_1 + 1 ));
			MY_FIXED FADE = saturate(  (1 - ao_uvv.y)/TX_1 /2 );
			FADE *= FADE * FADE;

#if USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION || USE_SIMPLE_SSAO_3
			//MY_FIXED2 topRightUV_x = uvv - MY_FIXED2(0, TX_1.y);
			//MY_FIXED2 bottomLeftUV_x = uvv- MY_FIXED2(-TX_1.x/2, -TX_1.y);
			//MY_FIXED2 topRightUV2_x = uvv -MY_FIXED2(TX_1.x/2, -TX_1.y);
			ao_uvv.y = max(ao_uvv.y ,TX_1*4 );
	
			MY_FIXED2 topRightUV_x = ao_uvv - MY_FIXED2(0, TX_1.y);
			MY_FIXED2 bottomLeftUV_x = ao_uvv- MY_FIXED2(-TX_1.x/2, -TX_1.y);
			MY_FIXED2 topRightUV2_x = ao_uvv -MY_FIXED2(TX_1.x/2, -TX_1.y);

#else 
			ao_uvv.y = max(ao_uvv.y ,TX_1 );
			MY_FIXED2 TX_2 = TX_1 / _aoGroundBias; // * 1
			MY_FIXED2 bottomLeftUV = ao_uvv - TX_1;
			MY_FIXED2 topRightUV = ao_uvv + TX_2;
			MY_FIXED2 bottomLeftUV2 = ao_uvv - MY_FIXED2(TX_2.x, -TX_2.y);
			MY_FIXED2 topRightUV2 = ao_uvv + MY_FIXED2(TX_1.x, -TX_1.y);
#endif

			// GRAPHIT
			//MY_FIXED2 bottomLeftUV = uvv ;
			//MY_FIXED2 topRightUV = uvv ;
			//MY_FIXED2 bottomLeftUV2 = uvv ;
			//MY_FIXED2 topRightUV2 = uvv ;
			//bottomLeftUV.x += -TX_1.x;
			//bottomLeftUV.y += -TX_1.y / 2;
			//topRightUV.x -= TX_2.x /2 ;
			//topRightUV.y += TX_2.y;
			//bottomLeftUV2.x += TX_1.x;
			//bottomLeftUV2.y += TX_1.y /2;
			//topRightUV2.x += -TX_2.x/2;
			//topRightUV2.y += -TX_2.y;

			MY_FIXED p = d;
			MY_FIXED AO = 0.0,cnd = 0.0 ,cnd_step = 0.0,stp,A,B;
			//MY_FIXED PosZ01 = p - 0.01;
			MY_FIXED ssaa_alpha = 1 - step( 1.0, raw_d);
			
#if USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION || USE_SIMPLE_SSAO_3
			MY_FIXED _A0_x = GET_Z_EYE(topRightUV_x);
			MY_FIXED _d0_x = _A0_x - p;
			MY_FIXED _A1_x = GET_Z_EYE(bottomLeftUV_x);
			MY_FIXED _d1_x = _A1_x - p;
			MY_FIXED _A2_x = GET_Z_EYE(topRightUV2_x);
			MY_FIXED _d2_x = _A2_x - p;
			//_A0_x *= ssaa_alpha;
			//_d1_x *= ssaa_alpha;
			//_d2_x *= ssaa_alpha;
			MY_FIXED3 _TAO = MY_FIXED3(0.0,0.0,0.0);
#else 
			MY_FIXED _A0 = GET_Z_EYE(bottomLeftUV);
			MY_FIXED _d0 = _A0 - p;
			MY_FIXED _A1 = GET_Z_EYE(topRightUV);
			MY_FIXED _d1 = _A1 - p;
			MY_FIXED _A2 = GET_Z_EYE(bottomLeftUV2);
			MY_FIXED _d2 = _A2 - p;
			MY_FIXED _A3 = GET_Z_EYE(topRightUV2);
			MY_FIXED _d3 = _A3 - p;
			//_d0 *= ssaa_alpha;
			//_d1 *= ssaa_alpha;
			//_d2 *= ssaa_alpha;
			//_d3 *= ssaa_alpha;
			MY_FIXED4 _TAO = MY_FIXED4(0.0,0.0,0.0,0.0);
#endif




#define CSSAO(d_0, d_1, d_TAO1) \
			cnd = d_0 + d_1 ;\
			cnd_step = step(cnd, -_aoThreshold); \
			d_TAO1 = (cnd_step - cnd*cnd)		 ; //MY_FIXED dthr_alpha = 1 - abs(dthr)/4; //falloff ssao
			// cnd; //step(0, cnd) *
			//cnd = saturate(cnd+0.5)-0.5;
			//AO += -5 * cnd + cnd_step;
			//cnd = saturate(p * 2 - cnd + p * 2); \

			//CSSAO(_d0, _d1, _TAO.x)
			//CSSAO(_d2, _d1, _TAO.y)
			//CSSAO(_d2, _d3, _TAO.z)
			//CSSAO(_d0, _d3, _TAO.w)

		

#if USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION || USE_SIMPLE_SSAO_3
//MY_FIXED asdasd =  _d0_x * _d1_x * _d2_x;
			CSSAO(_d0_x, _d1_x, _TAO.x)
			CSSAO(_d0_x , _d2_x, _TAO.y)
#if USE_SIMPLE_SSAO_3
			CSSAO(_d0_x , _aoNormalsBias, _TAO.z)
#endif
		//MY_FIXED yyy = (1 - saturate(_aoAmount * (_TAO.x + _TAO.y ) - _aoBlend));
//return  yyy - asdasd;
			//return _d1_x;
			//return _d2_x;
			//return _d0_x;
			//cnd = _d0_x + _d1_x - _d2_x ;
			//cnd_step = step(cnd, -_aoThreshold); 
			//_TAO = cnd_step - cnd* cnd ;
#else 
			CSSAO(_d0, _d1, _TAO.x)
			CSSAO(_d2, _d0, _TAO.y)
			CSSAO(_d2, _d3, _TAO.z)
			CSSAO(_d1, _d3, _TAO.w)
#endif
			//CSSAO(_d1, _d3, _TAO.y)


			//if (_TAO.x > 0.98) _TAO.x = 0;
			//if (_TAO.y > 0.98) _TAO.y = 0;
			//if (_TAO.z > 0.98) _TAO.z = 0;
			//if (_TAO.w > 0.98) _TAO.w = 0;
			//return _TAO.w;
#if USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION //|| USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION
			// USE AO FIXER
			MY_FIXED aoflv = unit_size.x * _aoDistanceFixAmount;
			MY_FIXED aofix = lerp(0.95, 1.01, aoflv);
			_TAO = step( _TAO, aofix) * _TAO;
			// USE AO FIXER
#endif
			// 
			//return _TAO.w - abs(_d0 - _d1);
				//return AO;
				FADE *=  ssaa_alpha;
#if USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION || USE_SIMPLE_SSAO_3
			MY_FIXED TEMP_SSAO = (_aoAmount * (_TAO.x + _TAO.y + _TAO.z) - _aoBlend);
			TEMP_SSAO *= FADE;
#else 
			MY_FIXED TEMP_SSAO = _aoAmount * dot(f1, _TAO) - _aoBlend;
			TEMP_SSAO *= FADE;
#endif
			AO = (1 - saturate(TEMP_SSAO));

			//return AO;
			//MY_FIXED4 ddd_44 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
			//MY_FIXED4 ddd_44_ = ddd_44;
			//ddd_44 *= ddd_44;
			//return lerp(ddd_44, ddd_44_ * 2, AO);
#endif


		





			/*#define USE_GLOW_SAMPLES_VERT 1
#else
#else
#else
#else
#else
#else
#define USE_GLOW_SAMPLES_FRAG 1
#if defined(USE_GLOW_MOD) || USE_DEPTH_OF_FIELD_SIMPLE*/
//#define USE_BLUR_AND_GLOW_PASS_FOR_BLUR 1
#if defined(USE_GLOW_SAMPLES_OFF_Z) || defined(USE_GLOW_SAMPLES_ON_Z) 
//#if defined(USE_GLOW_SAMPLES_FRAG) || defined(USE_GLOW_SAMPLES_VERT)
		
			MY_FIXED2 glow_offset = i.blur_a.xy;
#if defined(USE_GLOW_SAMPLES_ON_Z) 
			glow_offset *= unit_size * 100;
#endif
			MY_FIXED2 glow_offset_glowSamples_DitherSize =  glow_offset * _glowSamples_DitherSize;
			MY_FIXED2 glow_dthr0_xy =  MY_FIXED2(dthr, dthr_y) * glow_offset_glowSamples_DitherSize.xy;///6
#if GLOW_SAMPLES_4 || GLOW_SAMPLES_8 || GLOW_SAMPLES_16
			MY_FIXED2 glow_dthr0_yx = MY_FIXED2(dthr_y, dthr_z) * glow_offset_glowSamples_DitherSize.xy ;///6
#else
			MY_FIXED2 glow_dthr0_yx = MY_FIXED2(0,0);
#endif

#if GLOW_DIRECTION_BI
			glow_dthr0_xy.y = 0;
			glow_dthr0_yx.x = 0;
#elif GLOW_DIRECTION_X
			glow_dthr0_xy.y = 0;
			glow_dthr0_yx.y = 0;
			glow_offset.y = 0;
#elif GLOW_DIRECTION_Y
			glow_dthr0_xy.x = 0;
			glow_dthr0_yx.x = 0;
			glow_offset.x = 0;
#endif

			MY_FIXED2 glow_uv0 = uvv + glow_dthr0_xy;
#if GLOW_SAMPLES_4 || GLOW_SAMPLES_8 || GLOW_SAMPLES_16
			MY_FIXED2 glow_uv1 = uvv - glow_dthr0_yx;
			//glow_uv1.x += glow_offset.x/4;
			//glow_uv0.x -= glow_offset.x/4;
#endif
#if GLOW_SAMPLES_8 || GLOW_SAMPLES_16
			//MY_FIXED2 glow_max_dthr = 0.5 * glow_offset_glowSamples_DitherSize.xy;
			//MY_FIXED2 glow_dthr2 = MY_FIXED2( glow_max_dthr.x - glow_uv0.x, glow_max_dthr.y - glow_uv0.y);
			//MY_FIXED2 glow_uv2 = uvv + glow_dthr2.xy;
			//MY_FIXED2 glow_uv3 = uvv + glow_dthr2.yx;
			MY_FIXED2 glow_uv2 = uvv - glow_dthr0_xy;
			MY_FIXED2 glow_uv3 = uvv + glow_dthr0_yx;
			//glow_uv0 += MY_FIXED2(glow_offset.x, 0);
			//glow_uv2 -= MY_FIXED2(glow_offset.x, 0);
			//glow_uv1 += MY_FIXED2(0, glow_offset.y);
			//glow_uv3 -= MY_FIXED2(0, glow_offset.y);
//#if defined(USE_GLOW_SAMPLES_ON_Z) 
#if GLOW_SAMPLES_16 
			glow_offset *= 0.4;
#else
			glow_offset *= 0.2;
#endif
			glow_uv0.x += glow_offset.x;
			glow_uv2.x -= glow_offset.x;
			glow_uv1.y += glow_offset.y;
			glow_uv3.y -= glow_offset.y;
//#endif
#endif

#ifdef USE_ALPHA_OUTPUT
#define GLOW_ALPHA_FIX(d_glow_sample5) d_glow_sample5.a = saturate(d_glow_sample5.a);
#define UN_A(d_v) d_v.a
#else
#define GLOW_ALPHA_FIX(d_glow_sample5) 
#define UN_A(d_v) 1
#endif

#if GLOW_SAMPLES_16 
			MY_FIXED2 glow_uv4 = uvv +  MY_FIXED2( glow_dthr0_xy.x, -glow_dthr0_xy.y)	*1.5;
			MY_FIXED2 glow_uv5 = uvv +  MY_FIXED2(-glow_dthr0_xy.x, glow_dthr0_xy.y)	*1.5;
			
#endif
#if GLOW_SAMPLES_16 
			MY_FIXED4 glow_sample4 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv4);
			MY_FIXED4 glow_sample5 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv5);
			GLOW_ALPHA_FIX(glow_sample4)
			GLOW_ALPHA_FIX(glow_sample5)
#endif
#if GLOW_SAMPLES_16 || GLOW_SAMPLES_8
			MY_FIXED4 glow_sample2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv2);
			MY_FIXED4 glow_sample3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv3);
			GLOW_ALPHA_FIX(glow_sample2)
			GLOW_ALPHA_FIX(glow_sample3)
#endif
#if GLOW_SAMPLES_16 || GLOW_SAMPLES_8 || GLOW_SAMPLES_4
			MY_FIXED4 glow_sample1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv1);
			GLOW_ALPHA_FIX(glow_sample1)
#endif
			MY_FIXED4 glow_sample0 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, glow_uv0);
			GLOW_ALPHA_FIX(glow_sample0)



#define UNWRAP_GLOW_COLORS_4(d_s1,d_s2,d_s3,d_s4, d_res, d_factor4) MY_FIXED4 s_rrrr_4 = MY_FIXED4(d_s1.r, d_s2.r, d_s3.r, d_s4.r);\
			MY_FIXED4 s_gggg_4 = MY_FIXED4(d_s1.g, d_s2.g, d_s3.g, d_s4.g);\
			MY_FIXED4 s_bbbb_4 = MY_FIXED4(d_s1.b, d_s2.b, d_s3.b, d_s4.b);\
			MY_FIXED4 s_aaaa_4 = MY_FIXED4(UN_A(d_s1)/ d_factor4, UN_A(d_s2)/ d_factor4, UN_A(d_s3)/ d_factor4, UN_A(d_s4)/ d_factor4) ;\
			MY_FIXED3 d_res = MY_FIXED3(dot(s_aaaa_4,s_rrrr_4)  , dot(s_aaaa_4, s_gggg_4), dot(s_aaaa_4, s_bbbb_4));

#define UNWRAP_GLOW_COLORS_2(d_s1, d_s2, d_prev_res, d_res, d_factor2) MY_FIXED3 s_rrrr_2 = MY_FIXED3(d_s1.r, d_s2.r, d_prev_res.r);\
			MY_FIXED3 s_gggg_2 = MY_FIXED3(d_s1.g, d_s2.g, d_prev_res.g);\
			MY_FIXED3 s_bbbb_2 = MY_FIXED3(d_s1.b, d_s2.b, d_prev_res.b);\
			MY_FIXED3 s_aaaa_2 = MY_FIXED3(UN_A(UN_A) / d_factor2, UN_A(UN_A) / d_factor2, 1);\
			MY_FIXED3 d_res = MY_FIXED3(dot(s_aaaa_2,s_rrrr_2)  , dot(s_aaaa_2, s_gggg_2), dot(s_aaaa_2, s_bbbb_2));

#if GLOW_SAMPLES_16 

			UNWRAP_GLOW_COLORS_4(glow_sample2,glow_sample3,glow_sample0, glow_sample1, glow_temp_unwrap_4, 8.0)
			UNWRAP_GLOW_COLORS_2(glow_sample4, glow_sample5, glow_temp_unwrap_4, glow_sample_color, 4.0)
			//MY_FIXED3 glow_sample_color = glow_temp_unwrap_4 + glow_temp_unwrap_2;

#elif GLOW_SAMPLES_8

			UNWRAP_GLOW_COLORS_4(glow_sample2,glow_sample3,glow_sample0, glow_sample1, glow_sample_color, 4.0)

#elif GLOW_SAMPLES_4
#ifdef USE_ALPHA_OUTPUT
			glow_sample0.rgb *= glow_sample0.a;
			glow_sample1.rgb *= glow_sample1.a;
#endif
			MY_FIXED3 glow_sample_color = (glow_sample0*0.7 + glow_sample1*0.3);
			//UNWRAP_GLOW_COLORS_2(glow_sample0, glow_sample1, glow_sample_color, 2.0)
#else
			MY_FIXED3 glow_sample_color = glow_sample0.rgb ;
#ifdef USE_ALPHA_OUTPUT
			glow_sample_color *= glow_sample0.a;
#endif
#endif
			//MY_FIXED glow_pre = dot(glow_sample_color,d0);


#if defined(USE_GLOW_MOD)
			MY_FIXED3 glow_pre = glow_sample_color;
#endif
//#if defined(USE_BLUR_AND_GLOW_PASS_FOR_BLUR)
#ifdef USE_ALPHA_OUTPUT
			MY_FIXED glow_alpha = dot(glow_pre, d0);
#endif
#if USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z || USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z  

			MY_FIXED4 depth_color = MY_FIXED4( glow_sample_color.rgb , 1);

#ifdef USE_ALPHA_OUTPUT
			depth_color.a = glow_alpha;
#endif
#endif




#endif // GLOW SMAPLES










/// DOF SAPLES

#if defined(USE_BLUR_SAMPLE) 
//#if defined(USE_BLUR_SAMPLES_FRAG) || defined(USE_BLUR_SAMPLES_VERT)
			MY_FIXED2 blur_offset = i.blur_a.zw;

			MY_FIXED b_radius = saturate(abs(d - _dof_z_distance) / _dof_z_aperture);
			
			blur_offset *= b_radius;
//#if defined(USE_BLUR_SAMPLES_ON_Z) 
//			blur_offset *= unit_size * 100;
//#endif
			MY_FIXED2 blur_offset_blurSamples_DitherSize = blur_offset * _blurSamples_DitherSize;
			MY_FIXED2 blur_dthr0_xy = MY_FIXED2(dthr, dthr_y) * blur_offset_blurSamples_DitherSize.xy;///6
#if BLUR_SAMPLES_4 || BLUR_SAMPLES_8 || BLUR_SAMPLES_16
			MY_FIXED2 blur_dthr0_yx = MY_FIXED2(dthr_y, dthr_z) * blur_offset_blurSamples_DitherSize.xy;///6
#else
			MY_FIXED2 blur_dthr0_yx = MY_FIXED2(0, 0);
#endif





			MY_FIXED2 blur_uv0 = uvv + blur_dthr0_xy;
#if BLUR_SAMPLES_4 || BLUR_SAMPLES_8 || BLUR_SAMPLES_16
			MY_FIXED2 blur_uv1 = uvv - blur_dthr0_yx;
			//blur_uv1.x += blur_offset.x/4;
			//blur_uv0.x -= blur_offset.x/4;
#endif
#if BLUR_SAMPLES_8 || BLUR_SAMPLES_16
			//MY_FIXED2 blur_max_dthr = 0.5 * blur_offset_glowSamples_DitherSize.xy;
			//MY_FIXED2 blur_dthr2 = MY_FIXED2( blur_max_dthr.x - blur_uv0.x, blur_max_dthr.y - blur_uv0.y);
			//MY_FIXED2 blur_uv2 = uvv + blur_dthr2.xy;
			//MY_FIXED2 blur_uv3 = uvv + blur_dthr2.yx;
			MY_FIXED2 blur_uv2 = uvv - blur_dthr0_xy;
			MY_FIXED2 blur_uv3 = uvv + blur_dthr0_yx;
			//blur_uv0 += MY_FIXED2(blur_offset.x, 0);
			//blur_uv2 -= MY_FIXED2(blur_offset.x, 0);
			//blur_uv1 += MY_FIXED2(0, blur_offset.y);
			//blur_uv3 -= MY_FIXED2(0, blur_offset.y);
//#if defined(USE_BLUR_SAMPLES_ON_Z) 
#if BLUR_SAMPLES_16 
			blur_offset *= 0.4;
#else
			blur_offset *= 0.2;
#endif
			blur_uv0.x += blur_offset.x;
			blur_uv2.x -= blur_offset.x;
			blur_uv1.y += blur_offset.y;
			blur_uv3.y -= blur_offset.y;
			//#endif
#endif

#ifdef USE_ALPHA_OUTPUT
#define BLUR_ALPHA_FIX(d_blur_sample5) d_blur_sample5.a = saturate(d_blur_sample5.a);
#define UN_A(d_v) d_v.a
#else
#define BLUR_ALPHA_FIX(d_blur_sample5) 
#define UN_A(d_v) 1
#endif

#if BLUR_SAMPLES_16 
			MY_FIXED2 blur_uv4 = uvv + MY_FIXED2(blur_dthr0_xy.x, -blur_dthr0_xy.y) * 1.5;
			MY_FIXED2 blur_uv5 = uvv + MY_FIXED2(-blur_dthr0_xy.x, blur_dthr0_xy.y) * 1.5;

#endif
#if BLUR_SAMPLES_16 
			MY_FIXED4 blur_sample4 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv4);
			MY_FIXED4 blur_sample5 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv5);
			BLUR_ALPHA_FIX(blur_sample4)
				BLUR_ALPHA_FIX(blur_sample5)
#endif
#if BLUR_SAMPLES_16 || BLUR_SAMPLES_8
				MY_FIXED4 blur_sample2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv2);
			MY_FIXED4 blur_sample3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv3);
			BLUR_ALPHA_FIX(blur_sample2)
				BLUR_ALPHA_FIX(blur_sample3)
#endif
#if BLUR_SAMPLES_16 || BLUR_SAMPLES_8 || BLUR_SAMPLES_4
				MY_FIXED4 blur_sample1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv1);
			BLUR_ALPHA_FIX(blur_sample1)
#endif
				MY_FIXED4 blur_sample0 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, blur_uv0);
			BLUR_ALPHA_FIX(blur_sample0)



#define UNWRAP_BLUR_COLORS_4(d_blur_s1,d_blur_s2,d_blur_s3,d_blur_s4, d_blur_res, d_blur_factor4) MY_FIXED4 s_blur_rrrr_4 = MY_FIXED4(d_blur_s1.r, d_blur_s2.r, d_blur_s3.r, d_blur_s4.r);\
			MY_FIXED4 s_blur_gggg_4 = MY_FIXED4(d_blur_s1.g, d_blur_s2.g, d_blur_s3.g, d_blur_s4.g);\
			MY_FIXED4 s_blur_bbbb_4 = MY_FIXED4(d_blur_s1.b, d_blur_s2.b, d_blur_s3.b, d_blur_s4.b);\
			MY_FIXED4 s_blur_aaaa_4 = MY_FIXED4(UN_A(d_blur_s1)/ d_blur_factor4, UN_A(d_blur_s2)/ d_blur_factor4, UN_A(d_blur_s3)/ d_blur_factor4, UN_A(d_blur_s4)/ d_blur_factor4) ;\
			MY_FIXED3 d_blur_res = MY_FIXED3(dot(s_blur_aaaa_4,s_blur_rrrr_4)  , dot(s_blur_aaaa_4, s_blur_gggg_4), dot(s_blur_aaaa_4, s_blur_bbbb_4));

#define UNWRAP_BLUR_COLORS_2(d_blur_s1, d_blur_s2, d_blur_prev_res, d_blur_res, d_blur_factor2) MY_FIXED3 s_blur_rrrr_2 = MY_FIXED3(d_blur_s1.r, d_blur_s2.r, d_blur_prev_res.r);\
			MY_FIXED3 s_blur_gggg_2 = MY_FIXED3(d_blur_s1.g, d_blur_s2.g, d_blur_prev_res.g);\
			MY_FIXED3 s_blur_bbbb_2 = MY_FIXED3(d_blur_s1.b, d_blur_s2.b, d_blur_prev_res.b);\
			MY_FIXED3 s_blur_aaaa_2 = MY_FIXED3(UN_A(UN_A) / d_blur_factor2, UN_A(UN_A) / d_blur_factor2, 1);\
			MY_FIXED3 d_blur_res = MY_FIXED3(dot(s_blur_aaaa_2,s_blur_rrrr_2)  , dot(s_blur_aaaa_2, s_blur_gggg_2), dot(s_blur_aaaa_2, s_blur_bbbb_2));

#if BLUR_SAMPLES_16 

				UNWRAP_BLUR_COLORS_4(blur_sample2, blur_sample3, blur_sample0, blur_sample1, blur_temp_unwrap_4, 8.0)
				UNWRAP_BLUR_COLORS_2(blur_sample4, blur_sample5, blur_temp_unwrap_4, blur_sample_color, 4.0)
				//MY_FIXED3 blur_sample_color = blur_temp_unwrap_4 + blur_temp_unwrap_2;

#elif BLUR_SAMPLES_8

				UNWRAP_BLUR_COLORS_4(blur_sample2, blur_sample3, blur_sample0, blur_sample1, blur_sample_color, 4.0)

#elif BLUR_SAMPLES_4
#ifdef USE_ALPHA_OUTPUT
				blur_sample0.rgb *= blur_sample0.a;
			blur_sample1.rgb *= blur_sample1.a;
#endif
			MY_FIXED3 blur_sample_color = (blur_sample0 * 0.7 + blur_sample1 * 0.3);
			//UNWRAP_BLUR_COLORS_2(blur_sample0, blur_sample1, blur_sample_color, 2.0)
#else
				MY_FIXED3 blur_sample_color = blur_sample0.rgb;
#ifdef USE_ALPHA_OUTPUT
			blur_sample_color *= blur_sample0.a;
#endif
#endif
			//MY_FIXED blur_pre = dot(blur_sample_color,d0);


			MY_FIXED4 depth_color = MY_FIXED4(blur_sample_color.rgb, 1);

#ifdef USE_ALPHA_OUTPUT
			MY_FIXED blur_alpha = dot(blur_sample_color, d0);
			depth_color.a = blur_alpha;
#endif


			//return depth_color;
#endif // DOF SMAPLES










//MY_FIXED camSpeed=  _CameraVelocity.w *10;
//MY_FIXED2 camVelocity = _CameraVelocity.xy *camSpeed * MY_FIXED2( dthr, dthr_y) ;

			#if defined(USE_MOTION_BLUR)
			raw_color.rgb *= _CameraVelocity.w*2  + 1;
			#endif
			//raw_color.rgb *= _CameraVelocity.w * 2 + 1;





#if defined(USE_STOKE_DEFINED)  

#if APPLY_DEPTH_CORRECTION_TO_STROKE
			MY_FIXED2 strokeOffset =  _outlineStrokesScale * unit_size/20 * _outlineStrokesScale ;
#else
			//MY_FIXED2 strokeOffset =  _outlineStrokesScale /1000 ;
			MY_FIXED2 strokeOffset =  _outlineStrokesScale * MainTex_TexelSize.xy ; 
#endif
			//

			//MY_FIXED2 strokeOffset = unit_size * _outlineStrokesScale  ;

#if APPLY_DITHER_TO_STROKE
#if APPLY_DEPTH_CORRECTION_TO_STROKE
			MY_FIXED2 stroke_dthrOffset = MY_FIXED2( dthr, dthr_y) * unit_size /16 *_outlineStrokesScale;
#else
			//MY_FIXED2 stroke_dthrOffset = MY_FIXED2( dthr, dthr_y) / 1000 * _outlineStrokesScale;
			MY_FIXED2 stroke_dthrOffset = MY_FIXED2( dthr, dthr_y) * MainTex_TexelSize.xy  * _outlineStrokesScale;
#endif
#else
			MY_FIXED2 stroke_dthrOffset = MY_FIXED2( 0, 0);
#endif
			MY_FIXED2 stroke_bottomLeftUV = uvv + strokeOffset - stroke_dthrOffset;

			//MY_FIXED2 stroke_topRightUV = uvv + MainTex_TexelSize.xy * _outlineStrokesScale;
			//MY_FIXED2 stroke_topRightUV;
			//stroke_topRightUV.x = uvv.x - strokeOffset.x ;
			//stroke_topRightUV.y = uvv.y - strokeOffset.y ;
			//MY_FIXED depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, stroke_bottomLeftUV).r;
			//MY_FIXED depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, stroke_topRightUV).r;
			//
			//
			//float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
			//
			//
			//MY_FIXED depthFiniteDifference0 = depth1 - depth0;
			////depthFiniteDifference0 = depthFiniteDifference0 > 0.001 ? depthFiniteDifference0 : 0;
			//depthFiniteDifference0 = saturate((abs(depthFiniteDifference0 * 1000) - 0.2) * 10 + _outlineStrokesBlend);
			//return depthFiniteDifference0;

			//		#pragma shader_feature_local __ USE_OUTLINE_STROKES USE_OUTLINE_COLORED_STROKES USE_OUTLINE_STROKES_INVERTED

			MY_FIXED2 stroke_topRightUV = uvv - strokeOffset +stroke_dthrOffset;
			//stroke_topRightUV.x = uvv.x - strokeOffset.x ;
			//stroke_topRightUV.y = uvv.y - strokeOffset.y ;
			MY_FIXED stroke_depth0 = GET_Z_01( stroke_bottomLeftUV) * _Camera_Size.y;
			MY_FIXED stroke_depth1 = GET_Z_01( stroke_topRightUV) * _Camera_Size.y;


			MY_FIXED stroke_dot0 = abs(stroke_depth0 - d);
			MY_FIXED stroke_dot1 = abs(stroke_depth1 - d);
			//s_dot0 =  step(2, stroke_dot0) ;

			#if USE_OUTLINE_STROKES_INVERTED
			MY_FIXED depthFiniteDifference0 = (_outlineStrokesDetection*(stroke_dot0 - stroke_dot1 )) ;
			#else
			MY_FIXED depthFiniteDifference0 = saturate(_outlineStrokesDetection*abs(stroke_dot0 - stroke_dot1 )) ;
			#endif
			//depthFiniteDifference0 = (saturate( depthFiniteDifference0 +0.5) - 0.5)*2;
			
			//
			//MY_FIXED depthFiniteDifference0 = stroke_depth0 - stroke_depth1;
			//depthFiniteDifference0 = depthFiniteDifference0 > 0.001 ? depthFiniteDifference0 : 0;
			//depthFiniteDifference0 = saturate((abs(depthFiniteDifference0 * 1000) - 0.2) * 10 + _outlineStrokesBlend);
			//return depthFiniteDifference0;

			

			/* INVERSE STROE

					MY_FIXED2 stroke_topRightUV = uvv - strokeOffset +stroke_dthrOffset;
			//stroke_topRightUV.x = uvv.x - strokeOffset.x ;
			//stroke_topRightUV.y = uvv.y - strokeOffset.y ;
			MY_FIXED stroke_depth0 = GET_Z_01( stroke_bottomLeftUV) * _Camera_Size.y;
			MY_FIXED stroke_depth1 = GET_Z_01( stroke_topRightUV) * _Camera_Size.y;


			MY_FIXED stroke_dot0 = abs(s_depth0 - d);
			MY_FIXED stroke_dot1 = abs(s_depth1 - d);
			//s_dot0 =  step(2, stroke_dot0) ;
			MY_FIXED depthFiniteDifference0 = (s_dot0 - stroke_dot1 ) ;
			depthFiniteDifference0 = (saturate( depthFiniteDifference0 +0.5) - 0.5)*2;
			

				MY_FIXED2 stroke_topRightUV;
			stroke_topRightUV.x = uvv.x - strokeOffset.x ;
			stroke_topRightUV.y = uvv.y - strokeOffset.y ;
			MY_FIXED stroke_depth0 = GET_Z_01( stroke_bottomLeftUV) * _Camera_Size.y;
			MY_FIXED stroke_depth1 = GET_Z_01( stroke_topRightUV) * _Camera_Size.y;


			MY_FIXED stroke_dot0 = abs(s_depth0 - d);
			MY_FIXED stroke_dot1 = abs(s_depth1 - d);
			//s_dot0 =  step(2, stroke_dot0) ;
			MY_FIXED depthFiniteDifference0 = stroke_dot0 -s_dot1  ;
			*/


			depthFiniteDifference0 *= _outlineStrokesBlend;



			// depthFiniteDifference0 = sqrt(depthFiniteDifference0 * depthFiniteDifference0 * depthFiniteDifference0 ) * 100000;
			//return depthFiniteDifference0;
			// MODERN Z DEPTH STRKE
			/*
				MY_FIXED _aoRadius = 4;
			MY_FIXED _aoBlend = 1;

			MY_FIXED2 stroke_bottomLeftUV = uvv - MainTex_TexelSize.xy * _aoRadius;
			MY_FIXED2 stroke_topRightUV = uvv + MainTex_TexelSize.xy * _aoRadius;
			MY_FIXED depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, stroke_bottomLeftUV).r;
			MY_FIXED depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, stroke_topRightUV).r;
			MY_FIXED depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv).r;
			//MY_FIXED depthzz = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv);
			//float linearEyeDepth0 = LinearEyeDepth(depth0); Linear01Depth
			//float linearEyeDepth0 = (depth0) *_Camera_Size.y / _Camera_Size.w;
			//float linearEyeDepth1 = (depth1) *_Camera_Size.y / _Camera_Size.w;
			//MY_FIXED linearEyedepthzz = Linear01Depth(depthzz);
			MY_FIXED linearEyeDepth0 = Linear01Depth(depth0);
			MY_FIXED linearEyeDepth1 = Linear01Depth(depth1);
			MY_FIXED linearEyeDepth2 = Linear01Depth(depth2);
			//float linearEyeDepth1 = LinearEyeDepth(depth1);
			//MY_FIXED linearLerp = (linearEyeDepth2) * 0.2 + 0.1;
			MY_FIXED depthFiniteDifference0 = saturate( 0.0 - (linearEyeDepth0 + linearEyeDepth1 - linearEyeDepth2 * 2) * 200);
			//MY_FIXED4 ddd = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
			depthFiniteDifference0 = saturate((abs(depthFiniteDifference0 * depthFiniteDifference0 ) - linearEyeDepth2 - 0.1) * 10 + _aoBlend);
			*/




#endif






//			MY_FIXED qwe = MainTex_TexelSize.w / 8;
//			MY_FIXED scal = saturate((frac(uvv.y * qwe + _Fixed_Time.z*8) - 0.75) * 1000);  
//#if UNITY_UV_STARTS_AT_TOP
//#endif
//			//return scal;
//			MY_FIXED rty = MainTex_TexelSize.w / 4;
//			MY_FIXED2 uvv2 = uvv;
//			uvv2.y = floor(uvv2.y * rty) / rty;
//			MY_FIXED4 raw_color2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2 + raw_color.rg * MainTex_TexelSize.xy * 10);
//
//			return raw_color2;


			///  //////// ZDEPTH PIXELIZATION
			//  MY_HALF asdasd = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv);
			//  asdasd = Linear01Depth(asdasd);
			//   asdasd = asdasd * _Camera_Size.y;
			//  MY_FIXED rty = MainTex_TexelSize.w / 16;
			//  MY_FIXED2 uvv2 = uvv;
			//  uvv2.y = lerp(uvv2.y, floor(uvv2.y * rty) / rty, 0.5);
			//  MY_FIXED3 res_0 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2).rgb;
			//  MY_FIXED3 res_1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2 + MainTex_TexelSize.xy * 8).rgb;
			//  MY_FIXED3 res_2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2 - MainTex_TexelSize.xy * 8).rgb;
			//  MY_FIXED3 res_3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2 + MY_FIXED2(MainTex_TexelSize.x, -MainTex_TexelSize.y) * 8).rgb;
			//  MY_FIXED3 res_4 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv2 - MY_FIXED2(MainTex_TexelSize.x, -MainTex_TexelSize.y) * 8).rgb;
			//  res_0 = res_0 * 0.4 + (res_1 + res_2 + res_3 + res_4) / 4 * 0.6;
			//  
			//  raw_color.rgb = lerp(res_0.rgb, raw_color.rgb, 1-saturate(abs(asdasd-2) / 10));
			//  return raw_color;
			//////// ZDEPTH PIXELIZATION



			//raw_color.rgb = lerp(raw_color.rgb, raw_color2.rgb, scal);






///USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z
///#if defined(USE_BLUR_AND_GLOW_PASS_FOR_BLUR)
///#if USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z //USE RECT FOR BLURING
///			MY_FIXED d01 = saturate(abs(d - 15.0) / 10.0);
///			raw_color = lerp(raw_color, depth_color  , d01);
///#else
///#endif
///#endif




			//MY_FIXED4 _dof_zoff_borders;
			//MY_FIXED _dof_zoff_feather;
			//
			//MY_FIXED _dof_z_distance;
			//MY_FIXED _dof_z_aperture;

#if USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z

			MY_FIXED _d_t = saturate((_dof_zoff_borders.z - uvv.y) / _dof_zoff_feather);
			MY_FIXED _d_b = saturate( ( uvv.y - (1- _dof_zoff_borders.x)) / _dof_zoff_feather );
			MY_FIXED _d_l = saturate((_dof_zoff_borders.w - uvv.x) / _dof_zoff_feather);
			MY_FIXED _d_r = saturate((uvv.x - (1 - _dof_zoff_borders.y)) / _dof_zoff_feather);

			MY_FIXED _d_sum = saturate(_d_t + _d_b + _d_l + _d_r);
			raw_color.rgb = lerp(raw_color.rgb, depth_color.rgb, _d_sum);


			//MY_FIXED d01 = saturate(abs(d - 15.0) / 10.0);
			//raw_color = lerp(raw_color, depth_color  , d01);
#elif USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z  
			MY_FIXED d01 = saturate(abs(d - _dof_z_distance) / _dof_z_aperture);
			raw_color.rgb = lerp(raw_color.rgb, depth_color.rgb, d01);
			raw_color.a += depth_color.a;
#elif USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z
			raw_color.rgb = lerp(raw_color.rgb, depth_color.rgb, b_radius);
			raw_color.a += depth_color.a;

#endif


			



			MY_FIXED4 color = MY_FIXED4(saturate(raw_color.rgb), raw_color.a);









//#if defined(USE_SSAO) //&& !APPLY_SSAO_AFTER_EFFECTS
//			
//			color.rgb *= (AO.rrr );
//			color.rgb = saturate(color.rgb);
//			//raw_color.rgb = saturate(raw_color.rgb);
//#endif





















			/*vec3 applyFog( in vec3  rgb,       // original color of the pixel
   in float distance ) // camera to point distance
{
	float fogAmount = 1.0 - exp( -distance*b );
	vec3  fogColor  = vec3(0.5,0.6,0.7);
	return mix( rgb, fogColor, fogAmount );
}

vec3 applyFog( in vec3  rgb,      // original color of the pixel
			   in float distance, // camera to point distance
			   in vec3  rayDir,   // camera to point vector
			   in vec3  sunDir )  // sun light direction
{
	float fogAmount = 1.0 - exp( -distance*b );
	float sunAmount = max( dot( rayDir, sunDir ), 0.0 );
	vec3  fogColor  = mix( vec3(0.5,0.6,0.7), // bluish
						   vec3(1.0,0.9,0.7), // yellowish
						   pow(sunAmount,8.0) );
	return mix( rgb, fogColor, fogAmount );
}*/

#if USE_SHADER_DISTANCE_FOG || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION

			MY_FIXED fog_lerp_result = 0.0;
			//MY_FIXED2 start_fog_uv = sin(worldPos.y + worldPos.xz / 2) / 2 + worldPos.xz;
			//MY_FIXED2 start_fog_uv = sin(worldPos.y + _Time.x * _FogTexAnimateSpeed) / 4 + cos(worldPos.y / 2.2) * 2 / 2 + worldPos.xz;
#if USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX
			MY_FIXED wpp = worldPos.y/4;
			MY_FIXED2 start_fog_uv = (wpp.xx + worldPos.xz)/ _FogTexTile.xy;
#endif
			//MY_FIXED fogDetailTexture = 0;
#if USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION

			//MY_FIXED fog_time_1 = _Time.x * _FogTexAnimateSpeed;
			//MY_FIXED2 fog_time = fog_time_1.xx / _FogTexTile;
			MY_FIXED2 fog_d_uv = (start_fog_uv 
#if defined(NEED_DTHR)
				+ MY_FIXED2( dthr, dthr_y)/400
#endif
				+  _Fog_Time.xy) ;
			MY_FIXED2 fog_d_uv2 = (start_fog_uv - _Fog_Time.xy) ;
			MY_FIXED2 start_fog_uv_2 = start_fog_uv / 2;
			MY_FIXED2 fog_d_uv3 = (start_fog_uv_2 + _Fog_Time.zw) ;
			MY_FIXED2 fog_d_uv4 = (start_fog_uv_2 - _Fog_Time.zw) ;
			//MY_FIXED _fogDetailTexture = UNITY_SAMPLE_TEX2D_SAMPLER(_NoiseTexture, _LUT1, fog_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED _fogDetailTexture = UNITY_SAMPLE_TEX2D(_NoiseTexture, fog_d_uv3).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED _fogDetailTexture2 = UNITY_SAMPLE_TEX2D(_NoiseTexture,fog_d_uv4 ).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			MY_FIXED fogDetailTexture_a = max(_fogDetailTexture, _fogDetailTexture2);
			//fogDetailTexture_a *= UNITY_SAMPLE_TEX2D(_NoiseTexture, fog_d_uv ).r*1.5; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			////fogDetailTexture_a *= UNITY_SAMPLE_TEX2D(_NoiseTexture, fog_d_uv2 ).r*3; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			//fogDetailTexture_a = saturate(fogDetailTexture_a);

			MY_FIXED	foddgDetailTexture_a = UNITY_SAMPLE_TEX2D(_NoiseTexture, fog_d_uv ).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			foddgDetailTexture_a += UNITY_SAMPLE_TEX2D(_NoiseTexture, fog_d_uv2 ).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
			fogDetailTexture_a *= foddgDetailTexture_a;
			fogDetailTexture_a = saturate(fogDetailTexture_a);
			//fogDetailTexture *=  abs( frac( start_fog_uv.x *10)  + frac( start_fog_uv.y *10) - 1 );
#endif
#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX
			MY_FIXED2 fogDetailTexture_uv = start_fog_uv ;
			MY_FIXED fogDetailTexture_s = UNITY_SAMPLE_TEX2D(_NoiseTexture, fogDetailTexture_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2
#endif
#if USE_SHADER_DISTANCE_FOG || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
			//MY_FIXED fogDensity = (d - startUpdated) / (_EndY - startUpdated);

		
			
			MY_FIXED _St_m_Ed = _StartZ - _EndZ ;


			/*
#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION

#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			MY_FIXED fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (worldPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			MY_FIXED fogDetailTexture2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, -(worldPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			fogDetailTexture = max(fogDetailTexture, fogDetailTexture2);
			MY_FIXED currentPos = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (_WorldSpaceCameraPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			MY_FIXED currentPos2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, -(_WorldSpaceCameraPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			currentPos = 1 - max(currentPos, currentPos2);
#else
			MY_FIXED4 fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (worldPos.xz) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
			MY_FIXED currentPos = 1 - UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (_WorldSpaceCameraPos.xz) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
#endif
			MY_FIXED startUpdated = _StartY - _St_m_Ed * fogDetailTexture;
#else
#endif
			*/

#if USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
#if USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
			MY_FIXED startUpdated = _StartZ - _St_m_Ed * fogDetailTexture_a;
//---
#else
			MY_FIXED startUpdated = _StartZ - _St_m_Ed * fogDetailTexture_s;
			//--
#endif
#else
			MY_FIXED startUpdated = _StartZ;
#endif

			MY_FIXED fogDensity = (startUpdated - d ) / (_St_m_Ed);
			fog_lerp_result += saturate( fogDensity * _ZFogDensity ) ;
			//MY_FIXED3 finalColor = lerp(color, _FogColor, fogDensity);
			//return MY_FIXED4(finalColor, 1.0);

#endif


#if USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
		

			MY_FIXED _height_St_m_Ed  = _FogParams.x;
			//MY_FIXED cameraFull = _FogParams.y;
			//MY_FIXED cameraD = _FogParams.z;


#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			
//#if USE_POSITION_FOR_HEIGHT_FOG
//#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
//			//MY_FIXED2 fog_height_d_uv = (worldPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile;
//			//MY_FIXED fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, fog_height_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//			//MY_FIXED fogDetailTexture2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture,-fog_height_d_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//			//fogDetailTexture = max(fogDetailTexture, fogDetailTexture2);
//			//MY_FIXED2 fog_height_d_uv2 = (_WorldSpaceCameraPos.xz + _Time.x * _FogTexAnimateSpeed) / _FogTexTile;
//					//	MY_FIXED2 fog_height_d_uv2 = i.utils_a.zw;
//			MY_FIXED2 fog_height_d_uv1 = (i.utils_a.zw - _Fog_Time.xy) ;
//			MY_FIXED2 fog_height_d_uv2 = (i.utils_a.zw + _Fog_Time.xy) ;
//			MY_FIXED currentPos = UNITY_SAMPLE_TEX2D(_NoiseTexture,  fog_height_d_uv1).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//			MY_FIXED currentPos2 = UNITY_SAMPLE_TEX2D(_NoiseTexture,  fog_height_d_uv2).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//			currentPos = 1 - max(currentPos, currentPos2); 
//#else
//			//MY_FIXED fogDetailTexture = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_NoiseTexture, (worldPos.xz ) / _FogTexTile).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//			//MY_FIXED currentPos_uv = (_WorldSpaceCameraPos.xz) / _FogTexTile;
//			MY_FIXED2 currentPos_uv = i.utils_a.zw;
//			MY_FIXED currentPos = 1 - UNITY_SAMPLE_TEX2D(_NoiseTexture,  currentPos_uv).r; //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//#endif
//			cameraD += cameraFull * currentPos.r / 2.0;
//#endif
			//cameraD +=  i.utils_a.w;

#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			MY_FIXED height_startUpdated = _StartY - _height_St_m_Ed * fogDetailTexture_a ;
#else
			MY_FIXED height_startUpdated = _StartY - _height_St_m_Ed * fogDetailTexture_s ;
#endif
#else
			MY_FIXED height_startUpdated = _StartY;
#endif
			


			MY_FIXED height_ZFogDensity = (height_startUpdated - worldPos.y) / (height_startUpdated - _EndY) ;
			//fog_lerp_result += max(saturate(  (height_ZFogDensity  )* _HeightFogDensity ) ,  saturate((_WorldSpaceCameraPos.y -_EndY)/ (_StartY - _EndY)) * i.utils_a.w ) ;
			fog_lerp_result += (saturate(  (height_ZFogDensity  )* _HeightFogDensity ) + i.utils_a.w ) ;
			//return  fog_lerp_result ;
			//fog_lerp_result = max (fog_lerp_result, (saturate(height_ZFogDensity * _HeightFogDensity ) + cameraD  ) );
		//if (uvv.x < 0.1) return fog_lerp_result ;

#endif



#endif


			







			//MY_FIXED3 _CheckerAmount = 4 ;
			//MY_FIXED3 qqfrac = frac(worldPos * _CheckerAmount);
			//MY_FIXED3 qqv = saturate((qqfrac - 0.5) * 100);
			//MY_FIXED qq = qqv.z;
			//return qq;

			// 2D ///
			
			//MY_FIXED3 op = normalize(i.cameraDir) * d + _WorldSpaceCameraPos.xyz;
			///MY_FIXED3 op = worldPos;
			///   MY_FIXED3 _CheckerAmount = 4 * 1000 * _MainTex_AbsTexelSize.xyy;
			///   MY_FIXED3 qqfrac = frac(op.xyz * _CheckerAmount);
			///  MY_FIXED3 qqv = saturate((qqfrac - 0.5)*100);
			///
			///  MY_FIXED qq = qqv.z + qqv.y + qqv.x;
			///  return qq;
			//qq = saturate(abs(qq - 0.25) * 100);

			//MY_FIXED qq = qqv.y + qqv.y + qqv.x;
			//qq = qq - saturate(qq - 1 ) * 2;
			//MY_FIXED4 dqdq = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
			//MY_FIXED sat = (1 - saturate(d / 30)) * (1 - dqdq.r);
			////uvv += sat.xx * qq.xx * _MainTex_AbsTexelSize.xy * 4;
			//return sat;

			//MY_FIXED _CheckerAmount = 1;
			//MY_FIXED3 op = d * i.viewDir.xyz + _WPos.xyz;
			//MY_FIXED qq =saturate((frac(op.y * _CheckerAmount) - 0.5) * 100);






			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS

#if FOG_BEFORE_LUT

#if USE_SHADER_DISTANCE_FOG || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			fog_lerp_result = saturate(fog_lerp_result
//#if USE_GLOW_4 || USE_GLOW_8 || USE_GLOW_16
//				- 0.3333 * _USE_HDR_FOR_FOG * (glow_res)
//#elif USE_HDR_COLORS
//				- 0.3333 * _USE_HDR_FOR_FOG * (hdrColor.r)
//#endif
			) * _FogColor.a;

			fog_lerp_result *= 1.0 - _SKIP_FOG_FOR_BACKGROUND * step(1.0, raw_d);

#if USE_FOG_ADDITIVE_BLENDING
			color += MY_FIXED4(_FogColor.rgb * fog_lerp_result, fog_lerp_result);
#else
			color = lerp(color, MY_FIXED4(_FogColor.rgb, 1), fog_lerp_result);
#endif

			//color = lerp(color, color + fog_lerp_result*2 * _FogColor + color * MY_FIXED4(_FogColor.rgb,1), fog_lerp_result);

#endif
#endif








#if USE_LUT1 || USE_LUT2

			MY_FIXED3 lut1result = color.rgb;

			//MY_FIXED2 _pixels = (uvv.xy * MY_FIXED2(_MainTex_AbsTexelSize.zw));
			//MY_FIXED _bayer_value = bayer_matrix[_pixels.y % 4][_pixels.x % 4];
			//MY_FIXED _output_color = lut1result + (_bayer_value);
			//MY_FIXED _dither_amount = step(0.5, _output_color) ;
			//lut1result = _dither_amount / 100 + lut1result;

#if !UNITY_COLORSPACE_GAMMA
			lut1result = LinearToGammaSpace(lut1result);
#endif

#if SKIP_LUTS_FOR_BRIGHT_AREAS
			MY_FIXED3 maxvalue1 = saturate(lut1result - _lutsLevelClipMaxValue);
			lut1result = saturate(lut1result);
#endif
			MY_FIXED3 scaleOffset1 = _LUT1_params.xyz;
			lut1result.z *= scaleOffset1.z;
			MY_FIXED shift1 = floor(lut1result.z);
			lut1result.xy = (lut1result.xy * scaleOffset1.z) * scaleOffset1.xy;
			lut1result.x += shift1 * scaleOffset1.y;


		

#if USE_POSTERIZE_LUTS  
			MY_FIXED _tpostOffset = _posterizationStepsAmount.x 
#if USE_POSTERIZE_DITHER
				+ dthr * _posterizationDither.x
#endif
				;
			MY_FIXED _tpostValue = lut1result.y * _tpostOffset;
			lut1result.y = (floor(_tpostValue) + ceil(_tpostValue)/2) / _tpostOffset / 1.5;
#endif





#if APPLY_CUSTOM_GRADIENT_FOR_LUT0
			lut1result = UNITY_SAMPLE_TEX2D(_LUT1_GRAD, lut1result.xy).rgb;
#else
			lut1result = UNITY_SAMPLE_TEX2D(_LUT1, lut1result.xy).rgb;
#endif
		
			//lut1result = UNITY_SAMPLE_TEX2D(_LUT1, lut1result.xy).rgb;
#if SKIP_LUTS_FOR_BRIGHT_AREAS
			//lut1result = min(_lutsLevelClipMaxValue, lut1result);
			lut1result = saturate(lut1result / _lutsLevelClipMaxValue) * _lutsLevelClipMaxValue;
			lut1result += maxvalue1;
#endif

#if !UNITY_COLORSPACE_GAMMA
			lut1result = GammaToLinearSpace(lut1result);
#endif

#if LUTS_AMONT_MORE_THAN_1
			color.rgb = saturate(lut1result - color.rgb) * _LUT1_amount + color.rgb/2;
#else
			color.rgb = lerp(color.rgb, lut1result, _LUT1_amount);
#endif

			
			//color.rgb /= saturate(_LUT1_amount - 1);

#if USE_LUT2

			MY_FIXED3 lut2result = color.rgb;
#if !UNITY_COLORSPACE_GAMMA
			lut2result = LinearToGammaSpace(lut2result);
#endif


#if SKIP_LUTS_FOR_BRIGHT_AREAS
			MY_FIXED3 maxvalue2 = saturate(lut2result - _lutsLevelClipMaxValue);
			//MY_FIXED3 maxvalue2 = saturate(lut2result - 1);
			lut2result = saturate(lut2result);
#endif
			MY_FIXED3 scaleOffset2 = _LUT2_params.xyz;
			lut2result.z *= scaleOffset2.z;
			MY_FIXED shift2 = floor(lut2result.z);
			lut2result.xy = (lut2result.xy * scaleOffset2.z) * scaleOffset2.xy;
			lut2result.x += shift2 * scaleOffset2.y;

#if USE_POSTERIZE_LUTS 
			MY_FIXED _tpostOffset2 = _posterizationStepsAmount.x
#if USE_POSTERIZE_DITHER
				+ dthr * _posterizationDither.x
#endif
				;
			MY_FIXED _tpostValue2 = lut2result.y * _tpostOffset2;
			lut2result.y = (floor(_tpostValue2) + ceil(_tpostValue2) / 2) / _tpostOffset2 / 1.5;
#endif

#if APPLY_CUSTOM_GRADIENT_FOR_LUT0
			lut2result = UNITY_SAMPLE_TEX2D_SAMPLER(_LUT2, _LUT1_GRAD, lut2result.xy).rgb;
#else
			lut2result = UNITY_SAMPLE_TEX2D_SAMPLER(_LUT2, _LUT1, lut2result.xy).rgb;
#endif

#if SKIP_LUTS_FOR_BRIGHT_AREAS
			lut2result = saturate(lut2result / _lutsLevelClipMaxValue) * _lutsLevelClipMaxValue;
#endif
			color.rgb = lerp(color.rgb, lut2result, _LUT2_amount);

#if !UNITY_COLORSPACE_GAMMA
			color = GammaToLinearSpace(color);
#endif
#endif
#endif


#if USE_WRAPS
			color.rgb = max(0.00, color.rgb + _patTint);
#endif
#if USE_HDR_COLORS
			MY_FIXED3 hdrColor = saturate(raw_color.rgb - 1.0);
			color.rgb += hdrColor;
#endif

			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
			/// LUUUUUUUUUUTSS
		








#if defined(USE_SSAO)//&& APPLY_SSAO_AFTER_EFFECTS
			color.rgb *= AO.rrr;
#endif
#if defined(USE_STOKE_DEFINED) 
#if !APPLY_STROKES_AFTER_FOG
#if USE_OUTLINE_COLORED_STROKES_NORMAL 
			color.rgb = lerp(color.rgb, _outlineStrokesColor, depthFiniteDifference0);
#elif USE_OUTLINE_COLORED_STROKES_ADD
			color.rgb += depthFiniteDifference0 * _outlineStrokesColor;
#else
			color.rgb *= (1 - depthFiniteDifference0);
			//color.rgb = saturate(color.rgb);
#endif
#endif
#endif



#if defined(USE_GLOW_MOD)
			//#if USE_HDR_COLORS
			//			glow_pre = saturate(glow_pre - _glowTreshold) * 2;
			//#else 
			//			glow_pre = saturate((glow_pre - _glowTreshold) * 2);
			//#endif
			//* _glowBrightness
			//saturate
						//MY_FIXED glow_alpha = dot(glow_pre,d0);
						//glow_alpha = (glow_alpha - _glowTreshold)  ;
						//glow_alpha = step(0,glow_alpha) * glow_alpha  * _glowContrast;
						//MY_FIXED3 glow_res = glow_pre * glow_alpha;

			glow_pre = (glow_pre - _glowTreshold);
			glow_pre = step(0, glow_pre) * glow_pre;


			//MY_FIXED glow_res = glow_pre / saturate(2.0 - _glowTreshold);
			//MY_FIXED res = max(max(max(v1, v2), v3), v4);
			/*if (v1 > 0.65) {
				color.r += v1 * v1 / 2 * _glowAmount / 7;
				color.g += v1 * v1 / 10 * _glowAmount / 7;
			}*/
			//color.rgb = lerp(color.rgb, color.rgb*2,glow_res);
#if !USE_GLOW_BLEACH_STYLE
			MY_FIXED3 glow_res = glow_pre * _glowBrightness; //_glowContrast
			color.rgb += glow_res;
#else

			MY_FIXED3 glow_res = glow_pre * 100; //_glowContrast
			MY_FIXED gamma = _glowContrast;
			color.rgb = pow(color.rgb, MY_FIXED(gamma).xxx);
			MY_FIXED3 _hdrColor = color.rgb;
			MY_FIXED3 bloomColor = color.rgb * glow_res.rgb;
			_hdrColor += bloomColor; // additive blending
			// tone mapping
			//MY_FIXED exposure = 1;
			//MY_FIXED3 result = MY_FIXED3(1.0,1,1) - exp(-_hdrColor * exposure);
			MY_FIXED3 result = _hdrColor;
			// also gamma correct while we're at it       
			result = pow(result, MY_FIXED(1.0 / gamma).xxx);
			color.rgb = MY_FIXED3(result);

			//color.rgb += glow_res;
#endif
#ifdef USE_ALPHA_OUTPUT
			color.a += glow_alpha;
#endif
#endif









#if USE_BRIGHTNESS_FUNCTIONS
			MY_FIXED3 brtColor = lerp(AvgLimin, color.rgb, _b_contrastAmount) * _b_brightAmountVector.rgb;
			MY_FIXED intensityf = dot(brtColor, LuminaceCoeff);
			//MY_FIXED3 int33 = MY_FIXED3(intensityf, intensityf, intensityf);
			//MY_FIXED lrpbrt = (_b_saturate01Value) * (color.b * color.r + 1.0) * 0.5 + 1.0;
			//MY_FIXED lrpbrt = ((_b_saturate01Value) * (color.b * color.r + 1.0) + 0.5);
			color.rgb = lerp(intensityf.xxx, brtColor, _b_saturate01);
#endif

#if USE_POSTERIZE_SIMPLE || USE_POSTERIZE_IMPROVED
			//---------------------------------------
			// GAMMA EXAMPLE
			//MY_FIXED gamma = 5.2;
			//color = pow(color, gamma);
			// POSTERIZE FUNCTION
			//color = pow(color, 1.0/ gamma);
			//---------------------------------------
#if USE_POSTERIZE_SIMPLE
			// DEFAULT POSTERIZE FUNCTION 
			MY_FIXED simpePostOffset = _posterizationStepsAmount.y
#if USE_POSTERIZE_DITHER
				+ dthr * _posterizationDither.y
#endif
				;
			color.rgb = color.rgb * simpePostOffset;
			color.rgb = (floor(color.rgb)/1.5 + ceil(color.rgb)/3.0);
			color.rgb = color.rgb / simpePostOffset;
#else
			// IMPROVED POSTERIZE FUNCTION
			MY_FIXED improvedPostOffset = _posterizationStepsAmount.z;
			MY_FIXED posterizationOffsetZeroPoint = _posterizationOffsetZeroPoint;
#if USE_POSTERIZE_DITHER
				improvedPostOffset += dthr * _posterizationDither.z;
				posterizationOffsetZeroPoint += dthr * _posterizationDither.w;
#endif
			MY_FIXED min2 = min(color.r, min(color.g, color.b));
			color.rgb -= min2;
			MY_FIXED temp_post_res = (min2 + posterizationOffsetZeroPoint) * improvedPostOffset;
			//min2 = (floor() - 0.5) / improvedPostOffset;
			min2 = (floor(temp_post_res))/ improvedPostOffset *_posterizationStepsAmount.w ;
			color.rgb += min2;
#endif
#endif




//#if USE_FINAL_GRADIENT
//			MY_FIXED grad_lum = dot(color.rgb, d0) ;
//			grad_lum = saturate(grad_lum + _GradiendOffset);
//
//			MY_FIXED4 grad_result = UNITY_SAMPLE_TEX2D(_FinalGradientTexture, MY_FIXED2(grad_lum, 0.5));
//#if !UNITY_COLORSPACE_GAMMA
//			grad_result.rgb = GammaToLinearSpace(grad_result.rgb);
//#endif
//			MY_FIXED _fga = _GradiendBrightness * grad_result.a;
//			color.rgb = lerp(color.rgb, grad_result.rgb, _fga);
//			return MY_FIXED4(color.rgb, 1) ;
//#endif

/*
#if USE_FINAL_GRADIENT_DITHER_MATH || USE_FINAL_GRADIENT_DITHER_TEX


			//float2 noiseUV = i.uv * _NoiseTex_TexelSize.xy * _MainTex_AbsTexelSize.zw;
			//noiseUV += float2(_XOffset, _YOffset);
			//float3 threshold = tex2D(_NoiseTex, noiseUV);
			//noiseUV += float2(_XOffset, _YOffset);
			//float3 threshold = tex2D(_NoiseTex, noiseUV);
				MY_FIXED grad_lum = dot(color.rgb, lum_const);
		

#if USE_FINAL_GRADIENT_DITHER_MATH
				MY_FIXED thresholdLum = saturate(dthr + 0.5) / 1.5;
#endif

#if USE_FINAL_GRADIENT_DITHER_TEX
				//asd
				MY_FIXED thresholdLum = UNITY_SAMPLE_TEX2D(_FinalGradientTexture, MY_FIXED2(ramp_uv, 0.5f));
#endif

				 thresholdLum = dot(lum_const, thresholdLum);
				//grad_lum = saturate(grad_lum + _GradiendOffset);
				

				//return grad_lum < thresholdLum ? 1 : 0;
				MY_FIXED grad_step = step(grad_lum, thresholdLum);
				MY_FIXED ramp_uv = (grad_step * (thresholdLum - grad_lum) + 1 - grad_step) * _GradiendBrightness;
				//MY_FIXED ramp_uv = thresholdLum * grad_lum;
				ramp_uv = saturate(ramp_uv + _GradiendOffset)  ;
			
				//MY_FIXED ramp_uv = grad_lum * _GradiendBrightness;
				//return grad_step;
				//float ramp_uv = grad_lum < thresholdLum ? thresholdLum - grad_lum : 1.0f;
				//return ramp_uv * color.rgba;


				// simple mix 
				//return MY_FIXED4(ramp_uv.xxx * color.rgb, 1);

				//return ramp_uv;
				//return ramp_uv > 0.8 ? 1 : 0;
			MY_FIXED4 grad_result = UNITY_SAMPLE_TEX2D(_FinalGradientTexture, MY_FIXED2(ramp_uv, 0.5));
			//return grad_result;
			//return MY_FIXED4(grad_result.rgb, 1);
#if !UNITY_COLORSPACE_GAMMA
			grad_result.rgb = GammaToLinearSpace(grad_result.rgb);
#endif
			MY_FIXED _fga = _GradiendBrightness * grad_result.a;
			color.rgb = lerp(color.rgb, grad_result.rgb, _fga) * color.rgb;
			return MY_FIXED4(color.rgb, 1);


#endif
*/



#if USE_ULTRA_FAST_SHARPEN || USE_IMPROVED_SHARPEN
		

			//MY_FIXED2 sharpenSize = _sharpenSize * _MainTex_AbsTexelSize.xy;
			//MY_FIXED2 shr_uv_a = uvv + sharpenSize;
			//MY_FIXED2 shr_uv_b = uvv - sharpenSize;

			MY_FIXED2 shr_uv_a = i.sharp_uv.xy;
			MY_FIXED2 shr_uv_b = i.sharp_uv.zw;

			
			//MY_FIXED2 _sharpenSize;
			//_sharpenSize.y = 1.0/1000.0; //*_MainTex_AbsTexelSize.xy
			//_sharpenSize.x = _Camera_Converter.x * _sharpenSize.y;
			 //MY_FIXED _sharpenZDepthCorrectionDistance = 50;
			 //_sharpenSize *= (1 - saturate(d / _sharpenZDepthCorrectionDistance));
#if USE_ULTRA_FAST_SHARPEN
			 MY_FIXED shr_sample_a = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, shr_uv_a).r * _sharpenAmounta;
			 MY_FIXED shr_sample_b = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, shr_uv_b).r * _sharpenAmounta /2;
			// MY_FIXED shr_lerp = saturate(shr_sample_a - shr_sample_b + _sharpenBottomBright_Fast);
			 MY_FIXED shr_lerp = saturate((_sharpenDarkPoint_Fast - (shr_sample_b - shr_sample_a )));//saturate // + _sharpenBottomBright_Fast == 1.0
			// MY_FIXED shr_lerp = saturate((_sharpenDarkPoint_Fast - abs(shr_sample_b - shr_sample_a )- _sharpenBottomBright_Fast));//saturate
			 //shr_lerp =  (1-shr_lerp);
#endif
#if USE_IMPROVED_SHARPEN
			//MY_FIXED4 resres = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
			MY_FIXED shr_sample_a = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, shr_uv_a).r * _sharpenAmountb;
			MY_FIXED shr_sample_b = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, shr_uv_b).r * _sharpenAmountb;
			//MY_FIXED shr_res = saturate((_sharpenDarkPoint - (shr_sample_b - shr_sample_a + _sharpenBottomBright_Improved)));//saturate
			MY_FIXED shr_res = saturate(( (shr_sample_b - shr_sample_a + _sharpenDarkPoint)));//saturate
			MY_FIXED shr_lerp = lerp(shr_res, 1, raw_color.r * _sharpenLerp_Improved );
			//MY_FIXED shr_lerp = lerp(shr_res, raw_color.r, 0);
			//MY_FIXED shr_lerp = shr_res;
#endif


			color *= shr_lerp;
#endif







#if !FOG_BEFORE_LUT

#if USE_SHADER_DISTANCE_FOG || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION || USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION
			fog_lerp_result = saturate(fog_lerp_result
#if USE_GLOW_4 || USE_GLOW_8 || USE_GLOW_16
				- 0.3333 * _USE_HDR_FOR_FOG * (glow_res)
#elif USE_HDR_COLORS
				- 0.3333 * _USE_HDR_FOR_FOG * (hdrColor.r)
#endif
			) * _FogColor.a;

			fog_lerp_result *= 1.0 - _SKIP_FOG_FOR_BACKGROUND * step( 1.0, raw_d);

#if USE_FOG_ADDITIVE_BLENDING
			color += MY_FIXED4(_FogColor.rgb* fog_lerp_result,fog_lerp_result) ;
#else
			color = lerp(color, MY_FIXED4(_FogColor.rgb,1), fog_lerp_result);
#endif

				//color = lerp(color, color + fog_lerp_result*2 * _FogColor + color * MY_FIXED4(_FogColor.rgb,1), fog_lerp_result);
		
#endif
#endif



#if defined(USE_STOKE_DEFINED) 
#if APPLY_STROKES_AFTER_FOG
#if USE_OUTLINE_COLORED_STROKES_NORMAL 
			color.rgb = lerp(color.rgb,_outlineStrokesColor, depthFiniteDifference0 ) ;
#elif USE_OUTLINE_COLORED_STROKES_ADD
			color.rgb += depthFiniteDifference0 * _outlineStrokesColor;
#else
			color.rgb *= (1 - depthFiniteDifference0);
#endif
#endif
#endif







#if defined(USE_FINAL_GRADIENT)


#if USE_FINAL_GRADIENT_DITHER_ONE_BIT
			MY_FIXED _GB = _1_GradiendBrightness;
			MY_FIXED _GO = _1_GradiendOffset;
#else
			MY_FIXED _GB = _M_GradiendBrightness;
			MY_FIXED _GO = _M_GradiendOffset;
#endif
			
		//	color.rgb = saturate(color.rgb) ;
		//	MY_FIXED grad_lum = dot(color.rgb, d1) * _GB;
			
			//MY_FIXED grad_lum = (saturate(color.r) + saturate(color.g) + saturate(color.b)) * _GB;
			//grad_lum = saturate(grad_lum + _GO);

#if USE_FINAL_GRADIENT_DITHER_ONE_BIT
			MY_FIXED grad_lum = (saturate(color.r) + saturate(color.g) + saturate(color.b)) ;
			grad_lum = saturate(grad_lum );
			//MY_FIXED2 pixels = (frac(uvv.xy + i.cameraDir.x / 100 + i.cameraDir.y / 100) * MY_FIXED2(_MainTex_AbsTexelSize.zw));
			MY_FIXED2 pixels = (uvv.xy * MY_FIXED2(_MainTex_AbsTexelSize.zw));
			//MY_FIXED grad_lum = dot(color.rgb, d0);
			//grad_lum = saturate(grad_lum + _GradiendOffset);
			//MY_FIXED4 grad_result = UNITY_SAMPLE_TEX2D(_FinalGradientTexture, MY_FIXED2(grad_lum, 0.5));
			MY_FIXED bayer_r = _BayerMultyply;
			//MY_FIXED color_result = 1;
			MY_FIXED bayer_value = bayer_matrix[pixels.y % 4][pixels.x % 4];
			MY_FIXED output_color = grad_lum + (bayer_r * bayer_value);
			MY_FIXED dither_amount = step(0.5, output_color) ;
			MY_FIXED3 texture_color = dither_amount/ _GO;

#if GRADIENT_COLOR_ADD
			color.rgb = (color.rgb + texture_color.rgb) ;
#elif GRADIENT_COLOR_MULTY
			color.rgb = (color.rgb * texture_color.rgb);
#else
			color.rgb = texture_color.rgb;
			//
#endif


#else



			//MY_FIXED2 o = screen_resolution;

#if USE_FINAL_GRADIENT_DITHER_MATH
			MY_FIXED dither_amount = saturate(dthr + 0.5) / 1.5;
#elif USE_FINAL_GRADIENT_DITHER_TEX
			MY_FIXED dither_amount = UNITY_SAMPLE_TEX2D(_DitherTexture, uvv / _DitherTextureSize * _MainTex_AbsTexelSize.zw / 100).r / 2;
#elif USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX
			MY_FIXED dither_amount2 = UNITY_SAMPLE_TEX2D(_DitherTexture, uvv / _DitherTextureSize * _MainTex_AbsTexelSize.zw / 100).r / 2;
			MY_FIXED dither_amount = dither_amount2 * saturate(dthr + 0.5);
			//#if USE_FINAL_GRADIENT_DITHER_MATH || USE_FINAL_GRADIENT_DITHER_TEX || USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX	
			//#endif
#endif

			dither_amount *= _GO;
			 
			MY_FIXED3 texture_color = step(dither_amount, color.rgb);
			//texture_color.rgb = step(dither_amount, texture_color.rgb);
			//if (qweqwe.r > 0.999) return 1;
			//return 0;
			//return qweqwe;

#if GRADIENT_COLOR_ADD
			color.rgb = (color.rgb + texture_color.rgb) / 2;
#elif GRADIENT_COLOR_MULTY
			color.rgb = (color.rgb * texture_color.rgb);
#else
			color.rgb = texture_color.rgb;
			//
#endif


			//MY_FIXED4 texture_color = UNITY_SAMPLE_TEX2D(_FinalGradientTexture, MY_FIXED2(grad_lum, 0.5));
			//MY_FIXED4 texture_color = UNITY_SAMPLE_TEX2D(_FinalGradientTest, MY_FIXED2(grad_lum, 0.5));
			//MY_FIXED4 texture_color = color.rgba;

			//MY_FIXED4 qweqwe = step(dither_amount, texture_color);

			//MY_FIXED2 pixels = uvv.xy * MY_FIXED2(_MainTex_AbsTexelSize.zw);
			//MY_FIXED bayer_value = bayer_matrix[pixels.y % 4][pixels.x % 4];
			//MY_FIXED bayer_value = bayer_matrix[(texture_color.b * 18) % 4][(texture_color.b * 18) % 4];
			//return bayer_value;



#endif

			//*PIXEL_PTR((&screen), sx, sy, 1) = color_result;

			//return MY_FIXED4 ( )


#endif

		//	MY_FIXED dither_amount = saturate(dthr + 0.5) / 1.5;
		//	color.rgb = step(dither_amount, color.rgb);




			return color;

		}







//  //MODULE DETAIL TEXTURE
//   MY_HALF _raw_d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv);
//   _raw_d = Linear01Depth(_raw_d);
//   MY_FIXED _d = _raw_d * _Camera_Size.y;
//   MY_FIXED near_d = saturate(_d / 10);
//   
//  // MY_FIXED4 addColor1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, bottomLeftUV);
//  // MY_FIXED addColor1_a = addColor1.r + addColor1.g + addColor1.b;
//  // MY_FIXED4 addColor2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, topRightUV);
//  // MY_FIXED addColor2_a = addColor2.r + addColor2.g + addColor2.b;
//  // MY_FIXED4 addColor3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
//  // MY_FIXED addColor3_a = addColor3.r + addColor3.g + addColor3.b;
//  // MY_FIXED ccc = 1 - saturate(0.8 - (addColor1_a + addColor2_a - addColor3_a * 2) * 20) * 0.5 - 0.5;
//  // ccc = ccc * (1 - near_d);
//   MY_FIXED4 ddd = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv);
//   MY_FIXED4 ddd_2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DetailTexture, worldPos.xz / 1.5 + worldPos.yx/2); //- depthFiniteDifference0.xx * MainTex_TexelSize.xy *2 
//  // return ddd;
//   return  lerp( ddd_2 * ddd  + ddd, ddd, near_d);








			// OLD MODERN Z DEPTH STRKE
			/*
			MY_FIXED _aoRadius = 1;
			MY_FIXED _aoBlend = 1;

			MY_FIXED2 bottomLeftUV = uvv - MainTex_TexelSize.xy * _aoRadius;
			MY_FIXED2 topRightUV = uvv + MainTex_TexelSize.xy * _aoRadius;
			MY_FIXED depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, bottomLeftUV);
			MY_FIXED depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, topRightUV);
			//MY_FIXED depthzz = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvv);
			//float linearEyeDepth0 = LinearEyeDepth(depth0); Linear01Depth
			//float linearEyeDepth0 = (depth0) *_Camera_Size.y / _Camera_Size.w;
			//float linearEyeDepth1 = (depth1) *_Camera_Size.y / _Camera_Size.w;
			//MY_FIXED linearEyedepthzz = Linear01Depth(depthzz);
			MY_FIXED linearEyeDepth0 = Linear01Depth(depth0);
			MY_FIXED linearEyeDepth1 = Linear01Depth(depth1);
			//float linearEyeDepth1 = LinearEyeDepth(depth1);
			MY_FIXED linearLerp = (linearEyeDepth0) * 0.2 + 0.1;
			MY_FIXED depthFiniteDifference0 = (linearEyeDepth0 - linearEyeDepth1) * 10;
			depthFiniteDifference0 = saturate((abs(depthFiniteDifference0 * depthFiniteDifference0 ) - linearLerp) * 10 + _aoBlend);
			return depthFiniteDifference0;*/




ENDCG
		}
		}
		Fallback Off
}







			
//     #if !GLOW_SAMPLES_4 && !GLOW_SAMPLES_8 && !GLOW_SAMPLES_16 
//     
//     //#if defined(USE_GLOW_MOD)
//     //dthr = MY_FIXED2(1,1);
//     //MY_FIXED br = _glowSamples_BlurRadius;
//     //MY_FIXED2 qvv = uvv;
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(dthr, dthr_y));
//     ////return glow_pre;
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(dthr_y , dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(0.5-dthr_y , dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(dthr_y ,0.5- dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(dthr_y , 0.5-dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(0.5-dthr_y , dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(0.5-dthr_y , 0.5-dthr));
//     //br * 0.8;
//     //uvv = (qvv - 0.5) / br + 0.5;
//     //glow_pre += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + br * unit_size *  MY_FIXED2(0.5-dthr_y , 0.5-dthr));
//     //#endif
//     //			//depth_color += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uvv + _glowSamples_BlurRadius * unit_size *  MY_FIXED2(dthr, dthr_y));
//   


			//MY_FIXED v1 = dot(dt, MY_FIXED4( UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x + offset, uvv.y)).r,
			//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x - offset, uvv.y)).r,
			//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x, uvv.y + offset)).r,
			//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x, uvv.y - offset)).r));
			//MY_FIXED4 at = MY_FIXED4(0.3333, 0.3333, 0.3333, 0) / 4;


		

//     
//     #if defined(USE_GLOW_SAMPLES_VERT)
//     			BLUR4_VERT_CENTERED(s1, s2, s3, s4, i.blur_a, B00, B01, B02, B03);
//     #else
//     			BLUR4(s1, s2, s3, s4, offset, B00, B01, B02, B03);
//     #endif
//     			COMBINE(s1, s2, s3, s4, at);
//     
//     #if GLOW_SAMPLES_8 || GLOW_SAMPLES_16
//     			//MY_FIXED4 bt = MY_FIXED4(0.3333, 0.3333, 0.3333, 0) / 2;
//     			//offset_b *= 1-uvv.x;
//     			//offset_8 /= 1.5;
//     #if defined(USE_GLOW_SAMPLES_VERT)
//     			BLUR4_VERT(s1, s2, s3, s4, i.blur_b, B10, B11, B12, B13);
//     #else
//     			MY_FIXED2 offset_8 = offset / 1.4;
//     			BLUR4_XY(s1, s2, s3, s4, offset_8, B10, B11, B12, B13);
//     #endif
//     			COMBINE(s1, s2, s3, s4, bt);
//     			//q1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x + offset_8, uvv.y + offset_8));
//     			//q3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x - offset_8, uvv.y - offset_8));
//     			//q4 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x + offset_8, uvv.y - offset_8));
//     			//q2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x - offset_8, uvv.y + offset_8));
//     #endif
//     
//     #if GLOW_SAMPLES_16
//     
//     #if defined(USE_GLOW_SAMPLES_VERT)
//     			BLUR4_VERT(s1, s2, s3, s4, i.blur_c, B20, B21, B22, B23); COMBINE(s1, s2, s3, s4, ft);
//     			BLUR4_VERT(s1, s2, s3, s4, i.blur_d, B30, B31, B32, B33); COMBINE(s1, s2, s3, s4, ft);
//     #else
//     			offset *= 1.5;
//     			//MY_FIXED offset_y = offset / 2;
//     			MY_FIXED2 OF_0 = MY_FIXED2(uvv.x + offset.x, uvv.y + offset.y / 2);
//     			MY_FIXED2 OF_1 = MY_FIXED2(uvv.x + offset.x / 2, uvv.y - offset.y);
//     			MY_FIXED2 OF_2 = MY_FIXED2(uvv.x - offset.x, uvv.y - offset.y / 2);
//     			MY_FIXED2 OF_3 = MY_FIXED2(uvv.x - offset.x / 2, uvv.y + offset.y);
//     			BLUR4_BACKED(s1, s2, s3, s4, OF_0, OF_1, OF_2, OF_3); COMBINE(s1, s2, s3, s4, ft);
//     			MY_FIXED2 B30 = MY_FIXED2(OF_0.x, OF_2.y);
//     			MY_FIXED2 B31 = MY_FIXED2(OF_2.x, OF_0.y);
//     			MY_FIXED2 B32 = MY_FIXED2(OF_1.x, OF_3.y);
//     			MY_FIXED2 B33 = MY_FIXED2(OF_3.x, OF_1.y);
//     			BLUR4_BACKED(s1, s2, s3, s4, B30, B31, B32, B33); COMBINE(s1, s2, s3, s4, ft);
//     #endif
//     
//     			//offset *= 1.2;
//     			//s1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x, uvv.y + offset));
//     			//s2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, MY_FIXED2(uvv.x, uvv.y - offset));
//     			//glow_pre += (dot(ft, s1) + dot(ft, s2)) * 2;
//     
//     #endif
//     
//     #endif // GLOW_SAMPLES_8 || GLOW_SAMPLES_16