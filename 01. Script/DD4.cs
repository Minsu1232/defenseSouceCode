using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD4 : MonoBehaviour
{
    private static DD4 instance;

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
