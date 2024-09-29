using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalMagicianManaSkill : ManaSkill
{

    public float damageIncreasePercentage = 20f; // 데미지 증가 비율
    public float damageIncreaseDuration = 10f; // 데미지 증가 지속 시간
    public GameObject debuffPrefab;
    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {


        // 스킬 범위 내의 모든 적을 감지하여 배열로 반환
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        // 감지된 적이 하나라도 있는지 확인
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // 적이 존재하고 살아있는지 확인
            {
                Vector2 vector2 = enemy.transform.position; // 몬스터의 머리위에서 떨어짐
                vector2.y += 7f;
                vector2.x += 1f;
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, skillDamage, skillRange, enemy.transform.position, enemy.gameObject, true, false, 0, false, 0, false, 15f);


                }
                enemy.ApplyDamageIncrease(damageIncreasePercentage, damageIncreaseDuration);
                // 적의 Transform을 가져옴
                Transform enemyTransform = hit.transform;

                // 디버프 프리팹을 적의 자식으로 생성
                GameObject debuff = GameObject.Instantiate(debuffPrefab, enemyTransform.position, Quaternion.identity);

                // 생성된 디버프 오브젝트를 적의 자식으로 설정
                debuff.transform.SetParent(enemyTransform);
            }

        }
    }
}
