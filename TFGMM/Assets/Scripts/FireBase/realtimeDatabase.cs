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

    public void readData()
    {
        //Check if player exist
        reference.Child("User").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted) //If player exist put existing data
            {
                DataSnapshot snapshot = task.Result;

                //Check if player already in database
                if (snapshot.HasChild(user.userName)) 
                {
                    Debug.Log("Player already registered");

                    reference.Child("User").Child(user.userName).GetValueAsync().ContinueWith(task =>
                    {
                        if (task.IsCompleted) //If player exist put existing data
                        {
                            Debug.Log("Reading Previous Data from Firebase");

                            DataSnapshot snapshot = task.Result;

                            user.loadInfo(snapshot); //NO SE LLAMA
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
            //Create a new player in the database
            else
            {
                Debug.Log("Error checking if player exist ");
            }
        });
    }

    public void saveData() //if player doesn't exist in firebase
    {
        string json = JsonUtility.ToJson(user);

        Debug.Log(json);

        reference.Child("User").Child(user.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("saved Data Profile");
            }
            else
            {
                Debug.Log("No se han enviado los datos");
            }
        }
        );
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("ASIIIIIIIIS " + user.assists);
        if (Input.GetKeyDown(KeyCode.A))
        {
            user.assists++;
            Debug.Log("ASSIST");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Subir datos: " + user.assists);
            saveData();
        }
    }
}
