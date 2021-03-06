﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
using EazyEngine.Tools.Space;

public class DragObject : MonoBehaviour
{
    public float Speed = 100;
    Collider2D _collider2D;
    RectTransform rect;
    [HideInInspector]
    public bool EnableDrag;
    bool beganTouch = false;
   // Vector2 oldPos;
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        rect = GetComponent<RectTransform>();
        if(PlayerPrefs.GetInt("Control", 0) == 1)
        {
            enabled = false;
        }
    }
    
	public void movement(Vector2 pDelta){
        float pX = transform.position.x + pDelta.x;

        if (pX + _collider2D.bounds.extents.x > LevelManger.Instance.mainPlayCamera.Rect().xMax)
        {
            pX = LevelManger.Instance.mainPlayCamera.Rect().xMax - _collider2D.bounds.extents.x;
        }
        if(pX - _collider2D.bounds.extents.x < LevelManger.Instance.mainPlayCamera.Rect().xMin)
        {
            pX = LevelManger.Instance.mainPlayCamera.Rect().xMin + _collider2D.bounds.extents.x;
        }
        float pY = transform.position.y + pDelta.y;
        if (pY + _collider2D.bounds.extents.y > LevelManger.Instance.mainPlayCamera.Rect().yMax)
        {
            pY = LevelManger.Instance.mainPlayCamera.Rect().yMax - _collider2D.bounds.extents.y;
        }
        if (pY - _collider2D.bounds.extents.y < LevelManger.Instance.mainPlayCamera.Rect().yMin)
        {
            pY = LevelManger.Instance.mainPlayCamera.Rect().yMin + _collider2D.bounds.extents.y;
        }
        transform.position = new Vector3(pX, pY, transform.position.z);
     //   transform.Translate(pDelta, UnityEngine.Space.World);
	}

    private void Update()
    {
        if (InputManager.InstanceRaw != null && InputManager.Instance.isTouch && LevelManger.Instance.IsMatching)
        {
            Vector3 wp =InputManager.Instance.TouchPos;
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (beganTouch)
            {
                Vector2 deltaPos = touchPos - (Vector2)transform.position;
                movement(deltaPos * Speed * Time.deltaTime);
            }
            Rect pRect = rect.rect;
            pRect.position = (Vector2) transform.position - rect.sizeDelta/2;
            if (rect != null && pRect.Contains(touchPos))
            {
                if (!beganTouch)
                {
                    beganTouch = true;
                }
            }
            // oldPos = touchPos;
        }
        else
        {
            beganTouch = false;
        }
    }
}
