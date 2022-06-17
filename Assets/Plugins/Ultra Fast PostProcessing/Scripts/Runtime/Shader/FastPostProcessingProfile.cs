
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif
#if FAST_POSTPROCESSING_URP_USED
using UnityEngine.Rendering.Universal;
#endif

#if UNITY_EDITOR
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EM.PostProcessing.Editor")]
#endif
namespace EM.PostProcessing.Runtime
{

    // [CreateAssetMenu(fileName = "PostProcessing Profile", menuName = "Fast Post-processing Profile")]
    public class FastPostProcessingProfile : ScriptableObject
    {



        // internal const string ASSET_NAME = "Fast Post-processing Stack";
        // internal const string ASSET_NAME = "Ultra Fast Post Effects Stack";
        internal const string ASSET_NAME = "Ultra Fast PostProcessing";
        internal const string SHADER_NAME = "Hidden/Fast PostProcessing Shader Source";
        internal const string MENU_PATH = "Tools/" + ASSET_NAME + "/";
        internal const string PREFS_KEY = "EM.FastPostProcess.";
        internal const int MENU_ORDER = 100000;
        [SerializeField, HideInInspector] internal Material TargetMaterial = null;
        [SerializeField, HideInInspector] public bool Z_FEATURE_IS_USED = true;
        [SerializeField, HideInInspector] public bool FORCE_DISABLE_Z = false;





        public class DefaultDataClass
        {
            public Texture2D DEFAULT_LUT, DEFAULT_SECOND_LUT, DEFAULT_NOISE, PATTERN_TEX;
            public ScriptableObject _default_urp_asset_2019;
            public ScriptableObject _default_urp_data_2019; //ScriptableObject
            public ScriptableObject _default_urp_asset_2021;
            public ScriptableObject _default_urp_data_2021; //ScriptableObject

            public ScriptableObject _baking_helper;

            public ScriptableObject default_urp_asset {
                get {
#if UNITY_2020_1_OR_NEWER
                    return _default_urp_asset_2021;
#else
                    return _default_urp_asset_2019;
#endif
                }
            }
            public ScriptableObject default_urp_data {
                get {
#if UNITY_2020_1_OR_NEWER
                    return _default_urp_data_2021;
#else
                    return _default_urp_data_2019;
#endif
                }
            }



            public FastPostProcessingProfile default_profile;
            //public UnityEditorInternal.AssemblyDefinitionAsset[] assemblies = new UnityEditorInternal.AssemblyDefinitionAsset[0];

            [System.NonSerialized] public bool CanBeSwitchToURP_raw = false;
            [System.NonSerialized] public bool CanBeSwitchToURP = false;
            [System.NonSerialized] public Action SwitchToURP = null;

        }
#if UNITY_EDITOR
       [NonSerialized] public static DefaultDataClass DefaultData = null;
#endif



        void CreateTempMaterial()
        {
            var shader = Shader.Find(SHADER_NAME);
            TargetMaterial = new Material(shader);
            //TargetMaterial.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha );
            //TargetMaterial.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );
            TargetMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            TargetMaterial.SetInt("_ZWrite", 0);
            TargetMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);



        }



        [SerializeField, HideInInspector]
        internal int selected_category = 0;
        [SerializeField, HideInInspector]
        internal float COLUMN_FACTOR = 1 / 1.5f;
        [SerializeField, HideInInspector]
        int[] _set_keys = null;
        [SerializeField, HideInInspector]
        int[] _set_values = null;
        [NonSerialized]
        System.Collections.Hashtable _set = new System.Collections.Hashtable();
        [NonSerialized]
        bool? _set_wasinit = false;
        internal void ClearSet()
        {
            _set_wasinit = null;
            _set.Clear();
        }
        internal bool GET(string k, out int res)
        {
            if (_set_wasinit != true)
            {
                _set_wasinit = true;
                for (int i = 0; i < _set_keys.Length; i++) if (!_set.ContainsKey(_set_keys[i])) _set.Add(_set_keys[i], _set_values[i]);
            }
            var hash = k.GetHashCode();
            res = 0;
            if (!_set.ContainsKey(hash)) return false;
            res = (int)_set[hash];
            return true;
        }
        internal void SET(string k, int val)
        {
            if (_set_wasinit != true)
            {
                _set_wasinit = true;
                for (int _i = 0; _i < _set_keys.Length; _i++) if (!_set.ContainsKey(_set_keys[_i])) _set.Add(_set_keys[_i], _set_values[_i]);
            }
            var hash = k.GetHashCode();
            if (!_set.ContainsKey(hash)) _set.Add(hash, 0);
            _set[hash] = val;
            System.Array.Resize(ref _set_keys, _set.Count);
            System.Array.Resize(ref _set_values, _set.Count);
            int ind = 0;
            foreach (System.Collections.DictionaryEntry item in _set)
            {
                _set_keys[ind] = (int)item.Key;
                _set_values[ind] = (int)item.Value;
            }
        }


        [NonSerialized, HideInInspector] readonly Hashtable _cameraRenders = new Hashtable();
        public static Func<float> GET_DELTA_TIME = () => Time.deltaTime;

