namespace TelegramBotWantedCarsList
{
    partial class BotOptions
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
            this.startButton = new System.Windows.Forms.Button();
            this.tokenTextField = new System.Windows.Forms.TextBox();
            this.tokenTextLabel = new System.Windows.Forms.Label();
            this.pathToStorageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(38, 99);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Go";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // tokenTextField
            // 
            this.tokenTextField.Location = new System.Drawing.Point(38, 59);
            this.tokenTextField.Name = "tokenTextField";
            this.tokenTextField.Size = new System.Drawing.Size(413, 20);
            this.tokenTextField.TabIndex = 1;
            // 
            // tokenTextLabel
            // 
            this.tokenTextLabel.AutoSize = true;
            this.tokenTextLabel.Location = new System.Drawing.Point(38, 43);
            this.tokenTextLabel.Name = "tokenTextLabel";
            this.tokenTextLabel.Size = new System.Drawing.Size(81, 13);
            this.tokenTextLabel.TabIndex = 2;
            this.tokenTextLabel.Text = "Token telegram";
            // 
            // pathToStorageLabel
            // 
            this.pathToStorageLabel.AutoSize = true;
            this.pathToStorageLabel.Location = new System.Drawing.Point(38, 138);
            this.pathToStorageLabel.Name = "pathToStorageLabel";
            this.pathToStorageLabel.Size = new System.Drawing.Size(395, 13);
            this.pathToStorageLabel.TabIndex = 3;
            this.pathToStorageLabel.Text = "Path to Storage: C:\\Users\\singlefig-ap\\source\\repos\\TelegramBotWantedCarsList";
            // 
            // BotOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 215);
            this.Controls.Add(this.pathToStorageLabel);
            this.Controls.Add(this.tokenTextLabel);
            this.Controls.Add(this.tokenTextField);
            this.Controls.Add(this.startButton);
            this.Name = "BotOptions";
            this.Text = "TelegramBot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TextBox tokenTextField;
        private System.Windows.Forms.Label tokenTextLabel;
        private System.Windows.Forms.Label pathToStorageLabel;
    }
}

