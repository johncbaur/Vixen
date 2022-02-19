using System.ComponentModel;
using System.Runtime.InteropServices;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Abstract base class for a tagged filter module.
	/// </summary>
	/// <typeparam name="TFilterData">Type of the filter data</typeparam>
	/// <typeparam name="TOutput">Type of the filter output</typeparam>
	/// <typeparam name="TDescriptor">Type of the filter descriptor</typeparam>
	public abstract class TaggedFilterModuleBase<TFilterData, TOutput, TDescriptor> : OutputFilterModuleInstanceBase
		where TFilterData : IModuleDataModel, ITaggedFilterData
		where TOutput : class, ITaggedFilterOutput, IDataFlowOutput<IntentsDataFlowData>, IDataFlowOutput
		where TDescriptor : ITaggedFilterDescriptor, IModuleDescriptor
	{
		#region Fields

		/// <summary>
		/// Output associated with the filter.
		/// </summary>
		private TOutput _output;

		/// <summary>
		/// Outputs associated with the filter in array format.
		/// </summary>
		private TOutput[] _outputsArray;

		#endregion

		#region Protected Properties

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
		protected TFilterData Data { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Handles, filters, and converts intents.
		/// </summary>
		/// <param name="obj">Intents to process</param>
		public override void Handle(IntentsDataFlowData obj)
		{
			// Forward the call onto the output
			_output.ProcessInputData(obj);
		}

		/// <summary>
		/// Creates the output associated with filter.
		/// </summary>
		public void CreateOutput()
		{
			// Create the output and associated filter
			_output = CreateOutputInternal();
			
			// Convert the outputs to an array
			_outputsArray = new TOutput[] {_output};
		}

		#endregion

		#region Public Properties

		public override DataFlowType InputDataType => DataFlowType.MultipleIntents;

		public override DataFlowType OutputDataType => DataFlowType.MultipleIntents;

		public override IDataFlowOutput[] Outputs => _outputsArray;

		public override IModuleDataModel ModuleData
		{
			get => Data;
			set
			{
				// Save off the data associated with filter
				Data = (TFilterData)value;

				// Create the output associated with the filter
				CreateOutput();
			}
		}

		[Browsable(false)]
		public override IModuleDescriptor Descriptor
		{
			get
			{
				// Call the base class implementation
				ITaggedFilterDescriptor taggedFilterDescriptor = (ITaggedFilterDescriptor)base.Descriptor;

				if (Data != null)
				{
					// Assign the tag to the Type Name Postfix
					// This allows the graphical system to display the Tag
					taggedFilterDescriptor.TypeNamePostFix = Data.Tag;
				}

				return (IModuleDescriptor)taggedFilterDescriptor;

			}
			set
			{
				base.Descriptor = value;
			}
		}

		/// <summary>
		/// Gets or sets the tag associated with filter module.
		/// </summary>
		public string Tag
		{
			get
			{
				return Data.Tag;
			}
			set
			{
				Data.Tag = value;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates the output associated with the tagged filter module.
		/// </summary>
		/// <returns></returns>
		protected abstract TOutput CreateOutputInternal();

		#endregion

		public override string Name
		{
			get { return Descriptor.TypeName + ": " + Tag; }
		}
	}
}
