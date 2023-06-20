namespace merge_test_program_keypoint
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            blocksize_scr = new HScrollBar();
            label1 = new Label();
            blocksize_text = new TextBox();
            c_text = new TextBox();
            label2 = new Label();
            c_scr = new HScrollBar();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            openFileDialog1 = new OpenFileDialog();
            pictureBox4 = new PictureBox();
            label6 = new Label();
            pictureBox5 = new PictureBox();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            pictureBox6 = new PictureBox();
            pictureBox7 = new PictureBox();
            label10 = new Label();
            pictureBox8 = new PictureBox();
            keypoint_name2 = new TextBox();
            keypoint_name1 = new TextBox();
            pictureBox9 = new PictureBox();
            label11 = new Label();
            label12 = new Label();
            pictureBox10 = new PictureBox();
            label13 = new Label();
            pictureBox11 = new PictureBox();
            pictureBox12 = new PictureBox();
            label14 = new Label();
            surf_threshold_scr = new HScrollBar();
            surf_threshold_value = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(33, 56);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(446, 223);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(501, 56);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(446, 223);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(986, 56);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(446, 223);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(1817, 73);
            button1.Name = "button1";
            button1.Size = new Size(187, 49);
            button1.TabIndex = 3;
            button1.Text = "folder open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(2031, 73);
            button2.Name = "button2";
            button2.Size = new Size(187, 49);
            button2.TabIndex = 4;
            button2.Text = "image show";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // blocksize_scr
            // 
            blocksize_scr.Location = new Point(1822, 161);
            blocksize_scr.Name = "blocksize_scr";
            blocksize_scr.Size = new Size(311, 24);
            blocksize_scr.TabIndex = 5;
            blocksize_scr.Scroll += blocksize_scr_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1822, 134);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 6;
            label1.Text = "blocksize";
            // 
            // blocksize_text
            // 
            blocksize_text.Location = new Point(2136, 161);
            blocksize_text.Name = "blocksize_text";
            blocksize_text.Size = new Size(100, 23);
            blocksize_text.TabIndex = 7;
            // 
            // c_text
            // 
            c_text.Location = new Point(2136, 234);
            c_text.Name = "c_text";
            c_text.Size = new Size(100, 23);
            c_text.TabIndex = 10;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1822, 207);
            label2.Name = "label2";
            label2.Size = new Size(13, 15);
            label2.TabIndex = 9;
            label2.Text = "c";
            // 
            // c_scr
            // 
            c_scr.Location = new Point(1822, 234);
            c_scr.Name = "c_scr";
            c_scr.Size = new Size(311, 24);
            c_scr.TabIndex = 8;
            c_scr.Scroll += c_scr_Scroll;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(33, 38);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 11;
            label3.Text = "original";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(986, 29);
            label4.Name = "label4";
            label4.Size = new Size(89, 15);
            label4.TabIndex = 12;
            label4.Text = "adaptive binary";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(35, 599);
            label5.Name = "label5";
            label5.Size = new Size(64, 15);
            label5.TabIndex = 13;
            label5.Text = "keypoint 1";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(35, 626);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(468, 358);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 14;
            pictureBox4.TabStop = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(501, 38);
            label6.Name = "label6";
            label6.Size = new Size(56, 15);
            label6.TabIndex = 15;
            label6.Text = "grayscale";
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new Point(33, 332);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(446, 223);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 17;
            pictureBox5.TabStop = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(527, 599);
            label7.Name = "label7";
            label7.Size = new Size(64, 15);
            label7.TabIndex = 16;
            label7.Text = "keypoint 2";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(501, 314);
            label8.Name = "label8";
            label8.Size = new Size(56, 15);
            label8.TabIndex = 21;
            label8.Text = "grayscale";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(986, 305);
            label9.Name = "label9";
            label9.Size = new Size(89, 15);
            label9.TabIndex = 20;
            label9.Text = "adaptive binary";
            // 
            // pictureBox6
            // 
            pictureBox6.Location = new Point(501, 332);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(446, 223);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 19;
            pictureBox6.TabStop = false;
            // 
            // pictureBox7
            // 
            pictureBox7.Location = new Point(986, 332);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(446, 223);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 18;
            pictureBox7.TabStop = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(33, 314);
            label10.Name = "label10";
            label10.Size = new Size(47, 15);
            label10.TabIndex = 22;
            label10.Text = "original";
            // 
            // pictureBox8
            // 
            pictureBox8.Location = new Point(527, 626);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(420, 358);
            pictureBox8.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox8.TabIndex = 23;
            pictureBox8.TabStop = false;
            // 
            // keypoint_name2
            // 
            keypoint_name2.Location = new Point(618, 591);
            keypoint_name2.Name = "keypoint_name2";
            keypoint_name2.Size = new Size(261, 23);
            keypoint_name2.TabIndex = 24;
            // 
            // keypoint_name1
            // 
            keypoint_name1.Location = new Point(134, 596);
            keypoint_name1.Name = "keypoint_name1";
            keypoint_name1.Size = new Size(277, 23);
            keypoint_name1.TabIndex = 25;
            // 
            // pictureBox9
            // 
            pictureBox9.Location = new Point(986, 626);
            pictureBox9.Name = "pictureBox9";
            pictureBox9.Size = new Size(446, 358);
            pictureBox9.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox9.TabIndex = 26;
            pictureBox9.TabStop = false;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(986, 594);
            label11.Name = "label11";
            label11.Size = new Size(108, 15);
            label11.TabIndex = 27;
            label11.Text = "keypoint matching";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(1446, 332);
            label12.Name = "label12";
            label12.Size = new Size(59, 15);
            label12.TabIndex = 30;
            label12.Text = "wrap_img";
            // 
            // pictureBox10
            // 
            pictureBox10.Location = new Point(1446, 364);
            pictureBox10.Name = "pictureBox10";
            pictureBox10.Size = new Size(380, 287);
            pictureBox10.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox10.TabIndex = 29;
            pictureBox10.TabStop = false;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(1446, 665);
            label13.Name = "label13";
            label13.Size = new Size(79, 15);
            label13.TabIndex = 32;
            label13.Text = "merge_image";
            // 
            // pictureBox11
            // 
            pictureBox11.Location = new Point(1446, 697);
            pictureBox11.Name = "pictureBox11";
            pictureBox11.Size = new Size(380, 287);
            pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox11.TabIndex = 31;
            pictureBox11.TabStop = false;
            // 
            // pictureBox12
            // 
            pictureBox12.Location = new Point(1439, 29);
            pictureBox12.Name = "pictureBox12";
            pictureBox12.Size = new Size(380, 287);
            pictureBox12.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox12.TabIndex = 33;
            pictureBox12.TabStop = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(1822, 269);
            label14.Name = "label14";
            label14.Size = new Size(82, 15);
            label14.TabIndex = 35;
            label14.Text = "surf_threshold";
            // 
            // surf_threshold_scr
            // 
            surf_threshold_scr.Location = new Point(1822, 296);
            surf_threshold_scr.Name = "surf_threshold_scr";
            surf_threshold_scr.Size = new Size(311, 24);
            surf_threshold_scr.TabIndex = 34;
            surf_threshold_scr.Scroll += surf_threshold_scr_Scroll;
            // 
            // surf_threshold_value
            // 
            surf_threshold_value.Location = new Point(2136, 293);
            surf_threshold_value.Name = "surf_threshold_value";
            surf_threshold_value.Size = new Size(100, 23);
            surf_threshold_value.TabIndex = 36;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2253, 1041);
            Controls.Add(surf_threshold_value);
            Controls.Add(label14);
            Controls.Add(surf_threshold_scr);
            Controls.Add(pictureBox12);
            Controls.Add(label13);
            Controls.Add(pictureBox11);
            Controls.Add(label12);
            Controls.Add(pictureBox10);
            Controls.Add(label11);
            Controls.Add(pictureBox9);
            Controls.Add(keypoint_name1);
            Controls.Add(keypoint_name2);
            Controls.Add(pictureBox8);
            Controls.Add(label10);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(pictureBox6);
            Controls.Add(pictureBox7);
            Controls.Add(pictureBox5);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(pictureBox4);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(c_text);
            Controls.Add(label2);
            Controls.Add(c_scr);
            Controls.Add(blocksize_text);
            Controls.Add(label1);
            Controls.Add(blocksize_scr);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Button button1;
        private Button button2;
        private HScrollBar blocksize_scr;
        private Label label1;
        private TextBox blocksize_text;
        private TextBox c_text;
        private Label label2;
        private HScrollBar c_scr;
        private Label label3;
        private Label label4;
        private Label label5;
        private OpenFileDialog openFileDialog1;
        private PictureBox pictureBox4;
        private Label label6;
        private PictureBox pictureBox5;
        private Label label7;
        private Label label8;
        private Label label9;
        private PictureBox pictureBox6;
        private PictureBox pictureBox7;
        private Label label10;
        private PictureBox pictureBox8;
        private TextBox keypoint_name2;
        private TextBox keypoint_name1;
        private PictureBox pictureBox9;
        private Label label11;
        private Label label12;
        private PictureBox pictureBox10;
        private Label label13;
        private PictureBox pictureBox11;
        private PictureBox pictureBox12;
        private Label label14;
        private HScrollBar surf_threshold_scr;
        private TextBox surf_threshold_value;
    }
}