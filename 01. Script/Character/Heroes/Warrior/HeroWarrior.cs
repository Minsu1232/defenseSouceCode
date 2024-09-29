using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroWarrior : Warrior
{
    public GameObject[] chefPrefabs; // ��ų ������

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new ChefSkill
            {
                skillName = "����ī��",
                skillDescription = "ü���� ���� ���� ������ ������ ������ �����ϴ�.",
                skillDamage = characterData.attackPower * 2.5f,
                skillRange = 3.0f, // ���� ���� (��: 5.0f)
                skillProbability = 0.4f,
                gameObjects = chefPrefabs, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.01f,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
