using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager Instance { get; private set; }
    private FirebaseApp app;
    private DatabaseReference databaseReference;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase(); // 초기화와 동시에 캐시 초기화
            Debug.Log("파이어베이스매니저 실행");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                app = FirebaseApp.DefaultInstance;

                // 오프라인 모드 비활성화 및 캐시 초기화
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

                // Firebase 캐시를 초기화하고 재연결
                FirebaseDatabase.DefaultInstance.GoOffline();
                FirebaseDatabase.DefaultInstance.GoOnline();

                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                IsInitialized = true;
                Debug.Log("Firebase Initialized with Cache Cleared and Offline Mode Disabled");
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    public DatabaseReference GetDatabaseReference()
    {
        if (!IsInitialized)
        {
            Debug.LogError("Firebase is not initialized yet!");
            return null;
        }
        return databaseReference;
    }
}
