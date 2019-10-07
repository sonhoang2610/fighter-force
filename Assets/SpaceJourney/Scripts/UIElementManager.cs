using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;
using EazyEngine.Tools;
using DG.Tweening;
using Spine.Unity;

public struct UIMessEvent
{
    public string Event;
    public UIMessEvent(string pEvent)
    {
        Event = pEvent;
    }
}
public class UIElementManager : Singleton<UIElementManager>
{
    public UIPanel root;
    public EazyAction getTween(string pID,GameObject pObjectTarget = null)
    {
        if (pID == "FadeOut")
        {
            return NGUIFadeAction.to(0, 0.25f);
        }else if(pID == "FadeIn")
        {
            return NGUIFadeAction.to(1, 0.25f);
        }
        else if (pID.StartsWith("Move"))
        {
            string[] pInfos  = pID.Split('/');
            if(pInfos[1] == "H")
            {
                return EazyMove.by(new Vector2(int.Parse(pInfos[2]),0), 0.25f);
            }
            if (pInfos[1] == "V")
            {
                return EazyMove.by(new Vector2(0,int.Parse(pInfos[2])), 0.25f);
            }
        }
        return null;
    }

    public bool doAction(UIElement pElement,bool isActive,System.Action pComplete = null)
    {
        bool pAction = false;
        if (pElement.cateGory.Contains("Fade"))
        {
            pAction = true;
            var pSprites = pElement.gameObject.GetComponentsInChildren<SpriteRenderer>();
            var pSkes = pElement.gameObject.GetComponentsInChildren<SkeletonMecanim>();

            void PUpdateExtension(float pFloat)
            {
                foreach (var pSprite in pSprites)
                {
                    Color pColor = pSprite.color;
                    pColor.a = pFloat;
                    pSprite.color = pColor;
                }

                foreach (var pSke in pSkes)
                {
                    pSke.Skeleton.A = pFloat;
                }
            }

            if (isActive)
            {
   
                RootMotionController.runAction(pElement.gameObject,Sequences.create  (((EazyFloatAction) getTween("FadeIn")).setUpdateEvent(PUpdateExtension),CallFunc.create(
                    () => pComplete?.Invoke())));
            }
            else
            {
                RootMotionController.runAction(pElement.gameObject,Sequences.create( ((EazyFloatAction)getTween("FadeOut")).setUpdateEvent(PUpdateExtension), CallFunc.create(delegate {
                    pComplete?.Invoke();
                    pElement.gameObject.SetActive(false);
                })));
            }
        }
        if (pElement.cateGory.Contains("MoveFrom"))
        {
            int pSide = 1;
            if (pElement.cateGory.Contains("Left"))
            {
                pSide = -1;
            }
            Vector3 pPos = pElement.transform.localPosition;
            if (!cachePos.ContainsKey(pElement))
            {
                cachePos.Add(pElement,pElement.transform.localPosition);
            }
            else
            {
                pPos = cachePos[pElement];
            }
            if (isActive)
            {
                pElement.transform.localPosition = pPos+ new Vector3(root.GetWindowSize().x * pSide,0,0);
                RootMotionController.runAction(pElement.gameObject,Sequences.create  (EazyMove.to(pPos,0.75f).setEase(EaseCustomType.easeOutExpo),CallFunc.create(
                    () => pComplete?.Invoke())));
            }
            else
            {
                RootMotionController.runAction(pElement.gameObject,Sequences.create( EazyMove.to(pPos + new Vector3(root.GetWindowSize().x * pSide, 0, 0), 0.75f).setEase(EaseCustomType.easeOutExpo),CallFunc.create(delegate {
                    pComplete?.Invoke();
                    pElement.gameObject.SetActive(false);
                })));
            }
        }
        return pAction;
    }

    protected Dictionary<UIElement, Vector3> cachePos = new Dictionary<UIElement, Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
