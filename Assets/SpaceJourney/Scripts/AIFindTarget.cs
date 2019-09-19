using EazyEngine.Tools;
using EazyEngine.Tools.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space
{
    public class AIFindTarget : MonoBehaviour,IRespawn
    {
        public GameObject target;
        public TranformExtension.FacingDirection facingDefault = TranformExtension.FacingDirection.UP;
        public LayerMask TargetLayer;
        public UnityEvent onFindTarget;
        public float SpeedRotation = 360;
        protected CharacterHandleWeapon handleWeapon;
        public virtual Character[] findChar()
        {
            return FindObjectsOfType<Character>();
        }

        public virtual void findTargetMinDistance()
        {
            if (!LevelManger.InstanceRaw || !LevelManger.InstanceRaw.gameObject || LevelManger.InstanceRaw.gameObject.activeInHierarchy) return;
            target = null;
            var _chars = findChar();
            float distance = 0;
            for (int i = 0; i < _chars.Length; ++i)
            {
                if (!_chars[i].gameObject.activeSelf ) continue;
                float pDistance = Vector2.Distance(_chars[i].transform.position, transform.position);
                bool insideScreen = LevelManger.Instance.mainPlayCamera.Rect().Contains(_chars[i].transform.position);

                if (Layers.LayerInLayerMask(_chars[i].gameObject.layer, TargetLayer) && insideScreen)
                {
                    if (pDistance < distance || distance == 0)
                    {
                        distance = pDistance;
                        target = _chars[i].gameObject;
                        onFindTarget.Invoke();
                        OnFindTarget();
                    }
                }
            }

        }
        public virtual void OnFindTarget()
        {
        }

        public virtual void onRespawn()
        {
            target = null;
        }

        private void Awake()
        {
            handleWeapon = GetComponent<CharacterHandleWeapon>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!LevelManger.InstanceRaw)
            {
                return;
            }
            if(target == null  || !target.gameObject.activeSelf || !LevelManger.Instance.mainPlayCamera.Rect().Contains(target.transform.position))
            {
                findTargetMinDistance();
            }
            handleWeapon.setTarget(target);
        }
    }
}
