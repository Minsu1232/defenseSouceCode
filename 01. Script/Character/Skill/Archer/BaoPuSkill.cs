using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaoPuSkill : Skill
{   
    
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        caster.IncreaseStats(0.5f, 0.001f, 0.001f);

        if (target != null)
        {
            Vector2 vector2 = target.transform.position; // 몬스터의 머리위에서 활이 떨어짐
            vector2.y += 2.5f;
            GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
            SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
            if (skillBehavior != null)
            {
                skillBehavior.Initialize(caster, skillDamage, skillRange, target.transform.position, target.gameObject, isSingtarget, false, 0, false, 0, false,12f);                
            }
        }
    }
   
}
