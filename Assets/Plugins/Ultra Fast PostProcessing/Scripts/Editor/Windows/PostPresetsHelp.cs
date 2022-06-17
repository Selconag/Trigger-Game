using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using EM.PostProcessing.Runtime;

namespace EM.PostProcessing.Editor
{
    class WelcomeScreen : EditorWindow
    {
        public static void Init(Rect source)
        {   // var w = GetWindow<WelcomeScreen>( "Post Presets - Welcome Screen" , true, Params.WindowType,)
            var w = GetWindow<WelcomeScreen>(true, FastPostProcessingProfile.ASSET_NAME + " - Welcome Screen", true);
            var thisR = new Rect(0, 0, 530, 700);
            thisR.x = source.x + source.width / 2 - thisR.width / 2;
            thisR.y = source.y + source.height / 2 - thisR.height / 2;
            w.position = thisR;
        }


        void drawTexture(Texture2D texture)
        {
            var dif = Mathf.Clamp(position.width / texture.width, 0.01f, 1);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(texture.height * dif));
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
        Vector2 scrollPos;
        Dictionary<string, Texture2D> t = null;
        private void OnGUI()
        {
            // for (int i = 0; i < t.Length; i++)
            // {
            //     t[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(Params.EditorResourcesPath + "/Documentations/PostPresets - Welcome Screen 0" + (i + 1) + ".png");
            //     if (!t[i]) return;
            // }
            if (t == null)
            {
                t = new Dictionary<string, Texture2D>();

                foreach (var f in Directory.GetFiles(Icons.EditorFolder + "/Help", "*.png"))
                {
                    var item = AssetDatabase.LoadAssetAtPath<Texture2D>(f);
                    var key = Path.GetFileName(f);
                    t.Add(key.Remove(key.LastIndexOf('.')), item);

                }

            }
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("You can find asset opetion in the top menu 'Tools/Ultra Fast PostProcessing':");
            drawTexture(t["UFRP_HELP_IMAGE_LOCATE_0"]);
            GUILayout.Label("By default assest's files located in the 'Plugins/Ultra Fast PostProcessing' directory:");
            drawTexture(t["UFRP_HELP_IMAGE_LOCATE_1"]);

            GUILayout.Space(20);
            GUILayout.Label("How to use?");
            GUILayout.Space(20);
            GUILayout.Label("If you are using default render pipeline then just add the component on your camera");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_DEFAULT_0"]);
            GUILayout.Space(20);
            GUILayout.Label("If you are using URP render pipeline you can follow to few additiuonal steps");
            GUILayout.Label("Or just dd omponent on your camera, if you are already setup the URP");


            GUILayout.Label("Of course first of all your should add URP render pipeline in your packages manager.");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_URP_0"]);
            GUILayout.Label("If you didn't assign your pipeline's serttings, you can apply our build-in one.");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_URP_1"]);
            GUILayout.Label("You can check render settings in the project settings, are there any added pipeline settings.");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_URP_2"]);
            GUILayout.Label("If you have enabled URP render, UFP will autamtically offer to switch defines for URP render");
            GUILayout.Label("Just lick 'Yes'");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_URP_3"]);
            GUILayout.Label("And after then just add compnent on your camera.");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_DEFAULT_0"]);

            GUILayout.Space(20);
            GUILayout.Label("Note that if you will completely remove URP package, you also should remove UFP define from");
            GUILayout.Label("player settings. (but if you just temperary switch render pipeline you don'n need to do any");
            GUILayout.Label("additional steps)");
            drawTexture(t["UFRP_HELP_IMAGE_ADD_URP_5"]);
            GUILayout.Space(20);
            GUILayout.Label("(You can find information about how effects work, on official assetstore page).");


            //for (int i = 0; i < t.Length; i++) drawTexture(t[i]);
            GUILayout.EndScrollView();
            if (GUILayout.Button("Ok", GUILayout.Height(50)))
            {
                Close();
            }
        }

        // public bool Shown { get { return true; } }
    }
}
