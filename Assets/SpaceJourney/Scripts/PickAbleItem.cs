using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
using FlowCanvas;
using Sirenix.OdinInspector;


public class PickAbleItem : SerializedMonoBehaviour
{
    public string idItem;
    public int quantityItem;
    public GameObject effectOnPick;
    public FlowCanvas.FlowScript mainBehaviorEffect;
    public LayerMask _layerCanPick;
    public Dictionary<string, object> Variables = new Dictionary<string, object>();
    public void onPicked()
    {
        gameObject.SetActive(false);
        if (effectOnPick != null)
        {
            ParticleEnviroment.Instance.createEffect(effectOnPick, transform.position, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Layers.LayerInLayerMask(collision.gameObject.layer,_layerCanPick) && collision.GetComponent<Character>())
        {
            onPicked();
            EzEventManager.TriggerEvent(new PickEvent(idItem, quantityItem, gameObject,collision.gameObject, mainBehaviorEffect, Variables));
        }
    }
}
