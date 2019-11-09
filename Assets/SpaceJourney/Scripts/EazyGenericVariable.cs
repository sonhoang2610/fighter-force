using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyReflectionSupport;
using EazyEngine.Space;
using EazyEngine.Tools;
using Sirenix.Serialization;
using System;
#if UNITY_EDITOR

using UnityEditor;
#endif


public enum TypeConFig
{
    Float,
    Vector2
}
[System.Serializable]
public class CharacterInstancedConfigGroup : IExternalStringReferenceResolver
{
    public IExternalStringReferenceResolver NextResolver { get; set; }

    public bool CanReference(object value, out string id)
    {
#if UNITY_EDITOR
        if (value is MonoBehaviour)
        {
            id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(((MonoBehaviour)value).gameObject as UnityEngine.Object));
            id += "/" + value.GetType().Name;
            return true;
        }
        if (value is UnityEngine.Object)
        {
            id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as UnityEngine.Object));
            return true;
        }
#endif
        id = null;
        return false;
    }

    public bool TryResolveReference(string id, out object value)
    {
        var path = id.Split('/');
        value = null;
#if UNITY_EDITOR
        if (path.Length > 1)
        {
            var pGameObject = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(AssetDatabase.GUIDToAssetPath(path[0]));
            value = pGameObject.GetComponent(path[1]);
        }
        else
        {
            value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(id));
        }
#endif
        return value != null;
    }
    [PreviewField]
    public Texture2D preview;
    [TabGroup("Easy")]
    [HideLabel]
    public CharacterInstancedConfig easy;
    [TabGroup("Medium")]
    [HideLabel]
    public CharacterInstancedConfig medium;
    [TabGroup("Hard")]
    [HideLabel]
    public CharacterInstancedConfig hard;
    public CharacterInstancedConfig[] elements
    {
        get
        {
            return new CharacterInstancedConfig[] { easy, medium, hard };
        }
    }
}
[System.Serializable]
public class CharacterInstancedConfig
{

    public GameObject target;
    public float Health;
    public float DamgageBasic;
    public float Defense;
    public float DamgeSelf = -1;
    public bool isBoss = false;
    [System.NonSerialized]
    public int score;

    public WeaponInstancedConfig[] weapon;
    [OnValueChanged("addProp")]
    public EazyVariableConfigGroup[] propEdits;

    public void addProp()
    {
        if (propEdits.Length > 0)
        {
            propEdits[propEdits.Length - 1]._target = target;
        }
    }
    [Button("Generate Sub Health")]
    public void generateSubHealth()
    {
        var pHealths = target.GetComponentsInChildren<Health>(true);
        List<EazyVariableConfigGroup> props = new List<EazyVariableConfigGroup>();
        foreach (var pHealthh in pHealths)
        {
            if (pHealthh.gameObject == target) continue;
            props.Add(new EazyVariableConfigGroup()
            {
                _target = target,
                nameProp = pHealthh.gameObject.GetGameObjectPath(target),
                propEdits = new EazyVariableConfig[] {
                        new EazyVariableConfig(){ target = target,typeConfig = TypeConFig.Float,ApplyTargetNow = false,component = "Health.InitialHealth:",newTarget = new GameObjectPath(){ path = pHealthh.gameObject.GetGameObjectPath(target) },value = 10000 },
                              new EazyVariableConfig(){ target = target,typeConfig = TypeConFig.Float,ApplyTargetNow = false,component = "Health.MaxiumHealth:",newTarget = new GameObjectPath(){ path = pHealthh.gameObject.GetGameObjectPath(target)},value = 10000  }
                    }
            });
          
        }
        propEdits = props.ToArray();
        for(int i = 0; i < propEdits.Length; ++i)
        {
            foreach(var prop in propEdits[i].propEdits)
            {
                prop.reload();
            }
        }
    }

