using EazyEngine.Space;
using EazyEngine.Tools;
using EazyEngine.Tools.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponFindTarget : MonoBehaviour,EzEventListener<CharacterChangeState>
{

    public class FoundInfo
    {
        public float _distance;
        public int countTargeted = 0;
        public GameObject _obect;
        public FoundInfo(float pDitance, GameObject pObject)
        {
            _distance = pDitance;
            _obect = pObject;
        }
    }
    public List<Weapon> weapons = new List<Weapon>();
    public bool autoStartWeaponWhenFindTarget = false;
    public LayerMask TargetLayer;
    public UnityEvent onFindTarget;
   
    public void RequestTarget(Weapon pWeapon)
    {
        if (weapons.Contains(pWeapon)) return;
        weapons.Add(pWeapon);
        findTargetMinDistance(pWeapon);
    }

    public void UnRequestTargetFinding(Weapon pWeapon)
    {
        weapons.Remove(pWeapon);
    }
    public bool checkTargeted(GameObject pTarget,Weapon pWeapon)
    {
        for (int i = 0; i < pWeapon.targetDirection.Count; ++i)
        {
            if (pWeapon.targetDirection[i].activeSelf && pTarget == pWeapon.targetDirection[i])
            {
                return true;
            }
        }
        return false;
    }
    private void Awake()
    {
        for(int  i = 0; i < weapons.Count; ++i)
        {
            weapons[i].Radar = this;
        }
        
    }
    public int sortDistance(FoundInfo pInfo1, FoundInfo pInfo2)
    {
        if(pInfo1.countTargeted != pInfo2.countTargeted)
        {
            return pInfo1.countTargeted.CompareTo(pInfo2.countTargeted);
        }
        return pInfo1._distance.CompareTo(pInfo2._distance);
    }
    public virtual Character[] findChar()
    {
        return LevelManger.Instance._charList.ToArray();
    }

    public int  foundObject(GameObject pObject)
    {
        int index = -1;
        for (int i = foundTarget.Count - 1; i >= 0; --i)
        {
            if (foundTarget[i]._obect == pObject)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    protected List<FoundInfo> foundTarget = new List<FoundInfo>();
    protected List<Character> charInside = new List<Character>();
    public virtual void findTargetMinDistance(Weapon pWeapon)
    {
        if (isDirty)
        {
            for (int i = 0; i < charInside.Count; ++i)
            {
                if (!charInside[i].gameObject.activeSelf) continue;
                if (foundObject(charInside[i].gameObject) >= 0) continue;
                var pTargets = charInside[i].AvailableTargets;
                foreach(var pTarget in pTargets)
                {
                    float pDistance = Vector2.Distance(pTarget.transform.position, transform.position);
                    var insideScreen = LevelManger.Instance.mainPlayCamera.Rect(0.95f).Contains(pTarget.transform.position);
                    if (Layers.LayerInLayerMask(pTarget.gameObject.layer, TargetLayer) && insideScreen)
                    {
                        foundTarget.Add(new FoundInfo(pDistance, pTarget.gameObject));
                    }
                }
             
            }
            isDirty = false;
            isFirst = false;
            charInside.Clear();
        }
        if (foundTarget.Count > 0)
        {
            foundTarget.Sort(sortDistance);
            while (!pWeapon.targetDirection.Contains(foundTarget[0]._obect) && pWeapon.countTargetNeeded() > pWeapon.currentTargetCount())
            {
             
                pWeapon.addTargetDirection(foundTarget[0]._obect);
                foundTarget[0].countTargeted++;
                if (autoStartWeaponWhenFindTarget)
                {
                    if (pWeapon.CurrentState == WeaponState.WeaponIdle)
                    {
                        pWeapon.InputStart();
                    }
                }
                foundTarget.Sort(sortDistance);
            }
            
            onFindTarget.Invoke();
            OnFindTarget();
        }
     

    }
    public virtual void OnFindTarget()
    {
      //  GetComponent<Weapon>().TargetDirection = Target;
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    bool isFirst = true;
    private void OnEnable()
    {
        isFirst = true;
        isDirty = true;
        foundTarget.Clear();
        charInside.Clear();
        if (LevelManger.InstanceRaw)
        {
            charInside.AddRange(findChar());
        }
        EzEventManager.AddListener(this);
    }

    private void OnDisable()
    {
        EzEventManager.RemoveListener(this);
    }


    // Update is called once per frame
    void Update()
    {
            for (int i = 0; i < weapons.Count; ++i)
            {
                if (weapons[i].currentTargetCount() < weapons[i].countTargetNeeded())
                {
                    findTargetMinDistance(weapons[i]);
                }
            }
        
    }
    bool isDirty = true;
    public void OnEzEvent(CharacterChangeState eventType)
    {
        if(eventType.target != null )
        {
            if(eventType.target.CurrentState == StateCharacter.Death)
            {
                int index = foundObject(eventType.target.gameObject);
                if (index >= 0)
                {
                    foundTarget.RemoveAt(index);
                }
            }
            else if (eventType.target.CurrentState == StateCharacter.AliveInSide)
            {
                isDirty = true;
                charInside.Add(eventType.target);
            }
          
        }
    }
}
