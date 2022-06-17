//#define P128

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


namespace EM.PostProcessing.Editor
{
public partial class PostPresets1000Window : EditorWindow {
    void ExportLut()
    {
    
    }
    
    void RewriteHue()
    {   var gradients = GetGradientsPathsList( Params.USER_FOLDER );
        CreateCompareFile( ".LUTs User.data", gradients );
        ResetWindowAndCLearCache();
    }
    
    Texture2D ConvertToGamma(Texture2D source)
    {   if (PlayerSettings.colorSpace != ColorSpace.Linear) return source;
    
        Texture2D lut = new Texture2D(source.width, source.height, source.format, false, Params.OLD_VERSION);
        lut.LoadRawTextureData(source.GetRawTextureData());
        lut.wrapMode = TextureWrapMode.Clamp;
        if (Application.isPlaying) Destroy(source);
        else DestroyImmediate(source);
        var pxls = lut.GetPixels();
        for (int i = 0; i < pxls.Length; i++)
            for (int x = 0; x < 3; x++)
            {
            
                /* pxls[i][x] =   0.585122381f * Mathf.Sqrt(pxls[i][x]) +
                                0.783140355f * Mathf.Sqrt(Mathf.Sqrt(pxls[i][x])) -
                                0.368262736f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(pxls[i][x])));*/
                
                if (pxls[i][x] <= 0.0031308)
                    pxls[i][x] = pxls[i][x] * 12.92f;
                else
                    pxls[i][x] = 1.055f * Mathf.Pow(pxls[i][x], 1.0f / 2.4f) - 0.055f;
            }
        lut.SetPixels(pxls);
        return lut;
    }
    Color ConvertToGamma(Color pxls)
    {   for (int x = 0; x < 3; x++)
        {   if (pxls[x] <= 0.0031308)
                pxls[x] = pxls[x] * 12.92f;
            else
                pxls[x] = 1.055f * Mathf.Pow(pxls[x], 1.0f / 2.4f) - 0.055f;
        }
        return pxls;
    }
    
    Color ConvertToLinear(Color source)
    {   /* source.r = Mathf.Pow( source.r + 0.055f, 2.4f);
         source.g = Mathf.Pow( source.g + 0.055f, 2.4f);
         source.b = Mathf.Pow( source.b + 0.055f, 2.4f);*/
        
        var lum = (source[0] + source[1] + source[2]) / 3;
        var newlum = Mathf.Pow( (lum ) * 1.1f, 2.2f) * 2.2f;
        var dif = lum == 0 ? 0 : newlum / lum;
        for (int i = 0; i < 3; i++)
        {   var v1 = Mathf.Pow( (source[i] - 0.1f) * 1.1f, 2.2f) * 2.2f;
            var v2 = source[i] * dif;
            source[i] = Mathf.Lerp(v1, v2, 0.5f);
            // source[i] = Mathf.Clamp(source[i], 0, 0.6f) / 0.6f ;
        }
        return source;
        /* for (int i = 0; i < 3; i++)
         {   if (source[i] <= 0.0031308 * 12.92f)
                 source[i] = source[i] /  12.92f;
             else
                 source[i] = Mathf.Pow( (source[i] + 0.055f) / 1.055f, 2.4f);
             source[i] *= 1.8f;
         }
         return source;*/
    }
    
    void Export2DLut()
    {   var path = "";
        var gameCamera = Params.camera;
        if ( !gameCamera )
        {   EditorUtility.DisplayDialog( "Camera not Found", "Please Enable Camera", "Ok" );
            return;
        }
        if (!string.IsNullOrEmpty( path = EditorUtility.SaveFilePanel("2DLUT.png Export", Params.EditorResourcesPath + "/" + Params.USER_FOLDER, "My Full LUTs Table", "png").Replace('\\', '/')))
        {   var renderCamera = Instantiate( AssetDatabase.LoadAssetAtPath<GameObject>(Params.EditorResourcesPath + "/Presets Window/LUTs Camera Export/LUTs Camera Export Prefab.prefab" ),
                                            Vector3.down * -10000, Quaternion.identity).GetComponent<Camera>();
            // var gameCamera = GetCurrentCamera(firstLoop);
            renderCamera.transform.position = gameCamera .transform.position - gameCamera .transform.forward;
            foreach (var c in gameCamera.GetComponents<Component>())
            {   if (!c || c is Transform || c is Camera) continue;
                if (UnityEditorInternal. ComponentUtility.CopyComponent(c))
                    UnityEditorInternal. ComponentUtility.PasteComponentAsNew( renderCamera.gameObject );
            }
            var lut = TakeScreen(renderCamera, 1024, 32, false);
            lut.wrapMode = TextureWrapMode.Clamp;
            lut = ConvertToGamma(lut);
            if (Application.isPlaying) Destroy(renderCamera.gameObject);
            else DestroyImmediate(renderCamera.gameObject);
            File.WriteAllBytes(path, lut.EncodeToPNG());
            if (Application.isPlaying) Destroy(lut);
            else DestroyImmediate(lut);
            if (path.StartsWith(Application.dataPath.Replace('\\', '/')))
            {   path = "Assets" + path.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var exportedLut = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                FixImportSetting(exportedLut);
                Selection.objects = new[] {exportedLut};
            }
            
        }
    }
    void Export3DLutFull()
    {   var path = "";
        var gameCamera = Params.camera;
        if ( !gameCamera )
        {   EditorUtility.DisplayDialog( "Camera not Found", "Please Enable Camera", "Ok" );
            return;
        }
        if (!string.IsNullOrEmpty( path = EditorUtility.SaveFilePanel("3DLUT.CUBE Export", Params.EditorResourcesPath + "/" + Params.USER_FOLDER, "My Full CUBE Table", "CUBE").Replace('\\', '/')))
        {   var renderCamera = Instantiate( AssetDatabase.LoadAssetAtPath<GameObject>(Params.EditorResourcesPath + "/Presets Window/LUTs Camera Export/LUTs Camera Export Prefab.prefab" ),
                                            Vector3.down * -10000, Quaternion.identity).GetComponent<Camera>();
            // var gameCamera = GetCurrentCamera(firstLoop);
            renderCamera.transform.position = gameCamera .transform.position - gameCamera .transform.forward;
            foreach (var c in gameCamera.GetComponents<Component>())
            {   if (!c || c is Transform || c is Camera) continue;
                if (UnityEditorInternal. ComponentUtility.CopyComponent(c))
                    UnityEditorInternal. ComponentUtility.PasteComponentAsNew( renderCamera.gameObject );
            }
            var lut = TakeScreen(renderCamera, 1024, 32, false);
            lut.wrapMode = TextureWrapMode.Clamp;
            lut = ConvertToGamma(lut);
            if (Application.isPlaying) Destroy(renderCamera.gameObject);
            else DestroyImmediate(renderCamera.gameObject);
            File.WriteAllText(path, CreateCUBE(lut, true));
            var p2 = path.Remove(path.Length - 5) + "-G2.2.CUBE";
            File.WriteAllText(p2, CreateCUBE(lut, false));
            if (Application.isPlaying) Destroy(lut);
            else DestroyImmediate(lut);
            AssetDatabase.Refresh();
            if (path.StartsWith(Application.dataPath.Replace('\\', '/')))
            {   Selection.objects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + path.Substring(Application.dataPath.Length)),
                                           AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + p2.Substring(Application.dataPath.Length))
                                          };
            }
        }
    }
    void Export3DLutOnly()
    {   if (!lastLutTexture)
        {   EditorUtility.DisplayDialog("Texture not Found", "Please Enable Low Range Lut Texture For Your Colors Cameras Script", "Ok");
            return;
        }
        var path = "";
        if (!string.IsNullOrEmpty( path = EditorUtility.SaveFilePanel("Convert LUT.png to 3DLUT.CUBE", Params.EditorResourcesPath + "/" + Params.USER_FOLDER, "My CUBE Table", "CUBE").Replace('\\', '/')))
        {
        
            Texture2D lut = new Texture2D(lastLutTexture.width, lastLutTexture.height, lastLutTexture.format, false, Params.OLD_VERSION);
            lut.LoadRawTextureData(lastLutTexture.GetRawTextureData());
            lut.wrapMode = TextureWrapMode.Clamp;
            File.WriteAllText(path, CreateCUBE(lut, true));
            var p2 = path.Remove(path.Length - 5) + "-G2.2.CUBE";
            File.WriteAllText(p2, CreateCUBE(lut, false));
            
            
            /* var t = new Texture2D(lastLutTexture.height, lastLutTexture.width, TextureFormat.RGBA32, false, Params.OLD_VERSION);
             t.wrapMode = TextureWrapMode.Clamp;
             var ARR = DebugCreateCUBE(lut);
             t.SetPixels(ARR[0]);
             File.WriteAllBytes(path.Replace(".CUBE", ".png"), t.EncodeToPNG());
             if (Application.isPlaying) Destroy(t);
             else DestroyImmediate(t);*/
            
            
            
            AssetDatabase.Refresh();
            if (Application.isPlaying) Destroy(lut);
            else DestroyImmediate(lut);
            
            if (path.StartsWith(Application.dataPath.Replace('\\', '/')))
            {   Selection.objects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + path.Substring(Application.dataPath.Length)),
                                           AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + p2.Substring(Application.dataPath.Length))
                                          };
            }
            /*if (path.StartsWith(Application.dataPath.Replace('\\', '/')))
            {   path = "Assets" + path.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }*/
        }
    }
    
    
    void ConvertCUBEtoCUBE()
    {
    
        if (!Selection.activeObject || !AssetDatabase.GetAssetPath(Selection.activeObject).ToLower().EndsWith(".cube"))
        {   EditorUtility.DisplayDialog("3D LUT CUBE not Selected", "Please Select 3D LUT CUBE file in your project window", "Ok");
            return;
        }
        
        var path = "";
        if (!string.IsNullOrEmpty( path = EditorUtility.SaveFilePanel("Convert 3DLUT.CUBE to 3DLUT_G2.2.CUBE", Params.EditorResourcesPath + "/" + Params.USER_FOLDER,
                                          "Converted " + Selection.activeObject.name,
                                          "CUBE").Replace('\\', '/')))
        {   int size = -1;
            Color[] res = null;
            try
            {   float prs;
                var split = File.ReadAllText(AssetDatabase.GetAssetPath(Selection.activeObject)).Split('\n');
                res = split.Select(v =>
                {   var s = v.Split(' ');
                    if (!float.TryParse(s[0], out prs)) return (Color? )null ;
                    var asd = new Color();
                    asd.r = prs;
                    if (!float.TryParse(s[1], out prs)) return null ;
                    asd.g = prs;
                    if (!float.TryParse(s[2], out prs)) return null ;
                    asd.b = prs;
                    asd.a = 1;
                    return asd;
                }).Where(v => v != null).Select(v => v.Value).ToArray();
                size = int.Parse( split.FirstOrDefault(s => s.Contains("LUT_3D_SIZE")).Split(' ')[1]);
            }
            catch (Exception ex)
            {   EditorUtility.DisplayDialog("Cannot read file", ex.Message + "\n\n" + ex.StackTrace, "Ok");
            
            }
            
            if (res != null)
            {   File.WriteAllText(path, CreateCUBE(res, size, true));
                var p2 = path.Remove(path.Length - 5) + "-G2.2.CUBE";
                File.WriteAllText(p2, CreateCUBE(res, size, false));
                AssetDatabase.Refresh();
                if (path.StartsWith(Application.dataPath.Replace('\\', '/')))
                {   Selection.objects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + path.Substring(Application.dataPath.Length)),
                                               AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets" + p2.Substring(Application.dataPath.Length))
                                              };
                }
            }
            
            
            
            
            
            
            
            
            
            
        }
        
        
        
    }
    
    
    /*Color[][] DebugCreateCUBE(Texture2D source)
    {   var pxl = source.GetPixels();
        var res2 = new Color[2][];
        res2[0] = new Color[pxl.Length];
        res2[1] = new Color[pxl.Length];
        var col = source.width / source.height;
        var size = source.height;
    
        for (int _z = 0; _z < size; _z++)
        {   var _zI = _z / (size - 1f);
            var zindex = Mathf.FloorToInt( _zI * (col - 1f));
            if (zindex == col - 1f) zindex --;
            var zlerp = _zI * (col - 1f) - zindex;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {   var c0 = pxl[x + zindex * size + y * source.width];
                    var c1 = pxl[x + (zindex + 1) * size + y * source.width];
                    var res = Color.Lerp(c0, c1, zlerp);
    
                    res2[0][x + y * size + zindex * size * size] = res;
                    res.r = Mathf.Pow(2.2f, res.r);
                    res.g = Mathf.Pow(2.2f, res.g);
                    res.b = Mathf.Pow(2.2f, res.b);
                    res2[1][x + y * size + zindex * size * size] = res;
    
                }
        }
        return res2;
    }*/
    
    string CreateCUBE(Texture2D source, bool linear)
    {   var pxl = source.GetPixels();
        var col = source.width / source.height;
        var size = source.height;
        System.Text.StringBuilder result = new StringBuilder();
        result.AppendLine("# Created by 1000+ Fast Post Processing Unity asset");
        result.AppendLine("");
        result.AppendLine("LUT_3D_SIZE " + source.height);
        /* result.AppendLine("DOMAIN_MIN 0.0 0.0 0.0");
         result.AppendLine("DOMAIN_MAX 1.0 1.0 1.0");*/
        result.AppendLine("");
        for (int _z = 0; _z < size; _z++)
        {   var _zI = _z / (size - 1f);
            var zindex = Mathf.FloorToInt( _zI * (col - 1f));
            if (zindex == col - 1f) zindex --;
            var zlerp = _zI * (col - 1f) - zindex;
            
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {   var c0 = pxl[x + zindex * size + y * source.width];
                    var c1 = pxl[x + (zindex + 1) * size + y * source.width];
                    var res = Color.Lerp(c0, c1, zlerp);
                    if (linear)
                    {   /* res.r = Mathf.Pow(2.2f, res.r);
                         res.g = Mathf.Pow(2.2f, res.g);
                         res.b = Mathf.Pow(2.2f, res.b);*/
                        res = ConvertToLinear(res);
                    }
                    result.Append(res.r.ToString("0.#####"));
                    result.Append(' ');
                    result.Append(res.g.ToString("0.#####"));
                    result.Append(' ');
                    result.AppendLine(res.b.ToString("0.#####"));
                }
        }
        return result.ToString();
    }
    
    string CreateCUBE(Color[] source, int size, bool linear)
    {   System.Text.StringBuilder result = new StringBuilder();
        result.AppendLine("# Created by 1000+ Fast Post Processing Unity asset");
        result.AppendLine("");
        result.AppendLine("LUT_3D_SIZE " + size);
        /* result.AppendLine("DOMAIN_MIN 0.0 0.0 0.0");
         result.AppendLine("DOMAIN_MAX 1.0 1.0 1.0");*/
        result.AppendLine("");
        
        for (int x = 0; x < source.Length; x++)
        {   var res = source[x];
            if (linear)
            {   res = ConvertToLinear(res);
            }
            else
            {   res = ConvertToGamma(res);
            }
            result.Append(res.r.ToString("0.#####"));
            result.Append(' ');
            result.Append(res.g.ToString("0.#####"));
            result.Append(' ');
            result.AppendLine(res.b.ToString("0.#####"));
        }
        return result.ToString();
    }
    
    
    
    
    void InitializeFavorites()
    {   favorites.Clear();
        var path = Params.EditorResourcesPath + "/.Favorites.data";
        if (File.Exists( path ))
        {   using (StreamReader sr = new StreamReader( path ))
            {   while (!sr.EndOfStream)
                {   var f = sr.ReadLine();
                    if (!favorites.ContainsKey( f )) favorites.Add( f, 0 );
                }
            }
        }
    }
    
    
    
    static Texture2D texCopy;
    
    static public void CreateCompareFile(string fileName, Texture2D[] gradients)
    {   if (gradients.Length == 0) return;
    
        var first = GetReadableTexture(gradients[0]);
        if (!first) return;
        StringBuilder result = new StringBuilder();
        EditorUtility.DisplayProgressBar( "CreateCompareFile", "Converted:", 0 );
        int i = 0;
        
        foreach (var _gr in gradients)
        {   var gr = GetReadableTexture(_gr);
        
            if (!gr) return;
            
            result.Append( _gr.name );
            result.Append( ":" );
            result.Append( CALC_BRIGHT( gr ) );
            result.Append( ":" );
            result.Append( CALC_HUE( gr ) );
            result.Append( ":" );
            result.Append( CALC_FIRSTDIFF( gr, first ) );
            result.Append( "\n" );
            
            EditorUtility.DisplayProgressBar( "CreateCompareFile", "Converted: " + (i + 1) + " / " + gradients.Length, ++i / (float)gradients.Length );
        }
        EditorUtility.ClearProgressBar();
        
        File.WriteAllText( Params.EditorResourcesPath + "/" + fileName, result.ToString().TrimEnd( '\n' ) );
    }
    
    
    static Texture2D GetReadableTexture(Texture2D _gr)
    {   var path = AssetDatabase.GetAssetPath(_gr);
        if (string.IsNullOrEmpty( path )) return _gr;
        if (!((TextureImporter)TextureImporter.GetAtPath( path )).isReadable)
        {
        
            /*if (((TextureImporter)TextureImporter.GetAtPath( path )).textureCompression != TextureImporterCompression.Uncompressed) {
              throw new Exception( "'" + path + "' compressed texture can't be read, turn off compression or enable isReadable options" );
              return null;
            }*/
            
            if (texCopy == null) texCopy = new Texture2D( _gr.width, _gr.height, _gr.format, _gr.mipmapCount > 1, Params.OLD_VERSION );
            texCopy.LoadRawTextureData( _gr.GetRawTextureData() );
            texCopy.Apply();
            return texCopy;
        }
        return _gr;
    }
    
    static double CALC_BRIGHT(Texture2D t)
    {   var data = t. GetPixels();
        List<double>list_b = new List<double>();
        
        for (int i = 0 ; i < data.Length ; i += 4)
        {   var  r = data[i].r;
            var  g = data[i].g;
            var  b = data[i].b;
            
            list_b.Add( GET_SATURATE( r, g, b ) );
        }
        
        return list_b.Sum();
    }
    
    static double GET_SATURATE(double r, double g, double b)
    {   var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var d = max - min;
        if (d == 0) return 0;
        var l  = (max + min) / 2;
        return l > 0.5 ? d / (2 - max - min) : d / (max + min);
    }
    
    static double CALC_HUE(Texture2D t)
    {   var data = t. GetPixels();
        List<double>list_r = new List<double>();
        
        for (int i = 0 ; i < data.Length ; i += 4)
        {   var r = data[i].r;
            var g = data[i].g;
            var b = data[i].b;
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            float dif = max - min;
            
            if (dif == 0) continue;
            
            list_r.Add( r - g + g - b );
        }
        
        if (list_r.Count == 0 && !t.name.Contains( "B&W" )) Debug.LogError( "hue == 0 " + t.name );
        
        return list_r.Count == 0 ? 0 : list_r.Sum();
    }
    
    static double GET_HUE(double r, double g, double b)
    {   var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var d = max - min;
        
        if (d == 0) return 0;
        
        if (r == max) return (g - b) / d + (g < b ? 6 : 0);
        if (g == max) return (b - r) / d + 2;
        if (b == max) return (r - g) / d + 4;
        
        return 0;
    }
    
    static double CALC_FIRSTDIFF(Texture2D t, Texture2D first)
    {   var sourceData = t. GetPixels();
        var firstData = first. GetPixels();
        
        List<double>dif = new List<double>();
        for (int i = 0 ; i < sourceData.Length ; i++)
        {   dif.Add( GetLum( sourceData[i] ) - GetLum( firstData[i] ) );
        }
        
        return dif.Sum();
    }
    
    static double GetLum(Color c)
    {   return (c.r + c.g + c.b) / 3;
    }
    
    
    
    
    
    
    
    
    
    
    void ReadStandardData()
    {   STANDARD_DATA_READY = SORTING_DATA;
    }
    
    
    
    string SORTING_DATA = @"zPost1000 Action 1:6577.81758712561:-0.153926956112812:0