    [Button("Generate")]
    public void autoGenerate()
    {
        if (target)
        {
            var pHealth = target.GetComponent<Health>();
            if (pHealth)
            {
                Health = pHealth.MaxiumHealth;
                Defense = pHealth.deffense;
            }
            var pHealths = target.GetComponentsInChildren<Health>();
            List<EazyVariableConfigGroup> props = new List<EazyVariableConfigGroup>();
            foreach (var pHealthh in pHealths)
            {
                if (pHealthh.gameObject == target) continue;
                props.Add(new EazyVariableConfigGroup()
                {
                    _target = target,
                    propEdits = new EazyVariableConfig[] {
                        new EazyVariableConfig(){ target = target,typeConfig = TypeConFig.Float,ApplyTargetNow = false,component = "Health.InitialHealth:",newTarget = new GameObjectPath(){ path = pHealthh.gameObject.GetGameObjectPath(target) } },
                              new EazyVariableConfig(){ target = target,typeConfig = TypeConFig.Float,ApplyTargetNow = false,component = "Health.MaxiumHealth:",newTarget = new GameObjectPath(){ path = pHealthh.gameObject.GetGameObjectPath(target) } }
                    }
                });
            }
            var hanlde = target.GetComponent<CharacterHandleWeapon>();
            DamgageBasic = hanlde.FixDamage;

            if (hanlde.DatabaseWeapon.Length > 0)
            {
                List<Weapon> prebWeapons = new List<Weapon>();
                for (int i = 0; i < hanlde.DatabaseWeapon.Length; ++i)
                {
                    for (int j = 0; j < hanlde.DatabaseWeapon[i].weapons.Length; ++j)
                    {
                        if (!prebWeapons.Contains(hanlde.DatabaseWeapon[i].weapons[j].IntialWeapon))
                        {
                            prebWeapons.Add(hanlde.DatabaseWeapon[i].weapons[j].IntialWeapon);
                        }
                    }
                }
                List<WeaponInstancedConfig> listweapons = new List<WeaponInstancedConfig>();
                foreach (var pWeapon in prebWeapons)
                {
                    listweapons.Add(new WeaponInstancedConfig() { target = pWeapon });
                }
                weapon = listweapons.ToArray();
            }
            foreach (var pWeapon in weapon)
            {
                List<BulletInstancedConfig> pBullets = new List<BulletInstancedConfig>();
                var pools = pWeapon.target.GetComponentsInChildren<ObjectPooler>();
                foreach (var pool in pools)
                {
                    if (pool.GetType() == typeof(SimpleObjectPooler))
                    {
                        pBullets.Add(new BulletInstancedConfig() { prefab = ((SimpleObjectPooler)pool).GameObjectToPool, speedBullet = ((SimpleObjectPooler)pool).GameObjectToPool.GetComponent<Projectile>().Speed });
                    }
                    else if (pool.GetType() == typeof(MultipleObjectPooler))
                    {
                        foreach (var pPoolObject in ((MultipleObjectPooler)pool).Pool)
                        {
                            pBullets.Add(new BulletInstancedConfig() { prefab = pPoolObject.GameObjectToPool, speedBullet = pPoolObject.GameObjectToPool.GetComponent<Projectile>().Speed });
                        }
                    }
                }
                pWeapon.bullets = pBullets.ToArray();
            }



        }
    }
}

[System.Serializable]
public class WeaponInstancedConfig
{
    [HorizontalGroup("Weapon")]
    [HideLabel]
    public string desWeapon;
    [HorizontalGroup("Weapon")]
    [HideLabel]
    public Weapon target;
    [HideLabel]
    public DamageExtra extraDamge;
    [OnValueChanged("addWeapon")]
    public EazyVariableConfigGroup[] propEdits;

    public void addWeapon()
    {
        propEdits[propEdits.Length - 1]._target = target.gameObject;
    }
    public BulletInstancedConfig[] bullets;

