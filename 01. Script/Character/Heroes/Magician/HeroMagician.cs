using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroMagician : Magician
{
    public GameObject skeletonPrefab; // �ݵ�� ������

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new Skill
            {
                skillName = "������",
                skillDescription = "������ ������ �����ϴ�.",
                skillDamage = characterData.attackPower * 2f,
                skillRange = 2f,
                skillProbability = 0.5f,
                skillPrefab = skeletonPrefab, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.01f,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
