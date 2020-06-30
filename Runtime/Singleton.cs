using UnityEngine;
using System;
using System.Collections.Generic;
using OmiyaGames.Web;
using UnityEngine.Serialization;

namespace OmiyaGames.Global
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="Singleton.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2020 Omiya Games
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
    /// <strong>Date:</strong> 9/22/2016<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item> <item>
    /// <term>
    /// <strong>Version:</strong> 0.1.0-preview.1<br/>
    /// <strong>Date:</strong> 5/18/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Converting code to package.</description>
    /// </item> <item>
    /// <term>
    /// <strong>Version:</strong> 0.1.1-preview.1<br/>
    /// <strong>Date:</strong> 6/29/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>
    /// Updating code to compile with the latest version of Common and Web package.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Any GameObject with this script will not be destroyed when switching between
    /// scenes. However, only one instance of this script may exist in a scene.
    /// Allows retrieving any components in itself or its children.
    /// <seealso cref="ISingletonScript"/>
    /// </summary>
    public class Singleton : MonoBehaviour
    {
        /// <summary>
        /// Status of this build, based on <see cref="Application.genuine"/> and <see cref="Application.genuineCheckAvailable"/>.
        /// </summary>
        /// <seealso cref="Application.genuine"/>
        /// <seealso cref="Application.genuineCheckAvailable"/>.
        public enum GenuineStatus : short
        {
            /// <summary>
            /// Verification for whether this build is genuine or not has been executed yet.
            /// </summary>
            Unchecked = -1,
            /// <summary>
            /// According to Unity, build is genuine.
            /// Same as <see cref="Application.genuine"/> returning true.
            /// </summary>
            IsGenuine = 0,
            /// <summary>
            /// According to Unity, build is <em>not</em> genuine.
            /// Same as <see cref="Application.genuine"/> returning false.
            /// </summary>
            NotGenuine,
            /// <summary>
            /// Unity has no ability to confim whether this build is genuine or not.
            /// Same as <see cref="Application.genuineCheckAvailable"/> returning false.
            /// </summary>
            VerificationNotSupported
        }

        /// <summary>
        /// A cache of child components attached to this Singleton.
        /// </summary>
        /// <seealso cref="Get{COMPONENT}"/>
        private readonly Dictionary<Type, Component> mCacheRetrievedComponent = new Dictionary<Type, Component>();

        /// <summary>
        /// Called on Unity's event, OnUpdate().
        /// May slightly improve performance than simply defining the method.
        /// Argument is <see cref="Time.deltaTime"/>.
        /// </summary>
        public event Action<float> OnUpdate;
        /// <summary>
        /// Called on Unity's event, OnUpdate().
        /// May slightly improve performance than simply defining the method.
        /// Argument is <see cref="Time.unscaledDeltaTime"/>.
        /// </summary>
        public event Action<float> OnRealTimeUpdate;
        /// <summary>
        /// Called on Unity's event, OnLateUpdate().
        /// May slightly improve performance than simply defining the method.
        /// Argument is <see cref="Time.deltaTime"/>.
        /// </summary>
        public event Action<float> OnLateUpdate;
        /// <summary>
        /// Called on Unity's event, OnLateUpdate().
        /// May slightly improve performance than simply defining the method.
        /// Argument is <see cref="Time.unscaledDeltaTime"/>.
        /// </summary>
        public event Action<float> OnLateRealTimeUpdate;
        /// <summary>
        /// Called on Unity's event, OnFixedUpdate().
        /// May slightly improve performance than simply defining the method.
        /// Argument is <see cref="Time.deltaTime"/>.
        /// </summary>
        public event Action<float> OnFixedUpdate;

        /// <summary>
        /// Indicates how <see cref="IsAppFocused"/> changes.
        /// </summary>
        public delegate void FocusChanged(bool before, bool after);
        /// <summary>
        /// Called on Unity's event, OnApplicationFocus(bool),
        /// and before <see cref="IsAppFocused"/> changes.
        /// </summary>
        public event FocusChanged OnBeforeFocusChange;
        /// <summary>
        /// Called on Unity's event, OnApplicationFocus(bool),
        /// and after <see cref="IsAppFocused"/> changes.
        /// </summary>
        public event Action<bool> OnAfterFocusChange;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// Makes <see cref="OmiyaGames"/>.* tools and libraries to pretend this
        /// is not a genuine build of the game.
        /// </summary>
        [Header("Simulation")]
        [SerializeField]
        private bool simulateMalformedGame = false;
#endif
#if UNITY_EDITOR
        /// <summary>
        /// Makes <see cref="OmiyaGames"/>.* tools and libraries to pretend this
        /// is a WebGL build.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("simulateWebplayer")]
        private bool simulateWebGl = false;
#endif

        /// <summary>
        /// Links to this game's listing on various stores
        /// </summary>
        [Header("Store Information")]
        [SerializeField]
        private PlatformSpecificLink storeUrls;

        private ISingletonScript[] allSingletonScriptsCache = null;
        private GenuineStatus genuineStatus = GenuineStatus.Unchecked;

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
        /// Retrieves a <see cref="Component"/> under <see cref="Singleton"/>'s <see cref="GameObject"/>,
        /// or its children.
        /// </summary>
        /// <remarks>
        /// For efficiency, <see cref="Component"/>s retrieved from prior calls are cached.
        /// </remarks>
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
                    if (returnObject != null)
                    {
                        // If return object is not null, cache it
                        Instance.mCacheRetrievedComponent.Add(retrieveType, returnObject);
                    }
                }
            }
            return returnObject;
        }

        /// <summary>
        /// Indicates if this build is a web app or not.
        /// Basically checks if this is a WebGL build.
        /// </summary>
        public bool IsWebApp
        {
            get
            {
#if UNITY_EDITOR
                // Check if webplayer simulation checkbox is checked
                return simulateWebGl;
#elif UNITY_WEBGL
                // Always return true if WebGL build
                return true;
#else
                // Always return false, otherwise
                return false;
#endif
            }
        }

        /// <summary>
        /// Indicates whether the developer wants to pretend this build is not genuine.
        /// </summary>
        public bool IsSimulatingMalformedGame
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // If in editor or debug mode, return member variable.
                return simulateMalformedGame;
