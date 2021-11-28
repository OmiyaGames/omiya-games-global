using UnityEngine;

namespace OmiyaGames.Global
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="ComponentSingleton.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2021 Omiya Games
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
    /// <strong>Version:</strong> 1.1.0<br/>
    /// <strong>Date:</strong> 11/27/2021<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Use this class to get a static instance of a component.
    /// Mainly used to have a default instance.
    /// </summary>
    /// <remarks>
    /// Code from Unity's Core RenderPipeline package (<c>ComponentSingleton<TType></c>.)
    /// </remarks>
    /// <typeparam name="T">Component type.</typeparam>
    public static class ComponentSingleton<T> where T : Component
    {
        static T instance = null;

        /// <summary>
        /// Instance of the required component type.
        /// </summary>
        /// <remarks>
        /// If this property creates a <see cref="GameObject"/>,
        /// it will be deactivated by default.
        /// </remarks>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // Create gameobject
                    GameObject go = new GameObject(typeof(T).Name)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    go.SetActive(false);

                    // Create component
                    instance = go.AddComponent<T>();
                }

                return instance;
            }
        }

        /// <summary>
        /// Release the component singleton.
        /// </summary>
        public static void Release()
        {
            if (instance != null)
            {
                Helpers.Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}
