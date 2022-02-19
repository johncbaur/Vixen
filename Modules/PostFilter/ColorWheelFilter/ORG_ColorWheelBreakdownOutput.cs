using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Sys;
using VixenModules.App.Fixture;

namespace VixenModules.OutputFilter.ColorWheelBreakdown
{
	internal class ColorWheelBreakdownOutput : IDataFlowOutput<CommandsDataFlowData>
	{
		/// <summary>
		/// Collection of output commands.
		/// </summary>
		private readonly List<ICommand> _outputCommands;
		private readonly ColorWheelBreakdownFilter _filter;
		private readonly CommandsDataFlowData _commandsData;
		private List<FixtureColorWheel> _colorWheelData;

		public ColorWheelBreakdownOutput(string functionName, List<FixtureColorWheel> colorWheelData)
		{
			_colorWheelData = colorWheelData;

			_filter = new ColorWheelBreakdownFilter(functionName);

			// Create the collection of output commands
			_outputCommands = new List<ICommand>();
			
			// Create the IDataFlowOutput output wrapping the output commands
			_commandsData = new CommandsDataFlowData(_outputCommands);			
		}

		#region Public Methods

		/// <summary>
		/// Processes input intent data.
		/// </summary>
		/// <param name="intents">Intents to process</param>
		public void ProcessInputData(IntentsDataFlowData intents)
		{
			// Clear the output commands
			_outputCommands.Clear();

			// Loop over the intent states
			foreach (IIntentState intentState in intents.Value)
			{
				// Determine if the intent state is supported by this breakdown filter
				IIntentState state = _filter.Filter(intentState);
				
				// If the state is not null then...
				if (state != null)
				{
					// Handle the intent
					Handle((IIntentState<RGBValue>)state);
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles a <c>PositionValue</c> intent.
		/// </summary>
		/// <param name="intent"></param>
		private void Handle(IIntentState<RGBValue> intent)
		{

			RGBValue theRGBValue = intent.GetValue();
			Color color = theRGBValue.FullColor;

			FixtureColorWheel colorWheelItem = _colorWheelData.FirstOrDefault(
				colorWheelEntry => colorWheelEntry.Color1 == color &&
				colorWheelEntry.Color2 == color);

			_colorWheelData.IndexOf(colorWheelItem);

			// Declare the return value
			byte commandValue;

			/*
			// Get the position value from the intent
			double positionValue = intent.GetValue().Value;

			// Convert the double into a 16-bit integer			
			UInt16 positionInteger = (UInt16)(positionValue * UInt16.MaxValue);
			
			

			// If this output is responsible for the high byte then...
			if (_highByte)
			{
				// Shift the value to only be the high byte
				commandValue = (byte)(positionInteger >> 8);				
			}
			// Otherise the output is responsible for the low byte
			else
			{
				// Mask the input to only include the low byte
				commandValue = (byte)(positionInteger & 0xff);
			}

			// Create a new 8-bit command with the return value
			_outputCommands.Add(new _8BitCommand(commandValue));
			*/
		}

		#endregion

		#region IDataFlowOutput

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		IDataFlowData IDataFlowOutput.Data => Data;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string Name => "Coarse-Fine Breakdown";

		#endregion

		#region IDataFlowOutput<T>

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public CommandsDataFlowData Data => _commandsData;

		#endregion
	}

}