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
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 353);
            this.Controls.Add(this.switches);
            this.Name = "frmMain";
            this.Text = "PicoBlaze on Nexys2 Simulator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox switches;
    }
}