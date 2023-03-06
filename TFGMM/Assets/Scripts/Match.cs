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
    public PlayerMatch[] players;

    public team winner = team.red;


    public Match(int n)
    {
        players = new PlayerMatch[n];
    }

    public void addPlayer(string name, team t, int i)
    {
        players[i] = new PlayerMatch(name, 0, 0, 0, t);
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

    

    public string playerJSON(int i)
    {
        return JsonUtility.ToJson(players[i]);
    }
}
