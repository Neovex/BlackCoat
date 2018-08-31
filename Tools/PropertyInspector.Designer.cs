namespace BlackCoat.Tools
{
    partial class PropertyInspector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._Inspector = new System.Windows.Forms.PropertyGrid();
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this._EditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ExportTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SplitContainer = new System.Windows.Forms.SplitContainer();
            this._SceneGraph = new System.Windows.Forms.TreeView();
            this._TopMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).BeginInit();
            this._SplitContainer.Panel1.SuspendLayout();
            this._SplitContainer.Panel2.SuspendLayout();
            this._SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _Inspector
            // 
            this._Inspector.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Inspector.Location = new System.Drawing.Point(0, 0);
            this._Inspector.Name = "_Inspector";
            this._Inspector.Size = new System.Drawing.Size(280, 642);
            this._Inspector.TabIndex = 0;
            this._Inspector.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.Inspector_SelectedGridItemChanged);
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._EditorToolStripMenuItem});
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(484, 24);
            this._MenuStrip.TabIndex = 1;
            this._MenuStrip.Text = "menuStrip1";
            // 
            // _EditorToolStripMenuItem
            // 
            this._EditorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._TopMostToolStripMenuItem,
            this._ExportTargetToolStripMenuItem,
            this.toolStripSeparator,
            this._CloseToolStripMenuItem});
            this._EditorToolStripMenuItem.Name = "_EditorToolStripMenuItem";
            this._EditorToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this._EditorToolStripMenuItem.Text = "Inspector";
            // 
            // _ExportTargetToolStripMenuItem
            // 
            this._ExportTargetToolStripMenuItem.Name = "_ExportTargetToolStripMenuItem";
            this._ExportTargetToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this._ExportTargetToolStripMenuItem.Text = "Export current Object";
            this._ExportTargetToolStripMenuItem.Click += new System.EventHandler(this.ExportTargetToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(183, 6);
            // 
            // _CloseToolStripMenuItem
            // 
            this._CloseToolStripMenuItem.Name = "_CloseToolStripMenuItem";
            this._CloseToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this._CloseToolStripMenuItem.Text = "Close";
            this._CloseToolStripMenuItem.Click += new System.EventHandler(this.HideToolStripMenuItem_Clicked);
            // 
            // _SplitContainer
            // 
            this._SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainer.Location = new System.Drawing.Point(0, 24);
            this._SplitContainer.Name = "_SplitContainer";
            // 
            // _SplitContainer.Panel1
            // 
            this._SplitContainer.Panel1.Controls.Add(this._SceneGraph);
            // 
            // _SplitContainer.Panel2
            // 
            this._SplitContainer.Panel2.Controls.Add(this._Inspector);
            this._SplitContainer.Size = new System.Drawing.Size(484, 642);
            this._SplitContainer.SplitterDistance = 200;
            this._SplitContainer.TabIndex = 2;
            // 
            // _SceneGraph
            // 
            this._SceneGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SceneGraph.HideSelection = false;
            this._SceneGraph.Location = new System.Drawing.Point(0, 0);
            this._SceneGraph.Name = "_SceneGraph";
            this._SceneGraph.Size = new System.Drawing.Size(200, 642);
            this._SceneGraph.TabIndex = 0;
            this._SceneGraph.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SceneGraph_NodeSelected);
            this._SceneGraph.Enter += new System.EventHandler(this.SceneGraph_GotFocus);
            // 
            // topMostToolStripMenuItem
            // 
            this._TopMostToolStripMenuItem.Name = "topMostToolStripMenuItem";
            this._TopMostToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this._TopMostToolStripMenuItem.Text = "Top Most";
            this._TopMostToolStripMenuItem.Click += new System.EventHandler(this.TopMostToolStripMenuItem_Clicked);
            // 
            // PropertyInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 666);
            this.Controls.Add(this._SplitContainer);
            this.Controls.Add(this._MenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "PropertyInspector";
            this.Text = "Property Inspector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertyEditor_FormClosing);
            this._MenuStrip.ResumeLayout(false);
            this._MenuStrip.PerformLayout();
            this._SplitContainer.Panel1.ResumeLayout(false);
            this._SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).EndInit();
            this._SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid _Inspector;
        private System.Windows.Forms.MenuStrip _MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _EditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _ExportTargetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem _CloseToolStripMenuItem;
        private System.Windows.Forms.SplitContainer _SplitContainer;
        private System.Windows.Forms.TreeView _SceneGraph;
        private System.Windows.Forms.ToolStripMenuItem _TopMostToolStripMenuItem;
    }
}