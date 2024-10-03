using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;
    private void OnEnable()
    {   Vector3 vector3 = gameObject.transform.position;
        vector3.y += 28f; 
        Instantiate(sky,vector3, Quaternion.identity);
    }
   
    protected override async void Start()
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
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ù ��° ��ų �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(16);
        if (skillData != null)
        {
            PrimevalWarriorSkill baseSkill = new PrimevalWarriorSkill(skillData);
            skills.Add(baseSkill);

            // ù ��° ��ų �ڵ� ����
            skillHandle1 = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("ù ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        // �� ��° ��ų �ε�
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(17);
        if (manaSkillData != null)
        {
            PrimevalWarriorManaSkill manaSkill = new PrimevalWarriorManaSkill(manaSkillData);
            skills.Add(manaSkill);

            // �� ��° ��ų �ڵ� ����
            skillHandle2 = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("�� ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        
    }

    private void OnDestroy()
    {
        // ������Ʈ�� �ı��� �� Addressables ������ ������
        if (skillHandle1.IsValid())
        {
            Addressables.Release(skillHandle1);
        }

        if (skillHandle2.IsValid())
        {
            Addressables.Release(skillHandle2);
        }
    }
}
