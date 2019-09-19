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
        protected bool isActive = false;
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
                    start = true;
                }
                else
                {
                    return;
                }
            }
            if (isActive)
            {
                warning.transform.position = cachePos;
                return;
            }
            isActive = true;
            RaycastHit2D hit = MMDebug.RayCast(transform.position, Vector2.down.Rotate(transform.rotation.eulerAngles.z), 10, maskCam, Color.red, true);
            if (hit)
            {
                if (!warning.activeSelf)
                {
                   // warning.gameObject.SetActive(true);
                    StartCoroutine(delayActive(warning));
                    //warning.GetComponent<LineRenderer>().useWorldSpace = true;
                    //warning.GetComponent<LineRenderer>().SetPosition(1, (Vector3)hit.point + ((Vector3)Vector2.down.Rotate(transform.rotation.eulerAngles.z).normalized * 10));
                    //warning.GetComponent<LineRenderer>().SetPosition(0, hit.point);
                }
                cachePos = hit.point + (hit.point - (Vector2)transform.position).normalized * 0.5f;
                warning.transform.position = hit.point + (hit.point - (Vector2)transform.position).normalized*0.5f;
                warning.transform.rotation = transform.rotation;
                colliderHit = hit.collider;
            }
        }
        private void LateUpdate()
        {
            if (isActive)
            {
                warning.transform.position = cachePos;
                return;
            }
        }
        public IEnumerator delayActive(GameObject pObject)
        {
            yield return new WaitForSeconds(0.1f);
            pObject.gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (colliderHit != null && colliderHit == collision)
            {
                isActive = true;
                warning.gameObject.SetActive(false);
            }
        }

        public void onRespawn()
        {
            start = false;
            timeSinceStart = 0;
            warning.gameObject.SetActive(false);
            isActive = false;
            destinyStart = 0;
        }
    }
}