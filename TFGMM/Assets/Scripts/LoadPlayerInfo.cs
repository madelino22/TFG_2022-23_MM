using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadPlayerInfo : MonoBehaviour
{
    [Header("Player Name")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerID;

    [Header("Solo GameObjects")]
    public TextMeshProUGUI soloGames;
    public TextMeshProUGUI soloWins;
    public TextMeshProUGUI soloKills;
    public TextMeshProUGUI soloDeaths;

    [Header("Team GameObjects")]
    public TextMeshProUGUI teamGames;
    public TextMeshProUGUI teamWins;
    public TextMeshProUGUI teamKills;
    public TextMeshProUGUI teamDeaths;

    [Header("Global GameObjects")]
    public TextMeshProUGUI totalGames;
    public TextMeshProUGUI totalWins;
    public TextMeshProUGUI totalKills;
    public TextMeshProUGUI totalDeaths;

    void Start()
    {
        playerName.text = ComInfo.getPlayerName();
        playerID.text = ComInfo.getPlayerID();

        //Here access the info of server

        soloGames.text = "345";
        soloWins.text = "345";
        soloKills.text = "345";
        soloDeaths.text = "345";

        teamGames.text = "346";
        teamWins.text = "346";
        teamKills.text = "346";
        teamDeaths.text = "346";

        totalGames.text = "347";
        totalWins.text = "347";
        totalKills.text = "347";
        totalDeaths.text = "347";
    }
}
