using System;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif
#if FAST_POSTPROCESSING_URP_USED
using UnityEngine.Rendering.Universal;
#endif
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EM.PostProcessing.Editor")]

namespace EM.PostProcessing.Runtime
{


    [System.Serializable]
    internal enum BufferType
    {
        CameraColor,
        Custom
    }

    [System.Serializable]
    internal enum PostRenderPassEvent
    {
        AfterEverything = 0, BeforeRenderingPostProcessing, BeforeRenderingCameraSpaceUI, BeforeRenderingSkyBox//, AfterRenderingSkybox
    }


    [System.Serializable]
    public struct CameraSettings
    {
        // public bool wasInit;
        public bool? depthEnabled;
        public bool? defaultPostProcessing;
        public int? msaa;
    }

    [System.Serializable]
    public class Settings
    {
        [SerializeField] internal PostRenderPassEvent RenderPassEvent = PostRenderPassEvent.AfterEverything;
        [SerializeField] public FastPostProcessingProfile Profile = null;
        // [SerializeField] public CameraSettings CamSetMem = null;



        [NonSerialized] internal int blitMaterialPassIndex = 0;
        internal TargetTextureSettings TargetTexture = new TargetTextureSettings();

#if FAST_POSTPROCESSING_URP_USED
        public RenderPassEvent RenderPassEventToURP {
            get {
                switch (RenderPassEvent)
                {
                    default:
                        //case PostRenderPassEvent.AfterRenderingPostProcessing:
                        return UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingPostProcessing;
                    case PostRenderPassEvent.BeforeRenderingPostProcessing:
                        // blitPass.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
                        return UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing;
                    case PostRenderPassEvent.BeforeRenderingCameraSpaceUI:
                        return UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingSkybox;
                    case PostRenderPassEvent.BeforeRenderingSkyBox:
                        return UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingSkybox;
                        //case PostRenderPassEvent.AfterRenderingSkybox:
                        //    var source = renderingData.cameraData.resolveFinalTarget ? blitPass. m_AfterPostProcessColor.Identifier() : renderer.cameraColorTarget;
                        //    blitPass.Setup(source, source);
                        //    blitPass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingSkybox;
                        //    break;
                }
            }
        }
#endif
        public CameraEvent RenderPassEventToDEFAULT {
            get {
                switch (RenderPassEvent)
                {
                    default:
                        //case PostRenderPassEvent.AfterRenderingPostProcessing:
                        return CameraEvent.BeforeImageEffects;
                        //       return CameraEvent.AfterImageEffects;
                        //   case PostRenderPassEvent.BeforeRenderingPostProcessing:
                        //       // blitPass.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
                        //       return CameraEvent.BeforeImageEffects;
                        //   case PostRenderPassEvent.BeforeRenderingCameraSpaceUI:
                        //       return CameraEvent.AfterSkybox;
                        //   case PostRenderPassEvent.BeforeRenderingSkyBox:
                        //       return CameraEvent.BeforeSkybox;
                        //case PostRenderPassEvent.AfterRenderingSkybox:
                        //    var source = renderingData.cameraData.resolveFinalTarget ? blitPass. m_AfterPostProcessColor.Identifier() : renderer.cameraColorTarget;
                        //    blitPass.Setup(source, source);
                        //    blitPass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingSkybox;
                        //    break;
                }
            }
        }
    }
    [System.Serializable]
    internal class TargetTextureSettings
    {
        internal BufferType sourceType = BufferType.CameraColor;
        internal BufferType destinationType = BufferType.CameraColor;
        internal string sourceTextureId = "_SourceTexture";
        internal string destinationTextureId = "_DestinationTexture";
    }



#if FAST_POSTPROCESSING_URP_USED
    public class FastPostProcessingRenderFeature : ScriptableRendererFeature
    {



        //[SerializeField]
        //internal Settings settings = new Settings();
        [NonSerialized]
        PrettyFastPostProcessing_URP_Pass blitPass;

