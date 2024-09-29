using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalMagicianSkill : Skill
{
    
    public float buffDuration = 10f; // 강화 지속 시간
    public GameObject buffPrefab;
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (HeroManager.Instance == null || HeroManager.Instance.summonedHeroInstances.Count == 0)
        {
            Debug.LogError("No heroes found to apply the Holy Orb!");
            return;
        }

        // 랜덤한 영웅 선택
        GameObject targetHero = null;
        List<GameObject> potentialTargets = new List<GameObject>(HeroManager.Instance.summonedHeroInstances);

        // 버프를 받지 않은 영웅 중 랜덤하게 선택
        while (potentialTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialTargets.Count);
            GameObject hero = potentialTargets[randomIndex];

            if (!hero.GetComponent<CharacterInfo>().IsBuffed()) // 이미 버프를 받은 영웅은 제외
            {
                targetHero = hero;
                break;
            }

            potentialTargets.RemoveAt(randomIndex);
        }

        if (targetHero == null)
        {
            Debug.Log("No available heroes to receive the buff.");
            return;
        }

        // 구체 생성 및 타겟 영웅에게 보냄
        Vector3 vector3 = caster.transform.position;
        vector3.y += 1.5f;
        GameObject orbInstance = GameObject.Instantiate(skillPrefab,vector3, Quaternion.identity);
        orbInstance.GetComponent<HolyOrb>().Initialize(targetHero.GetComponent<CharacterInfo>(), buffDuration);
    }
}
