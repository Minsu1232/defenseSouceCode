using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // 17명의 캐릭터가 담긴 프리팹 리스트
    public List<GameObject> characterPrefabs;

    // 캐릭터를 나타낼 7개의 위치
    public List<Transform> spawnPositions;

    private List<int> chosenIndices = new List<int>();  // 선택된 캐릭터 인덱스 저장

    void Start()
    {
        // 17개의 리스트 중 7명을 랜덤하게 선택하여 원하는 위치에 등장
        SpawnRandomCharacters();
    }

    void SpawnRandomCharacters()
    {
        // 이미 선택된 인덱스를 저장하는 리스트
        chosenIndices.Clear();

        // 랜덤하게 7명을 선택
        while (chosenIndices.Count < 7)
        {
            int randomIndex = Random.Range(0, characterPrefabs.Count);

            // 중복되지 않는 인덱스를 선택
            if (!chosenIndices.Contains(randomIndex))
            {
                chosenIndices.Add(randomIndex);
            }
        }

        // 선택된 7명을 각기 다른 위치에 배치
        for (int i = 0; i < chosenIndices.Count; i++)
        {
            int index = chosenIndices[i];
            GameObject character = Instantiate(characterPrefabs[index], spawnPositions[i].position, Quaternion.identity);
            character.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            character.transform.SetParent(spawnPositions[i], false);  // 부모로 위치 트랜스폼 설정
        }
    }
}
