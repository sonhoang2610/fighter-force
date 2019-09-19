﻿#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System.Collections.Generic;

	internal class IssueDetectorBase
	{
		protected List<IssueRecord> issues;

		public IssueDetectorBase(List<IssueRecord> issues)
		{
			this.issues = issues;
		}
	}
}