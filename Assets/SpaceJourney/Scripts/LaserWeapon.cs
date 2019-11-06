using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Linq;

namespace EazyEngine.Space
{
    [System.Serializable]
    public struct LaserLineElement
    {
        public int level;
        public Vector2[] directions;
        private GameObject lineRender;
        public GameObject LineRender { get => lineRender; set => lineRender = value; }
#if UNITY_EDITOR
        public Color color; 
#endif
    }
    public struct LaserInfo
    {
        public Vector2 original;
        public Vector2 parrentdirection;
    }
 
    public class LaserWeapon : Weapon
    {
        public LineRenderer prepareLine;
        public LineRenderer lineLaser;
        public LayerMask targetMask;
        public ObjectPooler poolerEndPoint;
        public GameObject startPoint;
        public int levelLaser = 0;
        public float maxLength = 30;
        public LaserLineElement[] lineElement;
        public int damagePerSec =100;
        public bool Infinite = false;
        public System.Action<int,bool> onShootingLevel;
        protected float damageLeft = 0;
        protected Health _target = null;
        protected int countLine = 0;
        protected List<LineRenderer> lineHaveEndPoint = new List<LineRenderer>();
        protected Vector2 destinyPoint = Vector2.zero;
        [ShowInInspector]
        protected List<LineRenderer> lines = new List<LineRenderer>();
        private void Awake()
        {
            lines.Add(lineLaser);
            if (lineLaser)
            {
                lineLaser.gameObject.SetActive(true);
            }
        }
        public virtual void LaserWeaponUse()
        {
            if (IsPause) return;
            Vector3 rotate = transform.rotation.eulerAngles;
            Vector2 directon = Vector2.up.Rotate(rotate.z);
            Vector3 disteny = (Vector3)directon.normalized * 10 + transform.position;
            lineLaser.gameObject.SetActive(true);
            startPoint.gameObject.SetActive(true);
        }
        public override void WeaponStart()
        {
            base.WeaponStart();
            if (prepareLine)
            {
                Vector2 desWorld = Vector2.zero;
                Vector3 rotate = transform.rotation.eulerAngles;
                Vector2 directon = Vector2.up.Rotate(rotate.z);
                desWorld = transform.InverseTransformPoint( (Vector3)directon.normalized * 20 + transform.position);
                prepareLine.SetPositions(new Vector3[] { Vector3.zero, desWorld });
            }
        }
        protected Dictionary<LineRenderer, GameObject> dictEndpoint = new Dictionary<LineRenderer, GameObject>();
        
        public GameObject getEndPoint(LineRenderer pLine)
        {
            if (dictEndpoint.ContainsKey(pLine))
            {
                dictEndpoint[pLine].gameObject.SetActive(true);
                return dictEndpoint[pLine];
            }
            GameObject pEndPoint = poolerEndPoint.GetPooledGameObject();
            dictEndpoint.Add(pLine, pEndPoint);
            pEndPoint.gameObject.SetActive(true);
            return pEndPoint;
        }
        public LineRenderer getMoreLine(int index)
        {
            if (index < lines.Count)
            {
                lines[index].gameObject.SetActive(true);
                return lines[index];
            }
            var pLine =  Instantiate<LineRenderer>(lineLaser, lineLaser.transform.parent);
            pLine.gameObject.SetActive(true);
            lines.Add(pLine);
            return pLine;
        }
        public void disableEnpoints()
        {
            if (!GameManager.Instance.inGame || !LevelManger.InstanceRaw) return;
            for (int i = 0; i < dictEndpoint.Count; ++i)
            {
                dictEndpoint.ElementAt(i).Value.gameObject.SetActive(false) ;
            }
            for(int i = 0; i > lines.Count; ++i)
            {
                lines[i].gameObject.SetActive(false);
            }
            lineHaveEndPoint.Clear();
            dictEndpoint.Clear();
        }

        public void disableLine(LineRenderer pLine)
        {
            if (dictEndpoint.ContainsKey(pLine))
            {
                dictEndpoint[pLine].gameObject.SetActive(false);
                dictEndpoint.Remove(pLine);
            }
        }
        public override void pauseWeapon(bool isPause)
        {
            base.pauseWeapon(isPause);
            BlockState = isPause;
            if (isPause)
            {
                lineLaser.gameObject.SetActive(false);
                countLine = 0;
                disableEnpoints();
                startPoint.gameObject.SetActive(false);
            }
            else
            {
                LaserWeaponUse();
            }
      
        }
        private void Start()
        {
            if (lineLaser)
            {
                lineLaser.gameObject.SetActive(false);
            }
        }
        public override void WeaponUse()
        {
            base.WeaponUse();
            LaserWeaponUse();
        }
        public override void WeaponIdle()
        {
            base.WeaponIdle();
            countLine = 0;
            lineLaser.gameObject.SetActive(false);
            startPoint.gameObject.SetActive(false);
            disableEnpoints();
        }
        public float TotalDamage(Health health)
        {
                float pCurrentDamge = (FixDamage * FactorDamage );
                float pExtraDamage = 0;
                for (int i = 0; i < extraDamage.Count; ++i)
                {
                    pExtraDamage += extraDamage[i].type == DamageType.Normal ? extraDamage[i].damageExtra :
                        (extraDamage[i].type == DamageType.PecentHp ? (float)health.CurrentHealth * extraDamage[i].damageExtra :
                        (extraDamage[i].type == DamageType.PecentMaxHp ? (float)health.MaxiumHealth * extraDamage[i].damageExtra : (pCurrentDamge * extraDamage[i].damageExtra)));
                }
            return pCurrentDamge + pExtraDamage;
        }

