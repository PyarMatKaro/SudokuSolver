namespace Sudoku
{
    partial class GridForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.lblCells = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbJigsaw = new System.Windows.Forms.RadioButton();
            this.rbKiller = new System.Windows.Forms.RadioButton();
            this.rbNormal = new System.Windows.Forms.RadioButton();
            this.cbMajorDiagonal = new System.Windows.Forms.CheckBox();
            this.cbMinorDiagonal = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Box width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Box height";
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(83, 12);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(100, 20);
            this.tbWidth.TabIndex = 2;
            this.tbWidth.TextChanged += new System.EventHandler(this.WidthHeight_TextChanged);
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(83, 38);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(100, 20);
            this.tbHeight.TabIndex = 3;
            this.tbHeight.TextChanged += new System.EventHandler(this.WidthHeight_TextChanged);
            // 
            // lblCells
            // 
            this.lblCells.AutoSize = true;
            this.lblCells.Location = new System.Drawing.Point(12, 73);
            this.lblCells.Name = "lblCells";
            this.lblCells.Size = new System.Drawing.Size(204, 13);
            this.lblCells.TabIndex = 4;
            this.lblCells.Text = "This gives ? cells to a row, column or box.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbJigsaw);
            this.groupBox1.Controls.Add(this.rbKiller);
            this.groupBox1.Controls.Add(this.rbNormal);
            this.groupBox1.Location = new System.Drawing.Point(15, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
            // 
            // rbJigsaw
            // 
            this.rbJigsaw.AutoSize = true;
            this.rbJigsaw.Location = new System.Drawing.Point(6, 65);
            this.rbJigsaw.Name = "rbJigsaw";
            this.rbJigsaw.Size = new System.Drawing.Size(57, 17);
            this.rbJigsaw.TabIndex = 2;
            this.rbJigsaw.TabStop = true;
            this.rbJigsaw.Text = "Jigsaw";
            this.rbJigsaw.UseVisualStyleBackColor = true;
            // 
            // rbKiller
            // 
            this.rbKiller.AutoSize = true;
            this.rbKiller.Location = new System.Drawing.Point(6, 42);
            this.rbKiller.Name = "rbKiller";
            this.rbKiller.Size = new System.Drawing.Size(47, 17);
            this.rbKiller.TabIndex = 1;
            this.rbKiller.TabStop = true;
            this.rbKiller.Text = "Killer";
            this.rbKiller.UseVisualStyleBackColor = true;
            // 
            // rbNormal
            // 
            this.rbNormal.AutoSize = true;
            this.rbNormal.Location = new System.Drawing.Point(6, 19);
            this.rbNormal.Name = "rbNormal";
            this.rbNormal.Size = new System.Drawing.Size(58, 17);
            this.rbNormal.TabIndex = 0;
            this.rbNormal.TabStop = true;
            this.rbNormal.Text = "Normal";
            this.rbNormal.UseVisualStyleBackColor = true;
            // 
            // cbMajorDiagonal
            // 
            this.cbMajorDiagonal.AutoSize = true;
            this.cbMajorDiagonal.Location = new System.Drawing.Point(15, 100);
            this.cbMajorDiagonal.Name = "cbMajorDiagonal";
            this.cbMajorDiagonal.Size = new System.Drawing.Size(106, 17);
            this.cbMajorDiagonal.TabIndex = 6;
            this.cbMajorDiagonal.Text = "\\  Major diagonal";
            this.cbMajorDiagonal.UseVisualStyleBackColor = true;
            // 
            // cbMinorDiagonal
            // 
            this.cbMinorDiagonal.AutoSize = true;
            this.cbMinorDiagonal.Location = new System.Drawing.Point(16, 123);
            this.cbMinorDiagonal.Name = "cbMinorDiagonal";
            this.cbMinorDiagonal.Size = new System.Drawing.Size(108, 17);
            this.cbMinorDiagonal.TabIndex = 6;
            this.cbMinorDiagonal.Text = "/  Minor Diagonal";
            this.cbMinorDiagonal.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(37, 255);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(118, 255);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // GridForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(230, 286);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbMinorDiagonal);
            this.Controls.Add(this.cbMajorDiagonal);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblCells);
            this.Controls.Add(this.tbHeight);
            this.Controls.Add(this.tbWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GridForm";
            this.Text = "GridForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.Label lblCells;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbJigsaw;
        private System.Windows.Forms.RadioButton rbKiller;
        private System.Windows.Forms.RadioButton rbNormal;
        private System.Windows.Forms.CheckBox cbMajorDiagonal;
        private System.Windows.Forms.CheckBox cbMinorDiagonal;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}