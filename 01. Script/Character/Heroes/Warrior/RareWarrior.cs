using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class RareWarrior : Warrior
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
                skillName = "������",
                skillDescription = "ü���� ���� ���� ������ �˱⸦ �����ϴ�.",
                skillDamage = characterData.attackPower * 2f,
                skillRange = 5.0f, // ���� ���� (��: 5.0f)
                skillProbability = 0.4f,
                skillPrefab = swordSlash, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.01f,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
