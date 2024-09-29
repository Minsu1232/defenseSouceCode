using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackHandler : MonoBehaviour
{   
    public static BackHandler instance;
    // �˾� �гε��� ������ ����Ʈ
    public List<GameObject> popupPanels = new List<GameObject>();

    // �ڷΰ��� ��ư�� �����ϴ� ����
    public Button backButton;

    void Start()
    {
        // �ڷΰ��� ��ư Ŭ�� �̺�Ʈ�� HandleBackButton �Լ� ����
        backButton.onClick.AddListener(HandleBackButton);
    }

    // �ڷΰ��� ��ư�� ������ �� ������ �Լ�
    public void HandleBackButton()
    {
        // Ȱ��ȭ�� �г��� �������� �ϳ��� ����
        for (int i = popupPanels.Count - 1; i >= 0; i--)
        {
            if (popupPanels[i].activeSelf)
            {
                popupPanels[i].SetActive(false); // �г��� ��Ȱ��ȭ
                return;  // �ϳ��� �гθ� �ݰ� ����
            }
        }

        // ��� �г��� �������� �ٸ� ���� (��: ���� �޴��� ���ư���)
        ReturnToPrevious();
    }

    // ��� �г��� �������� Ȯ���ϴ� �Լ� (�ɼ�)
    private bool AllPanelsClosed()
    {
        foreach (var panel in popupPanels)
        {
            if (panel.activeSelf)
                return false;  // �ϳ��� ���������� false
        }
        return true;  // ��� ���� ������ true
    }

    // ��� �г��� ���� �� ������ ����
    private void ReturnToPrevious()
    {
        Debug.Log("��� �г��� �������ϴ�. ���� �޴��� ���ư��ų� �ٸ� ������ �����ϼ���.");
        // ���� �޴��� ���ư��� ������ �߰��ϰų�, ���� ������ �� ����
        // ��: Application.Quit(); �Ǵ� SceneManager.LoadScene("MainMenu");
    }
}
