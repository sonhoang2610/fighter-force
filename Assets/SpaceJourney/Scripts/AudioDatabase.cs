﻿using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
namespace EazyEngine.Audio
{
    [System.Serializable]
    public class AudioGroupInfo
    {
#if UNITY_EDITOR
        public string labelGroup()
        {
            return groupName + "(" +( clips == null ? "0" : clips.Length.ToString() )+ ")";
        }
#endif
        public string groupName;
        [Range(0, 1)]
        public float volume = 1;
        public AudioElementInfos[] clips;
    }
    [System.Serializable]
    public class MusicGroupInfo 
    {
#if UNITY_EDITOR
        public string labelGroup()
        {
            return groupName + "(" + (clips == null ? "0" : clips.Length.ToString()) + ")";
        }
#endif
        public string groupName;
        [Range(0, 1)]
        public float volume = 1;
        public MusicElementInfos[] clips;
    }
    [System.Serializable]
    public class AudioElementInfo
    {
        public AudioClip clip;
        [Range(0,1)]
        public float volume = 1;
        public float delay = 0;
        public bool isLoop = false;
    }
    [System.Serializable]
    public class AudioElementInfos
    {
        public AudioElementInfo[] elements;
        public int weight = 1;
    }
    public enum MusicPlayType
    {
        Sequence,
        All
    }
    [System.Serializable]
    public class MusicElementInfos : AudioElementInfos
    {
        [PropertyOrder(0)]
        public MusicPlayType playType;
    }
    [CreateAssetMenu(menuName = "EazyEngine/AudioDatabase",fileName = "AudioDatabase")]
    public class AudioDatabase : EzScriptTableObject
    {
        public static AudioDatabase dataBase;
        public static AudioDatabase Instance
        {
            get
            {
                if (dataBase != null)
                {
                    return dataBase;
                }
                dataBase = LoadAssets.loadAsset<AudioDatabase>("AudioDatabase", "Variants/Database/");
                return dataBase;
            }
        }

#if UNITY_EDITOR
        [OnValueChanged("changeFindField")]
        public string findField = "";
        [FoldoutGroup("Search Result")]
        [HideLabel]
        public AudioGroupInfo seatchResult;
        public void changeFindField()
        {
            foreach (var pGroup in groups)
            {
                if (pGroup.groupName.Contains(findField))
                {
                    seatchResult = pGroup;
                    break;
                }
            }
        }
#endif

        [ListDrawerSettings(ListElementLabelName = "labelGroup")]
        public AudioGroupInfo[] groups;
        [ListDrawerSettings(ListElementLabelName = "labelGroup")]
        public MusicGroupInfo[] musics;
        public string[] ArrayGroupNameSound
        {
            get
            {
                List<string> pGroups = new List<string>();
                for(int i = 0; i < groups.Length; ++i)
                {
                    pGroups.Add(groups[i].groupName);
                }
                return pGroups.ToArray();
            } 
        }
        public string[] ArrayGroupNameMusics
        {
            get
            {
                List<string> pGroups = new List<string>();
                for (int i = 0; i < musics.Length; ++i)
                {
                    pGroups.Add(musics[i].groupName);
                }
                return pGroups.ToArray();
            }
        }

#if UNITY_EDITOR
        public string filePath;
        [Button("Generate Constrant")]
        public void GenerateFileConstrant()
        {
            string pPath = Application.dataPath + filePath;
            Debug.Log(pPath);
            if (File.Exists(pPath))
            {
                var pFileStream = File.OpenText(pPath);
                var pString = pFileStream.ReadToEnd();
                pFileStream.Close();
                int pStartIndex = -1;
                int pEndIndex = -1;
                for (int i = 0; i <pString.Length -19; ++i)
                {
                    var pStartText = pString.Substring(i, 19);
                    if (pStartText == "//Constraint region")
                    {
                         pStartIndex = i+19;
                    }
                }
                for (int i = 0; i < pString.Length - 12; ++i)
                {
                    var pStartText = pString.Substring(i, 12);
                    if (pStartText == "//end region")
                    {
                         pEndIndex = i;
                    }
                }
                if(pStartIndex != -1 && pEndIndex != -1 && pEndIndex > pStartIndex)
                {
                    var pConstrainRegion = pString.Substring(pStartIndex + 2, pEndIndex - pStartIndex - 8);
                    string pReplaceString = "";
                    for(int i = 0; i < groups.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(groups[i].groupName))
                        {
                            pReplaceString += $"public static string {groups[i].groupName} = \"{ groups[i].groupName}\";\n";
                        }
                    }
                    string pMusic = "public static class MusicRegion \n{\n $replace \n}\n";
                    string pReplaceMusic = "";
                    for(int i = 0; i < musics.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(musics[i].groupName))
                        {
                            pReplaceMusic += $"public static string {musics[i].groupName} = \"{ musics[i].groupName}\";\n";
                        }
                    }
                    pMusic = pMusic.Replace("$replace", pReplaceMusic);
                    pReplaceString += pMusic;
                    pString = pString.Replace(pConstrainRegion, pReplaceString);
                    File.WriteAllText(pPath, pString);
                    Debug.Log(pConstrainRegion);
                    AssetDatabase.Refresh();
                }
            }
        }
#endif
    }
}