using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class AirShipCreator
{
    [MenuItem("Assets/Custom/CreateAirShip")]
    public static void Create()
    {
        var obj = Selection.activeObject;
        if(obj == null || !(obj is Texture2D))
        {
            EditorUtility.DisplayDialog("提示", "请选择图片", "确定");
            return;
        }
        string path = AssetDatabase.GetAssetPath(obj);
        Debug.Log(path);
        var arr = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> lstSprite = new List<Sprite>();
        for (int i = 0; i < arr.Length; i++)
        {
            if(arr[i] is Sprite)
            {
                lstSprite.Add((Sprite)arr[i]);
            }
        }
        if(lstSprite.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "当前图片必须是sprite格式", "确定");
            return;
        }
        if(lstSprite.Count > 1)
        {
            lstSprite.Add(lstSprite[0]);
        }
        string dir = path.Replace("\\", "/");
        dir = dir.Substring(0, dir.LastIndexOf("/"));
        int lastIdx = dir.LastIndexOf("/");
        string name = dir.Substring(lastIdx+1, dir.Length -lastIdx - 1);
        AnimationClip animClip = BuildAnimClip(dir, "Idle", lstSprite, true);
        AnimatorController animController = BuildAnimatorController(dir, name, new List<AnimationClip> { animClip});
        BuildPrefab("Assets/ResourceEx/Prefab/Unit", name, animController, lstSprite[0]);
        AssetDatabase.Refresh();
    }

    private static AnimationClip BuildAnimClip(string dir,string animName, List<Sprite> lst,bool loop)
    {
        AnimationClip animClip = new AnimationClip();
        animClip.frameRate = 60;

        if(loop)
        {
            AnimationClipSettings clipSettings = new AnimationClipSettings();
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(animClip, clipSettings);
        }

        EditorCurveBinding binding = new EditorCurveBinding();
        binding.type = typeof(SpriteRenderer);
        binding.propertyName = "m_Sprite";
        binding.path = "";

        double frameTime = 1d / 10d;
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[lst.Count];
        for (int i = 0; i < lst.Count; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe();
            keyframes[i].value = lst[i];
            keyframes[i].time = (float)(frameTime * i);
        }

        AnimationUtility.SetObjectReferenceCurve(animClip, binding, keyframes);

        string animPath = dir + "/"+ animName +".anim";
        animClip.name = animName;
        AssetDatabase.CreateAsset(animClip, animPath);
        AssetDatabase.SaveAssets();
        return animClip;
    }

    private static AnimatorController BuildAnimatorController(string dir,string name,List<AnimationClip> lst)
    {
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(dir + "/" + name + ".controller");
        AnimatorControllerLayer animatorLayer = animatorController.layers[0];
        var stateMachine = animatorLayer.stateMachine;
        for (int i = 0; i < lst.Count; i++)
        {
            AnimatorState animatorState = stateMachine.AddState(lst[i].name);
            animatorState.motion = lst[i];
            //AnimatorStateTransition trans = stateMachine.AddAnyStateTransition(animatorState);
            //trans.hasExitTime = false;
        }
        AssetDatabase.SaveAssets();
        return animatorController;
    }

    private static void BuildPrefab(string dir,string name,AnimatorController animatorController,Sprite showSprite)
    {
        GameObject go = new GameObject();
        GameObject view = AddChild(go, "View");
        view.transform.localEulerAngles = new Vector3(90,180,0);
        view.transform.localScale = new Vector3(8,8,8);
        var spriteRender = view.AddComponent<SpriteRenderer>();
        spriteRender.sprite = showSprite;
        var animator = view.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;

        GameObject firePos = AddChild(go, "FirePos");
        firePos.tag = "HangPoint_Position";

        GameObject center = AddChild(go, "Center");
        center.tag = "HangPoint_Transform";

        GameObject collider = AddChild(go, "Collider");
        collider.tag = "GameCollider";
        //默认以圆形碰撞体
        var sphereCollider = collider.AddComponent<SphereCollider>();
        sphereCollider.enabled = false;

        go.AddComponent<HangPointView>();
        go.AddComponent<GameColliderView>();

        PrefabUtility.CreatePrefab(dir + "/" + name + ".prefab", go);
        GameObject.DestroyImmediate(go);
        AssetDatabase.SaveAssets();
    }

    private static GameObject AddChild(GameObject parent,string name)
    {
        GameObject go = new GameObject();
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        go.name = name;
        return go;
    }

}
