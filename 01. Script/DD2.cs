using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD2 : MonoBehaviour
{
    private static DD2 instance;

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
}
