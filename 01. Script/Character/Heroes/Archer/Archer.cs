using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Archer : CharacterInfo
{
    

    protected override void UnlockNewSkill()
    {
        // ���ο� ��ų ��� ���� ���� ����
    }

    public override void PassiveEffect()
    {
        // �нú� ȿ�� ����
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
