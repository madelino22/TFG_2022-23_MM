using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMatch
{
    public PlayerMatch(int n)
    {
        name = name + n;
    }

    public string name = "Jugador ";

    public int kills = 0;

    public int damage = 0;

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

    public PlayerMatch[] players = new PlayerMatch[6];

    public team winner = team.red;
}
