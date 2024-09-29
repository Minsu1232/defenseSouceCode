using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPage : MonoBehaviour
{
    public GameObject page1; // 1������ GameObject
    public GameObject page2; // 2������ GameObject

    private int currentPage = 1; // ���� �������� �����ϴ� ����

    void Start()
    {
        // �ʱ� �������� 1�������� Ȱ��ȭ�ϰ� 2�������� ��Ȱ��ȭ
        ShowPage1();
    }

    public void ShowPage1()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        currentPage = 1;
    }

    public void ShowPage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        currentPage = 2;
    }

    public void Next()
    {
        if (currentPage == 1)
        {
            ShowPage2();
        }
    }

    public void Previous()
    {
        if (currentPage == 2)
        {
            ShowPage1();
        }
    }
}
