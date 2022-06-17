using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using EM.PostProcessing.Runtime;
using KW = EM.PostProcessing.Runtime._KW;
using PID = EM.PostProcessing.Runtime._PID;
using System.IO;
using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditorInternal;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif
#if FAST_POSTPROCESSING_URP_USED
using UnityEngine.Rendering.Universal;
#endif

namespace EM.PostProcessing.Editor
{

     [CustomEditor(typeof(FastPostProcessingProfile))]
     [DisallowMultipleComponent]
    public class InspectorURPProfileIntegration : UnityEditor.Editor
    {






        FastPostProcessingInspector inspector_instance = new FastPostProcessingInspector();
        public override void OnInspectorGUI()
        {
            var t = target as FastPostProcessingProfile;
            if (!t) return;
            if (inspector_instance == null) inspector_instance = new FastPostProcessingInspector();
            inspector_instance.SCRIPT = t;
            if (!inspector_instance.SCRIPT || !inspector_instance.SCRIPT.TargetMaterial) return;
            inspector_instance.OnInspectorGUI(Repaint);
        }


        [MenuItem(FastPostProcessingProfile.MENU_PATH + "Create Fast Post-processing Profile", false, FastPostProcessingProfile.MENU_ORDER + 1)]
        public static FastPostProcessingProfile CreateFastPostProcessingProfile()
        {


            var file_name = EditorUtility.SaveFilePanelInProject("Create Fast Post-processing Profile", "Fast PostProcessing Profile", "asset", "Choose a new profile name");
            if (string.IsNullOrEmpty(file_name)) return null;
            if (Directory.Exists(file_name)) return null;

            var result_ob = FastPostProcessingProfile.CreateInstance<FastPostProcessingProfile>();

            var shader = Shader.Find(FastPostProcessingProfile.SHADER_NAME);
            var result_mat = new Material(shader) { name = "Fast PostProcessing Profile" };
            //TargetMaterial.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha );
            //TargetMaterial.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );
            result_mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            result_mat.SetInt("_ZWrite", 0);
            result_mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);



            //SET FEATURES
            result_mat.EnableKeyword(_KW.USE_LUT1);
            result_mat.EnableKeyword(_KW.USE_HDR_COLORS);
            result_mat.EnableKeyword(_KW.USE_POSTERIZE_DITHER);
            result_mat.EnableKeyword(_KW.APPLY_SSAO_AFTER_EFFECTS);



            //SET VALUES
            result_mat.SetTexture(PID._LUT1, FastPostProcessingProfile.DefaultData.DEFAULT_LUT);
            result_mat.SetTexture(PID._LUT2, FastPostProcessingProfile.DefaultData.DEFAULT_SECOND_LUT);
            result_mat.SetTexture(PID._NoiseTexture, FastPostProcessingProfile.DefaultData.DEFAULT_NOISE);
            result_mat.SetTexture(PID._PatternTexture, FastPostProcessingProfile.DefaultData.PATTERN_TEX);



            //CREATE ASSET
            result_ob.TargetMaterial = result_mat;
            if (File.Exists(file_name)) File.Delete(file_name);
            if (File.Exists(file_name + ".meta")) File.Delete(file_name + ".meta");
            AssetDatabase.CreateAsset(result_ob, file_name);
            AssetDatabase.Refresh();
            // AssetDatabase.ImportAsset(file_name, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AssetDatabase.AddObjectToAsset(result_mat, result_ob);
            // AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(result_mat));
            AssetDatabase.Refresh();



            return result_ob;
        }


        [MenuItem(FastPostProcessingProfile.MENU_PATH + "Welcome Screen", false, FastPostProcessingProfile.MENU_ORDER + 1000)]
        public static void ShowWelcomeScreen()
        {
            WelcomeScreen.Init(WelcomePosition);
        }
        const int buttonW = 240;
        static Rect WelcomePosition {
            get {
                var d = Screen.currentResolution.height / 1080f;
                var source = new Rect(0, d * 140, Screen.currentResolution.width, Screen.currentResolution.height - d * 280);
                var thisR = new Rect(0, source.y, buttonW + (Screen.currentResolution.width < 1500 ? 430 : Screen.currentResolution.width < 2100 ? 750 : 1000), Math.Max(source.height,
                    Math.Min(Screen.currentResolution.height, 1080) - d * 280));
                thisR.x = source.x + source.width / 2 - thisR.width / 2;
                thisR.y = source.y + source.height / 2 - thisR.height / 2;
                return thisR;
            }
        }


        [MenuItem(FastPostProcessingProfile.MENU_PATH + "Switch Asset to URP Render", true, FastPostProcessingProfile.MENU_ORDER + 2000)]
        public static bool AssignSwitchToURPData_V()
        {
            return FastPostProcessingProfile.DefaultData.CanBeSwitchToURP_raw && GraphicsSettings.currentRenderPipeline;
        }
        [MenuItem(FastPostProcessingProfile.MENU_PATH + "Switch Asset to URP Render", false, FastPostProcessingProfile.MENU_ORDER + 2000)]
        public static void AssignSwitchToURPData()
        {
            if (FastPostProcessingProfile.DefaultData.CanBeSwitchToURP) FastPostProcessingProfile.DefaultData.SwitchToURP();
        }

        [MenuItem(FastPostProcessingProfile.MENU_PATH + "Assign Default URP Settings for Project ", false, FastPostProcessingProfile.MENU_ORDER + 2000)]
        public static void AssignDefaultURPData()
        {
            if (FastPostProcessingProfile.DefaultData.CanBeSwitchToURP) FastPostProcessingProfile.DefaultData.SwitchToURP();
#if FAST_POSTPROCESSING_URP_USED
            Undo.RecordObject(GraphicsSettings.renderPipelineAsset, "Assign Default URP Data");
            GraphicsSettings.renderPipelineAsset = FastPostProcessingProfile.DefaultData.default_urp_asset as UniversalRenderPipelineAsset;
            UnityEditor.EditorUtility.SetDirty(GraphicsSettings.renderPipelineAsset);
#endif
        }


        [InitializeOnLoadMethod]
        static void AttachEnablerToScript()
        {

            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;

            double lasstEditorTime = EditorApplication.timeSinceStartup;
            double lasstEditorTime2 = EditorApplication.timeSinceStartup;
            FastPostProcessingProfile.GET_DELTA_TIME = () => {
                if (Application.isPlaying) return Time.deltaTime;
                else
                {
                    if (EditorApplication.timeSinceStartup == lasstEditorTime) return (float)(EditorApplication.timeSinceStartup - lasstEditorTime2);
                    return (float)(EditorApplication.timeSinceStartup - lasstEditorTime);
                }
            };
            EditorApplication.update += () => {
                lasstEditorTime2 = lasstEditorTime;
                lasstEditorTime = EditorApplication.timeSinceStartup;
            };

            if (Params.showWelcome != 1)
            {
                Params.showWelcome.Set(1);
                InspectorURPProfileIntegration.ShowWelcomeScreen();
            }


            var inst = FastPostProcessingHelper.CreateInstance(typeof(FastPostProcessingHelper)) as FastPostProcessingHelper;
            inst.hideFlags = HideFlags.DontSave;
            var default_data = new FastPostProcessingProfile.DefaultDataClass();
            default_data.PATTERN_TEX = inst.PATTERN_TEX;
            default_data.DEFAULT_LUT = inst.DEFAULT_LUT;
            default_data.DEFAULT_SECOND_LUT = inst.DEFAULT_SECOND_LUT;
            default_data.DEFAULT_NOISE = inst.DEFAULT_NOISE;
            //      public ScriptableObject _default_urp_asset_2019;
            // public ScriptableObject _default_urp_data_2019; //ScriptableObject
            // public ScriptableObject _default_urp_asset_2021;
            // public ScriptableObject _default_urp_data_2021; //ScriptableObject

            default_data._default_urp_asset_2019 = inst._default_urp_asset_2019;
            default_data._default_urp_data_2019 = inst._default_urp_data_2019;
            default_data._default_urp_asset_2021 = inst._default_urp_asset_2021;
            default_data._default_urp_data_2021 = inst._default_urp_data_2021;
            default_data._baking_helper = inst._baking_helper;

            default_data.default_profile = inst.default_profile;
            if (Application.isPlaying) ScriptableObject.Destroy(inst);
            else ScriptableObject.DestroyImmediate(inst);
            FastPostProcessingProfile.DefaultData = default_data;


            //UnityEditor.Compilation.Assembly a1 = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().FirstOrDefault(c => c.name.Contains("EM.PostProcessing.Editor"));
            //UnityEditor.Compilation.Assembly a2 = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().FirstOrDefault(c => c.name.Contains("EM.PostProcessing.Runtime"));
            //if (a1  == null|| a2 == null) Debug.LogError("Cannot switch Fast PostProcessing Script to URP. Because script cannot find the assemblies references, please check FastPostProcessingHelper global variables or just reimport asset");
            // FastPostProcessingProfile.DefaultData.CanBeSwitchToURP = !a1.defines.Contains("FAST_POSTPROCESSING_URP_USED");
            const string FAST_POSTPROCESSING_URP_USED = "FAST_POSTPROCESSING_URP_USED";
            FastPostProcessingProfile.DefaultData.CanBeSwitchToURP_raw = !GetDefines().Contains(FAST_POSTPROCESSING_URP_USED);
            FastPostProcessingProfile.DefaultData.CanBeSwitchToURP = !GetDefines().Contains(FAST_POSTPROCESSING_URP_USED);
            if (FastPostProcessingProfile.DefaultData.CanBeSwitchToURP)
                EditorApplication.update += () => {
                    if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
                    if (FastPostProcessingProfile.DefaultData.CanBeSwitchToURP && GraphicsSettings.renderPipelineAsset)
                        FastPostProcessingProfile.DefaultData.SwitchToURP();
                };


            FastPostProcessingProfile.DefaultData.SwitchToURP = () => {
                FastPostProcessingProfile.DefaultData.CanBeSwitchToURP = false;
                if (UnityEditor.EditorUtility.DisplayDialog("Fast PortProcessing Asset", "Do you want to swtitch Fast PortProcessing Asset to URP", "Yes", "No"))
                {
                    var l1 = GetDefines();
                    l1.Remove(FAST_POSTPROCESSING_URP_USED);
                    l1.Add(FAST_POSTPROCESSING_URP_USED);
                    SetDefines(l1);
                    Debug.Log("Fast PostProcessing Asset switched to URP");
                }
                //FastPostProcessingProfile.DefaultData.assemblies[0].
                // FAST_POSTPROCESSING_URP_USED
                //UnityEditor.EditorUtility.RequestScriptReload();
            };


            Undo.undoRedoPerformed += undo_perform;

        }
        static void undo_perform()
        {
            FastPostProcessingCamera.reset_buffer = true;
        }



        public static List<string> GetDefines()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Split(';').ToList();
        }

        public static void SetDefines(List<string> definesList)
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var defines = string.Join(";", definesList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }
    }


#if FAST_POSTPROCESSING_URP_USED

    [CustomEditor(typeof(FastPostProcessingRenderFeature))]
    [DisallowMultipleComponent]
    public class InspectorURPCameraIntegration : UnityEditor.Editor
    {
        FastPostProcessingInspector inspector_instance = new FastPostProcessingInspector();
        public override void OnInspectorGUI()
        {
            var t = target as FastPostProcessingRenderFeature;
            if (!t) return;
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;


            if (!FastPostProcessingCamera.GET_ACTIVE_CAMERA)
            {
                GUILayout.Label("Cannot find main camera");
                return;
            }
            var scr = FastPostProcessingCamera.GET_SCRIPT(FastPostProcessingCamera.GET_ACTIVE_CAMERA);

            if (inspector_instance == null) inspector_instance = new FastPostProcessingInspector();
            inspector_instance.OnInspectorGUI(scr.settings, t, Repaint);
        }


        [InitializeOnLoadMethod]
        static void AttachEnablerToScript()
        {
            FastPostProcessingCamera.RenderFeatureEnable += RenderFeatureEnable;
            FastPostProcessingCamera.RenderFeatureDisable += RenderFeatureDisable;

        }

        private static void RenderFeatureDisable()
        {
            SET_RENDER_FEATURE(false);
        }

        private static void RenderFeatureEnable()
        {
            EditorApplication.CallbackFunction asd = null;
            asd = () => {
                if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
                SET_RENDER_FEATURE(true);
                EditorApplication.update -= asd;
            };
            EditorApplication.update += asd;
        }


        static FieldInfo m_RendererDataListInfo;
        static FastPostProcessingRenderFeature[] CHECK_RENDER_FEATURE()
        {
            // if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)) return null;
            if (!(GraphicsSettings.renderPipelineAsset is UniversalRenderPipelineAsset)) return null;
            if (m_RendererDataListInfo == null) m_RendererDataListInfo = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", (BindingFlags)(-1));
            if (m_RendererDataListInfo == null) throw new Exception("Cannot read m_RendererDataList field");
            var m_RendererDataList = m_RendererDataListInfo.GetValue(GraphicsSettings.currentRenderPipeline) as ScriptableRendererData[];
            if (m_RendererDataList == null) return null;
            if (m_RendererDataList.Length == 0 || m_RendererDataList[0] == null) return null;
            var f = m_RendererDataList[0].rendererFeatures == null ? null : m_RendererDataList[0].rendererFeatures.FirstOrDefault(r => r is FastPostProcessingRenderFeature) as FastPostProcessingRenderFeature;
            if (!f) return null;
            return m_RendererDataList[0].rendererFeatures.Select(f3 => f3 as FastPostProcessingRenderFeature).Where(f2 => f2).ToArray();
        }

        static void SET_RENDER_FEATURE(bool value)
        {
            if (!(GraphicsSettings.renderPipelineAsset is UniversalRenderPipelineAsset)) return;

            if (value && CHECK_RENDER_FEATURE() == null)
            {
                var s = FastPostProcessingHelper.GetInstance;
                if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)) GraphicsSettings.renderPipelineAsset = FastPostProcessingProfile.DefaultData.default_urp_asset as UniversalRenderPipelineAsset;
                var m_RendererDataList = m_RendererDataListInfo.GetValue(GraphicsSettings.currentRenderPipeline) as ScriptableRendererData[];
                if (m_RendererDataList == null) m_RendererDataList = new ScriptableRendererData[0];
                if (m_RendererDataList.Length < 1) Array.Resize(ref m_RendererDataList, 1);
                if (!Equals(m_RendererDataList[0], FastPostProcessingProfile.DefaultData.default_urp_data)) m_RendererDataList[0] = FastPostProcessingProfile.DefaultData.default_urp_data as ScriptableRendererData;
                var f = m_RendererDataList[0] == null ? null : m_RendererDataList[0].rendererFeatures == null ? null : m_RendererDataList[0].rendererFeatures.FirstOrDefault(r => r is FastPostProcessingRenderFeature) as FastPostProcessingRenderFeature;
                if (!f)
                {
                    var r = new FastPostProcessingRenderFeature();
                    if (m_RendererDataList[0] == null) m_RendererDataList[0] = FastPostProcessingProfile.DefaultData.default_urp_data as ScriptableRendererData;
                    if (!FastPostProcessingProfile.DefaultData.default_urp_data) throw new Exception("cannot find FastPostProcessingProfile.DefaultData.default_urp_data");
                    // if (m_RendererDataList[0].rendererFeatures == null) m_RendererDataList[0].rendererFeatures = new List<ScriptableRendererFeature>(); 
                    m_RendererDataList[0].rendererFeatures.Add(r);
                    r.Create();
                }
                s.Release();
                m_RendererDataList[0].SetDirty();
                EditorUtility.SetDirty(GraphicsSettings.currentRenderPipeline);
            }

            if (!value)
            {
                foreach (var f2 in CHECK_RENDER_FEATURE())
                {
                   // Undo.RecordObject(f2, "Render Feature");
                    f2.SetActive(false);
                   // Undo.RecordObject(f2, "Render Feature");
                }
            }
            else
            {
                var f2 = CHECK_RENDER_FEATURE().First();
               // Undo.RecordObject(f2, "Render Feature");
                f2.SetActive(true);
               // Undo.RecordObject(f2, "Render Feature");
            }


        }
    }
