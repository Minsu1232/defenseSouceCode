using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrimevalMagician : Magician
{
    public GameObject skillPrefab;    
    public GameObject manaSkillPrefab;
    public GameObject debuffPrefab;
    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;
    private AsyncOperationHandle<GameObject> skillHandle3;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ù ��° ��ų �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(18);
        if (skillData != null)
        {
            PrimevalMagicianSkill baseSkill = new PrimevalMagicianSkill(skillData);
            skills.Add(baseSkill);

            // ù ��° ��ų �ڵ� ����
            skillHandle1 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle1.DebugName} ù ��° ��ų �ڵ� �Ҵ��");
        }
        else
        {
            Debug.LogError("ù ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        // �� ��° ��ų �ε�
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(19);
        if (manaSkillData != null)
        {
            PrimevalMagicianManaSkill manaSkill = new PrimevalMagicianManaSkill(manaSkillData);
            skills.Add(manaSkill);

            // �� ��° ��ų �ڵ� ����
            skillHandle2 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle2.DebugName} �� ��° ��ų �ڵ� �Ҵ��");
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
