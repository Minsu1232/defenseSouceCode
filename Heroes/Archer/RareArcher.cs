using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RareArcher : Archer
{
    private SkillLoader skillLoader;
    // ��ų �����͸� �ε��� ��, �� �ڵ�� �������� �� �ֵ��� ����
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        Skill skillData = await skillLoader.LoadSkillFromCSV(3);

        if (skillData != null)
        {
            // �ε�� �����͸� ������� ArrowShotSkill ����
            ArrowShotSkill skill = new ArrowShotSkill(skillData);
            skills = new List<Skill> { skill };

            // ��ų ������ �ڵ��� ���� (���߿� ������ �� ���)
            skillHandle = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("��ų�� �ε��� �� �����ϴ�.");
        }
    }

   
    private void OnDestroy()
    {
        if (skillHandle.IsValid())
        {
            Addressables.Release(skillHandle);
            Debug.Log("��ų ������ ������ �Ϸ�");
        }
    }
}

