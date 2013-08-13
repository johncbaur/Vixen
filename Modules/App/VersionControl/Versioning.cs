﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using GitSharp;

namespace VersionControl
{
    public partial class Versioning : Form
    {
        Data _data;

        public Data VersionControlData
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

        Dictionary<string, List<ChangeDetails>> _changeDetails;

        GitSharp.Repository _repo;

        public Versioning(Data data, GitSharp.Repository repo, Dictionary<string, List<ChangeDetails>> changeDetails)
        {
            InitializeComponent();
            VersionControlData = data;
            _changeDetails = changeDetails;
            _repo = repo;

            LoadFileStructure();
        }

        private void LoadFileStructure()
        {

            treeViewFiles.Nodes.Clear();
            treeViewFiles.Nodes.Add("Vixen 3");
            PopulateTreeView(_repo.Directory.Replace("\\.git", string.Empty), treeViewFiles.Nodes[0]);
            treeViewFiles.Nodes[0].Expand();

            //_rootPaths.ToList().ForEach(dir => {
            //	var rootNode = CreateTreeNodeForDirectory(dir);
            //	rootNode.Expand();
            //	treeViewFiles.Nodes.Add(rootNode);
            //});

        }

        private void AddTreeNode(ChangeDetails detail)
        {
            try
            {

                var details = detail.FileName.Split('/');
                TreeNode node;
                foreach (var item in details)
                {

                    if (treeViewFiles.Nodes.ContainsKey(item))
                    {
                        node = treeViewFiles.Nodes.Find(item, false).First();
                    }
                    else
                    {
                        node = new TreeNode(item);
                        node.Tag = detail;
                        treeViewFiles.Nodes.Add(node);
                    }
                }

                node = new TreeNode(detail.ChangeDate.ToString());
                node.Tag = detail;
                node.Nodes.Add(node);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        string substringDirectory;

        private void PopulateTreeView(string directoryValue, TreeNode parentNode)
        {
            string[] directoryArray = Directory.GetDirectories(directoryValue);

            try
            {
                if (directoryArray.Length != 0)
                {
                    foreach (string directory in directoryArray)
                    {
                        substringDirectory = directory.Substring(
                        directory.LastIndexOf('\\') + 1,
                        directory.Length - directory.LastIndexOf('\\') - 1);

                        if (!substringDirectory.Equals(".git", StringComparison.CurrentCultureIgnoreCase) && !substringDirectory.Equals("Logs", StringComparison.CurrentCultureIgnoreCase))
                        {
                            TreeNode myNode = new TreeNode(substringDirectory);

                            parentNode.Nodes.Add(myNode);

                            PopulateTreeView(directory, myNode);
                        }
                    }

                }
                var fileArray = Directory.GetFiles(directoryValue);
                foreach (var file in fileArray)
                {
                    TreeNode fileNode = new TreeNode(new FileInfo(file).Name);
                    fileNode.Tag = file.Replace(_repo.Directory.Replace("\\.git", string.Empty) + "\\", string.Empty).Replace("\\", "/");
                    parentNode.Nodes.Add(fileNode);
                }

                parentNode.ClearEmptyChildren();
            }
            catch (UnauthorizedAccessException)
            {
                parentNode.Nodes.Add("Access denied");
            } // end catch  
        }





        private void treeViewFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBoxChangeHistory.Items.Clear();
            GetVersionControlInfo(e.Node.Tag as string);

        }

        private void GetVersionControlInfo(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (_changeDetails.ContainsKey(fileName))
                {
                    var details = _changeDetails[fileName];

                    details.ForEach(d =>
                    {
                        listBoxChangeHistory.Items.Add(d.ChangeDate);
                    });
                }
            }

            if (listBoxChangeHistory.Items.Count > 0)
                listBoxChangeHistory.SelectedIndex = 0;
        }

        private void listBoxChangeHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRestore.Enabled = listBoxChangeHistory.SelectedIndex > 0;

            this.txtChangeMessage.Text =
                this.txtChangeFileName.Text =
                this.txtChangeDate.Text =
                this.txtChangeHash.Text =

                this.txtChangeUser.Text = string.Empty;

            if (listBoxChangeHistory.SelectedItem != null)
            {
                var change = _changeDetails[treeViewFiles.SelectedNode.Tag as string].Where(w => w.ChangeDate == (DateTimeOffset)listBoxChangeHistory.SelectedItem).FirstOrDefault();
                if (change != null)
                {
                    this.txtChangeDate.Text = change.ChangeDate.ToString();
                    this.txtChangeHash.Text = change.Hash;
                    this.txtChangeUser.Text = change.UserName;
                    this.txtChangeMessage.Text = change.Message;
                    this.txtChangeFileName.Text = change.FileName;

                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Are you sure you want to restore this version?  \n\rIf you have not saved the current file, all changes will be lost!", "Restore File", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);

            if (res == DialogResult.Yes)
            {

                var commit = _repo.Get<Commit>(this.txtChangeHash.Text);
                var tree = commit.Tree;
                foreach (Tree subtree in tree.Trees)
                {
                    var leaf = subtree.Leaves.Where(l => l.Path.Equals(treeViewFiles.SelectedNode.Tag as string)).FirstOrDefault();
                    if (leaf != null)
                    {

                        var rawData = leaf.RawData;
                        var fileName = System.IO.Path.Combine(_repo.WorkingDirectory, treeViewFiles.SelectedNode.Tag as string);
                        var b = fileName;
                        var c = b;

                        lock (Module.fileLockObject)
                        {
                            Module.restoringFile = true;
                            File.WriteAllBytes(fileName, rawData);
                        }
                    }
                }
            }

        }


    }
}