        public override void Create()
        {
            name = "Fast Post Processing";
            blitPass = new PrettyFastPostProcessing_URP_Pass("Fast Post Processing");
            //blitPass.m_AfterPostProcessColor.Init("_AfterPostProcessTexture");
        }

#if UNITY_2021_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            base.SetupRenderPasses(renderer, renderingData);
            blitPass.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);

        }
#endif
        //public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        //{
        //    if (Profile == null)
        //    {
        //        //Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
        //        return;
        //    }
        //
        //    blitPass.renderPassEvent = RenderPassEvent;
        //    blitPass.Profile = Profile;
        //    blitPass.RenderPassEvent = RenderPassEvent;
        //    blitPass.settings = settings;
        //    renderer.EnqueuePass(blitPass);
        //}

        // Only inject passes if post processing is enabled
        //if (renderingData.cameraData.postProcessEnabled)
        //{
        // For each pass, only inject if there is at least one custom post-processing renderer class in it.
        // if (m_AfterOpaqueAndSky.HasPostProcessRenderers && m_AfterOpaqueAndSky.PrepareRenderers(ref renderingData))
        // {
        //     m_AfterOpaqueAndSky.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
        //     renderer.EnqueuePass(m_AfterOpaqueAndSky);
        // }
        // if (m_BeforePostProcess.HasPostProcessRenderers && m_BeforePostProcess.PrepareRenderers(ref renderingData))
        // {
        //     m_BeforePostProcess.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
        //     renderer.EnqueuePass(m_BeforePostProcess);
        // }
        // if (m_AfterPostProcess.HasPostProcessRenderers && m_AfterPostProcess.PrepareRenderers(ref renderingData))
        // {
        //     // If this camera resolve to the final target, then both the source & destination will be "_AfterPostProcessTexture" (Note: a final blit/post pass is added by the renderer).
        //     var source = renderingData.cameraData.resolveFinalTarget ? m_AfterPostProcessColor.Identifier() : renderer.cameraColorTarget;
        //     m_AfterPostProcess.Setup(source, source);
        //     renderer.EnqueuePass(m_AfterPostProcess);
        // }
        // }


        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif


            var scr = FastPostProcessingCamera.GET_SCRIPT(renderingData.cameraData.camera);
            if (!scr) return;

            if (scr.settings.Profile == null) return;


            // blitPass.settings = settings;

            if (!blitPass.PrepareRenderers(ref renderingData)) return;

#if !UNITY_2021_1_OR_NEWER
            blitPass.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