#else
                // Otherwise, always return false
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets the listing of this game on a store most relevant to this build's platform.
        /// </summary>
        public string PlatformSpecificStoreLink
        {
            get
            {
                return storeUrls.PlatformLink;
            }
        }

        /// <summary>
        /// Gets the slightly shortened listing of this game on a store most relevant to this build's platform.
        /// </summary>
        /// <seealso cref="PlatformSpecificStoreLink"/>
        /// <seealso cref="Helpers.ShortenUrl(string)"/>
        public string PlatformSpecificStoreLinkShortened
        {
            get
            {
                return UrlHelpers.ShortenUrl(PlatformSpecificStoreLink);
            }
        }

        /// <summary>
        /// Gets the link to this game's website.
        /// </summary>
        public string WebsiteLink
        {
            get
            {
                return storeUrls.WebLink;
            }
        }

        /// <summary>
        /// Gets the link to this game's website, shortened.
        /// </summary>
        /// <seealso cref="WebsiteLink"/>
        /// <seealso cref="Helpers.ShortenUrl(string)"/>
        public string WebsiteLinkShortened
        {
            get
            {
                return UrlHelpers.ShortenUrl(WebsiteLink);
            }
        }

        /// <summary>
        /// Gets the listing of this game on a store most relevant to <paramref name="platform"/>.
        /// </summary>
        public string GetStoreLink(PlatformSpecificLink.SupportedPlatforms platform)
        {
            return storeUrls.GetPlatformLink(platform);
        }

        /// <summary>
        /// Gets whether this build is genuine or not, through Unity's methods.
        /// </summary>
        /// <seealso cref="Application.genuine"/>
        /// <seealso cref="Application.genuineCheckAvailable"/>
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
            if (allSingletonScriptsCache == null)
            {
                // Cache all the singleton scripts
                allSingletonScriptsCache = Instance.GetComponentsInChildren<ISingletonScript>();

                // Go through every ISingletonScript, and run singleton awake
                foreach (ISingletonScript script in allSingletonScriptsCache)
                {
                    // Run singleton awake
                    script.IsPartOfSingleton = true;
                    script.SingletonAwake();
                }
            }

            // Go through every ISingletonScript, and run scene awake
            foreach (ISingletonScript script in allSingletonScriptsCache)
            {
                script.SceneAwake();
            }
        }
    }
}
