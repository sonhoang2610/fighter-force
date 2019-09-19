using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class PullAbleObject : MonoBehaviour
    {

        private void OnEnable()
        {
            LevelManger.Instance.pullObject.Add(gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
         //  PositionContraint
        }
    }
}