        internal void PreRender(Camera mainCam)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return;
#endif
            var eye = (int)mainCam.stereoActiveEye;
            if (eye < 0) eye = ((int)Camera.MonoOrStereoscopicEye.Mono >= 0) ? (int)Camera.MonoOrStereoscopicEye.Mono : 2;
            var id = mainCam.GetInstanceID();
            if (!_cameraRenders.ContainsKey(id)) _cameraRenders.Add(id, new CameraPostRender[3]);
            var arr = (CameraPostRender[])_cameraRenders[id];
            if (eye >= arr.Length) Array.Resize(ref arr, eye + 1);
            if (arr[eye] == null) arr[eye] = new CameraPostRender() { TargetProfile = this };
            arr[eye].TargetProfile = this;
            arr[eye].PreRender(mainCam);
        }
        internal Material RenderImage(Camera mainCam, float width, float height)
        {
#if UNITY_EDITOR
            if (EM.PostProcessing.Runtime.FastPostProcessingCamera.IsBuildingPlayer()) return null;
#endif
            var eye = (int)mainCam.stereoActiveEye;
            if (eye < 0) eye = ((int)Camera.MonoOrStereoscopicEye.Mono >= 0) ? (int)Camera.MonoOrStereoscopicEye.Mono : 2;
            var id = mainCam.GetInstanceID();
            if (!_cameraRenders.ContainsKey(id)) _cameraRenders.Add(id, new CameraPostRender[3]);
            var arr = (CameraPostRender[])_cameraRenders[id];
            if (eye >= arr.Length) Array.Resize(ref arr, eye + 1);
            if (arr[eye] == null) arr[eye] = new CameraPostRender() { TargetProfile = this };
            arr[eye].TargetProfile = this;
            return arr[eye].RenderImage(mainCam, width, height);
        }


        class CmaVelCalc
        {
            readonly Vector3?[] lastCameraForward = new Vector3?[2];
            Vector2 tv2;
            readonly Vector4[] lastCamVelocity = new Vector4[2];
            const int L = 4;
            readonly Vector4?[] averrageCamVel = new Vector4?[L];
            Vector4 tv4 = Vector4.zero;
            float dif { get { return 0.00666f / 0.03f; } } //0.03666f
            //float dif { get { return  GET_DELTA_TIME()/ 0.03666f ; } }
            internal Vector4 GET_CURRENT_ROT(Camera mainCam, Vector3 input)
            {
                //cam velocity
                if (!lastCameraForward[0].HasValue) lastCameraForward[0] = input.normalized;
                lastCameraForward[1] = lastCameraForward[0];
                lastCameraForward[0] = input.normalized;

                // lastCameraForward[0] = increase
                // ? Vector3.SmoothDamp(lastCameraForward[0].Value, mainCam.transform.forward, ref camVelocity, 10, 9999, GET_DELTA_TIME() * 250)
                //     : mainCam.transform.forward;

                //if ((lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude < 0.001f) lastCameraForward[1] = lastCameraForward[0];
                // tv4.w = (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude * dif;
                // if (tv4.w < 0.001) return Vector4.zero;
                //  if (lastCameraForward[0].Value == lastCameraForward[1].Value) return Vector4.zero;
                //tv4 = mainCam.transform.InverseTransformDirection(lastCameraForward[0].Value - lastCameraForward[1].Value).normalized;
                //tv4.w = (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude * dif;
                tv4 = -mainCam.transform.InverseTransformDirection(lastCameraForward[1].Value) * dif;
                tv4.z = tv4.w = 0;
                tv4.w = tv4.magnitude;
                lastCamVelocity[0] = tv4;
                //if (Vector3.Angle(lastCameraForward[0].Value, lastCameraForward[1].Value) < 10) lastCamVelocity[0] = tv4;
                tv2.Set(lastCamVelocity[0].x, lastCamVelocity[0].y);
                var velAngle = Vector2.SignedAngle(tv2, Vector2.up) * Mathf.Deg2Rad;
                lastCamVelocity[0].x = Mathf.Sin(velAngle);
                lastCamVelocity[0].y = Mathf.Cos(velAngle);
                if (lastCamVelocity[0].w < 0.00001f) lastCamVelocity[1] = Vector4.zero;


                // var increase = lastCameraForward[1].HasValue &&
                //                (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude < (lastCameraForward[0].Value - mainCam.transform.forward).magnitude;
                // Vector3.SmoothDamp(lastCameraForward[0].Value, mainCam.transform.forward, ref camVelocity, 10, 9999,
                //     GET_DELTA_TIME() * (increase ? 25 : 20));
                // var res = lastCamVelocity[1];
                for (int i = 1; i < averrageCamVel.Length; i++) averrageCamVel[i] = averrageCamVel[i - 1];
                averrageCamVel[0] = lastCamVelocity[0];
                //  var res = lastCamVelocity[1];
                //  lastCamVelocity[1] = lastCamVelocity[0];
                tv4 = Vector4.zero;
                for (int i = 0; i < averrageCamVel.Length; i++) if (averrageCamVel[i].HasValue) tv4 += averrageCamVel[i].Value;
                tv4 /= averrageCamVel.Length;
                return tv4;
            }
            Plane p = new Plane(Vector3.back, Vector3.forward);
            Ray ray;
            internal Vector4 GET_CURRENT_POS(Camera mainCam, Vector3 input)
            {
                //cam velocity
                if (!lastCameraForward[0].HasValue) lastCameraForward[0] = input;
                lastCameraForward[1] = lastCameraForward[0];
                lastCameraForward[0] = input;

                // lastCameraForward[0] = increase
                // ? Vector3.SmoothDamp(lastCameraForward[0].Value, mainCam.transform.forward, ref camVelocity, 10, 9999, GET_DELTA_TIME() * 250)
                //     : mainCam.transform.forward;

                // if ((lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude < 0.001f) lastCameraForward[1] = lastCameraForward[0];
                tv4.w = (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude * dif;
                // if (tv4.w < 0.001) return Vector4.zero;
                tv4 = mainCam.transform.InverseTransformDirection(lastCameraForward[0].Value - lastCameraForward[1].Value).normalized;
                tv4.w = (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude * dif;
                tv4.w *= Mathf.Clamp01(Mathf.Abs(tv4.z) - 0.5f) + 0.5f;
                tv4.w = Mathf.Min(tv4.w / 4, 0.1f);
                float projectedMag;
                ray.direction = tv4;
                if (!p.Raycast(ray, out projectedMag))
                {
                    ray.direction = -ray.direction;
                    tv4 = -tv4;
                    if (!p.Raycast(ray, out projectedMag))
                    {
                        projectedMag = 3;
                    }
                }
                // float projectedMag;
                // if (tv4.z < 0.1)
                // {
                //     projectedMag = 99;
                //     tv4.z = 0;
                // }
                // else projectedMag = 1 / Mathf.Abs(tv4.z);
                // if (tv4.z < 0) tv4 = -tv4;
                tv4.x *= projectedMag;
                tv4.y *= projectedMag;

                float tan = Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                var accept = (float)mainCam.scaledPixelWidth / mainCam.scaledPixelHeight;
                float h = 1 * tan * 2;
                float w = h * accept * 2;
                tv4.x /= (w - w / 2);
                tv4.y /= (h - h / 2);
                tv4.x += 0.5f;
                tv4.y += 0.5f;
                tv4.x *= 0.5f;
                tv4.y *= 0.5f;
                tv4.x += 0.5f;
                tv4.y += 0.5f;
                lastCamVelocity[0] = tv4;

                // var increase = lastCameraForward[1].HasValue &&
                //                (lastCameraForward[0].Value - lastCameraForward[1].Value).magnitude < (lastCameraForward[0].Value - mainCam.transform.forward).magnitude;
                // Vector3.SmoothDamp(lastCameraForward[0].Value, mainCam.transform.forward, ref camVelocity, 10, 9999,
                //     GET_DELTA_TIME() * (increase ? 25 : 20));
                lastCamVelocity[1] = lastCamVelocity[0];
                var res = lastCamVelocity[1];
                // Debug.Log(res);

                //var L = averrageCamVel.Length;
                for (int i = 1; i < averrageCamVel.Length; i++) averrageCamVel[i] = averrageCamVel[i - 1];
                averrageCamVel[0] = res;
                //  var res = lastCamVelocity[1];
                //  lastCamVelocity[1] = lastCamVelocity[0];
                tv4 = Vector4.zero;
                for (int i = 0; i < averrageCamVel.Length; i++) if (averrageCamVel[i].HasValue) tv4 += averrageCamVel[i].Value;
                tv4 /= averrageCamVel.Length;

                tv4.w *= 3;
                return tv4;
            }
        }


        class CameraPostRender
        {
            [NonSerialized] Texture2D[] last_textures = new Texture2D[10];
            Vector4 tv4 = Vector4.zero;
            Material lastAlphaMaterial;
            bool? lastAlphaValue;
            // internal Settings settings;
            internal FastPostProcessingProfile TargetProfile;
            CmaVelCalc[] cmaVelCalc = null;

            internal void PreRender(Camera mainCam)
            {

                //var m_single_pass_stereo = UnityEditor.PlayerSettings.stereoRenderingPath == UnityEditor.StereoRenderingPath.SinglePass;
                //mainCam.depthTextureMode |= DepthTextureMode.Depth;




                var TargetMaterial = TargetProfile.TargetMaterial;


                if (TargetProfile.FORCE_DISABLE_Z)
                {
#if FAST_POSTPROCESSING_URP_USED
                    var rp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
                    if (rp) rp.supportsCameraDepthTexture = false;
#endif
                    mainCam.depthTextureMode &= ~DepthTextureMode.Depth;
                }
                else
                {
                    if (TargetProfile.Z_FEATURE_IS_USED)
                    {
#if FAST_POSTPROCESSING_URP_USED
                        var rp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
                        if (rp) rp.supportsCameraDepthTexture = true;
#endif
                        mainCam.depthTextureMode |= DepthTextureMode.Depth;
                    }
                }






                var needsAlpha = mainCam.clearFlags == CameraClearFlags.Depth || mainCam.clearFlags == CameraClearFlags.Nothing ||
                    mainCam.clearFlags == CameraClearFlags.Color && mainCam.backgroundColor.a < 1;
                if (lastAlphaMaterial != TargetMaterial || lastAlphaValue != needsAlpha)
                {
                    lastAlphaMaterial = TargetMaterial;
                    lastAlphaValue = needsAlpha;
                    if (needsAlpha) TargetMaterial.EnableKeyword("USE_ALPHA_OUTPUT");
                    else TargetMaterial.DisableKeyword("USE_ALPHA_OUTPUT");
                    TargetProfile.ClearSet();
                }

                if (TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0))
                {



                    TargetMaterial.SetTexture(_PID._LUT1_GRAD, TargetProfile.CurrentGradient.GenerateLUT(TargetMaterial.GetFloat(_PID._LUT1_gradient_smooth)));
                }

                var current_texture = TargetMaterial.GetTexture(TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.APPLY_CUSTOM_GRADIENT_FOR_LUT0) ? _PID._LUT1_GRAD : _PID._LUT1) as Texture2D;
                //var tx = 
                //TargetMaterial.SetTexture(_PID._FinalGradientTexture, tx);
                if (current_texture != last_textures[1])
                {
                    last_textures[1] = current_texture;
                    TargetProfile.ClearSet();
                }
                if (last_textures[1])
                {
                    tv4.Set(1f / last_textures[1].width, 1f / last_textures[1].height, last_textures[1].height - 1f, 0);
                    TargetMaterial.SetVector(_PID._LUT1_params, tv4);
                }

                if (TargetMaterial.GetTexture(_PID._LUT2) != last_textures[2])
                {
                    last_textures[2] = TargetMaterial.GetTexture(_PID._LUT2) as Texture2D;
                    TargetProfile.ClearSet();
                }
                if (last_textures[2])
                {
                    tv4.Set(1f / last_textures[2].width, 1f / last_textures[2].height, last_textures[2].height - 1f, 0);
                    TargetMaterial.SetVector(_PID._LUT2_params, tv4);
                }



                if (cmaVelCalc == null)
                {
                    cmaVelCalc = new CmaVelCalc[2];
                    cmaVelCalc[0] = new CmaVelCalc();
                    cmaVelCalc[1] = new CmaVelCalc();
                }

                TargetMaterial.SetVector(_PID._CameraVelocity, cmaVelCalc[0].GET_CURRENT_ROT(mainCam, mainCam.transform.forward));
                TargetMaterial.SetVector(_PID._CameraDeepVel, cmaVelCalc[1].GET_CURRENT_POS(mainCam, mainCam.transform.position));




                var accept = (float)mainCam.scaledPixelWidth / mainCam.scaledPixelHeight;
                var accept1 = 1 / Mathf.Max(1, 1 / accept);
                var NC = mainCam.nearClipPlane;
                var FC = mainCam.farClipPlane;
                float tan = Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float h = FC * tan;
                float w = h * accept;
                float n_h = NC * tan;
                float n_w = n_h * accept;
                tv4.Set(1f / n_w, 1f / n_h, 1f / w, 0.1f / tan);
                TargetMaterial.SetVector(_PID._DepthCorrection, tv4);

                tv4.Set(Mathf.Lerp(tv4.y, tv4.w, 0.5f), FC, Mathf.Lerp(tv4.x, tv4.z, 0.5f), NC);
                TargetMaterial.SetVector(_PID._Camera_Size, tv4);
                var F = mainCam.transform.forward;
                tv4.Set(F.x, F.y, F.z, -1);
                TargetMaterial.SetVector(_PID._Camera_Forward, tv4);
                //F = mainCam.transform.position;
                //Debug.Log( mainCam.fieldOfView );
                var accept_0 = Mathf.Min(1, accept) * Mathf.Min(1, (float)mainCam.scaledPixelHeight / mainCam.scaledPixelWidth);
                tv4.Set(accept, accept1, /*Mathf.Lerp( 1, accept1, 0.5f ) **/ FC - NC, accept_0);
                TargetMaterial.SetVector(_PID._Camera_Converter, tv4);


                //BRIGHT
                var b_v = TargetMaterial.GetFloat(_PID._b_brightAmount);
                var b_c = TargetMaterial.GetVector(_PID._b_colorsAmount);
                b_c.x *= b_v;
                b_c.y *= b_v;
                b_c.z *= b_v;
                TargetMaterial.SetVector(_PID._b_brightAmountVector, b_c);
                var b_s = TargetMaterial.GetFloat(_PID._b_saturate01);
                TargetMaterial.SetFloat(_PID._b_saturate01Value, (b_s - 1.0f) * 0.5f);

#if UNITY_2021_1_OR_NEWER
                double now = Time.timeAsDouble;
#else
                double now = Time.time;
#endif
#if UNITY_EDITOR
                now = UnityEditor.EditorApplication.timeSinceStartup;
                //if (!Application.isPlaying) tv4.Set((float)(UnityEditor.EditorApplication.timeSinceStartup % 1), (float)((UnityEditor.EditorApplication.timeSinceStartup / 2) % 1), (float)((UnityEditor.EditorApplication.timeSinceStartup / 8) % 1), (float)((UnityEditor.EditorApplication.timeSinceStartup / 32) % 1));
                // else
#endif

                tv4.Set((float)((now) % 1), (float)((now / 2) % 1), (float)((now / 8) % 1), (float)((now / 32) % 1));
                TargetMaterial.SetVector(_PID._Fixed_Time, tv4);

                var _FogTexAnimateSpeed = TargetMaterial.GetFloat(_PID._FogTexAnimateSpeed);
                var _FogTexTile = TargetMaterial.GetVector(_PID._FogTexTile);
                //fixed fog_time_1 = _Time.x * _FogTexAnimateSpeed;
                //fixed2 fog_time = fog_time_1.xx / _FogTexTile;
                tv4.Set(
                    (float)((now * _FogTexAnimateSpeed / _FogTexTile.x / 32 / 2) % 1),
                    (float)((now * _FogTexAnimateSpeed / _FogTexTile.y / 32 / 2) % 1),
                    (float)((now * _FogTexAnimateSpeed / _FogTexTile.x / 32 / 4) % 1),
                    (float)((now * _FogTexAnimateSpeed / _FogTexTile.y / 32 / 4) % 1)
                );
                TargetMaterial.SetVector(_PID._Fog_Time, tv4);




                var wp = mainCam.transform.position;
                np_01.Set(-w, h, FC, 0);
                np_01 = mainCam.transform.TransformPoint(np_01) - wp;
                np_11.Set(w, h, FC, 0);
                np_11 = mainCam.transform.TransformPoint(np_11) - wp;
                np_10.Set(w, -h, FC, 0);
                np_10 = mainCam.transform.TransformPoint(np_10) - wp;
                np_00.Set(-w, -h, FC, 0);
                np_00 = mainCam.transform.TransformPoint(np_00) - wp;
                //np_01.Normalize();
                //np_11.Normalize();
                //np_10.Normalize();
                //np_00.Normalize();
                //np_01 = -np_01;
                //np_11 = -np_11;
                //np_10 = -np_10;
                //np_00 = -np_00;

                //cmt.SetRow( 0, np_01 );
                //cmt.SetRow( 1, np_11 );
                //cmt.SetRow( 2, np_10 );
                //cmt.SetRow( 3, np_00 );
                //3 = 0   2 = 1   0 = 2   1 = 3
                cmt.SetRow(2, np_01);
                cmt.SetRow(3, np_11);
                cmt.SetRow(1, np_10);
                cmt.SetRow(0, np_00);
                TargetMaterial.SetMatrix(_PID._CameraDirLeft, cmt);
                TargetMaterial.SetMatrix(_PID._Rotate2World, Matrix4x4.Rotate(mainCam.transform.rotation));



                var _StartY = TargetMaterial.GetFloat(_PID._StartY);
                var _EndY = TargetMaterial.GetFloat(_PID._EndY);
                var _HeightFogDensity = TargetMaterial.GetFloat(_PID._HeightFogDensity);

                var f_dif = _StartY - _EndY;
                //  var f_dif_clamp = _HeightFogDensity > 0 ? f_dif / _HeightFogDensity : 0;
                var f_dif_clamp = f_dif;
                //f_dif_clamp += (f_dif - f_dif_clamp) / 2;
                // tv4.Set(f_dif, Mathf.Clamp01((f_dif + _EndY - wp.y) / (_StartY - _EndY)), Mathf.Clamp01((f_dif_clamp + _EndY - wp.y) / (_StartY - _EndY)), -1);
                var fog_y = 1 - Mathf.Clamp01((wp.y - (_StartY - f_dif_clamp)) / (f_dif_clamp));
                fog_y = Mathf.Clamp01(fog_y - 0.4f) / 0.6f;
                // Debug.Log(fog_y);

                //var old_f = Mathf.Clamp01((f_dif_clamp + _EndY - wp.y) / (_StartY - _EndY));
                var new_f = Mathf.Clamp01((wp.y - _EndY) / (_StartY - _EndY));
                tv4.Set(f_dif, fog_y, new_f, -1);
                TargetMaterial.SetVector(_PID._FogParams, tv4);




                if (TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.USE_FINAL_GRADIENT_DITHER_ONE_BIT) ||
                   TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.USE_FINAL_GRADIENT_DITHER_MATH) ||
                   TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.USE_FINAL_GRADIENT_DITHER_TEX) ||
                   TargetProfile.TargetMaterial.IsKeywordEnabled(_KW.USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX))
                {
                    // var tx = TargetProfile.CurrentGradient.GenerateTexture(1);
                    // TargetMaterial.SetTexture(_PID._FinalGradientTexture, tx);
                    // var v4 = new Vector4(tx.width, 1f / tx.height, tx.height - 1f, 0);
                    // TargetMaterial.SetVector(_PID._FinalGradientTexture_params, v4);




                    np_01.Set(-0.5f, 0f, -0.375f, 0.125f);
                    cmt.SetRow(0, np_01);
                    np_01.Set(0.25f, -0.25f, 0.375f, -0.125f);
                    cmt.SetRow(1, np_01);
                    np_01.Set(-0.3125f, 0.1875f, -0.4375f, 0.0625f);
                    cmt.SetRow(2, np_01);
                    np_01.Set(0.4375f, -0.0625f, 0.3125f, -0.1875f);
                    cmt.SetRow(3, np_01);
                    TargetMaterial.SetMatrix(_PID._Bayer_Matrix, cmt);
                }

                /*
                Matrix4x4 leftToWorld;
                Matrix4x4 rightToWorld;
                Matrix4x4 leftEye;
                Matrix4x4 rightEye;
                //cache matrices so they can be used in render image step
                if ( mainCam.stereoEnabled )
                {
                    leftToWorld = mainCam.GetStereoViewMatrix( Camera.StereoscopicEye.Left ).inverse;
                    rightToWorld = mainCam.GetStereoViewMatrix( Camera.StereoscopicEye.Right ).inverse;

                    leftEye = mainCam.GetStereoProjectionMatrix( Camera.StereoscopicEye.Left );
                    rightEye = mainCam.GetStereoProjectionMatrix( Camera.StereoscopicEye.Right );

                    // Compensate for RenderTexture...
                    leftEye = GL.GetGPUProjectionMatrix( leftEye, true ).inverse;
                    rightEye = GL.GetGPUProjectionMatrix( rightEye, true ).inverse;

                }
                else
                {
                    leftToWorld = rightToWorld = mainCam.cameraToWorldMatrix;
                    leftEye = rightEye = GL.GetGPUProjectionMatrix( mainCam.projectionMatrix, true ).inverse;
                }
                rightEye[ 1, 1 ] *= -1;
                rightEye[ 1, 1 ] *= -1;
                TargetMaterial.SetMatrix( "_LeftEyeToWorld", leftToWorld );
                TargetMaterial.SetMatrix( "_RightEyeToWorld", rightToWorld );
                TargetMaterial.SetMatrix( "_LeftEyeProjection", leftEye );
                TargetMaterial.SetMatrix( "_RightEyeProjection", rightEye );


                 var p = mainCam.projectionMatrix;
                p[ 2, 3 ] = p[ 3, 2 ] = 0.0f;
                p[ 3, 3 ] = 1.0f;

                // I'll confess I don't understand entirely why this is right,
                // I just kept fiddling with numbers until it worked.
                p = Matrix4x4.Inverse( p * mainCam.worldToCameraMatrix )
                   * Matrix4x4.TRS( new Vector3( 0, 0, -p[ 2, 2 ] ), Quaternion.identity, Vector3.one );


                TargetMaterial.SetMatrix( "_ClipToWorld", p );

                    */

                /*
                var up = mainCam.transform.up;
                var right = mainCam.transform.right;
                var forward = mainCam.transform.forward;

                var near = mainCam.nearClipPlane;
                var fov = mainCam.fieldOfView;
                var aspect = mainCam.pixelHeight / mainCam.pixelWidth;

                float halfHeight = near * Mathf.Tan(fov/2);
                float halfRight = halfHeight * aspect;

                Vector3 toTop = up * halfHeight;
                Vector3 toRight = right * halfRight;

                Vector3 toTopLeft = forward + toTop - toRight;
                Vector3 toBottomLeft = forward - toTop - toRight;
                Vector3 toTopRight = forward + toTop + toRight;
                Vector3 toBottomRight = forward - toTop + toRight;

                toTopLeft /= mainCam.nearClipPlane;
                toBottomLeft /= mainCam.nearClipPlane;
                toTopRight /= mainCam.nearClipPlane;
                toBottomRight /= mainCam.nearClipPlane;

                Matrix4x4 frustumDir = Matrix4x4.identity;
                frustumDir.SetRow( 0, toBottomLeft );
                frustumDir.SetRow( 1, toBottomRight );
                frustumDir.SetRow( 2, toTopLeft );
                frustumDir.SetRow( 3, toTopRight );
                TargetMaterial.SetMatrix( "_FrustumDir", frustumDir );
                */


                //Vector2 sp = new Vector2(0,0);
                //var ray_00 = mainCam.ScreenPointToRay(sp);
                //sp.x = mainCam.scaledPixelWidth;
                //var ray_01 = mainCam.ScreenPointToRay(sp);
                //var ray_11 = mainCam.ScreenPointToRay(sp);
                //var ray_10 = mainCam.ScreenPointToRay(sp);



                //// Main eye inverse view matrix
                //matrices[ 0 ] = mainCam.cameraToWorldMatrix;
                //// Inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
                //matrices[ 1 ] = GL.GetGPUProjectionMatrix( mainCam.projectionMatrix, true ).inverse;
                //// Negate [1,1] to reflect Unity's CBuffer state
                //matrices[ 1 ][ 1, 1 ] *= -1;
                //mat.SetMatrix( "_LeftEyeProjection", leftEye );
                //renderMat.SetMatrixArray( "stereoMatrices", matrices );


            }


            Matrix4x4 cmt;
            Vector4 np_01, np_11, np_10, np_00;
            internal Material RenderImage(Camera mainCam, float width, float height)
            {
                var TargetMaterial = TargetProfile.TargetMaterial;

                tv4.Set(1f / width, 1f / height, width, height);
               // TargetMaterial.SetVector(_PID._MainTex_TexelSize, tv4);
                tv4.Set(Mathf.Clamp01((float)height / width), Mathf.Clamp01((float)width / height), -1, -1);
                TargetMaterial.SetVector(_PID._Texel_Size, tv4);
                return TargetMaterial;
            }




            /*
            var cmd = CommandBufferPool.Get(ProfilerTag);
        // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        var camTransform = camera.transform;

        // Create a transformation that hovers the Quad 1 unit in front of the camera.
        var fullScreenQuadMatrix = Matrix4x4.TRS(camTransform.TransformPoint(Vector3.forward), camTransform.rotation, Vector3.one);
        cmd.DrawMesh(RenderingUtils.fullscreenMesh, fullScreenQuadMatrix, _material, 0, _materialPassIndex);

        // cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        context.ExecuteCommandBuffer(cmd);
            */

        }







        const int TEXTURE_SIZE = 256;
        internal Gradient CurrentGradient {
            get {
                if (!gradient_has_init)
                {
                    gradient_has_init = true;
                    if (_currentGradient == null) _currentGradient = new Gradient();
                    _currentGradient.cameraInstance = GetInstanceID();
                }
                return _currentGradient;
            }
        }
        [NonSerialized] bool gradient_has_init = false;
        [SerializeField] Gradient _currentGradient = null;

        [Serializable]
        internal class Gradient
        {
            [SerializeField]
            internal Color[] colors = new Color[0];
            [SerializeField]
            internal float[] positions = new float[0];
            [SerializeField]
            internal int cameraInstance = -1;

            [NonSerialized] Texture2D[] _cachedTexture = new Texture2D[2];
            [NonSerialized] float[] _cachedTexture_Smooth = new float[2] { -1, -1 };


            Texture2D TryToGetCachedTextureForEditor(int index, float smooth)
            {
#if UNITY_EDITOR
                if (!_cachedTexture[index])
                {
                    _cachedTexture[index] = UnityEditor.EditorUtility.InstanceIDToObject(UnityEditor.SessionState.GetInt(PREFS_KEY + index.ToString() + cameraInstance, -1)) as Texture2D;
                    _cachedTexture_Smooth[index] = UnityEditor.SessionState.GetFloat(PREFS_KEY + index.ToString() + "_smooth_" + cameraInstance, 1);
                }
#else
#endif
                if (_cachedTexture[index] && _cachedTexture_Smooth[index] != smooth)
                {
                    TryToDestroyTexture(index);
                }
                return _cachedTexture[index];
            }

            void TryToDestroyTexture(int index)
            {
#if UNITY_EDITOR
                if (!_cachedTexture[index]) _cachedTexture[index] = UnityEditor.EditorUtility.InstanceIDToObject(UnityEditor.SessionState.GetInt(PREFS_KEY + index.ToString() + cameraInstance, -1)) as Texture2D;
#else
#endif
                if (Application.isPlaying) Destroy(_cachedTexture[index]);
                else DestroyImmediate(_cachedTexture[index], true);

                _cachedTexture_Smooth[index] = -1;
            }

            void WriteCachedTexture(int index, Texture2D output, float smooth)
            {
                _cachedTexture[index] = output;
                _cachedTexture_Smooth[index] = smooth;
#if UNITY_EDITOR
                UnityEditor.SessionState.SetInt(PREFS_KEY + index.ToString() + cameraInstance, output.GetInstanceID());
                UnityEditor.SessionState.SetFloat(PREFS_KEY + index.ToString() + "_smooth_" + cameraInstance, smooth);
#endif
            }




            internal Texture2D GenerateTexture(float smooth)
            {
                if (TryToGetCachedTextureForEditor(0, smooth)) return TryToGetCachedTextureForEditor(0, smooth);

                var temp_colors = new Texture2D(TEXTURE_SIZE, 1, TextureFormat.ARGB32, false);
                temp_colors.name = "EM temp Post Gradient";
                temp_colors.filterMode = FilterMode.Bilinear;
                temp_colors.wrapMode = TextureWrapMode.Clamp;

                int current_index = 0;
                for (int i = 0; i < TEXTURE_SIZE; i++)
                {
                    var global_lerp = i / (TEXTURE_SIZE - 1f);
                    var c = GetColor(global_lerp, ref current_index, smooth);
                    if (!c.HasValue) break;
                    temp_colors.SetPixel(i, 0, c.Value);
                    // output.SetPixel(i, 1, c.Value);
                    // output.SetPixel(i, 2, c.Value);
                }


                temp_colors.Apply(false, false);
                temp_colors.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                //                _cachedTexture[0] = temp_colors;
                //#if UNITY_EDITOR
                //                UnityEditor.SessionState.SetInt(PREFS_KEY + 0.ToString() + cameraInstance, temp_colors.GetInstanceID());
                //#endif
                WriteCachedTexture(0, temp_colors, smooth);

                return temp_colors;
            }

            // [UnityEditor.MenuItem("ttoo/get")]
            // static void ASD()
            // {
            //     string res = "";
            //     for (float i = 0; i <= 1; i+=0.1f)
            //     {
            //         var sign = Mathf.Sign(i - 0.5f);
            //         var lerp = Mathf.Pow(Mathf.Abs(i - 0.5f) * 2, 0.0f) * sign / 2 + 0.5f;
            //         res += lerp + " ";
            //     }
            //     Debug.Log(res);
            // }

            internal Texture2D GenerateLUT(float smooth)
            {

                if (TryToGetCachedTextureForEditor(1, smooth)) return TryToGetCachedTextureForEditor(1, smooth);


                var temp_colors = GenerateTexture(smooth);
                var temp_color_array = temp_colors.GetPixels();

                const int X = 1024;
                const int Y = 32;
                var output = new Texture2D(X, Y, TextureFormat.ARGB32, false);
                output.name = "EM temp Post LUT";

                // for (int i = 0; i < 32; i++)
                // {
                //     var start_x = i * X;
                //     for (int y = 0; y < X; y++)
                //     {
                //         for (int x = 0; x < X; x++)
                //         {
                //             var r = x / 32f;
                //             var g = y / 32f;
                //             var b = i / 32f;
                //             output.SetPixel(x + start_x, y, new Color(r, g, b, 1));
                //         }
                //     }
                // }
                var down = 1 / (0.2125f + 0.7153f + 0.1721f);
                var LuminaceCoeff = new float[3] { 0.2125f * down, 0.7153f * down, 0.1721f * down };

                for (int i = 0; i < 32; i++)
                {
                    var start_x = i * Y;
                    for (int y = 0; y < Y; y++)
                    {
                        for (int x = 0; x < Y; x++)
                        {
                            var r = x / (Y - 1f);
                            var g = y / (Y - 1f);
                            var b = i / (32 - 1f);

                            var pixel = new Color(r, g, b, 1);

                            // var pixel = output.GetPixel(x + start_x, y);
                            var lum = pixel.r * LuminaceCoeff[0] + pixel.g * LuminaceCoeff[1] + pixel.b * LuminaceCoeff[2];
                            var lum_index = Mathf.Clamp(Mathf.RoundToInt(lum * (TEXTURE_SIZE - 1f)), 0, TEXTURE_SIZE - 1);
                            var grad_pix = temp_color_array[lum_index];
                            //Debug.Log(grad_pix + " " + lum_index);
                            //if (x > 20) return null;
                            //  var grad_pix = temp_colors.GetPixel(Mathf.RoundToInt(lum * (TEXTURE_SIZE - 1f)), 0);

                            var result = Color.Lerp(pixel, grad_pix, grad_pix.a);
                            result.a = 1;
                            output.SetPixel(x + start_x, y, result);
                        }
                    }
                }


                // if (Application.isPlaying) Destroy(temp_colors);
                // else DestroyImmediate(temp_colors, false);


                output.Apply(true, true);
                output.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

                WriteCachedTexture(1, output, smooth);

                return output;
            }

            /*
            internal Texture2D GenerateTexture()
            {
                if (TryToGetCachedTextureForEditor(0)) return TryToGetCachedTextureForEditor(0);

                var temp_colors = new Texture2D(TEXTURE_SIZE, 1, TextureFormat.ARGB32, false);
                temp_colors.filterMode = FilterMode.Bilinear;
                temp_colors.wrapMode = TextureWrapMode.Clamp;

                int current_index = 0;
                for (int i = 0; i < TEXTURE_SIZE; i++)
                {
                    var global_lerp = i / (TEXTURE_SIZE - 1f);
                    var c = GetColor(global_lerp, ref current_index);
                    if (!c.HasValue) break;
                    temp_colors.SetPixel(i, 0, c.Value);
                    // output.SetPixel(i, 1, c.Value);
                    // output.SetPixel(i, 2, c.Value);
                }


                temp_colors.Apply(false, false);
                temp_colors.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                _cachedTexture[0] = temp_colors;
#if UNITY_EDITOR
                UnityEditor.SessionState.SetInt(PREFS_KEY + 0.ToString() + cameraInstance, temp_colors.GetInstanceID());
#endif

                return temp_colors;
            }

            internal Texture2D GenerateLUT()
            {

                if (TryToGetCachedTextureForEditor(1)) return TryToGetCachedTextureForEditor(1);


                var temp_colors = GenerateTexture();

                const int X = 1024;
                const int Y = 32;
                var output = new Texture2D(X, Y, TextureFormat.ARGB32, false);

                // for (int i = 0; i < 32; i++)
                // {
                //     var start_x = i * X;
                //     for (int y = 0; y < X; y++)
                //     {
                //         for (int x = 0; x < X; x++)
                //         {
                //             var r = x / 32f;
                //             var g = y / 32f;
                //             var b = i / 32f;
                //             output.SetPixel(x + start_x, y, new Color(r, g, b, 1));
                //         }
                //     }
                // }
                var LuminaceCoeff = new float[3] { 0.2125f, 0.7153f, 0.1721f };

                for (int i = 0; i < 32; i++)
                {
                    var start_x = i * X;
                    for (int y = 0; y < X; y++)
                    {
                        for (int x = 0; x < X; x++)
                        {
                            var r = x / 32f;
                            var g = y / 32f;
                            var b = i / 32f;

                            var pixel = new Color(r, g, b, 1);

                            // var pixel = output.GetPixel(x + start_x, y);
                            var lum = pixel.r * LuminaceCoeff[0] + pixel.g * LuminaceCoeff[1] + pixel.b * LuminaceCoeff[2];
                            var grad_pix = output.GetPixel(Mathf.RoundToInt(lum * (TEXTURE_SIZE - 1f)), 0);
                            //  var grad_pix = temp_colors.GetPixel(Mathf.RoundToInt(lum * (TEXTURE_SIZE - 1f)), 0);
                            var lerp = Color.Lerp(pixel, grad_pix, grad_pix.a);
                            lerp.a = 1;
                            output.SetPixel(x + start_x, y, lerp);
                        }
                    }
                }


                if (Application.isPlaying) Destroy(temp_colors);
                else DestroyImmediate(temp_colors, false);


                output.Apply(true, true);
                output.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                _cachedTexture[1] = output;
#if UNITY_EDITOR
                UnityEditor.SessionState.SetInt(PREFS_KEY + 1.ToString() + cameraInstance, output.GetInstanceID());
#endif

                return output;
            }*/

            internal void SetColors()
            {
                TryToDestroyTexture(0);
                TryToDestroyTexture(1);
            }

            Color? GetColor(float global_lerp, ref int current_index, float smooth)
            {
                if (colors == null || colors.Length < 1)
                {
                    colors = new Color[2];
                    colors[0] = Color.white;
                    colors[1] = Color.black;
                    positions = new float[2];
                    positions[1] = 1;
                }
                if (colors.Length == 1)
                {
                    return colors[0];
                }
                else
                {

                    if (global_lerp < positions[0])
                    {
                        return colors[0];
                    }
                    else if (global_lerp > positions[positions.Length - 1])
                    {
                        return colors[colors.Length - 1];
                    }
                    else
                    {
                        while (global_lerp > positions[current_index])
                        {
                            current_index++;
                        }
                        current_index--;
                        if (current_index < 0) current_index = 0;

                        if (current_index >= colors.Length - 1) return colors[colors.Length - 1];

                        var local_lerp = (global_lerp - positions[current_index]) / (positions[current_index + 1] - positions[current_index]);

                        //var sign = Mathf.Sign(local_lerp - 0.5f);
                        //local_lerp = Mathf.Pow(Mathf.Abs(local_lerp - 0.5f) * 2, smooth) * sign / 2 + 0.5f;
                        local_lerp = (local_lerp) * smooth;

                        return Color.Lerp(colors[current_index], colors[current_index + 1], local_lerp);
                    }
                }
            }
#if UNITY_EDITOR
            struct json_saver
            {
                public Color[] colors;
                public float[] positions;
            }
            public float DrawEditorGUI(Rect guiRect, UnityEngine.Object[] object_for_udo, float smooth)
            {
                //UnityEditor.EditorGUI.DrawRect(guiRect, Color.white);
                //guiRect.height *= 2;
                //return guiRect.height;

                var start_y = guiRect.y;

                // Debug.Log(guiRect);

                GUI.Label(guiRect, "Gradient Texture:");
                guiRect.y += guiRect.height;
                // UnityEditor.EditorGUI.DrawRect(guiRect, Color.black);
                // Debug.Log(guiRect);
                // return guiRect.y + guiRect.height - start_y;


                guiRect.height = UnityEditor.EditorGUIUtility.singleLineHeight * 2;
                guiRect.height -= marker_size;
                UnityEditor.EditorGUI.DrawRect(guiRect, Color.black);
                GUI.DrawTexture(guiRect, GenerateTexture(smooth));
                guiRect.y += guiRect.height;

                guiRect.height = UnityEditor.EditorGUIUtility.singleLineHeight * 0.5f;
                _draw_clicker_area(guiRect, object_for_udo, smooth);
                guiRect.y += guiRect.height;

                guiRect.height = UnityEditor.EditorGUIUtility.singleLineHeight;
                var bt = guiRect;
                bt.width /= 2;
                if (GUI.Button(bt, "Copy Gradient"))
                {
                    json_saver saver = new json_saver();
                    saver.colors = colors;
                    saver.positions = positions;
                    UnityEditor.EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(saver);
                }
                bt.x += bt.width;
                if (GUI.Button(bt, "Paste Gradient"))
                {
                    try
                    {
                        var result = JsonUtility.FromJson<json_saver>(UnityEditor.EditorGUIUtility.systemCopyBuffer);
                        foreach (var item in object_for_udo)
                            UnityEditor.Undo.RecordObject(item, "Change Gradient");
                        colors = result.colors;
                        positions = result.positions;
                        foreach (var item in object_for_udo)
                            UnityEditor.EditorUtility.SetDirty(item);
                        SetColors();
                    }
                    catch { }
                }
                guiRect.y += guiRect.height;


                guiRect.height = UnityEditor.EditorGUIUtility.singleLineHeight;
                if (GUI.Button(guiRect, "Repaint Texture")) SetColors();
                guiRect.y += guiRect.height;

                if (selected_index != -1)
                {
                    guiRect.height = UnityEditor.EditorGUIUtility.singleLineHeight;
                    var new_c = UnityEditor.EditorGUI.ColorField(guiRect, colors[selected_index]);
                    if (new_c != colors[selected_index])
                    {
                        foreach (var item in object_for_udo)
                            UnityEditor.Undo.RecordObject(item, "Change Gradient");
                        colors[selected_index] = new_c;
                        foreach (var item in object_for_udo)
                            UnityEditor.EditorUtility.SetDirty(item);
                        SetColors();
                    }
                }


                return guiRect.y + guiRect.height - start_y;

            }
            const float marker_size = 4;

            Rect GetRectForPoint(Rect global_rect, float lerp)
            {
                var point_x = GetWorldPosForPoint(global_rect, lerp);
                return new Rect(point_x - marker_size, global_rect.y - marker_size * 2, marker_size * 2, marker_size * 3);
            }
            float GetWorldPosForPoint(Rect global_rect, float lerp)
            {
                return global_rect.x + global_rect.width * lerp;
            }
            float GetMouseLocalLerp(Rect global_rect)
            {
                return (Event.current.mousePosition.x - global_rect.x) / global_rect.width;
            }

            Action _drag_action;
            int selected_index = -1;
            void _draw_clicker_area(Rect r, UnityEngine.Object[] object_for_udo, float smooth)
            {

                if (_drag_action != null)
                {
                    if (Event.current.type == EventType.MouseDown) _drag_action = null;
                    else _drag_action();
                }

                if (colors == null || colors.Length < 1)
                {
                    colors = new Color[2];
                    colors[0] = Color.white;
                    colors[1] = Color.black;
                    positions = new float[2];
                    positions[1] = 1;
                }

                for (int i = 0; i < colors.Length; i++)
                {
                    var local_rect = GetRectForPoint(r, positions[i]);
                    var draw_color = colors[i];
                    if (_drag_action != null && selected_index == i) draw_color.a /= 2;
                    UnityEditor.EditorGUI.DrawRect(local_rect, draw_color);
                    UnityEditor.EditorGUIUtility.AddCursorRect(local_rect, UnityEditor.MouseCursor.Link);
                }


                if (Event.current.type == EventType.MouseDown && _drag_action == null)
                {
                    var draw_rect = r;
                    var first_rect = GetRectForPoint(r, positions[0]);
                    draw_rect.height = first_rect.y + first_rect.height - draw_rect.y;
                    var start_mouse_position = Event.current.mousePosition.x;

                    //Debug.Log(UnityEditor.EditorWindow.focusedWindow.name);
                    if (Event.current.button == 0 && (string.IsNullOrEmpty(UnityEditor.EditorWindow.focusedWindow.name) ||
                        UnityEditor.EditorWindow.focusedWindow.name.ToLower().Contains("inspector")))
                    {
                        bool contains = false;
                        for (int i = positions.Length - 1; i >= 0; i--)
                        {
                            if (GetRectForPoint(r, positions[i]).Contains(Event.current.mousePosition))
                            {
                                // Debug.Log("ASD" + start_mouse_position);
                                selected_index = i;
                                _drag_action = () => {
                                    if (!Event.current.isMouse) return;
                                    if (Event.current.type == EventType.MouseUp)
                                    {
                                        _drag_action = null;
                                        return;
                                    }
                                    var dif = Event.current.mousePosition.x - start_mouse_position;
                                    start_mouse_position = Event.current.mousePosition.x;
                                    //Debug.Log(start_mouse_position);

                                    if (dif != 0)
                                    {
                                        dif /= r.width;
                                        foreach (var item in object_for_udo)
                                            UnityEditor.Undo.RecordObject(item, "Move Gradient");
                                        var target_pos = positions[selected_index] + dif;
                                        bool was = false;
                                        while (selected_index < positions.Length - 1 && target_pos > positions[selected_index + 1])
                                        {
                                            var t_f = positions[selected_index];
                                            positions[selected_index] = positions[selected_index + 1];
                                            positions[selected_index + 1] = t_f;
                                            var t_c = colors[selected_index];
                                            colors[selected_index] = colors[selected_index + 1];
                                            colors[selected_index + 1] = t_c;
                                            selected_index++;
                                            was = true;
                                        }
                                        if (!was) while (selected_index > 0 && target_pos < positions[selected_index - 1])
                                            {
                                                var t_f = positions[selected_index];
                                                positions[selected_index] = positions[selected_index - 1];
                                                positions[selected_index - 1] = t_f;
                                                var t_c = colors[selected_index];
                                                colors[selected_index] = colors[selected_index - 1];
                                                colors[selected_index - 1] = t_c;
                                                selected_index--;
                                            }
                                        positions[selected_index] = Mathf.Clamp01(target_pos);
                                        foreach (var item in object_for_udo)
                                            UnityEditor.EditorUtility.SetDirty(item);
                                        SetColors();
                                    }
                                };
                                contains = true;
                                break;
                            }
                        }
                        if (!contains && draw_rect.Contains(Event.current.mousePosition))
                        {
                            foreach (var item in object_for_udo)
                                UnityEditor.Undo.RecordObject(item, "Move Gradient");
                            int estim_index = 0;
                            var local_lerp = GetMouseLocalLerp(r);
                            var _e_i = estim_index;
                            var local_color = GetColor(local_lerp, ref _e_i, smooth) ?? Color.white;
                            for (int i = 0; i < positions.Length; i++)
                            {
                                var wr = GetWorldPosForPoint(r, positions[i]);
                                if (wr > Event.current.mousePosition.x) break;
                                estim_index++;
                            }
                            Array.Resize(ref positions, positions.Length + 1);
                            Array.Resize(ref colors, colors.Length + 1);
                            for (int i = estim_index; i < positions.Length - 1; i++)
                            {
                                positions[i + 1] = positions[i];
                                colors[i + 1] = colors[i];
                            }
                            positions[estim_index] = local_lerp;
                            colors[estim_index] = local_color;
                            foreach (var item in object_for_udo)
                                UnityEditor.EditorUtility.SetDirty(item);
                            SetColors();
                        }
                    }
                    if (draw_rect.Contains(Event.current.mousePosition) && Event.current.button > 0 && positions.Length > 1)
                    {
                        for (int i = positions.Length - 1; i >= 0; i--)
                        {
                            if (GetRectForPoint(r, positions[i]).Contains(Event.current.mousePosition))
                            {
                                foreach (var item in object_for_udo)
                                    UnityEditor.Undo.RecordObject(item, "Move Gradient");
                                for (int y = i; y < positions.Length - 1; y++)
                                {
                                    positions[y] = positions[y + 1];
                                    colors[y] = colors[y + 1];

                                }
                                Array.Resize(ref positions, positions.Length - 1);
                                Array.Resize(ref colors, colors.Length - 1);
                                selected_index = -1;
                                foreach (var item in object_for_udo)
                                    UnityEditor.EditorUtility.SetDirty(item);
                                SetColors();
                                break;
                            }
                        }
                    }

                }

            }
#endif
        }




    }



    internal static class _PID
    {
        internal static int _MainTex = Shader.PropertyToID("_MainTex");
        internal static int _DetailTexture = Shader.PropertyToID("_DetailTexture");
        internal static int _NoiseTexture = Shader.PropertyToID("_NoiseTexture");

        internal static int _MainTex_TexelSize = Shader.PropertyToID("_MainTex_TexelSize");
        internal static int _Texel_Size = Shader.PropertyToID("_Texel_Size");
        internal static int _Camera_Size = Shader.PropertyToID("_Camera_Size");
        internal static int _DepthCorrection = Shader.PropertyToID("_DepthCorrection");
        internal static int _Camera_Forward = Shader.PropertyToID("_Camera_Forward");
        internal static int _CameraDirLeft = Shader.PropertyToID("_CameraDirLeft");
        internal static int _CameraDirRight = Shader.PropertyToID("_CameraDirRight");
        internal static int _Bayer_Matrix = Shader.PropertyToID("bayer_matrix");
        internal static int _Rotate2World = Shader.PropertyToID("_Rotate2World");
        internal static int _Camera_Converter = Shader.PropertyToID("_Camera_Converter");
        internal static int _Fixed_Time = Shader.PropertyToID("_Fixed_Time");
        internal static int _Fog_Time = Shader.PropertyToID("_Fog_Time");




        internal static int _LUT1 = Shader.PropertyToID("_LUT1");
        internal static int _LUT1_GRAD = Shader.PropertyToID("_LUT1_GRAD");
        internal static int _LUT1_params = Shader.PropertyToID("_LUT1_params");
        internal static int _LUT1_amount = Shader.PropertyToID("_LUT1_amount");
        internal static int _LUT1_gradient_smooth = Shader.PropertyToID("_LUT1_gradient_smooth");
        internal static int _LUT2 = Shader.PropertyToID("_LUT2");
        internal static int _LUT2_params = Shader.PropertyToID("_LUT2_params");
        internal static int _LUT2_amount = Shader.PropertyToID("_LUT2_amount");

        internal static int _lutsLevelClipMaxValue = Shader.PropertyToID("_lutsLevelClipMaxValue");

        internal static int _FinalGradientTexture = Shader.PropertyToID("_FinalGradientTexture");
        internal static int _FinalGradientTest = Shader.PropertyToID("_FinalGradientTest");
        internal static int _DitherTexture = Shader.PropertyToID("_DitherTexture");
        internal static int _DitherSize = Shader.PropertyToID("_DitherTextureSize");
        internal static int _FinalGradientTexture_params = Shader.PropertyToID("_FinalGradientTexture_params");
        internal static int _GradiendDitherSize = Shader.PropertyToID("_GradiendDitherSize");
        internal static int _1_GradiendBrightness = Shader.PropertyToID("_1_GradiendBrightness");
        internal static int _1_GradiendOffset = Shader.PropertyToID("_1_GradiendOffset");
        internal static int _M_GradiendBrightness = Shader.PropertyToID("_M_GradiendBrightness");
        internal static int _M_GradiendOffset = Shader.PropertyToID("_M_GradiendOffset");
        internal static int _BayerMultyply = Shader.PropertyToID("_BayerMultyply");




        internal static int _b_brightAmount = Shader.PropertyToID("_b_brightAmount");
        internal static int _b_colorsAmount = Shader.PropertyToID("_b_colorsAmount");
        internal static int _b_brightAmountVector = Shader.PropertyToID("_b_brightAmountVector");
        internal static int _b_saturate01 = Shader.PropertyToID("_b_saturate01");
        internal static int _b_saturate01Value = Shader.PropertyToID("_b_saturate01Value");
        internal static int _b_contrastAmount = Shader.PropertyToID("_b_contrastAmount");

        internal static int _posterizationStepsAmount = Shader.PropertyToID("_posterizationStepsAmount");
        internal static int _posterizationDither = Shader.PropertyToID("_posterizationDither");
        internal static int _posterizationOffsetZeroPoint = Shader.PropertyToID("_posterizationOffsetZeroPoint");

        internal static int _glowContrast = Shader.PropertyToID("_glowContrast");
        internal static int _glowBrightness = Shader.PropertyToID("_glowBrightness");
        internal static int _glowTreshold = Shader.PropertyToID("_glowTreshold");


        internal static int _patTintAffect = Shader.PropertyToID("_patTintAffect");
        internal static int _patUvAffect = Shader.PropertyToID("_patUvAffect");
        internal static int _patSize = Shader.PropertyToID("_patSize");
        internal static int _patPixelization = Shader.PropertyToID("_patPixelization");
        internal static int _PatternTexture = Shader.PropertyToID("_PatternTexture");

        internal static int _MotionBlurMoveAmount = Shader.PropertyToID("_MotionBlurMoveAmount");
        internal static int _MotionBlurRotateAmount = Shader.PropertyToID("_MotionBlurRotateAmount");
        internal static int _MotionBlurPhaseShift = Shader.PropertyToID("_MotionBlurPhaseShift");

        internal static int _outlineStrokesBlend = Shader.PropertyToID("_outlineStrokesBlend");
        internal static int _outlineStrokesDetection = Shader.PropertyToID("_outlineStrokesDetection");
        internal static int _outlineStrokesScale = Shader.PropertyToID("_outlineStrokesScale");
        internal static int _outlineStrokesColor = Shader.PropertyToID("_outlineStrokesColor");

        internal static int _aoRadius = Shader.PropertyToID("_aoRadius");
        internal static int _aoAmount = Shader.PropertyToID("_aoAmount");
        internal static int _aoBlend = Shader.PropertyToID("_aoBlend");
        internal static int _aoThreshold = Shader.PropertyToID("_aoThreshold");
        internal static int _aoGroundBias = Shader.PropertyToID("_aoGroundBias");
        internal static int _aoStrokesSpread = Shader.PropertyToID("_aoStrokesSpread");
        internal static int _aoDistanceFixAmount = Shader.PropertyToID("_aoDistanceFixAmount");
        internal static int _aoNormalsBias = Shader.PropertyToID("_aoNormalsBias");

        internal static int _USE_HDR_FOR_FOG = Shader.PropertyToID("_USE_HDR_FOR_FOG");
        internal static int _SKIP_FOG_FOR_BACKGROUND = Shader.PropertyToID("_SKIP_FOG_FOR_BACKGROUND");
        internal static int _StartZ = Shader.PropertyToID("_StartZ");
        internal static int _EndZ = Shader.PropertyToID("_EndZ");
        internal static int _FogColor = Shader.PropertyToID("_FogColor");
        internal static int _StartY = Shader.PropertyToID("_StartY");
        internal static int _EndY = Shader.PropertyToID("_EndY");
        internal static int _ZFogDensity = Shader.PropertyToID("_ZFogDensity");
        internal static int _HeightFogDensity = Shader.PropertyToID("_HeightFogDensity");
        internal static int _FogParams = Shader.PropertyToID("_FogParams");
        internal static int _FogTexAnimateSpeed = Shader.PropertyToID("_FogTexAnimateSpeed");
        internal static int _FogTexTile = Shader.PropertyToID("_FogTexTile");

        internal static int _darkBorders = Shader.PropertyToID("_darkBorders");
        internal static int _sharpenAmounta = Shader.PropertyToID("_sharpenAmounta");
        internal static int _sharpenAmountb = Shader.PropertyToID("_sharpenAmountb");
        internal static int _sharpenSizea = Shader.PropertyToID("_sharpenSizea");
        internal static int _sharpenSizeb = Shader.PropertyToID("_sharpenSizeb");
        internal static int _sharpenDarkPoint_Fast = Shader.PropertyToID("_sharpenDarkPoint_Fast");
        internal static int _sharpenDarkPoint = Shader.PropertyToID("_sharpenDarkPoint");
        internal static int _sharpenBottomBright_Fast = Shader.PropertyToID("_sharpenBottomBright_Fast");
        internal static int _sharpenBottomBright_Improved = Shader.PropertyToID("_sharpenBottomBright_Improved");
        internal static int _sharpenLerp_Improved = Shader.PropertyToID("_sharpenLerp_Improved");

        // internal static int _blurRadius = Shader.PropertyToID("_blurRadius");
        internal static int _spread_AnimateSpeedMulti = Shader.PropertyToID("_spread_AnimateSpeedMulti");
        internal static int _spread_Variatnt = Shader.PropertyToID("_spread_Variatnt");

        internal static int _glowSamples_BlurRadius = Shader.PropertyToID("_glowSamples_BlurRadius");
        internal static int _glowSamples_DitherSize = Shader.PropertyToID("_glowSamples_DitherSize");
        internal static int _blurSamples_BlurRadius = Shader.PropertyToID("_blurSamples_BlurRadius");
        internal static int _blurSamples_DitherSize = Shader.PropertyToID("_blurSamples_DitherSize");

        internal static int _dof_zoff_borders = Shader.PropertyToID("_dof_zoff_borders");
        internal static int _dof_zoff_feather = Shader.PropertyToID("_dof_zoff_feather");
        internal static int _dof_z_distance = Shader.PropertyToID("_dof_z_distance");
        internal static int _dof_z_aperture = Shader.PropertyToID("_dof_z_aperture");

        internal static int _CameraVelocity = Shader.PropertyToID("_CameraVelocity");
        internal static int _CameraDeepVel = Shader.PropertyToID("_CameraDeepVel");

    }

    internal static class _KW
    {
        internal const string USE_LUT0 = "-";
        internal const string USE_LUT1 = "USE_LUT1";
        internal const string USE_LUT2 = "USE_LUT2";
        internal const string SKIP_LUTS_FOR_BRIGHT_AREAS = "SKIP_LUTS_FOR_BRIGHT_AREAS";
        internal const string USE_HDR_COLORS = "USE_HDR_COLORS";
        internal const string LUTS_AMONT_MORE_THAN_1 = "LUTS_AMONT_MORE_THAN_1";

        internal const string USE_BRIGHTNESS_FUNCTIONS = "USE_BRIGHTNESS_FUNCTIONS";
        internal const string USE_POSTERIZE_LUTS = "USE_POSTERIZE_LUTS", USE_POSTERIZE_IMPROVED = "USE_POSTERIZE_IMPROVED", USE_POSTERIZE_SIMPLE = "USE_POSTERIZE_SIMPLE";
        internal const string USE_POSTERIZE_DITHER = "USE_POSTERIZE_DITHER";

        internal const string USE_ULTRA_FAST_SHARPEN = "USE_ULTRA_FAST_SHARPEN", USE_IMPROVED_SHARPEN = "USE_IMPROVED_SHARPEN";


        //internal const string USE_GLOW_1 = "USE_GLOW_1", USE_GLOW_4 = "USE_GLOW_4", USE_GLOW_8 = "USE_GLOW_8", USE_GLOW_16 = "USE_GLOW_16";
        //internal const string USE_GLOW_DITHER = "USE_GLOW_DITHER";

        // internal const string USE_VINJETE_BLUR = "USE_VINJETE_BLUR", USE_DEPTH_OF_FIELD_SIMPLE = "USE_DEPTH_OF_FIELD_SIMPLE",
        //   USE_DEPTH_OF_FIELD_COMPLETE_4 = "USE_DEPTH_OF_FIELD_COMPLETE_4", USE_DEPTH_OF_FIELD_COMPLETE_8 = "USE_DEPTH_OF_FIELD_COMPLETE_8", USE_DEPTH_OF_FIELD_COMPLETE_16 = "USE_DEPTH_OF_FIELD_COMPLETE_16";
        // internal const string USE_Z_FOR_BLURING = "USE_Z_FOR_BLURING";
        internal const string USE_FINAL_GRADIENT_DITHER_ONE_BIT = "USE_FINAL_GRADIENT_DITHER_ONE_BIT";
        internal const string USE_FINAL_GRADIENT_DITHER_MATH = "USE_FINAL_GRADIENT_DITHER_MATH";
        internal const string USE_FINAL_GRADIENT_DITHER_TEX = "USE_FINAL_GRADIENT_DITHER_TEX";
        internal const string USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX = "USE_FINAL_GRADIENT_DITHER_MATH_AND_TEX";

        //internal const string GRADIENT_RAW = "-";
        internal const string GRADIENT_RAMP = "GRADIENT_RAMP";
        internal const string GRADIENT_NONE = "-";
        internal const string GRADIENT_COLOR_ADD = "GRADIENT_COLOR_ADD";
        internal const string GRADIENT_COLOR_MULTY = "GRADIENT_COLOR_MULTY";


        internal const string USE_OUTLINE_STROKES = "USE_OUTLINE_STROKES", USE_OUTLINE_COLORED_STROKES_NORMAL = "USE_OUTLINE_COLORED_STROKES_NORMAL", USE_OUTLINE_COLORED_STROKES_ADD = "USE_OUTLINE_COLORED_STROKES_ADD", USE_OUTLINE_STROKES_INVERTED = "USE_OUTLINE_STROKES_INVERTED";
        internal const string APPLY_STROKES_AFTER_FOG = "APPLY_STROKES_AFTER_FOG";
        internal const string APPLY_DITHER_TO_STROKE = "APPLY_DITHER_TO_STROKE";
        internal const string APPLY_DEPTH_CORRECTION_TO_STROKE = "APPLY_DEPTH_CORRECTION_TO_STROKE";
        internal const string APPLY_CUSTOM_GRADIENT_FOR_LUT0 = "APPLY_CUSTOM_GRADIENT_FOR_LUT0";

        internal const string APPLY_SSAO_AFTER_EFFECTS = "APPLY_SSAO_AFTER_EFFECTS";
        internal const string USE_SIMPLE_SSAO_3 = "USE_SIMPLE_SSAO_3", USE_SIMPLE_SSAO_4 = "USE_SIMPLE_SSAO_4"
            , USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION = "USE_SIMPLE_SSAO_3_DISTANCE_CORRECTION", USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION = "USE_SIMPLE_SSAO_4_DISTANCE_CORRECTION";

        internal const string USE_SHADER_DISTANCE_FOG = "USE_SHADER_DISTANCE_FOG", USE_SHADER_DISTANCE_FOG_WITH_TEX = "USE_SHADER_DISTANCE_FOG_WITH_TEX", USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION = "USE_SHADER_DISTANCE_FOG_WITH_TEX_N_ANIMATION";
        internal const string USE_GLOBAL_HEIGHT_FOG = "USE_GLOBAL_HEIGHT_FOG", USE_GLOBAL_HEIGHT_FOG_WITH_TEX = "USE_GLOBAL_HEIGHT_FOG_WITH_TEX", USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION = "USE_GLOBAL_HEIGHT_FOG_WITH_TEX_N_ANIMATION";
        internal const string USE_FOG_ADDITIVE_BLENDING = "USE_FOG_ADDITIVE_BLENDING";
        internal const string FOG_BEFORE_LUT = "FOG_BEFORE_LUT";
        //internal const string USE_POSITION_FOR_HEIGHT_FOG = "FOG_BEFORE_LUT";

        internal const string USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z = "USE_DOF_USE_GLOW_SAMPLES_BLUR_BORDERS_OFF_Z";
        internal const string USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z = "USE_DOF_USE_GLOW_SAMPLES_BLUR_DEPEND_ON_Z";
        internal const string USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z = "USE_DOF_BLUR_SAMPLES_SIZE_ON_Z_BLUR_DEPEND_ON_Z";
        internal const string BLUR_SAMPLES_1 = "-";
        internal const string BLUR_SAMPLES_4 = "BLUR_SAMPLES_4";
        internal const string BLUR_SAMPLES_8 = "BLUR_SAMPLES_8";
        internal const string BLUR_SAMPLES_16 = "BLUR_SAMPLES_16";

        internal const string USE_GLOW_MOD = "USE_GLOW_MOD";
        internal const string USE_GLOW_BLEACH_STYLE = "USE_GLOW_BLEACH_STYLE";
        internal const string GLOW_SAMPLES_NO_DISTANCE = "-";
        internal const string GLOW_SAMPLES_SIZE_ON_Z = "GLOW_SAMPLES_SIZE_ON_Z";
        internal const string GLOW_SAMPLES_1 = "-";
        internal const string GLOW_SAMPLES_4 = "GLOW_SAMPLES_4";
        internal const string GLOW_SAMPLES_8 = "GLOW_SAMPLES_8";
        internal const string GLOW_SAMPLES_16 = "GLOW_SAMPLES_16";
        internal const string GLOW_DIRECTION_XY = "-";
        internal const string GLOW_DIRECTION_BI = "GLOW_DIRECTION_BI";
        internal const string GLOW_DIRECTION_X = "GLOW_DIRECTION_X";
        internal const string GLOW_DIRECTION_Y = "GLOW_DIRECTION_Y";

        internal const string USE_WRAPS = "USE_WRAPS";
        internal const string USE_F_MOTION_BLUR_FAST = "F_MOTION_BLUR_FAST";

        // internal const string USE_BLUR_SAMPLE_DITHER = "USE_BLUR_SAMPLE_DITHER";
        // internal const string USE_GLOW_SAMPLE_DITHER = "USE_GLOW_SAMPLE_DITHER";


        //internal const string USE_BLUR = "USE_BLUR", USE_DEPTH_OF_FIELD_4 = "USE_DEPTH_OF_FIELD_4", USE_DEPTH_OF_FIELD_8 = "USE_DEPTH_OF_FIELD_8", USE_DEPTH_OF_FIELD_16 = "USE_DEPTH_OF_FIELD_16";
    }
}