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
/// Skills.CSV���� ��ų�� ��� �����͸� ����
/// </summary>
public class SkillLoader : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> handle;
    private AsyncOperationHandle<GameObject> otherHandle;
  
    // ���� �������� ����ϴ� ��ų ó��
    public async Task<Skill> LoadSkillFromCSV(int skillID)
    {
        // ��ȣȭ�� CSV ������ �ҷ���
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/Skills_Version_Test");
        string csvText = csvFile.text;
       
        string[] lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue; // �� ���� ������ ��ŵ

            // ��ǥ�� �������� �ʵ���� ���� (�ʿ� �� ���� ����)
            string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

            // ID�� ��ġ�ϴ� ��ų�� ã���� ���
            if (int.TryParse(fields[0].Trim(), out int parsedID) && parsedID == skillID)
            {
                // ��ų ���� �� �⺻ ���� �Ҵ�
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

                Debug.Log($"Prefab data: {fields[6]}"); // Prefab data Ȯ��

                // �������� ��ǥ�� ���е� ���� ���� ��� �迭�� ó��
                if (fields[6].Contains("|"))
                {
                    Debug.Log("Multiple prefabs detected.");
                    return await LoadMultiplePrefabs(skill, fields[6].Trim());
                }
                else
                {
                    Debug.Log("Single prefab detected.");
                    // ���� ������ �ε� �� �߰� ������ �ε�
                    return await LoadSinglePrefab(skill, fields[6].Trim(), fields[21].Trim());
                }
            }
        }

        Debug.LogError("Skill with ID " + skillID + " not found in CSV.");
        return null;
    }

    // ���� ������ �ε�
    private async Task<Skill> LoadSinglePrefab(Skill skill, string prefabName, string prefabName2)
    {
        // ��� ��ȿ�� �˻�
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("Prefab name is missing or empty. Cannot load prefab.");
            return null;
        }
        if (!string.IsNullOrEmpty(prefabName))
        {
            Debug.Log(skill.ID);
            // ���� ����
            prefabName = prefabName.Trim();

            // ù ��° ������ �ε�
            Debug.Log($"Loading Prefab: Assets/Resources_moved/Prefabs/{prefabName}.prefab");
            handle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{prefabName}.prefab");

            await handle.Task;

            // ù ��° �������� ���������� �ε�Ǿ����� Ȯ��
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                // Skill ��ü�� ������ �Ҵ�
                skill.skillPrefab = handle.Result;
                Debug.Log($"Prefab {prefabName} loaded successfully.");
            }
            else
            {
                Debug.LogError($"Failed to load prefab: {prefabName}");
                return null;
            }
        }

        
        // �� ��° ������ �ε� (�ʿ��� ���)
        if (!string.IsNullOrEmpty(prefabName2) && prefabName2.Trim() != "NoPrefab")
        {
            Debug.Log($"Prefab Name 2: {prefabName2} (ID: {skill.ID})"); // ����� �α׷� prefabName2 Ȯ��
            prefabName2 = prefabName2.Trim();

            Debug.Log($"Loading Second Prefab: Assets/Resources_moved/Prefabs/{prefabName2}.prefab");
            otherHandle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{prefabName2}.prefab");
            await otherHandle.Task;

            if (otherHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                // �߰����� �� ��° ������ ó���� �ʿ��ϴٸ� ���⿡ ���� �߰�
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

        return skill;  // ��ų�� �������� �߰��� ���·� ��ȯ
    }

    // ���� ������ �ε�
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
                Debug.LogError($"������ {prefabName} �ε� ����");
            }
            
        }

       
        return skill;
    }

    // ���� �ڵ� ��ȯ
    public AsyncOperationHandle<GameObject> GetCurrentHandle()
    {
        return handle;
    }
    public AsyncOperationHandle<GameObject> GetCurrentOtherHandle()
    {
        return otherHandle;
    }

    // ���� ��ȯ ���� ó��
    private float TryParseFloat(string input, float defaultValue)
    {
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    // Bool ��ȯ ���� ó��
    private bool TryParseBool(string input, bool defaultValue)
    {
        if (bool.TryParse(input, out bool result))
        {
            return result;
        }
        return defaultValue;
    }
}
