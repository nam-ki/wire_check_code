namespace test
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
            components = new System.ComponentModel.Container();
            camstartbtn = new Button();
            pictureBox1 = new PictureBox();
            textBox1 = new TextBox();
            timerCameraSequence = new System.Windows.Forms.Timer(components);
            btncalibration = new Button();
            pictureBox2 = new PictureBox();
            camstate = new TextBox();
            bindingSource1 = new BindingSource(components);
            dataGridView1 = new DataGridView();
            timerCameraviewer = new System.Windows.Forms.Timer(components);
            takepicture_timesecond = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // camstartbtn
            // 
            camstartbtn.Location = new Point(111, 535);
            camstartbtn.Name = "camstartbtn";
            camstartbtn.Size = new Size(142, 29);
            camstartbtn.TabIndex = 0;
            camstartbtn.Text = "ON";
            camstartbtn.UseVisualStyleBackColor = true;
            camstartbtn.Click += connect_camera_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(30, 95);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(696, 300);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1568, 95);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(160, 400);
            textBox1.TabIndex = 2;
            // 
            // timerCameraSequence
            // 
            timerCameraSequence.Tick += timerCameraSequence_Tick;
            // 
            // btncalibration
            // 
            btncalibration.Location = new Point(506, 535);
            btncalibration.Name = "btncalibration";
            btncalibration.Size = new Size(94, 29);
            btncalibration.TabIndex = 4;
            btncalibration.Text = "calibration";
            btncalibration.UseVisualStyleBackColor = true;
            btncalibration.Click += calibration_click;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(762, 95);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(766, 300);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // camstate
            // 
            camstate.Location = new Point(30, 7);
            camstate.Name = "camstate";
            camstate.Size = new Size(155, 27);
            camstate.TabIndex = 6;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(951, 447);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(300, 188);
            dataGridView1.TabIndex = 7;
            // 
            // timerCameraviewer
            // 
            timerCameraviewer.Tick += timerCameraviewer_Tick;
            // 
            // takepicture_timesecond
            // 
            takepicture_timesecond.Tick += takepicture_timesecond_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1761, 656);
            Controls.Add(dataGridView1);
            Controls.Add(camstate);
            Controls.Add(pictureBox2);
            Controls.Add(btncalibration);
            Controls.Add(textBox1);
            Controls.Add(pictureBox1);
            Controls.Add(camstartbtn);
            Name = "Form1";
            Text = "Form1";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button camstartbtn;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private System.Windows.Forms.Timer timerCameraSequence;
        private Button btncalibration;
        private PictureBox pictureBox2;
        private TextBox camstate;
        private BindingSource bindingSource1;
        private DataGridView dataGridView1;
        private System.Windows.Forms.Timer timerCameraviewer;
        private System.Windows.Forms.Timer takepicture_timesecond;
    }
}