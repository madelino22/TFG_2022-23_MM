using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string userName = "Este es mi usuario";

    public string email = "Este es mi correo";

    public int rankProgress = 0;

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

        rankProgress = int.Parse(snapshot.Child("rankProgress").Value.ToString());

        soloRank = (ranks)int.Parse(snapshot.Child("soloRank").Value.ToString());

        dps = float.Parse(snapshot.Child("dps").Value.ToString());

        gamesPlayed = int.Parse(snapshot.Child("gamesPlayed").Value.ToString());

        wins = int.Parse(snapshot.Child("wins").Value.ToString());

        loses = int.Parse(snapshot.Child("loses").Value.ToString());

        kills = int.Parse(snapshot.Child("kills").Value.ToString());

        deaths = int.Parse(snapshot.Child("deaths").Value.ToString());

        assists = int.Parse(snapshot.Child("assists").Value.ToString());
        Debug.Log("Me llegan :" + assists);//------------------------------------------

        averageDamagePerGame = int.Parse(snapshot.Child("averageDamagePerGame").Value.ToString());

        totalDamage = int.Parse(snapshot.Child("totalDamage").Value.ToString());

        killsDeathsAverage = int.Parse(snapshot.Child("killsDeathsAverage").Value.ToString());

        healedLifePerGame = int.Parse(snapshot.Child("healedLifePerGame").Value.ToString());

        damageReceivedPerGame = int.Parse(snapshot.Child("damageReceivedPerGame").Value.ToString());
    }

    
}
