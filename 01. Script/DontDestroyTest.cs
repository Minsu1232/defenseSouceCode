using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyTest : MonoBehaviour
{
    private static DontDestroyTest instance;

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
