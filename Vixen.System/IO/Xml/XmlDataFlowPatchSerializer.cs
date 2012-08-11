﻿using System;
using System.Xml.Linq;
using Vixen.Data.Flow;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlDataFlowPatchSerializer : IXmlSerializer<DataFlowPatch> {
		private const string ELEMENT_PATCH = "Patch";
		private const string ATTR_COMPONENT_ID = "id";
		private const string ATTR_SOURCE_COMPONENT_ID = "sourceId";
		private const string ATTR_SOURCE_OUTPUT_INDEX = "output";

		public XElement WriteObject(DataFlowPatch value) {
			XElement element = new XElement(ELEMENT_PATCH,
				new XAttribute(ATTR_COMPONENT_ID, value.ComponentId),
				value.SourceComponentId.HasValue ? new XAttribute(ATTR_SOURCE_COMPONENT_ID, value.SourceComponentId.Value) : null,
				value.SourceComponentId.HasValue ? new XAttribute(ATTR_SOURCE_OUTPUT_INDEX, value.SourceComponentOutputIndex) : null);
			return element;
		}

		public DataFlowPatch ReadObject(XElement source) {
			Guid? componentId = XmlHelper.GetGuidAttribute(source, ATTR_COMPONENT_ID);
			if(componentId == null) return null;
			Guid? sourceId = XmlHelper.GetGuidAttribute(source, ATTR_SOURCE_COMPONENT_ID);
			int sourceOutputIndex = XmlHelper.GetIntAttribute(source, ATTR_SOURCE_OUTPUT_INDEX).GetValueOrDefault();
			return new DataFlowPatch(componentId.Value, sourceId, sourceOutputIndex);
		}
	}
}
