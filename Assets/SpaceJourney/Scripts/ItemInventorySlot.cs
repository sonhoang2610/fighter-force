using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public interface IConvertBaseItemGameInstanced<T> where T : BaseItemGameInstanced
    {
        T convertFromBaseItemGameInstanced(BaseItemGameInstanced pObject);
    }
    [System.Serializable]
    public class GameDatabaseInventoryEventUnity : UnityEvent<GameDatabaseInventoryEvent>
    {

    }
    public class ItemInventorySlotGeneric<T> : BaseItem<T>, EzEventListener<GameDatabaseInventoryEvent> where T : BaseItemGameInstanced, new()
    {
        public UI2DSprite icon;
        public UI2DSprite iconCateGory;
        public UILabel quantity;
        public UIButton btnAddMore;
        public bool EffectOnShow = false;
        public bool autoSelfLoad = true;
        public GameDatabaseInventoryEventUnity onChange;

        [ShowIf("autoSelfLoad")]
        public BaseItemGame itemToLoad;

        public List<EventDelegate> onAddMore = new List<EventDelegate>();
        protected bool isInit = false;

   
        public override T Data
        {
	        get {return base.Data;}
	        set
            {
           
             
                base.Data = value;
                icon.sprite2D = value.item.CateGoryIcon;
                quantity.text = StringUtils.addDotMoney(value.quantity);
                if (iconCateGory)
                {
                    if (value.item.categoryItem == CategoryItem.CRAFT)
                    {
                        icon.sprite2D = value.item.iconShop;
                        iconCateGory.gameObject.SetActive(true);
                        iconCateGory.sprite2D = value.item.CateGoryIcon;
                    }
                    else
                    {
                        iconCateGory.gameObject.SetActive(false);
                    }
                }
                isInit = true;
            }
        }

        public void clickMore()
	    {
		    ShopManager.Instance.showBoxShop(Data.item.categoryItem.ToString());
            EventDelegate.Execute(onAddMore);
        }

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            if (eventType.item.item == itemToLoad && isInit)
            {
           
                //quantity.text = eventType.item.quantity.ToString();
                if(onChange != null)
                {
                    onChange.Invoke(eventType);
                }
          
                reloadData();

            }
        }
        public virtual void reloadData()
        {
            Data = Data;
        }
       

        // Start is called before the first frame update
         protected virtual void Start()
        {
            //if (btnAddMore)
            //{
            //    btnAddMore.onClick.Clear();
            //    btnAddMore.onClick.AddRange(onAddMore);
            //}
            if (autoSelfLoad && itemToLoad)
            {
                var pDataItem = GameManager.Instance.Database.getComonItem(itemToLoad);
                if (pDataItem == null)
                {
                    pDataItem = new BaseItemGameInstanced();
                    pDataItem.quantity = 0;
                    pDataItem.item = itemToLoad;

                }
                T pData = new T();
                if (typeof(IConvertBaseItemGameInstanced<T>).IsAssignableFrom(pData.GetType()))
                {
                    Data = ((IConvertBaseItemGameInstanced<T>)pData).convertFromBaseItemGameInstanced(pDataItem);
                }else if(typeof(T) == typeof(BaseItemGameInstanced))
                {
                    Data = (T)pDataItem;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected virtual void OnEnable()
        {
            if (autoSelfLoad)
            {
                EzEventManager.AddListener(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (autoSelfLoad)
            {
                EzEventManager.RemoveListener(this);
            }
        }

        public override void show()
        {
            base.show();
            if (EffectOnShow)
            {
                var widget = GetComponent<UIWidget>();
                widget.alpha = 0;
                var pFade = DG.Tweening.DOTween.To(() => widget.alpha, x => widget.alpha = x, 1, 0.25f);
                Sequence pSequence = DOTween.Sequence();
                pSequence.AppendInterval(Index * 0.25f);
                pSequence.Append(pFade);
                pSequence.Play();
            }
        }
    }
    public class ItemInventorySlot : ItemInventorySlotGeneric<BaseItemGameInstanced>
    {
    }
}
