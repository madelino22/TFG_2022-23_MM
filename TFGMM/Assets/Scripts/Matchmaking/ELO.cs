using System;
using System.Collections;
using System.Collections.Generic;
public static class ELO
{
    #region Calculate E (Chances of winning before playing the match)
    
    
    //These two methods return each team E (EA and EB)
    public static Tuple<int, int> CalculateWinningChances(int averageEA, int averageEB)
    {
        int EA = (int)(1 / (1 + Math.Pow(10f, (averageEB - averageEA) / 400)));
        int EB = (int)(1 / (1 + Math.Pow(10f, (averageEA - averageEB) / 400)));
        return new Tuple<int, int>(EA, EB);
    }



    public static Tuple<int, int> CalculateWinningChances(List<int> RA, List<int> RB)
    {
        int averageRatingA = 0;
        foreach (int individualRA in RA)
        {
            averageRatingA += individualRA;
        }
        averageRatingA = averageRatingA / RA.Count;

        int averageRatingB = 0;
        foreach (int individualRB in RB)
        {
            averageRatingB += individualRB;
        }
        averageRatingB = averageRatingB / RB.Count;

        int EA = (int)(1 / (1 + Math.Pow(10f, (averageRatingB - averageRatingA) / 400)));
        int EB = (int)(1 / (1 + Math.Pow(10f, (averageRatingA - averageRatingB) / 400)));


        return new Tuple<int, int>(EA,EB);  
    }


    #endregion
    public static int CalculteNewElo(int RA, int K, int SA, int EA)
    {
        int RÀ = RA + K * (SA - EA);
        Math.Clamp(RÀ, 0, 3000);
        return RÀ;
    }
}
