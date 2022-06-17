
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using EM.PostProcessing.Runtime;



namespace EM.PostProcessing.Editor
{
    class PostPresets_DefaultFastMobile : ISupportedPostComponent
    {
        public const string TITLE = "Ultra Fast Post Effects Stack [INTERNAL]";
        public const string DOWNLOAD_MESSAGE = "Download '1000 LUTs' and FastMobileLUT shader";
        public const string DOWNLOAD_LINK = "https://www.assetstore.unity3d.com/#!/content/111954";

        string ISupportedPostComponent.GET_TITLE { get { return TITLE; } }
        string ISupportedPostComponent.GET_DOWNLOAD_MESSAGE { get { return DOWNLOAD_MESSAGE; } }
        string ISupportedPostComponent.GET_DOWNLOAD_LINK { get { return DOWNLOAD_LINK; } }


        public static Type EffectType = null;
        void ISupportedPostComponent.InitializeTypes()
        {   /* foreach (var assembly in Params.AssemblyList) {
           if (AmplifyEffectType == null) AmplifyEffectType = assembly.GetType( "AmplifyColorEffect", false );
         }*/
            //EffectType = Params.GetTypeFromStringName( "LUTsFastMobileCameraScript" );
            EffectType = typeof(EM.PostProcessing.Runtime.FastPostProcessingCamera);
        }

