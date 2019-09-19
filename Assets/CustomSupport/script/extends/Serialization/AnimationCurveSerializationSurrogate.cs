using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using EazyEngine.Space;

public class AnimationCurveSerializationSurrogate : ISerializationSurrogate
{
    List<AnimationCurve> queue = new List<AnimationCurve>();
    

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as AnimationCurve);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        AnimationCurve item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}

public class Vector3Surrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        var vector = (Vector3)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
        info.AddValue("z", vector.z);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Func<string, float> get = name => (float)info.GetValue(name, typeof(float));
        return new Vector3(get("x"), get("y"), get("z"));
    }
}
public class UnityObjectSurrogate : ISerializationSurrogate
{
    GameObject pObject;
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        pObject = obj as GameObject;
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        return pObject;
    }
}

public class PickAbleItemSerialize : ISerializationSurrogate
{
    List<PickAbleItem> queue = new List<PickAbleItem>();
    

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as PickAbleItem);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        PickAbleItem item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}
public class GameObjectSerialize : ISerializationSurrogate
{
    List<GameObject> queue = new List<GameObject>();


    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as GameObject);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        GameObject item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}
public class GroupManagerSerialize : ISerializationSurrogate
{
    List<GroupManager> queue = new List<GroupManager>();


    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as GroupManager);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        GroupManager item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}