using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static CharacterInfo;

public class HeroMagician : Magician
{

    private SkillLoader skillLoader;
    // 스킬 데이터를 로드한 후, 이 핸들로 릴리즈할 수 있도록 저장
    private AsyncOperationHandle<GameObject> skillHandle;

    protected override async void Start()
    {
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // 스킬 데이터를 CSV에서 로드
        Skill skillData = await skillLoader.LoadSkillFromCSV(5);

        if (skillData != null)
        {
            // MagicianSkill 생성자를 사용하여 올바르게 객체 생성
            MagicianSkill skill = new MagicianSkill(skillData);

            // 스킬 프리팹 핸들을 저장 (나중에 해제할 때 사용)
            skillHandle = skillLoader.GetCurrentHandle();
            skills = new List<Skill> { skill };
        }
        else
        {
            Debug.LogError("스킬을 로드할 수 없습니다.");
        }
    }

    private void OnDestroy()
    {
        if (skillHandle.IsValid())
        {
            Addressables.Release(skillHandle);
            Debug.Log("스킬 프리팹 릴리즈 완료");
        }
    }
}
