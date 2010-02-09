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
            this.chkUseFrameBuffer = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSleepAfter = new System.Windows.Forms.NumericUpDown();
            this.leds = new Austin.PBConsoleFrontend.LEDs();
            ((System.ComponentModel.ISupportInitialize)(this.txtSleepAfter)).BeginInit();
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
            this.btnStart.Location = new System.Drawing.Point(12, 220);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(12, 249);
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
            this.txtSrc.Location = new System.Drawing.Point(199, 12);
            this.txtSrc.Multiline = true;
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSrc.Size = new System.Drawing.Size(426, 396);
            this.txtSrc.TabIndex = 2;
            this.txtSrc.WordWrap = false;
            // 
            // lblInstructionsPerSec
            // 
            this.lblInstructionsPerSec.AutoSize = true;
            this.lblInstructionsPerSec.Location = new System.Drawing.Point(12, 289);
            this.lblInstructionsPerSec.Name = "lblInstructionsPerSec";
            this.lblInstructionsPerSec.Size = new System.Drawing.Size(21, 13);
            this.lblInstructionsPerSec.TabIndex = 3;
            this.lblInstructionsPerSec.Text = "{0}";
            // 
            // chkUseFrameBuffer
            // 
            this.chkUseFrameBuffer.AutoSize = true;
            this.chkUseFrameBuffer.Location = new System.Drawing.Point(12, 197);
            this.chkUseFrameBuffer.Name = "chkUseFrameBuffer";
            this.chkUseFrameBuffer.Size = new System.Drawing.Size(104, 17);
            this.chkUseFrameBuffer.TabIndex = 4;
            this.chkUseFrameBuffer.Text = "Use frame buffer";
            this.chkUseFrameBuffer.UseVisualStyleBackColor = true;
            this.chkUseFrameBuffer.CheckedChanged += new System.EventHandler(this.chkUseFrameBuffer_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 315);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Sleep after ticks:";
            // 
            // txtSleepAfter
            // 
            this.txtSleepAfter.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtSleepAfter.Location = new System.Drawing.Point(15, 331);
            this.txtSleepAfter.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtSleepAfter.Name = "txtSleepAfter";
            this.txtSleepAfter.Size = new System.Drawing.Size(86, 20);
            this.txtSleepAfter.TabIndex = 6;
            // 
            // leds
            // 
            this.leds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leds.Location = new System.Drawing.Point(12, 157);
            this.leds.Name = "leds";
            this.leds.Size = new System.Drawing.Size(169, 34);
            this.leds.TabIndex = 7;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 420);
            this.Controls.Add(this.leds);
            this.Controls.Add(this.txtSleepAfter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUseFrameBuffer);
            this.Controls.Add(this.lblInstructionsPerSec);
            this.Controls.Add(this.txtSrc);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.switches);
            this.Name = "frmMain";
            this.Text = "PicoBlaze on Nexys2 Simulator";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.txtSleepAfter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox switches;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.Label lblInstructionsPerSec;
        private System.Windows.Forms.CheckBox chkUseFrameBuffer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtSleepAfter;
        private LEDs leds;
    }
}