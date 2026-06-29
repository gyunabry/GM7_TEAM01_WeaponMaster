using UnityEngine;

public class SwordSkill : MonoBehaviour, ISkillBase
{
    Transform parentTrans;
    private void Awake()
    {
        parentTrans = GetComponentInParent<Transform>();
    }
    public void Skill(SkillStat ss) 
    { 
        if(ss.skillEnable == false)
        {
            return;
        }

    }
    
}