    [Button("Revert All")]
    public void revertAll()
    {
        foreach (var pProps in propEdits)
        {
            foreach (var pProp in pProps.propEdits)
            {
                pProp.revert();
            }
        }
    }
}
[System.Serializable]
public class EazyVariableConfigGroup
{
    [HideLabel]
    public string nameProp;
    [OnValueChanged("addWeapon")]
    public EazyVariableConfig[] propEdits;
    [System.NonSerialized]
    public GameObject _target;

    public void addWeapon()
    {
        if (propEdits.Length <= 0) return;
        propEdits[propEdits.Length - 1].Target = _target;
    }
}
[System.Serializable]
public class BulletInstancedConfig
{
    [PreviewField]
    public GameObject prefab;
    public float speedBullet;
    public EazyVariableConfigGroup[] propEdits;
}
[System.Serializable]
public class GameObjectPath
{
    [HorizontalGroup("Object")]
    [HideLabel]
    [ShowIf("isEdit")]
    public GameObject parrent;
    [HorizontalGroup("Object")]
    [ShowIf("isEdit")]
    [HideLabel]
    public GameObject target;
    [HideIf("isEdit")]
    [HideLabel]
    [HorizontalGroup("Object")]
    public string path;
    protected bool isEdit;
    [HideLabel]
    [HorizontalGroup("Object", Width = 100)]
    [Button("@this.getNameButton()")]
    public void edit()
    {
        isEdit = !isEdit;
        if (target != null && !isEdit)
        {
            path = target.GetGameObjectPath(parrent);
        }
    }
    [HideLabel]
    [HorizontalGroup("Object", Width = 100)]
    [Button("Clear")]
    public void clear()
    {
        path = "";
    }

    public string getNameButton()
    {
        return !isEdit ? "Edit" : "Apply";
    }

}
[System.Serializable]
public class EazyVariableConfig
{
    protected string[] cachePath;
    public void setValue(GameObject pTarget = null)
    {
        if (ApplyTargetNow) return;
        cachePath = component.Split('.');
        if (cachePath.Length > 0 && cachePath[1].IndexOf(':') >= 0)
        {
            cachePath[1] = cachePath[1].Remove(cachePath[1].IndexOf(':'));
        }
        var pComponent = Target.GetComponent(cachePath[0]);
        if (pComponent)
        {
            var pMethod = Target.GetComponent(cachePath[0]).GetType().GetField(cachePath[1]);
            if (pMethod != null)
            {
                if (typeConfig == TypeConFig.Vector2)
                {
                    pMethod.SetValue(pComponent, valueVector);
                }
                else
                {
                    pMethod.SetValue(pComponent, Convert.ChangeType(value, pMethod.FieldType));
                }
            }
        }
    }
    //[HideIf("ApplyTargetNow")]
    [EnableIf("isEdit")]
    [HorizontalGroup("Var")]
    [HideLabel]
    public string nameProp;
    [HorizontalGroup("Var")]
    [HideLabel]
    [CustomValueDrawer("valueDraw")]
    [ShowIf("typeConfig", TypeConFig.Float)]
    public float value = 0;

