using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private bool isLoading = false; // 중복 로드를 막기 위한 플래그

    void Update()
    {
        // 마우스 클릭이나 터치 감지 및 씬이 아직 로드 중이 아닌 경우
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isLoading)
        {
            isLoading = true; // 중복 로드를 막기 위해 플래그 설정
            SceneLoader.Instance.LoadScene("PlayerScene");
        }
    }

}
