using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowShotSkill : Skill
{
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }
        // 스킬 범위 내의 모든 적을 감지하여 배열로 반환
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        // 감지된 적이 하나라도 있는지 확인
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // 적이 존재하고 살아있는지 확인
            {
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, skillDamage, skillRange, enemy.transform.position, enemy.gameObject,true,false,0,false,10f);
                }
            }
        }
    }
}
