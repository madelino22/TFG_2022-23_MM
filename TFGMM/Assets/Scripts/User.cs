using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string userName = "Este es mi usuario";

    public string email = "Este es mi correo";

    public float rankProgress = 0;

    public ranks soloRank = ranks.silver3;

    public float dps = 0;

    public int gamesPlayed = 0;

    public int wins = 0;

    public int loses = 0;

    public int kills = 0;

    public int deaths = 0;

    public int assists = 0;

    public int averageDamagePerGame = 0;

    public int totalDamage = 0;

    public int killsDeathsAverage = 0;

    public int damageReceivedPerGame = 0;

    public int healedLifePerGame = 0;

    public void loadInfo(DataSnapshot snapshot)
    {
        userName = snapshot.Child("userName").Value.ToString();


        email = snapshot.Child("email").Value.ToString();

        rankProgress = (float)snapshot.Child("rankProgress").Value;

        soloRank = (ranks)snapshot.Child("rankProgress").Value;

        dps = (float)snapshot.Child("dps").Value;

        gamesPlayed = (int)snapshot.Child("gamesPlayed").Value;

        wins = (int)snapshot.Child("wins").Value;

        loses = (int)snapshot.Child("loses").Value;

        kills = (int)snapshot.Child("kills").Value;

        deaths = (int)snapshot.Child("deaths").Value;

        assists = (int)snapshot.Child("assists").Value;

        averageDamagePerGame = (int)snapshot.Child("averageDamagePerGame").Value;

        totalDamage = (int)snapshot.Child("totalDamage").Value;

        killsDeathsAverage = (int)snapshot.Child("killsDeathsAverage").Value;

        healedLifePerGame = (int)snapshot.Child("healedLifePerGame").Value;

        damageReceivedPerGame = (int)snapshot.Child("damageReceivedPerGame").Value;
    }
}
