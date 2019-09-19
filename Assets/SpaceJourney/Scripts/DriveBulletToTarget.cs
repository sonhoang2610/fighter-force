using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class TrajectorDriver
    {
        public Vector2 start, destiny;
        public Vector2 directionRayCast;
        public float snapRayCast;
        private float cacheDistance;

        public float CacheDistance { get => cacheDistance; set => cacheDistance = value; }
    }
    public class DriveBulletToTarget : BaseEnviroment<DriveAble,DriveBulletToTarget>
    {
        public LayerMask mask,maskTarget;
        public TrajectorDriver[] drivers;

        private void Awake()
        {
            for (int i = 0; i < drivers.Length; ++i)
            {
                drivers[i].CacheDistance = Vector2.Distance(drivers[i].start + (Vector2)transform.position, drivers[i].destiny + (Vector2)transform.position);
            }
        }
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            for(int i = 0; i < drivers.Length; ++i)
            {
                Gizmos.DrawLine(drivers[i].start + (Vector2)transform.position, drivers[i].destiny + (Vector2)transform.position);
                Vector2 posmid = (drivers[i].start + (Vector2)transform.position + drivers[i].destiny + (Vector2)transform.position) / 2;
                MMDebug.DrawGizmoArrow(posmid, drivers[i].directionRayCast.normalized * 3, Color.yellow);
            }
#endif
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Layers.LayerInLayerMask(collision.gameObject.layer, mask))
            {
                var pDriveAble = collision.gameObject.GetComponent<DriveAble>();
                if (pDriveAble)
                {
                    Projectile prop = pDriveAble.prop;
                    prop.IgnoreMove = true;
                    listObjectDrive.Add(pDriveAble);
                    pDriveAble._onDeath = delegate
                    {
                        listObjectDrive.Remove(pDriveAble);
                    };
                    pDriveAble.onStartDrive(new Vector2[] { new Vector2(pDriveAble.transform.position.x,transform.position.y), posAbleDriveTo[Random.Range(0, posAbleDriveTo.Count)] }, drivers[0].directionRayCast);
                }         
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        [ShowInInspector]
        protected List<Vector2> posAbleDriveTo = new List<Vector2>();
        protected List<DriveAble> listObjectDrive = new List<DriveAble>();
        // Update is called once per frame
        void Update()
        {
            posAbleDriveTo.Clear();
            for (int i = 0; i < drivers.Length; ++i)
            {
               for(int j = 0; j < (int)(drivers[i].CacheDistance/drivers[i].snapRayCast); ++j)
                {
                    Vector2 pOriginal = Vector2.LerpUnclamped(drivers[i].start, drivers[i].destiny, (float)j / (float)(drivers[i].CacheDistance / drivers[i].snapRayCast));
                     var hit = MMDebug.RayCast(pOriginal + (Vector2)transform.position, drivers[i].directionRayCast.normalized ,20, maskTarget, Color.red, true);
                    if (hit)
                    {
                        posAbleDriveTo.Add(pOriginal + (Vector2)transform.position);
                    }
                 
                }
            }
        }
    }
}
