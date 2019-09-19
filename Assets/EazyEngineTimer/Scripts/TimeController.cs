using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace EazyEngine.Timer
{

    public enum DependTime
    {
        Global,
        Group
    }
    [ExecuteInEditMode]
    public class TimeController : MonoBehaviour
    {
        public string nameGroup;
        [CustomValueDrawer("customParent")]
        public int _parent;

        public int customParent(int pValue,GUIContent pLabel)
        {
            var keeper = FindObjectOfType<TimeKeeper>();
            string[] pOptions = keeper.getAllTimerName();
            if(pValue >= pOptions.Length)
            {
                pValue = pOptions.Length - 1;
            }
#if UNITY_EDITOR
            pValue = EditorGUILayout.Popup(pLabel.text,pValue, keeper.getAllTimerName(), GUI.skin.FindStyle("Popup"));
#endif
            return pValue;
        }

        [SerializeField]
        private float timScale = 1;
        public float deltaTime
        {
            get
            {
                return Time.deltaTime;
            }
        }

        public float TimScale { get => timScale; set => timScale = value; }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                var keeper = FindObjectOfType<TimeKeeper>();
                keeper.addTimerGroup(this);
            }
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                var keeper = FindObjectOfType<TimeKeeper>();
                if (keeper)
                {
                    keeper.removeTimerGroup(this);
                }
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

        public float getDeltaTime()
        {
            if(TimeKeeper.Instance == null)
            {
                return Time.deltaTime;
            }
            var pParent = TimeKeeper.Instance.getTimeController(_parent);
            if (pParent)
            {
               return pParent.getDeltaTime()*TimScale;
            }
            return Time.deltaTime *TimScale;
        }
    }
}
