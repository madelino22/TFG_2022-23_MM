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

        Debug.Log(json);

        reference.Child("User").Child(user.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("saved Data");
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
        //Check if player exist
        reference.Child("User").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted) //If player exist put existing data
            {
                DataSnapshot snapshot = task.Result;

                if(snapshot.HasChild(user.userName)) //Check if player already in database
                {
                    Debug.Log("Player already registered");

                    reference.Child("User").Child(user.userName).GetValueAsync().ContinueWith(task =>
                    {
                        if (task.IsCompleted) //If player exist put existing data
                        {
                            Debug.Log("Data read");

                            DataSnapshot snapshot = task.Result;

                            user.loadInfo(snapshot);


                        }
                        else //Create a new player in the database
                        {
                            Debug.Log("Error reading data");
                        }

                    });
                }
                else //Player dont exist create it
                {
                    Debug.Log("New player created");
                    saveData();
                }


            }
            else //Create a new player in the database
            {
                Debug.Log("Error checking if player exist ");
            }

        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
