using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
using FlowCanvas;
using Sirenix.OdinInspector;
using EazyEngine.Audio;

public class PickAbleItem : SerializedMonoBehaviour
{
    public string idItem;
    public int quantityItem;
    public Sprite icon;
    public GameObject effectOnPick;
    public FlowCanvas.FlowScript mainBehaviorEffect;
    public LayerMask _layerCanPick;
    public Dictionary<string, object> Variables = new Dictionary<string, object>();
    public AudioGroupSelector sfx = AudioGroupConstrant.CoinInGame;
    private void Awake()
    {
        if (effectOnPick && GameManager.Instance.inGame)
        {
            ParticleEnviroment.Instance.preloadEffect(3, effectOnPick, transform.position, 1);
        }
    }
    public void onPicked()
    {
        SoundManager.Instance.PlaySound(sfx, Vector3.zero);
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
