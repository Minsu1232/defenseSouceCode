using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HeroCombineManager : MonoBehaviour
{
    //public List<Toggle> heroToggles; // ������ ������ �� �ִ� Toggle ����Ʈ
    //public List<GameObject> summonedHeroes; // ��ȯ�� ���� ������Ʈ ����Ʈ
    //private List<GameObject> selectedHeroes = new List<GameObject>(); // ���õ� �������� ������ ����Ʈ
    //public Button combineButton; // �ռ� ��ư

    //void Start()
    //{
    //    // �ռ� ��ư�� Ŭ�� �̺�Ʈ ����
    //    combineButton.onClick.AddListener(OnCombineButtonClicked);

    //    // �� Toggle�� OnValueChanged �̺�Ʈ�� �޼��� ����
    //    foreach (Toggle toggle in heroToggles)
    //    {
    //        toggle.onValueChanged.AddListener(delegate { OnHeroToggleChanged(toggle); });
    //    }
    //}

    //public void OnHeroToggleChanged(Toggle changedToggle)
    //{
    //    selectedHeroes.Clear();

    //    // ���õ� Toggle�� ������� ���� ����
    //    foreach (var toggle in heroToggles)
    //    {
    //        if (toggle.isOn)
    //        {
    //            int toggleIndex = heroToggles.IndexOf(toggle); // ����� �ε����� ����
    //            if (toggleIndex >= 0 && toggleIndex < summonedHeroes.Count)
    //            {
    //                selectedHeroes.Add(summonedHeroes[toggleIndex]); // �ش� �ε����� ������ ���� ��Ͽ� �߰�
    //            }
    //        }
    //    }

    //    // �ռ� ���� ���� Ȯ�� (�� ������ ���õǾ����� Ȯ��)
    //    combineButton.interactable = selectedHeroes.Count == 3;
    //}

    //void OnCombineButtonClicked()
    //{
    //    if (selectedHeroes.Count == 3)
    //    {
    //        // �� ������ �����Ͽ� �ռ� �õ�
    //        GameObject hero1 = selectedHeroes[0];
    //        GameObject hero2 = selectedHeroes[1];
    //        GameObject hero3 = selectedHeroes[2];

    //        HeroManager.Instance.CombineHeroes(hero1, hero2, hero3);

    //        // ���� �ʱ�ȭ
    //        foreach (var toggle in heroToggles)
    //        {
    //            toggle.isOn = false;
    //        }
    //        selectedHeroes.Clear();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("You need to select exactly 3 heroes to combine.");
    //    }
    //}
}
