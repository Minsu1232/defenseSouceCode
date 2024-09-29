using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitClickHandler : MonoBehaviour
{
    public GameObject synthesisPanelPrefab; // �ռ� �г� ������
    private GameObject synthesisPanelInstance; // ������ �г� �ν��Ͻ�
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // ���� �г��� ���� ��� ����
        if (synthesisPanelInstance != null)
        {
            Destroy(synthesisPanelInstance);
        }

        // �г� �ν��Ͻ� ����
        synthesisPanelInstance = Instantiate(synthesisPanelPrefab, transform);

        // �г��� ��ġ�� ���� �Ӹ� ���� ���� (Z���� ����)
        Vector3 worldPosition = transform.position + new Vector3(0, 2f, 0); // ���� �Ӹ� ���� 2 ���� �ø�
        synthesisPanelInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

        // �г��� Z���� �����Ͽ� ȭ�鿡 �������� ���̰� ����
        synthesisPanelInstance.transform.localPosition = new Vector3(0, 2f, 0);

        // �г��� ��ư�� Ŭ�� �̺�Ʈ ����
        Button synthesisButton = synthesisPanelInstance.GetComponentInChildren<Button>();
        synthesisButton.onClick.RemoveAllListeners();
        synthesisButton.onClick.AddListener(OnSynthesizeClicked);
    }

    void OnSynthesizeClicked()
    {
        // �ռ� ��ư�� Ŭ���ϸ� �ռ� ������ �����մϴ�.
        HeroManager.Instance.TryCombineHero(gameObject);

        // �г��� �ٽ� ��Ȱ��ȭ�ϰų� �����մϴ�.
        Destroy(synthesisPanelInstance);
    }
}
