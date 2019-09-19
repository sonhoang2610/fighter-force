using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools.Space;

public class DisableWhenTrigger : MonoBehaviour,IRespawn
{
    public LayerMask mask;
    public int _countTriggerBoder = 0;
    protected int countTriggerBoder = 0;
    public bool active = false;
    private void OnEnable()
    {
        if (LevelManger.InstanceRaw != null && LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
        {
            active = true;
        }
    }

    private void LateUpdate()
    {
        if (!active)
        {
            if (LevelManger.InstanceRaw != null &&  LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
            {
                active = true;
            }
        }
    }

    private void OnDisable()
    {
        active = false;
    }
    private void Awake()
    {
        countTriggerBoder = _countTriggerBoder;
    }
    public void onRespawn()
    {
        countTriggerBoder = _countTriggerBoder;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active) return;
        if (Layers.LayerInLayerMask(collision.gameObject.layer, mask) && countTriggerBoder <= 0)
        {
            gameObject.SetActive(false);
        }
        if (Layers.LayerInLayerMask(collision.gameObject.layer, mask))
        {
            countTriggerBoder--;
        }

    }
}
