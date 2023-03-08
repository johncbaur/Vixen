﻿// =====================================================================
// AboutBox.cs
// Generated by Visual Studio tools and then customized
// version 1.0.0.0 - 1 june 2010
// =====================================================================

// =====================================================================
// Copyright (c) 2010 Joshua 1 Systems Inc. All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright notice, this list of
//       conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright notice, this list
//       of conditions and the following disclaimer in the documentation and/or other materials
//       provided with the distribution.
// THIS SOFTWARE IS PROVIDED BY JOSHUA 1 SYSTEMS INC. "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Joshua 1 Systems Inc.
// =====================================================================

using Common.Controls;

namespace VixenModules.Controller.E131
{
	using System.IO;
	using System.Reflection;

	internal partial class AboutBox : BaseForm
	{
		public AboutBox()
		{
			InitializeComponent();
			SetText();
			labelProductName.Text = AssemblyProduct;
			labelVersion.Text = string.Format("Version {0}", AssemblyVersion);
			textBoxDescription.Text = AssemblyDescription;
		}

		public string AssemblyCompany
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
				if (attributes.Length == 0) {
					return string.Empty;
				}

				return ((AssemblyCompanyAttribute) attributes[0]).Company;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0) {
					return string.Empty;
				}

				return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
			}
		}

		public string AssemblyTitle
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
				if (attributes.Length > 0) {
					var titleAttribute = (AssemblyTitleAttribute) attributes[0];
					if (titleAttribute.Title
					    != string.Empty) {
						return titleAttribute.Title;
					}
				}

				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
			}
		}

		private static string AssemblyProduct
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false);
				if (attributes.Length == 0) {
					return string.Empty;
				}

				return ((AssemblyProductAttribute) attributes[0]).Product;
			}
		}

		private static string AssemblyVersion
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		private static string AssemblyDescription
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0) {
					return string.Empty;
				}

				return ((AssemblyDescriptionAttribute) attributes[0]).Description;
			}
		}

		private void SetText()
		{
			Text = string.Format("About {0}", AssemblyProduct);
		}
	}
}