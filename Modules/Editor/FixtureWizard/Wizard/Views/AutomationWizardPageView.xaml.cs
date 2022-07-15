﻿namespace VixenModules.Editor.FixtureWizard.Wizard.Views
{
    /// <summary>
    /// Maintains the fixture automation wizard page view.
    /// </summary>
    public partial class AutomationWizardPageView
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public AutomationWizardPageView()
        {
            InitializeComponent();
        }

		#endregion

		#region Private Methods

        /// <summary>
        /// User control loaded event, used to draw background graphics.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e"><Event arguments/param>
		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{     
            // Draw two moving heads providing bookends to the widgets in the middle of the page
            DrawMovingHeads(400, System.Drawing.Color.Purple, LeftMainViewport, RightMainViewport);
        }

        #endregion
    }
}
