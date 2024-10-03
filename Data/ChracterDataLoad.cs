using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// Unitdata.CSV���� ���̽����� �Ϲݰ��� �����ո� ����
/// </summary>
public class ChracterDataLoad : MonoBehaviour
{
    // ������ ������ ScriptableObject ����Ʈ (�����Ϳ��� �Ҵ�)
    public List<CharacterData> characterDataList;

    // ��ȣȭ/��ȣȭ �޼��� (XOR ���)  

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        // ���� ���� �� ĳ���� �����͸� CSV���� �ε�
        await LoadCharacterDataFromCSV();
    }
    // ĳ���� �����͸� �񵿱�� CSV���� �ҷ�����, Addressables�� �������� �Ҵ��ϴ� �޼���
    public async Task LoadCharacterDataFromCSV()
    {
       
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/Unitdata_Version_Test");

        string csvText = csvFile.text;


        string[] lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue; // �� �� ����

            string[] fields = line.Split(',');

            // CSV���� ID�� �о�� (Trim �߰�)
            if (!int.TryParse(fields[0].Trim(), out int id))  // ID�� ���� �������� Ȯ��
            {
                continue;  // �߸��� �����ʹ� ��ŵ
            }

            // ID�� �´� ĳ���� �����͸� ã��
            CharacterData character = GetCharacterDataByID(id);

            if (character != null)
            {
                // CSV �����͸� ���� ScriptableObject�� ���� (���̽� ���� ����)
                character.heroName = fields[1].Trim();

                // ���� �ʵ带 �ѹ��� �Ľ��ϰ� Ȯ��
                if (!float.TryParse(fields[2].Trim(), out character.baseAttackPower) ||
                    !float.TryParse(fields[3].Trim(), out character.baseAttackSpeed) ||
                    !float.TryParse(fields[4].Trim(), out character.baseAttackRange) ||
                    !float.TryParse(fields[5].Trim(), out character.baseCriticalChance))
                {
                    continue; // ���� �ùٸ��� ������ ��ŵ
                }

                // AttackPrefab�� Addressables�� �ε�
                await LoadCharacterAttackPrefab(character, fields[6].Trim());
            }
            else
            {
                Debug.LogError($"ID�� {id}�� ĳ���� �����͸� ã�� �� �����ϴ�.");
            }
        }
        
    }

    // AttackPrefab�� Addressables�� �ε��ϴ� �޼���
    private async Task LoadCharacterAttackPrefab(CharacterData character, string attackPrefabName)
    {
        if (attackPrefabName == "NoPrefab") // ���� NoPrefab�̸� ��ŵ
        {
            character.attackPrefab = null;
            Debug.Log($"{character.heroName}�� AttackPrefab�� �����ϴ�.");
            return;
        }

        // AttackPrefab �ε�
        AsyncOperationHandle<GameObject> attackPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"Assets/Resources_moved/Prefabs/{attackPrefabName}.prefab");
        await attackPrefabHandle.Task;

        if (attackPrefabHandle.Status == AsyncOperationStatus.Succeeded)
        {
            character.attackPrefab = attackPrefabHandle.Result;
            Debug.Log($"ĳ���� {character.heroName}�� ���� ������ {attackPrefabName} �ε� �Ϸ�.");
        }
        else
        {
            Debug.LogError($"ĳ���� {character.heroName}�� ���� ������ {attackPrefabName} �ε� ����.");
        }
    }

    // ID�� ĳ���� �����͸� �������� �޼���
    private CharacterData GetCharacterDataByID(int id)
    {
        foreach (var character in characterDataList)
        {
            if (character.id == id) // ID�� ��
            {
                return character;
            }
        }

        Debug.LogError($"ID�� {id}�� ĳ���� �����͸� ã�� �� �����ϴ�.");
        return null;
    }
}
