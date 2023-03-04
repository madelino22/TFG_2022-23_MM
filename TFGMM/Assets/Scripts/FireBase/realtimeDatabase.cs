using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using Photon.Bolt;

public class realtimeDatabase : GlobalEventListener
{
    DatabaseReference reference;

    //Data stored in Firebase
    UserHistory userHistory = ComInfo.getPlayerData();
    //Data from Last Round
    RoundData user = new RoundData();

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        readData();
    }

    public override void OnEvent(saveGameEvent evnt)
    {
        saveMatch();
    }

    public void saveMatch()
    {
        string json2 = JsonUtility.ToJson(new Match()); //Cambiar el new Match por los datos reales de la partida


        //Saber que numero de partida es la siguiente
        int nMatches = 0;
        reference.Child("Matches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                nMatches = int.Parse(snapshot.Child("nMatches").Value.ToString());
            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });

        //Crear la partida en la base de datos
        reference.Child("Matches").Child("Partida " + nMatches.ToString()).SetRawJsonValueAsync(json2).ContinueWith(task =>
        {
              if (task.IsCompleted)
              {
                  //Debug.Log("saved the match");
              }
              else
              {
                  //Debug.Log("No se han enviado los datos");
              }
        }
       );

        Match aux = new Match(); //Cambiar por los datos reales de la partida

        //Guardar que ha hecho cada jugador en la partida

        for (int j = 0; j < 6; j++)
        {
            string json = aux.playerJSON(j);

            Debug.Log(json);

            reference.Child("Matches").Child("Partida " + nMatches.ToString()).Child(aux.players[j].name).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    //Debug.Log("saved players of game");
                }
                else
                {
                    //Debug.Log("No se ha guardado al jugador");
                }
            }
            );
        }

        //Guardar que se ha jugado una partida mas

        nMatches++;

        string json3 = JsonUtility.ToJson(nMatches);

        reference.Child("Matches").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                reference.Child("nMatches").SetRawJsonValueAsync(json3).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        //Debug.Log("saved the match");
                    }
                    else
                    {
                        //Debug.Log("No se han enviado los datos");
                    }
                });
            }
            else
            {
                Debug.Log("No se han encontrado el numero de partidas totales");
            }
        });

    }

    public void readData()
    {
        //Check if player exist
        reference.Child("User").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted) 
            {
                DataSnapshot snapshot = task.Result;

                //Check if player already in database
                if (snapshot.HasChild(userHistory.userName)) 
                {
                    //Debug.Log("Player already registered");

                    reference.Child("User").Child(userHistory.userName).GetValueAsync().ContinueWith(task =>
                    {
                        if (task.IsCompleted) //If player exist put existing data
                        {
                            //Debug.Log("Reading Previous Data from Firebase");

                            DataSnapshot snapshot = task.Result;

                            userHistory.loadInfo(snapshot);
                            user.ResetData(); //Initialize Data with 0
                        }
                        else //Create a new player in the database
                        {
                           // Debug.Log("Error reading data");
                        }
                    });
                }
                else //Player dont exist create it
                {
                  //  Debug.Log("New player created");
                    saveData();
                }
            }
            else
            {
              //  Debug.Log("Error checking if player exist ");
            }
        });
    }
    public override void OnEvent(savePlayerStatsEvent evnt)
    {
        saveData();
    }

    public void saveData() //if player doesn't exist in firebase
    {
        string json = JsonUtility.ToJson(userHistory);

        Debug.Log(json);

        //Save base data
        reference.Child("User").Child(userHistory.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
              //  Debug.Log("saved Data Profile");
            }
            else
            {
             //   Debug.Log("No se han enviado los datos");
            }
        }
        );


        for (int i = 0; i < 5; i++)
        {
            json = userHistory.saveGames(i);
            Debug.Log(i);
            Debug.Log(json);

            reference.Child("User").Child(userHistory.userName).Child("zzzLastGames").Child("Partida" + i).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                 //   Debug.Log("saved games");
                }
                else
                {
               //     Debug.Log("No se ha guardado la partida");
                }
            }
            );

            for (int j = 0; j < 6; j++)
            {
                json = userHistory.saveGamePlayer(i, j);

              //  Debug.Log(i);
              //  Debug.Log(json);

                reference.Child("User").Child(userHistory.userName).Child("zzzLastGames").Child("Partida" + i).Child(userHistory.lastMatches[i].players[j].name).SetRawJsonValueAsync(json).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                 //       Debug.Log("saved players of game");
                    }
                    else
                    {
                  //      Debug.Log("No se ha guardado al jugador");
                    }
                }
                );
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            user.assists++;
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("ASSISTS IN HISTORY; " + userHistory.assists);
            userHistory.UpdateUserHistory(user);
            Debug.Log("ASSISTS IN HISTORY (Post Update); " + userHistory.assists);
            saveData();
        }
    }
}
