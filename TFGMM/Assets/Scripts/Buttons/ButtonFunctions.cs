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

    public void giveRating(int gameRating)
    {
        //Solo tenemos en cuenta los clicks si ha jugado partida
        if(ComInfo.getPlayerData().gamesPlayed > ComInfo.getPlayerData().numPartidasRated)
        {
            //VALORACION GENERAL DE LA PARTIDA====================================================================================================            
            if(RoundData.winner == team.red && RoundData.isRed) //Ha ganado el jugador
            {
                Debug.Log("RATING: El jugador ha ganado");
                ComInfo.getPlayerData().mediaRatingGanadas = ((ComInfo.getPlayerData().mediaRatingGanadas * (ComInfo.getPlayerData().wins - 1)) + gameRating) / (ComInfo.getPlayerData().wins);
            }
            else if(RoundData.winner == team.none) //Ha empatado el jugador
            {
                Debug.Log("RATING: El jugador ha empatado");
                ComInfo.getPlayerData().mediaRatingEmpatadas = ((ComInfo.getPlayerData().mediaRatingEmpatadas * (ComInfo.getPlayerData().draws - 1)) + gameRating) / (ComInfo.getPlayerData().draws);
            }
            else //HA PERDIDO
            {
                Debug.Log("RATING: El jugador ha perdido");
                ComInfo.getPlayerData().mediaRatingPerdidas = ((ComInfo.getPlayerData().mediaRatingPerdidas * (ComInfo.getPlayerData().loses - 1)) + gameRating) / (ComInfo.getPlayerData().loses);
            }

            ComInfo.getPlayerData().mediaGeneralRating = ((ComInfo.getPlayerData().mediaGeneralRating * ComInfo.getPlayerData().numPartidasRated) + gameRating) / (ComInfo.getPlayerData().numPartidasRated + 1);

            ComInfo.getPlayerData().numPartidasRated++;

            Debug.Log("RATING: El jugador ha valorado " + ComInfo.getPlayerData().numPartidasRated + "partidas");

            //VALORACION DE ROL ESPECIFICO ========================================================================================================

            if (ComInfo.getPlayerData().lastRole == "Healer")
            {
                if (RoundData.winner == team.red && RoundData.isRed) //Ha ganado el jugador
                {
                    Debug.Log("RATING: El jugador ha ganado");
                    ComInfo.getPlayerData().mediaRatingGanadasHeal = ((ComInfo.getPlayerData().mediaRatingGanadasHeal * (ComInfo.getPlayerData().winsHeal - 1)) + gameRating) / (ComInfo.getPlayerData().winsHeal);
                }
                else if (RoundData.winner == team.none) //Ha empatado el jugador
                {
                    Debug.Log("RATING: El jugador ha empatado");
                    ComInfo.getPlayerData().mediaRatingEmpatadasHeal = ((ComInfo.getPlayerData().mediaRatingEmpatadasHeal * (ComInfo.getPlayerData().drawsHeal - 1)) + gameRating) / (ComInfo.getPlayerData().drawsHeal);
                }
                else //HA PERDIDO
                {
                    Debug.Log("RATING: El jugador ha perdido");
                    ComInfo.getPlayerData().mediaRatingPerdidasHeal = ((ComInfo.getPlayerData().mediaRatingPerdidasHeal * (ComInfo.getPlayerData().losesHeal - 1)) + gameRating) / (ComInfo.getPlayerData().losesHeal);
                }

                ComInfo.getPlayerData().mediaGeneralRatingHeal = 
                    ((ComInfo.getPlayerData().mediaGeneralRatingHeal * ComInfo.getPlayerData().numHeal) + gameRating) 
                    / (ComInfo.getPlayerData().numHeal + 1);

            }
            else if (ComInfo.getPlayerData().lastRole == "Sniper")
            {
                if (RoundData.winner == team.red && RoundData.isRed) //Ha ganado el jugador
                {
                    Debug.Log("RATING: El jugador ha ganado");
                    ComInfo.getPlayerData().mediaRatingGanadasFra = ((ComInfo.getPlayerData().mediaRatingGanadasFra * (ComInfo.getPlayerData().winsFra - 1)) + gameRating) / (ComInfo.getPlayerData().winsFra);
                }
                else if (RoundData.winner == team.none) //Ha empatado el jugador
                {
                    Debug.Log("RATING: El jugador ha empatado");
                    ComInfo.getPlayerData().mediaRatingEmpatadasFra = ((ComInfo.getPlayerData().mediaRatingEmpatadasFra * (ComInfo.getPlayerData().drawsFra - 1)) + gameRating) / (ComInfo.getPlayerData().drawsFra);
                }
                else //HA PERDIDO
                {
                    Debug.Log("RATING: El jugador ha perdido");
                    ComInfo.getPlayerData().mediaRatingPerdidasFra = ((ComInfo.getPlayerData().mediaRatingPerdidasFra * (ComInfo.getPlayerData().losesFra - 1)) + gameRating) / (ComInfo.getPlayerData().losesFra);
                }

                ComInfo.getPlayerData().mediaGeneralRatingFra = 
                    ((ComInfo.getPlayerData().mediaGeneralRatingFra * ComInfo.getPlayerData().numFranc) + gameRating) 
                    / (ComInfo.getPlayerData().numFranc + 1);
            }
            else if (ComInfo.getPlayerData().lastRole == "Duelist")
            {
                if (RoundData.winner == team.red && RoundData.isRed) //Ha ganado el jugador
                {
                    Debug.Log("RATING: El jugador ha ganado");
                    ComInfo.getPlayerData().mediaRatingGanadasDuel = ((ComInfo.getPlayerData().mediaRatingGanadasDuel * (ComInfo.getPlayerData().winsDuel - 1)) + gameRating) / (ComInfo.getPlayerData().winsDuel);
                }
                else if (RoundData.winner == team.none) //Ha empatado el jugador
                {
                    Debug.Log("RATING: El jugador ha empatado");
                    ComInfo.getPlayerData().mediaRatingEmpatadasDuel = 
                        ((ComInfo.getPlayerData().mediaRatingEmpatadasDuel * (ComInfo.getPlayerData().drawsDuel - 1)) + gameRating) / (ComInfo.getPlayerData().drawsDuel);
                }
                else //HA PERDIDO
                {
                    Debug.Log("RATING: El jugador ha perdido");
                    ComInfo.getPlayerData().mediaRatingPerdidasDuel = ((ComInfo.getPlayerData().mediaRatingPerdidasDuel * (ComInfo.getPlayerData().losesDuel - 1)) + gameRating) / (ComInfo.getPlayerData().losesDuel);
                }

                ComInfo.getPlayerData().mediaGeneralRatingDuel = 
                    ((ComInfo.getPlayerData().mediaGeneralRatingDuel * ComInfo.getPlayerData().numDuel) + gameRating)
                    / 
                    (ComInfo.getPlayerData().numDuel + 1);
            }

            UserHistory userHistory = ComInfo.getPlayerData();

            reference.Child("Matches").Child(userHistory.nameLastGamePlayed).Child(userHistory.userName).Child("gameRating").SetValueAsync(gameRating).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("saved Data Profile");
                }
                else
                {
                    Debug.Log("No se han enviado los datos");
                }
            });
            reference.Child("Matches").Child(userHistory.nameLastGamePlayed).Child(userHistory.userName).Child("lastRole").SetValueAsync(userHistory.lastRole).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("saved Data Profile");
                }
                else
                {
                    Debug.Log("No se han enviado los datos");
                }
            });
        }
    }

    struct info
    {
        public info(int rat)
        {
            gameRating = rat;
        }

        public static int gameRating = 0;
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
