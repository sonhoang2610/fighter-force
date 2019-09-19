using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
public enum TypeGeneration { Normal, Circle }
public class GenerateWeapon : MonoBehaviour
{
    public GameObject[] poolObject;
    public int currentIndex;
    public int countAttachMent;
    public TypeGeneration type;
    public float total;
    public float radiusStartPos = 0;
    public Vector2 randomReloadTine = new Vector2(0.2f, 0.2f);
    public bool isSemiType = false;
    public int countEachSemi = 3;

    [ContextMenu("Generate")]
    public void Generate()
    {
        var pGroup = transform.Find("group" + currentIndex);
        var pPool = transform.Find(name+"_pool" + currentIndex);
        var Weapon = GetComponent<ProjectileMultipeWeapon>();
        if (!pPool)
        {
            pPool = new GameObject().transform;
            pPool.parent = transform;
            pPool.name = name + "_pool" + currentIndex;
        }
        ObjectPooler finalPool;
        SimpleObjectPooler pPooler;
        if (pPooler = pPool.GetComponent<SimpleObjectPooler>())
        {
            if(poolObject.Length > 1)
            {
                DestroyImmediate(pPooler);
            }
            else
            {
                pPooler.GameObjectToPool = poolObject[0];
            }
        }
        else if(poolObject.Length  == 1)
        {
            pPooler = pPool.gameObject.AddComponent<SimpleObjectPooler>();
            pPooler.GameObjectToPool = poolObject[0];
        }
        finalPool = pPooler;
        if (poolObject.Length > 1)
        {
            MultipleObjectPooler pMultiPooler;
            if (!(pMultiPooler = pPool.GetComponent<MultipleObjectPooler>()))
            {
                pMultiPooler = pPool.gameObject.AddComponent<MultipleObjectPooler>();
                pMultiPooler.Pool = new List<MultipleObjectPoolerObject>();
            }

            pMultiPooler.Pool.Clear();
            for(int i = 0; i < poolObject.Length; ++i)
            {
                pMultiPooler.Pool.Add(new MultipleObjectPoolerObject() { GameObjectToPool = poolObject[i] });
            }
            finalPool = pMultiPooler;
        }
        if (!pGroup)
        {
            pGroup = new GameObject().transform;
            pGroup.parent = transform;
            pGroup.name = "group" + currentIndex;
        }
        pGroup.DestroyChildren();
        List<GameObject> attachMent = new List<GameObject>();
        for (int i = 0; i < countAttachMent; ++i)
        {
            GameObject pObject = new GameObject();
            pObject.transform.parent = pGroup;
            pObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            pObject.transform.localPosition = Vector3.zero;
            if (type == TypeGeneration.Circle)
            {
                float pAngle = -(total - 1) / 2 + ((total - 1) / (countAttachMent - 1)) * i;

                pObject.transform.localRotation = Quaternion.Euler(0, 0, pAngle);
            }
            else
            {
                pObject.transform.position = new Vector3(-(total) / 2 + ((total) / (countAttachMent-1)) * i, 0, 0);

            }
            attachMent.Add(pObject);
        }
        if (Weapon)
        {
            if(Weapon.AttachMentPosStartNew.Length <= currentIndex)
            {
                List<ObjectGroupAttachMent> pList = new List<ObjectGroupAttachMent>();
                pList.AddRange(Weapon.AttachMentPosStartNew);
                for(int i = pList.Count; i < currentIndex+1; ++i)
                {
                    pList.Add(new ObjectGroupAttachMent());
                }
                Weapon.AttachMentPosStartNew = pList.ToArray();
                Weapon.AttachMentPosStartNew[currentIndex].attachMentPosStart = attachMent.ToArray();
                Weapon.AttachMentPosStartNew[currentIndex].pooler = finalPool;
            }
        }
        Weapon.startWithReloadFirst = true;
        Weapon.randomReloadFirst = true;
        Weapon.timeReloadFirst = randomReloadTine.x;
        Weapon.randomTo = randomReloadTine.y;
        if (isSemiType)
        {
            Weapon.IsRandom = false;
            Weapon.timeReload = 0.1f;
            var pAmmo = Weapon.GetComponent<WeaponAmmo>();
            pAmmo.conditonRestore = ConditionRestoreAmmo.Zero;
            if (!pAmmo)
            {
                pAmmo = Weapon.gameObject.AddComponent<WeaponAmmo>();
            }
            pAmmo.isRandomTimeRestore = true;
            pAmmo.randomTimeRestore = randomReloadTine;
            pAmmo.quantityRestoreEachTime = attachMent.Count* countEachSemi;
            pAmmo.initStorage = attachMent.Count * countEachSemi;
        }
        else
        {
            Weapon.isRamdomTimeReload = true;
            Weapon.reloadRamdom = randomReloadTine; 
        }
    }
    [ContextMenu("Generate Attachment")]
    public void GenerateAttachMent()
    {
        var pGroup = transform.Find("group" + currentIndex);
        var pPool = transform.Find(name + "_pool" + currentIndex);
        var Weapon = GetComponent<ProjectileMultipeWeapon>();
        if (!pPool)
        {
            pPool = new GameObject().transform;
            pPool.parent = transform;
            pPool.name = name + "_pool" + currentIndex;
        }
        ObjectPooler finalPool;
        SimpleObjectPooler pPooler;
        if (pPooler = pPool.GetComponent<SimpleObjectPooler>())
        {
            if (poolObject.Length > 1)
            {
                DestroyImmediate(pPooler);
            }
            else
            {
                pPooler.GameObjectToPool = poolObject[0];
            }
        }
        else if (poolObject.Length == 1)
        {
            pPooler = pPool.gameObject.AddComponent<SimpleObjectPooler>();
            pPooler.GameObjectToPool = poolObject[0];
        }
        finalPool = pPooler;
        if (poolObject.Length > 1)
        {
            MultipleObjectPooler pMultiPooler;
            if (!(pMultiPooler = pPool.GetComponent<MultipleObjectPooler>()))
            {
                pMultiPooler = pPool.gameObject.AddComponent<MultipleObjectPooler>();
                pMultiPooler.Pool = new List<MultipleObjectPoolerObject>();
            }

            pMultiPooler.Pool.Clear();
            for (int i = 0; i < poolObject.Length; ++i)
            {
                pMultiPooler.Pool.Add(new MultipleObjectPoolerObject() { GameObjectToPool = poolObject[i] });
            }
            finalPool = pMultiPooler;
        }
        if (!pGroup)
        {
            pGroup = new GameObject().transform;
            pGroup.parent = transform;
            pGroup.name = "group" + currentIndex;
        }
        pGroup.DestroyChildren();
        List<GameObject> attachMent = new List<GameObject>();
        for (int i = 0; i < countAttachMent; ++i)
        {
            GameObject pObject = new GameObject();
            pObject.transform.parent = pGroup;
            pObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            pObject.transform.localPosition = Vector3.zero;
            if (type == TypeGeneration.Circle)
            {
                float pAngle = -(total - 1) / 2 + ((total - 1) / (countAttachMent - 1)) * i;

                pObject.transform.localRotation = Quaternion.Euler(0, 0, pAngle);
                pObject.transform.localPosition = (Vector2.down * radiusStartPos).Rotate( pAngle +180);
            }
            else
            {
                pObject.transform.position = new Vector3(-(total) / 2 + ((total) / (countAttachMent - 1)) * i, 0, 0);

            }
            attachMent.Add(pObject);
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
