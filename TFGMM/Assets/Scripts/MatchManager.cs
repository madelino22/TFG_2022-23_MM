using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    private enum teams
    {
        blue, red
    }

    // Start is called before the first frame update

    public TMPro.TextMeshProUGUI redPointsText;

    private int redPoints = 0;

    public TMPro.TextMeshProUGUI bluePointsText;

    private int bluePoints = 0;

    public TMPro.TextMeshProUGUI timeText;

    private float time = 15f;

    teams myTeam = teams.blue;

    void Start()
    {
        redPointsText.text = redPoints.ToString();
        bluePointsText.text = bluePoints.ToString();
        timeText.text = "2:00";
    }

    // Update is called once per frame
    void Update()
    {
        updateTime();

        if(Input.GetKeyDown(KeyCode.B)) //Testing ScorePoints
        {
            playerKill(0, 5, 0);//0 blue 1 red por ejemplo
        }
        else if (Input.GetKeyDown(KeyCode.R)) //Testing ScorePoints
        {
            playerKill(5, 0, 1);//0 blue 1 red por ejemplo
        }
    }

    private void updateTime()
    {
        time -= Time.deltaTime;

        int mins = (int)(time / 60);
        int secs = (int)(time % 60);


        if (mins > 0)
        {
            string aux;

            aux = mins.ToString() + ":";
            if (secs > 9)
            {
                aux = aux + secs.ToString();
            }
            else
            {
                aux = aux + "0" + secs.ToString();
            }

            timeText.text = aux;
        }
        else if (secs > 0)
        {
            timeText.text = secs.ToString();
        }
        else
        {
            if(redPoints > bluePoints)
            {
                if(myTeam == teams.red) ComInfo.setGameResult(result.win);
                else ComInfo.setGameResult(result.lose);
            }
            else if( bluePoints > redPoints)
            {
                if (myTeam == teams.red) ComInfo.setGameResult(result.lose);
                else ComInfo.setGameResult(result.win);
            }
            else
            {
                ComInfo.setGameResult(result.draw);
            }

           // SceneManager.LoadScene("WinLose", LoadSceneMode.Single);
        }
    }

    public void playerKill(int playerThatKill, int playerKilled, int teamScored)//Cuando lo metais online cambiar los parametros para que se sepa quien hizo la kill...
    {
        if(teamScored == 0)
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
