using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseManager : MonoBehaviour
{
    private enum rankUpdate
    {
        promotion, downgrade, none
    }

    [SerializeField]
    public GameObject winLoseText;

    [Header("Players Info")]
    [SerializeField]
    public GameObject leftPlayer;

    [SerializeField]
    public GameObject midPlayer;

    [SerializeField]
    public GameObject rightPlayer;

    [Header("Rank Info")]
    [SerializeField]
    public GameObject progressBar;

    [SerializeField]
    public GameObject actualRank;

    [SerializeField]
    public GameObject nextRank;

    [Header("Left Player Rank")]
    [SerializeField]
    public GameObject leftProgress;

    [SerializeField]
    public GameObject leftRealRank;

    [SerializeField]
    public GameObject leftNewRank;

    rankUpdate leftUpdate = rankUpdate.none;


    [Header("Right Player Rank")]
    [SerializeField]
    public GameObject rightProgress;

    [SerializeField]
    public GameObject rightrealRank;

    [SerializeField]
    public GameObject rightNewRank;

    rankUpdate rightUpdate = rankUpdate.none;



    private float amountProgress = 0;

    private float multiplayer = 1;

    void Awake()
    {
        //Win Lose Text
        if(ComInfo.getGameResult() == result.win)   winLoseText.GetComponent<TextMeshProUGUI>().text = "Victory!";
        else if(ComInfo.getGameResult() == result.lose)   winLoseText.GetComponent<TextMeshProUGUI>().text = "Defeat!";
        else winLoseText.GetComponent<TextMeshProUGUI>().text = "Draw";

        leftPlayer.GetComponent<TextMeshProUGUI>().text = "Izquierdo";
        rightPlayer.GetComponent<TextMeshProUGUI>().text = "Derecho";
        midPlayer.GetComponent<TextMeshProUGUI>().text = ComInfo.getPlayerName();

        //Modify ranks images when we have them

        progressBar.GetComponent<Slider>().value = ComInfo.getRankProgress();

        //Decide with the algorithm the progress
        amountProgress = 20f;


        ComInfo.addRankProgress(amountProgress);


        //Set Rank Progresss visual

        rightProgress.GetComponent<Image>().CrossFadeAlpha(1, 2.0f, false);


    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgressBar();









    }

    private void UpdateProgressBar()
    {
        if (progressBar.GetComponent<Slider>().value < ComInfo.getRankProgress())
        {
            float auxValue = progressBar.GetComponent<Slider>().value;

            float progress = Time.deltaTime * multiplayer;
            multiplayer *= 1.1f;

            auxValue += progress;

            if (auxValue > ComInfo.getRankProgress())
            {
                auxValue = ComInfo.getRankProgress();
            }

            progressBar.GetComponent<Slider>().value = auxValue;
        }
    }
}
