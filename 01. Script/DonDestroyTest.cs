using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonDestroyTest : MonoBehaviour
{
    private static DonDestroyTest instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 새로 생성된 오브젝트를 파괴
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeAndLoadData());
    }

    private IEnumerator InitializeAndLoadData()
    {
        // SaveLoadManager.Instance가 null이 아니게 될 때까지 대기
        while (SaveLoadManager.Instance == null)
        {
            yield return null; // 프레임 대기
        }

        if (SaveLoadManager.Instance != null)
        {
            // 여기서는 별도로 DontDestroyOnLoad를 호출할 필요가 없습니다.
            // 이미 Awake에서 처리되었습니다.
        }
        else
        {
            Debug.LogError("SaveLoadManager.Instance is still null after waiting. Cannot load player data.");
        }
    }
}
