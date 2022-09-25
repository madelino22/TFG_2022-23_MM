using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class realtimeDatabase : MonoBehaviour
{
    DatabaseReference reference;


    User user = new User();
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        saveData();
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
        reference.Child("User").Child("Este es mi usuario").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Datos enviados");
                DataSnapshot snapshot = task.Result;

                Debug.Log("Nombre: " + snapshot.Child("userName").Value.ToString());
                Debug.Log("Correo Electronico: " + snapshot.Child("email").Value.ToString());
            }
            else
            {
                Debug.Log("No se han recibido los datos o no existen");
            }

        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
