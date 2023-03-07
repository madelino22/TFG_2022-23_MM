using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundData : MonoBehaviour
{
    public int rankProgress = 0;

    public ranks soloRank = ranks.silver3;

    public float dps = 0; //damage per second

    public int kills = 0;

    public int deaths = 0;

    public bool won = false;

    public int assists = 0;

    public int totalShots = 0;

    public int damage = 0; //Damage Inflicted on Enemy
    public int damageReceived = 0; //Damage Received by Player

    public int healedLife = 0;

    public void ResetData()
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
