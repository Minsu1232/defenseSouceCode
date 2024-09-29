using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalWarrior : Warrior
{
    public GameObject swordSlash; // 스킬 프리팹
    public GameObject manaSkillPrefab;
    public GameObject swordAura; // 결계의 프리팹
    public GameObject sky; // 검기가날아갈 위치
    public GameObject swordCrack; //칼이 떨어진 곳 금 프리팹
    public Transform[] dropPoints; // 칼이 떨어질 수 있는 지정된 포인트들
    

    //public GameObject sky; // 목표 지점 (sky)
    //public GameObject meteorPrefab; // 운석 프리팹
    public Vector3 meteorDropPosition; // 운석이 떨어질 위치
    public Vector3 meteorStartPosition;
    //public GameObject swordPrefab; // 검기 프리팹
    // Start is called before the first frame update
    private void OnEnable()
    {   Vector3 vector3 = gameObject.transform.position;
        vector3.y += 28f; 
        Instantiate(sky,vector3, Quaternion.identity);
    }
    protected override void Start()
    {
        Vector3 vector = new Vector3(-5.2f, -2f, 0f);
        GameObject newDropPoint = new GameObject("DropPoint");
        newDropPoint.transform.position = vector;
        Vector3 vector1 = new Vector3(-5.2f, 2.8f, 0f);
        GameObject newDropPoint1 = new GameObject("DropPoint");
        newDropPoint1.transform.position = vector1;
        Vector3 vector2 = new Vector3(5.2f, -2f, 0f);
        GameObject newDropPoint2 = new GameObject("DropPoint");
        newDropPoint2.transform.position = vector2;
        Vector3 vector3 = new Vector3(5.2f, 2.8f, 0f);
        GameObject newDropPoint3 = new GameObject("DropPoint");
        newDropPoint3.transform.position = vector3;

        dropPoints[0] = newDropPoint.transform;
        dropPoints[1] = newDropPoint1.transform;
        dropPoints[2] = newDropPoint2.transform;
        dropPoints[3] = newDropPoint3.transform;

        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new PrimevalWarriorSkill
            {
                skillName = "혈겁만파",
                skillDescription = "신의 힘이 담긴 칼을 불러 옵니다.",
                skillDamage = characterData.attackPower * 5,
                skillRange = 3.0f, // 범위 설정 (예: 5.0f)
                skillProbability =0.25f,
                skillPrefab = swordSlash, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.2f,
                //barrierPrefab = barrierPrefab,
                dropPoints = dropPoints,
                swordCrack = swordCrack

            },
            new PrimevalWarriorManaSkill
            {
                skillName = "천상강림",
                skillDescription = "하늘을 갈라 유성을 떨어뜨립니다.",
                meteorPrefab = manaSkillPrefab, // 유성
                skillPrefab = swordAura,
                sky = sky,
                meteorDropPosition = meteorDropPosition,
                //meteorStartPosition =  meteorStartPosition,
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                mpBar = mpBar // UI에서 할당된 MP 바 이미지
            }
            // 추가 스킬 초기화
        };
    }
}
