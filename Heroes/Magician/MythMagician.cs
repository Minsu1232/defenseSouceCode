using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MythMagician : Magician
{
    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // ù ��° ��ų �ε�
        Skill skillData = await skillLoader.LoadSkillFromCSV(12);
        if (skillData != null)
        {
            AreaDevelopmentSkill baseSkill = new AreaDevelopmentSkill(skillData);
            skills.Add(baseSkill);

            // ù ��° ��ų �ڵ� ����
            skillHandle1 = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("ù ��° ��ų�� �ε��� �� �����ϴ�.");
        }

        // �� ��° ��ų �ε�
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(13);
        if (manaSkillData != null)
        {
            MythMagicianBuff manaSkill = new MythMagicianBuff(manaSkillData);
            skills.Add(manaSkill);

            // �� ��° ��ų �ڵ� ����
            skillHandle2 = skillLoader.GetCurrentHandle();
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
