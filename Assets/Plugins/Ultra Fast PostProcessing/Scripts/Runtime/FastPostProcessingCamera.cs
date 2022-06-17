using System;
using System.Collections;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif
#if FAST_POSTPROCESSING_URP_USED
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EM.PostProcessing.Editor")]
#endif
namespace EM.PostProcessing.Runtime
{
    [RequireComponent(typeof(Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class FastPostProcessingCamera : MonoBehaviour
    {



#if UNITY_EDITOR
        //static bool lastBuildFlag = false;
        public static bool IsBuildingPlayer()
        {
            // if (lastBuildFlag != UnityEditor.BuildPipeline.isBuildingPlayer)
            // {
            //     lastBuildFlag = UnityEditor.BuildPipeline.isBuildingPlayer;
            //     if (lastBuildFlag && RenderFeatureDisable != null) RenderFeatureDisable();
            // }
            // return true;
            if (UnityEditor.BuildPipeline.isBuildingPlayer) return true;
            return false;
        }
#endif


        [SerializeField] Settings _settings = new Settings();
        public Settings settings { get { return _settings ?? (_settings = new Settings()); } set { _settings = value; } }

        static Hashtable _get_script = new Hashtable();
        internal static FastPostProcessingCamera GET_SCRIPT(Camera cam)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return null;
#endif
            if (_get_script.ContainsKey(cam.GetInstanceID()))
            {
                var res = _get_script[cam.GetInstanceID()] as FastPostProcessingCamera;
                if (res && res.enabled) return res;
            }

            var added = cam.GetComponent<FastPostProcessingCamera>();
            if (!added) added = cam.gameObject.AddComponent<FastPostProcessingCamera>();
            _get_script.Remove(cam.GetInstanceID());
            _get_script.Add(cam.GetInstanceID(), added);
            return added;
        }

        internal static Camera GET_ACTIVE_CAMERA {
            get {
#if UNITY_EDITOR
                if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return null;
#endif
                Camera res = null;
                Camera res_depth = null;
                foreach (var c in Camera.allCameras)
                {
                    if (c.clearFlags == CameraClearFlags.Depth) { if (res_depth == null || c.depth > res_depth.depth) res_depth = c; }
                    else res = c;
                }
                if (!res) res = res_depth;
                return res;
            }
        }
        //  Camera.main

        //private void OnEnable()
        //{
        //    //if ( !TargetMaterial ) CreateTempMaterial();
        //}
        //private void Reset()
        //{
        //    //foreach (var item in GetComponents<Camera>()) if (item.renderingPath == RenderingPath.DeferredLighting || item.renderingPath == RenderingPath.DeferredShading) item.renderingPath = RenderingPath.Forward;
        //}


        // public FastPostProcessingProfile Profile;


        //void OnPreRender()
        //{
        //    if (!Profile) return;
        //    var mainCam = Camera.current;
        //    Profile.PreRender(mainCam);
        //}
        //
        //void OnRenderImage(RenderTexture source, RenderTexture destination)
        //{
        //    if (!Profile) return;
        //    var mainCam = Camera.current;
        //    Profile.TargetMaterial.SetTexture(_PID._MainTex, source);
        //    Graphics.Blit(source, destination, Profile.RenderImage(mainCam, source.width, source.height));
        //}
        FastPostProcessingCamera cache_this;
        private void Awake()
        {
            cache_this = this;
            lastAsset = GraphicsSettings.renderPipelineAsset;
        }

        [NonSerialized] static RenderPipelineAsset lastAsset;
        void Update()
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            if (lastAsset == GraphicsSettings.renderPipelineAsset) return;
            lastAsset = GraphicsSettings.renderPipelineAsset;
            OnDisable();
            if (!cache_this)
            {
                return;
            }
            OnEnable();
        }

        [NonSerialized] static int last_scene = -1;
        [NonSerialized] static Hashtable _activeScripts = new Hashtable();
        [NonSerialized] private CameraEvent _currentEvent = CameraEvent.BeforeLighting;
        [NonSerialized] private Resolution _currentScreenRes = new Resolution();
#if UNITY_EDITOR
        class asset_import : UnityEditor.AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                reset_buffer = true;
            }
        }

#endif
        internal static bool reset_buffer;

