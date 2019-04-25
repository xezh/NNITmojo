﻿// Created:       2009-06-21
// Last Modified: 2018-07-31
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using mojoPortal.Business.WebHelpers;
using SurveyFeature.Business;
using System;

namespace mojoPortal.Features
{
	public class SurveyContentDeleteHandler : ContentDeleteHandlerProvider
	{
		public SurveyContentDeleteHandler()
		{ }


		public override void DeleteContent(int moduleId, Guid moduleGuid)
		{
			Survey.DeleteFromModule(moduleId);
		}
	}
}