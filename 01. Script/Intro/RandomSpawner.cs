using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // 17���� ĳ���Ͱ� ��� ������ ����Ʈ
    public List<GameObject> characterPrefabs;

    // ĳ���͸� ��Ÿ�� 7���� ��ġ
    public List<Transform> spawnPositions;

    private List<int> chosenIndices = new List<int>();  // ���õ� ĳ���� �ε��� ����

    void Start()
    {
        // 17���� ����Ʈ �� 7���� �����ϰ� �����Ͽ� ���ϴ� ��ġ�� ����
        SpawnRandomCharacters();
    }

    void SpawnRandomCharacters()
    {
        // �̹� ���õ� �ε����� �����ϴ� ����Ʈ
        chosenIndices.Clear();

        // �����ϰ� 7���� ����
        while (chosenIndices.Count < 7)
        {
            int randomIndex = Random.Range(0, characterPrefabs.Count);

            // �ߺ����� �ʴ� �ε����� ����
            if (!chosenIndices.Contains(randomIndex))
            {
                chosenIndices.Add(randomIndex);
            }
        }

        // ���õ� 7���� ���� �ٸ� ��ġ�� ��ġ
        for (int i = 0; i < chosenIndices.Count; i++)
        {
            int index = chosenIndices[i];
            GameObject character = Instantiate(characterPrefabs[index], spawnPositions[i].position, Quaternion.identity);
            character.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            character.transform.SetParent(spawnPositions[i], false);  // �θ�� ��ġ Ʈ������ ����
        }
    }
}
