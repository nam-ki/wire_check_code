namespace image_check_bar
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
            image_load_button = new Button();
            blocksize_scr = new HScrollBar();
            c_scr = new HScrollBar();
            label1 = new Label();
            label2 = new Label();
            blocksize_val = new TextBox();
            c_val = new TextBox();
            canny_min_Scr = new HScrollBar();
            canny_max_scr = new HScrollBar();
            label3 = new Label();
            label4 = new Label();
            canny_min_val = new TextBox();
            canny_max_val = new TextBox();
            pictureBox2 = new PictureBox();
            openFileDialog1 = new OpenFileDialog();
            button1 = new Button();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox6 = new PictureBox();
            mopology_iteration_scr = new HScrollBar();
            label5 = new Label();
            mopology_iteration_value = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(37, 41);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(538, 248);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // image_load_button
            // 
            image_load_button.Location = new Point(1935, 79);
            image_load_button.Margin = new Padding(2);
            image_load_button.Name = "image_load_button";
            image_load_button.Size = new Size(150, 42);
            image_load_button.TabIndex = 3;
            image_load_button.Text = "button1";
            image_load_button.UseVisualStyleBackColor = true;
            image_load_button.Click += image_load_button_Click;
            // 
            // blocksize_scr
            // 
            blocksize_scr.Location = new Point(212, 820);
            blocksize_scr.Name = "blocksize_scr";
            blocksize_scr.Size = new Size(663, 30);
            blocksize_scr.TabIndex = 4;
            blocksize_scr.Scroll += blocksize_scr_Scroll;
            // 
            // c_scr
            // 
            c_scr.Location = new Point(212, 868);
            c_scr.Name = "c_scr";
            c_scr.Size = new Size(663, 30);
            c_scr.TabIndex = 5;
            c_scr.Scroll += c_scr_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(156, 868);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(13, 15);
            label1.TabIndex = 6;
            label1.Text = "c";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(136, 823);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(56, 15);
            label2.TabIndex = 7;
            label2.Text = "blocksize";
            // 
            // blocksize_val
            // 
            blocksize_val.Location = new Point(929, 823);
            blocksize_val.Margin = new Padding(2);
            blocksize_val.Name = "blocksize_val";
            blocksize_val.Size = new Size(98, 23);
            blocksize_val.TabIndex = 8;
            // 
            // c_val
            // 
            c_val.Location = new Point(929, 866);
            c_val.Margin = new Padding(2);
            c_val.Name = "c_val";
            c_val.Size = new Size(98, 23);
            c_val.TabIndex = 9;
            // 
            // canny_min_Scr
            // 
            canny_min_Scr.Location = new Point(1143, 820);
            canny_min_Scr.Name = "canny_min_Scr";
            canny_min_Scr.Size = new Size(663, 32);
            canny_min_Scr.TabIndex = 10;
            canny_min_Scr.Scroll += canny_min_Scr_Scroll;
            // 
            // canny_max_scr
            // 
            canny_max_scr.Location = new Point(1143, 868);
            canny_max_scr.Name = "canny_max_scr";
            canny_max_scr.Size = new Size(663, 32);
            canny_max_scr.TabIndex = 11;
            canny_max_scr.Scroll += canny_max_scr_Scroll;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1075, 830);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(65, 15);
            label3.TabIndex = 12;
            label3.Text = "canny_min";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1075, 878);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 13;
            label4.Text = "canny_max";
            // 
            // canny_min_val
            // 
            canny_min_val.Location = new Point(1860, 820);
            canny_min_val.Margin = new Padding(2);
            canny_min_val.Name = "canny_min_val";
            canny_min_val.Size = new Size(98, 23);
            canny_min_val.TabIndex = 14;
            // 
            // canny_max_val
            // 
            canny_max_val.Location = new Point(1860, 868);
            canny_max_val.Margin = new Padding(2);
            canny_max_val.Name = "canny_max_val";
            canny_max_val.Size = new Size(98, 23);
            canny_max_val.TabIndex = 15;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(629, 41);
            pictureBox2.Margin = new Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(538, 248);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 16;
            pictureBox2.TabStop = false;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.InitialDirectory = "D:\\\\과제\\\\삭도검사로봇\\\\삭도 코드\\\\삭도 코드";
            // 
            // button1
            // 
            button1.Location = new Point(1935, 164);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(145, 45);
            button1.TabIndex = 17;
            button1.Text = "folder open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(1268, 41);
            pictureBox3.Margin = new Padding(2);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(538, 245);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 18;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(37, 402);
            pictureBox4.Margin = new Padding(2);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(538, 241);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 19;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new Point(629, 402);
            pictureBox5.Margin = new Padding(2);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(538, 241);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 20;
            pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Location = new Point(1268, 402);
            pictureBox6.Margin = new Padding(2);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(538, 241);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 21;
            pictureBox6.TabStop = false;
            // 
            // mopology_iteration_scr
            // 
            mopology_iteration_scr.Location = new Point(689, 725);
            mopology_iteration_scr.Name = "mopology_iteration_scr";
            mopology_iteration_scr.Size = new Size(663, 32);
            mopology_iteration_scr.TabIndex = 22;
            mopology_iteration_scr.Scroll += mopology_iteration_scr_Scroll;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(689, 688);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(111, 15);
            label5.TabIndex = 23;
            label5.Text = "mopology_iteration";
            // 
            // mopology_iteration_value
            // 
            mopology_iteration_value.Location = new Point(1376, 725);
            mopology_iteration_value.Margin = new Padding(2);
            mopology_iteration_value.Name = "mopology_iteration_value";
            mopology_iteration_value.Size = new Size(98, 23);
            mopology_iteration_value.TabIndex = 24;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2184, 923);
            Controls.Add(mopology_iteration_value);
            Controls.Add(label5);
            Controls.Add(mopology_iteration_scr);
            Controls.Add(pictureBox6);
            Controls.Add(pictureBox5);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(button1);
            Controls.Add(pictureBox2);
            Controls.Add(canny_max_val);
            Controls.Add(canny_min_val);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(canny_max_scr);
            Controls.Add(canny_min_Scr);
            Controls.Add(c_val);
            Controls.Add(blocksize_val);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(c_scr);
            Controls.Add(blocksize_scr);
            Controls.Add(image_load_button);
            Controls.Add(pictureBox1);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button image_load_button;
        private HScrollBar blocksize_scr;
        private HScrollBar c_scr;
        private Label label1;
        private Label label2;
        private TextBox blocksize_val;
        private TextBox c_val;
        private HScrollBar canny_min_Scr;
        private HScrollBar canny_max_scr;
        private Label label3;
        private Label label4;
        private TextBox canny_min_val;
        private TextBox canny_max_val;
        private PictureBox pictureBox2;
        private OpenFileDialog openFileDialog1;
        private Button button1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
        private HScrollBar mopology_iteration_scr;
        private Label label5;
        private TextBox mopology_iteration_value;
    }
}