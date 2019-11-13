using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space {
    [System.Serializable]
    public struct EviromentPlayerTrigger {
        public string trigger;
        public GameObject prefab;
        public bool autoActiveOnEnable;

        private EnviromentInfo initiateObject;

        public EnviromentInfo InitiateObject { get => initiateObject; set => initiateObject = value; }
    }
    [System.Serializable]
    public class EnviromentInfo
    {
        public GameObject enviroment;
        public int relative;
    }
    public class PlayerEnviroment : MonoBehaviour, EzEventListener<MessageGamePlayEvent>
    {
        public EviromentPlayerTrigger[] enviroments;

        void EzEventListener<MessageGamePlayEvent>.OnEzEvent(MessageGamePlayEvent eventType)
        {
            if (eventType._objects != null && eventType._objects.Length > 0 && eventType._objects[0] != (object)gameObject) return;
            string[] pStrs = eventType._message.Split('/');
            if (pStrs.Length ==2 && pStrs[0] == ("ActiveEnviroment"))
            {
                for(int i = 0;i < enviroments.Length; ++i)
                {
                    if(enviroments[i].trigger == pStrs[1])
                    {
                        if (enviroments[i].InitiateObject.relative == 0)
                            enviroments[i].InitiateObject.enviroment.gameObject.SetActive(true);
                        enviroments[i].InitiateObject.relative++;
                    }
                }
            }
            if (pStrs.Length == 2 && pStrs[0] == ("DeactiveEnviroment"))
            {
                for (int i = 0; i < enviroments.Length; ++i)
                {
                    if (enviroments[i].trigger == pStrs[1])
                    {
                        enviroments[i].InitiateObject.relative--;
                        if (enviroments[i].InitiateObject.relative == 0) {
                            enviroments[i].InitiateObject.enviroment.gameObject.SetActive(false);
                        }
             
                    }
                }
            }
        }
        public static Dictionary<GameObject, EnviromentInfo> eviromentInstant = new Dictionary<GameObject, EnviromentInfo>();
        bool isInit = false;
        private void Awake()
        {
            if (!isInit)
            {
                for (int i = 0; i < enviroments.Length; ++i)
                {
                    if (!eviromentInstant.ContainsKey(enviroments[i].prefab))
                    {
                        var pObject = Instantiate(enviroments[i].prefab);
                        enviroments[i].InitiateObject = new EnviromentInfo() { enviroment = pObject };
                        eviromentInstant.Add(enviroments[i].prefab, enviroments[i].InitiateObject);
                    }
                    else
                    {
                        enviroments[i].InitiateObject = eviromentInstant[enviroments[i].prefab];
                    }
                }
                isInit = true;
            }
        }

        private void OnEnable()
        {
            Awake();
            for (int i = 0; i < enviroments.Length; ++i)
            {
                if (enviroments[i].autoActiveOnEnable )
                {
                
                    if (enviroments[i].InitiateObject.relative == 0)
                    {
                        if (enviroments[i].InitiateObject.enviroment.IsDestroyed())
                        {
                            continue;
                        }
                        enviroments[i].InitiateObject.enviroment.SetActive(true);
                    }
                    enviroments[i].InitiateObject.relative++;
                }
            }
            EzEventManager.AddListener(this);
        }

       
        private void OnDisable()
        {
            for (int i = enviroments.Length -1; i >= 0 ; --i)
            {
                if (enviroments[i].autoActiveOnEnable && enviroments[i].InitiateObject.relative > 0)
                {
                    enviroments[i].InitiateObject.relative--;
                    if (enviroments[i].InitiateObject.relative == 0)
                    {
                        if (enviroments[i].InitiateObject.enviroment.IsDestroyed())
                        {
                            continue;
                        }
                        enviroments[i].InitiateObject.enviroment.SetActive(false);
                    }
                }
            
            }
            EzEventManager.RemoveListener(this);
        }
        public static void clear()
        {
            var pObjects = FindObjectsOfType<PlayerEnviroment>();
            eviromentInstant.Clear();
            foreach(var pObject in pObjects)
            {
                pObject.enviroments = new EviromentPlayerTrigger[0];
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
