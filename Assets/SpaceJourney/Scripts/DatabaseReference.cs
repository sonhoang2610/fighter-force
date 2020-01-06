
using EazyEngine.Space;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public struct RefObjectInfo
{
    public int guid;
    public UnityEngine.Object asset;
}

public static class EzScriptTableObjectSupport{
    public static void SerializeEzScriptTableObject(this object pRawObject,List<EzScriptTableObject> pObjectSerilizeds)
    {
        EzScriptTableObject pObject = null;
        if (pRawObject.GetType().IsSubclassOf(typeof(EzScriptTableObject)))
        {
            pObject = (EzScriptTableObject)pRawObject;
            if (pObjectSerilizeds.Contains(pObject)) return;
            DatabaseReference.Instance.getUnique(pObject);
            pObjectSerilizeds.Add(pObject);
        }
        var pAllFiled = pRawObject.GetType().GetFields();
        if (pAllFiled != null && pObject)
        {
            var pFields = pAllFiled.Where(field => field != null && field.FieldType != null && !field.IsStatic && field.IsPublic && field.FieldType.IsClass &&  (field.FieldType.Namespace == null||!field.FieldType.Namespace.StartsWith("System")) && field.FieldType.IsDefined(typeof(System.SerializableAttribute), false));
            foreach (var pField in pFields)
            {
                if (!pField.FieldType.IsArray)
                {
                    var pSubObject = pField.GetValue(pObject);
                    if (pSubObject != null)
                    {
                        pSubObject.SerializeEzScriptTableObject(pObjectSerilizeds);
                    }
                }
                else
                {
                    var pArraySubObject = (System.Array)pField.GetValue(pObject);
                    for (int i = 0; i < pArraySubObject.Length; i++)
                    {
                        var pSubObject = pArraySubObject.GetValue(i);
                        if (pSubObject != null)
                        {
                            pSubObject.SerializeEzScriptTableObject(pObjectSerilizeds);
                        }
                    }
                }
            }
        }
    }
}
public class DatabaseReference : MonoBehaviour
{
    public List<RefObjectInfo> references = new List<RefObjectInfo>();

    public static DatabaseReference _instance;
    public static DatabaseReference Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = AssetBundle.FindObjectOfType<DatabaseReference>();
            }
            if (!_instance)
            {
                _instance = Resources.Load<DatabaseReference>("Database/DatabaseReference");
            }
            if (!_instance.gameObject.scene.IsValid())
            {
                _instance = Instantiate(_instance);
            }
            return _instance;
        }
    }

    public T getAsset<T>(string pGUID) where T : UnityEngine.Object
    {
        for (int i = 0; i < references.Count; ++i)
        {
            if (references[i].guid.ToString() == pGUID)
            {
               if(references[i].asset.GetType() == typeof(T))
                {
                    return (T)references[i].asset;
                }
            }
        }
        return null;
    }

    public string  getUnique(UnityEngine.Object pObject)
    {
       for(int i = 0; i < references.Count; ++i)
        {
            if(references[i].asset == pObject)
            {
                return references[i].guid.ToString();
            }
        }
        int pGuid = UnityEngine.Random.Range(0, 999999);
        do
        {
            pGuid = UnityEngine.Random.Range(0, 999999);
        } while (existID(pGuid));
        references.Add(new RefObjectInfo() { asset = pObject, guid = pGuid });
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SerializedObject pSerialize = new UnityEditor.SerializedObject(this);
            pSerialize.ApplyModifiedProperties();
            string pathPrefab = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this);
            UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, pathPrefab, UnityEditor.InteractionMode.AutomatedAction);
        }
#endif
        return pGuid.ToString();
    }

    public bool existID(int pID)
    {
        for (int i = 0; i < references.Count; ++i)
        {
            if (references[i].guid == pID)
            {
                return true;
            }
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button("Refresh Ref")]
    public void RefreshRef()
    {
        FileStream file = null;
        string destination = Application.persistentDataPath + "/GameInfo1.dat";
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        if (file != null)
        {
            GameDatabase.Instance.SerializeEzScriptTableObject(new List<EzScriptTableObject>());
        }
    }


}
