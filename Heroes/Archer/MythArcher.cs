using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MythArcher : Archer
{
    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;

    protected override async void Start()
    {
        base.Start();
        // SkillLoader�� MonoBehaviour�� �߰�
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ù ��° ��ų �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(14);
        if (skillData != null)
        {
            BaoPuSkill baseSkill = new BaoPuSkill(skillData);
            skills.Add(baseSkill);
            // ù ��° ��ų �ڵ� ����
            skillHandle1 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle1.Result.name} ù ��° ��ų �ڵ� �Ҵ��");
        }
        else
        {
            Debug.LogError("ù ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        // �� ��° ��ų �ε�
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(15);
        if (manaSkillData != null)
        {
            BaoPuManaSkill manaSkill = new BaoPuManaSkill(manaSkillData);
            skills.Add(manaSkill);
            // �� ��° ��ų �ڵ� ����
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
    
  