#endif

            blitPass.renderPassEvent = scr.settings.RenderPassEventToURP;

            renderer.EnqueuePass(blitPass);
        }

    }



    internal class PrettyFastPostProcessing_URP_Pass : ScriptableRenderPass
    {
        // internal FilterMode filterMode { get; set; }
        //internal Settings settings;
        //internal FastPostProcessingRenderFeature.PostRenderPassEvent RenderPassEvent = FastPostProcessingRenderFeature.PostRenderPassEvent.AfterRenderingPostProcessing;
        // internal FastPostProcessingProfile Profile = null;
        //string m_ProfilerTag;
        // 







        // private CustomPostProcessInjectionPoint injectionPoint;

        // private List<ProfilingSampler> m_ProfilingSamplers;
        //  public bool HasPostProcessRenderers => m_PostProcessRenderers.Count != 0;








        //internal RenderTargetHandle m_AfterPostProcessColor;
        ICommandBuffer b;
        public PrettyFastPostProcessing_URP_Pass(string tag)
        {
            //    m_ProfilerTag = tag;
            //    m_PassName = tag;
            //}
            //public CustomPostProcessRenderPass(CustomPostProcessInjectionPoint injectionPoint, List<CustomPostProcessRenderer> renderers)
            //{
            //    this.injectionPoint = injectionPoint;
            //    this.m_ProfilingSamplers = new List<ProfilingSampler>(renderers.Count);
            //    this.m_PostProcessRenderers = renderers;
            //    foreach (var renderer in renderers)
            //    {
            //        // Get renderer name and add it to the names list
            //        var attribute = CustomPostProcessAttribute.GetAttribute(renderer.GetType());
            //        m_ProfilingSamplers.Add(new ProfilingSampler(attribute?.Name));
            //    }
            //    // Pre-allocate a list for active renderers
            //    this.m_ActivePostProcessRenderers = new List<int>(renderers.Count);
            //    // Set render pass event and name based on the injection point.
            //    switch (injectionPoint)
            //    {
            //        case CustomPostProcessInjectionPoint.AfterOpaqueAndSky:
            //            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            //            m_PassName = "Custom PostProcess after Opaque & Sky";
            //            break;
            //        case CustomPostProcessInjectionPoint.BeforePostProcess:
            //            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            //            m_PassName = "Custom PostProcess before PostProcess";
            //            break;
            //        case CustomPostProcessInjectionPoint.AfterPostProcess:
            //            // NOTE: This was initially "AfterRenderingPostProcessing" but it made the builtin post-processing to blit directly to the camera target.
            //            renderPassEvent = RenderPassEvent.AfterRendering;
            //            m_PassName = "Custom PostProcess after PostProcess";
            //            break;
            //    }
            // Initialize the IDs and allocation state of the intermediate render targets
            b = new ICommandBuffer(tag); //, Blit5, Blit3
        }


        //void Blit5(CommandBuffer a, RenderTargetIdentifier b, RenderTargetIdentifier c, Material d, int e)
        //{
        //    Blit(a, b, c, d, settings.blitMaterialPassIndex);
        //}
        //void Blit3(CommandBuffer a, RenderTargetIdentifier b, RenderTargetIdentifier c)
        //{
        //    Blit(a, b, c);
        //}



#if !UNITY_2020_1_OR_NEWER


        //private FastPostProcessingRenderFeature m_AfterOpaqueAndSky, m_BeforePostProcess, m_AfterPostProcess;
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            if (!_camera_setup_1(cmd, ref renderingData)) return;
            _camera_setup_2(cmd, ref renderingData);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        // public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        // {
        // }
        [SerializeField] RenderingData renderingData;
        public bool PrepareRenderers(ref RenderingData renderingData)
        {
            this.renderingData = renderingData;
            return true;
        }

#else

        public bool PrepareRenderers(ref RenderingData renderingData)
        {
            // See if current camera is a scene view camera to skip renderers with "visibleInSceneView" = false.
            // bool isSceneView = renderingData.cameraData.cameraType == CameraType.SceneView;

            // Here, we will collect the inputs needed by all the custom post processing effects
            ScriptableRenderPassInput passInput = ScriptableRenderPassInput.None;

            // Collect the active renderers
            //m_ActivePostProcessRenderers.Clear();
            //for (int index = 0; index < m_PostProcessRenderers.Count; index++)
            //{
            //    var ppRenderer = m_PostProcessRenderers[index];
            //    // Skips current renderer if "visibleInSceneView" = false and the current camera is a scene view camera. 
            //    if (isSceneView && !ppRenderer.visibleInSceneView) continue;
            //    // Setup the camera for the renderer and if it will render anything, add to active renderers and get its required inputs
            //    if (ppRenderer.Setup(ref renderingData, injectionPoint))
            //    {
            //        m_ActivePostProcessRenderers.Add(index);
            //        passInput |= ppRenderer.input;
            //    }
            //}

            // Configure the pass to tell the renderer what inputs we need
            // passInput |= ScriptableRenderPassInput.Depth;
            //renderingData.cameraData.camera.depthTextureMode = DepthTextureMode.None;
            ConfigureInput(passInput);
            // return if no renderers are active
            //return m_ActivePostProcessRenderers.Count != 0;
            return true;
        }

        


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (!_camera_setup_1(cmd, ref renderingData)) return;
            PrepareRenderers(ref renderingData);
            _camera_setup_2(cmd, ref renderingData);
            base.OnCameraSetup(cmd, ref renderingData);
        }
