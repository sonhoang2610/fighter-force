using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventSerializationSurrogate : ISerializationSurrogate
{
    List<UnityEvent> queue = new List<UnityEvent>();


    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as UnityEvent);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        UnityEvent item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}
public class UnityEventGameObjectSerializationSurrogate : ISerializationSurrogate
{
    List<UnityEventGameObject> queue = new List<UnityEventGameObject>();


    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", queue.Count);
        queue.Add(obj as UnityEventGameObject);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        UnityEventGameObject item = queue[int.Parse(info.GetValue("index", typeof(int)).ToString())];
        return item;
    }
}

