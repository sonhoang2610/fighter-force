using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space
{
    public class AIMachine : AIBrain
    {
        public ObservableList<AIElement> _elements = new ObservableList<AIElement>();
        AIAttackInfo _infoMachine;
        protected LevelState parentState;
        int countDeathElement = 0;
        
        public void onChangeElement()
        {
            if (_infoMachine != null)
            {
                _elements[_elements.Count - 1].setInfo(_infoMachine.infoAI);
                if (_infoMachine.BaseAI == AIBaseType.SelfEmiter)
                {
                    _elements[_elements.Count - 1].CanThinking = true;
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();
            _elements.OnCollectionChange += onChangeElement;
        }      
        public virtual LevelState ParentState
        {
            set
            {
                parentState = value;
            }
            get
            {
                return parentState;
            }
        }
        public void setInfoAIMachine(AIAttackInfo pInfo)
        {
            _infoMachine = pInfo;
            if (_infoMachine.BaseAI == AIBaseType.Group)
            {
                CanThinking = true;
            }
            setInfo(pInfo.infoAI);
            for (int i = 0; i < _elements.Count; ++i)
            {
                if (_infoMachine.BaseAI == AIBaseType.Group)
                {
                    _elements[i].CanThinking = false;
                }
                else
                {
                    _elements[i].CanThinking = true;
                }
                _elements[i].setInfo(pInfo.infoAI);
            }
        }
        public override bool isSightMainPlayer()
        {
            for (int i = 0; i < _elements.Count; ++i)
            {
                if (_elements[i].isSightMainPlayer())
                {
                    return true;
                }
            }
            return false;
        }
        public override void attack()
        {
            List<AIElement> elementRandoms = new List<AIElement>();
            for (int i = 0; i < _elements.Count; ++i)
            {
                if (_elements[i].CanAttack)
                {
                    elementRandoms.Add(_elements[i]);
                }
            }
            int pIndexRandom = Random.Range(0, elementRandoms.Count);
            if (pIndexRandom < elementRandoms.Count)
            {
                if (elementRandoms[pIndexRandom].CanAttack)
                {
                    elementRandoms[pIndexRandom].attack();
                }
            }
        }
        // Start is called before the first frame update
        public override void onRespawn()
        {
            base.onRespawn();
            countDeathElement = 0;
        }

        public void triggerFromElement(string pMess, AIElement pElement)
        {
            if (_elements.Contains(pElement))
            {
                if (pMess == "Death")
                {
                    countDeathElement++;
                    if (countDeathElement == _elements.Count)
                    {
                        CanThinking = false;
                    }
                }
            }
        }


    }
}
