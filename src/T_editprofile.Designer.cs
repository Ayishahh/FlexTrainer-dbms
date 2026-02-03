namespace DB_phase2_project
{
    partial class T_editprofile
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
            button1 = new Button();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label15 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(732, 446);
            button1.Name = "button1";
            button1.Size = new Size(111, 29);
            button1.TabIndex = 42;
            button1.Text = "Save changes";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(191, 109);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(333, 27);
            textBox5.TabIndex = 38;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(191, 150);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(333, 27);
            textBox4.TabIndex = 37;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(191, 197);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(333, 27);
            textBox3.TabIndex = 36;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(191, 247);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(333, 27);
            textBox2.TabIndex = 35;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(191, 66);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(333, 27);
            textBox1.TabIndex = 34;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(28, 204);
            label15.Name = "label15";
            label15.Size = new Size(96, 20);
            label15.TabIndex = 33;
            label15.Text = "Date Of Birth";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(28, 254);
            label11.Name = "label11";
            label11.Size = new Size(91, 20);
            label11.TabIndex = 29;
            label11.Text = "Current Gym";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(28, 153);
            label10.Name = "label10";
            label10.Size = new Size(70, 20);
            label10.TabIndex = 28;
            label10.Text = "Password";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(28, 109);
            label9.Name = "label9";
            label9.Size = new Size(46, 20);
            label9.TabIndex = 27;
            label9.Text = "Email";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(28, 69);
            label8.Name = "label8";
            label8.Size = new Size(75, 20);
            label8.TabIndex = 26;
            label8.Text = "Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Black", 16F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(3, -1);
            label2.Name = "label2";
            label2.Size = new Size(166, 37);
            label2.TabIndex = 25;
            label2.Text = "Edit Profile";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(548, 15);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(339, 387);
            pictureBox1.TabIndex = 43;
            pictureBox1.TabStop = false;
            // 
            // T_editprofile
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(899, 500);
            Controls.Add(pictureBox1);
            Controls.Add(button1);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label15);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label2);
            Name = "T_editprofile";
            Text = "Form7";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox5;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private Label label15;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label2;
        private PictureBox pictureBox1;
    }
}