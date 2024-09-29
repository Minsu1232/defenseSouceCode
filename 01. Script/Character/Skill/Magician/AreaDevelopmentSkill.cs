using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDevelopmentSkill : Skill
{
    public bool isSpecialSkill = true;
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }
                                  
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, skillDamage, skillRange, targetPosition, target, false, true, slowAmount, true, defenseReductionAmount, isSpecialSkill);

        }
    }
}
