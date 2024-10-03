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
            InitializeFirebase(); // �ʱ�ȭ�� ���ÿ� ĳ�� �ʱ�ȭ
            Debug.Log("���̾�̽��Ŵ��� ����");
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

                // �������� ��� ��Ȱ��ȭ �� ĳ�� �ʱ�ȭ
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

                // Firebase ĳ�ø� �ʱ�ȭ�ϰ� �翬��
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
