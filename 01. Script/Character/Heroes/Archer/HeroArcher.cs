using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroArcher : Archer
{
    public GameObject skillArrow; // ��ų ȭ�� ������
    protected override void Start()
    {
        base.Start();       
        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new ArrowShotSkill
            {
                skillName = "�����ַο켦",
                skillDescription = "��ũ������ ���� ��� ȭ���� ���ϴ�.",
                skillDamage = characterData.attackPower * 1.5f,
                skillRange = 3.5f,
                skillProbability = 0.3f,
                skillPrefab = skillArrow, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
   

