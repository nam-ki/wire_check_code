using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// �߰� using
using OpenCvSharp;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;
using System.Xml.Linq;
using System.Security.Policy;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.Xml;
using System.Transactions;
using System.Runtime.CompilerServices;
using OpenCvSharp.Extensions;

namespace image_check_bar
{
    public partial class Form1 : Form
    {

        Mat binary = new Mat();
        Mat binary_gray = new Mat();
        int block_size;
        int c;
        Mat edge = new Mat();
        int canny_min;
        int canny_max;
        string image_path;
        string filePath;


        public Form1()
        {
            InitializeComponent();

            this.BackColor = Color.LightSteelBlue;

            blocksize_val.Text = 3.ToString();
            c_val.Text = 0.ToString();
            canny_max_val.Text = 0.ToString();
            canny_min_val.Text = 0.ToString();

            blocksize_scr.Minimum = 3;
            blocksize_scr.Maximum = 500 + 9;
            c_scr.Maximum = 500 + 9;
            canny_min_Scr.Maximum = 500 + 9;
            canny_max_scr.Maximum = 600 + 9;


        }



        #region event : ���� �� �ε�

        private void MainForm_Load(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = "D:\\����\\�赵�˻�κ�\\�赵 �ڵ�\\�赵 �ڵ�";
        }

        #endregion

        #region event : ���� �� ����
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        #endregion

        private void image_load_button_Click(object sender, EventArgs e)
        {
            Mat image = Cv2.ImRead(filePath);

            //�̹��� ó�� �κ�
            Rect roi = new Rect(300, 0, image.Width - 300, image.Height);

            image = image.SubMat(roi);



            // �׷��̽����Ϸ� ��ȯ
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);



            // �����ڸ� ����
            Mat edges = new Mat();
            Cv2.Canny(gray, edges, 250, 390);
            //Cv2.Canny(Sharpend_img, edges, 450,500);

            // �ܰ��� ã��
            var contours = Cv2.FindContoursAsArray(edges, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // �ܰ��� �׸���
            Mat contouriamge = image.Clone();
            Cv2.DrawContours(contouriamge, contours, -1, Scalar.Red, 2);

            //�ּҰ� �ִ밪 ����
            int minval = contours[0][0].X;
            int maxval = contours[0][0].X;


            //�ּ���, �ִ��� ����
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                foreach (OpenCvSharp.Point point in contour)
                {
                    if (point.X < minval)
                    {
                        minval = point.X;

                    }
                    if (point.X > maxval)
                    {
                        maxval = point.X;
                    }
                }
            }

            OpenCvSharp.Point[] outerPoints = new OpenCvSharp.Point[4]
            {
                new OpenCvSharp.Point(minval, 0),
                new OpenCvSharp.Point(maxval, 0),
                new OpenCvSharp.Point(maxval, image.Height),
                new OpenCvSharp.Point(minval, image.Height)
            };

            // �ٱ��� ���� �ð�ȭ
            Mat outsidepointimage = image.Clone();
            foreach (OpenCvSharp.Point point in outerPoints)
            {
                Cv2.Circle(outsidepointimage, point, 5, Scalar.Red, -1);
                Cv2.PutText(outsidepointimage, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            }

            //�簢�� �׸���
            Mat rectangle_image = outsidepointimage.Clone();
            Cv2.Polylines(rectangle_image, new OpenCvSharp.Point[][] { outerPoints }, true, Scalar.Red, 2);

            //����ũ ����
            Mat mask = new Mat(image.Size(), MatType.CV_8UC1, Scalar.Black);
            Cv2.FillConvexPoly(mask, outerPoints, Scalar.White);

            // ����ũ�� �����Ͽ� ��ü�� �и��� �̹��� ����
            Mat segmentedObject = new Mat();
            image.CopyTo(segmentedObject, mask);

            pictureBox3.Image = BitmapConverter.ToBitmap(segmentedObject);





            // ��ü Ž���� ���� ������ ����ȭ

            Cv2.CvtColor(segmentedObject, binary_gray, ColorConversionCodes.BGR2GRAY);

            pictureBox4.Image = BitmapConverter.ToBitmap(binary_gray);

            Cv2.AdaptiveThreshold(binary_gray, binary, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: 3, c: 0);

            pictureBox1.Image = BitmapConverter.ToBitmap(binary);



            Cv2.Canny(binary, edge, 0, 0);

            Cv2.Dilate(edge, edge, 3, iterations: 1);

            pictureBox2.Image = BitmapConverter.ToBitmap(edge);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void blocksize_scr_Scroll(object sender, ScrollEventArgs e)
        {
            if (blocksize_scr.Value % 2 != 0)
            {
                blocksize_val.Text = blocksize_scr.Value.ToString();
                block_size = blocksize_scr.Value;
                c = c_scr.Value;
                Cv2.AdaptiveThreshold(binary_gray, binary, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: block_size, c: c);
                pictureBox1.Image = BitmapConverter.ToBitmap(binary);

                Cv2.Canny(binary, edge, canny_min, canny_max);
                Cv2.Dilate(edge, edge, 3, iterations: 1);
                pictureBox2.Image = BitmapConverter.ToBitmap(edge);
            }

        }

        private void c_scr_Scroll(object sender, ScrollEventArgs e)
        {

            c_val.Text = c_scr.Value.ToString();
            block_size = blocksize_scr.Value;
            c = c_scr.Value;
            if (blocksize_scr.Value % 2 != 0)
            {
                Cv2.AdaptiveThreshold(binary_gray, binary, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: block_size, c: c);
            }
            pictureBox1.Image = BitmapConverter.ToBitmap(binary);

            Cv2.Canny(binary, edge, canny_min, canny_max);
            Cv2.Dilate(edge, edge, 3, iterations: 3);
            pictureBox2.Image = BitmapConverter.ToBitmap(edge);

        }

        private void canny_min_Scr_Scroll(object sender, ScrollEventArgs e)
        {
            canny_min_val.Text = canny_min_Scr.Value.ToString();
            canny_min = canny_min_Scr.Value;
            canny_max = canny_max_scr.Value;

            Cv2.Canny(binary, edge, canny_min, canny_max);
            Cv2.Dilate(edge, edge, 3, iterations: 1);
            pictureBox2.Image = BitmapConverter.ToBitmap(edge);
        }

        private void canny_max_scr_Scroll(object sender, ScrollEventArgs e)
        {
            canny_max_val.Text = canny_max_scr.Value.ToString();
            canny_min = canny_min_Scr.Value;
            canny_max = canny_max_scr.Value;

            Cv2.Canny(binary, edge, canny_min, canny_max);
            Cv2.Dilate(edge, edge, 3, iterations: 1);
            pictureBox2.Image = BitmapConverter.ToBitmap(edge);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //string fileName = Path.GetFileName(openFileDialog1.FileName);
                filePath = openFileDialog1.FileName;
                //image_path = filePath + "\\" + fileName;
                MessageBox.Show(filePath);
            }
        }

        private void mopology_iteration_scr_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}