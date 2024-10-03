using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// Skills.CSV에서 스킬의 모든 데이터를 관리
/// </summary>
public class SkillLoader : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> handle;
    private AsyncOperationHandle<GameObject> otherHandle;
  
    // 여러 프리팹을 사용하는 스킬 처리
    public async Task<Skill> LoadSkillFromCSV(int skillID)
    {
        // 암호화된 CSV 파일을 불러옴
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/Skills_Version_Test");
        string csvText = csvFile.text;
       
        string[] lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue; // 빈 줄이 있으면 스킵

            // 쉼표를 기준으로 필드들을 나눔 (필요 시 공백 제거)
            string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

            // ID가 일치하는 스킬을 찾았을 경우
            if (int.TryParse(fields[0].Trim(), out int parsedID) && parsedID == skillID)
            {
                // 스킬 생성 및 기본 정보 할당
                Skill skill = new Skill
                {
                    ID = parsedID,
                    skillName = fields[1].Trim(),
                    skillDescription = fields[2].Trim(),
                    skillDamage = TryParseFloat(fields[3].Trim(), 0f),
                    skillRange = TryParseFloat(fields[4].Trim(), 0f),
                    skillProbability = TryParseFloat(fields[5].Trim(), 0f),
                    isSingtarget = TryParseBool(fields[7].Trim(), false),
                    hasSlowEffect = TryParseBool(fields[8].Trim(), false),
                    slowAmount = TryParseFloat(fields[9].Trim(), 0f),
                    hasDefenseReduction = TryParseBool(fields[10].Trim(), false),
                    defenseReductionAmount = TryParseFloat(fields[11].Trim(), 0f),
                    isSpecialSkill = TryParseBool(fields[12].Trim(), false),
                    duration = TryParseFloat(fields[13].Trim(), 0f),
                    speed = TryParseFloat(fields[14].Trim(), 0f),
                    manaCost = TryParseFloat(fields[15].Trim(), 0f),
                    manaRegenRate = TryParseFloat(fields[16].Trim(), 0f),
                    maxMana = TryParseFloat(fields[17].Trim(), 0f),
                    manaGainPerAttack = TryParseFloat(fields[18].Trim(), 0f),
                    cloneDuration = TryParseFloat(fields[19].Trim(), 0f),
                    clonePowerMultiplier = TryParseFloat(fields[20].Trim(), 0f),
                };

                Debug.Log($"Prefab data: {fields[6]}"); // Prefab data 확인

                // 프리팹이 쉼표로 구분된 여러 개일 경우 배열로 처리
                if (fields[6].Contains("|"))
                {
                    Debug.Log("Multiple prefabs detected.");
                    return await LoadMultiplePrefabs(skill, fields[6].Trim());
                }
                else
                {
                    Debug.Log("Single prefab detected.");
                    // 단일 프리팹 로드 및 추가 프리팹 로드
                    return await LoadSinglePrefab(skill, fields[6].Trim(), fields[21].Trim());
                }
            }
        }

        Debug.LogError("Skill with ID " + skillID + " not found in CSV.");
        return null;
    }

    // 단일 프리팹 로드
    private async Task<Skill> LoadSinglePrefab(Skill skill, string prefabName, string prefabName2)
    {
        // 경로 유효성 검사
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("Prefab name is missing or empty. Cannot load prefab.");
            return null;
        }
        if (!string.IsNullOrEmpty(prefabName))
        {
            Debug.Log(skill.ID);
            // 공백 제거
            prefabName = prefabName.Trim();

            // 첫 번째 프리팹 로드
            Debug.Log($"Loading Prefab: Assets/Resources_moved/Prefabs/{prefabName}.prefab");
            handle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{prefabName}.prefab");

            await handle.Task;

            // 첫 번째 프리팹이 성공적으로 로드되었는지 확인
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                // Skill 객체에 프리팹 할당
                skill.skillPrefab = handle.Result;
                Debug.Log($"Prefab {prefabName} loaded successfully.");
            }
            else
            {
                Debug.LogError($"Failed to load prefab: {prefabName}");
                return null;
            }
        }

        
        // 두 번째 프리팹 로드 (필요할 경우)
        if (!string.IsNullOrEmpty(prefabName2) && prefabName2.Trim() != "NoPrefab")
        {
            Debug.Log($"Prefab Name 2: {prefabName2} (ID: {skill.ID})"); // 디버깅 로그로 prefabName2 확인
            prefabName2 = prefabName2.Trim();

            Debug.Log($"Loading Second Prefab: Assets/Resources_moved/Prefabs/{prefabName2}.prefab");
            otherHandle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{prefabName2}.prefab");
            await otherHandle.Task;

            if (otherHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                // 추가적인 두 번째 프리팹 처리가 필요하다면 여기에 로직 추가
                skill.otherSkillPrefab = otherHandle.Result;
                Debug.Log($"Second Prefab {prefabName2} loaded successfully.");
            }
            else
            {
                Debug.LogError($"Failed to load second prefab: {prefabName2}");
            }
        }
        else
        {
            Debug.Log($"No valid second prefab found for skill ID: {skill.ID}");
        }

        return skill;  // 스킬에 프리팹이 추가된 상태로 반환
    }

    // 다중 프리팹 로드
    private async Task<Skill> LoadMultiplePrefabs(Skill skill, string prefabNames)
    {
        string[] prefabArray = prefabNames.Split('|');
        
        List<GameObject> loadedPrefabs = new List<GameObject>();
        Debug.Log(skill.ID);
        foreach (string prefabName in prefabArray)
        {
            Debug.Log(prefabName);
            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{prefabName}.prefab");
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                skill.skillArray.Add(handle.Result);

            }
            else
            {
                Debug.LogError($"프리팹 {prefabName} 로드 실패");
            }
            
        }

       
        return skill;
    }

    // 현재 핸들 반환
    public AsyncOperationHandle<GameObject> GetCurrentHandle()
    {
        return handle;
    }
    public AsyncOperationHandle<GameObject> GetCurrentOtherHandle()
    {
        return otherHandle;
    }

    // 숫자 변환 예외 처리
    private float TryParseFloat(string input, float defaultValue)
    {
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    // Bool 변환 예외 처리
    private bool TryParseBool(string input, bool defaultValue)
    {
        if (bool.TryParse(input, out bool result))
        {
            return result;
        }
        return defaultValue;
    }
}
