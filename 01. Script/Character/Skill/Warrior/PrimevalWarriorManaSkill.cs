using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalWarriorManaSkill : ManaSkill
{
    public GameObject sky; // ��ǥ ���� (sky)
    public GameObject meteorPrefab; // � ������
    public Vector3 meteorDropPosition; // ��� ������ ��ġ
    

    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null || meteorPrefab == null || sky == null)
        {
            Debug.LogError("Prefab or target is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // �˱� ���� �� �ʱ�ȭ
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = sky.transform.position;

            // �˱Ⱑ ��ǥ ������ �����ϸ� ��� ����߸��� �ݹ� ����
            skillBehavior.OnSwordDestroyed += DropMeteor;

            // �˱� �ʱ�ȭ
            skillBehavior.Initialize(caster, skillDamage, skillRange, targetPosition, sky, false, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, true,3f ,10f);
        }
    }

    // �˱Ⱑ ��ǥ ������ �������� �� ȣ��Ǵ� �޼���
    private void DropMeteor(Vector3 dropPosition)
    {
        Debug.Log("Meteor will fall!");

        // ��� ������ ��ġ�� ����
        GameObject meteorInstance = GameObject.Instantiate(meteorPrefab, meteorDropPosition, Quaternion.identity);

        // ��� �߰����� ������ �ʿ��ϸ� ���⿡ �߰� ����
        // ��: meteorInstance.GetComponent<...>().SomeFunction();
    }
}
