using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByenightManaSkill : ManaSkill
{
    public float manaGainPerAttack = 7f; // �⺻ ���� �� ȹ���ϴ� ������
    public GameObject clonePrefab; // �н� ������
    public float cloneDuration = 10f; // �н� ���� �ð�
    public float clonePowerMultiplier = 0.8f; // �н��� �ɷ� ����

    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        // ĳ���Ϳ��� �н� ������ ��û
        caster.SpawnClone(clonePrefab, cloneDuration, clonePowerMultiplier, target);

        // ���� �ʱ�ȭ
        currentMana = 0;
        UpdateManaBar(); // ������ ������Ʈ
    }

    public void GainManaOnAttack()
    {
        currentMana += manaGainPerAttack;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateManaBar(); // ������ ������Ʈ
    }
}
