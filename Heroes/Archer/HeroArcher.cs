using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static CharacterInfo;

public class HeroArcher : Archer
{
    private SkillLoader skillLoader;

    // ��ų �����͸� �ε��� ��, �� �ڵ�� �������� �� �ֵ��� ����
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        Skill skillData = await skillLoader.LoadSkillFromCSV(6);

        if (skillData != null)
        {
            // �ε�� �����͸� ������� ArrowShotSkill ����
            ArrowShotSkill arrowShotSkill = new ArrowShotSkill(skillData);
            skills = new List<Skill> { arrowShotSkill };

            // ��ų ������ �ڵ��� ���� (���߿� ������ �� ���)
            skillHandle = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("��ų�� �ε��� �� �����ϴ�.");
        }
    }

    // HeroArcher ������Ʈ�� �ı��� �� Addressables ���ҽ��� ������
    private void OnDestroy()
    {
        if (skillHandle.IsValid())
        {
            Addressables.Release(skillHandle);
            Debug.Log("��ų ������ ������ �Ϸ�");
        }
    }

}
   

