using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalMagicianSkill : Skill
{
    
    public float buffDuration = 10f; // ��ȭ ���� �ð�
    public GameObject buffPrefab;
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (HeroManager.Instance == null || HeroManager.Instance.summonedHeroInstances.Count == 0)
        {
            Debug.LogError("No heroes found to apply the Holy Orb!");
            return;
        }

        // ������ ���� ����
        GameObject targetHero = null;
        List<GameObject> potentialTargets = new List<GameObject>(HeroManager.Instance.summonedHeroInstances);

        // ������ ���� ���� ���� �� �����ϰ� ����
        while (potentialTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialTargets.Count);
            GameObject hero = potentialTargets[randomIndex];

            if (!hero.GetComponent<CharacterInfo>().IsBuffed()) // �̹� ������ ���� ������ ����
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

        // ��ü ���� �� Ÿ�� �������� ����
        Vector3 vector3 = caster.transform.position;
        vector3.y += 1.5f;
        GameObject orbInstance = GameObject.Instantiate(skillPrefab,vector3, Quaternion.identity);
        orbInstance.GetComponent<HolyOrb>().Initialize(targetHero.GetComponent<CharacterInfo>(), buffDuration);
    }
}
