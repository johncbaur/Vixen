using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.OutputFilter.DimmingFilter;

namespace VixenModules.OutputFilter.FixtureBreakdown
{
	public partial class DimmingFilterSetup : /*Base*/Form
	{
		private DimmingFilterData _data;
		public DimmingFilterSetup(DimmingFilterData data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_data = data;

			if (_data.ConvertRGBIntoDimmingIntents)
			{
				checkBoxConvert.CheckState = CheckState.Checked;
			}

			textBoxTag.Text = _data.Tag;
		}
					
		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void checkBoxConvert_CheckedChanged(object sender, EventArgs e)
		{
			_data.ConvertRGBIntoDimmingIntents = !_data.ConvertRGBIntoDimmingIntents;
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			_data.ConvertRGBIntoDimmingIntents = (checkBoxConvert.CheckState == CheckState.Checked);
			_data.Tag = textBoxTag.Text;
		}
	}
}
