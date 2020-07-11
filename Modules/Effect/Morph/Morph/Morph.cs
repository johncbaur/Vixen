﻿using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Line = VixenModules.App.Polygon.Line;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Morph pixel effect.
	/// </summary>
	public class Morph : PixelEffectBase
	{
		#region Private Fields

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private MorphData _data;

		/// <summary>
		/// Logical buffer height.
		/// Note this height might not match the actual effect height when the effect is operating in Location mode.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Logical buffer height.
		/// Note this width might not match the actual effect width when the effect is operating in Location mode.
		/// </summary>
		private int _bufferWi;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to free form mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _freeFormEditorCapabilities;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to the time based mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _timeBasedEditorCapabilities;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to the pattern mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _patternEditorCapabilities;

		/// <summary>
		/// Collection of data needed to render the wipe polygons.
		/// </summary>
		private readonly List<MorphWipePolygonRenderData> _wipePolygonRenderData;

		/// <summary>
		/// Collection of morph polygons that were created by applying the repeating pattern.
		/// </summary>
		private List<IMorphPolygon> _patternExpandedMorphPolygons;

		/// <summary>
		/// Used in <c>CalculatePixel</c> to transfer pixels from bitmap to frame buffer.
		/// </summary>
		private static Color _emptyColor = Color.FromArgb(0, 0, 0, 0);

		/// <summary>
		/// Stagger used to draw repeating wipe patterns.
		/// </summary>
		private int _stagger;

		#endregion

		#region Private Constants

		/// <summary>
		/// Maximum acceleration of the wipe.
		/// </summary>
		private const double MaxAcceleration = 10.0;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Morph()
		{
			// Enable both string and location positioning
			EnableTargetPositioning(true, true);

			// Initialize the collection of morph polygons
			_morphPolygons = new MorphPolygonsObservableCollection();

			// Initialize the render data for patterned polygons			
			_wipePolygonRenderData = new List<MorphWipePolygonRenderData>();

			// Initialize the capabilities of the pattern mode polygon editor
			_patternEditorCapabilities = new PolygonEditorCapabilities()
			{
				DeletePolygons = false,
				DeletePoints = false,
				AddPolygons = false,				
				PastePolygons = false,
				CutPolygons = false,
				DefaultToSelect = true,
				ToggleStartSide = true,
				ToggleStartPoint = true,
				CopyPolygons = false,
				ShowStartSide = true,
				ShowTimeBar = false,
			};

			// Initialize the capabilities of the free form mode polygon editor
			_freeFormEditorCapabilities = new PolygonEditorCapabilities
			{
				DeletePolygons = true,
				DeletePoints = true,
				AddPolygons = true,				
				PastePolygons = true,
				CutPolygons = true,
				DefaultToSelect = true,
				ToggleStartSide = true,
				ToggleStartPoint = true,
				CopyPolygons = true,
				ShowStartSide = true,
				ShowTimeBar = false,
				AddPoint = true,
			};

			// Initialize the capabilities of the time based mode polygon editor
			_timeBasedEditorCapabilities = new PolygonEditorCapabilities()
			{
				DeletePolygons = true,
				DeletePoints = true,
				AddPolygons = true,				
				PastePolygons = true,
				CutPolygons = true,
				DefaultToSelect = false,
				ToggleStartSide = false,
				ToggleStartPoint = false,
				CopyPolygons = false,
				ShowStartSide = false,
				ShowTimeBar = true,
				AddPoint = true,
			};			
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// String orientation of the effect.
		/// </summary>
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();

				// Make sure the polygons/lines still fit on the display area				
				LimitShapes();
			}
		}

		/// <summary>
		/// Module data associated with the effect.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Update the serialized data 
				UpdateMorphSerializedData();

				// Return the effect data
				return _data;
			}
			set
			{
				// Save off the data for the effect
				_data = value as MorphData;

				// Update the morph polygon model data
				UpdatePolygonModel(_data);

				// Update the visibility of controls
				UpdateAttributes();

				// Mark the effect as dirty
				MarkDirty();
			}
		}

		#endregion

		#region Information Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/Morph/"; }
		}

		#endregion

		#region Polygon Type Configuration Properties

		/// <summary>
		/// Determines the type of polygon or line.  This setting controls the three modes of operation for the effect.
		/// </summary>
		[ProviderCategory(@"PolygonConfiguration", 2)]
		[ProviderDisplayName(@"PolygonType")]
		[ProviderDescription(@"PolygonType")]
		[PropertyOrder(1)]
		public PolygonType PolygonType
		{
			get { return _data.PolygonType; }
			set
			{
				_data.PolygonType = value;

				if (_data.PolygonType == PolygonType.FreeForm)
				{
					RepeatCount = 0;
				}

				// Pattern mode only allows one polygon to be drawn by the user
				if (_data.PolygonType == PolygonType.Pattern)
				{					
					while (MorphPolygons.Count > 1)
					{
						MorphPolygons.Remove(MorphPolygons[MorphPolygons.Count - 1]);
					}					
				}

				IsDirty = true;
				OnPropertyChanged();

				UpdatePolygonTypeAttributes(true);
			}
		}

		/// <summary>
		/// Determines how the polygon/line is filled (Wipe, Solid, or Outline).
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 3)]
		[ProviderDisplayName(@"FillType")]
		[ProviderDescription(@"FillType")]
		[PropertyOrder(2)]
		public PolygonFillType FillType
		{
			get
			{
				return _data.PolygonFillType;
			}
			set
			{
				_data.PolygonFillType = value;
				IsDirty = true;

				// Update the fill type on each of the morph polygons
				foreach(IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.FillType = value;
				}

				OnPropertyChanged();
				UpdatePolygonFillTypeAttributes();
			}
		}
		
		/// <summary>
		/// This property is used as the input to the Polygon Editor.  This property type is associated to the polygon editor.		
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 2)]
		[ProviderDisplayName(@" ")]
		[ProviderDescription(@"EditPolygons")]
		[PropertyOrder(3)]
		public PolygonContainer PolygonContainer
		{
			get
			{
				PolygonContainer container = new PolygonContainer();

				switch (PolygonType)
				{
					case PolygonType.Pattern:
						container.EditorCapabilities = _patternEditorCapabilities;
						break;
					case PolygonType.FreeForm:
						container.EditorCapabilities = _freeFormEditorCapabilities;
						break;
					case PolygonType.TimeBased:
						container.EditorCapabilities = _timeBasedEditorCapabilities;
						break;
					default:
						Debug.Assert(false, "Unsupported Polygon Type");
						break;
				}
				
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					if (morphPolygon.Polygon != null)
					{
						Polygon clone = morphPolygon.Polygon.Clone();

						// Reset the GUID as we want to maintain identity of the cloned polygon
						clone.ID = morphPolygon.Polygon.ID;
						container.Polygons.Add(clone);
						container.PolygonTimes.Add(morphPolygon.Time);
					}
					else
					{
						Line clone = morphPolygon.Line.Clone();

						// Reset the GUID as we want to maintain identity of the cloned line
						clone.ID = morphPolygon.Line.ID;
						container.Lines.Add(clone);
						container.LineTimes.Add(morphPolygon.Time);						
					}
				}

				// Give the container the dimensions of the display element
				container.Height = BufferHt;
				container.Width = BufferWi;

				return container;
			}
			set
			{
				// Default all the polygons and lines to have been removed in the editor
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.Removed = true;
				}

				UpdateMorphPolygonsFromContainerPolygons(value);
				UpdateMorphPolygonsFromContainerLines(value);

				// Remove any morph polygons were the corresponding polygon or line was removed in the polygon editor
				foreach (IMorphPolygon morphPolygon in MorphPolygons.ToList())
				{
					if (morphPolygon.Removed)
					{
						MorphPolygons.Remove(morphPolygon);
					}
				}
				
				OnPropertyChanged(nameof(MorphPolygons));

				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Pattern Configuration Properties

		/// <summary>
		/// Determines  how many times to repeat the polygon/line.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatCount")]
		[ProviderDescription(@"RepeatCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 50, 1)]
		[PropertyOrder(2)]		
		public int RepeatCount
		{
			get
			{
				return _data.RepeatCount;
			}
			set
			{
				_data.RepeatCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the direction of the repeating pattern of polygons or lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatDirection")]
		[ProviderDescription(@"RepeatDirection")]
		[PropertyOrder(3)]		
		public WipeRepeatDirection RepeatDirection
		{
			get
			{
				return _data.RepeatDirection;
			}
			set
			{
				_data.RepeatDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the spacing in-between the repeating polygons/lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatSkip")]
		[ProviderDescription(@"RepeatSkip")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(4)]
		public int RepeatSkip
		{
			get
			{
				return _data.RepeatSkip;
			}
			set
			{
				_data.RepeatSkip = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines if the start of the wipe is staggered for the repeating polygons/lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"Stagger")]
		[ProviderDescription(@"Stagger")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(5)]		
		public int Stagger
		{
			get
			{
				return _data.Stagger;
			}
			set
			{
				_data.Stagger = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Wipe Configuration Properties

		/// <summary>
		/// Determines the length of the wipe head.
		/// </summary>
		[Value]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadLength")]
		[ProviderDescription(@"HeadLength")]
		[PropertyOrder(1)]		
		public int HeadLength
		{
			get
			{
				return _data.HeadLength;
			}
			set
			{
				_data.HeadLength = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the percentage of the effect it takes the wipe head to travel across the polygon/line.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadDuration")]
		[ProviderDescription(@"HeadDuration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(2)]		
		public int HeadDuration
		{
			get
			{
				return _data.HeadDuration;
			}
			set
			{
				_data.HeadDuration = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the wipe head color over the duration of the effect.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadColor")]
		[ProviderDescription(@"HeadColor")]
		[PropertyOrder(5)]
		public ColorGradient HeadColor
		{
			get
			{
				return _data.HeadColor;
			}
			set
			{
				_data.HeadColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the wipe tail color over the duration of the effect.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"TailColor")]
		[ProviderDescription(@"TailColor")]
		[PropertyOrder(7)]
		public ColorGradient TailColor
		{
			get
			{
				return _data.TailColor;
			}
			set
			{
				_data.TailColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the acceleration of the polygon/line wipe.  The acceleration can be either increasing or decreasing (deceleration).
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"Acceleration")]
		[ProviderDescription(@"Acceleration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-10, 10, 1)]
		[PropertyOrder(2)]		
		public int Acceleration
		{
			get
			{
				return _data.Acceleration;
			}
			set
			{
				_data.Acceleration = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the color of the polygon or line.  This setting is only applicable to Time Based mode.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"PolygonColor")]
		[ProviderDescription(@"PolygonColor")]
		[PropertyOrder(6)]
		public ColorGradient FillColor
		{
			get
			{
				return _data.FillColor;
			}
			set
			{
				_data.FillColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines if the Fill Type of the polygon (Solid or Outline).
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"FillPolygon")]
		[ProviderDescription(@"FillPolygon")]
		[PropertyOrder(7)]
		public bool FillPolygon
		{
			get
			{
				return _data.FillPolygon;
			}
			set
			{
				_data.FillPolygon = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Configuration Properties

		private MorphPolygonsObservableCollection _morphPolygons;

		/// <summary>
		/// Gets and sets the Morph polygons.  This property is visible in the
		/// effect editor when the mode is set to free form but is used in the background for the other modes.
		/// </summary>
		[ProviderCategory(@"Config", 6)]
		[ProviderDisplayName(@"Polygons")]
		[ProviderDescription(@"Polygons")]
		[PropertyOrder(2)]
		public MorphPolygonsObservableCollection MorphPolygons
		{
			get
			{
				return _morphPolygons;
			}
			set
			{
				_morphPolygons = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Handles resizing polygons/lines when the display element changes.
		/// </summary>
		protected override void TargetNodesChanged()
		{
			// Call the base class implementation
			base.TargetNodesChanged();

			// Configure the display element size (strings vs position)
			ConfigureDisplayElementSize();
			
			// If there are not any polygons or lines then...
			if (MorphPolygons.Count == 0)
			{
				// Default to a polygon that fills the display element
				IMorphPolygon morphPolygon = new MorphPolygon();
				MorphPolygons.Add(morphPolygon);

				morphPolygon.Polygon = new Polygon();

				PolygonPoint ptTopLeft = new PolygonPoint();
				ptTopLeft.X = 0;
				ptTopLeft.Y = 0;
				PolygonPoint ptTopRight = new PolygonPoint();
				ptTopRight.X = BufferWi - 1;
				ptTopRight.Y = 0;
				PolygonPoint ptBottomRight = new PolygonPoint();
				ptBottomRight.X = BufferWi - 1;
				ptBottomRight.Y = BufferHt - 1;
				PolygonPoint ptBottomLeft = new PolygonPoint();
				ptBottomLeft.X = 0;
				ptBottomLeft.Y = BufferHt - 1;

				morphPolygon.Polygon.Points.Add(ptTopLeft);
				morphPolygon.Polygon.Points.Add(ptTopRight);
				morphPolygon.Polygon.Points.Add(ptBottomRight);
				morphPolygon.Polygon.Points.Add(ptBottomLeft);
			}

			// Make sure all polygons/lines fit on the display element
			LimitShapes();			
		}

		/// <summary>
		/// Gets the data associated with the effect.
		/// </summary>
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				UpdateMorphSerializedData();
				return _data;
			}
		}

		/// <summary>
		/// Releases resources from the rendering process.
		/// </summary>
		protected override void CleanUpRender()
		{
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Make a local copy that is faster than the logic to get it for reuse.
			int localBufferHt = BufferHt;

			// Create a virtual matrix based on the rendering scale factor
			PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				//Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Render the effet to the virtual frame buffer
				RenderEffect(frameNum, virtualFrameBuffer, frameBuffer.ElementLocations, frameBuffer);

				// If the polygon type is NOT time based then...				
				if (PolygonType != PolygonType.TimeBased)
				{
					// Loop through the sparse matrix
					foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
					{
						// Lookup the pixel from the virtual frame buffer
						UpdateFrameBufferForLocationPixel(
							elementLocation.X,
							elementLocation.Y,
							localBufferHt,
							virtualFrameBuffer,
							frameBuffer);
					}
				}

				// Clear the buffer for the next frame,
				virtualFrameBuffer.ClearBuffer();
			}
		}

		/// <summary>
		/// Renders the effect for string based display elements.
		/// </summary>		
		protected override void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer)
		{
			// Call a common method used for both string and location rendering
			RenderEffect(frameNum, frameBuffer, null, null);
		}

		/// <summary>
		/// Setup for rendering.
		/// </summary>
		protected override void SetupRender()
		{
			if (PolygonType == PolygonType.Pattern)
			{
				// Transfer the top level settings to the morph polygon
				MorphPolygons[0].HeadLength = HeadLength;
				MorphPolygons[0].HeadDuration = HeadDuration;
				MorphPolygons[0].Acceleration = Acceleration;
				MorphPolygons[0].HeadColor = new ColorGradient(HeadColor);
				MorphPolygons[0].TailColor = new ColorGradient(TailColor);
			}

			// Store off the matrix width and height
			_bufferWi = BufferWi;
			_bufferHt = BufferHt;
						
			// Clear the wipe polygon render data
			_wipePolygonRenderData.Clear();

			// Setup the pattern wipe polygons
			SetupRenderPattern();

			// Discard the expanded morph polygons
			_patternExpandedMorphPolygons = null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Setup for render pattern polygons.
		/// </summary>
		private void SetupRenderPattern()
		{
			// Get the polygon configured to be wipe polygons
			List<IMorphPolygon> wipePolygons = GetWipePolygons();

			// Loop over the wipe polygons
			for (int polygonIndex = 0; polygonIndex < wipePolygons.Count; polygonIndex++)
			{
				// Get the specified morph polygon
				IMorphPolygon morphPolygon = wipePolygons[polygonIndex];

				// Create a new instance of wipe render data
				_wipePolygonRenderData.Add(new MorphWipePolygonRenderData());

				// Get the model polygon from the morph polygon
				Polygon polygonModel = morphPolygon.Polygon;

				// If the MorphPolygon is a line then...
				if (polygonModel == null)
				{
					// Create a new polygon model
					polygonModel = new Polygon();

					// Convert the line into a skinny (line) polygon
					PolygonPoint pt1 = morphPolygon.Line.Points[0];
					PolygonPoint pt2 = morphPolygon.Line.Points[0];
					PolygonPoint pt3 = morphPolygon.Line.Points[1];
					PolygonPoint pt4 = morphPolygon.Line.Points[1];

					// Add the points to the polygons model
					polygonModel.Points.Add(pt1);
					polygonModel.Points.Add(pt2);
					polygonModel.Points.Add(pt3);
					polygonModel.Points.Add(pt4);
				}
				
				// Calculate the points along the side of the polygon
				// Note this is neither the start or end side
				StoreLine(
					(int)(polygonModel.Points[1].X),
					BufferHt - (int)(polygonModel.Points[1].Y),
					(int)(polygonModel.Points[2].X),
					BufferHt - (int)(polygonModel.Points[2].Y), _wipePolygonRenderData[polygonIndex].X1Points, _wipePolygonRenderData[polygonIndex].Y1Points);

				// Calculate the points along the other side of the polygon
				StoreLine(
					(int)(polygonModel.Points[0].X),
					BufferHt - (int)(polygonModel.Points[0].Y),
					(int)(polygonModel.Points[3].X),
					BufferHt - (int)(polygonModel.Points[3].Y), _wipePolygonRenderData[polygonIndex].X2Points, _wipePolygonRenderData[polygonIndex].Y2Points);

				// Calculate the direction of the polygon
				_wipePolygonRenderData[polygonIndex].Direction = CalculateDirection(polygonModel);

				// Determine the length of the long side of the polygon				
				int length = GetLengthOfWipePolygon(_wipePolygonRenderData[polygonIndex]);

				// Make a copy of the stagger spacing
				_stagger = Stagger;

				// Adjust the stagger so that it does not exceed the total duration of the effect
				if (TimeSpan.TotalMilliseconds * (_stagger / 100.0) * RepeatCount > TimeSpan.TotalMilliseconds)
				{
					_stagger = (int)(((TimeSpan.TotalMilliseconds / RepeatCount) / TimeSpan.TotalMilliseconds) * 100);
				}

				// Calculate the time each polygon has to perform the wipe
				// Subtracting off all the staggers
				double time = TimeSpan.TotalMilliseconds - TimeSpan.TotalMilliseconds * (_stagger / 100.0) * RepeatCount;

				// Need the head to travel past the end of the polygon/line for the length of the head
				int polygonLengthForHead = length + morphPolygon.HeadLength;

				// If the user selected an acceleration or de-acceleration then...
				if (morphPolygon.Acceleration != 0)
				{
					// If the acceleration is negative then...
					if (morphPolygon.Acceleration < 0)
					{
						// Calculate the head and tail accelerations using special iterative methods
						_wipePolygonRenderData[polygonIndex].HeadAcceleration = GetNegativeAcceleration(polygonLengthForHead, time * (morphPolygon.HeadDuration / 100.0), morphPolygon) * Math.Abs(morphPolygon.Acceleration / MaxAcceleration);
						_wipePolygonRenderData[polygonIndex].TailAcceleration = GetNegativeAcceleration(length, time, morphPolygon) * Math.Abs(morphPolygon.Acceleration / MaxAcceleration);
					}
					else				
					{
						// Otherwise calculate the positive accelerations
						_wipePolygonRenderData[polygonIndex].HeadAcceleration = GetPositiveAcceleration(length, time * (morphPolygon.HeadDuration / 100.0)) * morphPolygon.Acceleration / MaxAcceleration;
						_wipePolygonRenderData[polygonIndex].TailAcceleration = GetPositiveAcceleration(length, time) * morphPolygon.Acceleration / MaxAcceleration;
					}
				}
				else
				{
					// Otherwise set the accelerations to zero
					_wipePolygonRenderData[polygonIndex].HeadAcceleration = 0;
					_wipePolygonRenderData[polygonIndex].TailAcceleration = 0;
				}

				// Calculate the head and tail initial velocities of the wipe
				_wipePolygonRenderData[polygonIndex].HeadVelocityZero = GetHeadVelocityZero(polygonLengthForHead, _wipePolygonRenderData[polygonIndex].HeadAcceleration, time, morphPolygon);
				_wipePolygonRenderData[polygonIndex].TailVelocityZero = GetIncreasingVelocityZero(_wipePolygonRenderData[polygonIndex].TailAcceleration, length, time);
			}
		}

		/// <summary>
		/// Calculates the direction of the specified polygon model.  Direction is used to assist in drawing the polygon lines.
		/// </summary>		
		private bool CalculateDirection(Polygon polygonModel)
		{
			int x1A = (int)(polygonModel.Points[1].X);
			int y1A = BufferHt - (int)(polygonModel.Points[1].Y);

			int x1B = (int)(polygonModel.Points[0].X);
			int y1B = BufferHt - (int)(polygonModel.Points[0].Y);

			int x2A = (int)(polygonModel.Points[2].X);
			int y2A = BufferHt - (int)(polygonModel.Points[2].Y);

			int x2B = (int)(polygonModel.Points[3].X);
			int y2B = BufferHt - (int)(polygonModel.Points[3].Y);

			int deltaXa = x2A - x1A;
			int deltaXb = x2B - x1B;
			int deltaYa = y2A - y1A;
			int deltaYb = y2B - y1B;
			int direction = deltaXa + deltaXb + deltaYa + deltaYb;

			return (direction >= 0);
		}

		/// <summary>
		/// Updates the morph polygons from the specified polygon container polygons.
		/// </summary>		
		private void UpdateMorphPolygonsFromContainerLines(PolygonContainer container)
		{
			// Loop over the lines coming out of the polygon editor							
			for (int lineIndex = 0; lineIndex < container.Lines.Count; lineIndex++)
			{
				// Get the specified line
				Line line = container.Lines[lineIndex];

				// If the line already existed in the morph collection then...
				if (MorphPolygons.Any(poly => poly.Line != null && poly.Line.ID == line.ID))
				{
					// Find the line in the morph collection by GUID
					IMorphPolygon morphPolygon = MorphPolygons.Single(poly => poly.Line != null && poly.Line.ID == line.ID);

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Update the line model associated with morph polygon
					morphPolygon.Line = line;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.LineTimes[lineIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}
				}
				// Else the line was not found...
				else
				{
					IMorphPolygon morphPolygon = new MorphPolygon();
					morphPolygon.Line = line;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.LineTimes[lineIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Add the polygon to collection
					MorphPolygons.Add(morphPolygon);
				}
			}
		}

		/// <summary>
		/// Updates the morph polygons from the specified polygon container lines.
		/// </summary>						
		private void UpdateMorphPolygonsFromContainerPolygons(PolygonContainer container)
		{
			// Loop over the polygons coming out of the polygon editor
			for (int polygonIndex = 0; polygonIndex < container.Polygons.Count; polygonIndex++)
			{
				// Get the specified polygon
				Polygon polygon = container.Polygons[polygonIndex];

				// If the polygon already existed in the morph collection then...
				if (MorphPolygons.Any(poly => poly.Polygon != null && poly.Polygon.ID == polygon.ID))
				{
					// Find the polygon in the morph collection by GUID
					IMorphPolygon morphPolygon = MorphPolygons.Single(poly => poly.Polygon != null && poly.Polygon.ID == polygon.ID);

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Update the polygon model associated with morph polygon
					morphPolygon.Polygon = polygon;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.PolygonTimes[polygonIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}
				}
				// Else the polygon was not found...
				else
				{
					// Create a new morph polygon
					IMorphPolygon morphPolygon = new MorphPolygon();
					morphPolygon.Polygon = polygon;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.PolygonTimes[polygonIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Add the polygon to collection
					MorphPolygons.Add(morphPolygon);
				}
			}
		}

		/// <summary>
		/// Limits the shapes so that they fit on the display element.
		/// </summary>
		private void LimitShapes()
		{			
			// Loop over all the morph polygon shapes
			foreach (IMorphPolygon morphPolygon in MorphPolygons)
			{
				// Limit the points on the shape
				morphPolygon.LimitPoints(BufferWi, BufferHt);				
			}
		}
						
		/// <summary>
		/// Updates the frame buffer for a location based pixel.
		/// </summary>
		private void UpdateFrameBufferForLocationPixel(int x, int y, int bufferHt, IPixelFrameBuffer tempFrameBuffer, IPixelFrameBuffer frameBuffer)
		{
			// Save off the original location node
			int yCoord = y;
			int xCoord = x;

			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
			y = y - BufferHtOffset;
			x = x - BufferWiOffset;

			// Retrieve the color from the bitmap
			Color color = tempFrameBuffer.GetColorAt(x, y);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Calculates velocity at time zero for the specified length and effect duration (time).
		/// </summary>
		/// <remarks>
		/// This method was based on the math at the following URL:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		/// </remarks>
		private double GetIncreasingVelocityZero(double acceleration, double length, double time)
		{
			double t = time;
			double i = acceleration;

			double velocityZero = (length - 1.0 / 6.0 * i * t * t * t) / t;

			return velocityZero;
		}

		/// <summary>
		/// Calculates the position of the wipe head the specified time and startig velocity.
		/// </summary>
		/// <remarks>
		/// This method was based on the math at the following URL:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		/// </remarks>
		double GetIncreasingHeadPosition(double acceleration, double time, double velocityZero)
		{
			double t = time;
			double i = acceleration;
			double v = velocityZero;

			double x = 1.0 / 6.0 * (i * t * t * t) + (v * t); 

			return x;
		}

		/// <summary>
		/// Draws a line between the specified two points.
		/// </summary>		
		private void DrawThickLine(
			// ReSharper disable once InconsistentNaming
			int x0_,
			// ReSharper disable once InconsistentNaming
			int y0_,
			// ReSharper disable once InconsistentNaming
			int x1_,
			// ReSharper disable once InconsistentNaming
			int y1_, 
			Color color, bool direction, IPixelFrameBuffer frameBuffer)
		{
			int x0 = x0_;
			int x1 = x1_;
			int y0 = y0_;
			int y1 = y1_;
			int lastx = x0;
			int lasty = y0;

			int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
			int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
			int err = (dx > dy ? dx : -dy) / 2, e2;

			for(;;)
			{			
				frameBuffer.SetPixel(x0, y0, color);				
				if((x0 != lastx) && (y0 != lasty) && (x0_ != x1_) && (y0_ != y1_) )
				{
					int fix = 0;
					if(x0 > lastx ) fix += 1;
					if(y0 > lasty ) fix += 2;
					if(direction  ) fix += 4;
					switch (fix)
					{
					case 2:
					case 4:
						if(x0<BufferWi -2) frameBuffer.SetPixel(x0+1,y0, color);
						break;
					case 3:
					case 5:
						if(x0 > 0) frameBuffer.SetPixel(x0-1, y0, color);
						break;
					case 0:
					case 1:
						if(y0<BufferHt -2) frameBuffer.SetPixel(x0, y0+1, color);
						break;
					case 6:
					case 7:
						if(y0 > 0) frameBuffer.SetPixel(x0, y0-1, color);
						break;
					}
				}
				lastx = x0;
				lasty = y0;
				if (x0==x1 && y0==y1) break;
				e2 = err;
				if (e2 >-dx) { err -= dy; x0 += sx; }
				if (e2<dy) { err += dx; y0 += sy; }
			}
		}
		
		/// <summary>
		/// Calculates the points along the specified line and stores them in the specified collections.
		/// </summary>		
		private void StoreLine(
			// ReSharper disable once InconsistentNaming
			int x0_,
			// ReSharper disable once InconsistentNaming
			int y0_,
			// ReSharper disable once InconsistentNaming
			int x1_,
			// ReSharper disable once InconsistentNaming
			int y1_, 
			List<int> vx, List<int> vy)
		{
			int x0 = x0_;
			int x1 = x1_;
			int y0 = y0_;
			int y1 = y1_;

			int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
			int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
			int err = (dx > dy ? dx : -dy) / 2, e2;

			for(;;)
			{
				vx.Add(x0);
				vy.Add(y0);
				if (x0==x1 && y0==y1) break;
				e2 = err;
				if (e2 >-dx) { err -= dy; x0 += sx; }
				if (e2<dy) { err += dx; y0 += sy; }
			}
		}

		/// <summary>
		/// Calculates the smallest positive acceleration for the specified morph polygon
		/// that allows the wipe to make it to the end of the polygon/line with a zero starting velocity.
		/// </summary>		
		/// <remarks>
		/// If the acceleration gets any larger the velocity zero starts negative.
		/// This method finds the largest acceleration where the velocity zero is positive.
		/// This value is then scaled based on the user's selection for acceleration.
		/// </remarks>
		private double GetPositiveAcceleration(int length, double time)
		{
			return length / (time * time * time) * 6.0;						
		}

		/// <summary>
		/// Gets the head duration as a percentage.
		/// </summary>		
		private double GetHeadDurationPercentage(IMorphPolygon morphPolygon)
		{
			return morphPolygon.HeadDuration / 100.0; 
		}

		/// <summary>
		/// Refines the acceleration using the specified good and bad accelerations.  Using a binary search approach.
		/// </summary>
		/// <param name="validAcceleration">Valid acceleration</param>
		/// <param name="invalidAcceleration">Invalid acceleration</param>
		/// <param name="length">Length of the polygon/line</param>
		/// <param name="time">Time of the wipe</param>
		/// <returns>Refined acceleration</returns>
		private double RefineAcceleration(double invalidAcceleration, double validAcceleration, int length, double time)
		{
			const int MaxIterations = 10000;
			int iterations = 0;

			// Loop until the acceleration converge or we have iterated for the maximum number of iterations
			while (invalidAcceleration != validAcceleration && iterations < MaxIterations)
			{
				// Try an acceleration between the two accelerations
				double testAcceleration = (invalidAcceleration + validAcceleration) / 2.0;

				// If the middle acceleration is valid then...
				if (IsIncreasingAccelerationValid(testAcceleration, length, time))
				{
					// Swap out the valid acceleration
					validAcceleration = testAcceleration;
				}
				else
				{
					// Swap out the invalid acceleration
					invalidAcceleration = testAcceleration;
				}

				iterations++;
			}

			// Return the refined acceleration
			return validAcceleration;
		}
		

		/// <summary>
		/// Calculates the tail acceleration when the acceleration is negative.
		/// </summary>
		/// <param name="length">Length of the polygon/line</param>
		/// <param name="time">Time for the wipe</param>
		/// <param name="morphPolygon">Morph polygon associated with the wipe</param>
		/// <returns>Acceleration of the wipe</returns>
		private double GetNegativeAcceleration(int length, double time, IMorphPolygon morphPolygon)
		{
			// Default the acceleration to the morph polygon setting to something large
			double tailAcceleration = morphPolygon.Acceleration;
			
			// Keep track of the previous acceleration
			double previousAcceleration = 0.0;

			// Loop until we find a valid acceleration
			while (!IsIncreasingAccelerationValid(tailAcceleration, length, time))
			{
				// Keep reducing the acceleration by half until we find a valid acceleration
				previousAcceleration = tailAcceleration;
				tailAcceleration = tailAcceleration / 2.0;
			}

			// Now we have a valid acceleration and an invalid acceleration
			// Use these two values to refine the acceleration
			return RefineAcceleration(previousAcceleration, tailAcceleration, length, time);
		}

		/// <summary>
		/// Returns true if the specified acceleration is valid for the duration of the effect.
		/// </summary>
		/// <param name="acceleration">Acceleration to test</param>
		/// <param name="length">Length of the wipe</param>
		/// <param name="time">Duration of the wipe in milliseconds</param>
		/// <returns>Whether the acceleration is valid</returns>
		/// <remarks>
		/// Some of the math used in this method was derived from:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		///</remarks>
		private bool IsIncreasingAccelerationValid(double acceleration, double length, double time)
		{
			// Store off the time of the wipe
			double t = time;

			// Initialize the acceleration
			double i = acceleration;

			// Calculate initial velocity of the wipe
			double v = (length - 1.0 / 6.0 * i * t * t * t) / t;

			// Default the acceleration to valid
			bool valid = true;

			// Keep track of the previous X position
			double xPrevious = 0;

			// Iterate over all the frames
			for (double ti = 0; ti < t; ti += FrameTime)
			{
				// Calculate the position of X
				double x = GetIncreasingHeadPosition(i, ti, v);
				
				// If X starts backing up at any point then...
				if (x < xPrevious)
				{
					// Indicate the acceleration is invalid
					valid = false;
					break;
				}

				// Update the previous X value
				xPrevious = x;
			}

			// Return whether the starting velocity was positive and
			// that the wipe never starts going backwards
			return (v > 0.0) && valid;
		}
		
		/// <summary>
		/// Gets the head velocity zero for the specified morph polygon.
		/// </summary>		
		private double GetHeadVelocityZero(double length, double headAcceleration, double time, IMorphPolygon morphPolygon)
		{
			// Get the percentage of the effect that it should take the head to travel across the polygon
			double headDuration = GetHeadDurationPercentage(morphPolygon);			

			// Calculate the velocity zero of the head
			return GetIncreasingVelocityZero(headAcceleration, length, time * headDuration);
		}
		
		/// <summary>
		/// Renders time based polygons.
		/// </summary>		
		private void RenderEffectTimeBased(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos, IEnumerable<ElementLocation> sparseMatrix, PixelLocationFrameBuffer locationFrameBuffer)
		{			
			// If there is at least one morph polygon then...
			if (MorphPolygons.Count > 0)
			{
				// If the morph polygon contains a polygon then...
				if (MorphPolygons[0].Polygon != null)
				{
					// The first polygon determine the number of polygon points for all polygon in the effect
					int numberOfPoints = MorphPolygons[0].Polygon.Points.Count;
					RenderEffectTimeBasedPolygon(frameNum, frameBuffer, intervalPos, numberOfPoints, sparseMatrix, locationFrameBuffer);
				}
				// Otherwise if the morph polygon contains a line then...
				else if (MorphPolygons[0].Line != null)
				{
					RenderEffectTimeBasedLine(frameNum, frameBuffer, intervalPos, sparseMatrix, locationFrameBuffer);
				}
			}
		}

		/// <summary>
		/// Find the two applicable morph polygons frame snapshots based on the current position in the effect timeline.
		/// </summary>		
		private int FindTwoPolygonsOnTimeline(
			double time,
			List<IMorphPolygon> morpPolygons)			
		{
			int polygonIndex = -1;

			// Calculate how are we are into the effect as a percentage
			double timeFraction = time / TimeSpan.TotalMilliseconds;

			// Loop over the polygons and find the two polygons we are between based on the timeline
			for (int index = morpPolygons.Count - 1; index >= 1; index--)
			{
				IMorphPolygon firstPolygon = MorphPolygons[index - 1];
				IMorphPolygon secondPolygon = MorphPolygons[index];

				if (firstPolygon.Time < timeFraction && timeFraction <= secondPolygon.Time)
				{
					polygonIndex = index - 1;
					break;
				}
			}

			return polygonIndex;
		}

		/// <summary>
		///  Renders the time based polygon.
		/// </summary>		
		private void RenderEffectTimeBasedPolygon(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			int numberOfPoints, 
			IEnumerable<ElementLocation> sparseMatrix,
			PixelLocationFrameBuffer locationFrameBuffer)
		{
			// Get the morph polygons that have the same number of polygon points as the first polygon
			// All other polygons are going to be ignored
			List<IMorphPolygon> morpPolygons = MorphPolygons.Where(morphPolygon => morphPolygon.Polygon.Points.Count == numberOfPoints).ToList();

			// Render the time based morph polygons
			RenderEffectTimeBasedMorpPolygon(frameNum, morpPolygons, frameBuffer, intervalPos, sparseMatrix, locationFrameBuffer);			
		}

		/// <summary>
		/// Renders the time based line.
		/// </summary>		
		private void RenderEffectTimeBasedLine(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			IEnumerable<ElementLocation> sparseMatrix,
			PixelLocationFrameBuffer locationFrameBuffer)
		{
			// Get the morph polygons that contain lines
			// All other polygons are ignored
			List<IMorphPolygon> morphLines = MorphPolygons.Where(morphPolygon => morphPolygon.Line != null).ToList();

			// Render the time based morph polygons (lines)
			RenderEffectTimeBasedMorpPolygon(frameNum, morphLines, frameBuffer, intervalPos, sparseMatrix, locationFrameBuffer);
		}

		/// <summary>
		/// Renders the collection of morph polygons.
		/// </summary>		
		private void RenderEffectTimeBasedMorpPolygon(
			int frameNum, 
			List<IMorphPolygon> morpPolygons, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos,
			IEnumerable<ElementLocation> sparseMatrix,
			PixelLocationFrameBuffer locationFrameBuffer)
		{
			// Calculate how far into effect we are
			double time = frameNum * FrameTime;

			// Find the index of the two polygons based on where we are on the timelime of the effect
			// The polygonIndex points to the first polygon			
			int polygonIndex = FindTwoPolygonsOnTimeline(time, morpPolygons);

			// If two polygons were found then...
			if (polygonIndex < MorphPolygons.Count - 1 && polygonIndex != -1)
			{
				// Collection of points for the polygon that is moving between the two frame snapshots
				List<Point> points = new List<Point>();

				// Loop over the polygon points
				for (int ptIndex = 0; ptIndex < MorphPolygons[polygonIndex].GetPolygonPoints().Count; ptIndex++)
				{
					// Get the start polygon and the end polygon
					IMorphPolygon startPolygon = MorphPolygons[polygonIndex];
					IMorphPolygon endPolygon = MorphPolygons[polygonIndex + 1];

					// Add the point to the intermediate snapshot
					points.Add(CalculateItermediatePoint(time, startPolygon.GetPolygonPoints()[ptIndex], endPolygon.GetPolygonPoints()[ptIndex], startPolygon.Time, endPolygon.Time));
				}

				// Render the intermediate polygon
				RenderStaticPolygon(frameBuffer, intervalPos, points, sparseMatrix, locationFrameBuffer);
			}
		}

		/// <summary>
		/// Calculate a point on the intermediate snapshot polygon/line.
		/// </summary>		
		private Point CalculateItermediatePoint(double time, PolygonPoint startPoint, PolygonPoint endPoint, double startTime, double endTime)
		{
			// Calculate the total time between the two polygons
			double totalTimeOnLine = (endTime - startTime) * TimeSpan.TotalMilliseconds;

			// Calculate the distance in the x-axis between the two points
			double distanceX = endPoint.X - startPoint.X;

			// calculate the velocity in the x-axis required to move between the two points
			double velocityX = distanceX / totalTimeOnLine;

			// Calculate the distance in the y-axis between the two points
			double distanceY = endPoint.Y - startPoint.Y;

			// Calculate the velocity in the y-axis required to move between the two points
			double velocityY = distanceY / totalTimeOnLine;

			// Calculate the time on the line
			double currentTimeOnLine = time - (startTime * TimeSpan.TotalMilliseconds);

			// Calculate the x and Y positions on the line
			double x = startPoint.X + velocityX * currentTimeOnLine;
			double y = startPoint.Y + velocityY * currentTimeOnLine;

			// Return the intermediate point
			return new Point((int)Math.Round(x), (int)Math.Round(y));
		}

		/// <summary>
		/// Renders a static polygon.
		/// </summary>		
		private void RenderStaticPolygon(IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			List<Point> points, 
			IEnumerable<ElementLocation> sparseMatrix, 
			PixelLocationFrameBuffer locationFrameBuffer)
		{
			// Make copies of the display element width and height for performance
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			
			// Create a bitmap the size of the display element			
			using (var bitmap = new Bitmap(bufferWi, bufferHt))
			{
				if (points.Count > 2)
				{
					// Render the polygon on the bitmap
					InitialRenderPolygon(bitmap, points.ToArray(), FillPolygon, GetFillColor(intervalPos));
				}
				else
				{
					// Render the line on the bitmap
					InitialRenderLine(bitmap, points.ToArray(), GetFillColor(intervalPos));
				}

				// Create a FastPixel instance based on the bitmap				
				using (FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap))
				{					
					// If rendering in String mode then...
					if (sparseMatrix == null)
					{
						// Copy from the fastpixel bitmap to the frame buffer
						CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);												
					}
					// Otherwise we are in location mode...
					else
					{
						fp.Lock();

						// Loop over the sparse matrix pixels
						foreach (ElementLocation location in sparseMatrix)
						{
							// Copy the pixel from the fast pixel to the specified sparse matrix pixel (location frame buffer)
							CalculatePixel(location.X, location.Y, ref bufferHt, fp, locationFrameBuffer);
						}

						// Unlock the fast pixel
						fp.Unlock(false);
					}					
				}				
			}
		}
		
		/// <summary>
		/// Renders the collection of morph polygons.
		/// </summary>		
		private void RenderStaticPolygons(IPixelFrameBuffer frameBuffer, double intervalPos, List<IMorphPolygon> expandedMorphPolygon)
		{
			// If there are any static polygon/lines then...
			if (expandedMorphPolygon.Any())
			{
				// Make copies of the display element width and height for performance
				int bufferHt = BufferHt;
				int bufferWi = BufferWi;

				// Create a bitmap the size of the display element			
				using (Bitmap bitmap = new Bitmap(bufferWi, bufferHt))
				{
					// Loop over the morph polygons
					foreach (IMorphPolygon morphPolygon in expandedMorphPolygon)
					{
						// Convert the polygon/line points into Microsoft Drawing Points
						List<Point> points = morphPolygon.GetPolygonPoints()
							.Select(pt => new Point((int) Math.Round(pt.X), (int) Math.Round(pt.Y))).ToList();

						// If the points make a polygon then...
						if (points.Count > 2)
						{
							// Render the polygon
							InitialRenderPolygon(bitmap, points.ToArray(),
								morphPolygon.FillType == PolygonFillType.Solid,
								GetFillColor(intervalPos, morphPolygon));
						}
						else
						{
							// Otherwise render the line
							InitialRenderLine(bitmap, points.ToArray(), GetFillColor(intervalPos, morphPolygon));
						}
					}

					// Copy the polygons/lines to the frame buffer
					CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);
				}
			}
		}
		
		/// <summary>
		/// Copies the bitmap to the pixel frame buffer.
		/// </summary>		
		private void CopyBitmapToPixelFrameBuffer(Bitmap bitmap, IPixelFrameBuffer frameBuffer)
		{
			// Make copies of the display element width and height for performance
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;

			// Create a FastPixel instance based on the bitmap				
			using (FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap))
			{
				fp.Lock();

				// Copy from the fastpixel bitmap to the frame buffer
				for (int x = 0; x < bufferWi; x++)
				{
					for (int y = 0; y < bufferHt; y++)
					{
						// Transfer the pixel from the bitmap to the frame buffer
						CalculatePixel(x, y, ref bufferHt, fp, frameBuffer);
					}
				}
				fp.Unlock(false);
			}
		}
		
		/// <summary>
		/// Renders the specified polygon points.
		/// </summary>		
		private void InitialRenderPolygon(Bitmap bitmap, Point[] points, bool fillPolygon, Color fillColor)
		{			
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				if (fillPolygon)
				{
					using (SolidBrush brush = new SolidBrush(fillColor))
					{
						graphics.FillPolygon(brush, points);
					}
				}
				else				
				{
					using (Pen pen = new Pen(fillColor))
					{
						graphics.DrawPolygon(pen, points);
					}					
				}
			}
		}

		/// <summary>
		/// Renders the specified line points.
		/// </summary>		
		private void InitialRenderLine(Bitmap bitmap, Point[] points, Color fillColor)
		{
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{									
				using (Pen pen = new Pen(fillColor))
				{
					graphics.DrawLine(pen, points[0], points[1]);
				}				
			}
		}
	
		/// <summary>
		/// Calculates the color of the specified pixel from the fast pixel bitmap.
		/// </summary>		
		private void CalculatePixel(int x, int y, ref int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

			if (x >= 0 && y >= 0 && y < _bufferHt && x < _bufferWi)
			{
				Color color = bitmap.GetPixel(x, bufferHt - y - 1);

				if (!_emptyColor.Equals(color))
				{
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
				else if (TargetPositioning == TargetPositioningType.Locations)
				{
					frameBuffer.SetPixel(xCoord, yCoord, Color.Transparent);
				}
			}
		}
		
		/// <summary>
		/// Expands the pattern morph polygon based on the repeating pattern.
		/// </summary>
		private void ExpandPatternMorphPolygons()
		{
			// Create a new list of expanded morph polygons
			_patternExpandedMorphPolygons = new List<IMorphPolygon>();
			
			// Transfer the Fill Type to the morph polygon
			MorphPolygons[0].FillType = FillType;

			// Add the user drawn polygon to the collection
			_patternExpandedMorphPolygons.Add(MorphPolygons[0]);
			
			// Create morph polygons for the repeating pattern
			for (int index = 0; index < RepeatCount; index++)
			{
				// Clone the previous morph polygon
				IMorphPolygon morphPolygon = (IMorphPolygon)_patternExpandedMorphPolygons.Last().Clone();
				
				// Add the cloned morph polygon to the collection
				_patternExpandedMorphPolygons.Add(morphPolygon);

				// Move the morph polygon based on the repeat direction
				switch (RepeatDirection)
				{
					case WipeRepeatDirection.Down:
						morphPolygon.GetPointBasedShape().MovePoints(0, -RepeatSkip);
						break;
					case WipeRepeatDirection.Up:
						morphPolygon.GetPointBasedShape().MovePoints(0, +RepeatSkip);
						break;
					case WipeRepeatDirection.Left:
						morphPolygon.GetPointBasedShape().MovePoints(-RepeatSkip, 0);
						break;
					case WipeRepeatDirection.Right:
						morphPolygon.GetPointBasedShape().MovePoints(RepeatSkip, 0);
						break;
					default:
						Debug.Assert(false, "Unsupported Direction");
						break;
				}
			}			
		}

		/// <summary>
		/// Renders the effect for for pattern polygon mode.
		/// </summary>		
		private void RenderEffectPattern(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// If the polygon fill type is Wipe then...
			if (FillType == PolygonFillType.Wipe)
			{				
				// Render the morph polygons/lines using a wipe
				// Note this method handles the repeating pattern
				RenderEffectWipes(frameNum, frameBuffer, intervalPos, GetWipePolygons());
			}
			else
			{
				// If the pattern polygons have not been expanded then...
				if (_patternExpandedMorphPolygons == null)
				{
					// Expand the pattern polygon/line into multiple morph polygons
					ExpandPatternMorphPolygons();
				}

				// Render the solid or outline pattern polygons
				RenderStaticPolygons(frameBuffer, intervalPos, _patternExpandedMorphPolygons);
			}
		}

		/// <summary>
		/// Renders the effect for free form polygon mode.
		/// </summary>		
		private void RenderEffectFreeForm(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render the wipe polygons/lines
			List<IMorphPolygon> wipePolygons = GetWipePolygons();				
			RenderEffectWipes(frameNum, frameBuffer, intervalPos, wipePolygons);

			// Render the solid and outline polygons/lines
			List<IMorphPolygon> staticPolygons = MorphPolygons.Where(mp => mp.FillType != PolygonFillType.Wipe).ToList();			
			RenderStaticPolygons(frameBuffer, intervalPos, staticPolygons);
		}
					
		/// <summary>
		/// Renders the effect for the specified frame.
		/// </summary>		
		private void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer, IEnumerable<ElementLocation> sparseMatrix, PixelLocationFrameBuffer locationFrameBuffer)
        {
			// Get the position within the effect
			double intervalPos = GetEffectTimeIntervalPosition(frameNum);

			// Render based on the polygon type
			switch(PolygonType)
			{
				case PolygonType.TimeBased:
					RenderEffectTimeBased(frameNum, frameBuffer, intervalPos, sparseMatrix, locationFrameBuffer);
					break;
				case PolygonType.Pattern:
					RenderEffectPattern(frameNum, frameBuffer, intervalPos);
					break;
				case PolygonType.FreeForm:
					RenderEffectFreeForm(frameNum, frameBuffer, intervalPos);
					break;
				default:
					Debug.Assert(false, "Unsupported Polygon Type");
					break;

			}			
		}

		/// <summary>
		/// Returns the length of the wipe polygon.  Note not looking at the start or end sides of the polygon.
		/// </summary>		
		private int GetLengthOfWipePolygon(MorphWipePolygonRenderData wipeRenderData)
		{
			// Determine which side of the polygon is the longest in the x direction
			// Note this is NOT the start and stop sides.
			int length = wipeRenderData.X1Points.Count;

			if (wipeRenderData.X2Points.Count > length)
			{
				length = wipeRenderData.X2Points.Count;
			}

			return length;
		}

		/// <summary>
		/// Renders the wipes for the specified morph polygons.
		/// </summary>		
		private void RenderEffectWipes(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos, List<IMorphPolygon> morphPolygons)
		{
			// Loop over the morph polygons
			for (int polygonIndex = 0; polygonIndex < morphPolygons.Count; polygonIndex++)
			{
				// Get the specified morph polygon
				MorphPolygon morphPolygon = (MorphPolygon)morphPolygons[polygonIndex];

				// Determine which side of the polygon is the longest in the x direction
				// Note this is NOT the start and stop sides.
				int length = GetLengthOfWipePolygon(_wipePolygonRenderData[polygonIndex]);

				// Initialize the time for the wipe
				double time = TimeSpan.TotalMilliseconds;

				// If stagger has been specified then...
				if (_stagger != 0)
				{
					// Reduce the time for this wipe based on the number of polygons and the amount of stagger
					time = time - (_stagger * RepeatCount / 100.0) * time;
				}

				// Make copies of all the points
				List<int> x1PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].X1Points, 0);
				List<int> x2PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].X2Points, 0);
				List<int> y1PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].Y1Points, 0);
				List<int> y2PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].Y2Points, 0);

				// Draw a wipe for each repeated polygon
				for (int index = 0; index < RepeatCount + 1; index++)
				{
					// Determine the time in the wipe
					double timeInWipe = frameNum * FrameTime;

					// Adjust the time for the stagger
					timeInWipe = timeInWipe - (index * (Stagger / 100.0) * time);

					// If the wipe time is valid (positive) then...
					if (timeInWipe >= 0.0)
					{							
						// Calculate the wipe head position using the acceleration and initial velocity			
						double headPosition = GetIncreasingHeadPosition(
							_wipePolygonRenderData[polygonIndex].HeadAcceleration,
							timeInWipe,
							_wipePolygonRenderData[polygonIndex].HeadVelocityZero);

						// Calculate the interval position factor (0-100.0)
						double intervalPosFactor = intervalPos * 100;

						// Once the head has completed the wipe set a flag so that it doesn't back up
						// back onto the display element when de-accelerating.
						if (headPosition > length + morphPolygon.HeadLength - 1)
						{
							_wipePolygonRenderData[polygonIndex].HeadIsDone = true;								
						}

						// If the head has wiped across the polygon then....
						if (_wipePolygonRenderData[polygonIndex].HeadIsDone)
						{
							// Set its position just past the end of the polygon so that the tail
							// can wipe off the polygon							
							headPosition = length + morphPolygon.HeadLength;
						}

						// Calculate the tail position					
						int tailPosition = (int)GetIncreasingHeadPosition(
							_wipePolygonRenderData[polygonIndex].TailAcceleration,
							timeInWipe,
							_wipePolygonRenderData[polygonIndex].TailVelocityZero);
								
						// Calculate the position in the wipe
						double intervalHeadPos = headPosition / length;
						double intervalHeadPosFactor = intervalHeadPos * 100;

						// Retrieve the head and tail colors from the morph polygon gradients
						Color headColor = GetHeadColor(intervalPos, morphPolygon);
						Color tailColor = GetTailColor(intervalPos, morphPolygon);

						// Determine which set of points is longer
						if (_wipePolygonRenderData[polygonIndex].X1Points.Count > _wipePolygonRenderData[polygonIndex].X2Points.Count)
						{
							DrawWipe(
								intervalHeadPos,
								frameBuffer,
								tailPosition,
								x2PointsCopy, // Short Points
								y2PointsCopy,
								x1PointsCopy, // Long Points
								y1PointsCopy,
								length,
								_wipePolygonRenderData[polygonIndex].Direction,
								headColor,
								tailColor,
								morphPolygon.HeadLength);
						}
						else
						{
							DrawWipe(
								intervalHeadPos,
								frameBuffer,
								tailPosition,
								x1PointsCopy, // Short Points
								y1PointsCopy, // Long Points
								x2PointsCopy,
								y2PointsCopy,
								length,
								_wipePolygonRenderData[polygonIndex].Direction,
								headColor,
								tailColor,
								morphPolygon.HeadLength);
						}
					}

					// Update the points based on the repeating pattern
					switch (RepeatDirection)
					{
						case WipeRepeatDirection.Down:
							y1PointsCopy = UpdatePointList(y1PointsCopy, -RepeatSkip);
							y2PointsCopy = UpdatePointList(y2PointsCopy, -RepeatSkip);
							break;
						case WipeRepeatDirection.Up:
							y1PointsCopy = UpdatePointList(y1PointsCopy, RepeatSkip);
							y2PointsCopy = UpdatePointList(y2PointsCopy, RepeatSkip);
							break;
						case WipeRepeatDirection.Left:
							x1PointsCopy = UpdatePointList(x1PointsCopy, -RepeatSkip);
							x2PointsCopy = UpdatePointList(x2PointsCopy, -RepeatSkip);
							break;
						case WipeRepeatDirection.Right:
							x1PointsCopy = UpdatePointList(x1PointsCopy, RepeatSkip);
							x2PointsCopy = UpdatePointList(x2PointsCopy, RepeatSkip);
							break;
						default:
							Debug.Assert(false, "Unsupported Direction");
							break;
					}
				}				
			}
		}

		/// <summary>
		/// Update the following points with the specified offset.
		/// </summary>		
		private List<int> UpdatePointList(List<int> ptList, int offset)
		{
			List<int> ptListCopy = new List<int>();

			foreach (int pt in ptList)
			{
				ptListCopy.Add(pt + offset);
			}

			return ptListCopy;
		}

		/// <summary>
		/// Calculates the intensity of the specified position of the wipe tail.		
		/// </summary>		
		double GetTailIntensity(int x, int tailStartPosition, int tailEndPosition)
		{												
			// Calculate 0-1 intensity based on where the position is on the tail
			double intensity = (x - tailStartPosition + 1.0) / ((tailEndPosition- tailStartPosition));

			// Adjust the first half of the tail to full intensity
			if (intensity > .5)
			{
				intensity = 1.0;
			}
			// Then the second half of the tail fades from full intensity zero
			else
			{
				intensity = intensity / .5;
			}			
			
			return intensity;
		}

		/// <summary>
		/// Draws the specified wipe.
		/// </summary>		
		private void DrawWipe(			
			double intervalHeadPos,
			IPixelFrameBuffer frameBuffer,
			int tailPosition,
			List<int> xShortPoints,
			List<int> yShortPoints,
			List<int> xLongPoints,
			List<int> yLongPoints,
			int length,
			bool direction,
			Color headColor,
			Color tailColor,
			int headLength)
		{								
			// Calculate the end of the head			
			int endOfHead = (int)(length * intervalHeadPos);
			
			// Calculate the start of the head based on the current head length
			int startOfHead = endOfHead - headLength;
			
			// Don't allow the head to start off the display element so that
			// the tail is calculated properly.
			if (startOfHead < 0)
			{
				startOfHead = 0;
			}

			// If the start of the head is off the display element then...
			if (startOfHead > length - 1)						
			{
				// Set the start of the head to just off the display element
				startOfHead = length;
			}

			// Draw tail
			for (double x = tailPosition; x < startOfHead; x+=0.1)
			{				
				// Calculate the intensity of the tail x position
				HSV hsv = HSV.FromRGB(tailColor);
				int index = (int)x;
				hsv.V *= GetTailIntensity(index, tailPosition, startOfHead);

				DrawThickLine(x, length, xShortPoints, yShortPoints, xLongPoints, yLongPoints, hsv.ToRGB(), direction, frameBuffer);				
			}

			// If the start of the head is NOT off the display element then...
			// Drawing the head last so that if the head and tail overlap the head will win
			if (startOfHead < length)
			{
				// Don't let the end of the head extend off the display element
				if (endOfHead > length - 1)
				{
					endOfHead = length - 1;
				}

				// Loop over the head lines
				for (double x = startOfHead; x < endOfHead; x+=0.1)
				{
					DrawThickLine(x, length, xShortPoints, yShortPoints, xLongPoints, yLongPoints, headColor, direction, frameBuffer);
				}
			}			
		}

		/// <summary>
		/// Draws a line between the two specified points.
		/// </summary>		
		private void DrawThickLine(
			double longIndex,
			int length,
			List<int> xShortPoints,
			List<int> yShortPoints,
			List<int> xLongPoints,
			List<int> yLongPoints,
			Color color, 
			bool direction,
			IPixelFrameBuffer frameBuffer)
		{
			// Get the length of the short side
			int shortLength = xShortPoints.Count;

			// Calculate what percentage of the long side position
			double pct = longIndex / length;

			// Calculate the short side index based on the same percentage as the long side
			int shortIndex = (int)(shortLength * pct);

			// Convert the long side index to an integer						
			int integerLongIndex = (int)longIndex;

			// Draw a line from the short side to the long side
			DrawThickLine(xShortPoints[shortIndex], yShortPoints[shortIndex], xLongPoints[integerLongIndex], yLongPoints[integerLongIndex], color, direction, frameBuffer);
		}
		
		/// <summary>
		/// Gets all the polygons configured with the Wipe fill.
		/// </summary>		
		private List<IMorphPolygon> GetWipePolygons()
		{
			// Find all the morph polygons that are a polygon with 4 points OR is a line			
			return MorphPolygons.Where(mp => mp.FillType == PolygonFillType.Wipe &&
			                           ((mp.Polygon != null && mp.Polygon.Points.Count == 4 ||
									    mp.Line != null))).ToList();					
		}

		/// <summary>
		/// Updates the visibility of fields.
		/// </summary>
		private void UpdateAttributes()
		{			
			UpdateStringOrientationAttributes();
			UpdatePolygonTypeAttributes(true);
			TypeDescriptor.Refresh(this);
		}
											
		/// <summary>
		/// Gets the head color of the morph polygon.  This property only applies to pattern mode.
		/// </summary>		
		private Color GetHeadColor(double intervalPos, MorphPolygon morphPolygon)
		{
			return morphPolygon.HeadColor.GetColorAt(intervalPos);
		}

		/// <summary>
		/// Gets the tail color of the morph polygon.  This property only applies to pattern mode.
		/// </summary>		
		private Color GetTailColor(double intervalPos, IMorphPolygon morphPolygon)
		{
			return morphPolygon.TailColor.GetColorAt(intervalPos);
		}

		/// <summary>
		/// Gets the fill color of the morph polygon.  This property only applies to time based mode.
		/// </summary>		
		private Color GetFillColor(double intervalPos)
		{
			return FillColor.GetColorAt(intervalPos);
		}

		/// <summary>
		/// Gets the fill color of the specified morph polygon.  This property applies to free form mode and
		/// when the pattern mode is expanded out into morph polygons.
		/// </summary>		
		private Color GetFillColor(double intervalPos, IMorphPolygon morphPolygon)
		{
			return morphPolygon.FillColor.GetColorAt(intervalPos);
		}

		/// <summary>
		/// Converts from the model morph polygon data to the serialized morph polygon data.
		/// </summary>
		private void UpdateMorphSerializedData()
		{
			// Clear the collection of wave forms
			_data.MorphPolygonData.Clear();

			// Loop over the polygons in the model polygon collection
			foreach (IMorphPolygon morphPolygon in MorphPolygons.ToList())
			{
				// Create a new serialized polygon
				MorphPolygonData serializedPolygon = new MorphPolygonData();

				// Transfer the properties from the polygon model to the serialized polygon data
				serializedPolygon.HeadLength = morphPolygon.HeadLength;
				serializedPolygon.HeadDuration = morphPolygon.HeadDuration;
				serializedPolygon.Acceleration = morphPolygon.Acceleration;
				serializedPolygon.HeadColor = new ColorGradient(morphPolygon.HeadColor);
				serializedPolygon.TailColor =  new ColorGradient(morphPolygon.TailColor);
				serializedPolygon.Time = morphPolygon.Time;
				serializedPolygon.FillType = morphPolygon.FillType;
				serializedPolygon.FillColor = new ColorGradient(morphPolygon.FillColor);

				if (morphPolygon.Polygon != null)
				{
					serializedPolygon.Polygon = morphPolygon.Polygon.Clone();					
				}
				else
				{
					serializedPolygon.Line = morphPolygon.Line.Clone();						
				}

				// Add the serialized polygon to the collection
				_data.MorphPolygonData.Add(serializedPolygon);
			}
		}
		
		/// <summary>
		/// Converts from the serialized polygon data to the model polygon data.
		/// </summary>		
		private void UpdatePolygonModel(MorphData morphData)
		{
			// Clear the model polygon collection
			MorphPolygons.Clear();

			// Loop over the polygons in the serialized effect data
			foreach (MorphPolygonData serializedPolygon in morphData.MorphPolygonData)
			{
				// Create a new morph polygon in the model
				MorphPolygon morphPolygon = new MorphPolygon();

				// Transfer the properties from the serialized effect data to the morph polygon model
				morphPolygon.HeadLength = serializedPolygon.HeadLength;
				morphPolygon.HeadDuration = serializedPolygon.HeadDuration;
				morphPolygon.Acceleration = serializedPolygon.Acceleration;
				morphPolygon.HeadColor = new ColorGradient(serializedPolygon.HeadColor);
				morphPolygon.TailColor = new ColorGradient(serializedPolygon.TailColor);
				morphPolygon.Time = serializedPolygon.Time;
				morphPolygon.FillType = serializedPolygon.FillType;
				morphPolygon.FillColor = new ColorGradient(serializedPolygon.FillColor);

				if (serializedPolygon.Polygon != null)
				{
					morphPolygon.Polygon = serializedPolygon.Polygon.Clone();						
				}
				else
				{
					morphPolygon.Line = serializedPolygon.Line.Clone();												
				}

				// Add the polygon to the effect's collection
				MorphPolygons.Add(morphPolygon);				
			}
		}
		
		/// <summary>
		/// Updates which morph polygon attributes are visible.
		/// </summary>		
		private void UpdatePolygonTypeAttributes(bool refresh)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
				{					
					{ nameof(RepeatCount), PolygonType == PolygonType.Pattern },
					{ nameof(RepeatSkip), PolygonType == PolygonType.Pattern },
					{ nameof(Stagger), PolygonType == PolygonType.Pattern },
					{ nameof(FillColor), PolygonType == PolygonType.TimeBased },
					{ nameof(MorphPolygons), PolygonType == PolygonType.FreeForm },
					{ nameof(RepeatDirection), PolygonType == PolygonType.Pattern },

					{ nameof(HeadColor), PolygonType == PolygonType.Pattern },
					{ nameof(TailColor), PolygonType == PolygonType.Pattern },
					{ nameof(HeadDuration), PolygonType == PolygonType.Pattern },
					{ nameof(HeadLength), PolygonType == PolygonType.Pattern },
					{ nameof(Acceleration), PolygonType == PolygonType.Pattern },		
					{ nameof(FillPolygon), PolygonType == PolygonType.TimeBased },
					{ nameof(FillType), PolygonType == PolygonType.Pattern},					
				};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the browseable state of properties related to polygon fill type.
		/// </summary>
		private void UpdatePolygonFillTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{nameof(HeadLength), FillType == PolygonFillType.Wipe },
				{nameof(HeadDuration), FillType == PolygonFillType.Wipe },
				{nameof(Acceleration), FillType == PolygonFillType.Wipe },
				{nameof(HeadColor), FillType == PolygonFillType.Wipe },
				{nameof(TailColor), FillType == PolygonFillType.Wipe },
				{nameof(Stagger), FillType == PolygonFillType.Wipe },
				{nameof(FillColor), (FillType == PolygonFillType.Solid || FillType == PolygonFillType.Outline) },				
			};
			SetBrowsable(propertyStates);
			
			TypeDescriptor.Refresh(this);			
		}
	}

	#endregion
}