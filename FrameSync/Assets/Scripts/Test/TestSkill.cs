using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game;
using NodeEditor;
using Framework;
using BTCore;

public class TestSkillActionData
{
    [NEProperty("触发时间",true)]
    public FP time;
    public string desc;
    [NEProperty("次数")]
    public int times;
}

[SkillNode(typeof(TestSkillActionData))]
public class TestSkillAction : BaseTimeLineSkillAction
{
    public TestSkillActionData skillData;
    public override FP time
    {
        get
        {
            return skillData.time;
        }
    }
    private int m_nTimes;
    protected override void OnInitData(object data)
    {
        skillData = data as TestSkillActionData;
    }

    protected override void OnEnter(SkillBlackBoard blackBoard)
    {
        m_nTimes = skillData.times;
        CLog.Log("TestSkillAction:[OnEnter]");
    }

    public override BTActionResult OnRun(SkillBlackBoard blackBoard)
    {
        m_nTimes--;
        CLog.LogColorArgs(CLogColor.Red, skillData.desc);
        if (m_nTimes > 0) return BTActionResult.Running;
        return BTActionResult.Ready;
    }

    public override void OnExit(SkillBlackBoard blackBoard)
    {
        CLog.Log("TestSkillAction:[OnExit]");
    }

}

public class TestSkill : MonoBehaviour
{
    public TextAsset txt;
    private Skill skill;
    private NEDataLoader m_cLoader;
    void Start()
    {
        gameObject.AddComponent<ResourceSys>();
        ResourceSys.Instance.Init(true, "Assets/ResourceEx");
        m_cLoader = new NEDataLoader();
        Skill.Init();
        m_cLoader.Load(new List<string> { "Config/Skill/test.bytes" }, Skill.arrSkillNodeDataType, OnFinish);
        //Skill.Init();
        //NEData neData = NEUtil.DeSerializerObjectFromBuff(txt.bytes, typeof(NEData), Skill.lstSkillNodeDataType.ToArray()) as NEData;
        //skill = new Skill(null,neData);
    }

    private void OnFinish()
    {
        Debug.Log("aa");
    }

    void Update()
    {
        //if(skill.CanDo())
        //{
        //    skill.Do();
        //}
        //skill.Update(FP.FromFloat(Time.deltaTime));
    }
}
