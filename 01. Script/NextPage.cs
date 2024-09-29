using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPage : MonoBehaviour
{
    public GameObject page1; // 1페이지 GameObject
    public GameObject page2; // 2페이지 GameObject

    private int currentPage = 1; // 현재 페이지를 저장하는 변수

    void Start()
    {
        // 초기 설정으로 1페이지를 활성화하고 2페이지는 비활성화
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
