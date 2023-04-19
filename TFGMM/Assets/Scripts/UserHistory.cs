

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
    public float eloRanking = 1500;
    public float eloK = 40;
    //MEDIAS
    public float killsDeathsRatioAverage = 0;
    public float dps = 0;

    public string nameLastGamePlayed = "none";

    //ESTILO DEL JUGADOR none, healer, francotirador, duelista
    public string playerRole = "None"; //Rol maytoritario
    public string lastRole = "None"; //Rol de la ultima partida (ayuda para saber donde colocar la valoracion)
    public int numFranc = 0;
    public int numHeal = 0;
    public int numDuel = 0;
    public int numNone = 0;

    //VALORACION EXPERIENCIA JUGADOR
    public int numPartidasRated = 0;
    public float mediaRatingGanadas = 0;
    public float mediaRatingPerdidas = 0;
    public float mediaRatingEmpatadas = 0;
    public float mediaGeneralRating = 0;
    //Francotirador
    public float mediaRatingGanadasFra = 0;
    public float mediaRatingPerdidasFra = 0;
    public float mediaRatingEmpatadasFra = 0;
    public float mediaGeneralRatingFra = 0;
    public int winsFra = 0;
    public int drawsFra = 0;
    public int losesFra = 0;
    //Healer
    public float mediaRatingGanadasHeal = 0;
    public float mediaRatingPerdidasHeal = 0;
    public float mediaRatingEmpatadasHeal = 0;
    public float mediaGeneralRatingHeal = 0;
    public int winsHeal = 0;
    public int drawsHeal = 0;
    public int losesHeal = 0;
    //Duelista
    public float mediaRatingGanadasDuel = 0;
    public float mediaRatingPerdidasDuel = 0;
    public float mediaRatingEmpatadasDuel = 0;
    public float mediaGeneralRatingDuel = 0;
    public int winsDuel = 0;
    public int drawsDuel = 0;
    public int losesDuel = 0;
    //None
    public float mediaRatingGanadasNone = 0;
    public float mediaRatingPerdidasNone = 0;
    public float mediaRatingEmpatadasNone = 0;
    public float mediaGeneralRatingNone = 0;
    public int winsNone = 0;
    public int drawsNone = 0;
    public int losesNone = 0;

    public int totalShots = 0;
    public float distanceMedia = 0;

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
        distanceMedia = float.Parse(snapshot.Child("distanceMedia").Value.ToString());

        nameLastGamePlayed = snapshot.Child("nameLastGamePlayed").Value.ToString();
        //PLAYER ROLE
        playerRole = snapshot.Child("playerRole").Value.ToString();
        lastRole = snapshot.Child("lastRole").Value.ToString();
        numFranc = int.Parse(snapshot.Child("numFranc").Value.ToString());
        numHeal = int.Parse(snapshot.Child("numHeal").Value.ToString());
        numDuel = int.Parse(snapshot.Child("numDuel").Value.ToString());
        numNone = int.Parse(snapshot.Child("numNone").Value.ToString());

        //VALORACION EXPERIENCIA JUGADOR
        mediaRatingGanadas = float.Parse(snapshot.Child("mediaRatingGanadas").Value.ToString());
        mediaRatingPerdidas = float.Parse(snapshot.Child("mediaRatingPerdidas").Value.ToString());
        mediaRatingEmpatadas = float.Parse(snapshot.Child("mediaRatingEmpatadas").Value.ToString());
        mediaGeneralRating = float.Parse(snapshot.Child("mediaGeneralRating").Value.ToString());

        mediaRatingGanadas = float.Parse(snapshot.Child("mediaRatingGanadasFra").Value.ToString());
        mediaRatingPerdidasFra = float.Parse(snapshot.Child("mediaRatingPerdidasFra").Value.ToString());
        mediaRatingEmpatadasFra = float.Parse(snapshot.Child("mediaRatingEmpatadasFra").Value.ToString());
        mediaGeneralRatingFra = float.Parse(snapshot.Child("mediaGeneralRatingFra").Value.ToString());
        winsFra = int.Parse(snapshot.Child("winsFra").Value.ToString());
        drawsFra = int.Parse(snapshot.Child("drawsFra").Value.ToString());
        losesFra = int.Parse(snapshot.Child("losesFra").Value.ToString());

        mediaRatingGanadasHeal = float.Parse(snapshot.Child("mediaRatingGanadasHeal").Value.ToString());
        mediaRatingPerdidasHeal = float.Parse(snapshot.Child("mediaRatingPerdidasHeal").Value.ToString());
        mediaRatingEmpatadasHeal = float.Parse(snapshot.Child("mediaRatingEmpatadasHeal").Value.ToString());
        mediaGeneralRatingHeal = float.Parse(snapshot.Child("mediaGeneralRatingHeal").Value.ToString());
        winsHeal = int.Parse(snapshot.Child("winsHeal").Value.ToString());
        drawsHeal = int.Parse(snapshot.Child("drawsHeal").Value.ToString());
        losesHeal = int.Parse(snapshot.Child("losesHeal").Value.ToString());

        mediaRatingGanadasDuel = float.Parse(snapshot.Child("mediaRatingGanadasDuel").Value.ToString());
        mediaRatingPerdidasDuel = float.Parse(snapshot.Child("mediaRatingPerdidasDuel").Value.ToString());
        mediaRatingEmpatadasDuel = float.Parse(snapshot.Child("mediaRatingEmpatadasDuel").Value.ToString());
        mediaGeneralRatingDuel = float.Parse(snapshot.Child("mediaGeneralRatingDuel").Value.ToString());
        winsDuel = int.Parse(snapshot.Child("winsDuel").Value.ToString());
        drawsDuel = int.Parse(snapshot.Child("drawsDuel").Value.ToString());
        losesDuel = int.Parse(snapshot.Child("losesDuel").Value.ToString());

        mediaRatingGanadasNone = float.Parse(snapshot.Child("mediaRatingGanadasNone").Value.ToString());
        mediaRatingPerdidasNone = float.Parse(snapshot.Child("mediaRatingPerdidasNone").Value.ToString());
        mediaRatingEmpatadasNone = float.Parse(snapshot.Child("mediaRatingEmpatadasNone").Value.ToString());
        mediaGeneralRatingNone = float.Parse(snapshot.Child("mediaGeneralRatingNone").Value.ToString());
        winsNone = int.Parse(snapshot.Child("winsNone").Value.ToString());
        drawsNone = int.Parse(snapshot.Child("drawsNone").Value.ToString());
        losesNone = int.Parse(snapshot.Child("losesNone").Value.ToString());

        //Load Saved Games
        //for (int i = 0; i < NUM_SAVED_MATCHES; i++)
        //{
        //    string aux = snapshot.Child("zzzLastGames").Child("Partida " + i).Value.ToString();
        //    lastMatches[i] = aux;
        //}
    }

    public void UpdateUserHistory(team winner, int redDamage, int blueDamage, string partnerRole)
    {
        gamesPlayed++;
        Debug.Log("MI compañero es: " + partnerRole);

        float distanciaMediaPartida = RoundData.totalDistance / RoundData.hitEnemyShoots;
        distanceMedia = (distanceMedia * gamesPlayed - 1) / gamesPlayed;


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
        string resultLastMatch = "Win";
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
                    resultLastMatch = "Lost";
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
                else
                {
                    resultLastMatch = "Lost";
                    loses++;
                    eloK--;
                    SA = 0f;
                }
                break;
            case team.none:
                resultLastMatch = "Draw";
                draws++;
                SA = 0.5f;
                break;
        }


        if (gamesPlayed < 10)
        {
            eloK = 40;
        }
        else
        {
            if (eloRanking < 2100)
            {
                eloK = 32;
            }
            else if (eloRanking > 2400)
            {
                eloK = 16;
            }
            else
            {
                eloK = 24;
            }
        }

        if (RoundData.isRed)
            E = ELO.GetRedChances();
        else
            E = ELO.GetBlueChances();

        Debug.Log("CHANCES UH: las chances de ganar del red son: " + ELO.redChances);
        //MEDIAS

        killsDeathsRatioAverage = kills;
        if (deaths > 0)
            killsDeathsRatioAverage = ((float)kills) / (float)deaths;

        dps = ((float)damageInflicted) / ((float)gamesPlayed * 120.0f); //damage per second


        // SET LAST ROLE:
        float aux = ((float)RoundData.damageInflicted) / ((float)RoundData.damageReceived); //0.7272
        float dps_in_match = ((float)RoundData.damageInflicted) / 120.0f;   //33.3

        //PUEDE SER FRANCOTIRADOR O HEALER
        if (aux > 1.3f && RoundData.healedPlayers > 4000f)
        {
            if (aux / 1.3f > ((float)RoundData.healedPlayers) / 4000f)
            {
                lastRole = "Sniper";
            }
            else
            {
                lastRole = "Healer";
            }
        }
        else if (aux < 1.3f && RoundData.healedPlayers > 4000)
        {
            if (dps_in_match / 30 > ((float)RoundData.healedPlayers) / 4000.0f)
            {
                lastRole = "Duelist";
            }
            else
            {
                lastRole = "Healer";
            }
        }
        else if (aux > 1.3f && RoundData.healedPlayers < 4000)
        {
            lastRole = "Sniper";
        }
        else if (aux != 0 && RoundData.healedPlayers != 0)
        {
            if (dps_in_match / 30 > ((float)RoundData.healedPlayers) / 4000.0f)
            {
                lastRole = "Duelist";
            }
            else
            {
                lastRole = "Healer";
            }
        }
        else
        {
            lastRole = "None";
        }

        // SET GLOBAL ROLE
        switch (lastRole)
        {
            case "Sniper":
                numFranc++;
                if (resultLastMatch == "Win")
                    winsFra++;
                else if (resultLastMatch == "Lost")
                    losesFra++;
                else drawsFra++;

                break;
            case "Healer":
                numHeal++;
                if (resultLastMatch == "Win")
                    winsHeal++;
                else if (resultLastMatch == "Lost")
                    losesHeal++;
                else drawsHeal++;
                break;
            case "Duelist":
                numDuel++;
                if (resultLastMatch == "Win")
                    winsDuel++;
                else if (resultLastMatch == "Lost")
                    losesDuel++;
                else drawsDuel++;
                break;
            default:
                numNone++;
                if (resultLastMatch == "Win")
                    winsNone++;
                else if (resultLastMatch == "Lost")
                    losesNone++;
                else drawsNone++;
                break;
        }

        if (numFranc >= numHeal && numFranc >= numDuel && numFranc >= numNone)
        {
            playerRole = "Sniper";
        }
        else if (numHeal >= numFranc && numHeal >= numDuel && numHeal >= numNone)
        {
            playerRole = "Healer";
        }
        else if (numDuel >= numFranc && numDuel >= numHeal && numDuel >= numNone)
        {
            playerRole = "Duelist";
        }
        else
        {
            playerRole = "None";
        }

        float realContribution;

        if (RoundData.isRed)
            realContribution = calculateContribution(redDamage, blueDamage, partnerRole);
        else
            realContribution = calculateContribution(blueDamage, redDamage, partnerRole);

        float eloMultiplayer = realContribution / RoundData.expectedContribution; //Multiplicador de Elo

        float x = eloMultiplayer * 1.5f; //Para llegar a los margenes de la campana de Gauss

        float mu = 1.5f;
        float sigma = 0.4f;
        float yGauss = calculateGauss(x, mu, sigma);

        if (eloMultiplayer > 1)
        {
            eloMultiplayer = 2 - yGauss;
        }
        else eloMultiplayer = yGauss;

        float oldElo = eloRanking;
        eloRanking = ELO.CalculteNewElo(eloRanking, eloK, SA, E); // LOLITOOOOOO

        float diffElo = Mathf.Abs(oldElo - eloRanking); //Elo conseguido en esta partida

        eloRanking = oldElo;

        float diffContr = Mathf.Abs(diffElo - (float)(diffElo * eloMultiplayer));

        if (diffElo < 1) diffElo = 1;

        switch (winner)
        {
            case team.red:
                if (RoundData.isRed)
                {
                    eloRanking += diffElo + diffContr;
                }
                else
                {
                    eloRanking -= diffElo - diffContr;
                }
                break;
            case team.blue:
                if (!RoundData.isRed)
                {
                    eloRanking -= diffElo - diffContr;
                }
                else
                {
                    eloRanking += diffElo + diffContr;
                }
                break;
            case team.none:
                if (E > 0.5)
                {
                    eloRanking -= diffElo - diffContr;
                }
                else if (E < 0.5)
                {
                    eloRanking += diffElo + diffContr;
                }
                else
                {
                    if (eloMultiplayer > 1)
                    {
                        eloRanking += diffElo + diffContr;
                    }
                    else if (eloMultiplayer < 1) //Sio es igual a 1 es que ha hecho lo que deberia y nos da igual
                    {
                        eloRanking -= diffElo - diffContr;
                    }
                }
                break;
        }


        Debug.Log("Expectativa contribucion: " + RoundData.expectedContribution);
    }

    private float calculateContribution(int myTeamDamage, int enemyTeamDamage, string partnerRole)//Parte del 100% del hecho por el grupo del jugador
    {
        int lifeContribution;
        int damageContribution;
        int healContribution;
        int totalHeal = RoundData.healedMyLife + RoundData.healedPlayers;

        float contribution = 0;
        switch (playerRole)
        {
            case "Sniper":
                switch (partnerRole)
                {
                    case "Sniper":
                        lifeContribution = 45;
                        damageContribution = 50;
                        healContribution = 5;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Healer":
                        lifeContribution = 30;
                        damageContribution = 70;
                        healContribution = 0;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Duelist":
                        lifeContribution = 25;
                        damageContribution = 50;
                        healContribution = 25;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "None":
                        lifeContribution = 40;
                        damageContribution = 40;
                        healContribution = 20;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    default: //Error el rol none no deberia existir
                        lastRole = "UserHistory linea x este rol no deberia existir ERROR";
                        break;
                }
                break;
            case "Healer":
                switch (partnerRole)
                {
                    case "Sniper":
                        lifeContribution = 40;
                        damageContribution = 10;
                        healContribution = 50;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Healer":
                        lifeContribution = 33;
                        damageContribution = 33;
                        healContribution = 34;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Duelist":
                        lifeContribution = 25;
                        damageContribution = 25;
                        healContribution = 50;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "None":
                        lifeContribution = 25;
                        damageContribution = 25;
                        healContribution = 50;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    default: //Error el rol none no deberia existir
                        lastRole = "UserHistory linea x este rol no deberia existir ERROR";
                        break;
                }
                break;
            case "Duelist":
                switch (partnerRole)
                {
                    case "Sniper":
                        lifeContribution = 70;
                        damageContribution = 30;
                        healContribution = 0;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Healer":
                        lifeContribution = 25;
                        damageContribution = 75;
                        healContribution = 0;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "Duelist":
                        lifeContribution = 50;
                        damageContribution = 50;
                        healContribution = 0;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    case "None":
                        lifeContribution = 40;
                        damageContribution = 40;
                        healContribution = 20;


                        // Vida
                        if (enemyTeamDamage == 0)
                            contribution += lifeContribution / 2;
                        else
                        {
                            float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                            contribution += lifeContribution * ratioLife;
                        }
                        // Ataque
                        if (myTeamDamage == 0)
                            contribution += damageContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                            contribution += damageContribution * ratioLife;
                        }
                        //Heal
                        if (totalHeal == 0)
                            contribution += healContribution / 2;
                        else
                        {
                            float ratioLife = ((float)RoundData.healedPlayers / (float)totalHeal);

                            contribution += healContribution * ratioLife;
                        }
                        break;
                    default: //Error el rol none no deberia existir
                        lastRole = "UserHistory linea x este rol no deberia existir ERROR";
                        break;
                }
                break;
            case "None":
                lifeContribution = 40;
                damageContribution = 40;
                healContribution = 20;

                // Vida
                if (enemyTeamDamage == 0)
                    contribution += lifeContribution / 2;
                else
                {
                    float ratioLife = 1f - (float)((float)RoundData.damageReceived / (float)enemyTeamDamage);

                    contribution += lifeContribution * ratioLife;
                }
                // Ataque
                if (myTeamDamage == 0)
                    contribution += damageContribution / 2;
                else
                {
                    float ratioLife = ((float)RoundData.damageInflicted / (float)myTeamDamage);

                    contribution += damageContribution * ratioLife;
                }
                //Heal
                if (totalHeal == 0)
                    contribution += healContribution / 2;
                else
                {
                    float ratioLife = ((float)healedOthersLife / (float)totalHeal);

                    contribution += healContribution * ratioLife;
                }
                break;
            default: //Error el rol none no deberia existir
                lastRole = "UserHistory linea x este rol no deberia existir ERROR";
                break;
        }
        return contribution;
    }

    private float calculateGauss(float x, float mu, float sigma)
    {
        float y = 1.0f / (sigma * Mathf.Sqrt(2.0f * Mathf.PI)) * Mathf.Exp(-(Mathf.Pow((x - mu) / sigma, 2f) / 2.0f));
        if (y > 0.99) y = 1;
        return y;
    }
}
