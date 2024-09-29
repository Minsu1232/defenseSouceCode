using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScytheSkill : Skill
{
    public override float CalculateDamage(float targetMaxHealth, float damage)
    {

        float calculatedDamage = targetMaxHealth * damage; // 최대 체력의 5% 데미지
        return Mathf.Min(calculatedDamage, 3500f); // 최대 3500 데미지로 제한
    }

    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // 스킬 범위 내의 모든 적을 감지하여 배열로 반환
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        Monster highestHealthEnemy = null;
        float highestHealth = 0;

        // 감지된 적 중에서 가장 체력이 높은 적을 찾음
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie)
            {
                if (enemy.currentHealth > highestHealth)
                {
                    highestHealth = enemy.currentHealth;
                    highestHealthEnemy = enemy;
                }
            }
        }

        // 가장 체력이 높은 적이 있을 경우 스킬 실행
        if (highestHealthEnemy != null)
        {
            GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
            SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
            if (skillBehavior != null)
            {
                skillBehavior.Initialize(caster, CalculateDamage(highestHealthEnemy.maxHealth,skillDamage), 0, highestHealthEnemy.transform.position, highestHealthEnemy.gameObject, true, false, slowAmount, true, defenseReductionAmount);
            }
        }
    }
}
