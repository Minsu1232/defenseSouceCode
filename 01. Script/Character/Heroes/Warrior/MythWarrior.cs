using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythWarrior : Warrior
{
    public GameObject swordSlash; // ��ų ������
    public GameObject manaSkillPrefab;

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new DeathScytheSkill
            {
                skillName = "�������̵�",
                skillDescription = "ü���� ���� ���� ������ ����� ���� �ֵθ��ϴ�.",
                skillDamage = 0.05f,
                skillRange = 5.0f, // ���� ���� (��: 5.0f)
                skillProbability = 0.1f,
                skillPrefab = swordSlash, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.03f,
            },
            new ByenightManaSkill
            {
                skillName = "��ȥ���",
                skillDescription = "���� ������ ���� ���� ��ȥ�� ȹ���մϴ�. ��ȥ�� �������� �н��� ��ȯ�մϴ� ",                         
                clonePrefab = manaSkillPrefab, // �ν����Ϳ��� �Ҵ�� ������ ���
                manaCost = 300f,
                manaRegenRate = 0f,
                maxMana = 300f,
                mpBar = mpBar // UI���� �Ҵ�� MP �� �̹���
            }
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
