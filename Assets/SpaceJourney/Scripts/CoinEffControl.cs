using UnityEngine;
using System.Collections;


namespace EazyEngine.Space
{
    public class CoinEffControl : MonoBehaviour
    {
        public float distancePlane = 0.5f;
        public bool addForceOnEnable = true;
        Rigidbody2D rBody;
        private bool isTarget;
        private float distance;
        private Quaternion newRotation;
        int rand = 0;
        float f = 0;
        Vector3 scale;
        public Vector3 bigSize = new Vector3(0.7f, 0.7f, 1f);
        public Vector3 normalSize = new Vector3(0.5f, 0.5f, 1f);
        public Vector3 smallSize = new Vector3(0.35f, 0.35f, 1f);
        [System.NonSerialized]
        public bool isEnable = true;
        // Use this for initialization
        void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
        }
        Vector3 vel = Vector3.one;
        void OnEnable()
        {
            //if (Constants.currentLevel != 1)
            if (addForceOnEnable)
            {
                rBody.AddForce(Random.insideUnitCircle * 15, ForceMode2D.Force);
            }
        }
        
        void Update()
        {
            if (!isEnable) return;
            if (LevelManger.InstanceRaw != null && LevelManger.Instance.isPause == false)
            {
                if (isTarget)
                {
                   // newRotation = Quaternion.LookRotation(transform.position - LevelManger.Instance.CurrentPlayer.transform.position, Vector3.forward);
                    newRotation.x = 0.0f;
                    newRotation.y = 0.0f;
                    transform.rotation = newRotation;//Quaternion.Slerp(transform.rotation, newRotation, 0.15f);
                    rBody.velocity = (LevelManger.Instance.CurrentPlayer.transform.position-  transform.position).normalized * 18;
                    //transform.position = Vector3.SmoothDamp(transform.position, MyPlaneController.instance.transform.position, ref vel, 0.2f);
                }
                else
                {
                    if (LevelManger.Instance.CurrentPlayer.gameObject.activeSelf)
                    {
                        distance = Vector3.Distance(LevelManger.Instance.CurrentPlayer.transform.position, transform.position);
                        {
                            if (distance <= distancePlane* LevelManger.Instance.CurrentPlayer.getFactorWithItem("Collect"))
                            {
                                isTarget = true;
                            }
                        }
                    }
                    else
                    {
                        rBody.velocity = Vector2.zero;
                    }
                }
            }
            if (  transform.position.y < -10.0f)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnDisable()
        {
           isEnable = true;
           GetComponent<Collider2D>().enabled = true;
            isTarget = false;
        }
        public void SetInfo(Vector3 newPos)
        {
            //transform.position = newPos;
            // tỷ lê 40:40:20
            int random = Random.Range(0, 101);
            if (random < 40)
            {
                transform.localScale = smallSize;
            }
            else if (random >= 40 && random <= 80)
            {
                transform.localScale = normalSize;
            }
            else
            {
                transform.localScale = bigSize;
            }
          //  transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            transform.position = newPos + Random.insideUnitSphere / 2;
            //transform.position = new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z);
            gameObject.SetActive(true);
        }
    }
}
