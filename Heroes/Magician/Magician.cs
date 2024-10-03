using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : CharacterInfo
{

    protected override void UnlockNewSkill()
    {
        // 새로운 스킬 잠금 해제 로직 구현
    }

    public override void PassiveEffect()
    {
        float speedReductionPercentage = characterData.heroGrade.speedReductionPercentage;
        //MonsterSpawnManager.Instance.ApplyPassiveEffect(speedReductionPercentage);
    }
    protected override void BasicAttack()
    {
        base.BasicAttack();
        target = closetTarget;
        float damage = AttackPower; // 기본 공격력 설정
        animator.speed = AttackSpeed; // 애니메이션 속도를 공격 속도에 맞춤
        animator.SetTrigger("Attack"); // 공격 애니메이션 실행 (SetBool 대신 SetTrigger 사용)
        bool isCrtical = IsCriticalHit();
        if (isCrtical)
        {
            damage *= 2; // 크리티컬 히트 시 데미지 2배                    
            Debug.Log("Critical hit!");
        }

        Vector2 arrowSpawnPosition = arrowTransform.transform.position; // 캐릭터의 위치 기준으로 불렛 생성
        arrowSpawnPosition.y += 0.1f; // 캐릭터보다 살짝 위에서 생성

        GameObject energyBolt = Instantiate(attackprefab, arrowSpawnPosition, Quaternion.identity);
        EnergyBolt boltScript = energyBolt.GetComponent<EnergyBolt>();
        if (boltScript != null)
        {
            boltScript.Initialize(closetTarget, damage, this);
        }

    }

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용
                         // 스킬 초기화       
    }
    protected override void Update()
    {
        base.Update();
    }
}
