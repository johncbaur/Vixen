using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.DimmingFilter.Filters
{
	/// <summary>
	/// Fixture filter specific to dimmer functions.
	/// This filter handles color intents to help extract out the intensity.
	/// </summary>
	public class DimmingFilter : TaggedFilter.Filters.TaggedFilter
	{
		#region Public Methods

		/// <summary>
		/// Handles a discrete color intent.
		/// </summary>		
		/// <param name="obj">Intent to handle</param>
		public override void Handle(IIntentState<DiscreteValue> intent)
		{
			// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
			IntentValue = intent;			
		}

		/// <summary>
		/// Handles an RGB color intent.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<RGBValue> intent)
		{
			// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
			IntentValue = intent;			
		}

		//TODO: Need to figure out when we hit this method!
		public override void Handle(IIntentState<LightingValue> intent)
		{
			IntentValue = intent;			
		}
		
		#endregion
	}
}