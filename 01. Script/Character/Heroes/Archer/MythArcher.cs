using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythArcher : Archer
{
    public GameObject skillArrow; // ��ų ȭ�� ������
    public GameObject manaSkillPrefab;   // ���� ��ų ������
    
    protected override void Start()
    {
        base.Start();
       

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            // �⺻ ��ų �߰�
            new BaoPuSkill
            {
                skillName = "��â",
                skillDescription = "���� ���ݽ� ��â�� �Բ� ���� �մϴ�.",
                skillDamage = characterData.attackPower * 1.5f,
                skillRange = 5.5f,
                skillProbability = 1f,
                skillPrefab = skillArrow, // �ν����Ϳ��� �Ҵ�� ������ ���
                isSingtarget = true,
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },

            // ���� ��ų �߰�
            new BaoPuManaSkill
            {
                skillName = "�׸�",
                skillDescription = "�ϴÿ��� �볪���� ��� ���ϴ�.",
                skillDamage = characterData.attackPower * 4f,
                skillRange = 4f,
                
                skillPrefab = manaSkillPrefab,   // ���� ��ų ������ ���
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0f,
                mpBar = mpBar // UI���� �Ҵ�� MP �� �̹���
            }
        };
    }
}
    
  