#endif

        bool _camera_setup_1(CommandBuffer cmd, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return false;
#endif
            var scr = FastPostProcessingCamera.GET_SCRIPT(renderingData.cameraData.camera);
            if (!scr) return false;
            if (!scr.settings.Profile) return false;
            return true;
        }
        void _camera_setup_2(CommandBuffer cmd, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            var scr = FastPostProcessingCamera.GET_SCRIPT(renderingData.cameraData.camera);
            b.OnPreRenderSetup(cmd, ref renderingData, scr.settings);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            var scr = FastPostProcessingCamera.GET_SCRIPT(renderingData.cameraData.camera);
            if (!scr) return;
            if (!scr.settings.Profile) return;
            b.Execute(context, ref renderingData, scr.settings.Profile, scr.settings, this);
        }

        private void CleanupIntermediate(CommandBuffer cmd)
        {
            b.CleanupIntermediate(cmd);
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            b.FrameCleanup(cmd);
        }
        public void Setup(RenderTargetIdentifier rendererCameraColorTarget, RenderTargetIdentifier cameraColorTarget)
        {
            b.Setup(rendererCameraColorTarget, cameraColorTarget);
        }

        //RenderTargetIdentifier source;
        // RenderTargetIdentifier destination;
        //int temporaryRTId = Shader.PropertyToID("_TempRT");
        //private RenderTargetHandle destination;
        //private RenderTargetHandle source_temp_rt;

        // int sourceId;
        // int destinationId;



        // RenderTargetIdentifier source;
        //// RenderTargetIdentifier destination;
        // int temporaryRTId = Shader.PropertyToID("_TempRT");
        // private RenderTargetHandle destination;
        // private RenderTargetHandle source_temp_rt;
        //
        // int sourceId;
        // int destinationId;
        // 
        //bool isSourceAndDestinationSameTarget;
        // 
        //
        // 
        // 
        // 
        // 
        //
        // //new
        // public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        // {
        //     this.source = source;
        //     this.destination = destination;
        // }
        // public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        // {
        //     // Can't read and write to same color target, create a temp render target to blit.
        //     if (destination == RenderTargetHandle.CameraTarget)
        //     {
        //         // Create a temporary render texture that maches the camera
        //         //cmd.GetTemporaryRT(m_TemporaryColorTexture.id, cameraTextureDescriptor, filterMode);
        //         cmd.GetTemporaryRT(source_temp_rt.id, cameraTextureDescriptor);
        //     }
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     //RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        //     //blitTargetDescriptor.depthBufferBits = 0;
        //     //
        //     //isSourceAndDestinationSameTarget = settings.TargetTexture.sourceType == settings.TargetTexture.destinationType &&
        //     //    (settings.TargetTexture.sourceType == BufferType.CameraColor || settings.TargetTexture.sourceTextureId == settings.TargetTexture.destinationTextureId);
        //     //
        //     //var renderer = renderingData.cameraData.renderer;
        //     //
        //     //
        //     //if (settings.TargetTexture.sourceType == BufferType.CameraColor)
        //     //{
        //     //    sourceId = -1;
        //     //    source = renderer.cameraColorTarget;
        //     //}
        //     //else
        //     //{
        //     //    sourceId = Shader.PropertyToID(settings.TargetTexture.sourceTextureId);
        //     //    cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, filterMode);
        //     //    source = new RenderTargetIdentifier(sourceId);
        //     //}
        //     //
        //     //if (isSourceAndDestinationSameTarget)
        //     //{
        //     //    destinationId = temporaryRTId;
        //     //    cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
        //     //    destination = new RenderTargetIdentifier(destinationId);
        //     //}
        //     //else if (settings.TargetTexture.destinationType == BufferType.CameraColor)
        //     //{
        //     //    destinationId = -1;
        //     //    destination = renderer.cameraColorTarget;
        //     //}
        //     //else
        //     //{
        //     //    destinationId = Shader.PropertyToID(settings.TargetTexture.destinationTextureId);
        //     //    cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
        //     //    destination = new RenderTargetIdentifier(destinationId);
        //     //}
        //     //destinations.Init();
        // }
        //
        // /// <inheritdoc/>
        // public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        // {
        //     if (!Profile) return;
        //    
        //     CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
        //
        //     //// Can't read and write to same color target, create a temp render target to blit. 
        //     //if (isSourceAndDestinationSameTarget)
        //     //{
        //     //    Blit(cmd, source, destination, Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //     //    Blit(cmd, destination, source);
        //     //}
        //     //else
        //     //{
        //     //    Blit(cmd, source, destination, Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //     //}
        //     if (isSourceAndDestinationSameTarget)
        //     {
        //         Blit(cmd, source, source_temp_rt.Identifier(), Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //         Blit(cmd, source_temp_rt.Identifier(), source);
        //     }
        //     else
        //     {
        //         Blit(cmd, source, source_temp_rt.Identifier(), Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //     }
        //
        //
        //     context.ExecuteCommandBuffer(cmd);
        //     CommandBufferPool.Release(cmd);
        // }
        //
        // /// <inheritdoc/>
        // public override void FrameCleanup(CommandBuffer cmd)
        // {
        //     //if (destinationId != -1)
        //     //    cmd.ReleaseTemporaryRT(destinationId);
        //     //
        //     //if (source == destination && sourceId != -1)
        //     //    cmd.ReleaseTemporaryRT(sourceId);
        //
        //     if (destination == RenderTargetHandle.CameraTarget)
        //         cmd.ReleaseTemporaryRT(source_temp_rt.id);
        // }

    }

