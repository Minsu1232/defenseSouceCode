using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MythWarrior : Warrior
{

    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ù ��° ��ų �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(10);
        if (skillData != null)
        {
            // ù ��° ��ų �Ҵ� �� �ڵ� ����
            DeathScytheSkill baseSkill = new DeathScytheSkill(skillData);
            skills.Add(baseSkill);
            skillHandle1 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle1.Result.name} ù ��° ��ų �ڵ� �Ҵ��");
        }
        else
        {
            Debug.LogError("ù ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        // �� ��° ��ų �ε�
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(11);
        if (manaSkillData != null)
        {
            // �� ��° ��ų �Ҵ� �� �ڵ� ����
            ByenightManaSkill manaSkill = new ByenightManaSkill(manaSkillData);
            skills.Add(manaSkill);
            skillHandle2 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle2.Result.name} �� ��° ��ų �ڵ� �Ҵ��");
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