        string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson(((ISupportedPostComponent)this).MonoComponent); }
        void ISupportedPostComponent.CREATE_UNDO(string undoName) { Undo.RecordObject(((ISupportedPostComponent)this).MonoComponent, undoName); }
        void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty(((ISupportedPostComponent)this).MonoComponent); }
        Type ISupportedPostComponent.MonoComponentType { get { return EffectType; } }
        Type ISupportedPostComponent.SecondMonoComponentType { get { return null; } }
        public void SetMonoComponentDefaultParameters()
        {
            var path = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.EndsWith(".png") && p.EndsWith("Photographic Lens 2.png"));
            if (!string.IsNullOrEmpty(path))
            {
                var t = AssetDatabase.LoadAssetAtPath<Texture>(path) as Texture2D;
                if (t) ((ISupportedPostComponent)this).LutTexture = t;
            }
        }
        public void SetSecondMonoComponentDefaultParameters() { }

        MonoBehaviour ISupportedPostComponent.MonoComponent { get; set; } //keep this line
        MonoBehaviour ISupportedPostComponent.SecondMonoComponent { get; set; } //keep this line

        // SKIP //
        bool ISupportedPostComponent.IsAllowToDraw { get { return true; } }
        bool ISupportedPostComponent.AntiAliasEnable { get { return true; } set { } }
        int ISupportedPostComponent.LutEnable {
            get {
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                var res = 0;
                res |= fc.settings.Profile.TargetMaterial.IsKeywordEnabled(_KW.USE_LUT1) ? 1 : 0;
                res |= !fc.settings.Profile.TargetMaterial.IsKeywordEnabled(_KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0) ? 2 : 0;
                return res;
            }
            set {
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                if ((value & 1) != 0) fc.settings.Profile.TargetMaterial.EnableKeyword(_KW.USE_LUT1);
                else fc.settings.Profile.TargetMaterial.DisableKeyword(_KW.USE_LUT1);
                if ((value & 2) != 0) fc.settings.Profile.TargetMaterial.DisableKeyword(_KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0);
                else fc.settings.Profile.TargetMaterial.EnableKeyword(_KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0);

            }
        }
        void ISupportedPostComponent.CameraPreDrawAction() { }
        void ISupportedPostComponent.CameraPostDrawAction() { }
        // SKIP //

        bool ISupportedPostComponent.LutEffectExist { get { return true; } set { } }

        Texture2D ISupportedPostComponent.LutTexture {
            get {
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                if (!fc || !fc.settings.Profile || !fc.settings.Profile.TargetMaterial) return null;
                return fc.settings.Profile.TargetMaterial.GetTexture(_PID._LUT1) as Texture2D;
                // return LutTexture.GetValue(((ISupportedPostComponent)this).MonoComponent, null) as Texture2D; 
            }
            set {
                //LutTexture.SetValue(((ISupportedPostComponent)this).MonoComponent, value, null);
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                if (!fc || !fc.settings.Profile || !fc.settings.Profile.TargetMaterial) return;
                fc.settings.Profile.TargetMaterial.SetTexture(_PID._LUT1, value);
            }
        }

        float ISupportedPostComponent.LutAmount {
            get {
                //return (float)BlendAmount.GetValue(((ISupportedPostComponent)this).MonoComponent, null);
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                if (!fc || !fc.settings.Profile || !fc.settings.Profile.TargetMaterial) return 1;
                return fc.settings.Profile.TargetMaterial.GetFloat(_PID._LUT1_amount);
            }
            set {
                //BlendAmount.SetValue(((ISupportedPostComponent)this).MonoComponent, Mathf.Clamp01(value), null);
                var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
                if (!fc || !fc.settings.Profile || !fc.settings.Profile.TargetMaterial) return;
                fc.settings.Profile.TargetMaterial.SetFloat(_PID._LUT1_amount, value);
            }
        }





        Dictionary<MonoBehaviour, UnityEditor.Editor> p_to_e = new Dictionary<MonoBehaviour, UnityEditor.Editor>();

        bool ISupportedPostComponent.LeftSideGUI(EditorWindow window, float width)
        {
            var c = ((ISupportedPostComponent)this).MonoComponent;

            if (!p_to_e.ContainsKey(c))
                p_to_e.Add(c, UnityEditor.Editor.CreateEditor(c));
            var e = p_to_e[c];
            if (!e)
            {
                GUILayout.Label("Internal Plugin Error", Params.Label);
                return false;
            }


            Params.scroll.x = Params.scrollX;
            Params.scroll.y = Params.scrollY;
            Params.scroll = GUILayout.BeginScrollView(Params.scroll, alwaysShowVertical: true, alwaysShowHorizontal: false);
            Params.scrollX.Set(Params.scroll.x);
            Params.scrollY.Set(Params.scroll.y);
            e.OnInspectorGUI();
            GUILayout.EndScrollView();

            return true;
        }

        void ISupportedPostComponent.TopFastButtonsGUI(EditorWindow window, Rect rect)
        {

            rect.width /= 7;
            rect.height = 40;

            // DrawPostProcessingModelButton("Second LUT", window, rect, 2);
            // rect.x += rect.width;
            // DrawPostProcessingModelButton("Bright/Saturate", window, rect, 0);
            // rect.x += rect.width;
            // DrawPostProcessingModelButton("Glow", window, rect, 1);

            GUILayout.Space(20);
        }


        void DrawPostProcessingModelButton(string name, EditorWindow window, Rect rect, int offset)
        {
            var fc = ((ISupportedPostComponent)this).MonoComponent as FastPostProcessingCamera;
            if (!fc || !fc.settings.Profile || !fc.settings.Profile.TargetMaterial) return;

            var l1 = fc.settings.Profile.TargetMaterial.IsKeywordEnabled(_KW.USE_LUT1);
            var l2 = fc.settings.Profile.TargetMaterial.IsKeywordEnabled(_KW.USE_LUT2);

            var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
            // var enable = ((int)(float)Type.GetValue(currentComponent, null) & (1 << offset)) != 0;
            var enable = l1 || l2;
            if (enable) EditorGUI.DrawRect(rect, new Color(0.33f, 0.6f, 0.8f, 0.4f));
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            if (Params.TransparentButton(rect, Params.CONTENT(name, "enable/disable " + name.Replace('\n', ' '))))
            {
                // var newType = (int)Type.GetValue(currentComponent, null) & ~(1 << offset);
                // if (!enable) newType |= 1 << offset;
                if (!enable)
                {
                    fc.settings.Profile.TargetMaterial.DisableKeyword(_KW.USE_LUT0);
                    fc.settings.Profile.TargetMaterial.EnableKeyword(_KW.USE_LUT1);
                }
                else
                {
                    fc.settings.Profile.TargetMaterial.EnableKeyword(_KW.USE_LUT0);
                    fc.settings.Profile.TargetMaterial.DisableKeyword(_KW.USE_LUT1);
                    fc.settings.Profile.TargetMaterial.DisableKeyword(_KW.USE_LUT2);
                }
                EditorUtility.SetDirty(fc.settings.Profile.TargetMaterial);
                Params.RepaintImages();
            }
        } //! TOP FAST BUTTIONS



        // ////////////////////
        //! FieldsHelper *** //

        PropertyInfo _mBlendAmount;
        PropertyInfo BlendAmount { get { return _mBlendAmount ?? (_mBlendAmount = EffectType.GetProperty("LUT1_Amount", (BindingFlags)int.MaxValue)); } }

        PropertyInfo _mLutTexture;
        PropertyInfo LutTexture { get { return _mLutTexture ?? (_mLutTexture = EffectType.GetProperty("LUT1", (BindingFlags)int.MaxValue)); } }

        PropertyInfo _mType;
        PropertyInfo Type { get { return _mType ?? (_mType = EffectType.GetProperty("Type", (BindingFlags)int.MaxValue)); } }

        //! FieldsHelper *** //
        // ////////////////////
    }
}
