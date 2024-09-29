using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunOveSkill : Skill
{
    public bool isSpecialSkill = true;
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        //// ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        //Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        //// ������ ���� �ϳ��� �ִ��� Ȯ��
        //foreach (var hit in hits)
        //{
        //    Monster enemy = hit.GetComponent<Monster>();
        //    if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // ���� �����ϰ� ����ִ��� Ȯ��
        //    {                                
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, skillDamage, skillRange, targetPosition, target, false, false, 0, true, defenseReductionAmount, isSpecialSkill);

        }
    }
}
    



