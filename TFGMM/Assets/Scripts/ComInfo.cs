using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum heroes
{
    rojo, azul, verde, amarillo
};

public enum ranks
{
    iron1, iron2, iron3, bronze1, bronze2, bronze3, silver1, 
    silver2, silver3, gold1, gold2, gold3, diamond1, diamond2, 
    diamond3, emerald1, emerald2, emerald3, mystic1, mystic2, 
    mystic3, master
}

public enum result
{
    win, lose, draw
}

public enum team
{
    red, blue, none
}

public struct friendInfo
{
    string name;

    int id;

    ranks solo;
    ranks team;

    bool playing;
    bool conected;

    public friendInfo(string n,int identification, ranks s, ranks t, bool gaming, bool inGame)
    {
        name = n;
        id = identification;

        solo = s;
        team = t;
        playing = gaming;
        conected = inGame; 
    }
}

public static class ComInfo 
{
    public static UserHistory player = new UserHistory();

    public static UserHistory getPlayerData()
    {
        return player;
    }

    public static void setPlayerData(UserHistory newPlayer)
    {
        player = newPlayer;
    }

    public static string getPlayerName()
    {
        return player.userName;
    }

    public static void setPlayerName(string name)
    {
        player.userName = name;
    }

    static string playerID = "#696969";
    public static string getPlayerID()
    {
        return playerID;
    }

    public static void setPlayerID(string id)
    {
        playerID = id;
    }


    static result gameResult = result.win;

    public static result getGameResult()
    {
        return gameResult;
    }

    public static void setGameResult(result gameEnd)
    {
        gameResult = gameEnd;
    }

    public static float getRankProgress()
    {
        return player.rankProgress;
    }

    public static void addRankProgress(int progress)
    {
        player.rankProgress += progress;
    }

    public static void setRankProgress(int progress)
    {
        player.rankProgress = progress;
    }

    public static ranks getPlayerSoloRank()
    {
        return player.soloRank;
    }

    static heroes PlayerHero = heroes.rojo;
    public static heroes getPlayerHero() 
    {
        return PlayerHero;
    }
    public static void setHero(heroes newHero)
    {
        PlayerHero = newHero;
    }

    static int playerPoints = 1010;

    public static int getPlayerPoints()
    {
        return playerPoints;
    }
    public static void addPoints(int points)
    {
        playerPoints += points;
    }

    static team t = team.none;

    public static team getTeam()
    {
        return t;
    }

    public static void setTeam(team newTeam)
    {
        t = newTeam;
    }

    static int salaKey = -1;

    public static int getSalaKey()
    {
        return salaKey;
    }
    public static bool setSalaKey(int roomKey, bool host)
    {
        if(roomKey != -1)
        {
            if (host) //Cosas que tendria que hacer el que crea la sala del online
            {
                salaKey = roomKey;
                Debug.Log("Cosas online crear sala");

                return true;
            }
            else
            {
                salaKey = roomKey;
                bool found = false;
                int roomAux = 12345; //get rooms avalibles from server
                int nRooms = 3; //sercver
                int i = 0;

                while (!found && i < nRooms)
                {
                    if (roomKey == roomAux)
                    {
                        found = true;
                    }
                    else
                    {
                        i++;
                        if (i < nRooms) roomKey = 12345; //Server next room open
                    }
                }

                return found;
            }
        }
        else
        {
            //Cerrar sala del online
            Debug.Log("Sala cerrada");

            return true;
        }
        
    }
}
