
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WeaponTransform : MonoBehaviour
//{
//    //public SkeletonAnimation skeletonAnimation; // 영웅 데이터

//    enum WeaponType // 영웅에게 쥐어줄 무기 종류
//    {
//        Clown_Magician_Weapon,
//        Frozen_Witch_Weapon,
//        Fairy_Green_Magician_Weapon,
//        Science_Wizard_Weapon,
//        Star_Magician_Weapon_2,
//        Flame_Magician_Weapon
//    }

//    private Dictionary<string, WeaponType> weaponData = new Dictionary<string, WeaponType>(); // 영웅에게 줄 무기를 찾기 위한 딕셔너리

//    void Start()
//    {
//        // 데이터 초기화
//        InitializeWeaponData();

//        // 각 SkeletonAnimation에 무기 설정       
//        string weaponKey = skeletonAnimation.tag;  // 예시로 name 사용
//        SetWeapon(skeletonAnimation, weaponKey);

//    }

//    void InitializeWeaponData() // 생성된 영웅의 태그를 통해 무기를 쥐어줌
//    {
//        weaponData.Add("GreenMagician", WeaponType.Clown_Magician_Weapon);
//        weaponData.Add("FireMagician", WeaponType.Flame_Magician_Weapon);
//        weaponData.Add("IceMagician", WeaponType.Frozen_Witch_Weapon);
//        weaponData.Add("BuffMagician", WeaponType.Fairy_Green_Magician_Weapon);
//        weaponData.Add("DebuffMagician", WeaponType.Science_Wizard_Weapon);
//        weaponData.Add("BDMagician", WeaponType.Star_Magician_Weapon_2);

//    }

//    void SetWeapon(SkeletonAnimation skeletonAnimation, string weaponKey) // 무긱 착용 매서드
//    {
//        if (weaponData.TryGetValue(weaponKey, out WeaponType weaponType))
//        {
//            string weaponName = weaponType.ToString();
//            skeletonAnimation.skeleton.SetAttachment("Weapon", weaponName);
//        }
//        else
//        {
//            Debug.LogWarning("Weapon not found: " + weaponKey);
//        }
//    }
//}
