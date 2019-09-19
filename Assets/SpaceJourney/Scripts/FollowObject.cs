using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;

[ExecuteInEditMode]
public class FollowObject : MonoBehaviour
{

    public Transform source;
    public Vector2 offset;
    public bool activeOnStart = true ;
    protected bool isActive;
    public string activeString
    {
        get
        {
            return isActive ? "DeActive" : "Active";
        }
    }
    [Button("@activeString")]
    public void active()
    {
        isActive = !isActive;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (activeOnStart)
        {
            active();
        }
    }
    private void LateUpdate()
    {
        if (!source) return;
        if (isActive)
        {
            var pDelta = offset.Rotate(source.rotation.eulerAngles.z);
            transform.position = source.transform.position + (Vector3)pDelta;
            transform.rotation = source.rotation;
        }
        else if ((transform.hasChanged || source.hasChanged) && !Application.isPlaying)
        {
            offset = transform.position - source.transform.position;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    public float currentRotation
    {
        get
        {
            var pRotation = transform.rotation.eulerAngles.z;
            return pRotation;
        }
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    
}
