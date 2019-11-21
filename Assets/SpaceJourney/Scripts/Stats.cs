/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TypeStats
{
    Attack,
    Defence,
    Lucky,
    SpeedAttach,
    Health,
}
public class Stats
{

    public event EventHandler OnStatsChanged;

    public static int STAT_MIN = 0;
    public static int STAT_MAX = 100;



    private SingleStat[] stats;

    public SingleStat[] _Stats { get => stats; set => stats = value; }

    public Stats(int attackStatAmount, int defenceStatAmount, int lcuky, int speed, int healthStatAmount)
    {
        _Stats = new SingleStat[5] {
            new SingleStat(TypeStats.Attack,attackStatAmount),
              new SingleStat(TypeStats.Defence,defenceStatAmount),
                new SingleStat(TypeStats.Lucky,lcuky),
                    new SingleStat(TypeStats.SpeedAttach,speed),
                new SingleStat(TypeStats.Health,healthStatAmount),
                
                
        };
    }

    public Stats(int attackStatAmount, int lcuky, int speed)
    {
        _Stats = new SingleStat[3] {
            new SingleStat(TypeStats.Attack,attackStatAmount),
                  new SingleStat(TypeStats.Lucky,lcuky),
                    new SingleStat(TypeStats.SpeedAttach,speed),
        };
    }


    private SingleStat GetSingleStat(TypeStats statTypeStats)
    {
        foreach (var pSingle in _Stats)
        {
            if (pSingle.typeStat == statTypeStats)
            {
                return pSingle;
            }
        }
        return null;
    }

    public void SetStatAmount(TypeStats statTypeStats, int statAmount)
    {
        GetSingleStat(statTypeStats).SetStatAmount(statAmount);
        if (OnStatsChanged != null) OnStatsChanged(this, EventArgs.Empty);
    }
    public void SetStatAmount(int indexStat, int statAmount)
    {
        stats[indexStat].SetStatAmount(statAmount);
        if (OnStatsChanged != null) OnStatsChanged(this, EventArgs.Empty);
    }

    public void IncreaseStatAmount(TypeStats statTypeStats)
    {
        SetStatAmount(statTypeStats, GetStatAmount(statTypeStats) + 1);
    }

    public void DecreaseStatAmount(TypeStats statTypeStats)
    {
        SetStatAmount(statTypeStats, GetStatAmount(statTypeStats) - 1);
    }

    public int GetStatAmount(TypeStats statTypeStats)
    {
        return GetSingleStat(statTypeStats).GetStatAmount();
    }
    public int GetStatAmount(int indexStat)
    {
        return stats[indexStat].GetStatAmount();
    }
    public float GetStatAmountNormalized(TypeStats statTypeStats)
    {
        return GetSingleStat(statTypeStats).GetStatAmountNormalized();
    }
    public float GetStatAmountNormalized(int indexStat)
    {
        return stats[indexStat].GetStatAmountNormalized();
    }


    /*
     * Represents a Single Stat of any TypeStats
     * */
    public class SingleStat
    {

        private int stat;
        public TypeStats typeStat;
        public SingleStat(TypeStats pType, int statAmount)
        {
            SetStatAmount(statAmount);
            typeStat = pType;
        }

        public void SetStatAmount(int statAmount)
        {
            stat = Mathf.Clamp(statAmount, STAT_MIN, STAT_MAX);
        }

        public int GetStatAmount()
        {
            return stat;
        }

        public float GetStatAmountNormalized()
        {
            return (float)stat / STAT_MAX;
        }
    }
}
