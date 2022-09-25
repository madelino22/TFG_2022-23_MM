using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class realtimeDatabase : MonoBehaviour
{
    DatabaseReference reference;


    User user = ComInfo.getPlayerData();
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        readData();
    }

    public void saveData()
    {
        string json = JsonUtility.ToJson(user);

        reference.Child("User").Child(user.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("Datos enviados");
            }
            else
            {
                Debug.Log("No se han enviado los datos");
            }
        }
        );
    }

    public void readData()
    {
        reference.Child("User").Child(user.userName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted) //If player exist put existing data
            {
                Debug.Log("Datos enviados");

                DataSnapshot snapshot = task.Result;

                user.loadInfo(snapshot);

                
            }
            else //Create a new player in the database
            {
                saveData();
            }

        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
