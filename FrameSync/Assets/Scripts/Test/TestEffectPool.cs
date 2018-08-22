using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using Game;

public class TestEffectPool : MonoBehaviour
{
    void Start()
    {

        gameObject.AddComponentOnce<ResourceSys>();
        ResourceSys.Instance.Init(true, "Assets/ResourceEx");

        //初始化对象池
        GameObject goPool = new GameObject();
        goPool.name = "GameObjectPool";
        GameObject.DontDestroyOnLoad(goPool);
        goPool.AddComponentOnce<ResourceObjectPool>();
        //初始化特效池
        GameObject effectPool = new GameObject();
        effectPool.name = "EffectPool";
        GameObject.DontDestroyOnLoad(effectPool);
        effectPool.AddComponentOnce<SceneEffectPool>();
    }

    private GameObject effect;
    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,50),"创建特效"))
        {
            effect = SceneEffectPool.Instance.CreateEffect("Bullet", true);
        }

        if (GUI.Button(new Rect(100, 0, 100, 50), "销毁特效"))
        {
            if (effect != null)
            {
                SceneEffectPool.Instance.DestroyEffectGO(effect);
                effect = null;
            }
        }

        if (GUI.Button(new Rect(200, 0, 100, 50), "清除特效"))
        {
            SceneEffectPool.Instance.Clear();
        }
    }
}
