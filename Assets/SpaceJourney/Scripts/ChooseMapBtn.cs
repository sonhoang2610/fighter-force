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
#endif
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
                if (GameManager.Instance.Database.currentUnlockLevel < Index + 1)
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
