using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EffectCreator
{
    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame1")]
    public static void CreateRepeatFrameAnim1()
    {
        CreateRepeatFrameAnim(1);
    }
    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame2")]
    public static void CreateRepeatFrameAnim2()
    {
        CreateRepeatFrameAnim(2);
    }

    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame3")]
    public static void CreateRepeatFrameAnim3()
    {
        CreateRepeatFrameAnim(3);
    }

    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame4")]
    public static void CreateRepeatFrameAnim4()
    {
        CreateRepeatFrameAnim(4);
    }

    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame5")]
    public static void CreateRepeatFrameAnim5()
    {
        CreateRepeatFrameAnim(5);
    }

    [MenuItem("Assets/Custom/CreateRepeatEffect(Shader FrameAnim)/Frame6")]
    public static void CreateRepeatFrameAnim6()
    {
        CreateRepeatFrameAnim(6);
    }

    public static void CreateRepeatFrameAnim(int count)
    {
        var objs = Selection.objects;
        if (objs == null) return;
        int i = 0;
        EditorUtility.DisplayProgressBar("转化为特效", i + "/" + objs.Length, 0);
        for (; i < objs.Length; i++)
        {
            var obj = objs[i];
            if (obj == null || !(obj is Texture2D))
            {
                EditorUtility.DisplayDialog("提示", "当前选择对象必须是Texture2D", "确定");
                return;
            }
            string path = AssetDatabase.GetAssetPath(obj);
            string fileDir = EditorUtilTool.GetDirectory(path);
            string fileName = EditorUtilTool.GetFileName(path, false);

            //创建材质
            Texture2D tex = (Texture2D)obj;
            Shader shader = Shader.Find("Custom/Unlit/FrameAnim");
            if (shader == null)
            {
                EditorUtility.DisplayDialog("提示", "找不到Custom/Unlit/FrameAnim着色器", "确定");
                return;
            }
            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", tex);
            mat.SetInt("_Column", count);
            AssetDatabase.CreateAsset(mat, fileDir + "/Materials/" + fileName + ".mat");
            AssetDatabase.SaveAssets();

            //创建预制
            GameObject go = new GameObject();
            GameObject view = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var collider = view.GetComponent<Collider>();
            if (collider != null) GameObject.DestroyImmediate(collider);
            view = EditorUtilTool.AddChild(go, view, "view");
            view.transform.localEulerAngles = new Vector3(90, 0, 0);
            float perWidth = (float)tex.width / count;
            float height = tex.height / perWidth;
            view.transform.localScale = new Vector3(1f, height, 1f);
            var renderer = view.GetComponent<Renderer>();
            renderer.material = mat;
            PrefabUtility.CreatePrefab("Assets/ResourceEx/Prefab/Effect/" + fileName + ".prefab", go);
            GameObject.DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayProgressBar("转化为特效", i + "/" + objs.Length, (float)i / objs.Length);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame1")]
    public static void CreateOnceFrameAnim1()
    {
        CreateOnceFrameAnim(1);
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame2")]
    public static void CreateOnceFrameAnim2()
    {
        CreateOnceFrameAnim(2);
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame3")]
    public static void CreateOnceFrameAnim3()
    {
        CreateOnceFrameAnim(3);
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame4")]
    public static void CreateOnceFrameAnim4()
    {
        CreateOnceFrameAnim(4);
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame5")]
    public static void CreateOnceFrameAnim5()
    {
        CreateOnceFrameAnim(5);
    }

    [MenuItem("Assets/Custom/CreateOnceEffect(Particle FrameAnim)/Frame6")]
    public static void CreateOnceFrameAnim6()
    {
        CreateOnceFrameAnim(6);
    }

    public static void CreateOnceFrameAnim(int count)
    {
        var objs = Selection.objects;
        if (objs == null) return;
        int i = 0;
        EditorUtility.DisplayProgressBar("转化为特效", i + "/" + objs.Length, 0);
        for (; i < objs.Length; i++)
        {
            var obj = objs[i];
            if (obj == null || !(obj is Texture2D))
            {
                EditorUtility.DisplayDialog("提示", "当前选择对象必须是Texture2D", "确定");
                return;
            }
            string path = AssetDatabase.GetAssetPath(obj);
            string fileDir = EditorUtilTool.GetDirectory(path);
            string fileName = EditorUtilTool.GetFileName(path, false);

            //创建材质
            Texture2D tex = (Texture2D)obj;
            Shader shader = Shader.Find("Particles/Additive");
            if (shader == null)
            {
                EditorUtility.DisplayDialog("提示", "找不到Particles/Additive着色器", "确定");
                return;
            }
            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", tex);
            AssetDatabase.CreateAsset(mat, fileDir + "/Materials/" + fileName + ".mat");
            AssetDatabase.SaveAssets();

            //创建预制
            GameObject go = new GameObject();
            GameObject view = EditorUtilTool.AddChild(go, null, "view");
            ParticleSystem particleSys = view.AddComponent<ParticleSystem>();
            var renderer = view.GetComponent<ParticleSystemRenderer>();
            renderer.enabled = true;
            renderer.material = mat;
            renderer.sortingOrder = 1;

            var textureSheetAnimation = particleSys.textureSheetAnimation;
            textureSheetAnimation.enabled = true;
            textureSheetAnimation.numTilesX = count;

            var mainParticleSys = particleSys.main;
            mainParticleSys.maxParticles = 1;
            mainParticleSys.loop = false;
            mainParticleSys.duration = 1f;
            mainParticleSys.startDelay = 0;
            mainParticleSys.startLifetime = 1f;
            mainParticleSys.startSpeed = 0;
            mainParticleSys.duration = 0.1f;

            var particleShape = particleSys.shape;
            particleShape.angle = 0;
            particleShape.radius = 0;

            var emission = particleSys.emission;
            emission.rateOverTime = 0;
            ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 1, 1);
            emission.SetBursts(new ParticleSystem.Burst[] { burst});


            PrefabUtility.CreatePrefab("Assets/ResourceEx/Prefab/Effect/" + fileName + ".prefab", go);
            GameObject.DestroyImmediate(go);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayProgressBar("转化为特效", i + "/" + objs.Length, (float)i / objs.Length);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
}