#endif


    [CustomEditor(typeof(FastPostProcessingCamera))]
    [DisallowMultipleComponent]
    public class InspectorCameraIntegration : UnityEditor.Editor
    {
        FastPostProcessingInspector inspector_instance;
        public override void OnInspectorGUI()
        {
            var t = target as FastPostProcessingCamera;
            if (!t) return;

            // var old_p = t.settings.Profile;
            // var new_p = EditorGUILayout.ObjectField("Profile:", old_p, typeof(FastPostProcessingProfile), false) as FastPostProcessingProfile;
            // if (new_p != old_p)
            // {
            //     Undo.RecordObject(t, "Change Profile");
            //     t.settings.Profile = new_p;
            //     EditorUtility.SetDirty(t);
            //     AssetDatabase.SaveAssets();
            // }
            // if (!new_p)
            // {
            //     GUILayout.Space(20);
            //     GUILayout.Label("Please Choose a Profile");
            //     return;
            // }

            // var r = EditorGUILayout.GetControlRect();
            // r.height = 99999;
            // r.width += r.x;
            // r.x = 0;
            // GUI.BeginClip(r);
            if (inspector_instance == null) inspector_instance = new FastPostProcessingInspector();
            var s = t.settings;
            inspector_instance.OnInspectorGUI(s, t, Repaint);
            t.settings = s;
            // GUI.EndClip();
        }

    }

    class FastPostProcessingInspector
    {



        class SHARED_PARAM
        {
            internal ADDITIONAL_PARAM[] PARAMS;
        }
        class ADDITIONAL_SAMPLES_POINTER
        {
            internal string[] CONDITIONS;
            internal Vector2 OutPostion;
            internal Func<SHADER_FEATURE> TARGET_SAMPLES_FEATURE;
            internal bool inverce;
        }


        static readonly private ADDITIONAL_PARAM[] _lut_0_params = new[]
        {
             new ADDITIONAL_PARAM(new[] { "Gradient Option", KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, }, new[] { "Texture", "Manual Gradient", }, "LUT A Type:", "You can use custom gradient for lut texture creation", useassimpletoggle: false) ,
             new ADDITIONAL_PARAM(PID._LUT1,"Open LUTs Manager","LUTs Manager allows to change LUTs textures using handy interface", ADDITIONAL_PARAM_TYPE.Button, enabled_feature: KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, enabled_feature_inverse: true , enable_full_falling: true) { action = () => { PostPresets1000Window.Init(); } },
             new ADDITIONAL_PARAM(PID._LUT1_amount, "Amount of LUT A", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.@float_for_lut) { min = 0, max = 1 } ,
             new ADDITIONAL_PARAM(PID._LUT1, "LUT A Texture", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.TEXTURE, enabled_feature: KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, enabled_feature_inverse: true, enable_full_falling: true),
             new ADDITIONAL_PARAM(-1, "Left-Click at the bottom of texture location to add color marker; Right/Mid-Click to remove marker", "", ADDITIONAL_PARAM_TYPE.helpbox, enabled_feature: KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, enable_full_falling: true),
             new ADDITIONAL_PARAM(PID._FinalGradientTexture, "Gradient LUT A", "", ADDITIONAL_PARAM_TYPE.action_for_gradient, enabled_feature: KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, enable_full_falling: true),
             new ADDITIONAL_PARAM(PID._LUT1_gradient_smooth, "Gradient smooth", "You can change value to reach more sharp borders", ADDITIONAL_PARAM_TYPE.@float_for_lut, enabled_feature: KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0, enable_full_falling: true) { min = 0, max = 1 } ,
        };//n

        static readonly SHADER_FEATURE[] FE_COLORS = new SHADER_FEATURE[] {
                new SHADER_FEATURE() {
                S_TITLE = new[]{"Fast LUTs Presets & Colors", "This module allows to apply fast stylized presets for camera colors"},
                S_TIP_TEXT = "You can also create you own LUTs in different ways, you can also try using our built-in features, you can find it in the 'Tools/"+FastPostProcessingProfile.ASSET_NAME+"/Converter'",
                HELP_IMAGE = "HELP_LUTS",
                default_feature = 1,
                disable_toggle = true,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_LUT0 , "No LUT", "Use one LUT preset with blending adjustment" },
                    new[]{ KW.USE_LUT1 , "Single LUT", "Use one LUT preset with blending adjustment" },
                    new[] { KW.USE_LUT2, "Double LUTs", "You can blend two LUTs and main camera colors together" }
                },
                ADDITIONAL_PARAMS_SET = new[]{
                     new ADDITIONAL_PARAM[]{ },
                     _lut_0_params.Concat(
                    new[]{
                            new ADDITIONAL_PARAM( KW.SKIP_LUTS_FOR_BRIGHT_AREAS, "Skip LUTs For Bright Areas", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , HELP_KEY: "HELPTIP_SKIP_LUTS") ,
                        new ADDITIONAL_PARAM(PID._lutsLevelClipMaxValue,"Bright Point","Choose the point that will be a bright point",ADDITIONAL_PARAM_TYPE.@float, enabled_feature:KW.SKIP_LUTS_FOR_BRIGHT_AREAS){ min = 0, max = 1},
                    }).ToArray(),
                     _lut_0_params.Concat(
                    new[]{
                       // new ADDITIONAL_PARAM(PID._LUT1,"Open LUTs Manager","LUTs Manager allows to change LUTs textures using handy interface",ADDITIONAL_PARAM_TYPE.Button){ action =() =>{ PostPresets1000Window.Init(); } },
                       // new ADDITIONAL_PARAM(PID._LUT1_amount,"Amount of LUT A","How much the color will be changed",ADDITIONAL_PARAM_TYPE.@float_for_lut){ min = 0, max = 3},
                       // new ADDITIONAL_PARAM(PID._LUT1,"LUT A","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.TEXTURE),
                        new ADDITIONAL_PARAM(PID._LUT2_amount,"Amount of LUT B","How much the color will be changed",ADDITIONAL_PARAM_TYPE.@float){ min = 0, max = 1},
                        new ADDITIONAL_PARAM(PID._LUT2,"LUT B","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.TEXTURE),
                        new ADDITIONAL_PARAM( KW.SKIP_LUTS_FOR_BRIGHT_AREAS, "Skip LUTs For Bright Areas", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , HELP_KEY: "HELPTIP_SKIP_LUTS") ,
                        new ADDITIONAL_PARAM(PID._lutsLevelClipMaxValue,"Bright Point","Choose the point that will be a bright point",ADDITIONAL_PARAM_TYPE.@float, enabled_feature:KW.SKIP_LUTS_FOR_BRIGHT_AREAS){ min = 0, max = 1},
                    }).ToArray(),
                },
                POST_SHADER_FEATURES = new[]{
                    new SHADER_FEATURE(){
                        S_TITLE = new []{"Use HDR Colors (Fast)", "It handy if you are using dark post effects, if this options is disabled, the colors those outside the 0..1 range may be clamped"},
                        FEATURES_ON = new[]{ new[]{ KW.USE_HDR_COLORS , "", "" } },
                        HELP_IMAGE = "HELPTIP_HDR"
                    },
                   /* new SHADER_FEATURE(){
                        S_TITLE = new []{"Skip LUTs For Bright Areas", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors"},
                        FEATURES_ON = new[]{ new[]{ KW.SKIP_LUTS_FOR_BRIGHT_AREAS , "", "" } },
                        ADDITIONAL_PARAMS = new[]{
                            new[]{new ADDITIONAL_PARAM(PID._lutsLevelClipMaxValue, "Bright Point", "Choose the point that will be a bright point", ADDITIONAL_PARAM_TYPE.@float) },
                            },
                        HELP_IMAGE = "HELPTIP_SKIP_LUTS"
                    },*/
                    new SHADER_FEATURE(){
                        S_TITLE = new []{"Use Brightness/Contrast", "This feature allows you to adjust brightness, contrast and saturation"},
                        FEATURES_ON = new[]{ new[]{ KW.USE_BRIGHTNESS_FUNCTIONS , "", "" } }, //Standard Brightness Control Option
						ADDITIONAL_PARAMS_SET = new[]{
                            new[]{new ADDITIONAL_PARAM(PID._b_brightAmount,"Bright","Bright amount",ADDITIONAL_PARAM_TYPE.@float),
                            new ADDITIONAL_PARAM(PID._b_saturate01,"Saturate","Saturate amount",ADDITIONAL_PARAM_TYPE.@float)  { min = 0, max = 3},
                            new ADDITIONAL_PARAM(PID._b_contrastAmount,"Contrast","Contrast amount",ADDITIONAL_PARAM_TYPE.@float)  { min = 0, max = 3},
                            new ADDITIONAL_PARAM(PID._b_colorsAmount,"Colors Amount","Adjust colors separately (it does not reduce performance)",ADDITIONAL_PARAM_TYPE.ColorXYZ){ min = 0, max = 3} },
                            },
                    },

                }
        }
                ,

                  new SHADER_FEATURE()
            {
                S_TITLE = new[] { "Use Posterization", "Reduces the number of shades that are used for drawing, it giving some stylizing the image" },
                S_TIP_TEXT = "If you are using the LUTs block, you can try to use LUTs posterization",
                HELP_IMAGE = "HELP_POSTERIZATION",
                default_feature = 0,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_POSTERIZE_LUTS , "LUTs Posterization", "Pretty fast solution, that uses in the LUTs block," },
                    new[]{ KW.USE_POSTERIZE_IMPROVED , "Improved Posterization", "Pretty nice solution, that make a little more calculations" },
                    new[]{ KW.USE_POSTERIZE_SIMPLE , "Simple Posterization", "Standard posterization solution" },
    },
                ADDITIONAL_PARAMS_SET = new[]{
                    new[] { new ADDITIONAL_PARAM( PID._posterizationStepsAmount,"Number of Colors","Approximate number of shades that will be used for final image", ADDITIONAL_PARAM_TYPE.vectorX) { min = 0 , max = 16 },
                            new ADDITIONAL_PARAM( KW.USE_POSTERIZE_DITHER,"Use Diffusion","Diffusion makes small displacements in the pixel of color transition", HELP_KEY: "HELPTIP_DITHER"),
                            new ADDITIONAL_PARAM( PID._posterizationDither,"Diffusion Size","How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.vectorX, enabled_feature:KW.USE_POSTERIZE_DITHER)
                    },

                    new[] { new ADDITIONAL_PARAM( PID._posterizationStepsAmount, "Number of Colors", "Approximate number of shades that will be used for final image", ADDITIONAL_PARAM_TYPE.vectorZ ) { min = 0 , max = 16 },
                        new ADDITIONAL_PARAM( PID._posterizationStepsAmount, "Contrast", "Increase bright of bright areas", ADDITIONAL_PARAM_TYPE.vectorW ) { min = 0.2f , max = 2 },
                        new ADDITIONAL_PARAM( PID._posterizationOffsetZeroPoint, "Zero Point Offset", "Bright value that used as start point for color calculations", ADDITIONAL_PARAM_TYPE.@float )  { min = 0 , max = 1 },
                        new ADDITIONAL_PARAM( KW.USE_POSTERIZE_DITHER, "Use Diffusion", "Diffusion makes small displacements in the pixel of color transition" , HELP_KEY: "HELPTIP_DITHER") ,
                        new ADDITIONAL_PARAM( PID._posterizationDither, "Diffusion Size", "How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.vectorZ, enabled_feature:KW.USE_POSTERIZE_DITHER  ),
                        new ADDITIONAL_PARAM( PID._posterizationDither, "Diffusion For Dark Areas", "Diffusion size of the black areas", ADDITIONAL_PARAM_TYPE.vectorW, enabled_feature:KW.USE_POSTERIZE_DITHER  ) { min = -0.2f , max = 1 }
        },

                    new[] { new ADDITIONAL_PARAM( PID._posterizationStepsAmount, "Number of Colors", "Approximate number of shades that will be used for final image", ADDITIONAL_PARAM_TYPE.vectorY ) { min = 0 , max = 16 },
                        new ADDITIONAL_PARAM( KW.USE_POSTERIZE_DITHER, "Use Diffusion", "Diffusion makes small displacements in the pixel of color transition" , HELP_KEY: "HELPTIP_DITHER") ,
                        new ADDITIONAL_PARAM( PID._posterizationDither, "Diffusion Size", "How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.vectorY, enabled_feature:KW.USE_POSTERIZE_DITHER )
                    },
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_POSTERIZE_LUTS, KW.USE_POSTERIZE_DITHER}, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_POSTERIZE_IMPROVED, KW.USE_POSTERIZE_DITHER }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_POSTERIZE_SIMPLE, KW.USE_POSTERIZE_DITHER  }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                }
            }
                ,


                           new SHADER_FEATURE() {
                    S_TITLE = new[]{ "Dithering", "Color gradient as the final color corresponding to the pixel luminocity"},
                    S_TIP_TEXT = "It is alternative for lut option, that totaly replaces result colors/",
                    HELP_IMAGE = null,
                    default_feature = 1,
                      FEATURES_ON = new[]{
                        new[]{ KW.USE_FINAL_GRADIENT_DITHER_ONE_BIT, "1-Bit Dither", "1-Bit uses only black and white colors" },
                        new[]{ KW.USE_FINAL_GRADIENT_DITHER_MATH, "Animated Dither", "Used special dither algorithm, you can adjust it at the right side" },
                        new[]{ KW.USE_FINAL_GRADIENT_DITHER_TEX, "Texture Dither", "Dithering will be calculated according with texture's pixels" },
                        new[]{ KW.USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX, "Anim+Texture Fither", "Used Animated Dither and Texture Dither both" },
                    },




        ADDITIONAL_PARAMS_F = ()=> new[]{
                        GRADIENTS_RAMP.Concat(GRADIENTS_1_BIT).Concat(GRADIENTS_PARAMS_LAST_SECTION).ToArray(),
                        GRADIENTS_RAMP.Concat(GRADIENTS_M).Concat(GRADIENTS_PARAMS_LAST_SECTION).ToArray(),
                        GRADIENTS_RAMP.Concat(GRADIENTS_M).Concat(GRADIENTS_PARAMS_TEX).Concat(GRADIENTS_PARAMS_LAST_SECTION).ToArray(),
                        GRADIENTS_RAMP.Concat(GRADIENTS_M).Concat(GRADIENTS_PARAMS_TEX).Concat(GRADIENTS_PARAMS_LAST_SECTION).ToArray(),
                    },
                  samples_pointes = new[]  {
                        new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_FINAL_GRADIENT_DITHER_MATH }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                        new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                       // new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_FINAL_GRADIENT_DITHER_MATH }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                         },

        },



                  new SHADER_FEATURE() {
                S_TITLE = new[]{ "Pixelization", "Pixelization and Screen lines"},
                S_TIP_TEXT = "While you have to create patterns using the photoshop",
                HELP_IMAGE = null,
                default_feature = 0,
                  FEATURES_ON = new[]{
                    new[]{ KW.USE_WRAPS, "Deafult Beta 0.9", "Pixelization, lines, patterns and etc" },
                },
                 ADDITIONAL_PARAMS_SET = new[]{
                    new[]{
                         new ADDITIONAL_PARAM(PID._PatternTexture,"Pattern Texture","", ADDITIONAL_PARAM_TYPE.TEXTURE),
                        new ADDITIONAL_PARAM(PID._patTintAffect,"Tint Affect","Tint Affect",ADDITIONAL_PARAM_TYPE.@float){ min = -1f, max = 1f},
                        new ADDITIONAL_PARAM(PID._patUvAffect,"UV Affect","UV Affect",ADDITIONAL_PARAM_TYPE.@float){ min = -1f, max = 1f},
                        new ADDITIONAL_PARAM(PID._patSize,"Pattern Size","Pattern Size",ADDITIONAL_PARAM_TYPE.@float){ min = 0.001f, max = 10f},
                        new ADDITIONAL_PARAM(PID._patPixelization, "Pixelization", "Pixelization", ADDITIONAL_PARAM_TYPE.@float){ min = 0.1f, max = 1},
                    }
                },
    },
        };

        static readonly SHADER_FEATURE[] FE_GLOW_BLUR =  {

               new SHADER_FEATURE()
            {
                S_TITLE = new[] { "Use Sharpness", "Increases sharpness and stylizing of the borders" },
                S_TIP_TEXT = "You can also use this feature simply for styling your image using big radius",
                HELP_IMAGE = "HELP_SHARPNESS",
                default_feature = 1,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_ULTRA_FAST_SHARPEN , "Standard Sharpness", "Standard sharpness uses simple difference among pixels of the main texture" },
                    new[]{ KW.USE_IMPROVED_SHARPEN , "Threshold Sharpness", "Threshold uses camera colors to adjust sharpness according the pixel brightness" },
                },
                ADDITIONAL_PARAMS_SET = new[]{
                    new[]{
                        new ADDITIONAL_PARAM(PID._sharpenAmounta,"Amount","Sharpness Amount",ADDITIONAL_PARAM_TYPE.@float),
                        new ADDITIONAL_PARAM(PID._sharpenSizea,"Size","Pixels Offset Radius",ADDITIONAL_PARAM_TYPE.@float) { min = 0.00001f, max = 5},
                       // new ADDITIONAL_PARAM(PID._sharpenBottomBright_Fast,"Bright Point","Uses to adjust style",ADDITIONAL_PARAM_TYPE.@float) { min = -1, max = 1},
                        new ADDITIONAL_PARAM(PID._sharpenDarkPoint_Fast,"Dark Point","Uses to adjust style",ADDITIONAL_PARAM_TYPE.@float) { min = -1, max = 1}
                    },
                    new[]{
                        new ADDITIONAL_PARAM(PID._sharpenAmountb,"Amount","Sharpness Amount",ADDITIONAL_PARAM_TYPE.@float),
                        new ADDITIONAL_PARAM(PID._sharpenSizeb,"Size","Pixels Offset Radius",ADDITIONAL_PARAM_TYPE.@float){ min = 0.001f, max = 5},
                        // new ADDITIONAL_PARAM(PID._sharpenBottomBright_Improved,"Bright Point","Uses to adjust style",ADDITIONAL_PARAM_TYPE.@float) { min = -1, max = 1},
                        new ADDITIONAL_PARAM(PID._sharpenDarkPoint,"Dark Point","Uses to adjust style",ADDITIONAL_PARAM_TYPE.@float) { min = -1, max = 1},
                        new ADDITIONAL_PARAM(PID._sharpenLerp_Improved,"Threshold","Uses to adjust style",ADDITIONAL_PARAM_TYPE.@float) { min = 0.1f, max = 3}
                    },

                },
                samples_count_to_title = new[]
                {
                    new []{ "TRUE", "2"},
                }
            }
            ,
             new SHADER_FEATURE() {
                S_TITLE = new[]{"Outline Stroke", "This module allows to apply fast stylized presets for camera colors"},
                S_TIP_TEXT = "This is author's outline stroke - simultaneously fast and without artifacts and without calculating normals",
                HELP_IMAGE = null,
                default_feature = 0,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_OUTLINE_STROKES, "Simple Stroke", "Simple stroke keeps black colors for final image" },
                    new[]{ KW.USE_OUTLINE_COLORED_STROKES_ADD, "Use Additive Color", "Use summation for final colors blending" },
                    new[]{ KW.USE_OUTLINE_COLORED_STROKES_NORMAL, "Use Normal Color", "Use replacement for final colors blending" },
                    new[]{ KW.USE_OUTLINE_STROKES_INVERTED, "Inverse Mode", "Special mod allows to add contrast for your image" },
                },
                ADDITIONAL_PARAMS_F = ()=> new[]{
                    _stroke_params_a.ToArray(),
                    _stroke_params_b.Concat(_stroke_params_a).ToArray(),
                    _stroke_params_b.Concat(_stroke_params_a).ToArray(),
                    _stroke_params_a.ToArray(),
                },

                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_OUTLINE_STROKES}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_OUTLINE_STROKES_INVERTED}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_OUTLINE_COLORED_STROKES_ADD }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_OUTLINE_COLORED_STROKES_NORMAL }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.APPLY_DITHER_TO_STROKE }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },

                },
                samples_count_to_title = new[]
                {
                    new []{ "TRUE", "2"},
                }
        },
                        new SHADER_FEATURE()
          {
                S_TITLE = new[]{"Ultra Fast Glow", "This module allows to apply additional blurring for bright areas of the final image"},
                S_TIP_TEXT = "Use blending toggle to adjust glow style + or *",
                HELP_IMAGE = null,
                default_feature = 0,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_GLOW_MOD, "Default Glow", "Default Glow Feature. You can find glow samples settings at the left part of the current inspector window" },
                },
                ADDITIONAL_PARAMS_SET = new[]{
                    new[]{
                        new ADDITIONAL_PARAM(PID._glowContrast,"Gamma","Gamma",ADDITIONAL_PARAM_TYPE.@float, enabled_feature:KW.USE_GLOW_BLEACH_STYLE, enabled_feature_inverse: true, enable_full_falling: true ){ min = 0.01f, max = 10},
                       new ADDITIONAL_PARAM(PID._glowBrightness, "Brightness", "Contrast", ADDITIONAL_PARAM_TYPE.@float, enabled_feature:KW.USE_GLOW_BLEACH_STYLE, enabled_feature_inverse: false, enable_full_falling: true),
                        new ADDITIONAL_PARAM(PID._glowTreshold, "Threshold", "Threshold", ADDITIONAL_PARAM_TYPE.@float),
                        new ADDITIONAL_PARAM( KW.USE_GLOW_BLEACH_STYLE, "Use Multiply Blend", "In this case final glow result will be added with source colors" , HELP_KEY: "") ,
                    }
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_GLOW_MOD }, TARGET_SAMPLES_FEATURE = () => GLOW_SAMPLES },
               }
        },
                      new SHADER_FEATURE()
        {
            S_TITLE = new[] { "Ultra Fast DOF", "This module allows to blur different areas on the screen" },
                S_TIP_TEXT = "Use 1 sample or glow sample if you want ot reach maximum performance",
                HELP_IMAGE = null,
                default_feature = 2,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z, "Use Glow Samples / No Z Depth", "In this case module doesn't use the Z depth and doesn't create additional samples, you can just apply simple blur for left/top/right/bottom areas of the screen" },
                    new[]{ KW.USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z, "Use Glow Samples + Z Depth", "This option basically works like normal DOF, but in this case the DOF doesn't create additional samples, and uses glow samples with const blur size, to improve performance" },
                    new[]{ KW.USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z, "True DOF Samples", "This feature uses separate sample, but it works like normal DOF, with separate samples block. If you used 1 smaple option, it will be fast enough." }
                },
                 ADDITIONAL_PARAMS_F = ()=> new[]{
                    _dof_params_a.ToArray(),
                    _dof_params_b.ToArray(),
                    _dof_params_b.ToArray(),
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z }, TARGET_SAMPLES_FEATURE = () => GLOW_SAMPLES },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z }, TARGET_SAMPLES_FEATURE = () => GLOW_SAMPLES },

                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z }, TARGET_SAMPLES_FEATURE = () => BLUR_SAMPLES },

                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },

             }

        }
                      };




        static readonly private ADDITIONAL_PARAM[] _dof_params_a = new[]
     {
            new ADDITIONAL_PARAM(PID._dof_zoff_borders, "Border Top", "Border blur offset",ADDITIONAL_PARAM_TYPE.vectorX) {min = -1 , max = 1},
            new ADDITIONAL_PARAM(PID._dof_zoff_borders, "Border Right", "Border blur offset",ADDITIONAL_PARAM_TYPE.vectorY) {min = -1 , max = 1},
            new ADDITIONAL_PARAM(PID._dof_zoff_borders, "Border Bottom", "Border blur offset",ADDITIONAL_PARAM_TYPE.vectorZ) {min = -1 , max = 1},
            new ADDITIONAL_PARAM(PID._dof_zoff_borders, "Border Left", "Border blur offset",ADDITIONAL_PARAM_TYPE.vectorW) {min = -1 , max = 1},
            new ADDITIONAL_PARAM(PID._dof_zoff_feather, "Feather", "Border blur size",ADDITIONAL_PARAM_TYPE.@float) {min = 0 , max = 1},
        };
        static readonly private ADDITIONAL_PARAM[] _dof_params_b = new[]
     {
            new ADDITIONAL_PARAM(PID._dof_z_distance, "Z Distance", "Border blur size",ADDITIONAL_PARAM_TYPE.@float),
            new ADDITIONAL_PARAM(PID._dof_z_aperture, "Aperture", "Border blur size",ADDITIONAL_PARAM_TYPE.@float) { min = 0.2f, max = 5 },
        };

        static readonly private ADDITIONAL_PARAM[] _ssao_params_a = new[]
        {
            new ADDITIONAL_PARAM(PID._aoRadius, "Radius", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.@float),
            new ADDITIONAL_PARAM(PID._aoAmount, "Contrast", "Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float),
            new ADDITIONAL_PARAM(PID._aoBlend, "Bright", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.@float),
            new ADDITIONAL_PARAM(PID._aoThreshold, "Threshold", "Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float) {min = 0.001f, max = 0.5f},
            new ADDITIONAL_PARAM(PID._aoStrokesSpread, "Spread", "Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float) {min =  1 },
           // new ADDITIONAL_PARAM( new[]{ "-", KW.APPLY_SSAO_AFTER_EFFECTS, },new[]{ "Before Effects", "After Effects", } , "Apply SSAO:", "You can choose second option to increase contrast of ssao" ) ,
        };
        static readonly private ADDITIONAL_PARAM[] _ssao_params_b = new[]
        {
            new ADDITIONAL_PARAM(PID._aoDistanceFixAmount, "Distance Fix Amount", "Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float),
        };
        static readonly private ADDITIONAL_PARAM[] _ssao_params_c = new[]
        {
            new ADDITIONAL_PARAM(PID._aoNormalsBias, "Normals Bias", "Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float) {min = 0,max = 0.2f},
        };
        static readonly private ADDITIONAL_PARAM[] _ssao_params_d = new[]
        {
        new ADDITIONAL_PARAM(PID._aoGroundBias, "Ground Bias", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.@float) { min = 0.9f, max = 1.0f},
        };

        static readonly private ADDITIONAL_PARAM[] _stroke_params_a = new[]
        {
            new ADDITIONAL_PARAM(PID._outlineStrokesBlend,"Add Color","Adjust the contrast of strokes", ADDITIONAL_PARAM_TYPE.@float) { min = 0 , max = 5},
            new ADDITIONAL_PARAM(PID._outlineStrokesDetection,"Detection","Adjust the contrast of strokes", ADDITIONAL_PARAM_TYPE.@float) { min = 0, max = 30},
            new ADDITIONAL_PARAM(PID._outlineStrokesScale,"Radius","Size of strokes", ADDITIONAL_PARAM_TYPE.@float) { min = 0 , max = 10},
            new ADDITIONAL_PARAM( new[]{ "-", KW.APPLY_STROKES_AFTER_FOG, },new[]{ "Before Fog", "After Fog", } , "Apply Strokes:", "You can choose second option to increase contrast of strokes" ) ,
            new ADDITIONAL_PARAM(new[] { "-", KW.APPLY_DITHER_TO_STROKE, }, new[] { "-", "-", }, "Apply diffusion:", "Enable it if you think that edges are too sharp", useassimpletoggle: true) ,
            new ADDITIONAL_PARAM(new[] { "-", KW.APPLY_DEPTH_CORRECTION_TO_STROKE, }, new[] { "-", "-", }, "Distance correction:", "Outline size will be decreased on long distance", useassimpletoggle: true) ,
        };
        static readonly private ADDITIONAL_PARAM[] _stroke_params_b = new[]
        {
            new ADDITIONAL_PARAM(PID._outlineStrokesColor, "Color", "Color of strokes", ADDITIONAL_PARAM_TYPE.ColorField) { min = 0, max = 3} ,
        };




        static readonly SHADER_FEATURE[] FE_EFFECTS =  {
                     new SHADER_FEATURE() {
                S_TITLE = new[]{ "Fast Motion Blur", "Uses special not standard alghoritms"},
                S_TIP_TEXT = "Not completed yet",
                HELP_IMAGE = null,
                default_feature = 0,
                  FEATURES_ON = new[]{
                    new[]{ KW.USE_F_MOTION_BLUR_FAST, "Ultra Fast Screen Calc", "Alghoritm will estimate screen movement, but not objects" },
                },
                 ADDITIONAL_PARAMS_SET = new[]{
                    new[]{
                        new ADDITIONAL_PARAM(PID._MotionBlurRotateAmount,"Blur Rotate","",ADDITIONAL_PARAM_TYPE.@float){ min = 0, max = 5f},
                         new ADDITIONAL_PARAM(PID._MotionBlurMoveAmount,"Blur Movements","", ADDITIONAL_PARAM_TYPE.@float){ min = 0, max = 5f},
                         new ADDITIONAL_PARAM(PID._MotionBlurPhaseShift,"Phase Shift","", ADDITIONAL_PARAM_TYPE.@float){ min = -0.5f, max = 0.5f},
                    }
                },
                 samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_F_MOTION_BLUR_FAST }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                 }
    },
                   new SHADER_FEATURE() {
                S_TITLE = new[]{"Ultra Fast SSAO", "This module allows to apply fast stylized presets for camera colors"},
                S_TIP_TEXT = "This is author's ssao pretty fast simple variant that doesn't use pow max sqrt if or loop stuff",
                HELP_IMAGE = null,
                default_feature = 3,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_SIMPLE_SSAO_3, "3 Samples", "Unique EM algorithm uses only 3 texture samples" },
                    new[]{ KW.USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION, "3 Samples - No Normals Bias", "You can use it if you have any issues with ground for long distances" },
                    new[]{ KW.USE_SIMPLE_SSAO_4, "4 Samples", "Unique EM algorithm uses 4 texture samples" },
                    new[]{ KW.USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION, "4 Samples + Distance Fixer", "You can use it if you have any issues with ground for long distances" },
                },
                ADDITIONAL_PARAMS_F = ()=> new[]{
                    _ssao_params_a.Concat(_ssao_params_c).ToArray(),
                    _ssao_params_a.ToArray(),
                    _ssao_params_a.Concat(_ssao_params_d).ToArray(),
                    _ssao_params_a.Concat(_ssao_params_d).Concat(_ssao_params_b).ToArray(),
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_3}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_3}, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_4}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_4}, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },

                },
                samples_count_to_title = new[]
                {
                    new []{ KW.USE_SIMPLE_SSAO_3, "3"},new []{ KW.USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION, "3"},
                    new []{ KW.USE_SIMPLE_SSAO_4, "4"},new []{ KW.USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION, "4"},
                }
        },
                     new SHADER_FEATURE() {
                S_TITLE = new[]{ "Fog Distance", "This module allows to apply fast stylized presets for camera colors"},
                S_TIP_TEXT = "You can replace the standard fog with a new our variant, and use texture",
                HELP_IMAGE = null,
                default_feature = 0,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_SHADER_DISTANCE_FOG, "Simple", "Use one LUT preset with blending adjustment" },
                    new[]{ KW.USE_SHADER_DISTANCE_FOG_WITH_TEX, "Use Texture", "Use one LUT preset with blending adjustment" },
                    new[]{ KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION, "Animate Texture", "Use one LUT preset with blending adjustment" },
                },
                ADDITIONAL_PARAMS_F = ()=> new[]{
                    FOG_DISTANCE_PARAMS_A.ToArray(),
                      FOG_DISTANCE_PARAMS_A.Concat(FOG_COMMON_PARAMS_B).ToArray(),
                        FOG_DISTANCE_PARAMS_A.Concat(FOG_COMMON_PARAMS_B.Concat( FOG_COMMON_PARAMS_C)).ToArray(),
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },

                      new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX }, TARGET_SAMPLES_FEATURE = () => NOISE_TEXTURE_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION }, TARGET_SAMPLES_FEATURE = () => NOISE_TEXTURE_FEATURE },

                },
                samples_count_to_title = new[]
                {
                    new []{ KW.USE_SHADER_DISTANCE_FOG_WITH_TEX, "1"},
                    new []{ KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION, "2"}
                }
        },
                       new SHADER_FEATURE() {
                S_TITLE = new[]{ "Fog Height", "This module allows to apply fast stylized presets for camera colors"},
                S_TIP_TEXT = "Pretty fast possibility to add fog using y position with texture",
                HELP_IMAGE = null,
                default_feature = 0,
                FEATURES_ON = new[]{
                    new[]{ KW.USE_GLOBAL_HEIGHT_FOG, "Simple", "Use one LUT preset with blending adjustment" },
                    new[]{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX, "Use Texture", "Use one LUT preset with blending adjustment" },
                    new[]{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION, "Animate Texture", "Use one LUT preset with blending adjustment" },
                },
                ADDITIONAL_PARAMS_F = ()=> new[]{
                    FOG_HEIGHT_PARAMS_A.ToArray(),
                      FOG_HEIGHT_PARAMS_A.Concat(FOG_COMMON_PARAMS_B).ToArray(),
                        FOG_HEIGHT_PARAMS_A.Concat(FOG_COMMON_PARAMS_B.Concat( FOG_COMMON_PARAMS_C)).ToArray(),
                },
                samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG}, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },

                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX }, TARGET_SAMPLES_FEATURE = () => NOISE_TEXTURE_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {  KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION }, TARGET_SAMPLES_FEATURE = () => NOISE_TEXTURE_FEATURE },
                },
                samples_count_to_title = new[]
                {
                    new []{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX, "0", KW.USE_SHADER_DISTANCE_FOG_WITH_TEX},
                    new []{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION, "0", KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION},
                    new []{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX, "1"},
                    new []{ KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION, "2"}
                }
        },


              

