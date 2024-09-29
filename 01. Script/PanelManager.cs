using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{

    public GameObject gradePanel;
    public GameObject coinShopPanel;
    public GameObject commbinPanel;
    // 패널을 토글하는 퍼블릭 메서드
    public void ToggleGradePanel()
    {
        // 패널이 활성화되어 있으면 비활성화하고, 비활성화되어 있으면 활성화
        gradePanel.SetActive(!gradePanel.activeSelf);
    }

    public void ToggleCoinShopPanel()
    {
        // 패널이 활성화되어 있으면 비활성화하고, 비활성화되어 있으면 활성화
        coinShopPanel.SetActive(!coinShopPanel.activeSelf);
    }
    public void ToggleCommbinPanel()
    {
        // 패널이 활성화되어 있으면 비활성화하고, 비활성화되어 있으면 활성화
        commbinPanel.SetActive(!commbinPanel.activeSelf);
    }
}
