using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    public abstract class CharacrerAbility : MonoBehaviour
    {
        public virtual void initialAbility()
        {

        }
            
        public virtual void EveryFrame(float pDeltaTime)
        {

        }
        public virtual void InputButton(string pTrigger)
        {

        }

        public virtual void addChild(Character pChild)
        {

        }
    }
}
