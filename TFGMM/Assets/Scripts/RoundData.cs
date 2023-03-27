using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//INFORMACION DEL JUGADOR EN LA PARTIDA ACTUAL

public static class RoundData
{
    public static int kills = 0;

    public static int deaths = 0;

    public static int totalShots = 0;

    public static int damageInflicted = 0; //Damage Inflicted on Enemy
    public static int damageReceived = 0; //Damage Received by Player

    public static int healedLife = 0;

    public static bool isRed = true;

    public static void ResetData()
    {
        kills = 0;
        deaths = 0;
        damageInflicted = 0; //Damage Inflicted on Enemy
        damageReceived = 0; //Damage Received by Player
        healedLife = 0;
        totalShots = 0;
    }
}
