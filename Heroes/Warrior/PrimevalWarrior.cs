using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    private SkillLoader skillLoader;
    private AsyncOperationHandle<GameObject> skillHandle1;
    private AsyncOperationHandle<GameObject> skillHandle2;
    private void OnEnable()
    {   Vector3 vector3 = gameObject.transform.position;
        vector3.y += 28f; 
        Instantiate(sky,vector3, Quaternion.identity);
    }
   
    protected override async void Start()
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
        base.Start();
        skillLoader = gameObject.AddComponent<SkillLoader>();

        // 첫 번째 스킬 로드
        Skill skillData = await skillLoader.LoadSkillFromCSV(16);
        if (skillData != null)
        {
            PrimevalWarriorSkill baseSkill = new PrimevalWarriorSkill(skillData);
            skills.Add(baseSkill);

            // 첫 번째 스킬 핸들 저장
            skillHandle1 = skillLoader.GetCurrentHandle();
        }
        else
        {
            Debug.LogError("첫 번째 스킬을 로드할 수 없습니다.");
        }

        // 두 번째 스킬 로드
        Skill manaSkillData = await skillLoader.LoadSkillFromCSV(17);
        if (manaSkillData != null)
        {
            PrimevalWarriorManaSkill manaSkill = new PrimevalWarriorManaSkill(manaSkillData);
            skills.Add(manaSkill);

            // 두 번째 스킬 핸들 저장
            skillHandle2 = skillLoader.GetCurrentHandle();
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
