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
            this._InspectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._TopMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ExportTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ExtrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._CoreUpdateWithoutFocusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._RebuildSceneGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._RemoveEntitiyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._RenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._EmitterTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._LightsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._LoadLightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SaveLightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._AddLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._DuplicateLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SplitContainer = new System.Windows.Forms.SplitContainer();
            this._SceneGraph = new System.Windows.Forms.TreeView();
            this._OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
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
            this._Inspector.SelectedObjectsChanged += new System.EventHandler(this.InspectorSelectedObjectsChanged);
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._InspectorToolStripMenuItem,
            this._ExtrasToolStripMenuItem,
            this._LightsToolStripMenuItem});
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(484, 24);
            this._MenuStrip.TabIndex = 1;
            this._MenuStrip.Text = "menuStrip1";
            // 
            // _InspectorToolStripMenuItem
            // 
            this._InspectorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._TopMostToolStripMenuItem,
            this._ExportTargetToolStripMenuItem,
            this.toolStripSeparator,
            this._CloseToolStripMenuItem});
            this._InspectorToolStripMenuItem.Name = "_InspectorToolStripMenuItem";
            this._InspectorToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this._InspectorToolStripMenuItem.Text = "Inspector";
            // 
            // _TopMostToolStripMenuItem
            // 
            this._TopMostToolStripMenuItem.Name = "_TopMostToolStripMenuItem";
            this._TopMostToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this._TopMostToolStripMenuItem.Text = "Top Most";
            this._TopMostToolStripMenuItem.Click += new System.EventHandler(this.TopMostToolStripMenuItem_Clicked);
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
            // _ExtrasToolStripMenuItem
            // 
            this._ExtrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._CoreUpdateWithoutFocusToolStripMenuItem,
            this._RebuildSceneGraphToolStripMenuItem,
            this.toolStripSeparator1,
            this._RemoveEntitiyToolStripMenuItem,
            this._RenderToolStripMenuItem,
            this._EmitterTriggerToolStripMenuItem});
            this._ExtrasToolStripMenuItem.Name = "_ExtrasToolStripMenuItem";
            this._ExtrasToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this._ExtrasToolStripMenuItem.Text = "Extras";
            // 
            // _CoreUpdateWithoutFocusToolStripMenuItem
            // 
            this._CoreUpdateWithoutFocusToolStripMenuItem.Name = "_CoreUpdateWithoutFocusToolStripMenuItem";
            this._CoreUpdateWithoutFocusToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this._CoreUpdateWithoutFocusToolStripMenuItem.Text = "Core Update without Focus";
            this._CoreUpdateWithoutFocusToolStripMenuItem.Click += new System.EventHandler(this.CoreUpdateWithoutFocusToolStripMenuItemToolStripMenuItemClicked);
            // 
            // _RebuildSceneGraphToolStripMenuItem
            // 
            this._RebuildSceneGraphToolStripMenuItem.Name = "_RebuildSceneGraphToolStripMenuItem";
            this._RebuildSceneGraphToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this._RebuildSceneGraphToolStripMenuItem.Text = "Rebuild Scene Graph";
            this._RebuildSceneGraphToolStripMenuItem.Click += new System.EventHandler(this.RebuildSceneGraphToolStripMenuItemClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(281, 6);
            // 
            // _RemoveEntitiyToolStripMenuItem
            // 
            this._RemoveEntitiyToolStripMenuItem.Name = "_RemoveEntitiyToolStripMenuItem";
            this._RemoveEntitiyToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this._RemoveEntitiyToolStripMenuItem.Text = "IEntity - Remove From Parent Container";
            this._RemoveEntitiyToolStripMenuItem.Click += new System.EventHandler(this.RemoveEntitiyToolStripMenuItemClicked);
            // 
            // _RenderToolStripMenuItem
            // 
            this._RenderToolStripMenuItem.Name = "_RenderToolStripMenuItem";
            this._RenderToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this._RenderToolStripMenuItem.Text = "PrerenderedContainer - Render";
            this._RenderToolStripMenuItem.Click += new System.EventHandler(this.RenderToolStripMenuItemClicked);
            // 
            // _EmitterTriggerToolStripMenuItem
            // 
            this._EmitterTriggerToolStripMenuItem.Name = "_EmitterTriggerToolStripMenuItem";
            this._EmitterTriggerToolStripMenuItem.Size = new System.Drawing.Size(284, 22);
            this._EmitterTriggerToolStripMenuItem.Text = "ITriggerEmitter - Trigger";
            this._EmitterTriggerToolStripMenuItem.Click += new System.EventHandler(this.EmitterTriggerToolStripMenuItemClicked);
            // 
            // _LightsToolStripMenuItem
            // 
            this._LightsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._LoadLightMapToolStripMenuItem,
            this._SaveLightMapToolStripMenuItem,
            this.toolStripSeparator2,
            this._AddLightToolStripMenuItem,
            this._DuplicateLightToolStripMenuItem});
            this._LightsToolStripMenuItem.Name = "_LightsToolStripMenuItem";
            this._LightsToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this._LightsToolStripMenuItem.Text = "Lights";
            // 
            // _LoadLightMapToolStripMenuItem
            // 
            this._LoadLightMapToolStripMenuItem.Name = "_LoadLightMapToolStripMenuItem";
            this._LoadLightMapToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this._LoadLightMapToolStripMenuItem.Text = "Load";
            this._LoadLightMapToolStripMenuItem.Click += new System.EventHandler(this.LoadLightMapToolStripMenuItemClicked);
            // 
            // _SaveLightMapToolStripMenuItem
            // 
            this._SaveLightMapToolStripMenuItem.Name = "_SaveLightMapToolStripMenuItem";
            this._SaveLightMapToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this._SaveLightMapToolStripMenuItem.Text = "Save";
            this._SaveLightMapToolStripMenuItem.Click += new System.EventHandler(this.SaveLightMapToolStripMenuItemClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // _AddLightToolStripMenuItem
            // 
            this._AddLightToolStripMenuItem.Name = "_AddLightToolStripMenuItem";
            this._AddLightToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this._AddLightToolStripMenuItem.Text = "Add Light";
            this._AddLightToolStripMenuItem.Click += new System.EventHandler(this.AddLightToolStripMenuItemClicked);
            // 
            // _DuplicateLightToolStripMenuItem
            // 
            this._DuplicateLightToolStripMenuItem.Name = "_DuplicateLightToolStripMenuItem";
            this._DuplicateLightToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this._DuplicateLightToolStripMenuItem.Text = "Duplicate Light";
            this._DuplicateLightToolStripMenuItem.Click += new System.EventHandler(this.DuplicateLightToolStripMenuItemClicked);
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
            // _OpenFileDialog
            // 
            this._OpenFileDialog.DefaultExt = "bcl";
            this._OpenFileDialog.Filter = "Black Coat Lightmap|*.bcl|All Files|*.*";
            // 
            // _SaveFileDialog
            // 
            this._SaveFileDialog.DefaultExt = "bcl";
            this._SaveFileDialog.Filter = "Black Coat Lightmap|*.bcl|All Files|*.*";
            this._SaveFileDialog.OverwritePrompt = false;
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
        private System.Windows.Forms.ToolStripMenuItem _InspectorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _ExportTargetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem _CloseToolStripMenuItem;
        private System.Windows.Forms.SplitContainer _SplitContainer;
        private System.Windows.Forms.TreeView _SceneGraph;
        private System.Windows.Forms.ToolStripMenuItem _TopMostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _ExtrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _RemoveEntitiyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _RebuildSceneGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _RenderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _LightsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _LoadLightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SaveLightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _AddLightToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog _OpenFileDialog;
        private System.Windows.Forms.SaveFileDialog _SaveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem _DuplicateLightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _CoreUpdateWithoutFocusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _EmitterTriggerToolStripMenuItem;
    }
}