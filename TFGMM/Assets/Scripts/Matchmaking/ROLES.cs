using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;



//ESTA CLASE ES PARA TENER LA MATRIZ DE CORRELACIÓN, SI SE QUIERE ACCEDER AL ROL POR FIREBASE VA POR STRING,
//NO UTILIZAR ESTA CLASE PARA FIREBASE
public enum RolEnum {NONE, DUELIST, HEALER, SNIPER};
public static class ROLES 
{
    static bool initialized = false;
    static float[,] correlationMatrix = new float[Enum.GetNames(typeof(RolEnum)).Length, Enum.GetNames(typeof(RolEnum)).Length];

    public static void Initialize()
    {
        //None
        correlationMatrix[(int)RolEnum.NONE, (int)RolEnum.NONE] = -0.33f;
        correlationMatrix[(int)RolEnum.NONE, (int)RolEnum.DUELIST] = -0.23f;
        correlationMatrix[(int)RolEnum.NONE, (int)RolEnum.HEALER] = 0f;
        correlationMatrix[(int)RolEnum.NONE, (int)RolEnum.SNIPER] = 0.03f;

        //DUELIST
        correlationMatrix[(int)RolEnum.DUELIST, (int)RolEnum.NONE] = -0.23f;
        correlationMatrix[(int)RolEnum.DUELIST, (int)RolEnum.DUELIST] = 0.23f;
        correlationMatrix[(int)RolEnum.DUELIST, (int)RolEnum.HEALER] = -0.49f;
        correlationMatrix[(int)RolEnum.DUELIST, (int)RolEnum.SNIPER] = 0.25f;

        //HEALER
        correlationMatrix[(int)RolEnum.HEALER, (int)RolEnum.NONE] = 0f;
        correlationMatrix[(int)RolEnum.HEALER, (int)RolEnum.DUELIST] = -0.49f;
        correlationMatrix[(int)RolEnum.HEALER, (int)RolEnum.HEALER] = 0f;
        correlationMatrix[(int)RolEnum.HEALER, (int)RolEnum.SNIPER] = 0.02f;

        //SNIPER
        correlationMatrix[(int)RolEnum.SNIPER, (int)RolEnum.NONE] = 0.03f;
        correlationMatrix[(int)RolEnum.SNIPER, (int)RolEnum.DUELIST] = 0.25f;
        correlationMatrix[(int)RolEnum.SNIPER, (int)RolEnum.HEALER] = 0.02f;
        correlationMatrix[(int)RolEnum.SNIPER, (int)RolEnum.SNIPER] = 0.37f;

        initialized = true;
    }


    public static float GetCorrelation(RolEnum rol1, RolEnum rol2)
    {
        if (!initialized) Initialize();
        return correlationMatrix[(int)rol1, (int)rol2];
    }


    public static ((int, RolEnum), (int, RolEnum))[] FindBestRolePairs((int, RolEnum)[] roles)
    {
        if (!initialized) Initialize();

        ((int, RolEnum), (int, RolEnum))[] bestPairs = new ((int, RolEnum), (int, RolEnum))[2];
        float maxMeanScore = -2f;
        float minDiffScore = -2f;

        for (int i = 0; i < roles.Length; i++)
        {
            for (int j = i + 1; j < roles.Length; j++)
            {
                (int, RolEnum) role1 = roles[i];
                (int, RolEnum) role2 = roles[j];
                float score1 = correlationMatrix[(int)role1.Item2, (int)role2.Item2];

                int a = i;

                while (a == i || a == j)
                {
                    a = (++a % roles.Length);
                }

                int b = a;

                while (b == i || b == j || b == a)
                {
                    b = (++b % roles.Length);
                }

                (int, RolEnum) role3 = roles[a];
                (int, RolEnum) role4 = roles[b];
                float score2 = correlationMatrix[(int)role3.Item2, (int)role4.Item2];

                // Comprobar si el par de roles es mejor que los mejores pares actuales
                float scoreDiff = Math.Abs(score1 - score2);
                float scoreMean = (score1 + score2) / 2f;

                if (scoreMean >= maxMeanScore)
                {
                    if (scoreDiff <= 0.3f || scoreDiff <= minDiffScore)
                    {
                        minDiffScore = scoreDiff;
                        maxMeanScore = scoreMean;
                        bestPairs[0] = (role1, role2);
                        bestPairs[1] = (role3, role4);
                    }
                }

            }
        }

        return bestPairs;
    }
}
