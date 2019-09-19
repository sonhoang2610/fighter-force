﻿#region copyright
//------------------------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using UnityEditor.IMGUI.Controls;

	[Serializable]
	public class ReferencesFinderPersonalSettings
	{
		public bool showAssetsWithoutReferences;
		public bool selectedFindClearsResults;

		public bool fullProjectScanWarningShown;
		public string searchString;

		public TreeViewState referencesTreeViewState;
		public MultiColumnHeaderState referencesTreeHeaderState;

		public string splitterState;
	}
}