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
    public GameObject leftProgress; //How many progress in the match

    [SerializeField]
    public GameObject leftRealRank; //The rank of the player

    [SerializeField]
    public GameObject leftNewRank; // Image visible in case player promote o downgrade



    rankUpdate leftUpdate = rankUpdate.none;

    [Header("Right Player Rank")]
    [SerializeField]
    public GameObject rightProgress; 

    [SerializeField]
    public GameObject rightRealRank;

    [SerializeField]
    public GameObject rightNewRank;

    [Header("Mid Player Rank")]
    [SerializeField]
    public GameObject midProgress;

    rankUpdate rightUpdate = rankUpdate.none;


    private float newRankTimer = 0;

    private float sidesTimer = 0;

    private float amountProgress = 0;

    private float multiplayer = 1;

    private Vector4 colors;

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
        colors = rightProgress.GetComponent<Image>().color;
        colors.w = 0;
        rightProgress.GetComponent<Image>().color = colors;

        colors = leftProgress.GetComponent<Image>().color;
        colors.w = 0;
        leftProgress.GetComponent<Image>().color = colors;

        colors = leftRealRank.GetComponent<Image>().color;
        colors.w = 0;
        leftRealRank.GetComponent<Image>().color = colors;

        colors = rightRealRank.GetComponent<Image>().color;
        colors.w = 0;
        rightRealRank.GetComponent<Image>().color = colors;

        colors = leftNewRank.GetComponent<Image>().color;
        colors.w = 0;
        leftNewRank.GetComponent<Image>().color = colors;

        colors = rightNewRank.GetComponent<Image>().color;
        colors.w = 0;
        rightNewRank.GetComponent<Image>().color = colors;

        colors = midProgress.GetComponent<Image>().color;
        colors.w = 0;
        midProgress.GetComponent<Image>().color = colors;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgressBar();

        //midPlayer
        colors = midProgress.GetComponent<Image>().color;
        if(colors.w < 1)
        {
            colors.w += Time.deltaTime;
            midProgress.GetComponent<Image>().color = colors;
            moveUI(midProgress, new Vector2(0, -1), 50);
        }
        sidesTimer += Time.deltaTime;

        if(sidesTimer > 0.3f)  UpdateImageAnimation();
    }

    private void UpdateProgressBar()
    {
        if (progressBar.GetComponent<Slider>().value < ComInfo.getRankProgress())
        {
            float auxValue = progressBar.GetComponent<Slider>().value;

            float progress = Time.deltaTime * multiplayer;
            multiplayer *= 1.01f;

            auxValue += progress;

            if (auxValue > ComInfo.getRankProgress())
            {
                auxValue = ComInfo.getRankProgress();
            }

            progressBar.GetComponent<Slider>().value = auxValue;
        }
    }

    private void UpdateImageAnimation()
    {
        colors = rightProgress.GetComponent<Image>().color;
        if (colors.w < 1)
        {
            //rightPlayer
            colors.w += Time.deltaTime;
            rightProgress.GetComponent<Image>().color = colors;

            moveUI(rightProgress, new Vector2(0, -1), 40);

            //leftPlayer
            colors = leftProgress.GetComponent<Image>().color;
            colors.w += Time.deltaTime;
            leftProgress.GetComponent<Image>().color = colors;

            moveUI(leftProgress, new Vector2(0, -1), 40);
        }
        else //already visible how they perform
        {
            newRankTimer += Time.deltaTime;

            colors = rightRealRank.GetComponent<Image>().color;
            if (newRankTimer >= 1f && colors.w < 1) //Appear the new rank and if updagraded/downgraded
            {
                //rightPlayer
                colors.w += Time.deltaTime;
                rightRealRank.GetComponent<Image>().color = colors;

                colors = rightNewRank.GetComponent<Image>().color;
                colors.w += Time.deltaTime;
                rightNewRank.GetComponent<Image>().color = colors;

                moveUI(rightRealRank, new Vector2(1, 0), 50);

                moveUI(rightProgress, new Vector2(-1, 0), 45);

                //leftPlayer
                colors = leftRealRank.GetComponent<Image>().color;
                colors.w += Time.deltaTime;
                leftRealRank.GetComponent<Image>().color = colors;

                colors = leftNewRank.GetComponent<Image>().color;
                colors.w += Time.deltaTime;
                leftNewRank.GetComponent<Image>().color = colors;

                moveUI(leftRealRank, new Vector2(1, 0), 50);

                moveUI(leftProgress, new Vector2(-1, 0), 45);
            }
        }
    }

    private void moveUI(GameObject obj, Vector2 dir, int force)
    {
        obj.transform.position = new Vector3(obj.transform.position.x + Time.deltaTime * force * dir.x, obj.transform.position.y + Time.deltaTime * force * dir.y, obj.transform.position.z);
    }
}
