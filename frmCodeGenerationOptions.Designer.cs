namespace BestPracticesCodeGenerator
{
    partial class frmCodeGenerationOptions
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
            this.BTN_Generate = new System.Windows.Forms.Button();
            this.BTN_Cancel = new System.Windows.Forms.Button();
            this.SEL_GenerateCreateUseCase = new System.Windows.Forms.CheckBox();
            this.SEL_GenerateUpdateUseCase = new System.Windows.Forms.CheckBox();
            this.SEL_GenerateDeleteUseCase = new System.Windows.Forms.CheckBox();
            this.GRD_Properties = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.GRD_Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // BTN_Generate
            // 
            this.BTN_Generate.Location = new System.Drawing.Point(798, 500);
            this.BTN_Generate.Name = "BTN_Generate";
            this.BTN_Generate.Size = new System.Drawing.Size(105, 33);
            this.BTN_Generate.TabIndex = 0;
            this.BTN_Generate.Text = "Generate";
            this.BTN_Generate.UseVisualStyleBackColor = true;
            this.BTN_Generate.Click += new System.EventHandler(this.BTN_Generate_Click);
            // 
            // BTN_Cancel
            // 
            this.BTN_Cancel.Location = new System.Drawing.Point(930, 500);
            this.BTN_Cancel.Name = "BTN_Cancel";
            this.BTN_Cancel.Size = new System.Drawing.Size(105, 33);
            this.BTN_Cancel.TabIndex = 1;
            this.BTN_Cancel.Text = "Cancel";
            this.BTN_Cancel.UseVisualStyleBackColor = true;
            this.BTN_Cancel.Click += new System.EventHandler(this.BTN_Cancel_Click);
            // 
            // SEL_GenerateCreateUseCase
            // 
            this.SEL_GenerateCreateUseCase.AutoSize = true;
            this.SEL_GenerateCreateUseCase.Location = new System.Drawing.Point(807, 208);
            this.SEL_GenerateCreateUseCase.Name = "SEL_GenerateCreateUseCase";
            this.SEL_GenerateCreateUseCase.Size = new System.Drawing.Size(147, 17);
            this.SEL_GenerateCreateUseCase.TabIndex = 2;
            this.SEL_GenerateCreateUseCase.Text = "Generate CreateUseCase";
            this.SEL_GenerateCreateUseCase.UseVisualStyleBackColor = true;
            // 
            // SEL_GenerateUpdateUseCase
            // 
            this.SEL_GenerateUpdateUseCase.AutoSize = true;
            this.SEL_GenerateUpdateUseCase.Location = new System.Drawing.Point(807, 249);
            this.SEL_GenerateUpdateUseCase.Name = "SEL_GenerateUpdateUseCase";
            this.SEL_GenerateUpdateUseCase.Size = new System.Drawing.Size(151, 17);
            this.SEL_GenerateUpdateUseCase.TabIndex = 3;
            this.SEL_GenerateUpdateUseCase.Text = "Generate UpdateUseCase";
            this.SEL_GenerateUpdateUseCase.UseVisualStyleBackColor = true;
            // 
            // SEL_GenerateDeleteUseCase
            // 
            this.SEL_GenerateDeleteUseCase.AutoSize = true;
            this.SEL_GenerateDeleteUseCase.Location = new System.Drawing.Point(807, 288);
            this.SEL_GenerateDeleteUseCase.Name = "SEL_GenerateDeleteUseCase";
            this.SEL_GenerateDeleteUseCase.Size = new System.Drawing.Size(147, 17);
            this.SEL_GenerateDeleteUseCase.TabIndex = 4;
            this.SEL_GenerateDeleteUseCase.Text = "Generate DeleteUseCase";
            this.SEL_GenerateDeleteUseCase.UseVisualStyleBackColor = true;
            // 
            // GRD_Properties
            // 
            this.GRD_Properties.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GRD_Properties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GRD_Properties.Location = new System.Drawing.Point(26, 42);
            this.GRD_Properties.Name = "GRD_Properties";
            this.GRD_Properties.Size = new System.Drawing.Size(653, 380);
            this.GRD_Properties.TabIndex = 5;
            // 
            // frmCodeGenerationOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 545);
            this.Controls.Add(this.GRD_Properties);
            this.Controls.Add(this.SEL_GenerateDeleteUseCase);
            this.Controls.Add(this.SEL_GenerateUpdateUseCase);
            this.Controls.Add(this.SEL_GenerateCreateUseCase);
            this.Controls.Add(this.BTN_Cancel);
            this.Controls.Add(this.BTN_Generate);
            this.Name = "frmCodeGenerationOptions";
            this.Text = "frmCodeGenerationOptions";
            this.Shown += new System.EventHandler(this.frmCodeGenerationOptions_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.GRD_Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_Generate;
        private System.Windows.Forms.Button BTN_Cancel;
        private System.Windows.Forms.CheckBox SEL_GenerateCreateUseCase;
        private System.Windows.Forms.CheckBox SEL_GenerateUpdateUseCase;
        private System.Windows.Forms.CheckBox SEL_GenerateDeleteUseCase;
        private System.Windows.Forms.DataGridView GRD_Properties;
    }
}