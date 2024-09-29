using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByenightManaSkill : ManaSkill
{
    public float manaGainPerAttack = 7f; // 기본 공격 시 획득하는 마나량
    public GameObject clonePrefab; // 분신 프리팹
    public float cloneDuration = 10f; // 분신 지속 시간
    public float clonePowerMultiplier = 0.8f; // 분신의 능력 배율

    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        // 캐릭터에게 분신 생성을 요청
        caster.SpawnClone(clonePrefab, cloneDuration, clonePowerMultiplier, target);

        // 마나 초기화
        currentMana = 0;
        UpdateManaBar(); // 마나바 업데이트
    }

    public void GainManaOnAttack()
    {
        currentMana += manaGainPerAttack;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateManaBar(); // 마나바 업데이트
    }
}
