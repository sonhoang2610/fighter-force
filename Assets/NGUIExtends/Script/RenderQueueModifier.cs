using UnityEngine;
using System.Collections;
using System;
using Spine.Unity;


[System.Serializable]
public class MatRenderOrder
{
   public Material mat;
    public int delta;
}


public class RenderQueueModifier : MonoBehaviour
{
    public enum RenderType
    {
        FRONT,
        BACK,
        SAME
    }

    public UIWidget m_target = null;
    public RenderType m_type = RenderType.FRONT;
    public bool runOnUpdateObjectRender = true;
    public bool sharedMat = true;
    public bool isIncludeChild = true;
    public MatRenderOrder[] mats;
    Renderer[] _renderers;
    int _lastQueue = 0;

    SkeletonAnimation customMat;

    public bool isHaveMat
    {
        get
        {
             if(mats != null && mats.Length != 0)
            {
                return true;
            }
            return false;
        }
    }
    public void setTarget(UIWidget pTarget)
    {
        if (pTarget != null && m_target != pTarget)
        {
            pTarget.onRender += resort;
        }
        m_target = pTarget;
    }
    int index = 0;
    private void OnEnable()
    {
        isStarted = false;
        _lastQueue = 0;
        if (m_target == null)
            return;
        if (_renderers == null)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        if (!isHaveMat)
        {
            foreach (Renderer r in _renderers)
            {
                if (!isIncludeChild && r.gameObject != gameObject) continue;
                if (m_target.drawCall != null)
                {
                    if (sharedMat)
                    {
                        if (r.sharedMaterials != null)
                        {
                            for (int i = 0; i < r.sharedMaterials.Length; ++i)
                            {
                                if (r.sharedMaterials[i] != null)
                                {
                                    r.sharedMaterials[i].renderQueue = m_target.drawCall.renderQueue;
                                }
                            }
                        }
                        else
                        {
                            r.sharedMaterial.renderQueue = m_target.drawCall.renderQueue;
                        }
                    }
                    else
                    {
                        // r.sharedMaterial.renderQueue = m_target.drawCall.renderQueue;
                    }
                }
            }
        }
        else
        {
            if (m_target.drawCall)
            {
                for (int i = 0; i < mats.Length; ++i)
                {
                    mats[i].mat.renderQueue = m_target.drawCall.renderQueue + mats[i].delta;
                }
            }
        }
        m_target.onRender -= resort;
        m_target.onRender += resort;
    }

    private void OnDisable()
    {
        if (m_target == null)
            return;
        m_target.onRender -= resort;
    }
    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        customMat = GetComponent<SkeletonAnimation>();
    }

    float currentTime = 0;
    bool isStarted = false;
    [ContextMenu("Resort")]
    public void resort(Material mat)
    {

        if (runOnUpdateObjectRender || index == 0)
        {
            index++;
            int queue = mat.renderQueue;
            queue += m_type == RenderType.FRONT ? 1 : (m_type == RenderType.BACK ? -1 : 0);
            if (_lastQueue != queue)
            {
              //  Debug.Log("change Queue" + gameObject.name);
                currentTime = 0;
                _lastQueue = queue;
                if (!isHaveMat)
                {
                    if (_renderers != null)
                    {
                        if (sharedMat)
                        {
                            foreach (Renderer r in _renderers)
                            {
                                if (!isIncludeChild && r.gameObject != gameObject) continue;
                                if (r.sharedMaterials != null)
                                {
                                    for (int i = 0; i < r.sharedMaterials.Length; ++i)
                                    {
                                        if (r.sharedMaterials[i] != null)
                                        {
                                            r.sharedMaterials[i].renderQueue = _lastQueue;
                                        }
                                    }
                                }
                                else
                                {
                                    r.sharedMaterial.renderQueue = _lastQueue;
                                }

                            }
                        }
                        else
                        {
                            foreach (Renderer r in _renderers)
                            {
                                if (!isIncludeChild && r.gameObject != gameObject) continue;
                                r.material.renderQueue = _lastQueue;
                            }
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < mats.Length; ++i)
                    {
                        mats[i].mat.renderQueue = _lastQueue + mats[i].delta;
                    }
                }
                //}
            }
        }

    }
    //private void FixedUpdate()
    //{
    //    if (m_target == null || m_target.drawCall == null)
    //        return;
    //    int queue = m_target.drawCall.renderQueue;
    //    queue += m_type == RenderType.FRONT ? 1 : (m_type == RenderType.BACK ? -1 : 0);
    //    //if (_lastQueue != queue)
    //    //{
    //    _lastQueue = queue;

    //    foreach (Renderer r in _renderers)
    //    {
    //        r.material.renderQueue = _lastQueue;
    //    }
    //    //}
    //}
    void LateUpdate()
    {
        //if (m_target.drawCall != cacheDrawCall)
        //{
        //    m_target.onRender -= resort;
        //    m_target.drawCall.onRender += resort;
        //    cacheDrawCall = m_target.drawCall;
        //}
    }
}
