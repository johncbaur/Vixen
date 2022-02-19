using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorWheelBreakdown
{	
	internal class ColorWheelBreakdownFilter : IntentStateDispatch
	{
		private IIntentState _intentValue = null;

		public ColorWheelBreakdownFilter(string functionName)
		{
			FunctionTag = functionName;
		}

		public string FunctionTag { get; private set; }

		public IIntentState Filter(IIntentState intentValue)
		{
			_intentValue = null;
			intentValue.Dispatch(this);
			return _intentValue;
		}

		public override void Handle(IIntentState<RangeValue> obj)
		{
			if (obj.GetValue().Tag == FunctionTag)
			{
				_intentValue = obj;
			}
		}

		public override void Handle(IIntentState<CommandValue> obj)
		{
			Named8BitCommand taggedCommand = obj.GetValue().Command as Named8BitCommand;

			if (taggedCommand != null &&
				taggedCommand.Tag == FunctionTag)
			{
				_intentValue = obj;
			}
		}
	}
}