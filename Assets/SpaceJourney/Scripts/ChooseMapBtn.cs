using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class ChooseMapInfo
    {
       public Vector3 pos;
       public int[] star;
        public bool isBoss;
    }
    [System.Serializable]
    public struct BoxraftContainer
    {
        public Vector3 posBox;
        public Vector3 posLocalLine;
        public Vector3 rotationLine;
    }
    public class ChooseMapBtn : BaseItem<ChooseMapInfo>
    {
#if UNITY_EDITOR
        public bool isBoos;
        public GameObject starContainer;
        public Vector2[] starposs;
        public int indexGeneratePos;
        [Button("GeneratePosStar")]
        public void GeneratePosStar()
        {
            starContainer.transform.localPosition = starposs[indexGeneratePos];
        }

        public BoxraftContainer[] posContainer;
        public int indexGenerateBoxCraft;
        [Button("Generate BoxCraft")]
        public void GenerateBoxCraft()
        {
            boxCraft.transform.localPosition = posContainer[indexGenerateBoxCraft].posBox;
            boxCraft.transform.Find("line").localPosition = posContainer[indexGenerateBoxCraft].posLocalLine;
            boxCraft.transform.Find("line").localRotation = Quaternion.Euler(posContainer[indexGenerateBoxCraft].rotationLine);
        }
#endif
        public GameObject boxCraft;
        public UILabel level, levelBoss;
        public GameObject[] stars;
        public EazyTabNGUI tab;
        public Sprite[] normalSprite,bossSprite;
        public Sprite[] disableSprite, bossDisableSprite;
        public override ChooseMapInfo Data { get => base.Data; set {
                transform.localPosition = value.pos;

                for(int i = 0; i <value.star.Length; ++i)
                {
                    for(int j = 3 *i; j < 3 * i + 3; j++)
                    {
                        if(j - 3*i < value.star[i])
                        {
                            stars[j].GetComponent<UIWidget>().alpha = 1;
                        }
                        else
                        {
                            stars[j].GetComponent<UIWidget>().alpha = 0;
                        }
                    }
                }
                if (!value.isBoss)
                {
                    tab.normalSprite2D = normalSprite[0];
                    tab.pressedSprite2D = normalSprite[1];
                }
                else
                {
                    tab.normalSprite2D = bossSprite[0];
                    tab.pressedSprite2D = bossSprite[1];
                }
                level.GetComponent<FontSelector>().setFontIndex(0);
                levelBoss.GetComponent<FontSelector>().setFontIndex(0);
                if (GameManager.Instance.CurrentLevelUnlock < Index + 1)
                {
                    level.GetComponent<FontSelector>().setFontIndex(1);
                    levelBoss.GetComponent<FontSelector>().setFontIndex(1);
                    if (!value.isBoss)
                    {
                        tab.normalSprite2D = disableSprite[0];
                        tab.pressedSprite2D = disableSprite[1];
                    }
                    else
                    {
                        tab.normalSprite2D = bossDisableSprite[0];
                        tab.pressedSprite2D = bossDisableSprite[1];
                    }
                  //  tab.GetComponentInChildren<Collider>().enabled = false;
                }
                else
                {
                    tab.GetComponentInChildren<Collider>().enabled = true;
                }
                level.gameObject.SetActive(true);
                levelBoss.gameObject.SetActive(true);
                if (value.isBoss)
                {
                    level.gameObject.SetActive(false);
                }
                else
                {
                    levelBoss.gameObject.SetActive(false);
                }
                base.Data = value;
            }
        }
        public override int Index { get => base.Index; set {
                level.text = (value + 1).ToString();
                levelBoss.text = (value + 1).ToString();
                var pMissions = System.Array.Find(GameDatabase.Instance.missionConfig, x => x.level == value + 1);
                BaseItemGame itemCraft = null; 
                foreach (var pMission in pMissions.missions)
                {
                    foreach (var psubMision in pMission.missions)
                    {
                        foreach (var pReward in psubMision.rewards)
                        {
                            if (pReward.item.categoryItem == CategoryItem.CRAFT)
                            {
                                itemCraft = pReward.item;
                                break;
                            }
                        }
                    }
                }

                if (itemCraft != null)
                {
                    boxCraft.gameObject.SetActive(true);
                    boxCraft.transform.Find("icon").GetComponent<UI2DSprite>().sprite2D = itemCraft.iconShop;
                }
                base.Index = value;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
