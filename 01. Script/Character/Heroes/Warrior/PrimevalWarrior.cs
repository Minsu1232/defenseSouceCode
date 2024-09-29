using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalWarrior : Warrior
{
    public GameObject swordSlash; // ��ų ������
    public GameObject manaSkillPrefab;
    public GameObject swordAura; // ����� ������
    public GameObject sky; // �˱Ⱑ���ư� ��ġ
    public GameObject swordCrack; //Į�� ������ �� �� ������
    public Transform[] dropPoints; // Į�� ������ �� �ִ� ������ ����Ʈ��
    

    //public GameObject sky; // ��ǥ ���� (sky)
    //public GameObject meteorPrefab; // � ������
    public Vector3 meteorDropPosition; // ��� ������ ��ġ
    public Vector3 meteorStartPosition;
    //public GameObject swordPrefab; // �˱� ������
    // Start is called before the first frame update
    private void OnEnable()
    {   Vector3 vector3 = gameObject.transform.position;
        vector3.y += 28f; 
        Instantiate(sky,vector3, Quaternion.identity);
    }
    protected override void Start()
    {
        Vector3 vector = new Vector3(-5.2f, -2f, 0f);
        GameObject newDropPoint = new GameObject("DropPoint");
        newDropPoint.transform.position = vector;
        Vector3 vector1 = new Vector3(-5.2f, 2.8f, 0f);
        GameObject newDropPoint1 = new GameObject("DropPoint");
        newDropPoint1.transform.position = vector1;
        Vector3 vector2 = new Vector3(5.2f, -2f, 0f);
        GameObject newDropPoint2 = new GameObject("DropPoint");
        newDropPoint2.transform.position = vector2;
        Vector3 vector3 = new Vector3(5.2f, 2.8f, 0f);
        GameObject newDropPoint3 = new GameObject("DropPoint");
        newDropPoint3.transform.position = vector3;

        dropPoints[0] = newDropPoint.transform;
        dropPoints[1] = newDropPoint1.transform;
        dropPoints[2] = newDropPoint2.transform;
        dropPoints[3] = newDropPoint3.transform;

        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new PrimevalWarriorSkill
            {
                skillName = "���̸���",
                skillDescription = "���� ���� ��� Į�� �ҷ� �ɴϴ�.",
                skillDamage = characterData.attackPower * 5,
                skillRange = 3.0f, // ���� ���� (��: 5.0f)
                skillProbability =0.25f,
                skillPrefab = swordSlash, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.2f,
                //barrierPrefab = barrierPrefab,
                dropPoints = dropPoints,
                swordCrack = swordCrack

            },
            new PrimevalWarriorManaSkill
            {
                skillName = "õ�󰭸�",
                skillDescription = "�ϴ��� ���� ������ ����߸��ϴ�.",
                meteorPrefab = manaSkillPrefab, // ����
                skillPrefab = swordAura,
                sky = sky,
                meteorDropPosition = meteorDropPosition,
                //meteorStartPosition =  meteorStartPosition,
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                mpBar = mpBar // UI���� �Ҵ�� MP �� �̹���
            }
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
