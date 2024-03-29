﻿using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace OmiyaGames.Global.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SettingsEditor.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2020-2022 Omiya Games
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
	/// <strong>Version:</strong> 1.2.0<br/>
	/// <strong>Date:</strong> 2/4/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// <item></item>
	/// <term>
	/// <strong>Version:</strong> 1.3.0<br/>
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Adding <seealso cref="UxmlPath"/> and
	/// <seealso cref="CreateInspectorGUI"/> to allow
	/// customizing this editor a little better.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Editor for <see cref="ScriptableObject"/> used as project settings.
	/// Appears under inspector when selecting the asset.
	/// </summary>
	public abstract class SettingsEditor : UnityEditor.Editor
	{
		public const string HelpInfo = "This is a project settings asset that can be edited in the Project Settings window. Please do not move or delete it from the project. If using a version control tool, remember to add this asset into the repo.";
		static readonly GUIContent EditButtonText = new GUIContent("Edit in Project Settings");

		/// <summary>
		/// The name this settings will appear in the
		/// Project Setting's left-sidebar.
		/// </summary>
		public abstract string SidebarDisplayPath
		{
			get;
		}

		/// <summary>
		/// The path to the UXML file for this editor.
		/// Returns null if one isn't intended to be displayed.
		/// </summary>
		public virtual string UxmlPath => null;

		/// <summary>
		/// The visual element to display
		/// </summary>
		/// <remarks>
		/// Override to replace this property and/or
		/// <seealso cref="CreateInspectorGUI"/>
		/// to customize the editor to your liking.
		/// </remarks>
		protected VisualElement RootElement
		{
			get;
			set;
		} = null;

		/// <inheritdoc/>
		public override VisualElement CreateInspectorGUI()
		{
			// Check if UXML is empty
			if (string.IsNullOrEmpty(UxmlPath))
			{
				// do default behavior
				return base.CreateInspectorGUI();
			}

			// Create a tree from the UXML file.
			RootElement = new VisualElement();
			VisualTreeAsset originalTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
			originalTree.CloneTree(RootElement);

			// Bind to the object
			RootElement.Bind(serializedObject);
			return RootElement;
		}

		/// <summary>
		/// Draws a message and a button prompting
		/// user to open Project Settings window.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Check if root element exists
			if (RootElement != null)
			{
				// Do default behavior
				base.OnInspectorGUI();
				return;
			}

			// Draw custom inspector
			serializedObject.Update();

			// Post help box
			EditorGUILayout.HelpBox(HelpInfo, MessageType.Info);

			// Draw button
			if (GUILayout.Button(EditButtonText) == true)
			{
				// Open Project Settings
				SettingsService.OpenProjectSettings(SidebarDisplayPath);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

