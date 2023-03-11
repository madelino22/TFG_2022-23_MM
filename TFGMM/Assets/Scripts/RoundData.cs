using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoundData
{
    public static int rankProgress = 0;
    public static ranks soloRank = ranks.silver3;

    public static float dps = 0; //damage per second

    public static int kills = 0;

    public static int deaths = 0;

    public static bool won = false;

    public static int assists = 0;

    public static int totalShots = 0;

    public static int damage = 0; //Damage Inflicted on Enemy
    public static int damageReceived = 0; //Damage Received by Player

    public static int healedLife = 0;

    public static void ResetData()
    {
        rankProgress = 0;
        soloRank = ranks.silver3;
        dps = 0;
        kills = 0;
        deaths = 0;
        won = false;
        assists = 0;
        damage = 0; //Damage Inflicted on Enemy
        damageReceived = 0; //Damage Received by Player
        healedLife = 0;
    }
}
