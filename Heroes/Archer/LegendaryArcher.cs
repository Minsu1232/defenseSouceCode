using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LegendaryArcher : Archer
{
    private SkillLoader skillLoader;
    // ��ų �����͸� �ε��� ��, �� �ڵ�� �������� �� �ֵ��� ����
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        Skill skillData = await skillLoader.LoadSkillFromCSV(9);

        if (skillData != null)
        {
            // �ε�� �����͸� ������� ArrowShotSkill ����
            ArrowRainSkill skill = new ArrowRainSkill(skillData);
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
