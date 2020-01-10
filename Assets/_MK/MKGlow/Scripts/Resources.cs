//////////////////////////////////////////////////////
// MK Glow Resources	    	    	       		//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
using EazyEngine.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
#if UNITY_EDITOR
using UnityEditor;
#endif
*/

#pragma warning disable
namespace MK.Glow
{
    [System.Serializable]
    /// <summary>
    /// Stores runtime required resources
    /// </summary>
    public sealed class Resources : ScriptableObject
    {        
        internal static void ResourcesNotAvailableWarning()
        {
            Debug.LogWarning("MK Glow resources asset couldn't be found. Effect will be skipped.");
        }

        internal static MK.Glow.Resources LoadResourcesAsset()
        {
            return instance!= null ? instance : UnityEngine.Resources.Load<MK.Glow.Resources>("MKGlowResources");
        }
        static MK.Glow.Resources instance;
        internal static void LoadResourcesAsyncAsset()
        {
           var pAsync = UnityEngine.Resources.LoadAsync<MK.Glow.Resources>("MKGlowResources");
            pAsync.completed += delegate (AsyncOperation a)
            {
                instance = (MK.Glow.Resources)((ResourceRequest)a).asset;
                instance.init();
            };
        }
        public void init()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 && Application.platform != RuntimePlatform.IPhonePlayer )
            {
                var pAsync = _computeShaderGles.loadAssetAsync<ComputeShader>();
                pAsync.completed += delegate (AsyncOperation a)
                {
                    if(((ResourceRequest)a).asset == null)
                    {
                        _computeShaderGles.Asset = _computeShader;
                    }
                    else{
                        _computeShaderGles.Asset = (ComputeShader)(((ResourceRequest)a).asset);
                    }
                   
                };
            }
        }

        /*
        #if UNITY_EDITOR
        //[MenuItem("Window/MK/Glow/Create Resources Asset")]
        static void CreateAsset()
        {
            Resources asset = ScriptableObject.CreateInstance<Resources>();

            AssetDatabase.CreateAsset(asset, "Assets/_MK/MKGlow/Resources.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
        #endif
        */

        [SerializeField]
        private Texture2D _lensSurfaceDirtTextureDefault;
        internal Texture2D lensSurfaceDirtTextureDefault { get { return _lensSurfaceDirtTextureDefault; } }
        [SerializeField]
        private Texture2D _lensSurfaceDiffractionTextureDefault;
        internal Texture2D lensSurfaceDiffractionTextureDefault { get { return _lensSurfaceDiffractionTextureDefault; } }
        [SerializeField]
        private Texture2D _lensFlareColorRampDefault;
        internal Texture2D lensFlareColorRampDefault { get { return _lensFlareColorRampDefault; } }

        [SerializeField]
        private Shader _selectiveRenderShader;
        internal Shader selectiveRenderShader { get { return _selectiveRenderShader; } }
        [SerializeField]
        private Shader _sm20Shader;
        internal Shader sm20Shader { get { return _sm20Shader; } }
        [SerializeField]
        private Shader _sm25Shader;
        internal Shader sm25Shader { get { return _sm25Shader; } }
        [SerializeField]
        private Shader _sm30Shader;
        internal Shader sm30Shader { get { return _sm30Shader; } }
        [SerializeField]
        private Shader _sm35Shader;
        internal Shader sm35Shader { get { return _sm35Shader; } }
        [SerializeField]
        private Shader _sm40Shader;
        internal Shader sm40Shader { get { return _sm40Shader; } }
        [SerializeField]
        private Shader _sm40GeometryShader;
        internal Shader sm40GeometryShader { get { return _sm40GeometryShader; } }

        [SerializeField]
        private ComputeShader _computeShader;
        [SerializeField]
        private AssetSelectorRef _computeShaderGles;
        internal ComputeShader computeShader { get { return (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 && Application.platform != RuntimePlatform.IPhonePlayer) ? (ComputeShader)_computeShaderGles.Asset : _computeShader; } }
    }
}