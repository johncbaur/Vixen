﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewCustomCreateForm : Form
	{
		public PreviewCustomCreateForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		public string TemplateName
		{
			get { return textBoxTemplateName.Text; }
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (TemplateName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
				MessageBox.Show(
					"The template name must be a valid file name. Please ensure there are no invalid characters in the template name.",
					"Invalid Template Name", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
			}
			else {
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void button_Paint(object sender, PaintEventArgs e)
		{
			ThemeButtonRenderer.OnPaint(sender, e, null);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = true;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = false;
			var btn = sender as Button;
			btn.Invalidate();
		}
	}
}