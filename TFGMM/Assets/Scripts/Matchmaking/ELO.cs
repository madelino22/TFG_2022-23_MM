using System;
using System.Collections;
using System.Collections.Generic;
public class ELO
{
    #region Calculate E (Chances of winning before playing the match)
    
    
    //These two methods return each team E (EA and EB)
    public Tuple<float, float> CalculateWinningChances(float averageEA, float averageEB)
    {
        
        float EA = (float)(1 / (1 + Math.Pow(10f, (averageEB - averageEA) / 400)));
        float EB = (float)(1 / (1 + Math.Pow(10f, (averageEA - averageEB) / 400)));
        return new Tuple<float, float>(EA, EB);
    }



    public Tuple<float, float> CalculateWinningChances(List<float> RA, List<float> RB)
    {
        float averageRatingA = 0;
        foreach (float individualRA in RA)
        {
            averageRatingA += individualRA;
        }
        averageRatingA = averageRatingA / RA.Count;

        float averageRatingB = 0;
        foreach (float individualRB in RB)
        {
            averageRatingB += individualRB;
        }
        averageRatingB = averageRatingB / RB.Count;

        float EA = (float)(1 / (1 + Math.Pow(10f, (averageRatingB - averageRatingA) / 400)));
        float EB = (float)(1 / (1 + Math.Pow(10f, (averageRatingA - averageRatingB) / 400)));


        return new Tuple<float, float>(EA,EB);  
    }


    #endregion
    public float CalculteNewElo(float RA, float K, float SA, float EA)
    {
        float RÀ = RA + K * (SA - EA);
        Math.Clamp(RÀ, 0, 3000);
        return RÀ;
    }
}
