using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

namespace EazyEngine.Tools
{
 public class FireBaseInit : MonoBehaviour
 {
  // Start is called before the first frame update
  void Start()
  {
   FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
   {
    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
   });
  }
 }
}

