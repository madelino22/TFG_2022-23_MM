using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database; //Firebase

public class UserHistory : MonoBehaviour
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

    public int totalDamage = 0;

    public int killsDeathsAverage = 0;

    public int damageReceivedPerGame = 0;

    public int healedLifePerGame = 0;

    public int zzlastGameSaved = 1;

    public Match[] lastMatches = new Match[5];

    public string saveGames(int index)
    {
        lastMatches[index] = new Match(6);

        return JsonUtility.ToJson(lastMatches[index]);
    }

    public string saveGamePlayer(int indexGame, int nPlayer)
    {
        return JsonUtility.ToJson(lastMatches[indexGame].players[nPlayer]);
    }

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

        totalDamage = int.Parse(snapshot.Child("totalDamage").Value.ToString());

        killsDeathsAverage = int.Parse(snapshot.Child("killsDeathsAverage").Value.ToString());

        healedLifePerGame = int.Parse(snapshot.Child("healedLifePerGame").Value.ToString());

        damageReceivedPerGame = int.Parse(snapshot.Child("damageReceivedPerGame").Value.ToString());

        zzlastGameSaved = int.Parse(snapshot.Child("damageReceivedPerGame").Value.ToString());


        //Load Games

        for (int i = 0; i < 5; i++)
        {
            team aux = (team)int.Parse(snapshot.Child("zzzLastGames").Child("Partida" + i).Child("winner").Value.ToString());

            lastMatches[i] = new Match(aux, snapshot.Child("zzzLastGames").Child("Partida" + i));
        }
    }

    public void UpdateUserHistory()
    {
        gamesPlayed++;

        rankProgress += RoundData.rankProgress; //?????????

        soloRank = RoundData.soloRank; //??????????

        int dpsThisGame = RoundData.damage / 15; //15 HASTA QUE PONGAS 90 CON PARTIDAS BIEN
        dps = ((gamesPlayed - 1) * dps + dpsThisGame) / (gamesPlayed); //damage per second

        if(RoundData.won) wins++;

        else loses++;

        assists += RoundData.assists; 
        totalDamage += RoundData.damageReceived; //Damage Received by Player
        kills += RoundData.kills;
        deaths += RoundData.deaths;

        if (RoundData.deaths != 0) //Don't divide by zero
            killsDeathsAverage = ((gamesPlayed - 1) * killsDeathsAverage + (RoundData.kills / RoundData.deaths)) / (gamesPlayed);

        //Damage Received by Player
        damageReceivedPerGame = ((gamesPlayed - 1) * damageReceivedPerGame + RoundData.damageReceived) / (gamesPlayed);

        healedLifePerGame = ((gamesPlayed - 1) * healedLifePerGame + RoundData.healedLife) / (gamesPlayed);
    }
}
