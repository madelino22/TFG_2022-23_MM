using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMatch
{
    public PlayerMatch(int n)
    {
        name = name + n;
    }

    public PlayerMatch(string n, int k, int d, int tDamage, team myTeam)
    {
        name = n;

        kills = k;

        deaths = d;

        totalDamage = tDamage;

        t = myTeam;
    }

    public string name = "Jugador ";

    public int kills = 0;

    public int deaths = 0;

    public int totalDamage = 0;

    public team t = team.red;
}

public class Match
{
    public Match()
    {
        for(int i = 0; i < 6; i++)
        {
            players[i] = new PlayerMatch(i);
        }
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

            team t =(team) int.Parse(info.Child("Jugador " + i).Child("t").Value.ToString().ToString());

            players[i] = new PlayerMatch(name, kills, deaths, totalDamage, t);
        }
    }

    public PlayerMatch[] players = new PlayerMatch[6];

    public string playerJSON(int i)
    {
        return JsonUtility.ToJson(players[i]);
    }

    public team winner = team.red;
}
