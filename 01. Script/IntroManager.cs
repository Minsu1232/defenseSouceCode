using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private bool isLoading = false; // �ߺ� �ε带 ���� ���� �÷���

    void Update()
    {
        // ���콺 Ŭ���̳� ��ġ ���� �� ���� ���� �ε� ���� �ƴ� ���
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isLoading)
        {
            isLoading = true; // �ߺ� �ε带 ���� ���� �÷��� ����
            SceneLoader.Instance.LoadScene("PlayerScene");
        }
    }

}
