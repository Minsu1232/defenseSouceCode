using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LegendaryMagician : Magician
{
    private SkillLoader skillLoader;
    // ��ų �����͸� �ε��� ��, �� �ڵ�� �������� �� �ֵ��� ����
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        Skill skillData = await skillLoader.LoadSkillFromCSV(8);

        if (skillData != null)
        {
            // �ε�� �����͸� ������� ArrowShotSkill ����
            SunOveSkill skill = new SunOveSkill(skillData);
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
