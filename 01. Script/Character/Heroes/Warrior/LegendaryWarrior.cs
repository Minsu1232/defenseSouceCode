using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class LegendaryWarrior : Warrior
{
    public GameObject swordSlash; // ��ų ������

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new SwordSkill
            {
                skillName = "�巡�� ������",
                skillDescription = "ü���� ���� ���� ������ ���� ���� ��� �˱⸦ �����ϴ�.",
                skillDamage = characterData.attackPower * 4f,
                skillRange = 5.0f, // ���� ���� (��: 5.0f)
                skillProbability = 0.5f,
                skillPrefab = swordSlash, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.02f,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
