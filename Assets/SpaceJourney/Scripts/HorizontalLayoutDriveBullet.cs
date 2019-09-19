using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;

namespace EazyEngine.Space
{
    public class HorizontalLayoutDriveBullet : BaseEnviroment<DriveAbleHorizontal,HorizontalLayoutDriveBullet>
    {
        public float heightCell = 1;
        public RectTransform rect;
        public Pos clampRange = new Pos(1,5);

        protected Dictionary< DriveAbleHorizontal,int> fillSloot = new Dictionary<DriveAbleHorizontal, int>();
        protected List<int> slots = new List<int>();
        protected int limit;
        private void Awake()
        {
            limit = clampRange.y - clampRange.x;
            for(int i = 0; i <= limit; ++i)
            {
                slots.Add(i + clampRange.x);
            }
        }

        public override void registerElement(DriveAbleHorizontal pElement)
        {
            if(fillSloot.Count < limit && !elements.Contains(pElement))
            {
                base.registerElement(pElement);
                var prop = pElement.GetComponent<Projectile>();
                prop.IgnoreMove = true;
                int indexRow = Random.Range(0, slots.Count);
                fillSloot.Add(pElement, slots[indexRow]);
                List<EazyAction> pActions = new List<EazyAction>();
                pActions.Add(EazyMove.to(new Vector3(0, (slots[indexRow] + 0.5f) * heightCell + rect.rect.yMin, 0), prop.Speed, false).setCurve(pElement.curve));
                pActions.Add(DelayTime.create(1));
                pActions.Add(CallFunc.create(delegate {
                    pElement.GetComponent<Animator>().SetTrigger("Open");
                }));
                RootMotionController.runAction(prop.gameObject, Sequences.create(pActions.ToArray()));
                slots.RemoveAt(indexRow);
            }
        }
        public override void unregisterElement(DriveAbleHorizontal pElement)
        {
            base.unregisterElement(pElement);
            if (fillSloot.ContainsKey(pElement))
            {
                slots.Add(fillSloot[pElement]);
                fillSloot.Remove(pElement);
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
