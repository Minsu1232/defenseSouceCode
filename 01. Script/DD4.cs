using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD4 : MonoBehaviour
{
    private static DD4 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 새로 생성된 오브젝트를 파괴
        }
    }
}
