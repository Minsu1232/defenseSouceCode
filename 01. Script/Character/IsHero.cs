//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class IsHero : MonoBehaviour
//{
//   public static HeroManager Instance { get; private set; }

//    private HashSet<HeroGrade> activeHeroGrades = new HashSet<HeroGrade>();

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void RegisterHero(HeroGrade heroGrade)
//    {
//        activeHeroGrades.Add(heroGrade);
//        UpdatePassiveEffects();
//    }

//    public void UnregisterHero(HeroGrade heroGrade)
//    {
//        activeHeroGrades.Remove(heroGrade);
//        UpdatePassiveEffects();
//    }

//    private void UpdatePassiveEffects()
//    {
//        float totalSpeedReduction = 0f;
//        foreach (var grade in activeHeroGrades)
//        {
//            totalSpeedReduction += grade.speedReductionPercentage;
//        }
//        MonsterSpawnManager.Instance.ApplyPassiveEffect(totalSpeedReduction);
//    }
//}
