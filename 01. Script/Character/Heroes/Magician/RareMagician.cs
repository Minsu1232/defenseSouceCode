using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareMagician : Magician
{
    public GameObject coldBeamPrefab; // �ݵ�� ������

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new Skill
            {
                skillName = "�ݵ��",
                skillDescription = "���� ���鿡�� ���������� �����ϴ�.",
                skillDamage = characterData.attackPower * 1.5f,
                skillRange = 2f,
                skillProbability = 0.5f,
                skillPrefab = coldBeamPrefab, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = true,
                slowAmount = 3f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
