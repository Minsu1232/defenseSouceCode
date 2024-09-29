using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackHandler : MonoBehaviour
{   
    public static BackHandler instance;
    // 팝업 패널들을 관리할 리스트
    public List<GameObject> popupPanels = new List<GameObject>();

    // 뒤로가기 버튼을 참조하는 변수
    public Button backButton;

    void Start()
    {
        // 뒤로가기 버튼 클릭 이벤트에 HandleBackButton 함수 연결
        backButton.onClick.AddListener(HandleBackButton);
    }

    // 뒤로가기 버튼을 눌렀을 때 실행할 함수
    public void HandleBackButton()
    {
        // 활성화된 패널을 역순으로 하나씩 닫음
        for (int i = popupPanels.Count - 1; i >= 0; i--)
        {
            if (popupPanels[i].activeSelf)
            {
                popupPanels[i].SetActive(false); // 패널을 비활성화
                return;  // 하나의 패널만 닫고 종료
            }
        }

        // 모든 패널이 닫혔으면 다른 동작 (예: 메인 메뉴로 돌아가기)
        ReturnToPrevious();
    }

    // 모든 패널이 닫혔는지 확인하는 함수 (옵션)
    private bool AllPanelsClosed()
    {
        foreach (var panel in popupPanels)
        {
            if (panel.activeSelf)
                return false;  // 하나라도 열려있으면 false
        }
        return true;  // 모두 닫혀 있으면 true
    }

    // 모든 패널이 닫힌 후 실행할 동작
    private void ReturnToPrevious()
    {
        Debug.Log("모든 패널이 닫혔습니다. 메인 메뉴로 돌아가거나 다른 동작을 실행하세요.");
        // 메인 메뉴로 돌아가는 동작을 추가하거나, 앱을 종료할 수 있음
        // 예: Application.Quit(); 또는 SceneManager.LoadScene("MainMenu");
    }
}
