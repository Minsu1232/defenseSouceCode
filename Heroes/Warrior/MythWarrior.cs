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

        // 첫 번째 스킬 로드
        Skill skillData = await skillLoader.LoadSkillFromCSV(10);
        if (skillData != null)
        {
            // 첫 번째 스킬 할당 및 핸들 저장
            DeathScytheSkill baseSkill = new DeathScytheSkill(skillData);
            skills.Add(baseSkill);
            skillHandle1 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle1.Result.name} 첫 번째 스킬 핸들 할당됨");
        }
        else
        {
            Debug.LogError("첫 번째 스킬을 로드할 수 없습니다.");
        }

        // 두 번째 스킬 로드
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(11);
        if (manaSkillData != null)
        {
            // 두 번째 스킬 할당 및 핸들 저장
            ByenightManaSkill manaSkill = new ByenightManaSkill(manaSkillData);
            skills.Add(manaSkill);
            skillHandle2 = skillLoader.GetCurrentHandle();
            Debug.Log($"{skillHandle2.Result.name} 두 번째 스킬 핸들 할당됨");
        }
        else
        {
            Debug.LogError("두 번째 스킬을 로드할 수 없습니다.");
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
