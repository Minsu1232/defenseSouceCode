using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static CharacterInfo;

public class HeroMagician : Magician
{

    private SkillLoader skillLoader;
    // ��ų �����͸� �ε��� ��, �� �ڵ�� �������� �� �ֵ��� ����
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ��ų �����͸� CSV���� �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(5);

        if (skillData != null)
        {
            // MagicianSkill �����ڸ� ����Ͽ� �ùٸ��� ��ü ����
            MagicianSkill skill = new MagicianSkill(skillData);

            // ��ų ������ �ڵ��� ���� (���߿� ������ �� ���)
            skillHandle = skillLoader.GetCurrentHandle();
            skills = new List<Skill> { skill };
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
