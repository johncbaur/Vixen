﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using WeifenLuo.WinFormsUI.Docking;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.Property.Color;
using VixenModules.App.ElementTemplateHelper;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetupElementsDocument : DockContent
	{
		private VixenPreviewControl _preview;

		public VixenPreviewSetupElementsDocument(VixenPreviewControl preview)
		{
			InitializeComponent();
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddTemplate.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddTemplate.Text = "";
			var elementTemplates = ApplicationServices.GetAllElementTemplates();
			comboBoxNewItemType.BeginUpdate();
			foreach (IElementTemplate template in elementTemplates)
			{
				ComboBoxItem item = new ComboBoxItem(template.TemplateName, template);
				comboBoxNewItemType.Items.Add(item);
			}
			comboBoxNewItemType.EndUpdate();
			if (comboBoxNewItemType.Items.Count > 0)
				comboBoxNewItemType.SelectedIndex = 0;

			ThemeUpdateControls.UpdateControls(this);
			_preview = preview;
			treeElements.AllowPropertyEdit = false;
			treeElements.AllowWireExport = false;
		}

		private void VixenPreviewSetupElementsDocument_Load(object sender, EventArgs e)
		{
			treeElements.treeviewAfterSelect += treeElements_AfterSelect;
			treeElements.treeviewDeselected += TreeElementsOnTreeviewDeselected;
		}

		private void HighlightNode(ElementNode node)
		{
			// Is this a group?
			if (!node.IsLeaf)
			{
				// If so, iterate through children and highlight them
				foreach (var childNode in node.Children)
				{
					HighlightNode(childNode);
				}
			}

			// Finally, highlight the node passed to us
			_preview.HighlightedElements.Add(node.Id);
			_preview.DeSelectSelectedDisplayItem();
		}

		private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
		{
			_preview.BeginUpdate();
			_preview.HighlightedElements.Clear();

			foreach (var node in treeElements.SelectedElementNodes) {
				HighlightNode(node);
			}

			if (!_preview.SelectedDisplayItems.Any())
			{
				_preview.propertiesForm.ClearSetupControl();
			}

			_preview.EndUpdate();
		}

		private void TreeElementsOnTreeviewDeselected(object sender, EventArgs e)
		{
			TreeViewNodesDeselected();
		}

		private void TreeViewNodesDeselected()
		{
			_preview.BeginUpdate();
			_preview.HighlightedElements.Clear();
			if (!_preview.SelectedDisplayItems.Any())
			{
				_preview.propertiesForm.ClearSetupControl();
			}

			_preview.EndUpdate();
		}

		public ElementNode SelectedNode => treeElements.SelectedNode;

		public void SelectElementNode(ElementNode node)
		{
			treeElements.SelectElementNode(node);
		}

		private async void ButtonAddTemplate_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxNewItemType.SelectedItem as ComboBoxItem);

			if (item != null)
			{
				IElementTemplate template = item.Value as IElementTemplate;
				await SetupTemplate(template);
			}
		}

		internal void ClearSelectedNodes()
		{
			treeElements.ClearSelectedNodes();
			TreeViewNodesDeselected();
		}

		internal async Task<bool> SetupTemplate(IElementTemplate template)
		{
			// Create the element template helper
			ElementTemplateHelper elementTemplateHelper = new ElementTemplateHelper();

			// Process the template for the selected node(s)
			IEnumerable<ElementNode> createdElements = await elementTemplateHelper.ProcessElementTemplate(
				treeElements.SelectedElementNodes,
				template,
				this,
				(node) => AddNodeToTree(node),
				treeElements);
			
			// Return whether nodes were created
			return (createdElements != null);
		}

		internal void AddNodeToTree(ElementNode node)
		{
			if (node == null) return;
			treeElements.AddNodePathToTree(new[] { node });
			treeElements.UpdateScrollPosition();
		}

		private void ComboBoxNewItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonAddTemplate.Enabled = comboBoxNewItemType.SelectedIndex >= 0;
		}

		private void ComboBoxNewItemType_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}