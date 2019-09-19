using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Vector2SerializationSurrogate : ISerializationSurrogate
{
    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj,
                              SerializationInfo info, StreamingContext context)
    {

        Vector2 v3 = (Vector2)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector)
    {

        Vector2 v3 = (Vector2)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        obj = v3;
        return obj;   // Formatters ignore this return value //Seems to have been fixed!
    }
}
