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

        // 첫 번째 스킬 로드
        Skill skillData = await skillLoader.LoadSkillFromCSV(12);
        if (skillData != null)
        {
            AreaDevelopmentSkill baseSkill = new AreaDevelopmentSkill(skillData);
            skills.Add(baseSkill);

            // 첫 번째 스킬 핸들 저장
            skillHandle1 = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("첫 번째 스킬을 로드할 수 없습니다.");
        }

        // 두 번째 스킬 로드
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(13);
        if (manaSkillData != null)
        {
            MythMagicianBuff manaSkill = new MythMagicianBuff(manaSkillData);
            skills.Add(manaSkill);

            // 두 번째 스킬 핸들 저장
            skillHandle2 = skillLoader.GetCurrentHandle();
        }    
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 Addressables 프리팹 릴리즈
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
