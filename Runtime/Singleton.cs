using UnityEngine;
using System;
using System.Collections.Generic;
using OmiyaGames.Web;

namespace OmiyaGames.Global
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="Singleton.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.0.0-preview.1<br/>
    /// <strong>Date:</strong> 9/22/2016<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.1.0-preview.1<br/>
    /// <strong>Date:</strong> 5/18/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Converting code to package.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Any GameObject with this script will not be destroyed when switching between
    /// scenes. However, only one instance of this script may exist in a scene.
    /// Allows retrieving any components in itself or its children.
    /// </summary>
    /// <seealso cref="ISingletonScript"/>
    public class Singleton : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public enum GenuineStatus
        {
            Unchecked = -1,
            IsGenuine = 0,
            NotGenuine,
            VerificationNotSupported
        }

        readonly Dictionary<Type, Component> mCacheRetrievedComponent = new Dictionary<Type, Component>();

        /// <summary>
        /// 
        /// </summary>
        public event Action<float> OnUpdate;
        /// <summary>
        /// 
        /// </summary>
        public event Action<float> OnRealTimeUpdate;
        /// <summary>
        /// 
        /// </summary>
        public event Action<float> OnLateUpdate;
        /// <summary>
        /// 
        /// </summary>
        public event Action<float> OnLateRealTimeUpdate;
        /// <summary>
        /// 
        /// </summary>
        public event Action<float> OnFixedUpdate;

        /// <summary>
        /// 
        /// </summary>
        public delegate void FocusChanged(bool before, bool after);
        /// <summary>
        /// 
        /// </summary>
        public event FocusChanged OnBeforeFocusChange;
        /// <summary>
        /// 
        /// </summary>
        public event Action<bool> OnAfterFocusChange;

        /// <summary>
        /// 
        /// </summary>
        [Header("Simulation")]
        [SerializeField]
        bool simulateMalformedGame = false;
#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        bool simulateWebplayer = false;
#endif

        /// <summary>
        /// 
        /// </summary>
        [Header("Store Information")]
        [SerializeField]
        PlatformSpecificLink storeUrls;

        ISingletonScript[] allSingletonScriptsCache = null;
        GenuineStatus genuineStatus = GenuineStatus.Unchecked;

        /// <summary>
        /// The static instance of this object.  Use this property to access it's information
        /// </summary>
        public static Singleton Instance
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// State indicating whether this app is focused (e.g. on WebGL,
        /// the app is capturing the mouse input properly) on this app or not.
        /// </summary>
        public bool IsAppFocused
        {
            get;
            private set;
        } = true;

        /// <summary>
        /// 
        /// </summary>
        public static COMPONENT Get<COMPONENT>() where COMPONENT : Component
        {
            COMPONENT returnObject = null;
            Type retrieveType = typeof(COMPONENT);
            if (Instance != null)
            {
                // Check if the component is in the cache
                if (Instance.mCacheRetrievedComponent.ContainsKey(retrieveType) == true)
                {
                    // If so, return that
                    returnObject = Instance.mCacheRetrievedComponent[retrieveType] as COMPONENT;
                }
                else
                {
                    // Attempt to grab a component from children
                    returnObject = Instance.GetComponentInChildren<COMPONENT>();
                    if(returnObject != null)
                    {
                        // If return object is not null, cache it
                        Instance.mCacheRetrievedComponent.Add(retrieveType, returnObject);
                    }
                }
            }
            return returnObject;
        }

        /// <summary>
        /// Property indicating if this is a web app or not.
        /// Basically checks if this is a WebGL build.
        /// </summary>
        public bool IsWebApp
        {
            get
            {
#if UNITY_EDITOR
                // Check if webplayer simulation checkbox is checked
                return simulateWebplayer;
#elif UNITY_WEBGL
                // Always return true if already on a webplayer
                return true;
#else
                // Always return false, otherwise
                return false;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSimulatingMalformedGame
        {
            get
            {
                bool returnFlag = simulateMalformedGame;

                // Check if we're not in the editor, and this build is in debug mode
#if !UNITY_EDITOR
                if (Debug.isDebugBuild == false)
                {
                    // Always return false
                    returnFlag = false;
                }
#endif
                // Check if simulation checkbox is checked
                return returnFlag;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PlatformSpecificStoreLink
        {
            get
            {
                return storeUrls.PlatformLink;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PlatformSpecificStoreLinkShortened
        {
            get
            {
                return Helpers.ShortenUrl(PlatformSpecificStoreLink);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WebsiteLink
        {
            get
            {
                return storeUrls.WebLink;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WebsiteLinkShortened
        {
            get
            {
                return Helpers.ShortenUrl(WebsiteLink);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetStoreLink(PlatformSpecificLink.SupportedPlatforms platform)
        {
            return storeUrls.GetPlatformLink(platform);
        }

        /// <summary>
        /// 
        /// </summary>
        public GenuineStatus CheckGenuine
        {
            get
            {
                if (genuineStatus == GenuineStatus.Unchecked)
                {
                    // We'll be using Unity's own method of checking piracy for now
                    genuineStatus = GenuineStatus.VerificationNotSupported;

                    // Make sure the check is available
                    if (Application.genuineCheckAvailable == true)
                    {
                        genuineStatus = GenuineStatus.NotGenuine;

                        // Make sure this copy is genuine
                        if (Application.genuine == true)
                        {
                            genuineStatus = GenuineStatus.IsGenuine;
                        }
                    }
                }
                return genuineStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPiracyDetected
        {
            get
            {
                return (CheckGenuine == GenuineStatus.NotGenuine);
            }
        }

        // Use this for initialization
        void Awake()
        {
            // Check if the singleton is not instanced yet
            if (Instance == null)
            {
                // Set the instance variable
                Instance = this;

                // Run all the events
                Instance.RunSingletonEvents();

                // Prevent this object from destroying itself
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Run all the events
                Instance.RunSingletonEvents();

                // Destroy this gameobject
                Destroy(gameObject);
            }
        }

        void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
            OnRealTimeUpdate?.Invoke(Time.unscaledDeltaTime);
        }

        void LateUpdate()
        {
            OnLateUpdate?.Invoke(Time.deltaTime);
            OnLateRealTimeUpdate?.Invoke(Time.unscaledDeltaTime);
        }

        void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(Time.fixedDeltaTime); 
        }

        void OnApplicationFocus(bool focus)
        {
            OnBeforeFocusChange?.Invoke(IsAppFocused, focus);
            IsAppFocused = focus;
            OnAfterFocusChange?.Invoke(IsAppFocused);
        }

        void RunSingletonEvents()
        {
            int index = 0;
            if(allSingletonScriptsCache == null)
            {
                // Cache all the singleton scripts
                allSingletonScriptsCache = Instance.GetComponentsInChildren<ISingletonScript>();

                // Go through every ISingletonScript, and run singleton awake
                for (index = 0; index < allSingletonScriptsCache.Length; ++index)
                {
                    // Run singleton awake
                    allSingletonScriptsCache[index].IsPartOfSingleton = true;
                    allSingletonScriptsCache[index].SingletonAwake();
                }
            }

            // Go through every ISingletonScript, and run scene awake
            for (index = 0; index < allSingletonScriptsCache.Length; ++index)
            {
                allSingletonScriptsCache[index].SceneAwake();
            }
        }
    }
}
