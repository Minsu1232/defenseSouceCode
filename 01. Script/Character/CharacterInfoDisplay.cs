using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfoDisplay : MonoBehaviour
{

    public CharacterInfoPanel infoPanel; // 캐릭터 정보를 표시할 패널
    public CharacterInfo characterInfo; // 캐릭터의 정보를 가져오는 CharacterInfo
    public bool isPanelVisible = false; // 패널이 표시되고 있는지 확인하는 변수

    public GraphicRaycaster uiRaycaster; // UI 요소를 감지할 GraphicRaycaster
    public EventSystem eventSystem; // 입력 이벤트를 처리할 EventSystem

    private void Update()
    {
        // 마우스 클릭 입력 처리(유니티 에디터에서 테스트할 때)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            // UI 요소에 대한 레이캐스트 처리
            if (!IsPointerOverUI(mousePosition))
            {
                HandleTouchOrClick(mousePosition); // UI 요소가 아닌 오브젝트에 대한 레이캐스트 처리
            }
        }
    }

    private bool IsPointerOverUI(Vector3 inputPosition)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);

        return results.Count > 0; // UI 요소를 클릭했는지 여부 반환
    }

    private void HandleTouchOrClick(Vector3 inputPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // Raycast가 히트한 오브젝트 로그 출력

            if (hit.collider.gameObject == gameObject && characterInfo != null)
            {
                // 이미 패널이 보이고 있는 경우
                if (isPanelVisible)
                {
                    infoPanel.HideCharacterInfo(); // 패널 숨김
                    isPanelVisible = false; // 패널 비활성화 상태로 전환
                    Debug.Log("Panel hidden");
                }
                else
                {
                    // 새로운 유닛을 클릭했으므로 패널 업데이트 및 표시
                    infoPanel.UpdateCharacterInfo(characterInfo.characterData, characterInfo.skills);
                    infoPanel.gameObject.SetActive(true);  // 패널을 강제로 활성화
                    isPanelVisible = true; // 패널이 보이도록 상태 변경
                    Debug.Log("Panel shown for character: " + characterInfo.characterData.heroName);
                }
            }
        }
        else
        {
            // UI가 감지되지 않았을 때만 패널을 숨김
            if (!IsPointerOverUI(inputPosition))
            {
                infoPanel.HideCharacterInfo(); // 패널 숨김
                isPanelVisible = false; // 패널 비활성화 상태로 전환
                Debug.LogWarning("Raycast did not hit any collider and no UI element was clicked.");
            }
        }
    }
}
