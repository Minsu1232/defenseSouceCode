using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Database URL ¼³Á¤
                app.Options.DatabaseUrl = new System.Uri("https://defensegameproject-default-rtdb.firebaseio.com/");

                Debug.Log("Firebase is ready.");
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", task.Result));
            }
        });
    }
}
