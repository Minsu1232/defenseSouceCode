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
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �����ϴ� ��� ���� ������ ������Ʈ�� �ı�
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeAndLoadData());
    }

    private IEnumerator InitializeAndLoadData()
    {
        // SaveLoadManager.Instance�� null�� �ƴϰ� �� ������ ���
        while (SaveLoadManager.Instance == null)
        {
            yield return null; // ������ ���
        }

        if (SaveLoadManager.Instance != null)
        {
            // ���⼭�� ������ DontDestroyOnLoad�� ȣ���� �ʿ䰡 �����ϴ�.
            // �̹� Awake���� ó���Ǿ����ϴ�.
        }
        else
        {
            Debug.LogError("SaveLoadManager.Instance is still null after waiting. Cannot load player data.");
        }
    }
}
