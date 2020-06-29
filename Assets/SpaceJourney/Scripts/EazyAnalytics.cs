using Firebase.Analytics;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EazyEngine.Tools
{
    public class EazyAnalytics : MonoBehaviour
    {

    }
    public static class EazyAnalyticTool
    {
        public static string userid;
        public static EazyAnalytics anchor;
        public static bool isInitialized;
        public static void Init(System.Action pSuccess)
        {
            userid = SystemInfo.deviceUniqueIdentifier;
            if (anchor == null)
            {
                GameObject pObject = new GameObject();
                anchor = pObject.AddComponent<EazyAnalytics>();
                GameObject.DontDestroyOnLoad(anchor);
            }
            anchor.StartCoroutine(initEvent(delegate {
                pSuccess();
                isInitialized = true;
            }));
        }

        public static bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }
        public static IEnumerator initEvent(System.Action pSuccess)
        {
            string url = "http://34.94.28.183:1235/user/userinfo";
            UnityWebRequest www = new UnityWebRequest();
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.downloadHandler = new DownloadHandlerBuffer();
            www.uploadHandler = new UploadHandlerRaw(null);
            www.timeout = 5;
            // www.SetRequestHeader("Content-Type", "applicatopn/json");
            www.SetRequestHeader("user_id", userid);
            yield return www.SendWebRequest();
            pSuccess();
            www.Dispose();
            www = null;
        }
        public static void LogEvent(string pActivityName, params string[] pEvents)
        {
            if (anchor)
            {
                if (pEvents == null || pEvents.Length == 0)
                {
                    FirebaseAnalytics.LogEvent(pActivityName);
                }
                anchor.StartCoroutine(requestEvent(pActivityName,null,pEvents));
            }
        }

        public class EventInfo
        {
            public string eventName;
            public Coroutine corountine;
            public int loop;
            public int currentLoop = 0;
        }

        public static List<EventInfo> queuesEvent = new List<EventInfo>();
        public static void LogEventQueue(string pActivityName,System.Action<bool,string> callbackResult,int loop , params string[] pEvents)
        {
            if (anchor)
            {
                if (pEvents == null || pEvents.Length == 0)
                {
                    FirebaseAnalytics.LogEvent(pActivityName);
                }
                anchor.StartCoroutine(requestEvent(pActivityName,delegate(bool pResult,string pData) {
                    if (pResult)
                    {
                        callbackResult?.Invoke(pResult, pData);
                        queuesEvent.RemoveAll(x => x.eventName == pActivityName);
                    }
                    else
                    {
              
                       var pEvent = queuesEvent.Find(x => x.eventName == pActivityName);
                        if(pEvent == null)
                        {
                            pEvent = new EventInfo()
                            {
                                eventName = pActivityName,
                                currentLoop = 0,
                                loop = loop
                            };
                            queuesEvent.Add(pEvent);
                        }
                        if(pEvent.currentLoop >= pEvent.loop)
                        {
                            callbackResult?.Invoke(false, pData);
                            queuesEvent.RemoveAll(x => x.eventName == pActivityName);
                        }
                        else
                        {
                            if (pEvent.corountine != null)
                            {
                                anchor.StopCoroutine(pEvent.corountine);
                            }
                            anchor.StartCoroutine(delayAction(2, delegate
                            {
                                LogEventQueue(pActivityName, callbackResult,loop, pEvents);
                            }));
                            pEvent.currentLoop++;
                        }
                  
                    }
                }, pEvents));
            }
        }
        public static IEnumerator delayAction(float pSec,System.Action pAction)
        {

            yield return new WaitForSeconds(pSec);
            pAction?.Invoke();
        }
        public static IEnumerator requestEvent(string pActivityName,System.Action<bool,string> result,params string[] parameters)
        {
            string url = "http://34.94.28.183:1235/activity/activity";
            JsonWriter write = new JsonWriter();
            write.WriteObjectStart();
            for(int i = 0; i < parameters.Length; i+=2)
            {
                write.WritePropertyName(parameters[i]);
                write.Write(parameters[i+1]);
            }
            write.WriteObjectEnd();
            var pBody = write.ToString();
            UnityWebRequest www = new UnityWebRequest();
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.downloadHandler = new DownloadHandlerBuffer();
            www.uploadHandler = new UploadHandlerRaw(null);
            www.timeout = 5;
            // www.SetRequestHeader("Content-Type", "applicatopn/json");
            www.SetRequestHeader("user_id", userid);
            www.SetRequestHeader("activity", pActivityName);
            www.SetRequestHeader("version", Application.version);
            www.SetRequestHeader("parameter", pBody.ToString());
            yield return www.SendWebRequest();
            if (result != null && !www.isNetworkError)
            {
               var pJson = JsonMapper.ToObject(www.downloadHandler.text);
                if( int.Parse( pJson["error"].ToString()) == 0)
                {
                    result?.Invoke(true, pJson["data"].ToJson());
                }
                else
                {
                    result?.Invoke(false, "");
                }
            }
            else
            {
                result?.Invoke(false, "");
            }
            www.Dispose();
            www = null;
        }
    }
}
