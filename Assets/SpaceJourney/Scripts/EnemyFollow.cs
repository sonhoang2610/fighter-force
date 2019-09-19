using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools.Space;

namespace EazyEngine.Space {
    public class EnemyFollow : HomeMissile,IRespawn
    {
      
        GroupElement _element;
        bool isActive = false;
        bool findTarget = false;
        private void Awake()
        {
            _element = GetComponent<GroupElement>();
        }
        public override void OnFindTarget()
        {
            base.OnFindTarget();
            findTarget = true;
            if (isActive)
            {
                _element.detachMoveGroup();
            }
        }

        public override Character[] findChar()
        {
            return new Character[] { LevelManger.Instance.CurrentPlayer};
        }

        public override bool isUpdateAble()
        {
            return isActive;
        }

        private void Update()
        {
            if (!isActive && LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
            {
                isActive = true;
                if (findTarget)
                {
                    _element.detachMoveGroup();
                }
            }else if(isActive && !LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
            {
                _element.GetComponent<Health>().Kill();
            }
        }

        public void onRespawn()
        {
            isActive = false;
            findTarget = false;
            target = null;
            if (rb)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
        }
    }

    
}