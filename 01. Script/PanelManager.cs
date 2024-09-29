using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{

    public GameObject gradePanel;
    public GameObject coinShopPanel;
    public GameObject commbinPanel;
    // �г��� ����ϴ� �ۺ� �޼���
    public void ToggleGradePanel()
    {
        // �г��� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ�ϰ�, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ
        gradePanel.SetActive(!gradePanel.activeSelf);
    }

    public void ToggleCoinShopPanel()
    {
        // �г��� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ�ϰ�, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ
        coinShopPanel.SetActive(!coinShopPanel.activeSelf);
    }
    public void ToggleCommbinPanel()
    {
        // �г��� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ�ϰ�, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ
        commbinPanel.SetActive(!commbinPanel.activeSelf);
    }
}
