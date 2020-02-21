using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxReborn : MonoBehaviour
    {
        public string itemExchange;

        public int freeReborn = 3;
        public Localize labelFree;
        public int[] priceReborn;
        public UILabel priceRebornlabel;
        public UIButton btnWatch;
        [HideInInspector]
        private int currentReborn = 0;

        public int CurrentReborn { get => currentReborn; set
            {
                currentReborn = value;
                
            }

        }

        public int getPriceRebornCrystal()
        {
            return currentReborn >= priceReborn.Length ? priceReborn[priceReborn.Length - 1] : priceReborn[currentReborn];
        }

        public void watchReborn()
        {
            freeReborn--;
            btnWatch.isEnabled = freeReborn > 0;
            CurrentReborn++;
            if (labelFree)
            {
                labelFree.TermSuffix = freeReborn.ToString() + "/3";
                labelFree.OnLocalize(true);
            }
        }

        public void resetFreeTurn()
        {
            CurrentReborn = 0;
            freeReborn = 3;
            btnWatch.isEnabled = freeReborn > 0;
            if (labelFree)
            {
                labelFree.TermSuffix = freeReborn.ToString() + "/3";
                labelFree.OnLocalize(true);
            }
        }
    }
}
