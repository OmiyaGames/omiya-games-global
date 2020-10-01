using System.Text;
using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Global.Editor
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="SettingsHelpers.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2020 Omiya Games
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
    /// <strong>Version:</strong> 0.2.0-preview.1<br/>
    /// <strong>Date:</strong> 6/29/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper methods for setting up settings.
    /// </summary>
    public static class SettingsHelpers
    {
        /// <summary>
        /// Common directory to retrieve settings.
        /// </summary>
        public const string SettingsDirectory = "Assets/Omiya Games/Settings/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFullOmiyaGamesSettingsPath(string fileName)
        {
            StringBuilder assetPathBuilder = new StringBuilder(fileName.Length + SettingsDirectory.Length);
            assetPathBuilder.Append(SettingsDirectory);
            assetPathBuilder.Append(fileName);
            return assetPathBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="SETTINGS"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SETTINGS CreateOmiyaGamesSettings<SETTINGS>(string fileName) where SETTINGS : ScriptableObject
        {
            // Create the folders first
            AssetHelpers.CreateFolderRecursively(SettingsDirectory);

            // Create the asset
            SETTINGS settings = ScriptableObject.CreateInstance<SETTINGS>();
            string fullAssetPath = GetFullOmiyaGamesSettingsPath(fileName);
            AssetDatabase.CreateAsset(settings, fullAssetPath);

            // Save the asset
            AssetDatabase.SaveAssets();
            return settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="SETTINGS"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SETTINGS GetOmiyaGamesSettings<SETTINGS>(string fileName) where SETTINGS : ScriptableObject
        {
            string fullAssetPath = GetFullOmiyaGamesSettingsPath(fileName);
            SETTINGS settings = AssetDatabase.LoadAssetAtPath<SETTINGS>(fullAssetPath);
            if (settings == null)
            {
                settings = CreateOmiyaGamesSettings<SETTINGS>(fileName);
            }
            return settings;
        }
    }
}