//

        };

        static readonly ADDITIONAL_PARAM[] GRADIENTS_RAMP = new[] {

          //  new ADDITIONAL_PARAM(new[] { "-", KW.GRADIENT_RAMP, }, new[] { "-", "-", }, "Use Gradient Ramp:", "Enable it if you want to use special colors", useassimpletoggle: true) ,

                              new ADDITIONAL_PARAM( new[]{  KW.GRADIENT_NONE, KW.GRADIENT_COLOR_ADD, KW.GRADIENT_COLOR_MULTY, },
                  new[]{ "Replace",  "Additive Mix", "Multiply Mix", } , "Blending", "You can choose different blending for original colors" ) ,
                       //  new ADDITIONAL_PARAM(PID._FinalGradientTexture,"Gradient Texture","", ADDITIONAL_PARAM_TYPE.action_for_gradient),
                      //   new ADDITIONAL_PARAM(PID._FinalGradientTest,"Gradient Test","", ADDITIONAL_PARAM_TYPE.TEXTURE),
                      //  new ADDITIONAL_PARAM(PID._GradiendDitherSize,"Dither Size","Size of spread of dithering",ADDITIONAL_PARAM_TYPE.@float){ min = 0f, max = 1f},
        };

        static readonly ADDITIONAL_PARAM[] GRADIENTS_1_BIT = new[] {
                       //  new ADDITIONAL_PARAM(PID._1_GradiendBrightness,"Amount","", ADDITIONAL_PARAM_TYPE.@float){ min = 0.0f, max = 2f},
                        new ADDITIONAL_PARAM(PID._1_GradiendOffset,"Offset","",ADDITIONAL_PARAM_TYPE.@float){ min = 0.2f, max = 5f},//{ min = 0.1f, max = 10f},
                        new ADDITIONAL_PARAM(PID._BayerMultyply,"_BayerMultyply","",ADDITIONAL_PARAM_TYPE.@float){ min = 0.0f, max = 2f}//{ min = 0.1f, max = 10f},
        };
        static readonly ADDITIONAL_PARAM[] GRADIENTS_M = new[] {
                     //    new ADDITIONAL_PARAM(PID._M_GradiendBrightness,"Amount","", ADDITIONAL_PARAM_TYPE.@float){ min = 0.0f, max = 1f},
                        new ADDITIONAL_PARAM(PID._M_GradiendOffset,"Offset","",ADDITIONAL_PARAM_TYPE.@float){ min = 0.2f, max = 5f}//{ min = 0.1f, max = 10f},
        };
        static readonly ADDITIONAL_PARAM[] GRADIENTS_PARAMS_TEX = new[] {
                         new ADDITIONAL_PARAM(PID._DitherTexture,"Dither Texture","", ADDITIONAL_PARAM_TYPE.TEXTURE),
                        new ADDITIONAL_PARAM(PID._DitherSize,"Dither Texture Size","Size of texture",ADDITIONAL_PARAM_TYPE.@float)//{ min = 0.1f, max = 10f},
        };
        static readonly ADDITIONAL_PARAM[] GRADIENTS_PARAMS_LAST_SECTION = new ADDITIONAL_PARAM[0] {

                     //   new ADDITIONAL_PARAM(PID._DitherTexture,"Dither Texture","", ADDITIONAL_PARAM_TYPE.TEXTURE),
                     //  new ADDITIONAL_PARAM(PID._DitherSize,"Dither Texture Size","Size of texture",ADDITIONAL_PARAM_TYPE.@float),//{ min = 0.1f, max = 10f},




        };



        static readonly ADDITIONAL_PARAM[] FOG_DISTANCE_PARAMS_A = new[] {
                            new ADDITIONAL_PARAM( new[]{ "-", KW.USE_FOG_ADDITIVE_BLENDING, },new[]{ "Normal Blending", "Additive Blending", } , "Blending:", "Enable additive mod if you want to fog looks like sun shining or something like that" ) ,
                            new ADDITIONAL_PARAM( new[]{ "-", KW.FOG_BEFORE_LUT, },new[]{ "Fog After LUTs/Colors", "Fog Before LUTs/Colors", } , "Apply:", "Enable before LUTs if you want apply addition style for fog, but now, it is not awaliable to change order for other effects" ) ,
                            new ADDITIONAL_PARAM( PID._USE_HDR_FOR_FOG, "Add Glow Visibility", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , ADDITIONAL_PARAM_TYPE.toogle_as_float, HELP_KEY: "HELPTIP_GLOW_FOG_VIS") ,
                            new ADDITIONAL_PARAM( PID._SKIP_FOG_FOR_BACKGROUND, "Skip Fog For Sky", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , ADDITIONAL_PARAM_TYPE.toogle_as_float) ,


                        new ADDITIONAL_PARAM(PID._StartZ,"Fog Z Position","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.MinMaxSlider){ KEY2 = PID._EndZ, min =  -100, max = 10000},
                            new ADDITIONAL_PARAM(PID._ZFogDensity,"Common Fog Density","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float){min = 1 , max = 15},
                        new ADDITIONAL_PARAM(PID._FogColor,"Common Fog Color","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.ColorField),


                 };

        static readonly ADDITIONAL_PARAM[] FOG_HEIGHT_PARAMS_A = new[] {
                            new ADDITIONAL_PARAM( new[]{ "-", KW.USE_FOG_ADDITIVE_BLENDING, },new[]{ "Normal Blending", "Additive Blending", } , "Blending:", "Enable additive mod if you want to fog looks like sun shining or something like that" ) ,
                            new ADDITIONAL_PARAM( new[]{ "-", KW.FOG_BEFORE_LUT, },new[]{ "Fog After LUTs/Colors", "Fog Before LUTs/Colors", } , "Apply:", "Enable before LUTs if you want apply addition style for fog, but now, it is not awaliable to change order for other effects" ) ,
                            new ADDITIONAL_PARAM( PID._USE_HDR_FOR_FOG, "Add Glow Visibility", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , ADDITIONAL_PARAM_TYPE.toogle_as_float, HELP_KEY: "HELPTIP_GLOW_FOG_VIS") ,
                            new ADDITIONAL_PARAM( PID._SKIP_FOG_FOR_BACKGROUND, "Skip Fog For Sky", "If enabled, LUTs will be applied only for dark areas, bright parts of the screen will use camera colors" , ADDITIONAL_PARAM_TYPE.toogle_as_float) ,


                        new ADDITIONAL_PARAM(PID._StartY,"Fog Y Position","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.MinMaxSliderInverse){ KEY2 = PID._EndY, min =  -100, max = 10000},
                        new ADDITIONAL_PARAM(PID._HeightFogDensity,"Common Fog Density","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.@float){min = 1 , max = 15},
                        new ADDITIONAL_PARAM(PID._FogColor,"Common Fog Color","Choose one of thousands LUTs",ADDITIONAL_PARAM_TYPE.ColorField),

   };

        static readonly ADDITIONAL_PARAM[] FOG_COMMON_PARAMS_B = new[] {
                          //  new ADDITIONAL_PARAM( new[]{ "-", KW.USE_FOG_ADITIVE_BLENDING, },new[]{ "-", "-", } , "Calc Immersion Colors For Noise Texture:", "Then when the camera is in the light areas of a noise texture, the fog will have a more stronger effect", useassimpletoggle:true ) ,
                         new ADDITIONAL_PARAM(PID._NoiseTexture,"Noise Texture","LUTs Manager allows to change LUTs textures using handy interface", ADDITIONAL_PARAM_TYPE.TEXTURE) { action = () => { PostPresets1000Window.Init(); } },
                        new ADDITIONAL_PARAM(PID._FogTexTile,"Noise Texture Tile","Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.vectorXY),
           };
        static readonly ADDITIONAL_PARAM[] FOG_COMMON_PARAMS_C = new[] {
        new ADDITIONAL_PARAM(PID._FogTexAnimateSpeed, "Noise Texture Animation Speed", "Choose one of thousands LUTs", ADDITIONAL_PARAM_TYPE.@float) { min = 0, max = 30} ,
           };











        static SHADER_FEATURE[] _RIGHT_BAR_FEATURES;
        static SHADER_FEATURE[] RIGHT_BAR_FEATURES {
            get {
                if (_RIGHT_BAR_FEATURES == null) _RIGHT_BAR_FEATURES = new[] { GLOW_SAMPLES, BLUR_SAMPLES, DITHER_FEATURE, Z_FEATURE, NOISE_TEXTURE_FEATURE, DETAIL_TEXTURE_FEATURE };
                return _RIGHT_BAR_FEATURES;
            }
        }



        static SHADER_FEATURE GLOW_SAMPLES { get { return _GLOW_SAMPLES; } }
        static readonly SHADER_FEATURE _GLOW_SAMPLES = new SHADER_FEATURE() {
            S_TITLE = new[] { "Glow Samples", "This is special texture samples operations that uses for blurring camera images (a large number of texture, greatly slows down the shader)" },
            default_feature = 1,
            FEATURES_ON = new[] {
                new[] { KW.GLOW_SAMPLES_NO_DISTANCE, "Blur", "in this case pixels will not be adjusted according with the distance between camera and position in the world, and small objects located far from the camera may give very large glow"  } ,
                new[] { KW.GLOW_SAMPLES_SIZE_ON_Z, "Blur + Distance Z Fix", "If you are using 'Blur + Distance Fix' size for each pixel will be adjusted according with the distance between camera and position in the world (it is requires Z depth sample)" } ,
            }, //Standard Brightness Control Option
            ADDITIONAL_PARAMS_SET = new[]{
                            new[]{
                            new ADDITIONAL_PARAM(PID._glowSamples_BlurRadius,"Radius","Radius",ADDITIONAL_PARAM_TYPE.vectorXY),
                           // new ADDITIONAL_PARAM( new[]{ KW.GLOW_SAMPLES_BASE_BLUR_IN_FRAG_SIZE_ON_Z, KW.GLOW_SAMPLES_BASE_BLUR_IN_VERT_SIZE_OFF_Z, },new[]{ "Calc Size Per Pixel", "Skip Z (Fast)" }, "Z Depth", "" ) ,
                            new ADDITIONAL_PARAM( new[]{ KW.GLOW_SAMPLES_1, KW.GLOW_SAMPLES_4, KW.GLOW_SAMPLES_8, KW.GLOW_SAMPLES_16, },new[]{ "+1", "+2", "+4", "+6", } , "Glow Samples Count", "It is better to use minimum samples count" ) ,
                            new ADDITIONAL_PARAM( new[]{ KW.GLOW_DIRECTION_XY, KW.GLOW_DIRECTION_BI, KW.GLOW_DIRECTION_X, KW.GLOW_DIRECTION_Y, },new[]{ "Box", "Cross", "X", "Y" }, "Style", "You can choose blur only along horizontal or vertical axes" ) ,
                           // new ADDITIONAL_PARAM( KW.USE_GLOW_SAMPLE_DITHER, "Use Glow Samples Diffusion", "Diffusion makes small color displacements in the pixel of color transition" , HELP_KEY: "HELPTIP_DITHER") ,
                            new ADDITIONAL_PARAM( PID._glowSamples_DitherSize, "Diffusion Factor", "How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.@float ){min = 0.4f, max = 10}
                    },
        },

            samples_pointes = new[]  {
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  { KW.GLOW_SAMPLES_SIZE_ON_Z }, inverce = false, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {   "TRUE"  }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },
 },
            samples_count_to_title = new[]
            {
                new []{ KW.GLOW_SAMPLES_1 , "1"},new []{ KW.GLOW_SAMPLES_4 , "2"},new []{ KW.GLOW_SAMPLES_8 , "4"},new []{ KW.GLOW_SAMPLES_16 , "6"},
            }
        };
        static SHADER_FEATURE BLUR_SAMPLES { get { return _BLUR_SAMPLES; } }
        static readonly SHADER_FEATURE _BLUR_SAMPLES = new SHADER_FEATURE() {
            S_TITLE = new[] { "DOF Samples", "This is special additional texture samples operations that uses for blurring camera images  (a large number of texture, greatly slows down the shader)" },
            //FEATURES_ON = new[] { new[] { KW.USE_BRIGHTNESS_FUNCTIONS, "", "" } }, //Standard Brightness Control Option
            ADDITIONAL_PARAMS_SET = new[]{
                            new[]{
                            new ADDITIONAL_PARAM(PID._blurSamples_BlurRadius,"Radius","Radius",ADDITIONAL_PARAM_TYPE.vectorXY),
                            new ADDITIONAL_PARAM( new[]{ KW.BLUR_SAMPLES_1, KW.BLUR_SAMPLES_4, KW.BLUR_SAMPLES_8, KW.BLUR_SAMPLES_16, },new[]{ "+1", "+2", "+4", "+6", } , "DOF Samples Count", "It is better to use minimum samples count" ) ,


                            //    new ADDITIONAL_PARAM( KW.USE_BLUR_SAMPLE_DITHER, "Use Blur Samples Diffusion", "Diffusion makes small displacements in the pixel of color transition" , HELP_KEY: "HELPTIP_DITHER") , //, enabled_feature:KW.USE_BLUR_SAMPLE_DITHER
                            new ADDITIONAL_PARAM( PID._blurSamples_DitherSize, "Diffusion Factor", "How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.@float ){min = 0.5f, max = 10}
                   },
        },
            samples_pointes = new[]  {

                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  { "TRUE"  }, TARGET_SAMPLES_FEATURE = () => Z_FEATURE },
                    new ADDITIONAL_SAMPLES_POINTER(){CONDITIONS = new[]  {   "TRUE"  }, TARGET_SAMPLES_FEATURE = () => DITHER_FEATURE },

                },
            samples_count_to_title = new[]
            {
                new []{ KW.BLUR_SAMPLES_1, "1"},new []{ KW.BLUR_SAMPLES_4, "2"},new []{ KW.BLUR_SAMPLES_8, "4"},new []{ KW.BLUR_SAMPLES_16, "6"},
            }
        };

        static SHADER_FEATURE DITHER_FEATURE { get { return _DITHER_FEATURE; } }
        static readonly SHADER_FEATURE _DITHER_FEATURE = new SHADER_FEATURE() {
            S_TITLE = new[] { "Diffusion Offset", "This feature allows you to adjust brightness, contrasts and saturation" },
            default_feature = 0,
            /*  FEATURES_ON = new[] {
                  new[] { KW.USE_GLOW_BLUR_IN_FRAG_SIZE_ON_Z, "", "" } ,
                  new[] { KW.USE_GLOW_BLUR_IN_VERT_SIZE_OFF_Z, "", "" } ,
              }, //Standard Brightness Control Option*/
            ADDITIONAL_PARAMS_SET = new[]{
                            new[]{

                            new ADDITIONAL_PARAM( new float[]{ 20.58f, 245.511f , 90.58f, 0.2220f }, PID._spread_Variatnt, new[]{ "v1", "v2", "v3", "v4" } ,
                           // new ADDITIONAL_PARAM( new float[]{ 90.58f, 20.58f,  /*0.4f*/  143.511f, 0.2220f, }, PID._spread_Variatnt, new[]{ "v1", "v2", "v3", "v4" } ,
                                "Diffusion Pattern", "You can change noise pattern for diffusion" ) ,
                           // new ADDITIONAL_PARAM(PID._spread_Variatnt,"_spread_Variatnt","Bright amount",ADDITIONAL_PARAM_TYPE.toolbar_as_float),
                            new ADDITIONAL_PARAM(PID._spread_AnimateSpeedMulti,"Animation Speed","The animation will look like a little noise",ADDITIONAL_PARAM_TYPE.@float),
                    }, //0.4 0.63 0.2220
        }
        };
        static SHADER_FEATURE Z_FEATURE { get { return _Z_FEATURE; } }
        static readonly SHADER_FEATURE _Z_FEATURE = new SHADER_FEATURE() {
            S_TITLE = new[] { "Use Z Depth", "This feature allows you to adjust brightness, contrast and saturation" },
            default_feature = 1,
            /* FEATURES_ON = new[] {
                 new[] { KW.USE_GLOW_BLUR_IN_FRAG_SIZE_ON_Z, "", "" } ,
                 new[] { KW.USE_GLOW_BLUR_IN_VERT_SIZE_OFF_Z, "", "" } ,
             }, //Standard Brightness Control Option*/
            samples_count_to_title = new[]
            {
                new []{ "TRUE", "1"},
            }
        };


        static SHADER_FEATURE NOISE_TEXTURE_FEATURE { get { return _NOISE_TEXTURE_FEATURE; } }
        static readonly SHADER_FEATURE _NOISE_TEXTURE_FEATURE = new SHADER_FEATURE() {
            S_TITLE = new[] { "Use Noise Texture", ", if you are using your own texture set 1 channel option in the texture import settings" },
            S_TIP_TEXT = "This texture uses only 1 channel",
            default_feature = 1,
            /*  FEATURES_ON = new[] {
                  new[] { KW.USE_GLOW_BLUR_IN_FRAG_SIZE_ON_Z, "", "" } ,
                  new[] { KW.USE_GLOW_BLUR_IN_VERT_SIZE_OFF_Z, "", "" } ,
              }, //Standard Brightness Control Option*/
            ADDITIONAL_PARAMS_SET = new[]{
                            new[]{
                            new ADDITIONAL_PARAM(PID._NoiseTexture,"_NoiseTexture","Bright amount",ADDITIONAL_PARAM_TYPE.TEXTURE),
                    },
        }
        };

        static SHADER_FEATURE DETAIL_TEXTURE_FEATURE { get { return _DETAIL_TEXTURE_FEATURE; } }
        static readonly SHADER_FEATURE _DETAIL_TEXTURE_FEATURE = new SHADER_FEATURE() {
            S_TITLE = new[] { "Use Detail Texture", "This feature allows you to adjust brightness, contrast and saturation" },
            default_feature = 1,
            /*  FEATURES_ON = new[] {
                  new[] { KW.USE_GLOW_BLUR_IN_FRAG_SIZE_ON_Z, "", "" } ,
                  new[] { KW.USE_GLOW_BLUR_IN_VERT_SIZE_OFF_Z, "", "" } ,
              }, //Standard Brightness Control Option*/
            ADDITIONAL_PARAMS_SET = new[]{
                            new[]{
                            new ADDITIONAL_PARAM(PID._DetailTexture,"_DetailTexture","Bright amount",ADDITIONAL_PARAM_TYPE.TEXTURE),
                    },
        }
        };
        //uniform fixed _blurRadius;
        //uniform fixed _spread_AnimateSpeedMulti;// = 1;




        /*
#if USE_GLOW_4 || USE_GLOW_8 || USE_GLOW_16
        uniform fixed _glowContrast;
        uniform fixed _glowBrightness;
        uniform fixed _glowTreshold;
#if USE_DOTTED_GLOW
#endif
#endif

#if USE_OUTLINE_STROKES || USE_OUTLINE_COLORED_STROKES
        uniform fixed _outlineStrokesBlend;
        uniform fixed _outlineStrokesScale;
        uniform fixed4 _outlineStrokesColor;
#endif


#if USE_SIMPLE_SSAO || USE_SIMPLE_SSAO_DISTANCE_CORRECTION
        uniform fixed _aoRadius;// = 0.5;
        uniform fixed _aoAmount;// = 2;
        uniform fixed _aoBlend;// = 2;
        uniform fixed _aoTheshold;// = 0.1; //0.0001 0.5
        uniform fixed _aoGroundBias;// = 1; //0.0001 0.5
        uniform fixed _aoStrokesSpread;// = 1;
#endif

#if USE_SHADER_DISTANCE_FOG || USE_GLOBAL_HEIGHT_FOG || USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
#if USE_GLOBAL_HEIGHT_FOG_WITH_TEX || USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION || USE_SHADER_DISTANCE_FOG_WITH_TEX || USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION
        UNITY_DECLARE_SCREENSPACE_TEXTURE(_NoiseTexture);
#endif

        uniform fixed _USE_HDR_FOR_FOG;
        uniform fixed _SKIP_FOG_FOR_BACKGROUND;
        uniform fixed _StartZ;
        uniform fixed _EndZ;
        uniform fixed4 _FogColor;
        uniform fixed _StartY;
        uniform fixed _EndY;
        uniform fixed _FogDensity;
        uniform fixed4 _FogParams;
        uniform fixed _FogTexAnimateSpeed;
        uniform fixed2 _FogTexTile;
#endif

        */

        //float separator_factor = 1.5f;



        internal Func<CameraSettings> GET_CAM_SET {
            get {
#if FAST_POSTPROCESSING_URP_USED
                if (GraphicsSettings.renderPipelineAsset is UniversalRenderPipelineAsset) return GetCamSetUrp;
#endif
                return GetCamSetDef;
            }
        }
        internal Action<CameraSettings> SET_CAM_SET {
            get {
#if FAST_POSTPROCESSING_URP_USED
                if (GraphicsSettings.renderPipelineAsset is UniversalRenderPipelineAsset) return SetCamSetUrp;
#endif
                return SetCamSetDef;
            }
        }

        CameraSettings _s_null;
        CameraSettings _s;
        private CameraSettings GetCamSetUrp()
        {
#if FAST_POSTPROCESSING_URP_USED
            var rp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (!rp) return _s_null;
            _s.msaa = rp.msaaSampleCount;
            _s.depthEnabled = rp.supportsCameraDepthTexture;
            _s.defaultPostProcessing = null;
            // var f = CHECK_RENDER_FEATURE();
            //if (f.Length != 0) _s.defaultPostProcessing = f[0]
            //[SerializeField] internal ScriptableRendererData[] m_RendererDataList = new ScriptableRendererData[1];
#endif
            return _s;
        }
        private void SetCamSetUrp(CameraSettings s)
        {
#if FAST_POSTPROCESSING_URP_USED
            var rp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (!rp) return;
            Undo.RecordObject(rp, "Set Camera Settings");
            if (s.msaa.HasValue)
            {
                rp.msaaSampleCount = s.msaa.Value;
                if (FastPostProcessingCamera.GET_ACTIVE_CAMERA)
                {
                    Undo.RecordObject(FastPostProcessingCamera.GET_ACTIVE_CAMERA, "Set Camera Settings");
                    FastPostProcessingCamera.GET_ACTIVE_CAMERA.allowMSAA = rp.msaaSampleCount != 0;
                    EditorUtility.SetDirty(FastPostProcessingCamera.GET_ACTIVE_CAMERA);
                }
                QualitySettings.antiAliasing = rp.msaaSampleCount;
            }
            if (s.depthEnabled.HasValue)
            {
                rp.supportsCameraDepthTexture = s.depthEnabled.Value;
            }
            _s.depthEnabled = rp.supportsCameraDepthTexture;
            EditorUtility.SetDirty(rp);
#endif
        }
        private CameraSettings GetCamSetDef()
        {
            _s.msaa = QualitySettings.antiAliasing;
            _s.depthEnabled = null;
            _s.defaultPostProcessing = null;
            return _s;
        }
        private void SetCamSetDef(CameraSettings s)
        {
            if (s.msaa.HasValue)
            {
                if (FastPostProcessingCamera.GET_ACTIVE_CAMERA)
                {
                    Undo.RecordObject(FastPostProcessingCamera.GET_ACTIVE_CAMERA, "Set Camera Settings");
                    FastPostProcessingCamera.GET_ACTIVE_CAMERA.allowMSAA = s.msaa.Value != 0;
                    EditorUtility.SetDirty(FastPostProcessingCamera.GET_ACTIVE_CAMERA);
                }
                QualitySettings.antiAliasing = s.msaa.Value;
            }
            if (s.depthEnabled.HasValue)
            {
                //rp.supportsCameraDepthTexture = s.depthEnabled.Value;
            }
        }

        string[] msaa_names = null;
        int[] msaa_values = null;
        internal FastPostProcessingProfile SCRIPT;
        Vector2? start_drag_pos;
        public void OnInspectorGUI(Settings settings, UnityEngine.Object obToUndo, Action Repaint)
        {


            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;


            if (GraphicsSettings.renderPipelineAsset && FastPostProcessingProfile.DefaultData.CanBeSwitchToURP_raw)
            {
                EditorGUILayout.HelpBox("Warning! You are using URP but this asset didn't switch to URP", MessageType.Error);
                if (GUILayout.Button("Switch Asset to URP"))
                {
                    FastPostProcessingProfile.DefaultData.SwitchToURP();
                }
            }



            var camset = GET_CAM_SET();
            if (camset.msaa.HasValue)
            {
                var max_msaa = Mathf.Max(camset.msaa.Value, 8);
                if (msaa_names == null)
                {
                    msaa_names = new string[1];
                    msaa_values = new int[max_msaa + 1];
                    msaa_names[0] = "Disabled";
                    msaa_values[0] = 1;
                    for (int i = 2, _i = 1; i <= max_msaa; i *= 2, _i++)
                    {
                        Array.Resize(ref msaa_names, msaa_names.Length + 1);
                        msaa_names[_i] = i + "x";
                        msaa_values[_i] = i;
                    }
                }
                var old_msaa = 10;
                if (camset.msaa < 2) old_msaa = 0;
                for (int i = 2, _i = 1; i <= max_msaa; i *= 2, _i++) if (i == camset.msaa) old_msaa = _i;

                var new_msaa = EditorGUILayout.Popup("MSAA", old_msaa, msaa_names);
                if (old_msaa != new_msaa) camset.msaa = Mathf.Max(1, msaa_values[new_msaa]);
            }




            // if (camset.depthEnabled.HasValue) camset.depthEnabled = EditorGUILayout.ToggleLeft("Depth Texture", camset.depthEnabled.Value);
            // if (camset.defaultPostProcessing.HasValue) camset.defaultPostProcessing = EditorGUILayout.ToggleLeft("Default PostProcessing", camset.defaultPostProcessing.Value);
            if (!camset.Equals(GET_CAM_SET())) SET_CAM_SET(camset);
            //if (!camset.wasInit)
            //{
            //    var s = settings.GET_CAM_SET();
            //    camset.msaa = 
            //}




            var old_target = settings.RenderPassEvent;
            var new_target = (PostRenderPassEvent)EditorGUILayout.EnumPopup("Render Order", old_target);
            if (old_target != new_target)
            {
                Undo.RecordObject(obToUndo, "Change Profile");
                settings.RenderPassEvent = new_target;
                EditorUtility.SetDirty(obToUndo);
                AssetDatabase.SaveAssets();
            }

            if (settings.Profile)
            {
                var old_d = settings.Profile.FORCE_DISABLE_Z;
                var new_d = EditorGUILayout.ToggleLeft("Force Disable Z Depth Render", old_d);
                if (old_d != new_d)
                {
                    Undo.RecordObject(settings.Profile, "Change Profile");
                    settings.Profile.FORCE_DISABLE_Z = new_d;
                    EditorUtility.SetDirty(settings.Profile);
                    AssetDatabase.SaveAssets();
                }
                if (new_d)
                {
                    EditorGUILayout.HelpBox("Warning depth texture rendering disabled!", MessageType.Warning);
                }
            }


            var old_p = settings.Profile;
            var c = GUI.color;
            if (!old_p)
            {
                GUILayout.Space(20);
                GUI.color *= Color.red;
                GUILayout.Label("Please Choose a Profile");
            }
            var new_p = EditorGUILayout.ObjectField("Profile:", old_p, typeof(FastPostProcessingProfile), false) as FastPostProcessingProfile;
            if (new_p != old_p)
            {
                Undo.RecordObject(obToUndo, "Change Profile");
                settings.Profile = new_p;
                EditorUtility.SetDirty(obToUndo);
                AssetDatabase.SaveAssets();
            }
            if (!old_p)
            {
                GUI.color = c;
                return;
            }


            SCRIPT = settings.Profile;
            if (!SCRIPT || !SCRIPT.TargetMaterial) return;


            var r = EditorGUILayout.GetControlRect();
            r.height = 99999;
            r.width += r.x;
            r.x = 0;
            GUI.BeginClip(r);
            OnInspectorGUI(Repaint);
            GUI.EndClip();
        }
        public void OnInspectorGUI(Action Repaint)
        {

            // FastPostProcessingProfile SCRIPT = settings.Profile;
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
            if (!SCRIPT || !SCRIPT.TargetMaterial) return;

            current_group = -1;


            foreach (var item in RIGHT_BAR_FEATURES)
            {
                item.is_right_bar_features_used = 0;
                item.is_additional_features_used = 0;
            }


            var first_full_rect = R();
            first_full_rect.y = 0;
            //Debug.Log(first_full_rect.y + " " + Event.current.type);

            var start = first_full_rect.y;
            var headRect = first_full_rect;
            headRect.x += PAD_LEFT;
            headRect.y += 12;
            headRect.width -= PAD_LEFT * 2;
            //headRect.width *= SCRIPT.COLUMN_DACTOR;
            headRect.width /= 3;
            headRect.height = HEAD_HEIGHT;
            //  string[] categories = new string[3] { "LUTs / Colors", "Glow / Blur", "Post Effects" };
            string[] categories = new string[3] { "LUTs / Colors", "Image Effects", "Other Effects" };
            for (int i = 0; i < 3; i++)
            {
                EditorGUIUtility.AddCursorRect(headRect, MouseCursor.Link);
                var on = i == SCRIPT.selected_category;
                GUIStyle ON_STYLE = on ? ST_TOGGLE_ON : ST_TOGGLE_OFF;
                if (GUI.Button(headRect, CONT(categories[i], ""), ON_STYLE))
                {
                    Undo.RecordObject(SCRIPT, "Change Material Value");
                    SCRIPT.selected_category = i;
                    EditorUtility.SetDirty(SCRIPT);
                }
                headRect.x += headRect.width;
            }
            var _HEAD_HEIGHT = headRect.y + headRect.height + 12;
            // GUILayout.Space(_HEAD_HEIGHT - first_full_rect.y);
            first_full_rect.y += _HEAD_HEIGHT;
            //var full_w = EditorGUIUtility.currentViewWidth - 20;
            //var left_w = full_w / 1.5f;
            var cr = first_full_rect;
            //float main_h = 0;
            cr.height = 9999;
            cr.width *= SCRIPT.COLUMN_FACTOR;
            var old_rect = cr;
            GUI.BeginClip(cr);
            cr.y = cr.x = 0;

            foreach (var item in FE_COLORS) cr.y += CAT(cr, item, SCRIPT.selected_category != 0);
            foreach (var item in FE_GLOW_BLUR) cr.y += CAT(cr, item, SCRIPT.selected_category != 1);
            foreach (var item in FE_EFFECTS) cr.y += CAT(cr, item, SCRIPT.selected_category != 2);
            GUI.EndClip();

            //GUILayout.Space(cr.y );
            // var last_rect = R();

            var old_W = first_full_rect.width * SCRIPT.COLUMN_FACTOR;
            var r = first_full_rect;

            float DRAG_W = Math.Max(10, first_full_rect.width / 25);

            var drag_rect = new Rect(first_full_rect.x + old_W, first_full_rect.y, DRAG_W, old_rect.y + cr.y - first_full_rect.y);
            r.x += old_W + DRAG_W;
            r.width -= old_W + DRAG_W;
            r.y += 30;




            r.height = 9999;
            r.width -= 10;
            var new_rect = r;
            r.width -= 10;
            GUI.BeginClip(r);
            r.y = r.x = 0;
            gl_pos = 0;

            r.height = HEAD_HEIGHT;
            GUI.Label(r, CONT("Additional Options:", "These options may be used simultaneously for different parts of the shader, so they are placed in a separate area"), ST_CAT_NAME);
            r.y += 10;
            r.y += r.height;


            foreach (var item in RIGHT_BAR_FEATURES)
            {
                var h = RB_FEAT(item, r, old_rect, new_rect);
                //GUILayout.Space(h);
                r.y += h;
            }
            GUI.EndClip();

            if (gl_pos != 0)
            {
                GL_BEGIN();
                for (int i = 0; i < gl_pos; i += 2)
                {
                    var tp1 = gl_push[i];
                    tp1.x += new_rect.x;
                    tp1.y += new_rect.y;
                    var tp2 = gl_push[i + 1];
                    tp2.x += new_rect.x;
                    tp2.y += new_rect.y;
                    GL.Color(dad);
                    GL_VERTEX3(tp1);
                    GL_VERTEX3(tp2);
                    ALIAS(tp1, tp2, dad);
                }
                GL_END();
            }


            //var addSpace = Mathf.Max(0, old_rect.y + cr .y - (new_rect.y + r.y));
            //if (addSpace != 0) GUILayout.Space(addSpace);


            var HH = Mathf.Max((old_rect.y + cr.y - start), new_rect.y + r.y - start);
            drag_rect.height = HH - 20;


            EditorGUIUtility.AddCursorRect(drag_rect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                start_drag_pos = null;
                Repaint();
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && drag_rect.Contains(Event.current.mousePosition))
            {
                start_drag_pos = Event.current.mousePosition;
                Repaint();
            }
            if (start_drag_pos.HasValue && Event.current.type == EventType.MouseDrag)
            {
                var now = (Event.current.mousePosition.x - start_drag_pos.Value.x); //- first_full_rect.x
                var full = first_full_rect.width;
                var lerp = (now / full);
                if (Mathf.Abs(lerp) > 0.00001f)
                {
                    Undo.RecordObject(SCRIPT, "Change Material Value");
                    var res = Mathf.Clamp(SCRIPT.COLUMN_FACTOR + lerp, DRAG_W / first_full_rect.width, (first_full_rect.width - DRAG_W) / first_full_rect.width);
                    SCRIPT.COLUMN_FACTOR = res;
                    EditorUtility.SetDirty(SCRIPT);
                    start_drag_pos = Event.current.mousePosition;
                    Repaint();
                }

            }
            if (start_drag_pos.HasValue) EditorGUI.DrawRect(new Rect(drag_rect.x + drag_rect.width / 2 - 2, drag_rect.y, 2, drag_rect.height), new Color(1, 0, 0, 0.3f));


            if (Z_FEATURE.is_used != SCRIPT.Z_FEATURE_IS_USED)
            {
                SCRIPT.Z_FEATURE_IS_USED = Z_FEATURE.is_used;
                EditorUtility.SetDirty(SCRIPT);
            }

            GUILayout.Space(HH);

            //TOGGLE( KW.USE_LUT2, "Use Second LUT", "You can adjust blending between two different luts presets" );
            //TOGGLE( KW.DISABLE_LUTS_FOR_BRIGHT_AREAS, "Disable LUT effect for bright areas", "If the scene's color value is greater than the set maximum value, shader will keep scene's colors without LUTs changes" );
            //
            //TOGGLE( KW.USE_HDR_COLORS, "Use HDR Colors", "Some colors maybe greater than 1.0, if you will not use hdr colors, all values greater than 1.0 will be clamped (Using HDR colors doesn't reduce performance!)" );
            //
            //TOGGLE( KW.USE_BRIGHTNESS_FUNCTIONS, "Use Brightness Adjustments", "You can adjust brightness, saturation and contrast" );
            //
            //TOGGLE( new string[] { KW.USE_POSTERIZE_LUTS, KW.USE_POSTERIZE_IMPROVED, KW.USE_POSTERIZE_SIMPLE }, "Use Brightness Adjustments", "Posterization reduces the number of color shades, use it if you need to achieve a certain style" );
            //
            //TOGGLE( new string[] { KW.USE_GLOW_4, KW.USE_GLOW_8, KW.USE_GLOW_16 }, "Use Glow", "Try using as few samples as possible" );
            //TOGGLE( KW.USE_GLOW_BLEACH_STYLE, "Apply Bleach Style", "Bleach style uses multiplication operation with original camera image (Using bleach style doesn't reduce performance!)" );
            //TOGGLE( KW.USE_DOTTED_GLOW, "Apply Dotted Style", "Dotted style, a nice solution to improve the final style of the camera image (Using dotted style doesn't reduce performance!)" );
            //
            //
            //TOGGLE( new string[] { KW.USE_VINJETE_BLUR, KW.USE_DEPTH_OF_FIELD_SIMPLE, KW.USE_DEPTH_OF_FIELD_COMPLETE_4, KW.USE_DEPTH_OF_FIELD_COMPLETE_8, KW.USE_DEPTH_OF_FIELD_COMPLETE_16 }, "--", "--" );
            //
            //TOGGLE( KW.USE_Z_FOR_BLURING, "Z for bluring", "---" );
            //TOGGLE( KW.USE_OUTLINE_STROKES, "Outline Strokes", "--)" );
            //
            //
            //TOGGLE( new string[] { KW.USE_SHADER_DISTANCE_FOG, KW.USE_SHADER_DISTANCE_FOGWITH_TEX, KW.USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION }, "USE_SHADER_DISTANCE_FOG", "--" );
            //TOGGLE( new string[] { KW.USE_GLOBAL_HEIGHT_FOG, KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX, KW.USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION }, "USE_GLOBAL_HEIGHT_FOG", "--" );
            //TOGGLE( new string[] { KW.USE_SIMPLE_SSAO, KW.USE_SIMPLE_SSAO_DISTANCE_CORRECTION }, "USE_SIMPLE_SSAO", "--" );
            //TOGGLE( new string[] { KW.USE_ULTRA_FAST_SHARPEN, KW.USE_IMPROVED_SHARPEN }, "USE_ULTRA_FAST_SHARPEN", "--" );

        }


        string GET_ADDITIONAL_TITLE(string[][] source)
        {
            if (source.Length != 0)
            {
                foreach (var s in source)
                {
                    // if (s.Length > 2 && SCRIPT.TargetMaterial.IsKeywordEnabled(s[3])) continue;
                    if (s[0] != "-" && (s[0] == "TRUE" || SCRIPT.TargetMaterial.IsKeywordEnabled(s[0])))
                    {
                        if (s.Length < 3) return " (+" + s[1] + " samples)";
                        for (int i = 2; i < s.Length; i++)
                        {
                            if (!SCRIPT.TargetMaterial.IsKeywordEnabled(s[i])) break;
                            if (i == s.Length - 1) return " (+" + s[1] + " samples)";
                        }
                    }

                }
                if (source[0][0] == "-")
                    return " (+" + source[0][1] + " samples)";
            }
            return "";
        }

        readonly Vector3[] p = new Vector3[20];
        const float PAD_LEFT = 12;
        const float HEAD_HEIGHT = 23;
        Vector3[] gl_push = new Vector3[100];
        int gl_pos = 0;

        float RB_FEAT(SHADER_FEATURE item, Rect br, Rect old_rect, Rect new_rect)
        {
            var en = GUI.enabled;
            GUI.enabled &= item.is_right_bar_features_used != 0 || item.is_additional_features_used != 0;
            br.x += 0;
            br.width -= PAD_LEFT;

            current_group++;
            //var start = r.y;

            // BG
            // if (Event.current.type == EventType.Repaint)
            {
                //  var c = GUI.changed;
                // if (!GUI.enabled) GUI.color *= new Color(1, 1, 1, 0.3f);
                var gr_r = br;
                gr_r.height = groups_y[current_group];
                /* gr_r.width /= 2;
                 var old_w = gr_r.width;
                 gr_r.width = Mathf.Floor(old_w);
                 GUI.skin.textArea.Draw(gr_r, emptyContent, 0);
                 //ST_CAT_BG_LEFT.Draw(gr_r, emptyContent, 0);
                 gr_r.x += Mathf.Floor(old_w); ;
                 gr_r.width = Mathf.Ceil(old_w);
                 GUI.skin.textArea.Draw(gr_r, emptyContent, 0);*/
                //ST_CAT_BG_RIGHT.Draw(gr_r, emptyContent, 0);
                // GUI.color = c;
                if (Event.current.type == EventType.Repaint) GUI.skin.textArea.Draw(gr_r, emptyContent, 0);

            }
            groups_y[current_group] = 0;

            var start = br.y;
            br.y += 12;


            br.height = HEAD_HEIGHT;
            var lab = br;
            //lab.y += 5;





            string title_samples_count = GET_ADDITIONAL_TITLE(item.samples_count_to_title);
            GUI.Label(lab, CONT(item.S_TITLE, !GUI.enabled, title_samples_count), GUI.enabled ? ST_CAT_sm_NAME : ST_CAT_sm_NAME_OFF);
            var out_position = new Vector2(lab.x + lab.width, lab.y + lab.height / 2);
            br.y += br.height;



            GUI.enabled = en;
            if (item.is_right_bar_features_used == 0 && item.is_additional_features_used == 0)
            {
                item.is_used = false;
                br.y += 8;
                return groups_y[current_group] += br.y - start;
            }

            item.is_used = true;

            if (!string.IsNullOrEmpty((item.S_TIP_TEXT)))
            {
                br.y += 11;
                var hr = br;
                hr.x += PAD_LEFT;
                hr.width -= PAD_LEFT * 2;
                hr.height = br.height = EditorStyles.helpBox.CalcHeight(CONT(item.S_TIP_TEXT, item.S_TITLE), hr.width);
                if (Event.current.type == EventType.Repaint) EditorStyles.helpBox.Draw(hr, CONT(item.S_TIP_TEXT, item.S_TITLE), 0);
                br.y += br.height;
                br.y += 11;
            }


            if (Event.current.type == EventType.Repaint)
            {

                for (int i = 0; i < item.is_right_bar_features_used; i++)
                {
                    var from_pos = item.left_bar_out_poses[i];
                    if (from_pos == Vector2.zero) continue;
                    from_pos.x = from_pos.x + old_rect.x - new_rect.x;
                    from_pos.y = from_pos.y + old_rect.y - new_rect.y;
                    var top_pos = new Vector2(lab.x, lab.y + lab.height / 2);

                    if (i >= item.random_handler.Length) Array.Resize(ref item.random_handler, i + 3);
                    if (item.random_handler[i] == null) item.random_handler[i] = new RANDOM_HANDLER();
                    var rofp = item.random_handler[i].rofp;

                    var L = 3; //3
                    var D = 0f;
                    p[0] = from_pos;
                    p[L] = top_pos;
                    for (int _i = 1; _i < L; _i++) p[_i] = Vector3.Lerp(p[0], p[L], _i / (L - D));

                    for (int _i = 1; _i < 3; _i++)
                    {
                        p[_i].x += rofp[_i].x * 2.5f;
                        p[_i].y += rofp[_i].y * 2.5f;
                    }
                    p[1].y = Mathf.Lerp(p[1].y, p[0].y, 0.95f);
                    p[2].y = Mathf.Lerp(p[2].y, p[3].y, 0.95f);
                    var tvp = p[1].x;
                    p[1].x = p[2].x;
                    p[2].x = tvp;

                    //GL_BEGIN();
                    tp2 = p[0];
                    for (float t = 0; t < 1f; t += 0.05f)
                    {

                        tp1 = tp2;
                        tp2 = GetPoint((t), p);
                        if (tp2 == tp1 || tp2 == Vector3.zero || tp1 == Vector3.zero) continue;

                        if (gl_pos >= gl_push.Length) Array.Resize(ref gl_push, gl_push.Length + 100);
                        gl_push[gl_pos++] = tp1;
                        gl_push[gl_pos++] = tp2;
                        //GL.Color(dad);
                        //GL_VERTEX3(tp1);
                        //GL_VERTEX3(tp2);
                        //ALIAS(tp1, tp2, dad);
                    }
                    tp1 = tp2;
                    tp2 = p[L];

                    if (gl_pos >= gl_push.Length) Array.Resize(ref gl_push, gl_push.Length + 100);
                    gl_push[gl_pos++] = tp1;
                    gl_push[gl_pos++] = tp2;
                    // GL.Color(dad);
                    //GL_VERTEX3(tp1);
                    //GL_VERTEX3(tp2);
                    //ALIAS(tp1, tp2, dad);
                    //GL_END();
                }
            }


            br.x += PAD_LEFT;
            br.width -= PAD_LEFT * 2;
            int selected_add_i = 0;

            if (item.FEATURES_ON != null && item.FEATURES_ON.Length != 0)
            {
                var r = br;
                // r.x += br.width + 9;
                r.height = 32;
                // r.width = r.x + r.width - br.x - PAD_LEFT;
                Vector2 tv2 = Vector2.zero;

                out_position = new Vector2(br.x + br.width, br.y + br.height / 2);


                br.y += RAW_TOGGABLE_OPTIONS(r, item, ref out_position);


                // for (int i = 0; i < item.FEATURES_ON.Length; i++) 
                //     if (SCRIPT.TargetMaterial.IsKeywordEnabled(item.FEATURES_ON[i][0]))
                //     {
                //         selected_add_i = i;
                //         out_position = new Vector2(br.x + br.width, br.y + br.height / 2);
                //     }
            }

            br.y += 6;

            if (item.ADDITIONAL_PARAMS_GET.Length != 0)
            {
                var ADD = item.ADDITIONAL_PARAMS_GET[Mathf.Clamp(selected_add_i, 0, item.ADDITIONAL_PARAMS_GET.Length - 1)];
                foreach (ADDITIONAL_PARAM add in ADD)
                {
                    var en2 = GUI.enabled;
                    if (!string.IsNullOrEmpty(add.enabled_feature))
                    {
                        var ef = SCRIPT.TargetMaterial.IsKeywordEnabled(add.enabled_feature);
                        if (add.enabled_feature_inverse) ef = !ef;
                        GUI.enabled &= ef;
                    }
                    // if (GUI.enabled || !item.enable_full_falling)
                    // {
                    br.height = EditorGUIUtility.singleLineHeight;
                    DRAW_TYPE(ref br, add, skip_hier: true);
                    // }
                    GUI.enabled = en2;

                    br.y += br.height;
                }


            }


            if (item.samples_pointes != null && item.samples_pointes.Length != 0)
            {
                foreach (var sp in item.samples_pointes)
                {
                    bool allow = true;
                    foreach (var c in sp.CONDITIONS)
                    {
                        if (c == "TRUE") break;
                        var res = SCRIPT.TargetMaterial.IsKeywordEnabled(c);
                        if (sp.inverce)
                            res = !res;
                        if (!res)
                        {
                            allow = false;
                            break;
                        }
                    }
                    if (allow)
                    {
                        sp.TARGET_SAMPLES_FEATURE().addition_features_out_poses[sp.TARGET_SAMPLES_FEATURE().is_additional_features_used] = out_position;
                        sp.TARGET_SAMPLES_FEATURE().is_additional_features_used++;
                    }
                }

            }



            if (Event.current.type == EventType.Repaint)
            {

                for (int i = 0; i < item.is_additional_features_used; i++)
                {
                    var from_pos = item.addition_features_out_poses[i];
                    //from_pos.x = from_pos.x + old_rect.x - new_rect.x;
                    //from_pos.y = from_pos.y + old_rect.y - new_rect.y;
                    var top_pos = new Vector2(lab.x + lab.width, lab.y + lab.height / 2);

                    if (i >= item.random_handler.Length) Array.Resize(ref item.random_handler, i + 3);
                    if (item.random_handler[i] == null) item.random_handler[i] = new RANDOM_HANDLER();

                    p[0] = p[1] = from_pos;
                    p[2] = p[3] = top_pos;
                    //p[1].x += 10;
                    //p[2].x += 10;

                    var rofp = item.random_handler[i].rofp;
                    for (int _i = 1; _i < 3; _i++)
                    {
                        p[_i].x += -rofp[_i].x * 5f + 10;
                    }

                    //GL_BEGIN();
                    tp2 = p[0];
                    for (float t = 0; t < 1f; t += 0.1f)
                    {
                        tp1 = tp2;
                        tp2 = GetPoint((t), p);
                        if (tp2 == tp1 || tp2 == Vector3.zero || tp1 == Vector3.zero) continue;
                        //GL.Color(dad);
                        //GL_VERTEX3(tp1);
                        //GL_VERTEX3(tp2);
                        //ALIAS(tp1, tp2, dad);
                        if (gl_pos >= gl_push.Length) Array.Resize(ref gl_push, gl_push.Length + 100);
                        gl_push[gl_pos++] = tp1;
                        gl_push[gl_pos++] = tp2;
                    }
                    tp1 = tp2;
                    tp2 = p[3];
                    //GL.Color(dad);
                    //GL_VERTEX3(tp1);
                    //GL_VERTEX3(tp2);
                    //ALIAS(tp1, tp2, dad);
                    //GL_END();
                    if (gl_pos >= gl_push.Length) Array.Resize(ref gl_push, gl_push.Length + 100);
                    gl_push[gl_pos++] = tp1;
                    gl_push[gl_pos++] = tp2;
                }
            }

            //        FEATURES_ON = new[] {
            //        new[] { KW.GLOW_SAMPLES_BASE_BLUR_IN_FRAG_SIZE_ON_Z, "Calc Blur Size Per Pixel", "If you are using 'Calc Size Per Pixel' size for each pixel will be adjusted according with the distance between camera and position in the world" } ,
            //        new[] { KW.GLOW_SAMPLES_BASE_BLUR_IN_VERT_SIZE_OFF_Z, "Skip Z / Const Blur (Fast)", "If you are using 'Skip Z' uv calculation will be performed in the vert shader (it is a little faster), but in this case pixels will not be adjusted according with the distance between camera and position in the world, and small objects located far from the camera will give very large glow" } ,
            //    }, //Standard Brightness Control Option
            //    ADDITIONAL_PARAMS_SET = new[]{
            //                    new[]{
            //                    new ADDITIONAL_PARAM(PID._glowSamples_BlurRadius,"Glow Samples Radius","Radius",ADDITIONAL_PARAM_TYPE.toogle),
            //                    new ADDITIONAL_PARAM( new[]{ KW.GLOW_SAMPLES_BASE_BLUR_IN_FRAG_SIZE_ON_Z, KW.GLOW_SAMPLES_BASE_BLUR_IN_VERT_SIZE_OFF_Z, },new[]{ "Calc Size Per Pixel", "Skip Z (Fast)" }, "Z Depth", "" ) ,
            //                    new ADDITIONAL_PARAM( new[]{ KW.GLOW_SAMPLES_1, KW.GLOW_SAMPLES_4, KW.GLOW_SAMPLES_8, KW.GLOW_SAMPLES_16, },new[]{ "+1", "+4", "+8", "+16", } , "Glow Samples Count", "It is better to use minimum samples count" ) ,
            //                    new ADDITIONAL_PARAM( new[]{ KW.GLOW_DIRECTION_XY, KW.GLOW_DIRECTION_X, KW.GLOW_DIRECTION_Y, },new[]{ "Size XY", "Only X", "Only Y" }, "Size Direction", "You can choose blur only along horizontal or vertical axies" ) ,
            //                    new ADDITIONAL_PARAM( KW.USE_GLOW_SAMPLE_DITHER, "Use Glow Samples Diffusion", "Diffusion makes small color displacements in the pixel of color transition" , HELP_KEY: "HELPTIP_DITHER") ,
            //                    new ADDITIONAL_PARAM( PID._glowSamples_DitherSize, "Diffusion Size", "How huge will the size of the diffusion", ADDITIONAL_PARAM_TYPE.vectorY, enabled_feature:KW.USE_GLOW_SAMPLE_DITHER )
            //            },

            br.y += 8;

            return groups_y[current_group] += br.y - start;
        }

        static Material m;
        internal void GL_BEGIN()
        {
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            if (!m)
            {
                var id = SessionState.GetInt(FastPostProcessingProfile.PREFS_KEY + "_tempGUIMat", -1);
                m = EditorUtility.InstanceIDToObject(id) as Material;
                if (!m)
                {
                    m = new Material(Shader.Find("Hidden/Internal-GUITextureClip"));
                    m.hideFlags = HideFlags.HideAndDontSave;
                    SessionState.SetInt(FastPostProcessingProfile.PREFS_KEY + "_tempGUIMat", -1);
                }
            }
            // m.SetTexture("_GUIClipTexture", null);
            m.SetTexture("_MainTex", null);
            //_MainTex
            m.SetColor("_Color", GUI.color);
            m.SetPass(0);
            GL.Begin(GL.LINES);
        }

        internal void GL_END()
        {
            GL.End();
            GL.PopMatrix();
        }

        readonly Color dad = new Color32(0xc2, 0xcd, 0xd1, 255);
        Vector3 tp1;
        Vector3 tp2;
        protected void GL_VERTEX3(Vector3 r)
        {
            GL.Vertex3(r.x, r.y, 0);
        }
        void ALIAS(Vector3 p1, Vector3 p2, Color _c)
        {
            var c = _c;
            c.a *= 0.5f;
            do_al(ref c, 0.3f, ref p1, ref p2);
            c.a *= 0.5f;
            do_al(ref c, 0.5f, ref p1, ref p2);
            GL.Color(_c);
        }
        void do_al(ref Color c, float D, ref Vector3 p1, ref Vector3 p2)
        {
            GL.Color(c);
            p1.x -= D;
            p2.x -= D;
            p1.y -= D;
            p2.y -= D;
            GL_VERTEX3(p1);
            GL_VERTEX3(p2);
            p1.x += 2 * D;
            p2.x += 2 * D;
            GL_VERTEX3(p1);
            GL_VERTEX3(p2);
            p1.y += 2 * D;
            p2.y += 2 * D;
            GL_VERTEX3(p1);
            GL_VERTEX3(p2);
            p1.x -= 2 * D;
            p2.x -= 2 * D;
            GL_VERTEX3(p1);
            GL_VERTEX3(p2);
        }
        Vector2 m__tb;
        private Vector2 GetPoint(float t, Vector3[] p)
        {
            float cx = 3 * (p[1].x - p[0].x);
            float cy = 3 * (p[1].y - p[0].y);
            float bx = 3 * (p[2].x - p[1].x) - cx;
            float by = 3 * (p[2].y - p[1].y) - cy;
            float ax = p[3].x - p[0].x - cx - bx;
            float ay = p[3].y - p[0].y - cy - by;
            float Cube = t * t * t;
            float Square = t * t;
            float resX = (ax * Cube) + (bx * Square) + (cx * t) + p[0].x;
            float resY = (ay * Cube) + (by * Square) + (cy * t) + p[0].y;
            return new Vector2(resX, resY);
        }




        float RAW_TOGGABLE_OPTIONS(Rect br, SHADER_FEATURE FE, ref Vector2 out_position)
        {
            var st = br.y;
            int feat_index = 0;

            bool has_enabled = false;
            foreach (var item in FE.FEATURES_ON)
            {
                if (SCRIPT.TargetMaterial.IsKeywordEnabled(item[0]))
                {
                    has_enabled = true;
                    break;
                }
            }

            foreach (var item in FE.FEATURES_ON)
            {
                var add_on = !has_enabled ? item[0] == "-" : SCRIPT.TargetMaterial.IsKeywordEnabled(item[0]);
                GUIStyle add_style = add_on ? ST_OPTION_ON : ST_OPTION_OFF;
                EditorGUIUtility.AddCursorRect(br, MouseCursor.Link);
                if (GUI.Button(br, CONT(item[1], item[2]), add_style))
                {
                    if (!add_on)
                    {
                        Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                        Undo.RecordObject(SCRIPT, "Change Material Value");

                        foreach (var f in FE.FEATURES_ON)
                        {
                            if (SCRIPT.TargetMaterial.IsKeywordEnabled(f[0]))
                                SCRIPT.TargetMaterial.DisableKeyword(f[0]);
                        }
                        if (item[0] != "-") SCRIPT.TargetMaterial.EnableKeyword(item[0]);
                        SET("DEFAULT_CAT_" + FE.FEATURES_ON[0][0], feat_index);

                        EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        EditorUtility.SetDirty(SCRIPT);
                    }
                }
                if (add_on) out_position = new Vector2(br.x + br.width, br.y + br.height / 2);
                br.y += br.height;
                feat_index++;
            }
            return br.y - st;
        }


        bool needs_to_fix_layout;
        float layoutYPos;
        Rect R()
        {
            var res = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            res.width = EditorGUIUtility.currentViewWidth - 20;
            res.x -= 14;
            res.width += 14;
            return res;
        }

        readonly GUIContent emptyContent = new GUIContent();
        readonly float[] groups_y = new float[100];
        int current_group = -1;
        float CAT(Rect r, SHADER_FEATURE FE, bool skip)
        {
            var on = false;
            foreach (var item in FE.FEATURES_ON) on |= SCRIPT.TargetMaterial.IsKeywordEnabled(item[0]);

            if (on)
                if (FE.samples_pointes != null && FE.samples_pointes.Length != 0)
                {
                    foreach (var item in FE.samples_pointes)
                    {
                        bool allow = item.CONDITIONS.TakeWhile(c => c != "TRUE").All(c => SCRIPT.TargetMaterial.IsKeywordEnabled(c));
                        if (!allow) continue;
                        item.TARGET_SAMPLES_FEATURE().left_bar_out_poses[item.TARGET_SAMPLES_FEATURE().is_right_bar_features_used] = Vector2.zero;
                        item.TARGET_SAMPLES_FEATURE().is_right_bar_features_used++;
                    }
                }
            current_group++;
            if (skip) return 0;

            //var r = R();
            //var start = r.y;


            // BG

            {
                var gr_r = r;
                gr_r.height = groups_y[current_group];
                gr_r.width /= 2;
                var old_w = gr_r.width;
                gr_r.width = Mathf.Floor(old_w);
                if (Event.current.type == EventType.Repaint) ST_CAT_BG_LEFT.Draw(gr_r, emptyContent, 0);
                gr_r.x += Mathf.Floor(old_w); ;
                gr_r.width = Mathf.Ceil(old_w);
                if (Event.current.type == EventType.Repaint) ST_CAT_BG_RIGHT.Draw(gr_r, emptyContent, 0);


                //ST_CAT_BG_LEFT.Draw(gr_r, emptyContent, 0);


            }
            r.height = groups_y[current_group];
            groups_y[current_group] = 0;
            // BG


            ////GUI.BeginClip( r );
            //r.x = 0;
            //r.y = 0;
            //
            var start = r.y;
            GUIStyle ON_STYLE = on ? ST_TOGGLE_ON : ST_TOGGLE_OFF;
            //GUIStyle OFF_STYLE = !on ? ST_TOGGLE_ON : ST_TOGGLE_OFF;

            //TOGGLES
            var br = r;
            br.x += PAD_LEFT;
            br.y += 12;
            br.width = 70;
            br.height = HEAD_HEIGHT;
            if (!FE.disable_toggle)
            {
                EditorGUIUtility.AddCursorRect(br, MouseCursor.Link);
                if (GUI.Button(br, CONT(on ? "On" : "Off", FE.S_TITLE), ON_STYLE))
                {
                    Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                    Undo.RecordObject(SCRIPT, "Change Material Value");

                    if (on)
                    {
                        foreach (var item in FE.FEATURES_ON)
                        {
                            if (SCRIPT.TargetMaterial.IsKeywordEnabled(item[0]))
                                SCRIPT.TargetMaterial.DisableKeyword(item[0]);
                        }
                    }
                    else
                    {
                        var target = GET("DEFAULT_CAT_" + FE.FEATURES_ON[0][0], FE.default_feature);
                        var item = FE.FEATURES_ON[Mathf.Clamp(target, 0, FE.FEATURES_ON.Length - 1)];
                        SCRIPT.TargetMaterial.EnableKeyword(item[0]);
                    }
                    FastPostProcessingCamera.reset_buffer = true;

                    EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                    EditorUtility.SetDirty(SCRIPT);

                }
            }

            //br.x += br.width;
            //br.width = 53;
            //EditorGUIUtility.AddCursorRect( br, MouseCursor.Link );
            //if ( GUI.Button( br, CONT( "Off", FE.S_TITLE ), OFF_STYLE )  )
            //{
            //	if ( on )
            //	{
            //		foreach ( var item in FE.FEATURES_ON )
            //		{
            //			if ( SCRIPT.TargetMaterial.IsKeywordEnabled( item[ 0 ] ) )
            //				SCRIPT.TargetMaterial.DisableKeyword( item[ 0 ] );
            //		}
            //	}
            //	else
            //	{
            //		var item = FE.FEATURES_ON[ Mathf.Clamp( FE.default_feature, 0, FE.FEATURES_ON.Length - 1)];
            //		SCRIPT.TargetMaterial.EnableKeyword( item[ 0 ] );
            //	}
            //}
            br.x += 10 + br.width;
            br.width = r.x + r.width - br.x - PAD_LEFT;
            var lab = br;
            lab.y += 5;
            string title_samples_count = GET_ADDITIONAL_TITLE(FE.samples_count_to_title);
            GUI.Label(lab, CONT(FE.S_TITLE, !on, title_samples_count), on ? ST_CAT_NAME : ST_CAT_NAME_OFF);
            var out_position = new Vector2(lab.x + lab.width, lab.y + lab.height / 2);

            br.y += br.height;

            //TOGGLES

            if (!on && !FE.disable_toggle)
            {
                var H = br.y - start + 14;
                // EditorGUILayout.Space(H);

                //GUI.EndClip();
                return groups_y[current_group] += H;
            }

            r.y = br.y;
            br = r;
            br.x += PAD_LEFT;
            br.y += 11;
            var t_r = br;
            t_r.height = 1;
            t_r.width = 1;
            if (!string.IsNullOrEmpty(FE.HELP_IMAGE))
            {
                if (!Icons.HELP_DIC.ContainsKey(FE.HELP_IMAGE)) Icons.HELP_DIC.Add(FE.HELP_IMAGE, new Icons.IconBehaviour(FE.HELP_IMAGE));
                var help_text = (Texture2D)Icons.HELP_DIC[FE.HELP_IMAGE];
                t_r.width = Mathf.Max(1, help_text.width);
                t_r.height = Mathf.Max(1, help_text.height);
                t_r.width /= 2;
                t_r.height /= 2;
                //var t_r_c = t_r;
                //t_r_c.x += PAD_LEFT;
                if (oldInside[current_group])
                {
                    t_r.width *= 2;
                    t_r.height *= 2;
                }
                else
                {
                    var f = t_r.width / r.width;
                    var dif = Mathf.Clamp01(0.5f / f);
                    t_r.width *= dif;
                    t_r.height *= dif;
                }
                EditorGUIUtility.AddCursorRect(t_r, MouseCursor.Link);
                if (GUI.Button(t_r, TapToZoomContent))
                {
                    oldInside[current_group] = !oldInside[current_group];
                    /*if ( oldInside == current_group )
                        oldInside = null;
                    else
                        oldInside = current_group;*/
                }
                //if ( Event.current.type != EventType.Repaint && oldInside != t_r_c.Contains( Event.current.mousePosition ) )
                //{
                //	oldInside = t_r_c.Contains( Event.current.mousePosition );
                //	Repaint();
                //
                //}

                //else Repaint();

                GUI.DrawTexture(t_r, help_text);
                if (Event.current.type == EventType.Repaint) ST_MAIN_IMAGE_FRAME.Draw(t_r, emptyContent, 0);


            }


            // OPTIONS
            br = t_r;
            br.x += br.width + 9;
            br.height = 32;
            br.width = r.x + r.width - br.x - PAD_LEFT;
            var newH = RAW_TOGGABLE_OPTIONS(br, FE, ref out_position);

            // OPTIONS






            var MAX = Mathf.Max(newH, t_r.height);
            br = r;
            br.y = t_r.y + MAX;

            br.x += PAD_LEFT;
            br.width = r.x + r.width - br.x - PAD_LEFT;

            // HELP TEXT
            br.y += 11;
            //br.height = EditorStyles.helpBox.CalcHeight( CONT( FE.S_TIP_TEXT, FE.S_TITLE ), br.width );
            br.height = EditorStyles.helpBox.CalcHeight(CONT(FE.S_TIP_TEXT, FE.S_TITLE), br.width);
            //GUILayout.BeginArea( br );
            //EditorGUILayout.HelpBox( FE.S_TIP_TEXT, MessageType.None );
            //var lr = GUILayoutUtility.GetLastRect();
            //GUILayout.EndArea();
            if (Event.current.type == EventType.Repaint) EditorStyles.helpBox.Draw(br, CONT(FE.S_TIP_TEXT, FE.S_TITLE), 0);
            //EditorGUI.HelpBox( br, FE.S_TIP_TEXT, MessageType.None );
            //var ss = EditorStyles.helpBox.CalcScreenSize( new Vector2( br.width, br.height ) );
            //br.height = ss.y;
            br.y += br.height;
            br.y += 11;
            // HELP TEXT


            br.height = EditorGUIUtility.singleLineHeight;
            // ADDITIONAL
            if (FE.ADDITIONAL_PARAMS_GET.Length != 0)
            {
                int _i = 0;
                for (int i = 0; i < FE.FEATURES_ON.Length; i++) if (SCRIPT.TargetMaterial.IsKeywordEnabled(FE.FEATURES_ON[i][0])) _i = i;
                var ADD = FE.ADDITIONAL_PARAMS_GET[Mathf.Clamp(_i, 0, FE.ADDITIONAL_PARAMS_GET.Length - 1)];
                foreach (ADDITIONAL_PARAM item in ADD)
                {
                    var en = GUI.enabled;
                    if (!string.IsNullOrEmpty(item.enabled_feature))
                    {
                        var ef = SCRIPT.TargetMaterial.IsKeywordEnabled(item.enabled_feature);
                        if (item.enabled_feature_inverse) ef = !ef;
                        GUI.enabled &= ef;
                        // GUI.enabled &= SCRIPT.TargetMaterial.IsKeywordEnabled(item.enabled_feature);
                    }
                    if (GUI.enabled || !item.enable_full_falling)
                    {
                        br.height = EditorGUIUtility.singleLineHeight;
                        DRAW_TYPE(ref br, item);
                        br.y += br.height;
                    }
                    GUI.enabled = en;

                }
            }
            // ADDITIONAL



            //POST
            if (FE.POST_SHADER_FEATURES != null && FE.POST_SHADER_FEATURES.Length != 0)
            {


                {
                    var gr_r = br;
                    gr_r.height = ST_SET_BG_LEFT.normal.background.height;
                    gr_r.width /= 2;
                    if (Event.current.type == EventType.Repaint) ST_SET_BG_LEFT.Draw(gr_r, emptyContent, 0);
                    gr_r.x += gr_r.width;
                    if (Event.current.type == EventType.Repaint) ST_SET_BG_RIGHT.Draw(gr_r, emptyContent, 0);
                    br.height = gr_r.height;
                }

                br.y += br.height;
                br.height = EditorGUIUtility.singleLineHeight;

                // GUI.Label(br, "-");
                foreach (var post in FE.POST_SHADER_FEATURES)
                {
                    br.height = EditorGUIUtility.singleLineHeight;
                    var key = post.FEATURES_ON[0][0];
                    var new_e = EditorGUI.ToggleLeft(br, CONT(post.S_TITLE), SCRIPT.TargetMaterial.IsKeywordEnabled(key));
                    br.y += br.height;
                    if (new_e != SCRIPT.TargetMaterial.IsKeywordEnabled(key))
                    {
                        Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                        Undo.RecordObject(SCRIPT, "Change Material Value");
                        if (new_e) SCRIPT.TargetMaterial.EnableKeyword(key);
                        else SCRIPT.TargetMaterial.DisableKeyword(key);
                        FastPostProcessingCamera.reset_buffer = true;
                        EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        EditorUtility.SetDirty(SCRIPT);
                    }

                    if (new_e)
                    {
                        if (!string.IsNullOrEmpty(post.HELP_IMAGE))
                        {
                            var f = DRAW_TEXTURE(br, post.HELP_IMAGE);
                            br.height = f;
                            br.y += br.height;
                        }
                        if (post.ADDITIONAL_PARAMS_GET.Length != 0)
                        {
                            foreach (ADDITIONAL_PARAM add in post.ADDITIONAL_PARAMS_GET[0])
                            {
                                br.height = EditorGUIUtility.singleLineHeight;
                                DRAW_TYPE(ref br, add, true);
                                br.y += br.height;
                            }
                        }
                    }
                }

            }
            //POST


            if (FE.samples_pointes != null && FE.samples_pointes.Length != 0)
            {
                foreach (var item in FE.samples_pointes)
                {
                    bool allow = true;
                    foreach (var c in item.CONDITIONS)
                    {
                        if (c == "TRUE") break;
                        if (!SCRIPT.TargetMaterial.IsKeywordEnabled(c))
                        {
                            allow = false;
                            break;
                        }
                    }
                    if (allow && item.TARGET_SAMPLES_FEATURE().is_right_bar_features_used > 0)
                    {
                        item.TARGET_SAMPLES_FEATURE().left_bar_out_poses[item.TARGET_SAMPLES_FEATURE().is_right_bar_features_used - 1] = out_position;
                    }
                }

            }


            {
                var H = br.y - start + 14;//
                                          //GUILayout.Button( "ASD", GUILayout.Height( H ) );
                                          //EditorGUILayout.GetControlRect( GUILayout.Height( H ) );
                                          // GUILayout.Space(H);

                return groups_y[current_group] += H;
            }

            //start = R().y;
            //t_r.y = start;
            //t_r.height = br.y - start ;
            //EditorGUI.DrawRect( t_r, Color.red );

            //EditorGUILayout.Space( 20 );
            //GUI.EndClip();


        }




        void DRAW_TYPE(ref Rect _br, ADDITIONAL_PARAM item, bool use_hier = false, bool skip_hier = false)
        {

            var br = _br;
            bool have_to = false;
            var start = 0f;


            if ((!string.IsNullOrEmpty(item.enabled_feature) || use_hier) && !skip_hier)
            {
                var hier = (Texture2D)Icons.HIER_A;
                var h_r = br;
                h_r.width = 30;
                h_r.y -= h_r.height / 4 - 2;
                GUI.DrawTexture(h_r, hier);
                br.x += h_r.width + 4;
                br.width -= h_r.width + 4;
                if (string.IsNullOrEmpty(item.HELP_KEY) &&
                    item.TYPE != ADDITIONAL_PARAM_TYPE.MinMaxSlider &&
                    item.TYPE != ADDITIONAL_PARAM_TYPE.vectorXY &&
                    item.TYPE != ADDITIONAL_PARAM_TYPE.action_for_gradient &&
                    item.TYPE != ADDITIONAL_PARAM_TYPE.ColorXYZ
                )
                {
                    have_to = true;
                    GUI.BeginClip(br);
                    start = br.y;
                    br.x = br.y = 0;
                }
            }


            switch (item.TYPE)
            {

                case ADDITIONAL_PARAM_TYPE.@float:
                    {
                        //  if (item.TEXT == "Bright Point")   Debug.Log("ASD");
                        var nv = DRAW_FIELD(br, CONT(item.TEXT, item.TOOLTIP), SCRIPT.TargetMaterial.GetFloat(item.KEY), item);
                        if (nv != SCRIPT.TargetMaterial.GetFloat(item.KEY))
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetFloat(item.KEY, nv);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.@float_for_lut:
                    {
                        //  if (item.TEXT == "Bright Point")   Debug.Log("ASD");
                        var nv = DRAW_FIELD(br, CONT(item.TEXT, item.TOOLTIP), SCRIPT.TargetMaterial.GetFloat(item.KEY), item);

                        if (nv > 1 && !SCRIPT.TargetMaterial.IsKeywordEnabled(KW.LUTS_AMONT_MORE_THAN_1))
                        {
                            SCRIPT.TargetMaterial.EnableKeyword(KW.LUTS_AMONT_MORE_THAN_1);
                            FastPostProcessingCamera.reset_buffer = true;
                        }
                        else if (nv <= 1 && SCRIPT.TargetMaterial.IsKeywordEnabled(KW.LUTS_AMONT_MORE_THAN_1))
                        {
                            SCRIPT.TargetMaterial.DisableKeyword(KW.LUTS_AMONT_MORE_THAN_1);
                            FastPostProcessingCamera.reset_buffer = true;
                        }

                        if (nv != SCRIPT.TargetMaterial.GetFloat(item.KEY))
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetFloat(item.KEY, nv);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.MinMaxSliderInverse:
                case ADDITIONAL_PARAM_TYPE.MinMaxSlider:
                    {
                        //  if (item.TEXT == "Bright Point")   Debug.Log("ASD");
                        var old_minVal = SCRIPT.TargetMaterial.GetFloat(item.KEY);
                        var old_maxVal = SCRIPT.TargetMaterial.GetFloat(item.KEY2);
                        var minVal = old_minVal;
                        var maxVal = old_maxVal;
                        // EditorGUI.MinMaxSlider(br, CONT(item.TEXT, item.TOOLTIP), ref minVal, ref maxVal, item.min, item.max);
                        // EditorGUI.MinMaxSlider(br, CONT(item.TEXT, item.TOOLTIP), ref minVal, ref maxVal, item.min, item.max);
                        var tr = br;
                        tr.width /= 3;
                        GUI.Label(br, CONT(item.TEXT, item.TOOLTIP));
                        // br.y += br.height;
                        // var t_r = br;
                        //t_r.width /= 4;
                        // GUI.Label(t_r, "X:"); t_r.x += t_r.width;

                        // br.y += br.height;
                        // GUI.Label(t_r, "Y:"); t_r.x += t_r.width;
                        tr.x += tr.width;

                        string S1 = "Near:";
                        string S2 = "Far:";
                        if (item.TYPE == ADDITIONAL_PARAM_TYPE.MinMaxSliderInverse)
                        {
                            S1 = "Top:";
                            S2 = "Down:";
                        }
                        // tr.width /= 2;
                        var olw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = tr.width / 2;
                        minVal = Mathf.Clamp(EditorGUI.FloatField(tr, S1, minVal), item.min, item.max);
                        tr.x += tr.width;
                        maxVal = Mathf.Clamp(EditorGUI.FloatField(tr, S2, maxVal), item.min, item.max);
                        if (item.TYPE == ADDITIONAL_PARAM_TYPE.MinMaxSliderInverse)
                        {
                            if (minVal < maxVal + 0.01f) minVal = maxVal + 0.01f;
                        }
                        else
                        {
                            if (minVal > maxVal - 0.01f) minVal = maxVal - 0.01f;
                        }
                        EditorGUIUtility.labelWidth = olw;
                        if (minVal != old_minVal || maxVal != old_maxVal)
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetFloat(item.KEY, minVal);
                            SCRIPT.TargetMaterial.SetFloat(item.KEY2, maxVal);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.@int:
                    {
                        var nv = DRAW_FIELD(br, CONT(item.TEXT, item.TOOLTIP), (int)SCRIPT.TargetMaterial.GetFloat(item.KEY), item);
                        if (nv != (int)SCRIPT.TargetMaterial.GetFloat(item.KEY))
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetFloat(item.KEY, nv);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.TEXTURE:
                    {
                        //br.height *= 2;
                        var tex = SCRIPT.TargetMaterial.GetTexture(item.KEY);
                        var c = GUI.color;
                        if (!tex) GUI.color *= new Color(1, 0.3f, 0.2f);
                        var olw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = br.width / 2;
                        var new_text = EditorGUI.ObjectField(br, CONT(item.TEXT, item.TOOLTIP), tex, typeof(Texture2D), false) as Texture2D;
                        EditorGUIUtility.labelWidth = olw;
                        GUI.color = c;
                        if (tex != new_text)
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetTexture(item.KEY, new_text);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.Button:
                    {
                        //br.height *= 2;
                        br.height *= 1;
                        if (GUI.Button(br, CONT(item.TEXT, item.TOOLTIP)))
                        {
                            item.action();
                        }
                        br.height += item.space;
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.vectorX:
                case ADDITIONAL_PARAM_TYPE.vectorY:
                case ADDITIONAL_PARAM_TYPE.vectorZ:
                case ADDITIONAL_PARAM_TYPE.vectorW:
                    {
                        var ind = item.TYPE == ADDITIONAL_PARAM_TYPE.vectorX ? 0 : item.TYPE == ADDITIONAL_PARAM_TYPE.vectorY ? 1 : item.TYPE == ADDITIONAL_PARAM_TYPE.vectorZ ? 2 : 3;
                        var nv = DRAW_FIELD(br, CONT(item.TEXT, item.TOOLTIP), SCRIPT.TargetMaterial.GetVector(item.KEY)[ind], item);
                        if (nv != SCRIPT.TargetMaterial.GetVector(item.KEY)[ind])
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            var v = SCRIPT.TargetMaterial.GetVector(item.KEY);
                            v[ind] = nv;
                            SCRIPT.TargetMaterial.SetVector(item.KEY, v);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.vectorXY:
                    {
                        var old_v = (Vector2)SCRIPT.TargetMaterial.GetVector(item.KEY);
                        var nv = old_v;
                        if (skip_hier)
                        {
                            GUI.Label(br, CONT(item.TEXT, item.TOOLTIP));
                            br.y += br.height;
                            // var t_r = br;
                            //t_r.width /= 4;
                            // GUI.Label(t_r, "X:"); t_r.x += t_r.width;
                            var olw = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = br.width / 2;
                            nv.x = EditorGUI.FloatField(br, "X:", old_v.x);
                            br.y += br.height;
                            // GUI.Label(t_r, "Y:"); t_r.x += t_r.width;
                            nv.y = EditorGUI.FloatField(br, "Y:", old_v.y);
                            EditorGUIUtility.labelWidth = olw;
                        }
                        else
                        {
                            var olw = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = br.width / 2;
                            nv = EditorGUI.Vector2Field(br, CONT(item.TEXT, item.TOOLTIP), old_v);
                            EditorGUIUtility.labelWidth = olw;
                        }
                        if (nv != old_v)
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetVector(item.KEY, nv);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.ColorField:
                    {
                        var color = SCRIPT.TargetMaterial.GetColor(item.KEY);
                        var new_color = EditorGUI.ColorField(br, CONT(item.TEXT, item.TOOLTIP), color);
                        if (color != new_color)
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetColor(item.KEY, new_color);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.ColorXYZ:
                    {
                        var color = SCRIPT.TargetMaterial.GetVector(item.KEY);
                        GUI.Label(br, CONT(item.TEXT, item.TOOLTIP));


                        br.y += br.height;
                        color.x = DRAW_FIELD(br, CONT("Red:", item.TOOLTIP), color.x, item);
                        br.y += br.height;
                        color.y = DRAW_FIELD(br, CONT("Green:", item.TOOLTIP), color.y, item);
                        br.y += br.height;
                        color.z = DRAW_FIELD(br, CONT("Blue:", item.TOOLTIP), color.z, item);


                        if (color != SCRIPT.TargetMaterial.GetVector(item.KEY))
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetVector(item.KEY, color);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.toogle:
                    {
                        bool has_image = true;
                        if (!string.IsNullOrEmpty(item.TOOGLE_SHADER_FEATURE))
                        {
                            var old_e = SCRIPT.TargetMaterial.IsKeywordEnabled(item.TOOGLE_SHADER_FEATURE);
                            var new_e = EditorGUI.ToggleLeft(br, CONT(item.TEXT, item.TOOLTIP), old_e);
                            if (new_e != old_e)
                            {
                                Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                                if (new_e) SCRIPT.TargetMaterial.EnableKeyword(item.TOOGLE_SHADER_FEATURE);
                                else SCRIPT.TargetMaterial.DisableKeyword(item.TOOGLE_SHADER_FEATURE);
                                FastPostProcessingCamera.reset_buffer = true;
                                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                            }
                            if (!new_e) has_image = false;
                        }
                        if (has_image && !string.IsNullOrEmpty(item.HELP_KEY))
                        {
                            br.y += br.height;
                            var f = DRAW_TEXTURE(br, item.HELP_KEY);
                            br.height = f;
                        }
                        br.height += item.space;
                        break;
                    }
                case ADDITIONAL_PARAM_TYPE.toogle_as_float:
                    {
                        bool has_image = true;
                        var old_e = Mathf.Abs(SCRIPT.TargetMaterial.GetFloat(item.TOOGLE_SHADER_AS_FLOAT) - 1.0f) < 0.01f;
                        var new_e = EditorGUI.ToggleLeft(br, CONT(item.TEXT, item.TOOLTIP), old_e);
                        if (new_e != old_e)
                        {
                            Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                            SCRIPT.TargetMaterial.SetFloat(item.TOOGLE_SHADER_AS_FLOAT, new_e ? 1.0f : 0.0f);
                            EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                        }
                        if (!new_e) has_image = false;
                        if (has_image && !string.IsNullOrEmpty(item.HELP_KEY))
                        {
                            br.y += br.height;
                            var f = DRAW_TEXTURE(br, item.HELP_KEY);
                            br.height = f;
                        }
                        br.height += item.space;
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.toolbar_but_toggle:
                case ADDITIONAL_PARAM_TYPE.toolbar_as_float:
                case ADDITIONAL_PARAM_TYPE.toolbar:
                    {

                        br.height = (int)(br.height * 1.5f);
                        br.height += 8;

                        var lr = br;
                        lr.width /= 5;

                        var rr = br;

                        br.height += item.space;

                        if (ADDITIONAL_PARAM_TYPE.toolbar_but_toggle != item.TYPE)
                        {
                            rr.x += lr.width;
                            rr.width -= lr.width;

                            GUI.Label(lr, CONT(item.TEXT, item.TOOLTIP));
                        }


                        //var hash = WORDS[0].GetHashCode();
                        //for (int i = 1; i < WORDS.Length; i++) hash ^= WORDS[i].GetHashCode();
                        //if (!_contents.ContainsKey(hash))
                        //{
                        //    var contents = WORDS.Select(w => new GUIContent(w, w + "/n" + tooltip)).ToList();
                        //    contents.Insert(0, new GUIContent("Disable"));
                        //    _contents.Add(hash, contents.ToArray());
                        //}
                        if (item._content_cache == null)
                        {
                            item._content_cache = new GUIContent[item.display_names.Length];
                            for (int i = 0; i < item._content_cache.Length; i++)
                            {
                                item._content_cache[i] = new GUIContent(CONT(item.display_names[i], item.TOOLTIP));
                            }
                        }

                        if (item.TYPE == ADDITIONAL_PARAM_TYPE.toolbar_as_float)
                        {
                            var WORDS = item.TOOGLE_FLOAT_AS_TOOLBAR;
                            var current_val = SCRIPT.TargetMaterial.GetFloat(item.KEY);
                            var old_value = 0;
                            for (int i = 0; i < WORDS.Length; i++)
                                if (Mathf.Abs(current_val - WORDS[i]) < 0.01f)
                                    old_value = i;

                            var new_value = GUI.Toolbar(rr, old_value, item._content_cache);
                            if (new_value != old_value)
                            {
                                Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                                SCRIPT.TargetMaterial.SetFloat(item.KEY, WORDS[new_value]);
                                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                            }
                        }
                        else
                        {
                            var WORDS = item.TOOGLE_SHADER_AS_TOOLBAR;
                            var old_value = 0;
                            for (int i = 0; i < WORDS.Length; i++)
                            {
                                if (WORDS[i] == "-") continue;
                                if (old_value != 0)
                                {
                                    if (SCRIPT.TargetMaterial.IsKeywordEnabled(WORDS[i])) SCRIPT.TargetMaterial.DisableKeyword(WORDS[i]);
                                    continue;
                                }
                                if (SCRIPT.TargetMaterial.IsKeywordEnabled(WORDS[i]))
                                {
                                    old_value = i;
                                }
                            }
                            var new_value = old_value;
                            if (item.TYPE == ADDITIONAL_PARAM_TYPE.toolbar) new_value = GUI.Toolbar(rr, old_value, item._content_cache);
                            else new_value = EditorGUI.ToggleLeft(rr, CONT(item.TEXT, item.TOOLTIP), old_value == 1) ? 1 : 0;
                            if (new_value != old_value)
                            {
                                Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                                if (old_value != 0)
                                {
                                    SCRIPT.TargetMaterial.DisableKeyword(WORDS[old_value]);
                                    FastPostProcessingCamera.reset_buffer = true;
                                }
                                if (new_value != 0)
                                {
                                    SCRIPT.TargetMaterial.EnableKeyword(WORDS[new_value]);
                                    FastPostProcessingCamera.reset_buffer = true;
                                }
                                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
                            }
                        }


                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.action_for_gradient:
                    {
                        //  if (SCRIPT.TargetMaterial.IsKeywordEnabled(KW.GRADIENT_RAMP))
                        br.height = SCRIPT.CurrentGradient.DrawEditorGUI(br, new UnityEngine.Object[] { SCRIPT, SCRIPT.TargetMaterial }, SCRIPT.TargetMaterial.GetFloat(_PID._LUT1_gradient_smooth));
                        //   else br.height = 0;
                        break;
                    }

                case ADDITIONAL_PARAM_TYPE.helpbox:
                    {

                        content.text = item.TEXT;
                        br.height = EditorStyles.helpBox.CalcHeight(content, br.width);
                        EditorGUI.HelpBox(br, item.TEXT, MessageType.None);
                        // br.y += br.height;
                        // var t_r = br;
                        break;
                    }
            }


            if (have_to)
            {
                GUI.EndClip();
            }

            _br.y = br.y + start;
            _br.height = br.height;
        }
        GUIContent content = new GUIContent();


        float DRAW_FIELD(Rect r, GUIContent cont, float val, ADDITIONAL_PARAM p)
        {
            float res;
            var olw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = r.width / 2;
            if (p.min != -999 && p.max != -999) res = EditorGUI.Slider(r, cont, val, p.min, p.max);
            else
            {
                res = EditorGUI.FloatField(r, cont, val);
                if (p.min != -999) res = Mathf.Max(p.min, res);
                if (p.max != -999) res = Mathf.Min(p.max, res);
            }
            EditorGUIUtility.labelWidth = olw;
            return res;
        }
        int DRAW_FIELD(Rect r, GUIContent cont, int val, ADDITIONAL_PARAM p)
        {
            int res;
            var olw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = r.width / 2;
            if (p.min != -999 && p.max != -999) res = EditorGUI.IntSlider(r, cont, val, (int)p.min, (int)p.max);
            else
            {
                res = EditorGUI.IntField(r, cont, val);
                if (p.min != -999) res = Mathf.Max((int)p.min, res);
                if (p.max != -999) res = Mathf.Min((int)p.max, res);
            }
            EditorGUIUtility.labelWidth = olw;
            return res;
        }
        float DRAW_TEXTURE(Rect t_r, string HELP_IMAGE)
        {
            if (!Icons.HELP_DIC.ContainsKey(HELP_IMAGE)) Icons.HELP_DIC.Add(HELP_IMAGE, new Icons.IconBehaviour(HELP_IMAGE));
            t_r.y += 4;
            var help_text = (Texture2D)Icons.HELP_DIC[HELP_IMAGE];
            t_r.width = Mathf.Max(1, help_text.width);
            t_r.height = Mathf.Max(1, help_text.height);
            GUI.DrawTexture(t_r, help_text);
            if (Event.current.type == EventType.Repaint) ST_MAIN_IMAGE_FRAME.Draw(t_r, emptyContent, 0);
            return t_r.height + 10;
        }


        readonly bool[] oldInside = new bool[100];

        readonly GUIContent TapToZoomContent = new GUIContent() { tooltip = "Tap to zoom" };
        readonly GUIContent _cont = new GUIContent();
        GUIContent CONT(string[] s, bool stripBrack = false, string additinal_title = "")
        {
            _cont.text = s[0];
            if (string.IsNullOrEmpty(s[1])) _cont.tooltip = s[0];
            else _cont.tooltip = s[1];
            if (stripBrack)
            {
                var b = _cont.text.IndexOf("(");
                if (b != -1) _cont.text = _cont.text.Remove(b).Trim();
            }
            else
            {
                if (!string.IsNullOrEmpty(additinal_title)) _cont.text += additinal_title;
            }
            _cont.tooltip = "[ " + _cont.text + " ]\n" + _cont.tooltip;
            return _cont;
        }
        GUIContent CONT(string s, string[] s2)
        {
            _cont.text = s;
            if (string.IsNullOrEmpty(s2[1])) _cont.tooltip = s2[0];
            else _cont.tooltip = s2[1];
            _cont.tooltip = "[ " + _cont.text + " ]\n" + _cont.tooltip;
            return _cont;
        }
        GUIContent CONT(string s, string s2)
        {
            _cont.text = s;
            if (string.IsNullOrEmpty(s2)) _cont.tooltip = s;
            else _cont.tooltip = s2;
            _cont.tooltip = "[ " + _cont.text + " ]\n" + _cont.tooltip;
            return _cont;
        }


        void SET(string k, int val)
        {
            SCRIPT.SET(k, val);
            EditorPrefs.SetInt(FastPostProcessingProfile.PREFS_KEY + k, val);
        }
        int GET(string k, int def)
        {
            int res;
            if (!SCRIPT.GET(k, out res)) return EditorPrefs.GetInt(FastPostProcessingProfile.PREFS_KEY + k, def);
            return res;
        }

        class SHADER_FEATURE
        {
            internal string[][] FEATURES_ON;
            internal int default_feature = -1;
            internal string HELP_IMAGE;

            internal string[] S_TITLE;
            internal string S_TIP_TEXT;

            internal ADDITIONAL_PARAM[][] ADDITIONAL_PARAMS_SET;
            internal ADDITIONAL_PARAM[][] ADDITIONAL_PARAMS_GET {
                get {
                    if (ADDITIONAL_PARAMS_SET == null) ADDITIONAL_PARAMS_SET = ADDITIONAL_PARAMS_F != null ? ADDITIONAL_PARAMS_F() : new ADDITIONAL_PARAM[0][];
                    return ADDITIONAL_PARAMS_SET;
                }
            }

            internal Func<ADDITIONAL_PARAM[][]> ADDITIONAL_PARAMS_F;


            internal SHADER_FEATURE[] POST_SHADER_FEATURES;
            internal bool disable_toggle;

            internal string[][] samples_count_to_title = new string[0][];
            internal ADDITIONAL_SAMPLES_POINTER[] samples_pointes = new ADDITIONAL_SAMPLES_POINTER[0];
            internal int is_right_bar_features_used = 0;
            internal readonly Vector2[] left_bar_out_poses = new Vector2[100];
            internal int is_additional_features_used = 0;
            internal readonly Vector2[] addition_features_out_poses = new Vector2[20];

            internal RANDOM_HANDLER[] random_handler = new RANDOM_HANDLER[0];
            internal bool is_used;
        }

        class RANDOM_HANDLER
        {
            internal RANDOM_HANDLER()
            {
                start_line = UnityEngine.Random.Range(0.2f, 0.5f);
                end_line = UnityEngine.Random.Range(1.7f, 2.1f);
                var RR = 5;
                for (int i = 0; i < 10; i++)
                {
                    rofp[i].x = UnityEngine.Random.Range(-RR, RR);
                    rofp[i].y = UnityEngine.Random.Range(-RR, RR);
                    rofp2[i].x = UnityEngine.Random.Range(-RR, RR);
                    rofp2[i].y = UnityEngine.Random.Range(-RR, RR);
                }
            }

            internal float start_line;
            internal float end_line;
            internal readonly Vector3[] rofp2 = new Vector3[10];
            internal readonly Vector3[] rofp = new Vector3[10];
        }


        class ADDITIONAL_PARAM
        {
            internal readonly string TOOGLE_SHADER_FEATURE;
            internal readonly int TOOGLE_SHADER_AS_FLOAT;
            internal readonly string[] TOOGLE_SHADER_AS_TOOLBAR;
            internal readonly float[] TOOGLE_FLOAT_AS_TOOLBAR;
            internal readonly string[] display_names;
            internal GUIContent[] _content_cache;
            internal readonly string TEXT;
            internal readonly string TOOLTIP;
            internal readonly int KEY;
            internal int KEY2;
            internal readonly ADDITIONAL_PARAM_TYPE TYPE;
            internal float min = -999, max = -999;
            internal readonly string enabled_feature;
            internal readonly bool enabled_feature_inverse = false;
            internal readonly bool enable_full_falling = false;
            internal readonly string HELP_KEY;
            internal Action action;
            internal readonly float space;


            //Func<bool> enabled;
            public ADDITIONAL_PARAM(float[] shader_feature, int KEY, string[] display_names, string TEXT, string TOOLTIP, string HELP_KEY = null, float space = 0)
            {
                this.TEXT = TEXT; this.TOOLTIP = TOOLTIP; this.TOOGLE_FLOAT_AS_TOOLBAR = shader_feature; this.TYPE = ADDITIONAL_PARAM_TYPE.toolbar_as_float;
                this.HELP_KEY = HELP_KEY;
                this.KEY = KEY;
                this.display_names = display_names;
                this.space = space;
            }
            public ADDITIONAL_PARAM(string[] shader_feature, string[] display_names, string TEXT, string TOOLTIP, string HELP_KEY = null, bool useassimpletoggle = false, float space = 10)
            {
                this.TEXT = TEXT; this.TOOLTIP = TOOLTIP; this.TOOGLE_SHADER_AS_TOOLBAR = shader_feature; this.TYPE = useassimpletoggle ? ADDITIONAL_PARAM_TYPE.toolbar_but_toggle : ADDITIONAL_PARAM_TYPE.toolbar;
                this.HELP_KEY = HELP_KEY;
                this.display_names = display_names;
                this.space = space;
            }

            // public ADDITIONAL_PARAM(int shader_feature, string TEXT, string TOOLTIP, string HELP_KEY = null)
            // {
            //     this.TEXT = TEXT; this.TOOLTIP = TOOLTIP; this.TOOGLE_SHADER_AS_FLOAT = shader_feature; this.TYPE = ADDITIONAL_PARAM_TYPE.toogle_as_float;
            //     this.HELP_KEY = HELP_KEY;
            // }
            public ADDITIONAL_PARAM(string shader_feature, string TEXT, string TOOLTIP, string HELP_KEY = null)
            {
                this.TEXT = TEXT; this.TOOLTIP = TOOLTIP; this.TOOGLE_SHADER_FEATURE = shader_feature; this.TYPE = ADDITIONAL_PARAM_TYPE.toogle;
                this.HELP_KEY = HELP_KEY;
            }

            internal ADDITIONAL_PARAM(int KEY, string TEXT, string TOOLTIP, ADDITIONAL_PARAM_TYPE TYPE, string enabled_feature = null, string HELP_KEY = null, bool enabled_feature_inverse = false, float space = 0, bool enable_full_falling = false)
            {
                if (TYPE == ADDITIONAL_PARAM_TYPE.toogle_as_float) this.TOOGLE_SHADER_AS_FLOAT = KEY;
                else this.KEY = KEY;
                this.TEXT = TEXT; this.TOOLTIP = TOOLTIP; this.TYPE = TYPE;
                this.enabled_feature = enabled_feature;
                this.enabled_feature_inverse = enabled_feature_inverse;
                this.HELP_KEY = HELP_KEY;
                this.space = space;
                this.enable_full_falling = enable_full_falling;
            }

        }
        internal enum ADDITIONAL_PARAM_TYPE
        {
            @float, @float_for_lut, @int, TEXTURE, vectorX, vectorY, vectorZ, vectorW, vectorXY, toogle_as_float, toogle, ColorXYZ, ColorField, Button, MinMaxSlider, MinMaxSliderInverse,
            toolbar, toolbar_but_toggle,
            toolbar_as_float, action_for_gradient, helpbox
        }


        //static internal  IconBehaviour TOGGLE_E_N = new IconBehaviour("TOGGLE_E_N");
        //static internal  IconBehaviour TOGGLE_E_H = new IconBehaviour("TOGGLE_E_H");
        //static internal  IconBehaviour TOGGLE_D_N = new IconBehaviour("TOGGLE_D_N");
        //static internal  IconBehaviour TOGGLE_D_H = new IconBehaviour("TOGGLE_D_H");
        //
        //static internal  IconBehaviour OPTION_E_N = new IconBehaviour("OPTION_E_N");
        //static internal  IconBehaviour OPTION_E_H = new IconBehaviour("OPTION_E_H");
        //static internal  IconBehaviour OPTION_D_N = new IconBehaviour("OPTION_D_N");
        //static internal  IconBehaviour OPTION_D_H = new IconBehaviour("OPTION_D_H");
        //
        //static internal  IconBehaviour NEXT_LVL = new IconBehaviour("NEXT_LVL");
        //static internal  IconBehaviour MAIN_IMAGE_FRAME = new IconBehaviour("MAIN_IMAGE_FRAME");
        //static internal  IconBehaviour HIER = new IconBehaviour("HIER");
        //static internal  IconBehaviour CAT_BG = new IconBehaviour("CAT_BG");

        static GUIStyle _ST_TOGGLE_ON; static GUIStyle ST_TOGGLE_ON {
            get {
                if (_ST_TOGGLE_ON != null) return _ST_TOGGLE_ON;
                _ST_TOGGLE_ON = new GUIStyle();
                _ST_TOGGLE_ON.normal.background = Icons.TOGGLE_E_N;
                _ST_TOGGLE_ON.hover.background =
                _ST_TOGGLE_ON.active.background =
                _ST_TOGGLE_ON.focused.background = Icons.TOGGLE_E_H;
                _ST_TOGGLE_ON.border = new RectOffset(3, 3, 3, 3);
                _ST_TOGGLE_ON.normal.textColor = _ST_TOGGLE_ON.hover.textColor = _ST_TOGGLE_ON.active.textColor = _ST_TOGGLE_ON.focused.textColor = new Color32(0x62, 0x71, 0x79, 255);
                _ST_TOGGLE_ON.fontSize = (int)(_ST_TOGGLE_ON.normal.background.height / 2f);
                _ST_TOGGLE_ON.fontStyle = FontStyle.Bold;
                _ST_TOGGLE_ON.alignment = TextAnchor.MiddleLeft;
                _ST_TOGGLE_ON.padding = new RectOffset(5, 5, 5, 5);
                _ST_TOGGLE_ON.clipping = TextClipping.Clip;
                return _ST_TOGGLE_ON;
            }
        }
        static GUIStyle _ST_TOGGLE_OFF; static GUIStyle ST_TOGGLE_OFF {
            get {
                if (_ST_TOGGLE_OFF != null) return _ST_TOGGLE_OFF;
                _ST_TOGGLE_OFF = new GUIStyle();
                _ST_TOGGLE_OFF.normal.background = Icons.TOGGLE_D_N;
                _ST_TOGGLE_OFF.hover.background =
                _ST_TOGGLE_OFF.active.background =
                _ST_TOGGLE_OFF.focused.background = Icons.TOGGLE_D_H;
                _ST_TOGGLE_OFF.border = new RectOffset(3, 3, 3, 3);
                _ST_TOGGLE_OFF.normal.textColor = _ST_TOGGLE_OFF.hover.textColor = _ST_TOGGLE_OFF.active.textColor = _ST_TOGGLE_OFF.focused.textColor = new Color32(0xd2, 0xdd, 0xe1, 255);
                _ST_TOGGLE_OFF.fontSize = (int)(_ST_TOGGLE_OFF.normal.background.height / 2f);
                _ST_TOGGLE_OFF.fontStyle = FontStyle.Bold;
                _ST_TOGGLE_OFF.alignment = TextAnchor.MiddleLeft;
                _ST_TOGGLE_OFF.padding = new RectOffset(5, 5, 5, 5);
                _ST_TOGGLE_OFF.clipping = TextClipping.Clip;
                return _ST_TOGGLE_OFF;
            }
        }
        static GUIStyle _ST_CAT_NAME; static GUIStyle ST_CAT_NAME {
            get {
                if (_ST_CAT_NAME != null) return _ST_CAT_NAME;
                _ST_CAT_NAME = new GUIStyle();
                _ST_CAT_NAME.normal.textColor = _ST_CAT_NAME.hover.textColor = _ST_CAT_NAME.active.textColor = _ST_CAT_NAME.focused.textColor = new Color32(0xd2, 0xdd, 0xe1, 255);
                _ST_CAT_NAME.fontSize = (int)(ST_TOGGLE_OFF.normal.background.height / 2f);
                _ST_CAT_NAME.fontStyle = FontStyle.Bold;
                _ST_CAT_NAME.alignment = TextAnchor.MiddleLeft;
                _ST_CAT_NAME.padding = new RectOffset(5, 5, 5, 5);
                _ST_CAT_NAME.clipping = TextClipping.Clip;
                return _ST_CAT_NAME;
            }
        }
        static GUIStyle _ST_CAT_NAME_OFF; static GUIStyle ST_CAT_NAME_OFF {
            get {
                if (_ST_CAT_NAME_OFF != null) return _ST_CAT_NAME_OFF;
                _ST_CAT_NAME_OFF = new GUIStyle(ST_CAT_NAME);
                _ST_CAT_NAME_OFF.normal.textColor = _ST_CAT_NAME_OFF.hover.textColor = _ST_CAT_NAME_OFF.active.textColor = _ST_CAT_NAME_OFF.focused.textColor = new Color32(0x42, 0x51, 0x49, 255);
                return _ST_CAT_NAME_OFF;
            }
        }
        static GUIStyle _ST_CAT_sm_NAME; static GUIStyle ST_CAT_sm_NAME {
            get {
                if (_ST_CAT_sm_NAME != null) return _ST_CAT_sm_NAME;
                _ST_CAT_sm_NAME = new GUIStyle(ST_CAT_NAME);
                _ST_CAT_sm_NAME.fontSize -= 2;
                _ST_CAT_sm_NAME.padding.left = 5;
                return _ST_CAT_sm_NAME;
            }
        }
        static GUIStyle _ST_CAT_sm_NAME_OFF; static GUIStyle ST_CAT_sm_NAME_OFF {
            get {
                if (_ST_CAT_sm_NAME_OFF != null) return _ST_CAT_sm_NAME_OFF;
                _ST_CAT_sm_NAME_OFF = new GUIStyle(ST_CAT_NAME);
                _ST_CAT_sm_NAME_OFF.fontSize -= 5;
                _ST_CAT_sm_NAME_OFF.padding.left = 20;
                return _ST_CAT_sm_NAME_OFF;
            }
        }


        static GUIStyle _ST_OPTION_ON; static GUIStyle ST_OPTION_ON {
            get {
                if (_ST_OPTION_ON != null) return _ST_OPTION_ON;
                _ST_OPTION_ON = new GUIStyle();
                _ST_OPTION_ON.normal.background = Icons.OPTION_E_N;
                _ST_OPTION_ON.hover.background =
                _ST_OPTION_ON.active.background =
                _ST_OPTION_ON.focused.background = Icons.OPTION_E_H;
                _ST_OPTION_ON.border = new RectOffset(26, 12, 12, 12);
                _ST_OPTION_ON.padding = new RectOffset(26, 5, 5, 5);
                _ST_OPTION_ON.normal.textColor = _ST_OPTION_ON.hover.textColor = _ST_OPTION_ON.active.textColor = _ST_OPTION_ON.focused.textColor = new Color32(0xd2, 0xdd, 0xe1, 255);
                //_ST_OPTION_ON.fontSize = (int)(ST_TOGGLE_OFF.normal.background.height / 2f);
                _ST_OPTION_ON.fontStyle = FontStyle.Bold;
                _ST_OPTION_ON.alignment = TextAnchor.MiddleLeft;
                _ST_OPTION_ON.clipping = TextClipping.Clip;
                return _ST_OPTION_ON;
            }
        }
        static GUIStyle _ST_OPTION_OFF; static GUIStyle ST_OPTION_OFF {
            get {
                if (_ST_OPTION_OFF != null) return _ST_OPTION_OFF;
                _ST_OPTION_OFF = new GUIStyle(ST_OPTION_ON);
                _ST_OPTION_OFF.normal.background = Icons.OPTION_D_N;
                _ST_OPTION_OFF.hover.background =
                _ST_OPTION_OFF.active.background =
                _ST_OPTION_OFF.focused.background = Icons.OPTION_D_H;
                _ST_OPTION_OFF.normal.textColor = _ST_OPTION_OFF.hover.textColor = _ST_OPTION_OFF.active.textColor = _ST_OPTION_OFF.focused.textColor = new Color32(0x7e, 0x84, 0x86, 255);
                return _ST_OPTION_OFF;
            }
        }

        static GUIStyle _ST_NEXT_LVL; static GUIStyle ST_NEXT_LVL { get { return _ST_NEXT_LVL ?? (_ST_NEXT_LVL = new GUIStyle() { normal = new GUIStyleState() { background = Icons.NEXT_LVL } }); } }
        static GUIStyle _ST_MAIN_IMAGE_FRAME; static GUIStyle ST_MAIN_IMAGE_FRAME { get { return _ST_MAIN_IMAGE_FRAME ?? (_ST_MAIN_IMAGE_FRAME = new GUIStyle() { normal = new GUIStyleState() { background = Icons.MAIN_IMAGE_FRAME }, border = new RectOffset(4, 4, 4, 4) }); } }
        static GUIStyle _ST_HIER; static GUIStyle ST_HIER { get { return _ST_HIER ?? (_ST_HIER = new GUIStyle() { normal = new GUIStyleState() { background = Icons.HIER } }); } }
        //static GUIStyle _CAT_BG; GUIStyle ST_CAT_BG { get { return _CAT_BG ?? (_CAT_BG = new GUIStyle() { normal = new GUIStyleState() { background = Icons.CAT_BG }, border = new RectOffset( 4, 4, 10, 4 ) }); } }
        static GUIStyle _ST_CAT_BG_LEFT; static GUIStyle ST_CAT_BG_LEFT { get { return _ST_CAT_BG_LEFT ?? (_ST_CAT_BG_LEFT = new GUIStyle() { normal = new GUIStyleState() { background = Icons.CAT_BG_LEFT }, border = new RectOffset(4, 50, 20, 4) }); } }
        static GUIStyle _ST_CAT_BG_RIGHT; static GUIStyle ST_CAT_BG_RIGHT { get { return _ST_CAT_BG_RIGHT ?? (_ST_CAT_BG_RIGHT = new GUIStyle() { normal = new GUIStyleState() { background = Icons.CAT_BG_RIGHT }, border = new RectOffset(50, 4, 20, 4) }); } }

        static GUIStyle _ST_SET_BG_LEFT; static GUIStyle ST_SET_BG_LEFT { get { return _ST_SET_BG_LEFT ?? (_ST_SET_BG_LEFT = new GUIStyle() { normal = new GUIStyleState() { background = Icons.SEP_BG_LEFT }, border = new RectOffset(0, 50, 0, 0) }); } }
        static GUIStyle _ST_SET_BG_RIGHT; static GUIStyle ST_SET_BG_RIGHT { get { return _ST_SET_BG_RIGHT ?? (_ST_SET_BG_RIGHT = new GUIStyle() { normal = new GUIStyleState() { background = Icons.SEP_BG_RIGHT }, border = new RectOffset(50, 0, 0, 0) }); } }



        static readonly GUIContent gc = new GUIContent();
        void TOGGLE(string WORD, string label, string tooltip)
        {
            gc.text = label;
            gc.tooltip = label + "/n" + tooltip;
            var new_value = EditorGUILayout.ToggleLeft(gc, SCRIPT.TargetMaterial.IsKeywordEnabled(WORD));
            if (new_value != SCRIPT.TargetMaterial.IsKeywordEnabled(WORD))
            {
                Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                if (new_value) SCRIPT.TargetMaterial.EnableKeyword(WORD);
                else SCRIPT.TargetMaterial.DisableKeyword(WORD);
                FastPostProcessingCamera.reset_buffer = true;
                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
            }
        }

        static readonly Dictionary<int, GUIContent[]> _contents = new Dictionary<int, GUIContent[]>();
        void TOGGLE(string[] WORDS, string label, string tooltip)
        {

            var hash = WORDS[0].GetHashCode();
            for (int i = 1; i < WORDS.Length; i++) hash ^= WORDS[i].GetHashCode();
            if (!_contents.ContainsKey(hash))
            {
                var contents = WORDS.Select(w => new GUIContent(w, w + "/n" + tooltip)).ToList();
                contents.Insert(0, new GUIContent("Disable"));
                _contents.Add(hash, contents.ToArray());
            }


            var old_value = 0;
            for (int i = 1; i < WORDS.Length + 1; i++)
            {
                if (old_value != 0)
                {
                    if (SCRIPT.TargetMaterial.IsKeywordEnabled(WORDS[i - 1])) SCRIPT.TargetMaterial.DisableKeyword(WORDS[i - 1]);
                    continue;
                }
                if (SCRIPT.TargetMaterial.IsKeywordEnabled(WORDS[i - 1]))
                {
                    old_value = i;
                }
            }
            var new_value = GUILayout.Toolbar(old_value, _contents[hash]);
            if (new_value != old_value)
            {
                Undo.RecordObject(SCRIPT.TargetMaterial, "Change Material Value");
                if (old_value != 0)
                {
                    SCRIPT.TargetMaterial.DisableKeyword(WORDS[old_value - 1]);
                    FastPostProcessingCamera.reset_buffer = true;
                }
                if (new_value != 0)
                {
                    SCRIPT.TargetMaterial.EnableKeyword(WORDS[new_value - 1]);
                    FastPostProcessingCamera.reset_buffer = true;
                }

                EditorUtility.SetDirty(SCRIPT.TargetMaterial);
            }
        }




    }






    class Icons
    {




        static string _EditorFolder = null;
        static internal string EditorFolder {
            get {
                if (_EditorFolder == null || !Directory.Exists(_EditorFolder))
                {
                    var tp = AssetDatabase.GetAssetPath(FastPostProcessingHelper.GetMonoScript);
                    if (string.IsNullOrEmpty(tp)) return "";
                    _EditorFolder = tp.Remove(tp.LastIndexOf('/'));
                }
                return _EditorFolder;
            }
        }
        static internal string AssetFolder {
            get {
                var f = EditorFolder;
                f = f.Remove(f.LastIndexOf('/'));
                f = f.Remove(f.LastIndexOf('/'));
                return f;
            }
        }


        static internal Dictionary<string, IconBehaviour> HELP_DIC = new Dictionary<string, IconBehaviour>();

        [NonSerialized] static internal IconBehaviour TOGGLE_E_N = new IconBehaviour("TOGGLE_E_N");
        [NonSerialized] static internal IconBehaviour TOGGLE_E_H = new IconBehaviour("TOGGLE_E_H");
        [NonSerialized] static internal IconBehaviour TOGGLE_D_N = new IconBehaviour("TOGGLE_D_N");
        [NonSerialized] static internal IconBehaviour TOGGLE_D_H = new IconBehaviour("TOGGLE_D_H");

        [NonSerialized] static internal IconBehaviour OPTION_E_N = new IconBehaviour("OPTION_E_N");
        [NonSerialized] static internal IconBehaviour OPTION_E_H = new IconBehaviour("OPTION_E_H");
        [NonSerialized] static internal IconBehaviour OPTION_D_N = new IconBehaviour("OPTION_D_N");
        [NonSerialized] static internal IconBehaviour OPTION_D_H = new IconBehaviour("OPTION_D_H");

        [NonSerialized] static internal IconBehaviour NEXT_LVL = new IconBehaviour("NEXT_LVL");
        [NonSerialized] static internal IconBehaviour MAIN_IMAGE_FRAME = new IconBehaviour("MAIN_IMAGE_FRAME");
        [NonSerialized] static internal IconBehaviour HIER = new IconBehaviour("HIER");
        [NonSerialized] static internal IconBehaviour HIER_A = new IconBehaviour("HIER_A");
        [NonSerialized] static internal IconBehaviour CAT_BG = new IconBehaviour("CAT_BG");
        [NonSerialized] static internal IconBehaviour CAT_BG_LEFT = new IconBehaviour("CAT_BG_LEFT");
        [NonSerialized] static internal IconBehaviour CAT_BG_RIGHT = new IconBehaviour("CAT_BG_RIGHT");

        [NonSerialized] static internal IconBehaviour SEP_BG_LEFT = new IconBehaviour("SEP_BG_LEFT");
        [NonSerialized] static internal IconBehaviour SEP_BG_RIGHT = new IconBehaviour("SEP_BG_RIGHT");


        internal class IconBehaviour
        {
            internal IconBehaviour(string key)
            {
                _mDataKey = key;
            }

            internal static Dictionary<string, string> _cache = null;
            internal static Dictionary<string, string> cache {
                get {
                    if (_cache == null)
                    {
                        _cache = new Dictionary<string, string>();
                        if (!File.Exists(Icons.EditorFolder + "/em.post.icons.pack"))
                        {
                            Debug.LogWarning("Asset cannot find the em.post.icons.pack file");
                            return _cache;
                        }
                        var bts = File.ReadAllLines(Icons.EditorFolder + "/em.post.icons.pack");
                        foreach (var item in bts)
                        {
                            var spl = item.Split(' ');
                            if (spl.Length != 2) continue;
                            _cache.Add(spl[0], spl[1]);
                        }
                    }
                    return _cache;
                }
            }


            readonly string _mDataKey;
            Texture2D _cachedTexture = null;

            public static implicit operator Texture2D(IconBehaviour ib)
            {
                if (ib._cachedTexture) return ib._cachedTexture;

                var id = SessionState.GetInt(FastPostProcessingProfile.PREFS_KEY + "_iconsCache_" + ib._mDataKey, -1);
                var result = EditorUtility.InstanceIDToObject(id) as Texture2D;
                if (!result)
                {

                    if (!cache.ContainsKey(ib._mDataKey))
                    {
                        Debug.LogWarning("Asset cannot find the " + ib._mDataKey + " icon");
                        return Texture2D.whiteTexture;
                    }
                    result = new Texture2D(1, 1, TextureFormat.RGBA32, false, false);
                    result.name = "EM icon";
                    var _mData = cache[ib._mDataKey];
#if !UNITY_2017_1_OR_NEWER
            result.LoadImage( Convert.FromBase64String( _mData ) );
#else
                    ImageConversion.LoadImage(result, Convert.FromBase64String(_mData), false);
#endif
                    result.name = ib._mDataKey;
                    result.alphaIsTransparency = true;
                    result.mipMapBias = 0;
                    result.filterMode = FilterMode.Trilinear;
                    result.wrapMode = TextureWrapMode.Clamp;
                    result.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInBuild;
                    result.Apply();

                    SessionState.SetInt(FastPostProcessingProfile.PREFS_KEY + "_iconsCache_" + ib._mDataKey, result.GetInstanceID());
                }
                return ib._cachedTexture = result;
            }
        }


        // #if UNITY_EDITOR
        //         [MenuItem("TooToo/Create Post Icons")]
        //         static void asD()
        //         {
        //             var files = Directory.GetFiles("Assets/Icons");
        //             StringWriter sw = new StringWriter();
        //             foreach (var f in files)
        //             {
        //                 var t = AssetDatabase.LoadAssetAtPath<Texture2D>(f);
        //                 if (!t) continue;
        //                 var value = Convert.ToBase64String(File.ReadAllBytes(f));
        //                 //var traw = t.data();
        //                 //var qq = new Texture2D( 1, 1, TextureFormat.ARGB32, false, true );
        //                 //qq.LoadRawTextureData( traw );
        //                 //
        //                 //var value = Convert.ToBase64String(ImageConversion.EncodeToPNG(qq));
        //                 var key = t.name;
        //                 sw.WriteLine(key + " " + value);
        //             }
        //             // int r = 0;
        //             foreach (var item in HELP_DIC)
        //             {
        //                 var id = SessionState.GetInt(FastPostProcessingProfile.PREFS_KEY + "_iconsCache_" + item.Key, -1);
        //                 var result = EditorUtility.InstanceIDToObject(id) as Texture2D;
        //                 if (result)
        //                 {
        //                     ScriptableObject.DestroyImmediate(result, true);
        //                     //  r++;
        //                 }
        //             }
        //             foreach (var item in IconBehaviour.cache)
        //             {
        //                 var id = SessionState.GetInt(FastPostProcessingProfile.PREFS_KEY + "_iconsCache_" + item.Key, -1);
        //                 var result = EditorUtility.InstanceIDToObject(id) as Texture2D;
        //                 if (result)
        //                 {
        //                     ScriptableObject.DestroyImmediate(result, true);
        //                     // r++;
        //                 }
        //             }
        //             //Debug.Log("removed " + r);
        //             HELP_DIC.Clear();
        //             IconBehaviour._cache = null;
        //             File.WriteAllText(Icons.EditorFolder + "/em.post.icons.pack", sw.ToString());
        //             AssetDatabase.Refresh();
        //             EditorUtility.RequestScriptReload();
        //         }
        // #endif
    }
}


// NEED_Z + 
// 4 samples +

// used z-depth - (USE_OUTLINE_STROKES too)
// 8/16 samples -
// USE_DEPTH_OF_FIELD_COMPLETE -

// SKIP_LUTS_FOR_BRIGHT_AREAS !





// posterization for luts +
// gradients
// full depth of field
// sharpen +
// noise grain
// detail texture +
// ao +
// glow horizontal / vertical +
// camera movements blur
// fog / volume fog +

