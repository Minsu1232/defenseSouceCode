using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Warrior : CharacterInfo
{
    protected override void UnlockNewSkill()
    {
        // ���ο� ��ų ��� ���� ���� ����
    }



    public override void PassiveEffect()
    {

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
    }
    protected override void Update()
    {
        base.Update();
    }
}
