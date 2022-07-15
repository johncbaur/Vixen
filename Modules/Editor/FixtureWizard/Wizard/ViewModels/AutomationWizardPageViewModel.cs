namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	using Catel.MVVM;
	using Orc.Wizard;
	using System.Linq;
	using VixenModules.App.Fixture;
	using VixenModules.Editor.FixtureWizard.Wizard.Models;

    /// <summary>
    /// Wizard view model page that configures fixture automation options.
    /// </summary>
	public class AutomationWizardPageViewModel : WizardPageViewModelBase<AutomationWizardPage>
    {
		#region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Automation wizard page model object</param>
		public AutomationWizardPageViewModel(AutomationWizardPage wizardPage) :
            base(wizardPage)
        {
            // Get the color support page model
            ColorSupportWizardPage colorSupportPage = (ColorSupportWizardPage)Wizard.Pages.Single(page => page is ColorSupportWizardPage);

            // If the user picked some color support then...
            if (!colorSupportPage.NoColorSupport)
            {
                // Default to automating the opening and closing of the shutter
                AutomaticallyOpenAndCloseShutter = true;
            }

            // If the fixture has a color wheell then...
            if (colorSupportPage.ColorWheel)
            {
                // Default to converting color intents into color wheel commands
                AutomaticallyControlColorWheel = true;

                // Default to converting color intensity into dimmer intents
                AutomaticallyControlDimmer = true;

                // Enable the Color Wheel checkbox
                EnableColorWheel = true;

                // Enable the Dimmer checkbox
                EnableDimmer = true;
            }     
            // Otherwise the fixture is color mixing
            else 
            {
                // Disable the color wheel automation option
                EnableColorWheel = false;

                // Disable the dimmer automation option
                EnableDimmer = false;
            }

            // Retrieve the select profile page model
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);

            // Attempt to find an Open Prism function on the fixture
            FixtureFunction openPrism = selectProfilePage.Fixture.FunctionDefinitions.SingleOrDefault(func => func.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.OpenClosePrism);

            // If an open prism function has been found then..
            if (openPrism != null)
            {
                // If the fixture's channels reference the Open Prism function then...
                if (selectProfilePage.Fixture.ChannelDefinitions.Any(channel => channel.Function == openPrism.Name))
                {
                    // Enable the Prism automation option
                    EnablePrism = true;

                    // Default to automatically opening and closing prism
                    AutomaticallyOpenAndClosePrism = true;
                }
            }
        }

		#endregion

		#region Public Catel Properties

        /// <summary>
        /// Indicates whether to automate opening and closing the shutter.
        /// </summary>
		[ViewModelToModel]
        public bool AutomaticallyOpenAndCloseShutter { get; set; }

        /// <summary>
        /// Indicates whether to automate converting color intents into color wheel index commands.
        /// </summary>
        [ViewModelToModel]
        public bool AutomaticallyControlColorWheel { get; set; }        

        /// <summary>
        /// Indicats whether to automate converting color intensity into dimmer intents.
        /// </summary>
        [ViewModelToModel]
        public bool AutomaticallyControlDimmer { get; set; }

        /// <summary>
        /// Controls whether filters are used to automatically open and close the prism based on
        /// presence or abscence of prism index intents.
        /// </summary>
        [ViewModelToModel]
        public bool AutomaticallyOpenAndClosePrism { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Enables the color wheel automation option.
        /// </summary>
        public bool EnableColorWheel { get; set; }

        /// <summary>
        /// Enables the dimmer automation option.
        /// </summary>
        public bool EnableDimmer { get; set; }

        /// <summary>
        /// Enables the prism automation option.
        /// </summary>
        public bool EnablePrism { get; set; }

        #endregion
    }
}