zPost1000 Action 1 +:6568.53806973739:-0.151505359761738:312.034008827664
zPost1000 Action 1 ++:6569.57071708758:-0.148294278929148:699.899383822011
zPost1000 Action 2:6602.19646242694:-0.110215661486324:138.873200573958
zPost1000 Action 2 +:6560.00203953927:-0.104543247115697:475.339889724418
zPost1000 Action 2 ++:6587.12303288149:-0.102124064594339:886.609187156263
zPost1000 Action 3:6651.78488512698:-0.065830265202923:267.223525665389
zPost1000 Action 3 +:6588.65767864095:-0.057561829904042:617.286293889533
zPost1000 Action 3 ++:6592.38596663273:-0.0543895579867351:1056.94121302241
zPost1000 Action 4:6662.14279941236:-0.0190697132949522:-0.836603849505318
zPost1000 Action 4 +:6588.90698841195:-0.00940602111456158:350.296751203006
zPost1000 Action 4 ++:6549.48504139497:-0.00517432687900545:792.796113914497
zPost1000 Action 5:6643.15465606789:0.0294167888403087:-263.460131868713
zPost1000 Action 5 +:6568.75248782942:0.0402632226822178:85.8849863302811
zPost1000 Action 5 ++:6513.11729836899:0.045804781533473:528.835327315943
zPost1000 Action 6:6743.60678916233:-0.07752183277222:-886.743814972653
zPost1000 Action 6 +:6662.46719634749:-0.0820470510489031:-748.555562035273
zPost1000 Action 6 ++:6707.09264595339:-0.084558827044475:-563.427450469913
zPost1000 Action 7:7521.14935589207:-0.0848551456872997:749.860033603154
zPost1000 Action 7 +:7481.84591228351:-0.0620184224775926:659.887498126673
zPost1000 Action 7 ++:7463.49604031809:-0.0496792677409985:638.040459550961
zPost1000 Action 8:7416.76479251125:-0.0778851738114668:162.243054442742
zPost1000 Action 8 +:7286.22161308223:-0.0944307274902485:859.620851002968
zPost1000 Action 8 ++:7146.38350855214:-0.107327096633583:1240.62087117983
zPost1000 Action 9:6388.3596112786:0.078475974463505:-4865.09680195355
zPost1000 Action 9 +:6095.43323129099:0.0976408359047676:-4751.66279589092
zPost1000 Action 9 ++:5792.5855902085:0.113047642353592:-4451.26931428098
zPost1000 Action A:6749.94083174348:-0.163236678682817:-360.70849201538
zPost1000 Action A +:7470.65706025265:-0.109268276176086:-1416.29937579249
zPost1000 Action A ++:6614.41545604247:-0.153740918001194:-263.355522136176
zPost1000 Autumn 1:7591.04924501019:0.0353951197849369:-4370.25759251244
zPost1000 Autumn 1 +:7549.24799288062:0.0407159495951532:-4524.08372961224
zPost1000 Autumn 1 ++:7509.50641558151:0.0439027027762791:-4626.25758237312
zPost1000 Autumn 2:7361.27637856086:-0.0231751705785541:-6015.55434692691
zPost1000 Autumn 2 +:7312.63885370021:-0.023689302422099:-5886.06544163025
zPost1000 Autumn 2 ++:7243.94070146712:-0.0226902409287959:-5760.16869426647
zPost1000 Autumn 3:7008.79690298704:0.050932029342695:-6966.17396589597
zPost1000 Autumn 3 +:6973.31625214682:0.0583418310988895:-6908.26938290467
zPost1000 Autumn 3 ++:6905.84245316956:0.0643540316625035:-6771.6719890005
zPost1000 Autumn 4:7439.19058937881:0.0999435106719488:-6248.53997365281
zPost1000 Autumn 4 +:7424.47807426285:0.107374960029915:-6009.05891851707
zPost1000 Autumn 4 ++:7390.16719433603:0.112108894901269:-5754.64714507199
zPost1000 Autumn 5:6822.40939292674:0.0568462191423678:-1516.04840773977
zPost1000 Autumn 5 +:6795.93594904392:0.0598824244331126:-1046.47845174245
zPost1000 Autumn 5 ++:6781.55158915549:0.0590250836777397:-559.63529931284
zPost1000 Autumn 6:7261.44070762007:0.0972972170212074:-4974.68638380528
zPost1000 Autumn 6 +:7211.99310624282:0.104717177099189:-4815.03931628719
zPost1000 Autumn 6 ++:7152.6717098343:0.107204221732796:-4539.67329438022
zPost1000 Autumn 7:6964.67432853423:0.0163851835520177:-3563.37782074782
zPost1000 Autumn 7 +:6984.76850955041:0.0223355129397191:-3460.39742235373
zPost1000 Autumn 7 ++:6981.06753684133:0.0261484149414741:-3353.24578863822
zPost1000 Autumn 8:6805.43469063156:-0.00862726487633836:-1456.36991987156
zPost1000 Autumn 8 +:6661.34947154734:-0.00251417552090061:-1081.84702792736
zPost1000 Autumn 8 ++:6452.1593309579:-0.00143660614952523:-661.501916434304
zPost1000 Autumn 9:6850.92912710374:-0.122330736243441:-2080.50847200104
zPost1000 Autumn 9 +:6699.80180047266:-0.123494472947471:-1935.98035626895
zPost1000 Autumn 9 ++:7239.61148273565:-0.109925044169358:-1584.67187198468
zPost1000 Autumn A:5937.06638106508:-0.022038724164247:-399.950291765595
zPost1000 Autumn A +:5824.01983554678:-0.0200181017622185:-202.22871000274
zPost1000 Autumn A ++:5937.06638106508:-0.022038724164247:-399.950291765595
zPost1000 B&W Gothic:452.818515261573:-0.00422578677829507:-3625.73333780976
zPost1000 B&W Gothic +:481.662996424369:-0.00418308357190785:-3512.15031294031
zPost1000 B&W Gothic ++:468.467722479476:-0.00431742114931088:-3301.45226064925
zPost1000 B&W GreenDown:0:0:-1189.18836221563
zPost1000 B&W GreenDown +:0:0:-956.655020841001
zPost1000 B&W GreenDown ++:0:0:-525.886381422959
zPost1000 B&W Horror:0:0:-2019.17654338367
zPost1000 B&W Horror +:0:0:-2033.71379283091
zPost1000 B&W Horror ++:0:0:-525.952884497525
zPost1000 B&W Humans:132.680478138037:0.0113389746816347:-3181.75158758421
zPost1000 B&W Humans +:138.696510207645:0.0113398960977994:-2926.54766121474
zPost1000 B&W Humans ++:137.060751425967:0.0113312477578026:-2605.89929199942
zPost1000 B&W Inverse:1665.38179790295:-0.00916631859059532:5519.83015941639
zPost1000 B&W Inverse +:1625.08075229672:-0.00835251941132849:7667.63672031161
zPost1000 B&W Inverse ++:1702.02155377684:-0.00780285243559991:9271.64848611874
zPost1000 B&W RedDown:286.872661736087:0.0160606730565335:-98.2692469405621
zPost1000 B&W RedDown +:263.319151726795:0.0165722422285231:214.011844266401
zPost1000 B&W RedDown ++:243.808776437915:0.0175854383462255:826.826268901671
zPost1000 B&W Yellow:0:0:-6988.25108283205
zPost1000 B&W Yellow +:0:0:-1432.04699705283
zPost1000 B&W Yellow ++:0:0:-2493.92543155411
zPost1000 Back Yard 1:6904.57713968973:-0.0230406558738423:-402.078453026099
zPost1000 Back Yard 1 +:6774.01524203787:-0.0231229932002748:-126.024838506082
zPost1000 Back Yard 1 ++:6698.8836127063:-0.0254289254119726:168.244459016578
zPost1000 Back Yard 2:6575.30159832749:-0.184866159715511:987.201324177894
zPost1000 Back Yard 2 +:6552.62987668174:-0.187894939765158:1232.6876173359
zPost1000 Back Yard 2 ++:6513.93066014219:-0.193110569896425:1458.66410224338
zPost1000 Back Yard 3:7108.4730527155:-0.165342879216025:1943.64048257804
zPost1000 Back Yard 3 +:7106.67771594995:-0.168139115678429:2220.81435889207
zPost1000 Back Yard 3 ++:7074.82414672721:-0.172091804244051:2472.45228383396
zPost1000 Back Yard 4:6545.7902698155:-0.0239612097819304:-1129.19352159089
zPost1000 Back Yard 4 +:6474.78880930811:-0.0221651006021943:-1020.88239624892
zPost1000 Back Yard 4 ++:6503.2883854645:-0.0278037915389859:-901.864084213651
zPost1000 Back Yard 5:5700.6100003605:0.0600052633588299:1972.99346588379
zPost1000 Back Yard 5 +:5798.38957976614:0.0608932288680119:2404.09544317349
zPost1000 Back Yard 5 ++:5867.9971026584:0.0595707871945024:2882.32160898845
zPost1000 Back Yard 6:6805.12278250205:-0.053729373493125:1793.75807763611
zPost1000 Back Yard 6 +:6960.64491753349:-0.0517630815514849:2236.74501965886
zPost1000 Back Yard 6 ++:7098.72547134871:-0.0529536265381125:2780.87052548119
zPost1000 Back Yard 7:5400.64612085043:-0.0701530957113619:1176.32949736022
zPost1000 Back Yard 7 +:5148.2102807402:-0.0638174070177797:1739.32560418178
zPost1000 Back Yard 7 ++:5026.26760276316:-0.0594425026599197:2233.00014243665
zPost1000 Back Yard 8:7068.4265167532:-0.174409282225326:1859.37775042535
zPost1000 Back Yard 8 +:7087.15763121493:-0.173457723483796:2151.223518799
zPost1000 Back Yard 8 ++:7062.1462975143:-0.173704287720822:2373.83138172632
zPost1000 Back Yard 9:7543.12853152895:-0.0705398870074418:-2894.59227082523
zPost1000 Back Yard 9 +:7503.60460949058:-0.0659873826254511:-2756.98050033403
zPost1000 Back Yard 9 ++:7455.27278202211:-0.0640529273268271:-2635.0798388436
zPost1000 Back Yard A:7521.19464860014:-0.0799603679022312:-1895.03797304558
zPost1000 Back Yard A +:7532.58683687546:-0.0774074232268163:-1847.62228050663
zPost1000 Back Yard A ++:7526.21395038948:-0.0752054556658256:-1859.61050661846
zPost1000 Bleach Bypass 1:4542.22452376292:-0.0336270576479154:-1135.98431928285
zPost1000 Bleach Bypass 1 +:4426.53342053226:-0.0278531448601673:-1060.21698105583
zPost1000 Bleach Bypass 1 ++:4377.42252990358:-0.0208791223400123:-927.560756240164
zPost1000 Bleach Bypass 2:6953.92292009124:-0.0960793923364918:1170.22609711792
zPost1000 Bleach Bypass 2 +:6822.58194916113:-0.0731483643050979:1166.30846879971
zPost1000 Bleach Bypass 2 ++:6743.09669860255:-0.0532772313646888:1177.20129929786
zPost1000 Bleach Bypass 3:6918.36616153376:-0.134977215320532:1282.41300628208
zPost1000 Bleach Bypass 3 +:6908.65062461414:-0.118222057611635:1360.21694328716
zPost1000 Bleach Bypass 3 ++:6985.6731283763:-0.0949979910346883:1469.43525028976
zPost1000 Bleach Bypass 4:1841.83485803947:-0.0217955454613899:841.159600352087
zPost1000 Bleach Bypass 4 +:1510.58176335474:-0.0206017741910361:1362.89036091152
zPost1000 Bleach Bypass 4 ++:1377.62756731088:-0.0197036785165982:1818.49692799603
zPost1000 Bleach Bypass 5:7255.24022873807:-0.0779593960132372:2767.36077513999
zPost1000 Bleach Bypass 5 +:7243.30408160164:-0.0709439698940327:3213.72811381974
zPost1000 Bleach Bypass 5 ++:7223.78018868008:-0.0607287952643674:3746.44839067898
zPost1000 Bleach Bypass 6:5488.18047936495:0.00157542090494189:3294.29543651848
zPost1000 Bleach Bypass 6 +:5111.54810775001:-0.00416571493514084:3988.96083838521
zPost1000 Bleach Bypass 6 ++:4839.60679889318:-0.00962537047456635:4672.47852955969
zPost1000 Bleach Bypass 7:7187.62551481572:-0.0381226916485084:-3121.73859380695
zPost1000 Bleach Bypass 7 +:7046.29728714697:-0.0353981020029307:-3242.19609713035
zPost1000 Bleach Bypass 7 ++:6910.43078363304:-0.0349299476389456:-3399.36471686179
zPost1000 Bleach Bypass 8:6040.89768122369:0.115751483728124:-2614.9386230851
zPost1000 Bleach Bypass 8 +:5915.55360512385:0.0846112610585759:-1904.90592609992
zPost1000 Bleach Bypass 8 ++:7199.06213141677:0.0224011506859334:-434.873306536997
zPost1000 Bleach Bypass 9:4613.3645080031:-0.0469064131666879:2026.67066850509
zPost1000 Bleach Bypass 9 +:5473.69051725718:-0.0867717997912665:2248.88633541167
zPost1000 Bleach Bypass 9 ++:5416.7410838766:-0.0908371940715466:2914.715099284
zPost1000 Bleach Bypass A:5479.16654456231:0.0232603419161087:103.589536547655
zPost1000 Bleach Bypass A +:7307.40449178342:-0.206033448878315:1603.19077595995
zPost1000 Bleach Bypass A ++:7820.19633481573:-0.159036508951033:2063.3632632233
zPost1000 Bleak 1:2061.9835196228:0.0460504200995783:1306.2916398837
zPost1000 Bleak 1 +:1829.66114051495:0.0449716302113078:1478.2589815151
zPost1000 Bleak 1 ++:1694.80934813274:0.0454583989691373:1675.15833486859
zPost1000 Bleak 2:6860.50105261387:0.0531704686873695:4443.37776152751
zPost1000 Bleak 2 +:7221.51601486238:0.0525740001278905:4520.01564808905
zPost1000 Bleak 2 ++:7139.99613006681:0.069857720273319:5420.09540283828
zPost1000 Bleak 3:5580.66857918071:-0.0631850871630195:-704.411750629076
zPost1000 Bleak 3 +:5738.42299413193:-0.0651094363898892:-552.360758651994
zPost1000 Bleak 3 ++:5861.30336937421:-0.0696906630348622:-325.758134196808
zPost1000 Bleak 4:5277.83082700082:-0.0381862178882664:2031.52158590326
zPost1000 Bleak 4 +:5367.06910060123:-0.0357776740867203:2522.64448539602
zPost1000 Bleak 4 ++:5455.6351743668:-0.03490248994009:3180.95954659853
zPost1000 Bleak 5:5851.09439160386:-0.155823955550829:-1133.54894766035
zPost1000 Bleak 5 +:5856.61572933093:-0.156238439886229:-855.142391925818
zPost1000 Bleak 5 ++:5871.19661716946:-0.152909108321694:-601.920162130776
zPost1000 Bleak 6:6379.83373007381:-0.0772705156388724:-1155.41565333243
zPost1000 Bleak 6 +:6309.01796202902:-0.0763195458359693:-848.152884126949
zPost1000 Bleak 6 ++:6240.5939300022:-0.0772480158360622:-563.152870312363
zPost1000 Bleak 7:5365.24514073974:-0.153174790790672:-1040.35285473273
zPost1000 Bleak 7 +:5276.55674961846:-0.151679309929648:-762.04825981271
zPost1000 Bleak 7 ++:5561.86795371517:-0.134301283248192:-1567.15286900758
zPost1000 Bleak 8:5336.17465049236:-0.055125041620272:-1131.86007690256
zPost1000 Bleak 8 +:5231.21890540121:-0.0567322846593822:-585.728021944415
zPost1000 Bleak 8 ++:5188.68027988617:-0.0540656632821879:25.8785390730631
zPost1000 Bleak 9:5281.99207066519:-0.188391987436289:-2913.01958484194
zPost1000 Bleak 9 +:5111.83467874412:-0.182884524110477:-3113.33722679226
zPost1000 Bleak 9 ++:5029.8746973613:-0.176407834720274:-3174.71108394977
zPost1000 Bleak A:5226.63265097883:-0.0911568467172401:-1962.00648761907
zPost1000 Bleak A +:4920.17303303268:-0.0764083593181226:-1537.19600551082
zPost1000 Bleak A ++:4829.44061840016:-0.0599834391607601:-1143.33325460348
zPost1000 Cinema 1:7258.29805984899:-0.00288852178397292:-231.519010916816
zPost1000 Cinema 1 +:7114.45536431948:-0.00123028009880954:86.6862372923663
zPost1000 Cinema 1 ++:7003.57337013132:0.000516522492830518:452.295405765386
zPost1000 Cinema 2:7353.87511457523:-0.114439986130928:679.592097770151
zPost1000 Cinema 2 +:7302.55610543646:-0.110898037984346:846.762047905436
zPost1000 Cinema 2 ++:7313.97633014808:-0.105505088720736:1040.39604657353
zPost1000 Cinema 3:7437.20629654768:-0.114806069129722:-584.588296839045
zPost1000 Cinema 3 +:7345.37910907908:-0.121737636555557:-352.094160519372
zPost1000 Cinema 3 ++:7322.03058009297:-0.127965595391629:-123.424862066751
zPost1000 Cinema 4:6756.0242085297:-0.166682470263424:-1521.937255449
zPost1000 Cinema 4 +:6722.68901573846:-0.171687353060975:-1339.11240475922
zPost1000 Cinema 4 ++:6777.33070336257:-0.174044985691182:-1164.67971886846
zPost1000 Cinema 5:6011.6604472296:0.12877986602274:688.762113950092
zPost1000 Cinema 5 +:5783.68063350989:0.130267418217157:1054.95560307087
zPost1000 Cinema 5 ++:5606.16578286817:0.127531877150659:1384.94385987895
zPost1000 Cinema 6:7153.06998347063:-0.137884885295989:3569.09929892849
zPost1000 Cinema 6 +:7247.98496269354:-0.133187333487285:4227.04703173448
zPost1000 Cinema 6 ++:7340.51213837363:-0.130018293107526:4991.19868584544
zPost1000 Cinema 7:6087.00765256405:-0.223200025639261:-1522.03920794951
zPost1000 Cinema 7 +:5904.56246926483:-0.230918260348403:-1234.3895096304
zPost1000 Cinema 7 ++:5938.34402248849:-0.231276788484357:-918.269240628904
zPost1000 Cinema 8:6530.96423485914:-0.193144441245408:-2269.1346628966
zPost1000 Cinema 8 +:6347.08479462337:-0.197380041938402:-2174.46013207518
zPost1000 Cinema 8 ++:6220.44798617525:-0.197859706355018:-1962.91109774886
zPost1000 Cinema 9:7066.22525029047:-0.0695063626239971:-1013.58306194967
zPost1000 Cinema 9 +:6919.00345527667:-0.0619844376603851:-1027.19219677365
zPost1000 Cinema 9 ++:6764.97512058705:-0.0529909670846678:-1128.16472664754
zPost1000 Cinema A:7100.08182739502:-0.0892027664858688:-1945.61836256387
zPost1000 Cinema A +:6939.28645004667:-0.08293081633451:-2050.38044054683
zPost1000 Cinema A ++:6806.15002700967:-0.0780972397686241:-2243.88499919015
zPost1000 City 1:5002.75323867755:-0.0198022005136522:1459.27849084186
zPost1000 City 1 +:4652.14941941221:-0.0184254394420122:1742.82231142113
zPost1000 City 1 ++:4525.09542985757:-0.016990753868356:2014.33997731403
zPost1000 City 2:6027.16498826095:-0.0555434334685287:909.295463608406
zPost1000 City 2 +:5861.67863937867:-0.0563117578050196:1098.66018137352
zPost1000 City 2 ++:5732.39327373601:-0.0584697687607445:1265.63665875466
zPost1000 City 3:6520.08741549031:-0.0760140885756888:-1584.21175725782
zPost1000 City 3 +:6366.23577991981:-0.0858771377598416:-1443.91500294156
zPost1000 City 3 ++:6251.88883890449:-0.0959520561218028:-1249.90713973358
zPost1000 City 4:6632.2335778652:-0.027163280411628:1273.46797023219
zPost1000 City 4 +:6498.92433117516:-0.0196073673426297:2025.18302683198
zPost1000 City 4 ++:6439.06717487613:-0.0147934882443792:2816.27847983314
zPost1000 City 5:4971.61374238545:-0.0448153307839924:300.51896980513
zPost1000 City 5 +:5001.4101773994:-0.0399825776169109:496.431402229126
zPost1000 City 5 ++:5016.12626343351:-0.0361141452740981:778.426183764335
zPost1000 City 6:7371.873951801:-0.199592627200786:2314.389497178
zPost1000 City 6 +:7245.68714420545:-0.211952365624882:2857.14245962609
zPost1000 City 6 ++:7152.12892611413:-0.222007129546114:3255.44836926956
zPost1000 City 7:4329.33692337885:0.0254203015410894:3854.13341190515
zPost1000 City 7 +:4086.47816976171:0.0249296252314366:4293.88900200417
zPost1000 City 7 ++:3981.05367782469:0.0232038850992353:4720.06027494152
zPost1000 City 8:6980.16632205447:0.0321336845470341:1330.42094514407
zPost1000 City 8 +:6888.20102833233:0.0383353354779956:1469.75559853077
zPost1000 City 8 ++:6837.22475946398:0.0418878213121729:1631.52030787506
zPost1000 City 9:7087.12641635151:-0.00722082165998472:1506.57639925582
zPost1000 City 9 +:7564.01564568919:-0.00184302567680561:1393.85874161761
zPost1000 City 9 ++:7135.33020260255:0.0135330470284316:2103.88622053323
zPost1000 City A:7374.03432978195:0.0417676020534164:844.307093361111
zPost1000 City A +:7372.26519126868:0.0460797445709869:1343.36592867973
zPost1000 City A ++:7452.5892474985:0.0818335413843556:-608.275899482553
zPost1000 Cold 1:6383.32159366667:-0.0990473754740151:690.185629587911
zPost1000 Cold 1 +:6339.66547593348:-0.0908740129456663:1005.15558469058
zPost1000 Cold 1 ++:6315.13255335197:-0.0876450504732134:1464.33076151702
zPost1000 Cold 2:5711.6744583315:-0.0913744670551575:-208.762039999171
zPost1000 Cold 2 +:5728.81978791473:-0.0815467023568237:8.04059013474203
zPost1000 Cold 2 ++:5747.02917850934:-0.076452399572986:350.844519789675
zPost1000 Cold 3:5874.60371355079:-0.206539146399564:-68.6025507276445
zPost1000 Cold 3 +:5860.82018191041:-0.212994511715863:130.692886276536
zPost1000 Cold 3 ++:5857.6754422429:-0.214834662179953:322.264140017349
zPost1000 Cold 4:5468.55674945381:-0.0308459758946924:-2690.32158986945
zPost1000 Cold 4 +:5557.33321126119:-0.0397652471297647:-2526.67974839921
zPost1000 Cold 4 ++:5640.86306658412:-0.0459329098791841:-2325.52157381536
zPost1000 Cold 5:7361.80889210587:-0.174294390819:-5248.04453263291
zPost1000 Cold 5 +:7265.88270194844:-0.175730990759291:-5779.28896799691
zPost1000 Cold 5 ++:7243.54881036709:-0.173493992432554:-6224.57262748412
zPost1000 Cold 6:4539.77769734462:0.111107602684793:-2376.40653026468
zPost1000 Cold 6 +:4098.88064247623:0.113851423325672:-2264.57775367272
zPost1000 Cold 6 ++:3882.73337545789:0.116170729067903:-2127.19996686919
zPost1000 Cold 7:5675.46511229665:0.102588826808485:-176.614361366104
zPost1000 Cold 7 +:5234.56169077068:0.110094671261796:477.351687892343
zPost1000 Cold 7 ++:5064.08812942343:0.11325606796936:1066.61707175256
zPost1000 Cold 8:4961.54246582533:-0.0718223436851986:-3853.93853599659
zPost1000 Cold 8 +:5035.98984288833:-0.0725286735756478:-4235.82219965961
zPost1000 Cold 8 ++:5021.30574371396:-0.0756395545231499:-4434.51501577676
zPost1000 Cold 9:6351.98337699989:-0.11455529594347:-978.585623552607
zPost1000 Cold 9 +:6439.48878665676:-0.113375080289359:-939.120252162167
zPost1000 Cold 9 ++:6476.8515846239:-0.112578511888671:-855.793449348473
zPost1000 Cold A:6125.38159246747:-0.189246573916242:1154.63660488323
zPost1000 Cold A +:6865.88138956803:-0.193595402988063:1422.11638096053
zPost1000 Cold A ++:5006.70626768225:-0.155803114362743:2375.23406823162
zPost1000 Contrast Air 1:6506.73454419139:-0.0129533382903446:3470.65747029562
zPost1000 Contrast Air 1 +:6409.79294271922:-0.0145364225699609:3606.54375919782
zPost1000 Contrast Air 1 ++:6422.9362801146:-0.0186111773697917:3607.49017505626
zPost1000 Contrast Air 2:7534.00879483985:-0.148605527018276:1208.01162462703
zPost1000 Contrast Air 2 +:7592.43873927176:-0.131857958823446:1162.84823517043
zPost1000 Contrast Air 2 ++:7646.80938573219:-0.11618652424994:1146.47831065173
zPost1000 Contrast Air 3:7861.32851605597:-0.0776941671373947:-2952.31514752097
zPost1000 Contrast Air 3 +:7810.87274366497:-0.0811710170884226:-2788.61316771958
zPost1000 Contrast Air 3 ++:7739.60160444031:-0.0955633465774213:-2483.74778462504
zPost1000 Contrast Air 4:6827.00753403938:-0.00215945157947317:-1283.88110673002
zPost1000 Contrast Air 4 +:6918.95613205942:0.00936877203088216:-983.247104027582
zPost1000 Contrast Air 4 ++:6955.47852136013:0.0166073013983805:-659.755588704075
zPost1000 Contrast Air 5:7321.3554831277:-0.0366124799521685:-3474.24583663401
zPost1000 Contrast Air 5 +:7152.00755460058:-0.0577923981342678:-2149.77915661779
zPost1000 Contrast Air 5 ++:7037.56063313521:-0.0884488028462442:-822.234039813046
zPost1000 Contrast Air 6:7669.27184160912:0.0204517707613572:-3984.44322685145
zPost1000 Contrast Air 6 +:7681.1801905531:0.00759794491480016:-2143.68502153328
zPost1000 Contrast Air 6 ++:7561.11377299424:-0.027338463343084:-528.079777829648
zPost1000 Contrast Air 7:7780.37944308036:-0.0768358448241884:-4289.68898595858
zPost1000 Contrast Air 7 +:7769.56535093196:-0.0759148124855074:-4131.96218585199
zPost1000 Contrast Air 7 ++:7743.76932369537:-0.0758157202372445:-4028.61446793317
zPost1000 Contrast Air 8:7905.45631775047:-0.0429481735569652:-1794.20533103093
zPost1000 Contrast Air 8 +:7867.17917711802:-0.0373228863340387:-1707.21839252018
zPost1000 Contrast Air 8 ++:7828.09638295779:-0.0344310165810953:-1649.95955846079
zPost1000 Contrast Air 9:7145.26691386719:-0.212439688649738:852.803861776961
zPost1000 Contrast Air 9 +:6896.18824158847:-0.237923825132548:1649.91500755416
zPost1000 Contrast Air 9 ++:6691.77276449827:-0.261898750044111:2430.05491229202
zPost1000 Contrast Air A:7746.2874485485:-0.216718853313182:-3248.28243896466
zPost1000 Contrast Air A +:7718.83725190657:-0.210990165773183:-3268.14386083567
zPost1000 Contrast Air A ++:7376.66164992082:-0.240096516232597:-2321.39089952183
zPost1000 Dramatical 1:5823.02968870407:-0.0940582889394932:1159.42225866161
zPost1000 Dramatical 1 +:5582.78269429415:-0.0860925803191577:1627.05888628493
zPost1000 Dramatical 1 ++:5533.51570226348:-0.0803524255105117:2057.05367507946
zPost1000 Dramatical 2:5997.95912132316:0.0200727232610219:1009.90851032951
zPost1000 Dramatical 2 +:5703.9388875661:0.0268625767463565:1293.62618099111
zPost1000 Dramatical 2 ++:5619.43295584653:0.0306779447979579:1600.13600141745
zPost1000 Dramatical 3:7077.66649765887:0.0722270465277212:-2454.2484104782
zPost1000 Dramatical 3 +:6875.66133942175:0.0723285446847509:-2103.34773381912
zPost1000 Dramatical 3 ++:6733.8698358078:0.0693935655494094:-1719.99608173334
zPost1000 Dramatical 4:5677.19351095157:-0.0896895493020522:2150.93338836903
zPost1000 Dramatical 4 +:5598.3144392106:-0.0920917639247136:2687.8484459547
zPost1000 Dramatical 4 ++:5548.30451564075:-0.0931975772681994:3220.15958485803
zPost1000 Dramatical 5:6631.49983871385:-0.0980650716621199:-1548.586931003
zPost1000 Dramatical 5 +:6490.0116414789:-0.095606911065147:-1043.54638397455
zPost1000 Dramatical 5 ++:6412.88186143001:-0.100129735802739:-411.223495461669
zPost1000 Dramatical 6:6130.84564364932:-0.0012652286356456:237.766077232195
zPost1000 Dramatical 6 +:5947.45279373834:0.000164669810146734:449.156945915671
zPost1000 Dramatical 6 ++:5955.20390533427:0.00141074472156788:623.153032741634
zPost1000 Dramatical 7:6474.43983875672:-0.0271541132021386:-5482.78700768878
zPost1000 Dramatical 7 +:6601.80937929406:-0.0307808709736719:-4365.12422724666
zPost1000 Dramatical 7 ++:6602.68245198023:-0.0306466554994871:-3316.7908605567
zPost1000 Dramatical 8:7038.84843948274:0.00476265447906599:-6762.9844191945
zPost1000 Dramatical 8 +:6967.56123558675:0.00889629047901508:-5964.39876960177
zPost1000 Dramatical 8 ++:6780.50763873969:0.00114602396945429:-5159.67324695636
zPost1000 Dramatical 9:5643.55562730357:0.0316647014127517:-4159.13340450321
zPost1000 Dramatical 9 +:5948.41172806287:0.0514134592625806:-3989.1150931984
zPost1000 Dramatical 9 ++:6131.48046034591:0.0674505010206293:-3700.00135484934
zPost1000 Dramatical A:5719.2136703639:-0.00870013265834014:-735.30976839529
zPost1000 Dramatical A +:5645.38867014882:-0.00471574873029113:-409.236548175406
zPost1000 Dramatical A ++:5548.76510601548:0.00361231844777876:-31.0966581210653
zPost1000 Exterior Wide 1:6006.42981346236:0.0277267088471262:1430.9241871033
zPost1000 Exterior Wide 1 +:5964.98693480523:0.0315300361564255:1747.65623561373
zPost1000 Exterior Wide 1 ++:5975.56715950322:0.0328450443909674:2059.62749354913
zPost1000 Exterior Wide 2:7456.37879906175:0.0109571204948224:1069.91367794589
zPost1000 Exterior Wide 2 +:7379.58673408088:0.00658365188655807:1540.71892449568
zPost1000 Exterior Wide 2 ++:7300.16878787795:0.000816667616732047:2087.02351852596
zPost1000 Exterior Wide 3:7679.54191671142:0.0229712307541945:39.488862380985
zPost1000 Exterior Wide 3 +:7591.09132485367:0.0124324935781601:435.471884682465
zPost1000 Exterior Wide 3 ++:7554.58311857977:0.00193540387806479:781.800003819971
zPost1000 Exterior Wide 4:7273.04650221729:0.0330920571785782:1212.38560872866
zPost1000 Exterior Wide 4 +:7230.76560944348:0.0400261286828254:1647.83399783988
zPost1000 Exterior Wide 4 ++:7190.8737207376:0.0360108204292273:2076.034022655
zPost1000 Exterior Wide 5:6888.37559329829:-0.0261512940795114:-4852.97914306077
zPost1000 Exterior Wide 5 +:6502.80841568577:-0.0477318889239768:-3455.68103598225
zPost1000 Exterior Wide 5 ++:6207.28538845648:-0.0701588390818415:-2264.78687288198
zPost1000 Exterior Wide 6:7228.4235224818:0.0280608509740432:-6097.8798319805
zPost1000 Exterior Wide 6 +:7194.14157880961:0.0343544135050138:-5987.33080539607
zPost1000 Exterior Wide 6 ++:7128.76880646481:0.0379011546505694:-5825.5621729489
zPost1000 Exterior Wide 7:7288.30938104163:-0.0288382763439472:47.9201755615157
zPost1000 Exterior Wide 7 +:7259.42059728385:-0.0412305674469167:578.684890855108
zPost1000 Exterior Wide 7 ++:7241.38490775862:-0.0559192159257691:954.739804691812
zPost1000 Exterior Wide 8:7017.70973399196:-0.0368298963419679:-1913.73734748208
zPost1000 Exterior Wide 8 +:6982.77616060563:-0.0406765815461343:-1736.70465270579
zPost1000 Exterior Wide 8 ++:6939.67281069313:-0.0456756736363812:-1518.7726108675
zPost1000 Exterior Wide 9:6877.00260706918:-0.0502024967070156:1840.74241121441
zPost1000 Exterior Wide 9 +:6860.69143375897:-0.0415053461044295:2317.5698808359
zPost1000 Exterior Wide 9 ++:6808.32481814076:-0.0309670875025176:2769.97905521607
zPost1000 Exterior Wide A:6920.04606183677:-0.0751914894403285:2249.02086264996
zPost1000 Exterior Wide A +:6916.48275726097:-0.0759081139227078:2929.29670151861
zPost1000 Exterior Wide A ++:6896.33205914073:-0.0803059961930899:3644.76469665713
zPost1000 Glowing 1:6599.97283565744:-0.0460615622939046:2414.73464506074
zPost1000 Glowing 1 +:6544.17165089915:-0.0410270850854711:2806.1045952555
zPost1000 Glowing 1 ++:6548.79201688972:-0.035540180250905:3228.26931294364
zPost1000 Glowing 2:6488.68069770199:-0.0542743812773097:3814.14772314535
zPost1000 Glowing 2 +:6215.25524133694:-0.0512288772160404:4785.70722400254
zPost1000 Glowing 2 ++:6104.80066793746:-0.0433182339257323:5513.56606972377
zPost1000 Glowing 3:6477.15513997788:-0.0762685295883498:4852.19346363094
zPost1000 Glowing 3 +:6306.55173260208:-0.0798263630496186:5823.89413793784
zPost1000 Glowing 3 ++:6260.90024164774:-0.0773794025590391:6663.04448800059
zPost1000 Glowing 4:6157.91330037706:-0.0861677635350588:-1432.16599152892
zPost1000 Glowing 4 +:6002.25069692849:-0.0861347321429093:-1370.68362218442
zPost1000 Glowing 4 ++:5911.65042531146:-0.0865665264104223:-825.816947105865
zPost1000 Glowing 5:6075.10806412334:-0.092980723885546:2481.32291843373
zPost1000 Glowing 5 +:5998.65672655201:-0.0914751902108719:3106.81313060778
zPost1000 Glowing 5 ++:5929.74658968992:-0.0882683309455956:3680.11118431693
zPost1000 Glowing 6:7573.69949488791:0.472064565534708:2035.26395505988
zPost1000 Glowing 6 +:7445.83418131192:0.477299227624087:2422.81953107003
zPost1000 Glowing 6 ++:7363.71506985806:0.476998120500355:2839.38294387123
zPost1000 Glowing 7:6482.44178728414:0.115613489546068:38.3176101729174
zPost1000 Glowing 7 +:6477.24186187943:0.122590663441542:331.494094072844
zPost1000 Glowing 7 ++:6481.10129000989:0.122732024066338:669.90587572637
zPost1000 Glowing 8:7447.96572811408:-0.00614900156494969:1177.03525295188
zPost1000 Glowing 8 +:7385.83806424733:0.000557820694589298:1450.4012783808
zPost1000 Glowing 8 ++:7344.88266454786:0.00421883569401871:1808.03396984737
zPost1000 Glowing 9:7762.48247566661:-0.0432973462799498:-209.177853622181
zPost1000 Glowing 9 +:7684.02429871839:-0.0234077390124182:344.159418897701
zPost1000 Glowing 9 ++:7839.41745154438:0.0262573470701445:-943.411828844798
zPost1000 Glowing A:7559.497833243:-0.0741414455045949:-248.811785861542
zPost1000 Glowing A +:7466.01525112578:-0.0704990740250659:246.213070653693
zPost1000 Glowing A ++:7786.36999744799:-0.0018365660011053:-858.224852399122
zPost1000 Hair Raiser 1:4211.64635735777:-0.0742890954318036:4018.98571302011
zPost1000 Hair Raiser 1 +:4005.48028206911:-0.0710359383027055:4315.52561044093
zPost1000 Hair Raiser 1 ++:3877.785700462:-0.0627666447015904:4616.3648590273
zPost1000 Hair Raiser 2:6603.13575865887:-0.159799140836867:-956.166029938542
zPost1000 Hair Raiser 2 +:6401.25512895595:-0.175264104817028:-836.575163032198
zPost1000 Hair Raiser 2 ++:6269.00228843446:-0.185005941912721:-690.356842314045
zPost1000 Hair Raiser 3:6207.74939740528:-0.138300181329717:442.01440674766
zPost1000 Hair Raiser 3 +:6207.66779235303:-0.144694876677831:988.39220964717
zPost1000 Hair Raiser 3 ++:6274.37268364979:-0.149626137309781:1552.31119205668
zPost1000 Hair Raiser 4:7194.31817816671:-0.0247649572797854:-5652.49156469674
zPost1000 Hair Raiser 4 +:7220.06901651643:-0.0166901248692284:-5844.37260687049
zPost1000 Hair Raiser 4 ++:7224.6804315297:-0.012366441896063:-6057.29286783466
zPost1000 Hair Raiser 5:7041.32531803238:-0.152725104101591:-1037.72948576574
zPost1000 Hair Raiser 5 +:7074.94979398435:-0.156926059379908:-1013.28895484729
zPost1000 Hair Raiser 5 ++:7063.8906379296:-0.156858911035038:-786.121624053118
zPost1000 Hair Raiser 6:6716.36911784561:-0.0873595773282916:1223.88754292564
zPost1000 Hair Raiser 6 +:6747.74540919756:-0.0758412521723178:1478.24833816713
zPost1000 Hair Raiser 6 ++:6793.02300939429:-0.0631539653469923:1710.28495015352
zPost1000 Hair Raiser 7:5241.90547174187:0.0337664832925477:-5778.17651042271
zPost1000 Hair Raiser 7 +:5069.72308696896:0.0327327616228884:-5600.9438217235
zPost1000 Hair Raiser 7 ++:4954.12654077467:0.0282527286434568:-5361.50068375642
zPost1000 Hair Raiser 8:5885.60101562371:-0.0216002225496936:-6066.46147940569
zPost1000 Hair Raiser 8 +:5885.60101562371:-0.0216002225496936:-6066.46147940569
zPost1000 Hair Raiser 8 ++:5885.60101562371:-0.0216002225496936:-6066.46147940569
zPost1000 Hair Raiser 9:5795.94929806119:-0.0396632805540662:-2558.43529369841
zPost1000 Hair Raiser 9 +:5686.08577438263:-0.0257970509117627:-2433.39737043654
zPost1000 Hair Raiser 9 ++:5680.75892184324:-0.0128867987212402:-2242.10716530295
zPost1000 Hair Raiser A:6057.46968492186:-0.0465456308808495:-3914.03270809083
zPost1000 Hair Raiser A +:6271.01140736916:-0.0425369987666908:-4078.19348603503
zPost1000 Hair Raiser A ++:6373.0462301656:-0.0425137531659184:-4064.46930119399
zPost1000 Industrial 1:5944.75586476926:-0.0961789636138519:1558.00521933644
zPost1000 Industrial 1 +:5954.30227923137:-0.0916772020360668:1742.29673981106
zPost1000 Industrial 1 ++:6069.80970620095:-0.0895440830570919:1914.31374323652
zPost1000 Industrial 2:5977.30374446095:-0.0889953378379434:1817.41702783425
zPost1000 Industrial 2 +:5834.06882406051:-0.0836025226801887:2321.60267569891
zPost1000 Industrial 2 ++:5850.50480308247:-0.0779037756548369:2767.45760360501
zPost1000 Industrial 3:7276.02341252:0.0677644749973167:-4851.46413369202
zPost1000 Industrial 3 +:7295.3114728624:0.0834920628403992:-4746.37261689503
zPost1000 Industrial 3 ++:7211.50774967095:0.0930718948717214:-4411.35953384009
zPost1000 Industrial 4:6094.27843852329:0.0949091431593252:-6565.16351001509
zPost1000 Industrial 4 +:6267.23213911406:0.109905420520044:-6862.64063273291
zPost1000 Industrial 4 ++:6303.1391775463:0.121318744306507:-7014.72821106212
zPost1000 Industrial 5:7569.42417542256:-0.212332453303418:-695.234076427617
zPost1000 Industrial 5 +:7523.57758130293:-0.218275123371882:-370.244516504345
zPost1000 Industrial 5 ++:7501.84589926089:-0.222550935029005:-56.6706460679131
zPost1000 Industrial 6:6701.02442411216:-0.14442242299393:-1120.57258213978
zPost1000 Industrial 6 +:6608.06321913331:-0.156309842760265:-990.375176032682
zPost1000 Industrial 6 ++:6531.49249680735:-0.170904942857143:-868.789534619983
zPost1000 Industrial 7:4550.75720708009:-0.0504173600388003:-1888.00125690798
zPost1000 Industrial 7 +:4445.00394662634:-0.0503169797921488:-1932.09013136937
zPost1000 Industrial 7 ++:4369.30167683958:-0.0474519398881029:-1888.39470127089
zPost1000 Industrial 8:7679.91091842999:0.00371619999367567:-4543.32295184272
zPost1000 Industrial 8 +:7663.63517872899:0.0229899067130646:-4736.88895450434
zPost1000 Industrial 8 ++:7827.22423915706:0.00449088519428867:-3511.33077741622
zPost1000 Industrial 9:7670.43256396035:-0.0029742120825631:-4303.23407229489
zPost1000 Industrial 9 +:7583.76316192492:0.00136383160639753:-3696.42881013331
zPost1000 Industrial 9 ++:7793.81665418009:-0.0240915567555575:-2892.69415800061
zPost1000 Industrial A:7202.16973089772:-0.106446344403366:-4285.50853567434
zPost1000 Industrial A +:7182.82987667369:-0.108188853477741:-4276.55035963834
zPost1000 Industrial A ++:7146.64744118421:-0.10879576936992:-4273.07323242791
zPost1000 Interior Blue 1:6348.09460643099:-0.235009258082918:150.477130422064
zPost1000 Interior Blue 1 +:6099.79593580738:-0.261483711145502:619.877164922333
zPost1000 Interior Blue 1 ++:5996.76195999375:-0.287565589629537:1078.67980911966
zPost1000 Interior Blue 2:6498.49463119052:-0.247611326043269:769.55817404296
zPost1000 Interior Blue 2 +:6287.65164034651:-0.268827079764037:1308.97128103396
zPost1000 Interior Blue 2 ++:6113.52035351106:-0.291348812641047:1694.91248634567
zPost1000 Interior Blue 3:6255.06861014412:-0.249029425069876:-987.022172402887
zPost1000 Interior Blue 3 +:6229.42807466055:-0.248501170500219:-648.252221355381
zPost1000 Interior Blue 3 ++:6202.52676077401:-0.248707971765327:-253.060056028715
zPost1000 Interior Blue 4:6045.47562684828:-0.173852062894071:-875.606482046182
zPost1000 Interior Blue 4 +:6056.62289432196:-0.174383427344083:-521.428685818657
zPost1000 Interior Blue 4 ++:6011.79174365507:-0.174932982828182:-107.708415375692
zPost1000 Interior Blue 5:5716.42366729275:-0.0825257582414451:-252.152883641268
zPost1000 Interior Blue 5 +:6697.26957074937:-0.0678600457975511:-916.688893300157
zPost1000 Interior Blue 5 ++:6657.77486055801:-0.0625220266211954:64.4614588447561
zPost1000 Interior Blue 6:6310.77662942098:-0.1157456496449:1182.74770653974
zPost1000 Interior Blue 6 +:7263.47832361168:-0.118637508667405:861.176411888409
zPost1000 Interior Blue 6 ++:7477.05919855008:-0.148157386580541:1257.54370611268
zPost1000 Interior Blue 7:5781.8654872419:-0.113625921233051:1109.25229333247
zPost1000 Interior Blue 7 +:7064.67027821888:-0.141759410916885:1080.22346619592
zPost1000 Interior Blue 7 ++:7254.76506924829:-0.099586401236138:190.514979690454
zPost1000 Interior Blue 8:5095.51827891127:-0.111035937932348:639.534707173542
zPost1000 Interior Blue 8 +:5931.04534040663:-0.137381314949652:150.236624851986
zPost1000 Interior Blue 8 ++:6630.56629280435:-0.0533466473249291:254.956886700976
zPost1000 Interior Blue 9:6316.04560499702:-0.123213470870382:-685.214337478205
zPost1000 Interior Blue 9 +:6999.42245425683:-0.127045042961925:-171.989541696079
zPost1000 Interior Blue 9 ++:6751.92158628722:-0.146668205906224:-341.377742173925
zPost1000 Interior Blue A:6060.73625726229:-0.129682240548448:388.635358653452
zPost1000 Interior Blue A +:6671.27305035102:-0.120012836374428:544.521592768492
zPost1000 Interior Blue A ++:6722.09660731287:-0.161439671262372:1343.12420457268
zPost1000 Interior Blue B:5393.1707501329:-0.151743307768875:434.428793539743
zPost1000 Interior Blue B +:6329.09840455283:-0.166920865386658:107.219614190047
zPost1000 Interior Blue B ++:7492.93805103771:-0.150460043213911:146.886218867274
zPost1000 Interior Blue C:5307.84021768588:-0.0442955296083498:-3443.60130127503
zPost1000 Interior Blue C +:5949.57100959118:-0.0714879471150833:-2793.05359325281
zPost1000 Interior Blue C ++:7304.82502383449:0.0463350126264004:-862.724240632508
zPost1000 Interior Blue D:5995.68613611156:-0.18118662692994:-1247.21174794818
zPost1000 Interior Blue D +:6570.32228382172:-0.210610604591373:-930.90458210294
zPost1000 Interior Blue D ++:6954.10129662624:-0.287631649045466:-1177.24055019576
zPost1000 Interior Standard 1:5992.45880098842:-0.135299385735948:-800.977722938169
zPost1000 Interior Standard 1 +:5955.25636916045:-0.132973347771895:-437.626070025694
zPost1000 Interior Standard 1 ++:5935.65744287547:-0.135199335353889:-14.7803097593765
zPost1000 Interior Standard 2:5974.22916184723:-0.0955810570085873:-723.203866778874
zPost1000 Interior Standard 2 +:5917.54503794818:-0.0910152483417922:-351.084893335594
zPost1000 Interior Standard 2 ++:5874.85402231964:-0.0922442393012103:82.3830893530402
zPost1000 Interior Standard 3:5958.13175741953:-0.0542565084913102:-896.11759262703
zPost1000 Interior Standard 3 +:5894.82762383013:-0.0482919751332247:-520.73456877487
zPost1000 Interior Standard 3 ++:5819.00674823981:-0.0483369732226038:-80.8979601333815
zPost1000 Interior Standard 4:6550.51565659529:-0.0289442523946787:109.264054532627
zPost1000 Interior Standard 4 +:6557.11452641758:-0.0196001849135768:430.760806014916
zPost1000 Interior Standard 4 ++:6530.76569743508:-0.0157307958312458:898.115066261313
zPost1000 Interior Standard 5:6545.43041299219:-0.0639543164823522:523.576454885743
zPost1000 Interior Standard 5 +:7095.36104880635:-0.038583319751865:-63.6993712913114
zPost1000 Interior Standard 5 ++:7141.15279130065:-0.0133932731232562:-304.477133511246
zPost1000 Interior Standard 6:7877.11286946133:-0.112860734132545:612.018156479334
zPost1000 Interior Standard 6 +:7859.8381331331:-0.104081018148525:538.237778550837
zPost1000 Interior Standard 6 ++:7895.97526992001:-0.118900855232769:2578.60116568772
zPost1000 Interior Standard 7:7216.09020263847:-0.108476777441295:1232.20644764308
zPost1000 Interior Standard 7 +:7639.95524177069:-0.168460651266493:810.714883871377
zPost1000 Interior Standard 7 ++:6722.40545356556:-0.0927003312248723:1025.33850870732
zPost1000 Interior Standard 8:6246.44424103539:-0.107974785420311:916.85618975185
zPost1000 Interior Standard 8 +:6872.29520842419:-0.119545615237087:22.9894938181782
zPost1000 Interior Standard 8 ++:6690.19082141193:-0.139566582481677:853.832664873635
zPost1000 Interior Standard 9:7410.128604947:-0.132079126299573:-3.89025764554942
zPost1000 Interior Standard 9 +:7466.25477799302:-0.127961763890767:-245.232749833614
zPost1000 Interior Standard 9 ++:7113.31540114146:-0.149190031936996:1214.81042728781
zPost1000 Interior Standard A:7266.1753287531:-0.0826243746078035:1139.34374653293
zPost1000 Interior Standard A +:7367.0827546452:-0.0658528718536218:1369.93198981815
zPost1000 Interior Standard A ++:6878.38905766205:-0.0718888881672228:2185.64705744433
zPost1000 Interior Standard B:6136.54988189355:-0.159943502221793:1009.90587028874
zPost1000 Interior Standard B +:6620.62411464747:-0.153900244374436:980.048330976305
zPost1000 Interior Standard B ++:7500.47353585037:-0.184584675726342:743.418219681489
zPost1000 Interior Standard C:5802.11550353918:0.045224896538798:-2620.68760120791
zPost1000 Interior Standard C +:6891.18234045497:0.124981745085499:-3351.71248195535
zPost1000 Interior Standard C ++:7162.26320636503:0.197830497694554:-3508.57524782665
zPost1000 Interior Standard D:7002.57639335771:-0.184734701260326:-848.081091419173
zPost1000 Interior Standard D +:7339.06258962815:-0.138676089193752:-30.9072759243103
zPost1000 Interior Standard D ++:7819.01537291405:-0.242213717573794:372.759347352344
zPost1000 Interior Warming 1:5962.98502413067:-0.0109063867436703:-1058.30321503896
zPost1000 Interior Warming 1 +:5906.07413323209:-0.00408722373806218:-682.053526903667
zPost1000 Interior Warming 1 ++:5810.45128012976:-0.00340929101899488:-241.245677171427
zPost1000 Interior Warming 2:6600.47873505223:0.0350322299657935:-283.250978263558
zPost1000 Interior Warming 2 +:6613.68536866987:0.0470631299199867:42.5908694025032
zPost1000 Interior Warming 2 ++:6557.23207158017:0.0521493930659744:514.12813368486
zPost1000 Interior Warming 3:6606.71247939314:0.068859888514998:-479.460128725966
zPost1000 Interior Warming 3 +:6626.45879153501:0.0810133526604834:-154.648348162729
zPost1000 Interior Warming 3 ++:6573.80275170097:0.0869759849823616:319.447085160951
zPost1000 Interior Warming 4:6018.14810237355:0.0788191219125451:685.729422003795
zPost1000 Interior Warming 4 +:6851.5761826995:0.0852816670899301:-96.6941381446171
zPost1000 Interior Warming 4 ++:6087.03531530312:0.0981387822687907:372.096778822076
zPost1000 Interior Warming 5:6849.23192521294:-0.00956552721208936:767.121509746959
zPost1000 Interior Warming 5 +:7681.4798805037:-0.0277990045171919:29.5933933088921
zPost1000 Interior Warming 5 ++:7872.20225188709:-0.0454589493444772:2004.5815976551
zPost1000 Interior Warming 6:6844.44593553703:-0.00380524414799766:1235.80647785805
zPost1000 Interior Warming 6 +:7442.27927446065:-0.0314056499101313:701.586825831627
zPost1000 Interior Warming 6 ++:5767.0007931251:0.059217981161737:882.069298527285
zPost1000 Interior Warming 7:5650.04392446016:0.0355009167938647:1177.88629320137
zPost1000 Interior Warming 7 +:6070.52477496221:0.045383345140408:1223.80657637901
zPost1000 Interior Warming 7 ++:6112.00577544659:0.0320695432176876:1464.22751447998
zPost1000 Interior Warming 8:6597.61405064743:-0.0341150674743176:977.203914385124
zPost1000 Interior Warming 8 +:6880.98818415471:-0.0269727567224436:1231.74247238209
zPost1000 Interior Warming 8 ++:6719.74022348543:-0.00800159311268089:1622.78170699289
zPost1000 Interior Warming 9:6592.3076448651:0.0244036299261676:960.257520197614
zPost1000 Interior Warming 9 +:7001.60977783823:0.0471464176831091:1129.41702590658
zPost1000 Interior Warming 9 ++:6648.85070066192:0.0649849611565401:390.214393516603
zPost1000 Interior Warming A:4128.29731000406:0.11174938321011:2432.46806373634
zPost1000 Interior Warming A +:4698.70059221812:0.0964767123548427:2566.3451517369
zPost1000 Interior Warming A ++:4911.78188352166:0.117474721524843:2584.65100251992
zPost1000 Interior Warming B:4994.99406625739:0.0130619997858048:1568.10069372737
zPost1000 Interior Warming B +:6546.11274888518:-0.0331671837436988:778.576443561197
zPost1000 Interior Warming B ++:7479.81801367387:0.0494887401839605:579.014293129773
zPost1000 Interior Warming C:4727.44022004979:0.238106524852185:-2025.96601410966
zPost1000 Interior Warming C +:5714.2460385694:0.341150622794487:-1858.73729651478
zPost1000 Interior Warming C ++:6594.36712553707:0.373853976942371:-2958.08504285047
zPost1000 Interior Warming D:5647.02914976158:-0.0473795988474861:-239.473190132815
zPost1000 Interior Warming D +:6137.10743535273:-0.0714643852127544:425.645768468891
zPost1000 Interior Warming D ++:7276.75298121792:-0.0794342682647198:-586.583058388912
zPost1000 Light Tint 1:6450.36162602809:-0.0797061728684412:316.445738667893
zPost1000 Light Tint 1 +:6230.25404725588:-0.0752024958491688:622.588244040822
zPost1000 Light Tint 1 ++:6193.21943462052:-0.0699673546629356:910.901984452018
zPost1000 Light Tint 2:3980.08819460842:0.0401869818368823:2197.61708459055
zPost1000 Light Tint 2 +:3619.57000869358:0.035875266166272:2554.5975127893
zPost1000 Light Tint 2 ++:3483.55358338377:0.0316937594134856:2907.94133175804
zPost1000 Light Tint 3:3805.84495820361:0.0477074705756877:2246.53734918
zPost1000 Light Tint 3 +:3420.69089387556:0.0442631721223847:2598.43673346491
zPost1000 Light Tint 3 ++:3253.2634059003:0.0407508983809066:2952.63807204389
zPost1000 Light Tint 4:5692.88811250555:-0.0839528589323323:-957.396045205461
zPost1000 Light Tint 4 +:5642.58699099907:-0.0766951187001849:-787.257471685917
zPost1000 Light Tint 4 ++:5589.25612962327:-0.0644262519850393:-547.394718759379
zPost1000 Light Tint 5:6588.3039244267:-0.0900644036584735:-334.303311656116
zPost1000 Light Tint 5 +:6617.63925803056:-0.0779904853453104:-51.0222502794711
zPost1000 Light Tint 5 ++:6685.589222156:-0.0690152058622857:257.763380307957
zPost1000 Light Tint 6:5242.32845250042:-0.0165697092965918:1533.21835646716
zPost1000 Light Tint 6 +:5697.56807335606:-0.0705182897728608:1292.67979818802
zPost1000 Light Tint 6 ++:5649.33055233556:-0.0795999940594069:1382.89548928727
zPost1000 Light Tint 7:6193.80167689527:-0.0870672514295165:-52.3816745453502
zPost1000 Light Tint 7 +:6107.6104784741:-0.0771690243080343:234.338608333537
zPost1000 Light Tint 7 ++:7140.08650735587:-0.120691162356366:3626.29412031726
zPost1000 Light Tint 8:6596.1509844251:0.00704838140903921:1660.48495332768
zPost1000 Light Tint 8 +:6305.5688237896:0.0240106246211272:2110.96471689118
zPost1000 Light Tint 8 ++:6162.94278995025:0.0384999271686891:2470.02355508073
zPost1000 Light Tint 9:7540.40267123932:-0.0831811075410087:-745.576557162352
zPost1000 Light Tint 9 +:7500.23386650772:-0.0828263851464612:-510.209224728095
zPost1000 Light Tint 9 ++:7462.24236901454:-0.0830174703551214:-287.637971051025
zPost1000 Light Tint A:7038.05101507:-0.159132097293241:1118.87576226347
zPost1000 Light Tint A +:6956.07583598016:-0.152923944722356:1626.80780926893
zPost1000 Light Tint A ++:6934.67043218789:-0.14741690040961:2103.65750158224
zPost1000 Nature Shade 1:6372.00378244839:-0.09086261273316:582.113733880878
zPost1000 Nature Shade 1 +:6364.39129218543:-0.0849184463854846:887.163415175377
zPost1000 Nature Shade 1 ++:6426.7522745419:-0.0807831654950903:1157.53727312258
zPost1000 Nature Shade 2:6480.91297517772:0.0209847673582294:-1100.74507814706
zPost1000 Nature Shade 2 +:6190.143767974:0.0239243878713604:-575.147678654936
zPost1000 Nature Shade 2 ++:6114.18850436851:0.0270976030223426:-134.241793054021
zPost1000 Nature Shade 3:4958.51178211517:0.0322840072448685:538.716423819932
zPost1000 Nature Shade 3 +:4775.92113754984:0.0244212438609566:723.674618561141
zPost1000 Nature Shade 3 ++:4668.75977671201:0.0172673528268024:909.495552302623
zPost1000 Nature Shade 4:4925.41126346498:0.0278604228606445:2747.13340457793
zPost1000 Nature Shade 4 +:4712.66245560865:0.0146548855031462:3557.13866052277
zPost1000 Nature Shade 4 ++:4601.47176487544:0.00579208133716966:4249.92823596166
zPost1000 Nature Shade 5:4169.18236669335:-0.000378181560279245:3577.40531709085
zPost1000 Nature Shade 5 +:3963.63842765759:-0.00776606902985577:4105.35567295796
zPost1000 Nature Shade 5 ++:3856.84682507476:-0.0176781787148457:4576.08119272361
zPost1000 Nature Shade 6:7084.23707904108:0.00100863165516785:-4984.50856260273
zPost1000 Nature Shade 6 +:7028.35804993379:-0.00399481494935117:-3849.94120368789
zPost1000 Nature Shade 6 ++:6856.86257894581:-0.0195815205265149:-2698.11631820953
zPost1000 Nature Shade 7:6774.49275484805:-0.0803059946895246:-4748.88371374407
zPost1000 Nature Shade 7 +:6775.45350724107:-0.087582342133544:-3661.82745331132
zPost1000 Nature Shade 7 ++:6697.14507823953:-0.0976375865616887:-2637.42610860479
zPost1000 Nature Shade 8:6934.27202167548:-0.0789072108300957:-5497.39352473072
zPost1000 Nature Shade 8 +:6978.35001124217:-0.103766470602011:-4276.58040332842
zPost1000 Nature Shade 8 ++:7013.88732557089:-0.141610662106189:-3390.04575235704
zPost1000 Nature Shade 9:7543.79269636976:-0.0822969343256545:-3799.58962136759
zPost1000 Nature Shade 9 +:7501.2675108541:-0.083766258750411:-3587.55431539187
zPost1000 Nature Shade 9 ++:7447.41777407901:-0.0842301823573326:-3426.14384821152
zPost1000 Nature Shade A:7804.49354091379:-0.0925214463474686:-1799.82364971871
zPost1000 Nature Shade A +:7746.65337312427:-0.0907480306448178:-1730.85763391973
zPost1000 Nature Shade A ++:7686.46830851439:-0.0887985645783486:-1699.80403066349
zPost1000 Neutral Colors 1:6261.84641060458:-0.319406526550428:1873.1673464979
zPost1000 Neutral Colors 1 +:6158.95141973074:-0.349986719617755:2021.98174415173
zPost1000 Neutral Colors 1 ++:6090.54182106749:-0.363171152997552:2259.53338846072
zPost1000 Neutral Colors 2:6116.75461396017:-0.0693967362718695:2297.38300787409
zPost1000 Neutral Colors 2 +:6002.19777364904:-0.0721263242550094:3092.53336437084
zPost1000 Neutral Colors 2 ++:6035.79144037489:-0.072905659257799:4012.11509775272
zPost1000 Neutral Colors 3:7252.8456832163:-0.23979013938748:-490.332045190647
zPost1000 Neutral Colors 3 +:7238.16014053308:-0.241572365739273:-251.752942804239
zPost1000 Neutral Colors 3 ++:7289.04168241068:-0.246688787004075:-18.5463982913599
zPost1000 Neutral Colors 4:6615.87684960296:-0.125925825620584:701.837906668724
zPost1000 Neutral Colors 4 +:6472.84342954313:-0.134394633405805:1393.75819913712
zPost1000 Neutral Colors 4 ++:6421.20122800949:-0.141338280105401:2138.60528409959
zPost1000 Neutral Colors 5:5527.64264619726:0.0316914434070552:3805.60004807968
zPost1000 Neutral Colors 5 +:5291.7908161281:0.0365049133797331:4163.83928903805
zPost1000 Neutral Colors 5 ++:5199.55225783562:0.035723515770826:4507.00009882757
zPost1000 Neutral Colors 6:6306.03101274903:0.00313026321043708:-4844.74382555187
zPost1000 Neutral Colors 6 +:6259.60928501397:0.00885105918659641:-4555.70459747637
zPost1000 Neutral Colors 6 ++:6213.66012970188:0.0124358504200472:-4205.14641767197
zPost1000 Neutral Colors 7:7037.32435487551:0.00572772446491854:-2721.29156158109
zPost1000 Neutral Colors 7 +:6997.62517651447:0.0126457357389673:-2419.97520854232
zPost1000 Neutral Colors 7 ++:6953.89749059929:0.0169184597410776:-2063.17912047707
zPost1000 Neutral Colors 8:6728.95708888458:-0.000128313547818707:-1140.62225230021
zPost1000 Neutral Colors 8 +:6706.63104907926:0.00974790833805628:-1111.92942925226
zPost1000 Neutral Colors 8 ++:6655.01920502504:0.0162099751564142:-1092.50196496953
zPost1000 Neutral Colors 9:6195.22655529649:0.0144181759255275:-304.922876444794
zPost1000 Neutral Colors 9 +:6117.16564675893:0.0154210023664993:154.696751018836
zPost1000 Neutral Colors 9 ++:6105.96283329152:0.0158854375880231:577.138594504487
zPost1000 Neutral Colors A:5393.46939160608:-0.136088293867374:1498.55298437787
zPost1000 Neutral Colors A +:5180.93381974376:-0.137008818065762:1801.55039241915
zPost1000 Neutral Colors A ++:5112.67146385857:-0.13603276430149:2068.02884346647
zPost1000 Paradise Reverie 1:6650.51732867415:-0.0905403159235863:-503.930716813085
zPost1000 Paradise Reverie 1 +:6495.89688921771:-0.0934358899584039:-276.007822221738
zPost1000 Paradise Reverie 1 ++:6472.32157838587:-0.0976524250075386:-55.2901650909792
zPost1000 Paradise Reverie 2:7886.42335016448:-0.158171533033169:2291.70051128648
zPost1000 Paradise Reverie 2 +:7836.92847644301:-0.154589846163162:3010.41686376545
zPost1000 Paradise Reverie 2 ++:7792.07152427322:-0.152451940334117:3782.28877415023
zPost1000 Paradise Reverie 3:7744.56797628549:-0.169914218664701:2226.9842184653
zPost1000 Paradise Reverie 3 +:7739.34875970678:-0.167510630724223:2628.09403759048
zPost1000 Paradise Reverie 3 ++:7760.02928307261:-0.165035715244585:2907.02215891063
zPost1000 Paradise Reverie 4:6147.44911935952:-0.00337488659710061:1220.71636181682
zPost1000 Paradise Reverie 4 +:6037.26497401985:-0.00796520876542672:1408.46016720454
zPost1000 Paradise Reverie 4 ++:5971.4945500535:-0.0137551707400689:1564.83534254422
zPost1000 Paradise Reverie 5:7890.16879129969:-0.0572423613072601:-816.742598863188
zPost1000 Paradise Reverie 5 +:7845.21491295434:-0.0607680392969314:-623.359581017216
zPost1000 Paradise Reverie 5 ++:7799.02394958123:-0.0631663618535185:-386.329501533712
zPost1000 Paradise Reverie 6:6795.69969317694:-0.0228568317703548:-1440.7686439874
zPost1000 Paradise Reverie 6 +:6725.39530136793:-0.0215509184954499:-1179.66405403303
zPost1000 Paradise Reverie 6 ++:6740.54967251223:-0.020593982895889:-920.199995097693
zPost1000 Paradise Reverie 7:7523.80521543615:-0.0765309093493443:-286.679790901017
zPost1000 Paradise Reverie 7 +:7397.41875522221:-0.082249544370427:36.6287266251947
zPost1000 Paradise Reverie 7 ++:7316.41847772846:-0.0884535879822579:392.771225697329
zPost1000 Paradise Reverie 8:7727.16796120163:-0.135614377651677:-3656.92817819065
zPost1000 Paradise Reverie 8 +:7762.25869535934:-0.130376364288452:-3872.48373434856
zPost1000 Paradise Reverie 8 ++:7774.84315479209:-0.124233115733034:-4138.83406395779
zPost1000 Paradise Reverie 9:6550.20795367884:-0.103367228477225:1824.21430116401
zPost1000 Paradise Reverie 9 +:6521.31411635681:-0.0678361503549354:2228.37771771045
zPost1000 Paradise Reverie 9 ++:6420.14763812276:-0.027619028997614:2630.86924123236
zPost1000 Paradise Reverie A:6961.26814529619:-0.00840705770099248:3046.6888319114
zPost1000 Paradise Reverie A +:6936.7977703833:0.00922133178818285:4364.15553715097
zPost1000 Paradise Reverie A ++:6980.3379603951:0.0273882657920126:845.813036256038
zPost1000 Photographic Lens 1:5121.05964869391:-0.00616458625900054:-3311.24703844892
zPost1000 Photographic Lens 1 +:4897.36220777082:-0.00867523227051626:-3032.7411457439
zPost1000 Photographic Lens 1 ++:4705.25130593447:-0.0071858728201164:-2697.11369322967
zPost1000 Photographic Lens 2:6026.78028282256:-0.0150826269430127:-3203.48101688104
zPost1000 Photographic Lens 2 +:5813.76392709383:-0.0101527038180184:-2967.52545492493
zPost1000 Photographic Lens 2 ++:5637.24974763422:-0.00618585467600496:-2654.03787411588
zPost1000 Photographic Lens 3:6540.10388890027:-0.0289766268317635:1254.75036084571
zPost1000 Photographic Lens 3 +:6383.99525992777:-0.0326842133080731:1529.45495635116
zPost1000 Photographic Lens 3 ++:6272.52241986176:-0.0363171211655526:1785.65104783106
zPost1000 Photographic Lens 4:6920.81549370699:0.0736902487509497:251.915041120564
zPost1000 Photographic Lens 4 +:6841.68139872313:0.0796037179413815:519.94642844519
zPost1000 Photographic Lens 4 ++:6804.36790423032:0.084337652769193:780.971279490742
zPost1000 Photographic Lens 5:5399.62578787938:0.143538408235713:3558.96077452194
zPost1000 Photographic Lens 5 +:5213.18969026196:0.152877504711569:4174.66014658413
zPost1000 Photographic Lens 5 ++:5040.37901925949:0.155796180908908:4764.5555998775
zPost1000 Photographic Lens 6:7107.27827864065:0.000616166620758493:-4481.2915834265
zPost1000 Photographic Lens 6 +:6903.30014466408:-0.0109896837616361:-3294.9765164257
zPost1000 Photographic Lens 6 ++:6742.86394510961:-0.0296678791002591:-2028.71766151377
zPost1000 Photographic Lens 7:6970.61910216007:0.00139781997899036:-4379.88633110764
zPost1000 Photographic Lens 7 +:6968.01776064806:0.0163176846115789:-4266.28501908824
zPost1000 Photographic Lens 7 ++:6950.92261096774:0.0286616260631263:-4128.44580341005
zPost1000 Photographic Lens 8:7058.68564236853:-0.0113562241070735:-1676.41960887802
zPost1000 Photographic Lens 8 +:6905.3341476625:-0.0122520346345141:-1502.03266752753
zPost1000 Photographic Lens 8 ++:6725.1623220458:-0.0153779922066519:-1248.04572411225
zPost1000 Photographic Lens 9:6171.85306955663:-0.0489420417311314:-2933.94123163382
zPost1000 Photographic Lens 9 +:6274.2363339851:-0.0458199333106677:-2902.12945040336
zPost1000 Photographic Lens 9 ++:6317.17098933917:-0.0457294585772274:-2864.73858092491
zPost1000 Photographic Lens A:6945.63623006503:-0.0723460519483297:-995.717697065294
zPost1000 Photographic Lens A +:7598.52987814985:-0.043671954383763:-2046.30593046521
zPost1000 Photographic Lens A ++:7716.41560110322:-0.0144557593237742:-2472.22488045657
zPost1000 Ragged Shade 1:5963.15890503029:-0.00173196356490735:-2219.30851123341
zPost1000 Ragged Shade 1 +:5821.30194124661:0.00345908010950622:-2339.09542668595
zPost1000 Ragged Shade 1 ++:5782.43393957727:0.00401730916991028:-2170.39083697697
zPost1000 Ragged Shade 2:6148.31283026409:-0.179266991478194:-5324.08760134208
zPost1000 Ragged Shade 2 +:6094.71510478658:-0.182720008688162:-4974.03138016455
zPost1000 Ragged Shade 2 ++:5993.35403776047:-0.18575468694236:-4586.18954360913
zPost1000 Ragged Shade 3:3846.09441660794:-0.0485228154603665:3430.96086777364
zPost1000 Ragged Shade 3 +:3406.02512498574:-0.0805182680512733:4449.09945651429
zPost1000 Ragged Shade 3 ++:3078.85761609462:-0.103124922304406:5547.2158162724
zPost1000 Ragged Shade 4:7458.88920264999:-0.0873394227010154:-2490.44448457347
zPost1000 Ragged Shade 4 +:7570.84483493345:-0.0813498138778721:-2071.87715388415
zPost1000 Ragged Shade 4 ++:7592.49426502067:-0.0817827416973093:-1699.98694599731
zPost1000 Ragged Shade 5:6864.98566647154:0.0721099379709529:-6119.33081101019
zPost1000 Ragged Shade 5 +:6776.76849892342:0.0729026746454957:-6077.62753389534
zPost1000 Ragged Shade 5 ++:6675.88466680313:0.0721403939060266:-5968.73863348238
zPost1000 Ragged Shade 6:3568.69322448608:0.15844746011815:649.145141993501
zPost1000 Ragged Shade 6 +:3321.96990005534:0.160514704350338:680.807902183873
zPost1000 Ragged Shade 6 ++:3246.63213015618:0.1608417599615:733.237977463879
zPost1000 Ragged Shade 7:6547.90660563194:-0.0468365435290334:2589.35423313659
zPost1000 Ragged Shade 7 +:6455.14780349464:-0.0408943080797363:3161.06667834755
zPost1000 Ragged Shade 7 ++:6487.23229943492:-0.0333867440310959:3722.59741637315
zPost1000 Ragged Shade 8:5767.56423744407:0.125932575400124:1541.70721698148
zPost1000 Ragged Shade 8 +:5425.47650038934:0.138674442802216:1799.84318581264
zPost1000 Ragged Shade 8 ++:5301.83981360882:0.142655198465604:2107.4824159489
zPost1000 Ragged Shade 9:5907.94890340343:0.0835549742383819:-4910.20399031084
zPost1000 Ragged Shade 9 +:5591.67760427722:0.0798473885088811:-5173.15953399145
zPost1000 Ragged Shade 9 ++:5359.12298306882:0.0742106125627515:-5310.50724080572
zPost1000 Ragged Shade A:4284.49378574892:0.019409635264159:-1678.69404179657
zPost1000 Ragged Shade A +:4284.93377161986:0.020346010875528:-1741.8234456735
zPost1000 Ragged Shade A ++:4317.45384768096:0.0206107362201351:-1777.1175600577
zPost1000 Ragged Shade Red Light District:5438.35853972781:0.366343741728764:-1087.44314699789
zPost1000 Ragged Shade Red Light District +:5414.43993638548:0.358224191237923:-1707.47843762382
zPost1000 Ragged Shade Red Light District ++:5430.4656953635:0.389921782759075:-1316.99999771687
zPost1000 Retro 1:7432.64744166931:-0.0230066645893316:2621.3881501031
zPost1000 Retro 1 +:7353.85028911632:-0.0161630674934372:3179.59601364903
zPost1000 Retro 1 ++:7305.86388926917:-0.0127115893876635:3800.06401149965
zPost1000 Retro 2:4594.15418920675:-0.103161678109568:-814.824784287889
zPost1000 Retro 2 +:4436.63341010754:-0.102702395200385:-621.355514955809
zPost1000 Retro 2 ++:4436.46054054116:-0.101373892336426:-351.376448398677
zPost1000 Retro 3:4592.87303634704:0.0733513307048383:4536.04451905591
zPost1000 Retro 3 +:4327.0511397085:0.0667188426481857:5165.17396477572
zPost1000 Retro 3 ++:4175.96657748996:0.0542963928286895:5830.48903207125
zPost1000 Retro 4:5079.75115853523:0.164678593173449:632.198731792817
zPost1000 Retro 4 +:4622.64450305285:0.171964376664098:828.772605731915
zPost1000 Retro 4 ++:4307.46986181669:0.174266353108277:1039.27458665247
zPost1000 Retro 5:7023.77124527254:-0.0263394255600247:-2612.23792234254
zPost1000 Retro 5 +:6926.04731121543:-0.0308100714597686:-2278.77515684445
zPost1000 Retro 5 ++:6825.06120222909:-0.039626420327977:-1854.72154491341
zPost1000 Retro 6:5589.1210388284:-0.0218797902909955:1447.85495322855
zPost1000 Retro 6 +:5473.80876667454:-0.0237319075820892:1959.00399156417
zPost1000 Retro 6 ++:5367.90412158433:-0.0299297289230367:2549.05629524336
zPost1000 Retro 7:5612.37918391931:-0.0535869493924166:1063.38432691883
zPost1000 Retro 7 +:5692.33447717856:-0.048782537319187:1381.90068760734
zPost1000 Retro 7 ++:5663.17646688251:-0.0408049083359753:1770.3569135158
zPost1000 Retro 8:7576.27827503215:0.017444562181538:-2012.80529728108
zPost1000 Retro 8 +:7472.40066796782:0.0247812273497061:-1879.44188839298
zPost1000 Retro 8 ++:7343.57905735273:0.0303226124771333:-1739.06541042888
zPost1000 Retro 9:5364.76273258427:-0.0160606276519957:-486.720235142218
zPost1000 Retro 9 +:5157.00824386303:-0.00829892565509114:160.082404894066
zPost1000 Retro 9 ++:5364.76273258427:-0.0160606276519957:-486.720235142218
zPost1000 Retro A:5463.77913073344:0.0348646631194891:-4362.49284968302
zPost1000 Retro A +:5190.03014157379:0.0409825937057917:-4820.25363372929
zPost1000 Retro A ++:6588.74590973366:0.0168610182906832:-4143.12945723722
zPost1000 Science Fiction 1:5037.91088899363:-0.0986175671136938:1498.8366816195
zPost1000 Science Fiction 1 +:4959.14051881137:-0.0942311036397427:1921.6903042251
zPost1000 Science Fiction 1 ++:4953.4164776721:-0.0834052707419389:2327.86417727631
zPost1000 Science Fiction 2:7256.60513753109:-0.0818249359617198:-1980.68368818347
zPost1000 Science Fiction 2 +:7100.93935136521:-0.0723599388617799:-1871.75949285697
zPost1000 Science Fiction 2 ++:6936.24005684706:-0.0653376216960846:-1764.47320579431
zPost1000 Science Fiction 3:7379.36979683511:-0.041383279327647:-2880.29022797846
zPost1000 Science Fiction 3 +:7345.15579685094:-0.0269708441459215:-2650.14904666971
zPost1000 Science Fiction 3 ++:7299.93719173868:-0.0183316163251561:-2463.57911209583
zPost1000 Science Fiction 4:6333.33489526813:0.021815155125239:2855.03921560007
zPost1000 Science Fiction 4 +:6107.15272092987:0.0328493537970189:3492.21702431396
zPost1000 Science Fiction 4 ++:5953.27456144281:0.050072535301271:4093.43273818873
zPost1000 Science Fiction 5:4971.03300199228:-0.0139708625712797:3208.70331213934
zPost1000 Science Fiction 5 +:4863.29891687184:-0.0059884047054301:3789.46020553047
zPost1000 Science Fiction 5 ++:5556.80933341676:0.00795707959078453:54.1320621997971
zPost1000 Science Fiction 6:7680.76007467321:-0.0979812369696233:-117.730817301674
zPost1000 Science Fiction 6 +:7651.64205441534:-0.103057349510452:-61.4732907622345
zPost1000 Science Fiction 6 ++:7588.59251760799:-0.106535230430804:212.635222371369
zPost1000 Science Fiction 7:5151.40102203219:0.138161568108274:4634.67848017747
zPost1000 Science Fiction 7 +:5047.39928318525:0.136604732213167:5405.50204361928
zPost1000 Science Fiction 7 ++:4962.24656327273:0.128879914013339:6138.19620065499
zPost1000 Science Fiction 8:6890.94740871861:-0.253918460846887:-493.924239793828
zPost1000 Science Fiction 8 +:6800.07024502465:-0.261142957572494:-402.380429814275
zPost1000 Science Fiction 8 ++:6784.00854215106:-0.268043433181788:-319.143813600317
zPost1000 Science Fiction 9:3881.02200501262:-0.028399415626306:-5431.00916056229
zPost1000 Science Fiction 9 +:3934.17675802345:-0.0316555006570542:-5501.85753172732
zPost1000 Science Fiction 9 ++:4021.7372204834:-0.0344238295510308:-5505.44316122292
zPost1000 Science Fiction A:6092.97745458547:-0.12766938166828:1727.70454939614
zPost1000 Science Fiction A +:6227.94892607944:-0.128226200342736:2193.32286456206
zPost1000 Science Fiction A ++:6339.08314412444:-0.128592122190677:2701.28235836896
zPost1000 Sea Cruise 1:5764.4149808003:-0.0976021579317035:5081.2078665389
zPost1000 Sea Cruise 1 +:5592.22816601144:-0.105743530300913:6144.0797868207
zPost1000 Sea Cruise 1 ++:5472.3960160094:-0.100256109968598:6995.44189552684
zPost1000 Sea Cruise 2:4389.66477257521:-0.0636643309050885:3252.30075685044
zPost1000 Sea Cruise 2 +:4241.21425294211:-0.0749435691576132:4130.17398916167
zPost1000 Sea Cruise 2 ++:4116.87197703206:-0.0810526171292686:5023.25768513957
zPost1000 Sea Cruise 3:6767.65649571348:-0.150050875621774:1163.41175572384
zPost1000 Sea Cruise 3 +:6711.8044884089:-0.151152036321682:1398.17256249157
zPost1000 Sea Cruise 3 ++:6651.44244254203:-0.156129282591129:1642.39219200224
zPost1000 Sea Cruise 4:7203.45144058947:-0.0590145375883822:294.215638597635
zPost1000 Sea Cruise 4 +:7110.80174469065:-0.0646891314852951:806.820881492385
zPost1000 Sea Cruise 4 ++:7085.10183980922:-0.0703670767389895:1290.61567079234
zPost1000 Sea Cruise 5:7800.45314313662:-0.0759814132810401:-558.462862384051
zPost1000 Sea Cruise 5 +:7834.72404112512:-0.067268478436942:-185.146511442192
zPost1000 Sea Cruise 5 ++:7852.71265551674:-0.0607606178394492:262.590760372505
zPost1000 Sea Cruise 6:4250.82570244733:0.0228988772220414:256.083702578964
zPost1000 Sea Cruise 6 +:3697.85336668056:0.0108824491007908:-165.151550330963
zPost1000 Sea Cruise 6 ++:4985.75289087766:0.0138916383782272:477.858894023878
zPost1000 Sea Cruise 7:6549.24509008581:-0.00305032459942822:-2416.56732547637
zPost1000 Sea Cruise 7 +:4489.13928027111:-0.0297783523546535:-917.491433944968
zPost1000 Sea Cruise 7 ++:6440.88432229596:0.00654105192194265:-2386.41437442199
zPost1000 Sea Cruise 8:7401.48175830785:0.00666072860781242:-5428.62621801374
zPost1000 Sea Cruise 8 +:7380.12229099471:0.0169387241255139:-5546.71118081542
zPost1000 Sea Cruise 8 ++:7345.19877164529:0.0253338794050116:-5666.53993845179
zPost1000 Sea Cruise 9:7208.23314082415:0.043602078162337:-6467.5464855777
zPost1000 Sea Cruise 9 +:6800.50311904967:0.0415364560879539:-3899.32814352501
zPost1000 Sea Cruise 9 ++:7179.3800413245:0.0543026185571875:-6581.15955686539
zPost1000 Sea Cruise A:6555.2309939589:-0.121141626443602:-2133.5699438828
zPost1000 Sea Cruise A +:6432.74427939204:-0.121148328890797:-2183.30195065515
zPost1000 Sea Cruise A ++:6319.22632441603:-0.122732849647662:-2277.69931990377
zPost1000 Secret Lab 1:3461.86582718002:-0.0776557748256784:-3017.6823487963
zPost1000 Secret Lab 1 +:3204.41927539492:-0.0850332245203731:-3613.13332549159
zPost1000 Secret Lab 1 ++:3082.02514225371:-0.0898992104092511:-3955.07450335998
zPost1000 Secret Lab 2:7221.35172041177:-0.0446865197763807:-2177.99875548586
zPost1000 Secret Lab 2 +:6581.05353844559:-0.0961478489355159:257.422196830316
zPost1000 Secret Lab 2 ++:6768.16779439427:-0.0703981916776684:-1156.73989684271
zPost1000 Secret Lab 3:4748.83867670561:0.0432162366414559:1638.55691526269
zPost1000 Secret Lab 3 +:4515.482607493:0.0501536606242325:1847.96870222392
zPost1000 Secret Lab 3 ++:4342.15702820283:0.0520115225587006:2027.01970309384
zPost1000 Secret Lab 4:7421.02195747689:0.027635755643928:-4453.39617510585
zPost1000 Secret Lab 4 +:7350.19872657136:0.0349494428357389:-4248.41707396319
zPost1000 Secret Lab 4 ++:7233.50774176042:0.0410582243989097:-3930.15953994814
zPost1000 Secret Lab 5:7005.8743907321:-0.132533418495655:-4851.64188862487
zPost1000 Secret Lab 5 +:6955.25261484456:-0.141023767239574:-4929.70462432817
zPost1000 Secret Lab 5 ++:6888.09417572915:-0.146420232084211:-4982.88762459164
zPost1000 Secret Lab 6:6708.6336022812:-0.110010754635237:-1164.0248752633
zPost1000 Secret Lab 6 +:6582.76166543268:-0.113752304246304:-769.423545159454
zPost1000 Secret Lab 6 ++:6451.95834228821:-0.11620645990595:-367.577773335392
zPost1000 Secret Lab 7:5524.0748523785:-0.229737807747873:-2383.98171899045
zPost1000 Secret Lab 7 +:5593.73272727604:-0.235984046000134:-2526.23922461865
zPost1000 Secret Lab 7 ++:5698.66538821359:-0.242494364339388:-2627.4169925972
zPost1000 Secret Lab 8:6179.32869952766:-0.139342548298714:-4363.12159292074
zPost1000 Secret Lab 8 +:6297.4797541835:-0.138548851748055:-4369.66799194897
zPost1000 Secret Lab 8 ++:6245.19929765592:-0.136692904542031:-4296.52942419191
zPost1000 Secret Lab 9:7031.98791134897:-0.206978970803123:-4034.94251525057
zPost1000 Secret Lab 9 +:6917.43786064138:-0.199923412473083:-5307.07193097812
zPost1000 Secret Lab 9 ++:6985.91732869422:-0.178094367254914:-5187.90462834207
zPost1000 Secret Lab A:4468.75219865655:0.0421578198589714:-2313.04310436473
zPost1000 Secret Lab A +:5332.53635794626:0.0206284443436289:-1868.07448924224
zPost1000 Secret Lab A ++:6018.83183378376:0.0992648523506002:-1697.51113591919
zPost1000 SunSet 1:6677.03299597947:0.12622471161065:-822.381741705639
zPost1000 SunSet 1 +:6437.52627821241:0.13697434499881:-485.81047733932
zPost1000 SunSet 1 ++:6330.66246345716:0.142834712306978:-160.966018901578
zPost1000 SunSet 2:6718.4270274638:-0.00735964660236732:-948.555570805274
zPost1000 SunSet 2 +:6699.94341720147:-0.00782351381462831:-589.784310843465
zPost1000 SunSet 2 ++:6695.29355910382:-0.0100832842704841:-219.135937385306
zPost1000 SunSet 3:5937.4698073755:-0.0415685355862365:-1836.45225348561
zPost1000 SunSet 3 +:5905.64287581266:-0.0457069583144403:-1575.56335485034
zPost1000 SunSet 3 ++:5880.3047581801:-0.043158321688054:-1304.78557885203
zPost1000 SunSet 4:7489.58199675158:-0.152259020110762:-1073.59357427432
zPost1000 SunSet 4 +:7482.31677833992:-0.141670018627963:-968.385721019794
zPost1000 SunSet 4 ++:7473.0138397627:-0.133816349930555:-797.76479772458
zPost1000 SunSet 5:7927.88516051152:-0.1211847091663:1144.26654850986
zPost1000 SunSet 5 +:7914.51284049428:-0.111506895500084:1350.38681347774
zPost1000 SunSet 5 ++:7896.67045050937:-0.114402469014323:1727.75806400838
zPost1000 SunSet 6:7696.31105693826:-0.0937313367358001:922.598585307446
zPost1000 SunSet 6 +:7769.94955196449:-0.076203955081553:1008.39859318966
zPost1000 SunSet 6 ++:7805.20075738832:-0.0713307169782524:1232.62605735581
zPost1000 SunSet 7:6847.1654510692:0.18872740199663:2271.08360243001
zPost1000 SunSet 7 +:6751.65800192687:0.20748506126921:2711.62087229599
zPost1000 SunSet 7 ++:6829.93906349453:0.220768704895079:3037.49146933079
zPost1000 SunSet 8:6593.71298560267:0.248614616878967:1743.49017218858
zPost1000 SunSet 8 +:6452.34204379249:0.282112625905256:1982.00390866352
zPost1000 SunSet 8 ++:6393.88235621262:0.315032356686061:2112.54770405367
zPost1000 SunSet 9:7192.47577469654:0.0764035638171663:-3781.25890166081
zPost1000 SunSet 9 +:7074.87645345523:0.0808114984532153:-3635.22097704471
zPost1000 SunSet 9 ++:6915.21819496389:0.0841294177457712:-3409.80789513062
zPost1000 SunSet A:7361.74210704321:0.345258528101435:-4653.15697605084
zPost1000 SunSet A +:7163.33673026978:0.363345663720085:-4353.6994518578
zPost1000 SunSet A ++:6913.51584648462:0.381404935166794:-3969.86937999034
zPost1000 Swamp 1:6955.06124973958:-0.146324160649175:329.050935833854
zPost1000 Swamp 1 +:6782.54306463008:-0.14646300262866:688.194747261725
zPost1000 Swamp 1 ++:6748.31730651326:-0.145475741929658:1060.09933981308
zPost1000 Swamp 2:5739.38307918273:-0.0284352034738049:-2375.7267477153
zPost1000 Swamp 2 +:5570.46366467451:-0.0413976347896892:-2231.17247206258
zPost1000 Swamp 2 ++:5450.98086779851:-0.0546989911393894:-2065.86132227011
zPost1000 Swamp 3:3883.2564254785:0.147815181342082:531.85366896507
zPost1000 Swamp 3 +:3337.80895018784:0.148029643092571:822.339978388689
zPost1000 Swamp 3 ++:2953.42694063612:0.146707934051278:1147.84197590579
zPost1000 Swamp 4:7455.64986168127:-0.162883035563916:625.218204400076
zPost1000 Swamp 4 +:7330.19807893742:-0.163824262178623:998.789464716523
zPost1000 Swamp 4 ++:7248.37924944405:-0.164368751738027:1431.2613789172
zPost1000 Swamp 5:6758.26764211797:-0.0104741158244224:991.299351654211
zPost1000 Swamp 5 +:6602.35900899319:-0.01717840956087:1253.0483957169
zPost1000 Swamp 5 ++:6489.8022395326:-0.0266783524484708:1525.6967865735
zPost1000 Swamp 6:7411.53941154703:0.0327659234523396:-2670.47066821547
zPost1000 Swamp 6 +:7411.04985675712:0.0446958232652828:-2506.59483937936
zPost1000 Swamp 6 ++:7373.18685570958:0.0499237526724136:-2256.55430152213
zPost1000 Swamp 7:7130.96641104042:0.0110260564707119:-1425.66801878973
zPost1000 Swamp 7 +:6955.20434766092:0.00938925556952227:-1119.26146491476
zPost1000 Swamp 7 ++:6768.95496870768:0.00412551814081024:-701.852291877068
zPost1000 Swamp 8:6908.8059428641:-0.0539841722753756:-1737.84316616442
zPost1000 Swamp 8 +:6933.02290862105:-0.0572627593536681:-1304.65621313439
zPost1000 Swamp 8 ++:6928.28274357108:-0.0686789365295498:-833.351612539819
zPost1000 Swamp 9:7579.37164031731:-0.0281475067985184:-1165.38829325453
zPost1000 Swamp 9 +:7577.99253591643:-0.0297521334961743:-706.564739901286
zPost1000 Swamp 9 ++:7547.97483601031:-0.0377977641171583:-206.65752645848
zPost1000 Swamp A:6922.52713821934:-0.116996978382019:-3849.88370614867
zPost1000 Swamp A +:6788.77541038721:-0.12709147540761:-3842.94251715463
zPost1000 Swamp A ++:6614.94373874267:-0.137024648282875:-3693.66014767383
zPost1000 Warm Day 1:7505.2990921582:0.105393950687267:2762.63132922227
zPost1000 Warm Day 1 +:7411.31559788863:0.119924344308788:3104.35160554799
zPost1000 Warm Day 1 ++:7382.18728729573:0.131221497361301:3534.98822704155
zPost1000 Warm Day 2:6006.31248205933:-0.0786157020725498:2520.53467114611
zPost1000 Warm Day 2 +:5995.56375635388:-0.0659533974074407:2555.16606226457
zPost1000 Warm Day 2 ++:6090.37749804267:-0.0561418161739198:2638.07326940948
zPost1000 Warm Day 3:7509.76222923875:0.278181964259034:2165.79734248607
zPost1000 Warm Day 3 +:7453.94594607248:0.291732247879452:2719.52938488697
zPost1000 Warm Day 3 ++:7489.83249713413:0.299000936839093:3213.4588127348
zPost1000 Warm Day 4:5039.44592322215:0.153744443170297:2229.90980237998
zPost1000 Warm Day 4 +:4923.42013029197:0.156689927094703:2382.10066921994
zPost1000 Warm Day 4 ++:4822.34406069305:0.154833979064847:2450.40526392934
zPost1000 Warm Day 5:6026.02955852124:0.144360346189785:3742.70849490659
zPost1000 Warm Day 5 +:5965.64693402509:0.143614998861267:4554.36472512189
zPost1000 Warm Day 5 ++:5878.107324805:0.144896976745031:5442.60919169175
zPost1000 Warm Day 6:5385.1029259785:0.120298131388552:4552.53206339638
zPost1000 Warm Day 6 +:5491.52193999691:0.119322047150092:5096.39613256205
zPost1000 Warm Day 6 ++:5562.33796862647:0.118975462994399:5695.36085439999
zPost1000 Warm Day 7:4379.41664112498:0.195400577237365:3340.81703964125
zPost1000 Warm Day 7 +:4400.30280560115:0.197554758569254:3688.80921024598
zPost1000 Warm Day 7 ++:4425.85599370804:0.198604083609212:4128.42360150017
zPost1000 Warm Day 8:4956.42841898675:0.215508190912033:4468.67322558019
zPost1000 Warm Day 8 +:4946.02244230191:0.212249630379119:5137.26016967776
zPost1000 Warm Day 8 ++:4969.41185260108:0.202602244509762:5951.52031791892
zPost1000 Warm Day 9:4775.02045240741:0.122097596567812:572.11371874497
zPost1000 Warm Day 9 +:4586.87822059207:0.137915994048399:844.572564325788
zPost1000 Warm Day 9 ++:4433.58260108776:0.150290572818562:1121.97258601369
zPost1000 Warm Day A:5968.45571857798:0.0681894272105978:4429.02483242647
zPost1000 Warm Day A +:6093.03252122644:0.0624353660875272:4914.48498246714
zPost1000 Warm Day A ++:6235.06981936893:0.0567234312723599:5369.55820027299
zPost1000 Wasteland 1:5884.97387350207:-0.020606073552756:1197.87192559258
zPost1000 Wasteland 1 +:5775.23703308739:-0.00944585206701731:1616.08501939358
zPost1000 Wasteland 1 ++:5779.28215845614:0.00160510181257223:2016.19091821493
zPost1000 Wasteland 2:5916.30142866922:-0.0504054692676164:2206.23923155039
zPost1000 Wasteland 2 +:5767.46595079098:-0.0459807790097102:2516.2850053976
zPost1000 Wasteland 2 ++:5728.68651913445:-0.0414970076075905:2823.96083475526
zPost1000 Wasteland 3:7676.22627609065:-0.0878735866935765:-1191.65896160681
zPost1000 Wasteland 3 +:7654.51646070177:-0.090967848425634:-880.801436508175
zPost1000 Wasteland 3 ++:7647.29423032859:-0.100437830960893:-650.788351118442
zPost1000 Wasteland 4:7290.96622481761:0.0948488087950545:-3112.6314670479
zPost1000 Wasteland 4 +:7248.69622495813:0.100035899956652:-2754.8458244595
zPost1000 Wasteland 4 ++:7195.24765801638:0.101160860907896:-2333.62488860977
zPost1000 Wasteland 5:6376.04928094903:-0.103207014609906:-912.091561731235
zPost1000 Wasteland 5 +:6486.7897946208:-0.0968893648183666:-652.000046257593
zPost1000 Wasteland 5 ++:6596.45821510815:-0.0931214613586917:-348.511146849563
zPost1000 Wasteland 6:7536.08443299666:-0.0901553954766428:-1795.22622768232
zPost1000 Wasteland 6 +:7488.31403285698:-0.0842261293798288:-1632.64713154684
zPost1000 Wasteland 6 ++:7436.48081861197:-0.0807535879645229:-1495.48895004486
zPost1000 Wasteland 7:5624.63971125371:0.189894212938562:1102.79083139306
zPost1000 Wasteland 7 +:5477.41174001649:0.20027480679708:1705.40131754331
zPost1000 Wasteland 7 ++:5267.67717693194:0.20160222804077:2319.99350386225
zPost1000 Wasteland 8:4362.7264584871:-0.0528159797180364:2715.72686406989
zPost1000 Wasteland 8 +:3875.74409547636:-0.0637969472308538:3165.48377388335
zPost1000 Wasteland 8 ++:3606.0576425076:-0.0734062510546096:3581.88643465331
zPost1000 Wasteland 9:6889.71240949785:-0.127563484102154:-1763.2143456884
zPost1000 Wasteland 9 +:6593.75063911801:-0.129912022405279:-1038.1097376221
zPost1000 Wasteland 9 ++:7105.065292173:-0.0858791072524241:-1459.24440127283
zPost1000 Wasteland A:6861.1301180669:-0.140692979711616:3214.00384073299
zPost1000 Wasteland A +:6789.97963920368:-0.145285217431194:3966.45745462123
zPost1000 Wasteland A ++:6802.48464845776:-0.148924348795276:4902.10192111228
zPost1000 White Diffusion 1:6258.6203455732:-0.0219173578153768:7230.30451946609
zPost1000 White Diffusion 1 +:6187.65740205972:-0.0138572423790422:7573.55811123074
zPost1000 White Diffusion 1 ++:6259.89635935185:-0.00946830116903622:7663.59602039716
zPost1000 White Diffusion 2:5559.81964399674:-0.0711767845105917:5270.58037948876
zPost1000 White Diffusion 2 +:5784.95896687931:-0.0616917885418156:5748.29542757309
zPost1000 White Diffusion 2 ++:5931.96549369804:-0.0512226040551461:6037.78303002566
zPost1000 White Diffusion 3:5295.98851853509:-0.0270077010348473:6262.79997494293
zPost1000 White Diffusion 3 +:5849.41490280499:-0.0588857661563225:6073.62611256716
zPost1000 White Diffusion 3 ++:5547.97789320067:-0.042756208402011:4451.64312448991
zPost1000 White Diffusion 4:5940.59673868086:-0.0636548979997384:6668.41302575696
zPost1000 White Diffusion 4 +:6019.10527853857:-0.0642277122098386:7103.27185138723
zPost1000 White Diffusion 4 ++:6079.12140476824:-0.0690282978995344:7411.81564577412
zPost1000 White Diffusion 5:6303.25382498961:-0.0398057720222809:7375.59737215753
zPost1000 White Diffusion 5 +:6342.5612821637:-0.0421377717939998:7596.25619744319
zPost1000 White Diffusion 5 ++:6949.92342558582:-0.0794233307418928:7072.23919845529
zPost1000 White Diffusion 6:5550.21851148506:-0.100420787695157:4750.67583112564
zPost1000 White Diffusion 6 +:5670.81636571797:-0.108323764747013:5376.56473603555
zPost1000 White Diffusion 6 ++:5800.83042561721:-0.114546100977793:6023.27194122576
zPost1000 White Diffusion 7:6435.84812544986:0.0566426433982111:8022.02873852717
zPost1000 White Diffusion 7 +:6470.5868500717:0.0586265651188185:8242.94116260509
zPost1000 White Diffusion 7 ++:6527.27491867084:0.0598780199571083:8387.90456614224
zPost1000 White Diffusion 8:6357.9867289745:-0.174809752949953:6960.95680328506
zPost1000 White Diffusion 8 +:6098.36622749452:-0.109958544526421:7728.91891998497
zPost1000 White Diffusion 8 ++:6316.5281657617:-0.107936966640921:8902.12285068563
zPost1000 White Diffusion 9:6682.41501790745:-0.141926608156325:5549.98298172747
zPost1000 White Diffusion 9 +:6965.2103144979:-0.105711939373919:1170.79216294922
zPost1000 White Diffusion 9 ++:6766.94431487063:-0.173714675507199:6598.69540486484
zPost1000 White Diffusion A:4653.57705406751:-0.0371742278718149:3428.29280012071
zPost1000 White Diffusion A +:4429.25711367922:-0.0329859868720871:3551.02091685133
zPost1000 White Diffusion A ++:4279.16481051885:-0.0284665642018511:3717.06275503297";



}
}