        protected List<GameObject> ignoreObject = new List<GameObject>();
        private void Update()
        {
        
            if (lineLaser.gameObject.activeSelf)
            {    
                Vector3 rotate = transform.rotation.eulerAngles;
                bool isHit = false;
                Vector2 directon = Vector2.up.Rotate(rotate.z);
                int pCountHitLevel = 1;
                countLine = 0;
              
                for (int i = 0; i < lines.Count; ++i)
                {
                    lines[i].gameObject.SetActive(false);
                }
                ignoreObject.Clear();
                lineHaveEndPoint.Clear();
                //disableEnpoints();
                LaserInfo[] pLasers = new LaserInfo []{ new LaserInfo() {original = Vector2.zero,parrentdirection = Vector2.up} };
                for (int j = 0; j < levelLaser + 1; ++j)
                {
                    List<LaserInfo> pResults = new List<LaserInfo>();
                    for (int i = 0; i < pLasers.Length; ++i)
                    {
                        pResults.AddRange(laser(pLasers[i], j));
                    }
                    pLasers = pResults.ToArray();
                }
                for (int i = 0; i < lines.Count; ++i)
                {
                    if (!lineHaveEndPoint.Contains(lines[i]))
                    {
                        disableLine(lines[i]);
                    }
                }

            }
        }

        public LaserInfo[] laser(LaserInfo pInfo, int pLevel)
        {
            List<LaserInfo> pResults = new List<LaserInfo>(); float pDamageLeft = damageLeft;
            Vector3 originalPos = pInfo.original;
            Vector2 oldDirection = pInfo.parrentdirection;
            LaserLineElement pLineElement = new LaserLineElement() { directions = new Vector2[] { Vector2.up }, level = 0 }; ;
            if(pLevel != 0)
            {
                pLineElement = lineElement[pLevel-1];
            }
            for (int j = 0; j < pLineElement.directions.Length; ++j)
            {
                float pRotation = Vector2.SignedAngle(Vector2.up, pLineElement.directions[j]);
                Vector2 directon = oldDirection.Rotate(pRotation);
                var hitss = MMDebug.RayCastAll(transform.TransformPoint(originalPos), transform.TransformDirection( directon.normalized), maxLength, targetMask, Color.red);
                List<RaycastHit2D> hits = new List<RaycastHit2D>();
                var pLineLaser = getMoreLine(countLine);
                countLine++;
             
                for(int g = 0;g < hitss.Length; ++g)
                {
                    if (!ignoreObject.Contains( hitss[g].collider.gameObject))
                    {
                        hits.Add(hitss[g]);
                    }
                }
                if (hits != null && hits.Count > 0)
                {
                    List<RaycastHit2D> pHist = new List<RaycastHit2D>();
            
                    if (Infinite)
                    {
                        pHist.AddRange(hits);
                    }
                    else
                    {
                        pHist.Add(hits[0]);
                    }

                    if (pHist.Count == 0)
                    {
                        onShootingLevel?.Invoke(pLevel, false);
                    }
                    for (int i = 0; i < pHist.Count; ++i)
                    {
                        var hit = pHist[i];
                        Health pTarget = null;
                        bool ishit = false;
                        if (pTarget = hit.collider.gameObject.GetComponent<Health>())
                        {
                            ishit = true;
                            ignoreObject.Add(pTarget.gameObject);
                            float pDamage = TotalDamage(pTarget) * time.deltaTime;
                            pDamage += pDamageLeft;
                            pTarget.Damage((int)pDamage, Owner ? Owner.gameObject : null, 0, 0);
                            damageLeft = pDamage - (int)pDamage;
                        }
                        if (ishit)
                        {
                            lineHaveEndPoint.Add(pLineLaser);
                            pResults.Add(new LaserInfo() { original = transform.InverseTransformPoint(hit.point), parrentdirection = directon });
                            GameObject pEndPoint = getEndPoint(pLineLaser);
                            pEndPoint.transform.position = hit.point;
                        }

                        onShootingLevel?.Invoke(pLevel, ishit);

                        if (levelLaser == 0)
                        {
                            startPoint.transform.position = transform.position;
                        }

                        pLineLaser.SetPosition(0, originalPos);
                        pLineLaser.SetPosition(1, transform.InverseTransformPoint(hit.point));
                    }
                    if (Infinite)
                    {
                        Vector2 pEndPos = originalPos + (Vector3)directon.normalized * maxLength;
                        pLineLaser.SetPosition(0, originalPos);
                        pLineLaser.SetPosition(1, pEndPos);
                    }
                }
                else
                {
                        onShootingLevel?.Invoke(pLevel, false);
                    
                    Vector2 pEndPos = originalPos + (Vector3)directon.normalized * maxLength;
                    pLineLaser.SetPosition(0, originalPos);
                    pLineLaser.SetPosition(1, pEndPos);
                }
            }
            //if (!isCut && HaveDestinyPoint)
            //{
            //    var pEndPoint = getEndPoint(0);
            //    pEndPoint.transform.position = transform.TransformPoint(DestinyPoint);
            //    lineLaser.SetPosition(1, DestinyPoint);
            //    if (isHittingNow && weaponParrent)
            //    {
            //        weaponParrent.SendMessage("unCutting", this, SendMessageOptions.DontRequireReceiver);
            //        isHittingNow = false;
            //    }
            //}
            //else if (indexEnd == 0)
            //{
            //    disableEnpoints();
            //}
            return pResults.ToArray();
        }
        //private void OnDrawGizmosSelected()
        //{
        //    for(int i = 0; i < lineElement.Length; ++i)
        //    {
        //        MMDebug.DrawGizmoArrow(transform.position, lineElement[i].direction, lineElement[i].color);
        //    }
        //}

    }
}