    [HorizontalGroup("Var")]
    [HideLabel]
    [ShowIf("typeConfig", TypeConFig.Vector2)]
    [CustomValueDrawer("valueDrawVector2")]
    public Vector2 valueVector = Vector2.zero;

#if UNITY_EDITOR
    protected Dictionary<string, SerializedObject> cacheSerialize = new Dictionary<string, SerializedObject>();
    public float valueDraw(float pValue, GUIContent pContent)
    {
        // EditorGUILayout.PropertyField()
        if (ApplyTargetNow && Target && !string.IsNullOrEmpty(component) && cacheMethods != null)
        {
            int index = cacheMethods.findIndex(component);
            if (index < props.Length)
            {
                EazyMethodInfo pMethod = props[index];
                var pComponent = Target.GetComponent(pMethod.componentString);
                if (pComponent)
                {
                    SerializedObject pObject = null;
                    if (cacheSerialize == null)
                    {
                        cacheSerialize = new Dictionary<string, SerializedObject>();
                    }
                    if (cacheSerialize.ContainsKey(component))
                    {
                        pObject = cacheSerialize[component];
                    }
                    else
                    {
                        pObject = new SerializedObject(pComponent);
                        cacheSerialize.Add(component, pObject);
                    }
                    var prop = pObject.FindProperty(pMethod.methodString);
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop, new GUIContent(""));
                    }
                    pObject.ApplyModifiedProperties();
                    return pValue;
                }

            }

        }
        pValue = EditorGUILayout.FloatField(pValue);
        return pValue;
    }

    public Vector2 valueDrawVector2(Vector2 pValue, GUIContent pContent)
    {
        // EditorGUILayout.PropertyField()
        if (ApplyTargetNow && Target && !string.IsNullOrEmpty(component) && cacheMethods != null)
        {
            int index = cacheMethods.findIndex(component);
            if (index < props.Length)
            {
                EazyMethodInfo pMethod = props[index];
                var pComponent = Target.GetComponent(pMethod.componentString);
                if (pComponent)
                {
                    SerializedObject pObject = null;
                    if (cacheSerialize == null)
                    {
                        cacheSerialize = new Dictionary<string, SerializedObject>();
                    }
                    if (cacheSerialize.ContainsKey(component))
                    {
                        pObject = cacheSerialize[component];
                    }
                    else
                    {
                        pObject = new SerializedObject(pComponent);
                        cacheSerialize.Add(component, pObject);
                    }
                    var prop = pObject.FindProperty(pMethod.methodString);
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop, new GUIContent(""));
                    }
                    pObject.ApplyModifiedProperties();
                    return pValue;
                }

            }

        }
        pValue = EditorGUILayout.Vector2Field("", pValue);
        return pValue;
    }
