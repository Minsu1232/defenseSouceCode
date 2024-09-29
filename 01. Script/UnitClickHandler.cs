using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitClickHandler : MonoBehaviour
{
    public GameObject synthesisPanelPrefab; // 합성 패널 프리팹
    private GameObject synthesisPanelInstance; // 생성된 패널 인스턴스
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // 기존 패널이 있을 경우 제거
        if (synthesisPanelInstance != null)
        {
            Destroy(synthesisPanelInstance);
        }

        // 패널 인스턴스 생성
        synthesisPanelInstance = Instantiate(synthesisPanelPrefab, transform);

        // 패널의 위치를 유닛 머리 위로 설정 (Z축을 고정)
        Vector3 worldPosition = transform.position + new Vector3(0, 2f, 0); // 유닛 머리 위로 2 유닛 올림
        synthesisPanelInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

        // 패널의 Z축을 고정하여 화면에 정면으로 보이게 설정
        synthesisPanelInstance.transform.localPosition = new Vector3(0, 2f, 0);

        // 패널의 버튼에 클릭 이벤트 연결
        Button synthesisButton = synthesisPanelInstance.GetComponentInChildren<Button>();
        synthesisButton.onClick.RemoveAllListeners();
        synthesisButton.onClick.AddListener(OnSynthesizeClicked);
    }

    void OnSynthesizeClicked()
    {
        // 합성 버튼을 클릭하면 합성 로직을 실행합니다.
        HeroManager.Instance.TryCombineHero(gameObject);

        // 패널을 다시 비활성화하거나 제거합니다.
        Destroy(synthesisPanelInstance);
    }
}
