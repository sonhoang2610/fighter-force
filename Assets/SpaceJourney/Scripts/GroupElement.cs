using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools.Space;
namespace EazyEngine.Space
{
    public class GroupElement : TimeControlBehavior,IRespawn
    {
    
        [HideInInspector]
        public int indexInGroup;
        [HideInInspector]
        public MovingLeader objectAnchor;
        protected Vector2 deltaPos;
       // protected LevelState infoSort;
        protected Health _health;
        public GroupManager _parentGroup;
        public float RotationSpeed = 360;
        public float speedSpringPoss = 6;
        public bool ignoreMoveFollowLeader = false;
        public void setInfoSort(int pIndex)
        {
            indexInGroup = pIndex;
        }
        public void setAnchor(MovingLeader pAnchorObject)
        {
            objectAnchor = pAnchorObject;
            float pDegree = (360 - objectAnchor.transform.localRotation.eulerAngles.z);
            deltaPos = pAnchorObject.transform.localPosition - transform.localPosition;
        }
        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health)
            {
                _health.onDeath.AddListener(onDeath);
            }
           
        }
        
        public void onDeath()
        {
            if (_parentGroup)
            {
                _parentGroup.detachElement(this);
            }
        }
        public void pauseSelf()
        {
            ignoreMoveFollowLeader = true;
        }
        public void resumeSelft()
        {
            ignoreMoveFollowLeader = false;
        }
        public void detachMoveGroup()
        {
            ignoreMoveFollowLeader = true;
            objectAnchor.elements.Remove(this);
        }
        public void pauseGroup( bool pBool)
        {
            if (pBool)
            {
                objectAnchor.GetComponent<RootMotionController>().pauseAllAction();
            }
            else
            {
                objectAnchor.GetComponent<RootMotionController>().resumeAllAction();
            }
        }
        // Use this for initialization
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
        Vector2 targetPoint = Vector2.zero;
        bool _rightPlace = false;
        public bool RightPlace
        {
            get
            {
                return _rightPlace;
            }
            set
            {
                _rightPlace = value;
                if (value)
                {
                    objectAnchor.triggerRightPositionElement();
                }
            }
        }

        public void sortImediately()
        {
            float pDegree = VectorExtension.FindDegree(Vector2.up.Rotate(objectAnchor.transform.localRotation.eulerAngles.z + 180));
            Vector2[] pArrayPos = new Vector2[] { deltaPos };
            pArrayPos = pArrayPos.convertAfterRotation(Vector2.zero, pDegree);
            targetPoint = pArrayPos[0] + (Vector2)objectAnchor.transform.localPosition;
            transform.localPosition = targetPoint;
        }
        private void LateUpdate()
        {
            if (objectAnchor != null && !ignoreMoveFollowLeader)
            {
                float pDegree = VectorExtension.FindDegree(Vector2.up.Rotate(objectAnchor.transform.localRotation.eulerAngles.z +180));
                Vector2[] pArrayPos = new Vector2[] { deltaPos };
                pArrayPos = pArrayPos.convertAfterRotation(Vector2.zero, pDegree);
                targetPoint = pArrayPos[0] + (Vector2)objectAnchor.transform.localPosition;
                if (targetPoint != (Vector2)transform.localPosition)
                {

                    if (objectAnchor._moveInfo.lookType == TypeLook.LookDirection)
                    {
                        Vector3 pRotationVec = objectAnchor.transform.localRotation.eulerAngles;
                        pRotationVec.z += 180;
                        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(pRotationVec), time.deltaTime* RotationSpeed);
                        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPoint, speedSpringPoss * time.deltaTime);
                       // transform.localPosition = targetPoint;
                    }
                    else if (objectAnchor._moveInfo.lookType == TypeLook.LookDown)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPoint, speedSpringPoss * time.deltaTime);
                    }
                    else if (objectAnchor._moveInfo.lookType == TypeLook.LookMainPlayer)
                    {
                        //transform.RotationDirect2D(LevelManger.Instance.CurrentPlayer.transform.position - transform.position, TranformExtension.FacingDirection.UP);
                        Vector2 newDirection = LevelManger.Instance.CurrentPlayer.transform.position - transform.position;
                        var angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                        angle += 90;
                        Quaternion newPoint = Quaternion.AngleAxis(angle, Vector3.forward);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, newPoint, RotationSpeed * time.deltaTime);
                        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPoint, speedSpringPoss * time.deltaTime);
                    }
                    else
                    {
                        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPoint, speedSpringPoss * time.deltaTime);
                    }
                    if(Vector3.Distance( transform.localPosition,targetPoint) <= 0.01f)
                    {
                        RightPlace = true;
                    }
                    else
                    {
                        RightPlace = false;
                    }
                }
            }
        }

        public void onRespawn()
        {
            ignoreMoveFollowLeader = false;
        }
    }
}