#endif


        class ICommandBuffer
    {
        // private Action<CommandBuffer, RenderTargetIdentifier, RenderTargetIdentifier, Material, int> Blit5;
        // private Action<CommandBuffer, RenderTargetIdentifier, RenderTargetIdentifier> Blit3;
        internal ICommandBuffer(string tag
            //     , Action<CommandBuffer, RenderTargetIdentifier, RenderTargetIdentifier, Material, int> Blit5,
            //  Action<CommandBuffer, RenderTargetIdentifier, RenderTargetIdentifier> Blit3
            )
        {
            m_PassName = tag;
#if FAST_POSTPROCESSING_URP_USED
            m_Intermediate = new RenderTargetHandle[2];
            m_Intermediate[0].Init("_IntermediateRT0");
            m_Intermediate[1].Init("_IntermediateRT1");
            m_IntermediateAllocated = new bool[2];
            m_IntermediateAllocated[0] = false;
            m_IntermediateAllocated[1] = false;
#endif
            //  this.Blit5 = Blit5;
            //  this.Blit3 = Blit3;
        }

        private string m_PassName;

        private RenderTargetIdentifier m_Source; int m_SourceID;
        private RenderTargetIdentifier m_Destination;

#if FAST_POSTPROCESSING_URP_USED
        private RenderTargetHandle[] m_Intermediate;
        private bool[] m_IntermediateAllocated;
        private RenderTextureDescriptor m_IntermediateDesc;
        private RenderTargetIdentifier GetIntermediate(CommandBuffer cmd, int index)
        {
            if (!m_IntermediateAllocated[index])
            {
                // m_IntermediateDesc.width = 50;
                // m_IntermediateDesc.height = 50;

                // cmd.GetTemporaryRT(m_Intermediate[index].id, m_IntermediateDesc, FilterMode.Point);
                CreateTemporaryRT(cmd, m_Intermediate[index].id, m_IntermediateDesc); //, filterMode
                m_IntermediateAllocated[index] = true;
            }
            return m_Intermediate[index].Identifier();
        }


        internal void CleanupIntermediate(CommandBuffer cmd)
        {
            for (int index = 0; index < 2; ++index)
            {
                if (m_IntermediateAllocated[index])
                {
                    cmd.ReleaseTemporaryRT(m_Intermediate[index].id);
                    m_IntermediateAllocated[index] = false;
                }
            }
        }
#endif

        internal void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

#if FAST_POSTPROCESSING_URP_USED
            CleanupIntermediate(cmd);
#endif

            if (sourceId != -1)
            {
                cmd.ReleaseTemporaryRT(sourceId);
                sourceId = -1;
            }

            if (destinationId != -1)
            {
                cmd.ReleaseTemporaryRT(destinationId);
                destinationId = -1;
            }

            // if (destination != RenderTargetHandle.CameraTarget)
            // {
            //     cmd.ReleaseTemporaryRT(destination.id);
            //     destination = RenderTargetHandle.CameraTarget;
            // }
        }




        // context.ExecuteCommandBuffer(cmd);
        // cmd.Clear();

        //int width = m_IntermediateDesc.width;
        //int height = m_IntermediateDesc.height;
        //cmd.SetGlobalVector("_ScreenSize", new Vector4(width, height, 1.0f / width, 1.0f / height));

        // The variable will be true if the last renderer couldn't blit to destination.
        // This happens if there is only 1 renderer and the source is the same as the destination.
        // bool requireBlitBack = false;
        // // The current intermediate RT to use as a source.
        // int intermediateIndex = 0;
        //
        // for (int index = 0; index < m_ActivePostProcessRenderers.Count; ++index)
        // {
        //     var rendererIndex = m_ActivePostProcessRenderers[index];
        //     var renderer = m_PostProcessRenderers[rendererIndex];
        //
        //     RenderTargetIdentifier source, destination;
        //     if (index == 0)
        //     {
        //         // If this is the first renderers then the source will be the external source (not intermediate).
        //         source = m_Source;
        //         if (m_ActivePostProcessRenderers.Count == 1)
        //         {
        //             // There is only one renderer, check if the source is the same as the destination
        //             if (m_Source == m_Destination)
        //             {
        //                 // Since we can't bind the same RT as a texture and a render target at the same time, we will blit to an intermediate RT.
        //                 destination = GetIntermediate(cmd, 0);
        //                 // Then we will blit back to the destination.
        //                 requireBlitBack = true;
        //             }
        //             else
        //             {
        //                 // Otherwise, we can directly blit from source to destination.
        //                 destination = m_Destination;
        //             }
        //         }
        //         else
        //         {
        //             // If there is more than one renderer, we will need to the intermediate RT anyway.
        //             destination = GetIntermediate(cmd, intermediateIndex);
        //         }
        //     }
        //     else
        //     {
        //         // If this is not the first renderer, we will want to the read from the intermediate RT.
        //         source = GetIntermediate(cmd, intermediateIndex);
        //         if (index == m_ActivePostProcessRenderers.Count - 1)
        //         {
        //             // If this is the last renderer, blit to the destination directly.
        //             destination = m_Destination;
        //         }
        //         else
        //         {
        //             // Otherwise, flip the intermediate RT index and set as destination.
        //             // This will act as a ping pong process between the 2 RT where color data keeps moving back and forth while being processed on each pass.
        //             intermediateIndex = 1 - intermediateIndex;
        //             destination = GetIntermediate(cmd, intermediateIndex);
        //         }
        //     }
        //
        //     using (new ProfilingScope(cmd, m_ProfilingSamplers[rendererIndex]))
        //     {
        //         // If the renderer was not already initialized, initialize it.
        //         if (!renderer.Initialized)
        //             renderer.InitializeInternal();
        //         // Execute the renderer.
        //         renderer.Render(cmd, source, destination, ref renderingData, injectionPoint);
        //     }
        //
        // }
        //
        // // If blit back is needed, blit from the intermediate RT to the destination (see above for explanation)
        // if (requireBlitBack)
        //     Blit(cmd, m_Intermediate[0].Identifier(), m_Destination);



        //// Can't read and write to same color target, create a temp render target to blit. 
        //if (isSourceAndDestinationSameTarget)
        //{
        //    Blit(cmd, source, destination, Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //    Blit(cmd, destination, source);
        //}
        //else
        //{
        //    Blit(cmd, source, destination, Profile.TargetMaterial, settings.blitMaterialPassIndex);
        //}

        internal void Execute(CommandBuffer cmd, Camera mainCam, FastPostProcessingProfile Profile)
        {

#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            //cmd.SetRenderTarget(mainCam.activeTexture);

            //cmd.SetRenderTarget(Graphics.activeColorBuffer, BuiltinRenderTextureType.CameraTarget);
            //cmd.Clear();

            // commandBuffer.GetTemporaryRT(screenTexPropId, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            //
            // commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenRtId);
            // commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive);
            //
            // commandBuffer.ReleaseTemporaryRT(screenTexPropId);

            // b.Execute(context, ref r
            // enderingData, settings.Profile, settings);

            // cmd.GetTemporaryRT(m_SourceID, -1, -1, 0, FM, RenderTextureFormat.ARGB32);
            cmd.BeginSample("FastPostProcessingRenderSinglePass");

            // cmd.DrawRenderer(testRenderer, Profile.RenderImage(mainCam, mainCam.pixelWidth, mainCam.pixelHeight), 0, 0);

            // mBlurCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, mBlurTempRT1);
            //
            // for (int i = 0; i < sampleNum - 1; i++)
            // {
            //     mBlurCommandBuffer.Blit(mBlurTempRT1, BuiltinRenderTextureType.CameraTarget, blurMaterial);
            //     mBlurCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, mBlurTempRT1);
            // }
            //
            // mBlurCommandBuffer.Blit(mBlurTempRT1, BuiltinRenderTextureType.CameraTarget, blurMaterial);
            //#if UNITY_2021_1_OR_NEWER
            //          cmd.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive,
            //             Profile.RenderImage(mainCam, mainCam.pixelWidth, mainCam.pixelHeight));
            //#else
            var bt = Shader.PropertyToID("FastPostProcessinTempRT1");
            cmd.GetTemporaryRT(bt, -1, -1);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, bt, Profile.RenderImage(mainCam, mainCam.pixelWidth, mainCam.pixelHeight));
            // cmd.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive);
            cmd.Blit(bt, BuiltinRenderTextureType.CameraTarget);
            cmd.ReleaseTemporaryRT(bt);
            //#endif

            //  cmd.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, Profile.TargetMaterial);

            cmd.EndSample("FastPostProcessingRenderSinglePass");
            // cmd.ReleaseTemporaryRT(m_SourceID);

        }


        internal void OnPreRenderSetup(CommandBuffer cmd, Camera mainCam, Settings settings)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            if (!settings.Profile) return;
            sourceId = -1;
            settings.Profile.PreRender(mainCam);
        }

