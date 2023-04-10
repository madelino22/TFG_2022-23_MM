

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database; //Firebase
//
using Photon.Bolt;
using Photon.Bolt.Utils;
using UdpKit;

public class UserHistory //: Photon.Bolt.IProtocolToken
{
    public string userName = "Este es mi usuario";
    public string email = "Este es mi correo";
    public int gamesPlayed = 0;
    public int wins = 0;
    public int draws = 0;
    public int loses = 0;
    //
    public int kills = 0;
    public int deaths = 0;
    public int damageReceived = 0;
    public int damageInflicted = 0; // NO SE USA
    //
    public int healedMyLife = 0;
    public int healedOthersLife = 0;
    //
    public int zzlastGameSaved = 0;
    //ELO
    public int eloRanking = 1500;
    public float eloK = 40;
    //MEDIAS
    public float killsDeathsRatioAverage = 0;
    public float dps = 0;
    //VALORACION EXPERIENCIA JUGADOR
    public int numPartidasRated = 0;
    public float mediaPartidasGanadas = 0;
    public float mediaPartidasPerdidas = 0;
    public float mediaPartidasEmpatadas = 0;
    public float mediaGeneralRating = 0;


    public int totalShots = 0;
    public void loadInfo(DataSnapshot snapshot)
    {
        userName = snapshot.Child("userName").Value.ToString();
        email = snapshot.Child("email").Value.ToString();
        gamesPlayed = int.Parse(snapshot.Child("gamesPlayed").Value.ToString());
        wins = int.Parse(snapshot.Child("wins").Value.ToString());
        draws = int.Parse(snapshot.Child("draws").Value.ToString());
        loses = int.Parse(snapshot.Child("loses").Value.ToString());
        //
        kills = int.Parse(snapshot.Child("kills").Value.ToString());
        deaths = int.Parse(snapshot.Child("deaths").Value.ToString());
        totalShots = int.Parse(snapshot.Child("totalShots").Value.ToString());
        damageReceived = int.Parse(snapshot.Child("damageReceived").Value.ToString());
        damageInflicted = int.Parse(snapshot.Child("damageInflicted").Value.ToString());
        //
        healedMyLife = int.Parse(snapshot.Child("healedMyLife").Value.ToString());
        healedOthersLife = int.Parse(snapshot.Child("healedOthersLife").Value.ToString());
        //MEDIAS
        dps = float.Parse(snapshot.Child("dps").Value.ToString());
        killsDeathsRatioAverage = float.Parse(snapshot.Child("killsDeathsRatioAverage").Value.ToString());
        eloRanking = int.Parse(snapshot.Child("eloRanking").Value.ToString());
        eloK = float.Parse(snapshot.Child("eloK").Value.ToString());
        zzlastGameSaved = int.Parse(snapshot.Child("zzlastGameSaved").Value.ToString());

        //VALORACION EXPERIENCIA JUGADOR
        numPartidasRated = int.Parse(snapshot.Child("numPartidasRated").Value.ToString());
        mediaPartidasGanadas = float.Parse(snapshot.Child("mediaPartidasGanadas").Value.ToString());
        mediaPartidasPerdidas = float.Parse(snapshot.Child("mediaPartidasPerdidas").Value.ToString());
        mediaPartidasEmpatadas = float.Parse(snapshot.Child("mediaPartidasEmpatadas").Value.ToString());
        mediaGeneralRating = float.Parse(snapshot.Child("mediaGeneralRating").Value.ToString());

        //Load Saved Games
        //for (int i = 0; i < NUM_SAVED_MATCHES; i++)
        //{
        //    string aux = snapshot.Child("zzzLastGames").Child("Partida " + i).Value.ToString();
        //    lastMatches[i] = aux;
        //}
    }

    public void UpdateUserHistory(team winner)
    {
        gamesPlayed++;

        damageReceived += RoundData.damageReceived;
        damageInflicted += RoundData.damageInflicted;
        kills += RoundData.kills;
        deaths += RoundData.deaths;
        totalShots += RoundData.totalShots;

        healedMyLife += RoundData.healedMyLife;
        healedOthersLife += RoundData.healedPlayers;
        float SA = 0;
        float E = 0.5f;

        RoundData.winner = winner;

        switch (winner)
        {
            case team.red:
                if (RoundData.isRed)
                {
                    wins++;
                    eloK++;
                    SA = 1f;
                }
                else
                {
                    loses++;
                    eloK--;
                    SA = 0f;
                }
                break;
            case team.blue:
                if (!RoundData.isRed)
                {
                    wins++;
                    eloK++;
                    SA = 1f;
                }
                else {
                    loses++;
                    eloK--;
                    SA = 0f;
                } 
                break;
            case team.none:
                draws++;
                SA = 0.5f;
                break;
        }

        if (RoundData.isRed)
            E = ELO.GetRedChances();
        else
            E = ELO.GetBlueChances();

        Debug.Log("CHANCES UH: las chances de ganar del red son: " + ELO.redChances);

        if (gamesPlayed < 10)
        {
            eloK = 40;
        }
        else
        {
            if(eloRanking < 2100)
            {
                eloK = 32;
            }
            else if(eloRanking > 2400)
            {
                eloK = 16;
            }
            else
            {
                eloK = 24;
            }
        }
        eloRanking = ELO.CalculteNewElo(eloRanking, eloK, SA, E); // LOLITOOOOOO
        //MEDIAS

        killsDeathsRatioAverage = kills;
        if (deaths > 0)
            killsDeathsRatioAverage = ((float)kills) / (float)deaths;

        dps = (damageReceived) / (gamesPlayed * 120); //damage per second

    }
}
