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
    public int eloRanking = 1500;
    public float dps = 0;
    public int gamesPlayed = 0;
    public int wins = 0;
    public int draws = 0;
    public int loses = 0;
    public int kills = 0;
    public int deaths = 0;
    public int killsDeathsRatioAverage = 0;
    public int damageReceivedPerGame = 0;
    public int healedLifePerGame = 0;
    public int zzlastGameSaved = 0;
    private const int NUM_SAVED_MATCHES = 5;

    public int totalShots = 0;
    //private string[] lastMatches = new string[NUM_SAVED_MATCHES];

    //public string saveGames(int index)
    //{
    //    return JsonUtility.ToJson(lastMatches[index]);
    //}

    //public string initLastMatchesAtNull(int i)
    //{
    //    lastMatches[i] = "Partida Null";
    //    return JsonUtility.ToJson(lastMatches[i]);
    //}

    //public string lastGameNotSaved(string newGame)
    //{
    //    lastMatches[zzlastGameSaved] = newGame;
    //    zzlastGameSaved++;
    //    zzlastGameSaved %= NUM_SAVED_MATCHES;

    //    return JsonUtility.ToJson(newGame);
    //}

    //public string saveGamePlayer(int indexGame, int nPlayer)
    //{
    //    return JsonUtility.ToJson(lastMatches[indexGame].players[nPlayer]);
    //}

    public void loadInfo(DataSnapshot snapshot)
    {
        userName = snapshot.Child("userName").Value.ToString();
        email = snapshot.Child("email").Value.ToString();
        eloRanking = int.Parse(snapshot.Child("eloRanking").Value.ToString());
        dps = float.Parse(snapshot.Child("dps").Value.ToString());
        gamesPlayed = int.Parse(snapshot.Child("gamesPlayed").Value.ToString());
        wins = int.Parse(snapshot.Child("wins").Value.ToString());
        draws = int.Parse(snapshot.Child("draws").Value.ToString());
        loses = int.Parse(snapshot.Child("loses").Value.ToString());
        kills = int.Parse(snapshot.Child("kills").Value.ToString());
        deaths = int.Parse(snapshot.Child("deaths").Value.ToString());
        killsDeathsRatioAverage = int.Parse(snapshot.Child("killsDeathsRatioAverage").Value.ToString());
        healedLifePerGame = int.Parse(snapshot.Child("healedLifePerGame").Value.ToString());
        damageReceivedPerGame = int.Parse(snapshot.Child("damageReceivedPerGame").Value.ToString());
        zzlastGameSaved = int.Parse(snapshot.Child("damageReceivedPerGame").Value.ToString());

        totalShots = int.Parse(snapshot.Child("totalShots").Value.ToString());
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

        eloRanking += 0; // LOLITOOOOOO

        int daño = RoundData.damage;

        int dpsThisGame = daño / 25; //15 HASTA QUE PONGAS 90 CON PARTIDAS BIEN
        dps = ((gamesPlayed - 1) * dps + dpsThisGame) / (gamesPlayed); //damage per second

        switch (winner)
        {
            case team.red:
                if (RoundData.isRed) wins++;
                else loses++;
                break;
            case team.blue:
                if (!RoundData.isRed) wins++;
                else loses++;
                break;
            case team.none:
                draws++;
                break;
        }

        kills += RoundData.kills;
        deaths += RoundData.deaths;

        totalShots += RoundData.totalShots;

        if (RoundData.deaths != 0) //Don't divide by zero
            killsDeathsRatioAverage = ((gamesPlayed - 1) * killsDeathsRatioAverage + (RoundData.kills / RoundData.deaths)) / (gamesPlayed);

        //Damage Received by Player
        damageReceivedPerGame = ((gamesPlayed - 1) * damageReceivedPerGame + RoundData.damageReceived) / (gamesPlayed);

        healedLifePerGame = ((gamesPlayed - 1) * healedLifePerGame + RoundData.healedLife) / (gamesPlayed);
    }

    // METODOSO DE ProtocolToken--------------------------------------------
    //public override void Read(UdpPacket packet)
    //{
    //    // Deserialize Token data

    //    userName = packet.ReadString();
    //    email = packet.ReadString();
    //    eloRanking = packet.ReadInt();
    //    dps = packet.ReadFloat();
    //    gamesPlayed = packet.ReadInt();
    //    wins = packet.ReadInt();
    //    draws = packet.ReadInt();
    //    loses = packet.ReadInt();
    //    kills = packet.ReadInt();
    //    deaths = packet.ReadInt();
    //    killsDeathsAverage = packet.ReadInt();
    //    healedLifePerGame = packet.ReadInt();
    //    damageReceivedPerGame = packet.ReadInt();
    //    zzlastGameSaved = packet.ReadInt();
    //    //MATCHES???????????????????????'

    //    for (int i = 0; i < NUM_SAVED_MATCHES; i++)
    //    {
    //        string aux = packet.ReadString();

    //        lastMatches[i] = aux;
    //    }


    //}

    //public override void Write(UdpPacket packet)
    //{
    //    // Serialize Token Data
    //    packet.WriteString(userName);
    //    packet.WriteString(email);
    //    packet.WriteInt(email);
    //}

    //public override void Reset()
    //{
    //    // Reset Token Data
    //    SkinId = default(int);
    //    HatId = default(int);
    //}
}