#if FAST_POSTPROCESSING_URP_USED
        internal void Execute(
            ScriptableRenderContext context,
            ref RenderingData renderingData,
            FastPostProcessingProfile Profile,
            Settings settings, PrettyFastPostProcessing_URP_Pass a)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            if (!Profile) return;

            // Copy camera target description for intermediate RTs. Disable multisampling and depth buffer for the intermediate targets.
            m_IntermediateDesc = renderingData.cameraData.cameraTargetDescriptor;
            m_IntermediateDesc.msaaSamples = 1; //1
            m_IntermediateDesc.depthBufferBits = 0;

            CommandBuffer cmd = CommandBufferPool.Get(m_PassName);
            cmd.BeginSample("FastPostProcessingRenderSinglePass");

            var m = Profile.RenderImage(renderingData.cameraData.camera, m_IntermediateDesc.width, m_IntermediateDesc.height);

            if (isSourceAndDestinationSameTarget)
            {
                //  Debug.Log("ASD");
                var im = GetIntermediate(cmd, 0);
                a.Blit(cmd, m_Source, im, m, settings.blitMaterialPassIndex);
                a.Blit(cmd, im, m_Destination);
                //Blit5(cmd, m_Source, im, m, settings.blitMaterialPassIndex);
                //Blit3(cmd, im, m_Destination);
            }
            else
            {
                a.Blit(cmd, m_Source, m_Destination);
                //Blit5(cmd, m_Source, m_Destination, m, settings.blitMaterialPassIndex);
            }



            // Release allocated Intermediate RTs.
            //  CleanupIntermediate(cmd);
            cmd.EndSample("FastPostProcessingRenderSinglePass");

            // Send command buffer for execution, then release it.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }



        internal void OnPreRenderSetup(CommandBuffer cmd, ref RenderingData renderingData, Settings settings)
        {

            if (!settings.Profile) return;
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif

            isSourceAndDestinationSameTarget = settings.TargetTexture.sourceType == settings.TargetTexture.destinationType &&
                                               (settings.TargetTexture.sourceType == BufferType.CameraColor
                                               || settings.TargetTexture.sourceTextureId == settings.TargetTexture.destinationTextureId);

            m_IntermediateDesc = renderingData.cameraData.cameraTargetDescriptor;
            m_IntermediateDesc.msaaSamples = 0;
            m_IntermediateDesc.depthBufferBits = 0;

            // var renderer = renderingData.cameraData.renderer;
            if (settings.TargetTexture.sourceType == BufferType.CameraColor)
            {
                sourceId = -1;
                // m_Source = renderer.cameraColorTarget;
            }
            else
            {
                sourceId = Shader.PropertyToID(settings.TargetTexture.sourceTextureId);
                CreateTemporaryRT(cmd, sourceId, m_IntermediateDesc); //, filterMode
                m_Source = new RenderTargetIdentifier(sourceId);
            }
            if (isSourceAndDestinationSameTarget)
            {
                //destinationId = temporaryRTId;
                //cmd.GetTemporaryRT(destinationId, m_IntermediateDesc, filterMode);
                //destination = new RenderTargetIdentifier(destinationId);
            }
            else if (settings.TargetTexture.destinationType == BufferType.CameraColor)
            {
                destinationId = -1;
                //destination = renderer.cameraColorTarget;
            }
            else
            {
                destinationId = Shader.PropertyToID(settings.TargetTexture.destinationTextureId);
                // cmd.GetTemporaryRT(destinationId, m_IntermediateDesc, filterMode);
                CreateTemporaryRT(cmd, destinationId, m_IntermediateDesc); //, filterMode
                m_Destination = new RenderTargetIdentifier(destinationId);
                //m_Destination = m_IntermediateDesc;
            }
            // destinations.Init();

            settings.Profile.PreRender(renderingData.cameraData.camera);

        }
