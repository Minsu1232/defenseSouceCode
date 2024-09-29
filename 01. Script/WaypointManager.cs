using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    public GameObject[] buildingPrefabs; // ������ �ǹ� ������ �迭
    public BuildingTaxManager[] taxManager;
    // �� ��������Ʈ�� �ǹ� ��ġ �迭
    public Transform[] position0; // ��������Ʈ 0�� ��ġ��
    public Transform[] position1; // ��������Ʈ 1�� ��ġ��
    public Transform[] position2; // ��������Ʈ 2�� ��ġ��
    public Transform[] position3; // ��������Ʈ 3�� ��ġ��
    public Transform[] position4; // ��������Ʈ 4�� ��ġ��
    public Transform[] position5; // ��������Ʈ 5�� ��ġ��
    public Transform[] position6; // ��������Ʈ 6�� ��ġ��
    public Transform[] position7; // ��������Ʈ 7�� ��ġ��
    public Transform[] position8; // ��������Ʈ 8�� ��ġ��
    public Transform[] position9; // ��������Ʈ 9�� ��ġ��
    public Transform[] position10; // ��������Ʈ 10�� ��ġ��
    public Transform[] position11; // ��������Ʈ 11�� ��ġ��
    public Transform[] position12; // ��������Ʈ 12�� ��ġ��

    private Dictionary<int, Transform[]> waypointBuildingPositions; // ��������Ʈ�� �ǹ� ���� ��ġ �迭

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ı�
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

                // �ǹ��� �����ϰ� �ش� ��ġ�� �ڽ����� ����
                GameObject buildingInstance = Instantiate(buildingPrefab, positions[clearCount - 1].position, Quaternion.identity);

                // ������ �ǹ��� �θ� �ش� ��ġ�� ����
                buildingInstance.transform.SetParent(positions[clearCount - 1]);

                // ��ġ�� �θ��� ���� ��ġ�� �ٽ� ���� (�ʿ信 ����)
                buildingInstance.transform.localPosition = Vector3.zero;

                Debug.Log($"Building placed at waypoint {waypointIndex}, position {clearCount - 1}");

                // �ǹ��� ��ġ�� �� �� ���� UI Ȱ��ȭ
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

    // �ش� ��������Ʈ�� �� ���� UI�� Ȱ��ȭ�ϴ� �Լ�
    private void ActivateCollectMoneyUI(int waypointIndex)
    {
        taxManager[waypointIndex].moneyReadyForWaypoint = true;
        taxManager[waypointIndex].collectMoneyUI.SetActive(true);
    }
}
