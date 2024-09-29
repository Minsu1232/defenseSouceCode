
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WeaponTransform : MonoBehaviour
//{
//    //public SkeletonAnimation skeletonAnimation; // ���� ������

//    enum WeaponType // �������� ����� ���� ����
//    {
//        Clown_Magician_Weapon,
//        Frozen_Witch_Weapon,
//        Fairy_Green_Magician_Weapon,
//        Science_Wizard_Weapon,
//        Star_Magician_Weapon_2,
//        Flame_Magician_Weapon
//    }

//    private Dictionary<string, WeaponType> weaponData = new Dictionary<string, WeaponType>(); // �������� �� ���⸦ ã�� ���� ��ųʸ�

//    void Start()
//    {
//        // ������ �ʱ�ȭ
//        InitializeWeaponData();

//        // �� SkeletonAnimation�� ���� ����       
//        string weaponKey = skeletonAnimation.tag;  // ���÷� name ���
//        SetWeapon(skeletonAnimation, weaponKey);

//    }

//    void InitializeWeaponData() // ������ ������ �±׸� ���� ���⸦ �����
//    {
//        weaponData.Add("GreenMagician", WeaponType.Clown_Magician_Weapon);
//        weaponData.Add("FireMagician", WeaponType.Flame_Magician_Weapon);
//        weaponData.Add("IceMagician", WeaponType.Frozen_Witch_Weapon);
//        weaponData.Add("BuffMagician", WeaponType.Fairy_Green_Magician_Weapon);
//        weaponData.Add("DebuffMagician", WeaponType.Science_Wizard_Weapon);
//        weaponData.Add("BDMagician", WeaponType.Star_Magician_Weapon_2);

//    }

//    void SetWeapon(SkeletonAnimation skeletonAnimation, string weaponKey) // ���� ���� �ż���
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
