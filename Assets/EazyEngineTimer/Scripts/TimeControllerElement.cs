using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EazyEngine.Timer
{
    public class TimeControllerElement : MonoBehaviour
    {
        [CustomValueDrawer("customParent")]
        public int _groupName;
        [HideInInspector]
        public TimeControllerElement timeLineParent;
        [SerializeField]
        private float timeScale = 1;
        public int customParent(int pValue, GUIContent pLabel)
        {
            var keeper = FindObjectOfType<TimeKeeper>();
            string[] pOptions = keeper.getAllTimerName();
            if (pValue >= pOptions.Length)
            {
                pValue = pOptions.Length - 1;
            }
#if UNITY_EDITOR
            pValue = EditorGUILayout.Popup(pLabel.text, pValue, keeper.getAllTimerName(), GUI.skin.FindStyle("Popup"));
#endif
            return pValue;
        }

        public float deltaTime
        {
            get
            {
                if (timeLineParent) return timeLineParent.deltaTime;
                if (TimeKeeper.Instance)
                {
                    var pController = TimeKeeper.Instance.getTimeController(_groupName);
                    if (pController)
                    {
                       return pController.getDeltaTime()* TimeScale;
                    }
                }
                return Time.deltaTime*TimeScale;
            }
        }

        public float TimeScale
        {
            get
            {
                return timeScale;
            }

            set
            {
                timeScale = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