#endif
        int sourceId = -1, destinationId = -1;
        bool isSourceAndDestinationSameTarget;

        FilterMode FM = FilterMode.Point;// FilterMode.Point; FilterMode.Bilinear;
        // private void CreateTemporaryRT(PostProcessRenderContext context, int nameID, int width, int height, RenderTextureFormat RTFormat)
        private void CreateTemporaryRT(CommandBuffer cmd, int nameID, RenderTextureDescriptor rtDesc)//,  int width, int height, RenderTextureFormat RTFormat)
        {
            // var cmd = context.command;
            //  var rtDesc = context.GetDescriptor(0, RTFormat, RenderTextureReadWrite.Linear);
            // rtDesc.width = width;
            // rtDesc.height = height;

#if UNITY_2019_1_OR_NEWER
            cmd.GetTemporaryRT(nameID, rtDesc, FM);
#elif UNITY_2017_3_OR_NEWER
            cmd.GetTemporaryRT(nameID, rtDesc.width, rtDesc.height, rtDesc.depthBufferBits, FM, rtDesc.colorFormat, RenderTextureReadWrite.Linear, rtDesc.msaaSamples, rtDesc.enableRandomWrite, rtDesc.memoryless, context.camera.allowDynamicResolution);
#else
            cmd.GetTemporaryRT(nameID, rtDesc.width, rtDesc.height, rtDesc.depthBufferBits, FM, rtDesc.colorFormat, RenderTextureReadWrite.Linear, rtDesc.msaaSamples, rtDesc.enableRandomWrite, rtDesc.memoryless);
#endif
        }

        public void Setup(int rendererCameraColorTarget, int cameraColorTarget)
        {
            m_SourceID = rendererCameraColorTarget;
            m_Source = rendererCameraColorTarget;
            m_Destination = cameraColorTarget;
        }

        public void Setup(RenderTargetIdentifier rendererCameraColorTarget, RenderTargetIdentifier cameraColorTarget)
        {
            m_Source = rendererCameraColorTarget;
            m_Destination = cameraColorTarget;
        }
    }
}
