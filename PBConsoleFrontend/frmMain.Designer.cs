namespace Austin.PBConsoleFrontend
{
    partial class frmMain
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
            this.switches = new System.Windows.Forms.CheckedListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.lblInstructionsPerSec = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // switches
            // 
            this.switches.FormattingEnabled = true;
            this.switches.Items.AddRange(new object[] {
            "SW7",
            "SW6",
            "SW5",
            "SW4",
            "SW3",
            "SW2",
            "SW1",
            "SW0"});
            this.switches.Location = new System.Drawing.Point(12, 12);
            this.switches.Name = "switches";
            this.switches.Size = new System.Drawing.Size(120, 139);
            this.switches.TabIndex = 0;
            this.switches.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.switches_ItemCheck);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 157);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 186);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtSrc
            // 
            this.txtSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSrc.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrc.Location = new System.Drawing.Point(138, 12);
            this.txtSrc.Multiline = true;
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSrc.Size = new System.Drawing.Size(487, 396);
            this.txtSrc.TabIndex = 2;
            this.txtSrc.WordWrap = false;
            // 
            // lblInstructionsPerSec
            // 
            this.lblInstructionsPerSec.AutoSize = true;
            this.lblInstructionsPerSec.Location = new System.Drawing.Point(12, 226);
            this.lblInstructionsPerSec.Name = "lblInstructionsPerSec";
            this.lblInstructionsPerSec.Size = new System.Drawing.Size(21, 13);
            this.lblInstructionsPerSec.TabIndex = 3;
            this.lblInstructionsPerSec.Text = "{0}";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 420);
            this.Controls.Add(this.lblInstructionsPerSec);
            this.Controls.Add(this.txtSrc);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.switches);
            this.Name = "frmMain";
            this.Text = "PicoBlaze on Nexys2 Simulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox switches;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.Label lblInstructionsPerSec;
    }
}