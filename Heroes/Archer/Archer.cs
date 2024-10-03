using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Archer : CharacterInfo
{
    private SkillLoader loader;


    protected override void UnlockNewSkill()
    {
        // 새로운 스킬 잠금 해제 로직 구현
    }

    public override void PassiveEffect()
    {
        // 패시브 효과 구현
    }
    protected override void BasicAttack()
    {
        base.BasicAttack();       

        // 화살의 생성 위치는 캐릭터의 현재 위치로 고정
        Vector2 arrowSpawnPosition = arrowTransform.transform.position; // 캐릭터의 위치 기준으로 화살을 생성
        arrowSpawnPosition.y += 0.1f; // 캐릭터보다 살짝 위에서 생성

        GameObject arrow = Instantiate(attackprefab, arrowSpawnPosition, Quaternion.identity); // 화살 생성
        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            // 타겟이 움직여도 상관없이 화살은 고정된 위치에서 생성되며, 타겟을 향해 발사됨
            arrowScript.Initialize(closetTarget, damage, this); // 화살이 타겟을 향하도록 초기화
        }


    }

    //public override void LevelUp()
    //{
    //    characterData.level++;
    //    if (characterData.level == 3)
    //    {
    //        UnlockNewSkill();
    //    }
    //}
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
