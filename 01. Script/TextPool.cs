using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPool : MonoBehaviour
{
    public GameObject damageCanvasPrefab;
    public GameObject textPoolPrefab;
    public int poolSize = 6000;
    public int poolIncrement = 2500; // 필요할 때마다 추가로 생성할 수량
    private Queue<GameObject> pool = new Queue<GameObject>();
    private bool isExpandingPool = false; // 풀 확장이 진행 중인지 확인

    void Start()
    {
        InitializePool(poolSize);
    }

    // 초기 풀 생성
    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(damageCanvasPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public void ReturnDamageCanvas(GameObject obj)
    {
        obj.SetActive(false);

        // 부모 관계에서 해제하고, 풀로 반환
        obj.transform.SetParent(null); // 부모를 없애거나, 특정 부모 오브젝트로 설정
        pool.Enqueue(obj);


    }
    public GameObject GetDamageCanvas()
    {
        // 풀에 남은 오브젝트가 있는지 확인
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // 풀링된 오브젝트가 없으면 풀 확장을 진행하고, 즉시 새로운 오브젝트 생성
            if (!isExpandingPool)
            {
                
                StartCoroutine(ExpandPoolGradually(poolIncrement, 250)); // 한 번에 250개씩 생성
            }
            else
            {
                Debug.LogWarning("Pool is already expanding, waiting for new objects.");
            }

            // 풀 확장이 진행 중일 때 임시로 빈 오브젝트를 반환하거나, 대기하는 로직을 추가할 수 있습니다.
            return null; // 임시로 null 반환 (추후 로직에 따라 조정 가능)
        }
    }
    // 풀을 일정 수씩 생성하며 점진적으로 확장
    private IEnumerator ExpandPoolGradually(int increment, int batchSize)
    {
        isExpandingPool = true; // 풀 확장 플래그 설정

        for (int i = 0; i < increment; i++)
        {
            GameObject obj = Instantiate(damageCanvasPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);

            // 일정 수의 오브젝트를 생성한 후 한 프레임 대기
            if ((i + 1) % batchSize == 0)
            {
                yield return null; // 한 프레임 대기
            }
        }

        isExpandingPool = false; // 풀 확장 완료 후 플래그 해제
    }

   

  
}
