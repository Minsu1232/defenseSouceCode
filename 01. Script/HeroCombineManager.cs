using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HeroCombineManager : MonoBehaviour
{
    //public List<Toggle> heroToggles; // 영웅을 선택할 수 있는 Toggle 리스트
    //public List<GameObject> summonedHeroes; // 소환된 영웅 오브젝트 리스트
    //private List<GameObject> selectedHeroes = new List<GameObject>(); // 선택된 영웅들을 저장할 리스트
    //public Button combineButton; // 합성 버튼

    //void Start()
    //{
    //    // 합성 버튼에 클릭 이벤트 연결
    //    combineButton.onClick.AddListener(OnCombineButtonClicked);

    //    // 각 Toggle의 OnValueChanged 이벤트에 메서드 연결
    //    foreach (Toggle toggle in heroToggles)
    //    {
    //        toggle.onValueChanged.AddListener(delegate { OnHeroToggleChanged(toggle); });
    //    }
    //}

    //public void OnHeroToggleChanged(Toggle changedToggle)
    //{
    //    selectedHeroes.Clear();

    //    // 선택된 Toggle을 기반으로 영웅 선택
    //    foreach (var toggle in heroToggles)
    //    {
    //        if (toggle.isOn)
    //        {
    //            int toggleIndex = heroToggles.IndexOf(toggle); // 토글의 인덱스를 얻음
    //            if (toggleIndex >= 0 && toggleIndex < summonedHeroes.Count)
    //            {
    //                selectedHeroes.Add(summonedHeroes[toggleIndex]); // 해당 인덱스의 영웅을 선택 목록에 추가
    //            }
    //        }
    //    }

    //    // 합성 가능 여부 확인 (세 영웅이 선택되었는지 확인)
    //    combineButton.interactable = selectedHeroes.Count == 3;
    //}

    //void OnCombineButtonClicked()
    //{
    //    if (selectedHeroes.Count == 3)
    //    {
    //        // 세 영웅을 선택하여 합성 시도
    //        GameObject hero1 = selectedHeroes[0];
    //        GameObject hero2 = selectedHeroes[1];
    //        GameObject hero3 = selectedHeroes[2];

    //        HeroManager.Instance.CombineHeroes(hero1, hero2, hero3);

    //        // 선택 초기화
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