        private void OnPreRender() // OnCameraSetup
        {
            if (!enabled) return;

#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
#if UNITY_EDITOR
            if (!settings.Profile && FastPostProcessingProfile.DefaultData.default_profile)
            {
                settings.Profile = FastPostProcessingProfile.DefaultData.default_profile;
                UnityEditor.EditorUtility.SetDirty(settings.Profile);
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            if (!settings.Profile) return;

#if UNITY_EDITOR
            //if (RenderSettings.skybox)
            //{
            //    RenderSettings.skybox.SetInt("_SrcBlendMode", (int)BlendMode.One);
            //    RenderSettings.skybox.SetInt("_DstBlendMode", (int)BlendMode.Zero);
            //}
            //settings.Profile.TargetMaterial.SetInt("_SrcBlendMode", (int)BlendMode.One);
            //settings.Profile.TargetMaterial.SetInt("_DstBlendMode", (int)BlendMode.Zero);
#endif


            var mainCam = Camera.current;
            if (!mainCam) return;
            if ((mainCam.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                if (_activeScripts.Count == 0 && last_scene == gameObject.scene.GetHashCode()) return;
            }
            else
            {
                if (!_activeScripts.ContainsKey(mainCam.GetInstanceID())) _activeScripts.Add(mainCam.GetInstanceID(), mainCam);
            }


            if (_currentEvent != settings.RenderPassEventToDEFAULT || _currentScreenRes.height != mainCam.pixelHeight || _currentScreenRes.width != mainCam.pixelWidth || reset_buffer)
            {
                _currentEvent = settings.RenderPassEventToDEFAULT;
                _currentScreenRes.height = mainCam.scaledPixelHeight;
                _currentScreenRes.width = mainCam.scaledPixelWidth;
                reset_buffer = false;
                CreateCommandBuffer(mainCam);
            }

            b.OnPreRenderSetup(cmd, mainCam, settings);
        }

        string _name = "Fast Post Processing";
        [NonSerialized] CommandBuffer cmd;
        [NonSerialized] ICommandBuffer b;
        private void CreateCommandBuffer(Camera mainCam)
        {
            DestroyCommandBuffer(mainCam);

            //name = "Fast Post Processing";
            b = new ICommandBuffer("Fast Post Processing");
            var screenTexPropID = Shader.PropertyToID("_MainTex");
            b.Setup(screenTexPropID, screenTexPropID);
            // 
#if UNITY_2021_1_OR_NEWER && FAST_POSTPROCESSING_URP_USED
            cmd = CommandBufferPool.Get(_name);
#else
            if (cmd == null) cmd = new CommandBuffer { name = _name };
            else cmd.Clear();
#endif

            b.Execute(cmd, mainCam, settings.Profile);

            mainCam.AddCommandBuffer(settings.RenderPassEventToDEFAULT, cmd);
        }
        private void DestroyCommandBuffer(Camera mainCam)
        {
            if (cmd != null)
            {


                mainCam.RemoveCommandBuffer(settings.RenderPassEventToDEFAULT, cmd);
                cmd.Clear();
                cmd.Dispose();
                cmd = null;
            }


            // Make sure we don't have any duplicates of our command buffer.
            CommandBuffer[] commandBuffers = mainCam.GetCommandBuffers(settings.RenderPassEventToDEFAULT);
            foreach (CommandBuffer cBuffer in commandBuffers)
            {
                if (cBuffer.name == _name)
                {
                    mainCam.RemoveCommandBuffer(settings.RenderPassEventToDEFAULT, cBuffer);
                    cBuffer.Clear();
                    cBuffer.Dispose();
                }
            }
        }


        internal static Action RenderFeatureEnable, RenderFeatureDisable;
        Camera mainCam;
        void OnEnable()
        {
            if (!gameObject.scene.IsValid()) return;
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            mainCam = GetComponent<Camera>();
            if ((mainCam.hideFlags & HideFlags.HideInInspector) != 0) return;
            if (mainCam && !_activeScripts.ContainsKey(mainCam.GetInstanceID())) _activeScripts.Add(mainCam.GetInstanceID(), mainCam);

            needToRestore.Remove(mainCam.GetInstanceID());
            if (RenderFeatureEnable != null) RenderFeatureEnable();
        }


        void OnDisable()
        {
            // Remove events, clean up resources
            //Camera.main.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
            //commandBuffer.Clear();
            //renderTexture.Release();
            //if (!settings.Profile) return;
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            if (!gameObject.scene.IsValid()) return;

            if (!mainCam && !(mainCam = GetComponent<Camera>())) return;
            DestroyCommandBuffer(mainCam);

            if (!_activeScripts.ContainsKey(mainCam.GetInstanceID())) return;
            _activeScripts.Remove(mainCam.GetInstanceID());
            if (!needToRestore.ContainsKey(mainCam.GetInstanceID())) needToRestore.Add(mainCam.GetInstanceID(), mainCam);
            if (_activeScripts.Count == 0)
            {
                if (RenderFeatureDisable != null) RenderFeatureDisable();
                foreach (var v in Resources.FindObjectsOfTypeAll<Camera>())
                {
                    //if (v == mainCam) continue;
                    //var target_buffers = v.GetCommandBuffers(settings.RenderPassEventToDEFAULT);
                    //foreach (var t_b in target_buffers)
                    //    if (t_b.name == cmd.name)
                    //    {
                    //        v.RemoveCommandBuffer(settings.RenderPassEventToDEFAULT, t_b);
                    //        t_b.Clear();
                    //        t_b.Dispose();
                    //    }
                    if ((v.hideFlags & HideFlags.HideInHierarchy) != 0)
                    {
                        v.RemoveAllCommandBuffers();
                        var c = v.GetComponent<FastPostProcessingCamera>();
                        if (c)
                        {
                            if (!Application.isPlaying) DestroyImmediate(c, false);
                            else Destroy(c);
                        }
                    }
                }
            }
            _currentEvent = CameraEvent.BeforeLighting;
            // Debug.Log("OnDisable" + " " + _activeScripts.Count + " " + mainCam.name + "  " + mainCam.hideFlags + " " + needToRestore.Count);

        }
        [NonSerialized] static Hashtable needToRestore = new Hashtable();
        void OnDestroy()
        {
            if (!gameObject.scene.IsValid()) return;

            if (needToRestore.Count > 0 && !gameObject.scene.isLoaded)
            {
                needToRestore.Clear();
            }
            //Debug.Log("OnDestroy" + " " + _activeScripts.Count + " " + mainCam.name + "  " + mainCam.hideFlags + " " + ((bool)gameObject.scene.isLoaded));
        }

        //void Blit5(CommandBuffer a, RenderTargetIdentifier b, RenderTargetIdentifier c, Material d, int e)
        //{
        //    a.Blit(b, c, d, e);
        //    //Profile.TargetMaterial.SetTexture(_PID._MainTex, source);
        //    //Graphics.Blit(source, destination, Profile.RenderImage(mainCam, source.width, source.height));
        //    //Blit(a, b, c, d, settings.blitMaterialPassIndex);
        //}
        //void Blit3(CommandBuffer a, RenderTargetIdentifier b, RenderTargetIdentifier c)
        //{
        //    a.Blit(b, c);
        //}

        // CommandBuffer commandBuffer;

        //var mainCam = GetComponent<Camera>();
        //if (!mainCam) return;
        //CreateCommandBuffer(mainCam);



        // targetRenderer = targetObject.GetComponentInChildren<Renderer>();
        // // Apply for RT
        // renderTexture = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
        //
        // commandBuffer = new CommandBuffer();
        // // Set the Command Buffer render target for the applied RT
        // commandBuffer.SetRenderTarget(renderTexture);
        // //The initial color is set to gray
        // commandBuffer.ClearRenderTarget(true, true, Color.gray);
        //
        // // draw the target object, if there is no replacement material, use your own material
        //
        // Material mat = replaceMaterial == null ? targetRenderer.sharedMaterial : replaceMaterial;
        //
        // commandBuffer.DrawRenderer(targetRenderer, mat);
        //
        // // Then accept the material of the object using this RT as the main texture
        //
        // this.GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture;
        //
        // if (_Material)
        //
        // {
        //     //This is a dangerous way of writing. An RT is used as input and output. It may not be supported on some graphics cards. If it is not lazy like me... or apply for an additional RT
        //     commandBuffer.Blit(renderTexture, renderTexture, _Material);
        // }
        //
        // // directly into the camera's CommandBuffer event queue
        //
        // Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);


    }

}



