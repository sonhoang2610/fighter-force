using UnityEngine;
using System.Collections;

namespace EazyEngine.Space
{
    public class MapTutorial : MonoBehaviour
    {
        public float speed;
        public float posRespawn;
        public float posSpawn;
        public Transform lastBg;
        bool isDouble;
        private Vector3 posSpeed;
        private Vector3 posRespawnX;
        public GameObject parrentParralax;
        bool isRepawn = false;
        float pSec = 0;
        void Awake()
        {
            posSpeed.Set(0, -speed, 0);
            posRespawnX.Set(0, posSpawn, 0);
            isDouble = false;
      
        }
        public Transform findMaxY()
        {
            Transform transformMax = null;
            foreach (Transform child in parrentParralax.transform)
            {
                if (transformMax == null || transformMax.transform.position.y < child.transform.position.y)
                {
                    transformMax = child;
                }
            }
            return transformMax;
        }
        private void LateUpdate()
        {
            if (isRepawn)
            {
                Transform pMax = findMaxY();
                float pHeight = pMax.gameObject.GetComponent<SpriteRenderer>().size.y;
                Vector3 pPos = pMax.position + new Vector3(0, pHeight, 0);
                transform.position = pPos;
                isRepawn = false;
            }
        }
        private void Start()
        {
        }
        // Update is called once per frame
        void Update()
        {
            pSec += Time.deltaTime;
            if (LevelManger.Instance.isMovingMap)
            {
                if (transform.localPosition.y > posRespawn)
                {
                    transform.localPosition += posSpeed * Time.deltaTime;
                }
                if (transform.localPosition.y <= posRespawn)
                {

                    isRepawn = true;
                    //transform.position.po = lastBg.transform
                }
            }
            //if (LevelManger.Instance.isFireAble && !isDouble)
            //{
            //    speed = speed * 2.5f;
            //    posSpeed.Set(0, -speed, 0);
            //    isDouble = true;
            //}


            //if (Constants.currentLevel == 1 && TutorialManager.instance.step == 6 && !isDouble)
            //{
            //    speed *= 2.5f;
            //    posSpeed.Set(0, -speed, 0);
            //    isDouble = true;
            //}
        }
    }
}