#endif
    [HorizontalGroup("Var", MinWidth = 100)]
    [DisableIf("isEdit")]
    [Sirenix.OdinInspector.Button("Edit")]

    public void edit()
    {
        isEdit = true;
    }

    [EnableIf("isEdit")]
    [HorizontalGroup("Var", MinWidth = 100)]
    [Sirenix.OdinInspector.Button("Apply")]
    public void Apply()
    {
        isEdit = false;
    }
    [HideLabel]
    [ShowIf("isEdit")]
    public GameObjectPath newTarget;
    [HorizontalGroup("Prop")]
    [HideLabel]
    [ShowIf("isEdit")]
    public GameObject target;
    [HideLabel]
    [HorizontalGroup("Prop")]
    [ShowIf("isEdit")]
    [ValueDropdown("ValuesFunctionComponents")]
    public string component;
    [HorizontalGroup("Type")]
    [ShowIf("isEdit")]
    public bool ApplyTargetNow = false;
    [ShowIf("isEdit")]
    [HorizontalGroup("Type")]
    [DisableIf("typeConfig", TypeConFig.Float)]
    [Sirenix.OdinInspector.Button("Float")]

    public void Float()
    {
        component = null;
        typeConfig = TypeConFig.Float;
        reload();
    }

    [EnableIf("typeConfig", TypeConFig.Float)]
    [ShowIf("isEdit")]
    [HorizontalGroup("Type")]
    [Sirenix.OdinInspector.Button("Vector2")]
    public void Vector()
    {
        component = null;
        typeConfig = TypeConFig.Vector2;
        reload();
    }
    [HideInInspector]
    public TypeConFig typeConfig;
    [ShowIf("isEdit")]
    [Sirenix.OdinInspector.Button("Reload")]
    public void reload()
    {
#if UNITY_EDITOR
        cacheSerialize.Clear();
#endif
        props = null;
        cacheMethods = null;
        cache = null;
        cacheTargetForPath = null;
        if (Target)
        {
            if (props == null || props.Length == 0) { props = typeConfig == TypeConFig.Float ? getAllNumberType() : Target.getAllEzFieldreturnType(typeof(Vector2)); };
            if (cacheMethods == null || cacheMethods.Length == 0) { cacheMethods = props.convertToStringMethods(); }
        }
    }

    public EazyMethodInfo[] getAllNumberType()
    {
        List<EazyMethodInfo> pInfos = new List<EazyMethodInfo>();
        pInfos.AddRange(Target.getAllEzFieldreturnType(typeof(float)));
        pInfos.AddRange(Target.getAllEzFieldreturnType(typeof(int)));
        return pInfos.ToArray();
    }
    public void revert()
    {
        cachePath = component.Split('.');
        if (cachePath.Length > 0 && cachePath[1].IndexOf(':') >= 0)
        {
            cachePath[1] = cachePath[1].Remove(cachePath[1].IndexOf(':'));
        }
        var pComponent = Target.GetComponent(cachePath[0]);
        if (pComponent)
        {
            var pMethod = Target.GetComponent(cachePath[0]).GetType().GetField(cachePath[1]);
            if (pMethod != null)
            {
                if (pMethod.FieldType == typeof(Vector2))
                {
                    typeConfig = TypeConFig.Vector2;
                }
                else if (pMethod.FieldType == typeof(float))
                {
                    typeConfig = TypeConFig.Float;
                }
                else if (pMethod.FieldType == typeof(int))
                {
                    typeConfig = TypeConFig.Float;
                }
            }
        }
        if (Target)
        {
            if (props == null || props.Length == 0) { props = typeConfig == TypeConFig.Float ? getAllNumberType() : Target.getAllEzFieldreturnType(typeof(Vector2)); };
            if (cacheMethods == null || cacheMethods.Length == 0) { cacheMethods = props.convertToStringMethods(); }
        }
    }

    //[HideLabel]
    //[HorizontalGroup("Prop")]
    //public string Prop;
    protected bool isEdit;
    protected EazyMethodInfo[] props;
    protected string[] cacheMethods;
    protected ValueDropdownList<string> cache;
    protected GameObject cacheTargetForPath = null;
    public virtual GameObject Target
    {
        get
        {
            if (target && !string.IsNullOrEmpty(newTarget.path))
            {
                if (cacheTargetForPath == null)
                {
                    string[] paths = newTarget.path.Split('/');
                    Transform pTrans = null;
                    int index = 0;
                    do
                    {
                        pTrans = (pTrans ? pTrans.gameObject : target).transform.Find(paths[index]);
                        index++;
                    } while (pTrans != null && index < paths.Length);
                    if (pTrans != null && index == paths.Length)
                    {
                        cacheTargetForPath = pTrans.gameObject;
                    }
                }

                return cacheTargetForPath;
            }
            return target;
        }
        set => target = value;
    }

    private IList<ValueDropdownItem<string>> ValuesFunctionComponents()
    {
        if (cache == null || cache.Count == 0)
        {
            cache = new ValueDropdownList<string>();
            if (Target)
            {
                if (props == null || props.Length == 0) { props = typeConfig == TypeConFig.Float ? getAllNumberType() : Target.getAllEzFieldreturnType(typeof(Vector2)); };
                if (cacheMethods == null || cacheMethods.Length == 0) { cacheMethods = props.convertToStringMethods(); }
                for (int i = 0; i < cacheMethods.Length; ++i)
                {
                    cache.Add(cacheMethods[i]);
                }
            }
        }
        return cache;
    }

    //private IList<ValueDropdownItem<string>> ValuesFunctionProp()
    //{
    //    var drops = new ValueDropdownList<string>();
    //    if (target && !string.IsNullOrEmpty(component))
    //    {
    //        props.convertToStringMethods
    //        if (componentReal == null) { componentReal = target.GetComponent(component); };
    //        if (componentReal)
    //        {
    //            componentReal.GetType().ez
    //        }
    //    }
    //    return drops;
    //}
}

