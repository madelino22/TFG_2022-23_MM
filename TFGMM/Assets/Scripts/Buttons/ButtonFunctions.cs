using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    AudioSource audioData;

    //Referencia a Firebase
    DatabaseReference reference;

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void returnToMenu()
    {
        //audioData.Play(0);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void returnFromInfo()
    {
        //audioData.Play(0);
        SceneManager.LoadScene("ChangeHero", LoadSceneMode.Single);
    }

    public void heroChosen()
    {
        //audioData.Play(0);
        ComInfo.setHero((heroes)LoadInfo.HeroExp);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void openFriends()
    {
        //audioData.Play(0);
        SceneManager.LoadScene("addFriends", LoadSceneMode.Additive);
    }

    public void goSearch()
    {
        //audioData.Play(0);
        SceneManager.LoadScene("Searching", LoadSceneMode.Single);
    }

    public void giveRating(int rating)
    {
        //Solo tenemos en cuenta los clicks si ha jugado partida
        if(ComInfo.getPlayerData().gamesPlayed > ComInfo.getPlayerData().numPartidasRated)
        {
            if(RoundData.winner == team.red && RoundData.isRed) //Ha ganado el jugador
            {
                Debug.Log("RATING: El jugador ha ganado");
                ComInfo.getPlayerData().mediaPartidasGanadas = ((ComInfo.getPlayerData().mediaPartidasGanadas * ComInfo.getPlayerData().wins) + rating) / (ComInfo.getPlayerData().wins);
            }
            else if(RoundData.winner == team.none) //Ha empatado el jugador
            {
                Debug.Log("RATING: El jugador ha empatado");
                ComInfo.getPlayerData().mediaPartidasEmpatadas = ((ComInfo.getPlayerData().mediaPartidasEmpatadas * ComInfo.getPlayerData().draws) + rating) / (ComInfo.getPlayerData().draws);
            }
            else //HA PERDIDO
            {
                Debug.Log("RATING: El jugador ha perdido");
                ComInfo.getPlayerData().mediaPartidasPerdidas = ((ComInfo.getPlayerData().mediaPartidasPerdidas * ComInfo.getPlayerData().loses) + rating) / (ComInfo.getPlayerData().loses);
            }

            ComInfo.getPlayerData().mediaGeneralRating = ((ComInfo.getPlayerData().mediaGeneralRating * ComInfo.getPlayerData().numPartidasRated) + rating) / (ComInfo.getPlayerData().numPartidasRated + 1);

            ComInfo.getPlayerData().numPartidasRated++;

            Debug.Log("RATING: El jugador ha valorado " + ComInfo.getPlayerData().numPartidasRated + "partidas");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("RATING: Application ending after " + Time.time + " seconds");

        UserHistory userHistory = ComInfo.getPlayerData();

        saveData(userHistory);
    }

    private void saveData(UserHistory userHistory)
    {
        string json = JsonUtility.ToJson(userHistory);

        Debug.Log(json);

        //Save base data
        reference.Child("User").Child(userHistory.userName).SetRawJsonValueAsync(json).ContinueWith(task =>
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
}
