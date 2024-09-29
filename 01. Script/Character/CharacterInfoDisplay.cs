using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfoDisplay : MonoBehaviour
{

    public CharacterInfoPanel infoPanel; // ĳ���� ������ ǥ���� �г�
    public CharacterInfo characterInfo; // ĳ������ ������ �������� CharacterInfo
    public bool isPanelVisible = false; // �г��� ǥ�õǰ� �ִ��� Ȯ���ϴ� ����

    public GraphicRaycaster uiRaycaster; // UI ��Ҹ� ������ GraphicRaycaster
    public EventSystem eventSystem; // �Է� �̺�Ʈ�� ó���� EventSystem

    private void Update()
    {
        // ���콺 Ŭ�� �Է� ó��(����Ƽ �����Ϳ��� �׽�Ʈ�� ��)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            // UI ��ҿ� ���� ����ĳ��Ʈ ó��
            if (!IsPointerOverUI(mousePosition))
            {
                HandleTouchOrClick(mousePosition); // UI ��Ұ� �ƴ� ������Ʈ�� ���� ����ĳ��Ʈ ó��
            }
        }
    }

    private bool IsPointerOverUI(Vector3 inputPosition)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);

        return results.Count > 0; // UI ��Ҹ� Ŭ���ߴ��� ���� ��ȯ
    }

    private void HandleTouchOrClick(Vector3 inputPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // Raycast�� ��Ʈ�� ������Ʈ �α� ���

            if (hit.collider.gameObject == gameObject && characterInfo != null)
            {
                // �̹� �г��� ���̰� �ִ� ���
                if (isPanelVisible)
                {
                    infoPanel.HideCharacterInfo(); // �г� ����
                    isPanelVisible = false; // �г� ��Ȱ��ȭ ���·� ��ȯ
                    Debug.Log("Panel hidden");
                }
                else
                {
                    // ���ο� ������ Ŭ�������Ƿ� �г� ������Ʈ �� ǥ��
                    infoPanel.UpdateCharacterInfo(characterInfo.characterData, characterInfo.skills);
                    infoPanel.gameObject.SetActive(true);  // �г��� ������ Ȱ��ȭ
                    isPanelVisible = true; // �г��� ���̵��� ���� ����
                    Debug.Log("Panel shown for character: " + characterInfo.characterData.heroName);
                }
            }
        }
        else
        {
            // UI�� �������� �ʾ��� ���� �г��� ����
            if (!IsPointerOverUI(inputPosition))
            {
                infoPanel.HideCharacterInfo(); // �г� ����
                isPanelVisible = false; // �г� ��Ȱ��ȭ ���·� ��ȯ
                Debug.LogWarning("Raycast did not hit any collider and no UI element was clicked.");
            }
        }
    }
}
