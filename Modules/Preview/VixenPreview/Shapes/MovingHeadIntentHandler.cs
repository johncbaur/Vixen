using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;
using VixenModules.App.Fixture;
using VixenModules.Preview.VixenPreview.Fixtures;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Dispatches and handles intents for a moving head preview shape.
	/// </summary>
	public class MovingHeadIntentHandler : IntentStateDispatch,
		IHandler<IIntentState<RangeValue<FunctionIdentity>>>
	{
		#region Public Properties

		public IElementNode Node { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int PanStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int PanStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int TiltStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int TiltStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Beam length factor in percent (0-100%).
		/// </summary>
		public int BeamLength
		{
			get;			
			set;			
		}
		
		/// <summary>
		/// Moving head settings associated with the preview shape.
		/// </summary>
		public IMovingHead MovingHead { get; set; }

		//public bool ConvertColorIntentsIntoColorWheel { get; set; }

		public bool ConvertColorIntentsIntoShutter { get; set; }

		public bool ConvertColorIntentsIntoDimmer { get; set; }

		#endregion

		#region Public Methods

		public void Reset()
		{
			_legendValues.Clear();
		}

		/// <summary>
		/// Dispatches intents to a specific handler method.
		/// </summary>
		/// <param name="states">Collection of IIntentState to dispatch</param>
		public void Dispatch(IIntentStates states)
		{
			// If there are valid intents then...
			if (states != null)
			{				
				// Loop over the intents
				foreach (IIntentState state in states)
				{
					state.Dispatch(this);

					/*
					// If the intent is a positional intent then...
					if (state is IIntentState<PositionValue>)
					{
						// Handle the position intent
						Handle((IIntentState<PositionValue>)state);
					}
					// Otherwise if the intent is a lighting intent then...
					else if (state is IIntentState<LightingValue>)
					{
						// Handle the lighting intent
						Handle((IIntentState<LightingValue>)state);
					}
					else if (state is IIntentState<RGBValue>)
					{
						// Handle the lighting intent
						Handle((IIntentState<RGBValue>)state);
					}
					*/
				}
			}
		}

		/// <summary>
		/// Handles position intents.
		/// </summary>
		/// <param name="positionIntent">Position intent to handle</param>
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> rangeIntent)
		{
			// Determine which type of position type is in the intent
			switch ((FunctionIdentity)rangeIntent.GetValue().TagType)
			{
				case FunctionIdentity.Pan:
					double pan = rangeIntent.GetValue().Value * (PanStopPosition - PanStartPosition) + PanStartPosition;
					while (pan > 360.0)
					{
						pan -= 360;
					}
					while (pan < -360)
					{
						pan += 360;
					}
					// Set the moving head pan angle converting to degrees
					MovingHead.PanAngle = pan;
					
					break;
				case FunctionIdentity.Tilt:

					double tilt = rangeIntent.GetValue().Value * (TiltStopPosition - TiltStartPosition) + TiltStartPosition;
					while (tilt > 360.0)
					{
						tilt -= 360;
					}
					while (tilt < -360)
					{
						tilt += 360;
					}
					// Set the moving head tilt angle converting to degrees
					MovingHead.TiltAngle = tilt;
					
					break;
				case FunctionIdentity.Zoom:
					MovingHead.Focus = (int)(rangeIntent.GetValue().Value * 100.0);
					break;
				case FunctionIdentity.Dim:
					// Set the intensity of the beam color

					// DID NOT WORK ON X7
					//MovingHead.Intensity = (int)(rangeIntent.GetValue().Value / 255.0 * 100.0);
					MovingHead.Intensity = (int)(rangeIntent.GetValue().Value * 100.0);
					break;
				case FunctionIdentity.Custom:
					// None of the custom functions are supported by the preview
					break;
				default:
					Debug.Assert(false, "Unsupported Function Type");
					break;
			}
		}

		//public override void Handle(IIntentState<ZoomValue> zoomIntent)
		//{
		//	MovingHead.Focus = (int)(zoomIntent.GetValue().Zoom * 100.0);
		//}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			// Optionally open the shutter
			OpenShutter();

			IIntentState<DiscreteValue> discreteIntent = (IIntentState<DiscreteValue>)obj;

			DiscreteValue discreteValue = discreteIntent.GetValue();

			Color color = discreteValue.FullColor;

			// Set the beam color of the moving head
			MovingHead.BeamColor = color;

			if (ConvertColorIntentsIntoDimmer)
			{
				// Set the intensity of the beam color
				MovingHead.Intensity = (int)(discreteValue.Intensity * 100);
			}

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;

			//FixtureColorWheel colorWheelItem = _colorWheelData.FirstOrDefault(
			//	colorWheelEntry => colorWheelEntry.Color1.ToArgb() == color.ToArgb() &&
			//	!colorWheelEntry.HalfStep);
		}

		/*
		public override void Handle(IIntentState<TaggedLightingValue> lightingIntent)
		{
			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().Color;

			// Set the intensity of the beam color
			MovingHead.Intensity = 100; // (int)(lightingIntent.GetValue().Intensity * 100.0);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}
		*/

		/// <summary>
		/// Handles lighting intents.
		/// </summary>
		/// <param name="lightingIntent">Lighting intent to handle</param>
		public override void Handle(IIntentState<LightingValue> lightingIntent)
		{
			// Optionally open the shutter
			OpenShutter();

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().Color;

			// Set the intensity of the beam color
			MovingHead.Intensity = 100; // (int)(lightingIntent.GetValue().Intensity * 100.0);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		public override void Handle(IIntentState<RGBValue> lightingIntent)
		{
			// Optionally open the shutter
			OpenShutter();

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().FullColor;

			// Set the intensity of the beam color
			MovingHead.Intensity = 100;  //(int)(lightingIntent.GetValue().Intensity * 100.0);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		private void OpenShutter()
		{
			if (ConvertColorIntentsIntoShutter)
			{
				// Turn on the beam
				MovingHead.OnOff = true;
			}
		}


		Dictionary<string, string> _legendValues = new Dictionary<string, string>();
		
		public override void Handle(IIntentState<CommandValue> commandIntent)
		{
			Named8BitCommand taggedCommand = commandIntent.GetValue().Command as Named8BitCommand;

			if (taggedCommand != null &&
			    !string.IsNullOrEmpty(taggedCommand.Legend))
			{
				if (!_legendValues.ContainsKey(taggedCommand.Legend))
				{
					_legendValues.Add(taggedCommand.Legend, taggedCommand.CommandValue.ToString());
				}
				else
				{
					_legendValues[taggedCommand.Legend] = taggedCommand.CommandValue.ToString();
				}
			}

			List<string> legendItems = new List<string>();
			foreach (KeyValuePair<string, string> legendPair in _legendValues)
			{				
				legendItems.Add(legendPair.Key + legendPair.Value);
			}
			legendItems.Sort();

			MovingHead.Legend = string.Join(", " , legendItems);

			if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.LampOff ||
				((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ShutterClosed)
			{
				MovingHead.OnOff = false;
			}
			else if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.LampOn ||
				     ((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ShutterOpen)
			{
				MovingHead.OnOff = true;
			}
			else if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ColorWheel)
			{
				IntelligentFixtureModule property = (IntelligentFixtureModule)Node.Properties.SingleOrDefault(prop => prop is IntelligentFixtureModule);

				FixtureFunction colorWheelFunction = property.GetFixtureSpecification().FunctionDefinitions.Single(func => func.FunctionType == FixtureFunctionType.ColorWheel);

				FixtureColorWheel wheelEntry = colorWheelFunction.ColorWheelData.Single(clr => clr.StartValue == taggedCommand.CommandValue);

				MovingHead.BeamColor = wheelEntry.Color1;
			}

			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		#endregion
	}
}
