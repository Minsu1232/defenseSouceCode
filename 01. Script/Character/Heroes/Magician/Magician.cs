using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : CharacterInfo
{

    protected override void UnlockNewSkill()
    {
        // ���ο� ��ų ��� ���� ���� ����
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
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����
                         // ��ų �ʱ�ȭ       
    }
    protected override void Update()
    {
        base.Update();
    }
}
