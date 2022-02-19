using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using VixenModules.App.FixtureSpecificationManager;

namespace VixenModules.OutputFilter.ColorWheelBreakdown
{
	public partial class ColorWheelBreakdownSetup : /*Base*/Form
	{
		private ColorWheelBreakdownData _data;
		
		public ColorWheelBreakdownSetup(ColorWheelBreakdownData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_data = data;

			IFixtureSpecificationManager fixtureManager = FixtureSpecificationManager.Instance();
			string[] fixtureNames = fixtureManager.FixtureSpecifications.Select(item => item.Name).ToArray();
			_fixtureComboBox.Items.AddRange(fixtureNames);

			if (fixtureNames.Contains(_data.FixtureName))
			{
				_fixtureComboBox.SelectedItem = _data.FixtureName;
				SelectedFixtureSpecification = _data.FixtureName;
			}
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

		public string SelectedFixtureSpecification { get; set; }

		private void _fixtureComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			SelectedFixtureSpecification = (string)_fixtureComboBox.SelectedItem;
		}
	}
}
