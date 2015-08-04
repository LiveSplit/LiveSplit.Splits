namespace LiveSplit.UI.Components
{
    partial class ColumnSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnSettings));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmbColumnType = new System.Windows.Forms.ComboBox();
            this.cmbComparison = new System.Windows.Forms.ComboBox();
            this.cmbTimingMethod = new System.Windows.Forms.ComboBox();
            this.btnRemoveColumn = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.groupColumn = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupColumn.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbColumnType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbComparison, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbTimingMethod, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnRemoveColumn, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveDown, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveUp, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.tableLayoutPanel1.SetColumnSpan(this.txtName, 3);
            this.txtName.Name = "txtName";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // cmbColumnType
            // 
            resources.ApplyResources(this.cmbColumnType, "cmbColumnType");
            this.tableLayoutPanel1.SetColumnSpan(this.cmbColumnType, 3);
            this.cmbColumnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColumnType.FormattingEnabled = true;
            this.cmbColumnType.Items.AddRange(new object[] {
            resources.GetString("cmbColumnType.Items"),
            resources.GetString("cmbColumnType.Items1"),
            resources.GetString("cmbColumnType.Items2"),
            resources.GetString("cmbColumnType.Items3"),
            resources.GetString("cmbColumnType.Items4"),
            resources.GetString("cmbColumnType.Items5")});
            this.cmbColumnType.Name = "cmbColumnType";
            this.cmbColumnType.SelectedIndexChanged += new System.EventHandler(this.cmbColumnType_SelectedIndexChanged);
            // 
            // cmbComparison
            // 
            resources.ApplyResources(this.cmbComparison, "cmbComparison");
            this.tableLayoutPanel1.SetColumnSpan(this.cmbComparison, 3);
            this.cmbComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComparison.FormattingEnabled = true;
            this.cmbComparison.Name = "cmbComparison";
            this.cmbComparison.SelectedIndexChanged += new System.EventHandler(this.cmbComparison_SelectedIndexChanged);
            // 
            // cmbTimingMethod
            // 
            resources.ApplyResources(this.cmbTimingMethod, "cmbTimingMethod");
            this.tableLayoutPanel1.SetColumnSpan(this.cmbTimingMethod, 3);
            this.cmbTimingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimingMethod.FormattingEnabled = true;
            this.cmbTimingMethod.Items.AddRange(new object[] {
            resources.GetString("cmbTimingMethod.Items"),
            resources.GetString("cmbTimingMethod.Items1"),
            resources.GetString("cmbTimingMethod.Items2")});
            this.cmbTimingMethod.Name = "cmbTimingMethod";
            this.cmbTimingMethod.SelectedIndexChanged += new System.EventHandler(this.cmbTimingMethod_SelectedIndexChanged);
            // 
            // btnRemoveColumn
            // 
            resources.ApplyResources(this.btnRemoveColumn, "btnRemoveColumn");
            this.btnRemoveColumn.Name = "btnRemoveColumn";
            this.btnRemoveColumn.UseVisualStyleBackColor = true;
            this.btnRemoveColumn.Click += new System.EventHandler(this.btnRemoveColumn_Click);
            // 
            // btnMoveDown
            // 
            resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // groupColumn
            // 
            resources.ApplyResources(this.groupColumn, "groupColumn");
            this.groupColumn.Controls.Add(this.tableLayoutPanel1);
            this.groupColumn.Name = "groupColumn";
            this.groupColumn.TabStop = false;
            // 
            // ColumnSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupColumn);
            this.Name = "ColumnSettings";
            this.Load += new System.EventHandler(this.ColumnSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupColumn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbTimingMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbColumnType;
        private System.Windows.Forms.ComboBox cmbComparison;
        private System.Windows.Forms.Button btnRemoveColumn;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
    }
}