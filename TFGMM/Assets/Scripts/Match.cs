using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMatch
{
    public PlayerMatch(int n)
    {
        name = name +n;
    }
    public PlayerMatch(string n, team myTeam)
    {
        name = n;
        t = myTeam;
    }

    public PlayerMatch(string n, int k, int d, int tDamage, int damageR, int tshots, team myTeam)
    {
        name = n;
        kills = k;
        deaths = d;
        damageInflicted = tDamage;
        damageReceived = damageR;
        totalShots = tshots;
        t = myTeam;
    }

    public string name = "Jugador ";
    public int kills = 0;
    public int deaths = 0;
    public int damageInflicted = 0;
    public int damageReceived = 0;
    public int totalShots = 0; //Para saber el porcentaje de acierto multiplicar por 500(El daño que recibe un jugador) y dividir con daño hecho
    public team t = team.red;
}

public class Match
{
    public PlayerMatch[] players;
    public team winner = team.red;
    public int pointsBlue = 0;
    public int pointsRed = 0;


    //EA should be the chances that the red team win and EB the blue's one
    private float EA { get; set;}
    private float EB { get; set;}


    public Match(int n)
    {
        players = new PlayerMatch[n];
    }

    public void addPlayer(string name, team t, int i)
    {
        players[i] = new PlayerMatch(name, t);
    }

    public Match(team w, DataSnapshot info)
    {
        winner = w;

        for (int i = 0; i < 6; i++)
        {
            string name = info.Child("Jugador " + i).Child("name").Value.ToString();
            int kills = int.Parse(info.Child("Jugador " + i).Child("kills").Value.ToString().ToString());
            int deaths = int.Parse(info.Child("Jugador " + i).Child("deaths").Value.ToString().ToString());
            int totalDamage = int.Parse(info.Child("Jugador " + i).Child("totalDamage").Value.ToString().ToString());
            int damageReceived = int.Parse(info.Child("Jugador " + i).Child("damageReceived").Value.ToString().ToString());
            int totalShots = int.Parse(info.Child("Jugador " + i).Child("totalShots").Value.ToString().ToString());
            team t =(team) int.Parse(info.Child("Jugador " + i).Child("t").Value.ToString().ToString());

            players[i] = new PlayerMatch(name, kills, deaths, totalDamage, damageReceived, totalShots,  t);
        }
    }

    public void killed(string killed, string killedBy)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].name == killed)
            {
                players[i].deaths++;
            }
            else if (players[i].name == killedBy)
            {
                players[i].kills++;
            }
        }
    }

    public void damaged(string damaged, string damagedBy)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].name == damaged)
            {
                players[i].damageReceived +=500;
            }
            else if (players[i].name == damagedBy)
            {
                players[i].damageInflicted += 500;
            }
        }
    }

    public void shoot(string shotBy)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].name == shotBy)
            {
                players[i].totalShots++;
                break;
            }
        }
    }
    //"{\"name\":\"Este es mi usuario\",\"kills\":0,\"deaths\":0,\"totalDamage\":0,\"damageReceived\":0,\"totalShots\":0,\"t\":0}"

    public string playerJSON(int i)
    {
        string s = JsonUtility.ToJson(players[i]);
        return s;
    }
}
