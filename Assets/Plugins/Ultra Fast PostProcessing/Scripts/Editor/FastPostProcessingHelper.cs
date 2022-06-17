using EM.PostProcessing.Runtime;
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif

namespace EM.PostProcessing.Editor
{
    public class FastPostProcessingHelper : ScriptableObject
    {
        public Texture2D DEFAULT_LUT, DEFAULT_SECOND_LUT, DEFAULT_NOISE, PATTERN_TEX;
        public ScriptableObject _default_urp_asset_2019;
        public ScriptableObject _default_urp_data_2019; //ScriptableObject
        public ScriptableObject _default_urp_asset_2021;
        public ScriptableObject _default_urp_data_2021; //ScriptableObject
        public ScriptableObject _baking_helper; //ScriptableObject
        
        //  public ScriptableObject default_urp_asset;
        //  public ScriptableObject default_urp_data; //ScriptableObject
        public FastPostProcessingProfile default_profile;
        //public UnityEditorInternal.AssemblyDefinitionAsset[] assemblies = new UnityEditorInternal.AssemblyDefinitionAsset[0];

        static internal MonoScript GetMonoScript {
            get {
                var id = SessionState.GetInt(FastPostProcessingProfile.PREFS_KEY + "_tempInst", -1);
                var ms = EditorUtility.InstanceIDToObject(id) as MonoScript;
                if (!ms)
                {
                    var inst = FastPostProcessingHelper.CreateInstance(typeof(FastPostProcessingHelper));
                    inst.name = "_tempInst";
                    inst.hideFlags = HideFlags.HideAndDontSave;
                    ms = MonoScript.FromScriptableObject(inst);
                    SessionState.SetInt(FastPostProcessingProfile.PREFS_KEY + "_tempInst", ms.GetInstanceID());
                    if (Application.isPlaying) ScriptableObject.Destroy(inst);
                    else ScriptableObject.DestroyImmediate(inst);
                }
                return ms;
            }
        }
        static internal FastPostProcessingHelperTempInstance GetInstance {
            get {
                var inst = FastPostProcessingHelper.CreateInstance(typeof(FastPostProcessingHelper)) as FastPostProcessingHelper;
                inst.name = "_tempInst";
                inst.hideFlags = HideFlags.HideAndDontSave;

                return new FastPostProcessingHelperTempInstance() { o = inst };
            }
        }
        internal class FastPostProcessingHelperTempInstance
        {
            internal FastPostProcessingHelper o;
            internal void Release()
            {
                if (Application.isPlaying) ScriptableObject.Destroy(o);
                else ScriptableObject.DestroyImmediate(o);
            }
        }




        //static public AssignDefaultURPDatta()
        //{
        //
        //}



    }
}