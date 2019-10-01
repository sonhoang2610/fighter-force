using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using EazyEngine.Space;
using System;
using System.Reflection;
using EazyReflectionSupport;
using System.Linq;
using EazyEngine.Tools;

public class EzSerializeGenericSurrogate<T> : ISerializationSurrogate where T : EzScriptTableObject
{
    public virtual void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        T pMainObject = (T)obj;
        Debug.Log(pMainObject);
        string path = DatabaseReference.Instance.getUnique(pMainObject);
        Debug.Log(path);
        info.AddValue("path", path);
        //var pFields =  pMainObject.GetType().GetFields().Where(field => ((field.IsDefined(typeof(EzSerializeField), false) || field.IsPublic) && ((!field.FieldType.IsArray && !field.FieldType.IsSubclassOf(typeof(EzScriptTableObject))) || (field.FieldType.IsArray && !field.FieldType.GetElementType().IsSubclassOf(typeof(EzScriptTableObject))))));
        //foreach(var pField in pFields)
        //{
        //    info.AddValue(pField.Name, pField.GetValue(pMainObject));
        //}
    }

    public virtual object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        string path = info.GetValue("path",typeof(string)).ToString();
        T pMainObject = null;
        pMainObject = DatabaseReference.Instance.getAsset<T>(path);
        //if (pMainObject)
        //{
        //    var pFields = pMainObject.GetType().GetFields().Where(field => ((field.IsDefined(typeof(EzSerializeField), false) || field.IsPublic) && ((!field.FieldType.IsArray && !field.FieldType.IsSubclassOf(typeof(UnityEngine.Object))) || (field.FieldType.IsArray && !field.FieldType.GetElementType().IsSubclassOf(typeof(UnityEngine.Object))))));
        //    foreach (var pField in pFields)
        //    {
        //        pField.SetValue(pMainObject, info.GetValue(pField.Name, pField.FieldType));
        //    }
        //    return pMainObject;
        //}
  
        return pMainObject;
    }
}

public class EzSerializeGenericMonoSurrogate<T> : ISerializationSurrogate where T : UnityEngine.Object
{
    public virtual void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        T pMainObject = (T)obj;
        string path = DatabaseReference.Instance.getUnique(pMainObject);
        info.AddValue("path", path);
        //var pFields = pMainObject.GetType().GetFields().Where(field => field.IsDefined(typeof(EzSerializeField), false) || field.IsPublic);
        //foreach (var pField in pFields)
        //{
        //    info.AddValue(pField.Name, pField.GetValue(pMainObject));
        //}
    }

    public virtual object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        string path = info.GetValue("path", typeof(string)).ToString();
        T pMainObject = null;
        //T[] pObjects = Resources.FindObjectsOfTypeAll<T>();
        //foreach (T pObject in pObjects)
        //{
        //    if ((pObject).GetInstanceID().ToString() == path)
        //    {
        //        pMainObject = pObject;
        //        break;
        //    }
        //}
        pMainObject = DatabaseReference.Instance.getAsset<T>(path);
        if (pMainObject)
        {
            //var pFields = pMainObject.GetType().GetFields().Where(field => field.IsPublic || field.IsDefined(typeof(EzSerializeField)));
            //foreach (var pField in pFields)
            //{
            //    pField.SetValue(pMainObject, info.GetValue(pField.Name, pField.FieldType));
            //}
            return pMainObject;
        }

        return null;
    }
}


public class AbilitySerialize : EzSerializeGenericSurrogate<AbilityInfo>
{

}
public class PackageInfoSerialize : EzSerializeGenericSurrogate<ItemPackage>
{

}
public class PlaneInfoSerialize : EzSerializeGenericSurrogate<PlaneInfo>
{

}

public class BaseItemSerialize : EzSerializeGenericSurrogate<BaseItemGame>
{

}
public class ItemGameSerialize : EzSerializeGenericSurrogate<ItemGame>
{

}
public class MissionSerialize : EzSerializeGenericSurrogate<MissionItem>
{

}
public class SkillInfoSerialize : EzSerializeGenericSurrogate<SkillInfo>
{

}
public class GameDatabaseSerialize : EzSerializeGenericSurrogate<GameDatabase>
{

}

public class CharacterSerialize : EzSerializeGenericMonoSurrogate<Character>
{

}
public class SpriteSerialize : EzSerializeGenericMonoSurrogate<Sprite>
{

}

public class ScriptTableObjectSerialize : EzSerializeGenericMonoSurrogate<ScriptableObject>
{

}