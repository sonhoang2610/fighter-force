using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;


namespace EazyEngine.Space {
    public class WarningLaserFollowDirection : MonoBehaviour,IRespawn
    {
        TranformExtension.FacingDirection defaultFaceing = TranformExtension.FacingDirection.DOWN;
        public LayerMask maskCam;
        public GameObject warning;
        protected Collider2D colliderHit;
        protected bool start = false;
        protected float timeSinceStart = 0;
        protected float destinyStart = 0;
        // Start is called before the first frame update
        void Start()
        {

        }
        protected Vector3 cachePos;

        public void moveAfter(float pSec)
        {
            if(pSec - 1 > 0)
            {
                destinyStart = pSec - 1;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (!start)
            {
                timeSinceStart += Time.deltaTime;
                if (timeSinceStart >= destinyStart)
                {
                    if (!warning.activeSelf)
                    {
                        StartCoroutine(delayActive(warning));
                    }
                    start = true;
                }
                else
                {
                    RaycastHit2D hit = MMDebug.RayCast(transform.position, Vector2.down.Rotate(transform.rotation.eulerAngles.z), 10, maskCam, Color.red, false);
                    if (hit)
                    {
                        cacheRotation = transform.rotation;
                        cachePos = hit.point + (hit.point - (Vector2)transform.position).normalized * 0.5f;
                        warning.transform.position = hit.point + (hit.point - (Vector2)transform.position).normalized*0.5f;
                        warning.transform.rotation = cacheRotation;
                        colliderHit = hit.collider;
                    }
                    return;
                }
            }
            if (start)
            {
                warning.transform.position = cachePos;
                warning.transform.rotation = cacheRotation;
                return;
            }
        

        }

        protected Quaternion cacheRotation;
        private void OnEnable()
        {
            
        }

        private void LateUpdate()
        {
            if (start)
            {
                warning.transform.position = cachePos;
                warning.transform.rotation = cacheRotation;
            }
        }

        private IEnumerator delayActive(GameObject pObject)
        {
            yield return new WaitForSeconds(0.1f);
            pObject.gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (colliderHit != null && colliderHit == collision)
            {
                warning.gameObject.SetActive(false);
            }
        }

        public void onRespawn()
        {
            start = false;
            timeSinceStart = 0;
            warning.gameObject.SetActive(false);
            destinyStart = 0;
        }
    }
}