using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythMagicianBuff : ManaSkill
{
    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        caster.StartCoroutine(Buff(caster,10));
        skillPrefab.SetActive(true);
    }
    IEnumerator Buff(CharacterInfo caster, int duration)
    {
        float attack = caster.baseAttackPower * 0.15f;
        float speed = caster.baseAttackSpeed * 0.15f;
        float critical = caster.baseAttackCritical * 0.1f;
        caster.IncreaseStats(attack, speed, critical); // ���� ����
        skillPrefab.SetActive(true);
        yield return new WaitForSeconds(duration); // ���� ���� �ð�
        skillPrefab.SetActive(false);
        caster.IncreaseStats(attack,speed, critical); // ���� ����
    }
}
