using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
#if UNITY_EDITOR
    using UnityEditor;
    public class CreatetorScriptTableObjectDTB 
    {
       // [MenuItem("Assets/Create/EazyEngine/Space/"]
        public static void CreateMyAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/" + typeof(T).Name+".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

    public class GiftOnlineDataBaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/GiftOnlineDataBase")]
        public static void CreateAsset()
        {
            CreatetorScriptTableObjectDTB.CreateMyAsset<GiftOnlineDataBase>();
        }
    }
#endif
    [System.Serializable]
    public class GiftOnlineModule
    {
        public string id;
        public float onlineTime;
        public int calimedIndex = -1;
        public int DateInYear = 0;
    }

    public class BoxGiftOnline : BaseBox<ItemGiftOnline, ItemGiftOnlineInfo>
    {
        protected GiftOnlineDataBase databse;
        protected int currentEffect = 0;
        private void OnEnable()
        {
            if (databse == null) {
                databse = GameDatabase.Instance.databaseGiftOnline;
            }
            reload();
            bool isDirty = false;
            for (int i = 0; i < databse.item.Length; ++i)
            {
                if (databse.item[i].time < GameManager.Instance.giftOnlineModule.onlineTime && i > GameManager.Instance.giftOnlineModule.calimedIndex)
                {
                    currentEffect++;
                    items[i].claim(delegate {
                        currentEffect--;
                        if (currentEffect <= 0)
                        {
                            reload();
                        }
                    });
                    GameManager.Instance.giftOnlineModule.calimedIndex = i;
                    isDirty = true;
                }
            }
            if (isDirty)
            {
                GameManager.Instance.SaveGame();
            }

        }

        public void reload()
        {
            for (int i = 0; i < databse.item.Length; ++i)
            {
                int status = 0;

                databse.item[i].isNext = false;
                if (i <= GameManager.Instance.giftOnlineModule.calimedIndex)
                {
                    status = 1;
                    databse.item[i].timeLeft = 0;
                }
                else if (i == GameManager.Instance.giftOnlineModule.calimedIndex + 1)
                {
                    databse.item[i].isNext = true;
                    databse.item[i].timeLeft =(int)( (double)GameManager.Instance.giftOnlineModule.onlineTime- databse.item[i].time);
                }
                else
                {
                    databse.item[i].timeLeft = (int)((double)GameManager.Instance.giftOnlineModule.onlineTime - databse.item[i].time);
                }
                databse.item[i].status = status;
            }

            DataSource = databse.item.ToObservableList();
        }
        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void LateUpdate()
        {
            bool isDirty = false;
            for (int i = 0; i < databse.item.Length; ++i)
            {
                if (databse.item[i].time < GameManager.Instance.giftOnlineModule.onlineTime && i > GameManager.Instance.giftOnlineModule.calimedIndex)
                {
                    currentEffect++;
                    items[i].claim(delegate {
                        currentEffect--;
                        if (currentEffect <= 0)
                        {
                            reload();
                        }
                    });
                    GameManager.Instance.giftOnlineModule.calimedIndex = i;
                    isDirty = true;
                }
            }
            if (isDirty)
            {
                GameManager.Instance.SaveGame();
            }
        }
    }
}
