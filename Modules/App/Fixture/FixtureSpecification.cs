using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Data.Value;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains meta-data (channels and functions) of an intelligent fixture.
	/// </summary>
    [DataContract]
	public class FixtureSpecification
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public FixtureSpecification()
		{
			// Create the channel definition collection
			ChannelDefinitions = new List<FixtureChannel>();
			
			// Create the function definition collection
			FunctionDefinitions = new List<FixtureFunction>();			
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the filename of the fixture.
		/// This method removes any special characters that are not allowed by Windows as a file name.
		/// </summary>
		/// <returns>File name of the fixture</returns>
		public string GetFileName()
		{
			// < (less than)
			string fileName = Name.Replace('<', '(');

			// > (greater than)
			fileName = fileName.Replace('>', '(');

			// : (colon - sometimes works, but is actually NTFS Alternate Data Streams)
			fileName = fileName.Replace(':', '_');

			// " (double quote)
			fileName = fileName.Replace('"', '_');

			// / (forward slash)
			fileName = fileName.Replace('/', '_');

			// \ (backslash)
			fileName = fileName.Replace('\\', '_');

			// | (vertical bar or pipe)
			fileName = fileName.Replace('|', '_');

			// ? (question mark)
			fileName = fileName.Replace('?', '_');

			// * (asterisk)
			fileName = fileName.Replace('*', '_');

			// Append the .xml file extension
			fileName += ".xml";

			return fileName;
		}

		/// <summary>
		/// Clones the fixture specification.
		/// </summary>
		/// <returns>Clone of the fixture specification</returns>
		public FixtureSpecification CreateInstanceForClone()
		{
			// Create a new fixture specification
			FixtureSpecification clone = new FixtureSpecification();

			// Clone the name of the fixture
			clone.Name = Name;

			// JCB - TODO: Remove this property
			clone.TranslateRGBIntoColorWheel = TranslateRGBIntoColorWheel;

			// Loop over the channel definitions
			foreach(FixtureChannel channel in ChannelDefinitions)					
			{
				// Clone the channel definition
				clone.ChannelDefinitions.Add(channel.CreateInstanceForClone());
			}

			// Loop over the function definitions
			foreach(FixtureFunction function in FunctionDefinitions)
			{
				// Clone the function definition
				clone.FunctionDefinitions.Add(function.CreateInstanceForClone());
			}

			return clone;
		}

		/// <summary>
		/// Returns true if the specified function name is supported by the fixture.
		/// </summary>
		/// <param name="functionName">Name of the function to check</param>
		/// <returns>True if the function is supported</returns>
		public bool SupportsFunction(string functionName)
		{
			// Return whether the function is supported
			return FunctionDefinitions.Any(item => item.Name == functionName);
		}

		/// <summary>
		/// Adds a function to the fixture specification.
		/// </summary>
		/// <param name="name">Name of the function</param>
		/// <param name="functionType">Type of the function</param>
		/// <param name="identity">Preview identity of the function</param>
		/// <returns></returns>
		public FixtureFunction AddFunctionType(
			string name,
			FixtureFunctionType functionType,
			FunctionIdentity identity)
		{
			// Create the new function
			FixtureFunction function = new FixtureFunction();

			// Configure the name on the function
			function.Name = name;

			// Configure the function type
			function.FunctionType = functionType;			
			
			// Configure the function identity
			function.FunctionIdentity = identity;
			
			// Add the function to the fixture specification
			FunctionDefinitions.Add(function);

			return function;
		}

		/// <summary>
		/// 
		/// </summary>
		public void InitializeBuiltInFunctions()
		{
			FunctionDefinitions.Clear();

			AddFunctionType(
				"Pan",
				FixtureFunctionType.Range,
				FunctionIdentity.Pan);

			AddFunctionType(
				"Tilt",
				FixtureFunctionType.Range,
				FunctionIdentity.Tilt);

			AddFunctionType(
				"Color Wheel",
				FixtureFunctionType.ColorWheel,
				FunctionIdentity.Custom);
			
			AddFunctionType(
				"Dimmer",
				FixtureFunctionType.Range,
				FunctionIdentity.Dim);

			AddFunctionType(
				"Color",
				FixtureFunctionType.RGBWColor,
				FunctionIdentity.Custom);

			AddFunctionType(
				"Zoom",
				FixtureFunctionType.Range,
				FunctionIdentity.Zoom);

			AddFunctionType(
				"Shutter",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Shutter);

			AddFunctionType(
				"None",
				FixtureFunctionType.None,
				FunctionIdentity.Custom);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Name of the fixture.
		/// </summary>
		[DataMember]
		public string Name { get; set; }
		
		/// <summary>
		/// Collection of channel definitions.
		/// </summary>
		[DataMember]
		public List<FixtureChannel> ChannelDefinitions { get; set; }

		/// <summary>
		/// Collection of function definitions.
		/// </summary>
		[DataMember]
		public List<FixtureFunction> FunctionDefinitions { get; set; }

		/// <summary>
		/// JCB - Consider moving this property!
		/// </summary>
		[DataMember]
		public bool TranslateRGBIntoColorWheel { get; set; }

		/// <summary>
		/// JCB - Consider moving this property!
		/// </summary>
		[DataMember]
		public bool TranslateColorIntensityIntoDimmer { get; set; }

		#endregion		
	}
}
