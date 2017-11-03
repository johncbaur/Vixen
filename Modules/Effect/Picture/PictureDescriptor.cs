﻿using System;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Picture
{
	public class PictureDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("5f33435c-eb96-4018-ae56-07e8e14e9ec9");

		public PictureDescriptor()
		{
			ModulePath = EffectName;
		}

		[ModuleDataPath]
		public static string ModulePath { get; set; }

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override bool SupportsImage
		{
			get { return true; }
		}

		public override string MediaPath
		{
			get { return PictureDescriptor.ModulePath; }
		}

		public override string[] SupportsExtensions
		{
			get { return new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };}
		}

		public override Type ModuleClass
		{
			get { return typeof(Picture); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(PictureData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies an effect based on a picture to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Picture"; }
		}
	}
}
