using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    public GameObject[] buildingPrefabs; // 생성할 건물 프리팹 배열
    public BuildingTaxManager[] taxManager;
    // 각 웨이포인트별 건물 위치 배열
    public Transform[] position0; // 웨이포인트 0의 위치들
    public Transform[] position1; // 웨이포인트 1의 위치들
    public Transform[] position2; // 웨이포인트 2의 위치들
    public Transform[] position3; // 웨이포인트 3의 위치들
    public Transform[] position4; // 웨이포인트 4의 위치들
    public Transform[] position5; // 웨이포인트 5의 위치들
    public Transform[] position6; // 웨이포인트 6의 위치들
    public Transform[] position7; // 웨이포인트 7의 위치들
    public Transform[] position8; // 웨이포인트 8의 위치들
    public Transform[] position9; // 웨이포인트 9의 위치들
    public Transform[] position10; // 웨이포인트 10의 위치들
    public Transform[] position11; // 웨이포인트 11의 위치들
    public Transform[] position12; // 웨이포인트 12의 위치들

    private Dictionary<int, Transform[]> waypointBuildingPositions; // 웨이포인트별 건물 생성 위치 배열

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 파괴
        }
    }

    private void Start()
    {
        InitializeBuildingPositions();
    }

    private void InitializeBuildingPositions()
    {
        waypointBuildingPositions = new Dictionary<int, Transform[]>
        {
            { 0, position0 },
            { 1, position1 },
            { 2, position2 },
            { 3, position3 },
            { 4, position4 },
            { 5, position5 },
            { 6, position6 },
            { 7, position7 },
            { 8, position8 },
            { 9, position9 },
            { 10, position10 },
            { 11, position11 },
            { 12, position12 }
        };
    }

    public void PlaceBuildingAtWaypoint(int waypointIndex, int clearCount)
    {
        if (waypointBuildingPositions.ContainsKey(waypointIndex))
        {
            var positions = waypointBuildingPositions[waypointIndex];
            if (clearCount > 0 && clearCount <= positions.Length && clearCount <= buildingPrefabs.Length)
            {
                GameObject buildingPrefab = buildingPrefabs[clearCount - 1];

                // 건물을 생성하고 해당 위치의 자식으로 설정
                GameObject buildingInstance = Instantiate(buildingPrefab, positions[clearCount - 1].position, Quaternion.identity);

                // 생성된 건물의 부모를 해당 위치로 설정
                buildingInstance.transform.SetParent(positions[clearCount - 1]);

                // 위치를 부모의 로컬 위치로 다시 설정 (필요에 따라)
                buildingInstance.transform.localPosition = Vector3.zero;

                Debug.Log($"Building placed at waypoint {waypointIndex}, position {clearCount - 1}");

                // 건물이 배치된 후 돈 수거 UI 활성화
                ActivateCollectMoneyUI(waypointIndex);
            }
            else
            {
                Debug.LogError($"Clear count {clearCount} is out of range for waypoint {waypointIndex}");
            }
        }
        else
        {
            Debug.LogError($"No building positions defined for waypoint {waypointIndex}");
        }
    }

    // 해당 웨이포인트의 돈 수거 UI를 활성화하는 함수
    private void ActivateCollectMoneyUI(int waypointIndex)
    {
        taxManager[waypointIndex].moneyReadyForWaypoint = true;
        taxManager[waypointIndex].collectMoneyUI.SetActive(true);
    }
}
