#region copyright
//------------------------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using Core;

	[Serializable]
	public class ReferencesFinderSettings
	{
		public FilterItem[] pathIgnoresFilters = new FilterItem[0];
		public FilterItem[] pathIncludesFilters = new FilterItem[0];
	}
}