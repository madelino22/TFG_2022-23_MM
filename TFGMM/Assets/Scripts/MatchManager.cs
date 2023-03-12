using Firebase.Database;
using Photon.Bolt;
using Photon.Bolt.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchManager : GlobalEventListener
{
    private enum teams
    {
        blue, red
    }

    // Start is called before the first frame update

    public Text redPointsText;

    private int redPoints = 0;

    public Text bluePointsText;

    private int bluePoints = 0;

    public Text timeText;

    teams myTeam = teams.blue;

    public static int nPlayerRoom = 0;
    public override void OnEvent(setPlayerEvent evnt)
    {
        nPlayerRoom = (int)evnt.nPlayer;
        BoltLog.Warn("SOY EL JUGADOR: " + nPlayerRoom);
    }

    public int getPlayerRoom()
    {
        return nPlayerRoom;
    }

    void Start()
    {
        redPointsText.text = redPoints.ToString();
        bluePointsText.text = bluePoints.ToString();
        timeText.text = "1:30";
    }

    public void endGameScene()
    {
        if (redPoints > bluePoints)
        {
            if (myTeam == teams.red) ComInfo.setGameResult(result.win);
            else ComInfo.setGameResult(result.lose);
        }
        else if (bluePoints > redPoints)
        {
            if (myTeam == teams.red) ComInfo.setGameResult(result.lose);
            else ComInfo.setGameResult(result.win);
        }
        else
        {
            Debug.Log("Empate");
            ComInfo.setGameResult(result.draw);
        }

        SceneManager.LoadScene("WinLose", LoadSceneMode.Single);

        deletePlayersEvent del = deletePlayersEvent.Create(GlobalTargets.OnlyServer);
        del.numPlayer = nPlayerRoom;
        del.Send();
    }

    public void UpdateUI(int blueScore, int redScore, int time)
    {
        bluePointsText.text = blueScore.ToString();
        redPointsText.text = redScore.ToString();

        int coc = time / 60;
        int rest = time % 60;

        if (rest < 10)
            timeText.text = coc.ToString() + ":0" + rest.ToString();
        else
            timeText.text = coc.ToString() + ":" + rest.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //updateTime();

        //if(Input.GetKeyDown(KeyCode.B)) //Testing ScorePoints
        //{
        //    playerKill(0, 5, 0);//0 blue 1 red por ejemplo
        //}
        //else if (Input.GetKeyDown(KeyCode.R)) //Testing ScorePoints
        //{
        //    playerKill(5, 0, 1);//0 blue 1 red por ejemplo
        //}
    }

    public void playerKill(int playerThatKill, int playerKilled, int teamScored)//Cuando lo metais online cambiar los parametros para que se sepa quien hizo la kill...
    {
        if (teamScored == 0)
        {
            bluePoints++;
            bluePointsText.text = bluePoints.ToString();

            //Una lista con los players acceder a los dos jugadores... pasar info pa tener datos
        }
        else
        {
            redPoints++;
            redPointsText.text = redPoints.ToString();

            //Una lista con los players acceder a los dos jugadores... pasar info pa tener datos
        }
    }
}
