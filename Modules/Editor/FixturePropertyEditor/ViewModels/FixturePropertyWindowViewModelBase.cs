using Catel.MVVM;
using System.Windows.Input;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Abstract base class for Fixture Property window view models. 
    /// </summary>
    public abstract class FixturePropertyWindowViewModelBase : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FixturePropertyWindowViewModelBase()
        {
            // Create the OK button command
            OkCommand = new Command(OK, CanExecuteOK);

            // Create the Cancel button
            CancelCommand = new Command(Cancel);
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// Ok button command.
        /// </summary>
        public ICommand OkCommand { get; private set; }

        /// <summary>
        /// Cancel button command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines if the OK command can execute.
        /// </summary>
        /// <returns>True when the OK command can execute.</returns>
        protected abstract bool CanExecuteOK();
              
        /// <summary>
        /// OK command handler.
        /// </summary>
        protected virtual void OK()
        {            
            // Call Catel processing
            this.SaveAndCloseViewModelAsync();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Cancel button command handler.
        /// </summary>
        private void Cancel()
        {
            // Call Catel processing
            this.CancelAndCloseViewModelAsync();
        }

        #endregion
    }
}
