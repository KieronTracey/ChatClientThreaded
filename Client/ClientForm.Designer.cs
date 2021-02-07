
namespace Client
{
    partial class ClientForm
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
            this.MessageWindow = new System.Windows.Forms.TextBox();
            this.InputField = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.NickNameBox = new System.Windows.Forms.TextBox();
            this.ChangeNicknameButton = new System.Windows.Forms.Button();
            this.WhisperBox1 = new System.Windows.Forms.TextBox();
            this.WhisperButton = new System.Windows.Forms.Button();
            this.ConnectionsLabel = new System.Windows.Forms.Label();
            this.ClientList = new System.Windows.Forms.TextBox();
            this.PlayHangmanButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MessageWindow
            // 
            this.MessageWindow.Location = new System.Drawing.Point(12, 6);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.Size = new System.Drawing.Size(777, 333);
            this.MessageWindow.TabIndex = 0;
            this.MessageWindow.TextChanged += new System.EventHandler(this.MessageWindow_TextChanged);
            // 
            // InputField
            // 
            this.InputField.Location = new System.Drawing.Point(16, 347);
            this.InputField.Multiline = true;
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(670, 89);
            this.InputField.TabIndex = 1;
            this.InputField.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(692, 347);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(96, 89);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(795, 12);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(112, 35);
            this.DisconnectButton.TabIndex = 3;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // NickNameBox
            // 
            this.NickNameBox.Location = new System.Drawing.Point(795, 114);
            this.NickNameBox.Name = "NickNameBox";
            this.NickNameBox.Size = new System.Drawing.Size(121, 20);
            this.NickNameBox.TabIndex = 4;
            this.NickNameBox.Text = "Enter Nickname";
            this.NickNameBox.TextChanged += new System.EventHandler(this.NickNameBox_TextChanged);
            // 
            // ChangeNicknameButton
            // 
            this.ChangeNicknameButton.Location = new System.Drawing.Point(795, 85);
            this.ChangeNicknameButton.Name = "ChangeNicknameButton";
            this.ChangeNicknameButton.Size = new System.Drawing.Size(121, 23);
            this.ChangeNicknameButton.TabIndex = 5;
            this.ChangeNicknameButton.Text = "Change Nickname";
            this.ChangeNicknameButton.UseVisualStyleBackColor = true;
            this.ChangeNicknameButton.Click += new System.EventHandler(this.ChangeNicknameButton_Click);
            // 
            // WhisperBox1
            // 
            this.WhisperBox1.Location = new System.Drawing.Point(795, 388);
            this.WhisperBox1.Multiline = true;
            this.WhisperBox1.Name = "WhisperBox1";
            this.WhisperBox1.Size = new System.Drawing.Size(121, 48);
            this.WhisperBox1.TabIndex = 6;
            this.WhisperBox1.Text = "Enter Whisper Recipient";
            this.WhisperBox1.TextChanged += new System.EventHandler(this.WhisperBox1_TextChanged);
            // 
            // WhisperButton
            // 
            this.WhisperButton.Location = new System.Drawing.Point(795, 347);
            this.WhisperButton.Name = "WhisperButton";
            this.WhisperButton.Size = new System.Drawing.Size(121, 35);
            this.WhisperButton.TabIndex = 7;
            this.WhisperButton.Text = "Whisper";
            this.WhisperButton.UseVisualStyleBackColor = true;
            this.WhisperButton.Click += new System.EventHandler(this.WhisperButton_Click);
            // 
            // ConnectionsLabel
            // 
            this.ConnectionsLabel.AutoSize = true;
            this.ConnectionsLabel.Location = new System.Drawing.Point(820, 149);
            this.ConnectionsLabel.Name = "ConnectionsLabel";
            this.ConnectionsLabel.Size = new System.Drawing.Size(66, 13);
            this.ConnectionsLabel.TabIndex = 8;
            this.ConnectionsLabel.Text = "Connections";
            this.ConnectionsLabel.Click += new System.EventHandler(this.ConnectionsLabel_Click);
            // 
            // ClientList
            // 
            this.ClientList.Location = new System.Drawing.Point(795, 165);
            this.ClientList.Multiline = true;
            this.ClientList.Name = "ClientList";
            this.ClientList.Size = new System.Drawing.Size(121, 174);
            this.ClientList.TabIndex = 9;
            this.ClientList.TextChanged += new System.EventHandler(this.ClientList_TextChanged);
            // 
            // PlayHangmanButton
            // 
            this.PlayHangmanButton.Location = new System.Drawing.Point(795, 53);
            this.PlayHangmanButton.Name = "PlayHangmanButton";
            this.PlayHangmanButton.Size = new System.Drawing.Size(112, 23);
            this.PlayHangmanButton.TabIndex = 10;
            this.PlayHangmanButton.Text = "Play Hangman";
            this.PlayHangmanButton.UseVisualStyleBackColor = true;
            this.PlayHangmanButton.Click += new System.EventHandler(this.PlayHangmanButton_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 450);
            this.Controls.Add(this.PlayHangmanButton);
            this.Controls.Add(this.ClientList);
            this.Controls.Add(this.ConnectionsLabel);
            this.Controls.Add(this.WhisperButton);
            this.Controls.Add(this.WhisperBox1);
            this.Controls.Add(this.ChangeNicknameButton);
            this.Controls.Add(this.NickNameBox);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.MessageWindow);
            this.Name = "ClientForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.TextBox NickNameBox;
        private System.Windows.Forms.Button ChangeNicknameButton;
        private System.Windows.Forms.TextBox WhisperBox1;
        private System.Windows.Forms.Button WhisperButton;
        private System.Windows.Forms.Label ConnectionsLabel;
        private System.Windows.Forms.TextBox ClientList;
        private System.Windows.Forms.Button PlayHangmanButton;
    }
}