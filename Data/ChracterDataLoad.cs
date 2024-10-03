using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// Unitdata.CSV에선 베이스류와 일반공격 프리팹만 관리
/// </summary>
public class ChracterDataLoad : MonoBehaviour
{
    // 기존에 생성된 ScriptableObject 리스트 (에디터에서 할당)
    public List<CharacterData> characterDataList;

    // 암호화/복호화 메서드 (XOR 방식)  

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        // 게임 시작 시 캐릭터 데이터를 CSV에서 로드
        await LoadCharacterDataFromCSV();
    }
    // 캐릭터 데이터를 비동기로 CSV에서 불러오고, Addressables로 프리팹을 할당하는 메서드
    public async Task LoadCharacterDataFromCSV()
    {
       
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/Unitdata_Version_Test");

        string csvText = csvFile.text;


        string[] lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue; // 빈 줄 무시

            string[] fields = line.Split(',');

            // CSV에서 ID를 읽어옴 (Trim 추가)
            if (!int.TryParse(fields[0].Trim(), out int id))  // ID가 숫자 형식인지 확인
            {
                continue;  // 잘못된 데이터는 스킵
            }

            // ID에 맞는 캐릭터 데이터를 찾음
            CharacterData character = GetCharacterDataByID(id);

            if (character != null)
            {
                // CSV 데이터를 기존 ScriptableObject에 적용 (베이스 값만 적용)
                character.heroName = fields[1].Trim();

                // 여러 필드를 한번에 파싱하고 확인
                if (!float.TryParse(fields[2].Trim(), out character.baseAttackPower) ||
                    !float.TryParse(fields[3].Trim(), out character.baseAttackSpeed) ||
                    !float.TryParse(fields[4].Trim(), out character.baseAttackRange) ||
                    !float.TryParse(fields[5].Trim(), out character.baseCriticalChance))
                {
                    continue; // 값이 올바르지 않으면 스킵
                }

                // AttackPrefab을 Addressables로 로드
                await LoadCharacterAttackPrefab(character, fields[6].Trim());
            }
            else
            {
                Debug.LogError($"ID가 {id}인 캐릭터 데이터를 찾을 수 없습니다.");
            }
        }
        
    }

    // AttackPrefab만 Addressables로 로드하는 메서드
    private async Task LoadCharacterAttackPrefab(CharacterData character, string attackPrefabName)
    {
        if (attackPrefabName == "NoPrefab") // 만약 NoPrefab이면 스킵
        {
            character.attackPrefab = null;
            Debug.Log($"{character.heroName}은 AttackPrefab이 없습니다.");
            return;
        }

        // AttackPrefab 로드
        AsyncOperationHandle<GameObject> attackPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{attackPrefabName}.prefab");
        await attackPrefabHandle.Task;

        if (attackPrefabHandle.Status == AsyncOperationStatus.Succeeded)
        {
            character.attackPrefab = attackPrefabHandle.Result;
            Debug.Log($"캐릭터 {character.heroName}의 공격 프리팹 {attackPrefabName} 로드 완료.");
        }
        else
        {
            Debug.LogError($"캐릭터 {character.heroName}의 공격 프리팹 {attackPrefabName} 로드 실패.");
        }
    }

    // ID로 캐릭터 데이터를 가져오는 메서드
    private CharacterData GetCharacterDataByID(int id)
    {
        foreach (var character in characterDataList)
        {
            if (character.id == id) // ID로 비교
            {
                return character;
            }
        }

        Debug.LogError($"ID가 {id}인 캐릭터 데이터를 찾을 수 없습니다.");
        return null;
    }
}
