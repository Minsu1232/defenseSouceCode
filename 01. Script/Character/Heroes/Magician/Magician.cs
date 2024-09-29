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
