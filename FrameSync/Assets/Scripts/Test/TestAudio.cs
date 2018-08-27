using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

public class TestAudio : MonoBehaviour
{
    private string[] arr = new string[] {
        "atk_ac_01.ogg","atk_ac_02.ogg",
    };
    void Awake()
    {
        gameObject.AddComponentOnce<ResourceSys>();
        ResourceSys.Instance.Init(true, "Assets/ResourceEx");
        gameObject.AddComponentOnce<ResourceObjectPool>();
        gameObject.AddComponentOnce<PrefabPool>();
        gameObject.AddComponentOnce<AudioSys>();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,50),"播放声音"))
        {
            int index = UnityEngine.Random.Range(0,arr.Length);
            AudioSys.Instance.Play("Audio/"+ arr[index], AudioChannelType.UI, true, 1);
        }
    }
}
