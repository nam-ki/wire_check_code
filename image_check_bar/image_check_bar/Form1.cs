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
using OpenCvSharp.XFeatures2D;

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
        int mopology_iteration = 1;
        Mat image;


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

            mopology_iteration_value.Text = 1.ToString();
            mopology_iteration_scr.Maximum = 10 + 9;
            mopology_iteration_scr.Minimum = 1;


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
            image = Cv2.ImRead(filePath);

            pictureBox8.Image = BitmapConverter.ToBitmap(image);

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
                //Cv2.MorphologyEx(binary, binary, MorphTypes.Close, 5, iterations: 20);
                Cv2.Erode(binary, binary, 5, iterations: 20);
                Cv2.Canny(binary, edge, canny_min, canny_max);
                

                pictureBox2.Image = BitmapConverter.ToBitmap(edge);

                #region ������ �׸� �� �߽��� ����(������)
                //������ �׸���
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                // ����� ������ �׸���

                Mat resultImage = Mat.Zeros(binary.Size(), MatType.CV_8UC3);
                Cv2.DrawContours(resultImage, contours, -1, Scalar.Red, 2);

                pictureBox6.Image = BitmapConverter.ToBitmap(resultImage);

                Mat center_img = resultImage.Clone();


                //// ����� ������ �߽����� ����մϴ�.
                //var centers = new List<Point2f>();
                //foreach (var contour in contours)
                //{
                //    var moments = Cv2.Moments(contour);
                //    var center = new Point2f((float)(moments.M10 / moments.M00), (float)(moments.M01 / moments.M00));
                //    centers.Add(center);
                //}

                //foreach (var center in centers)
                //{
                //    Cv2.Circle(center_img, (int)center.X, (int)center.Y, 5, Scalar.Blue, -1);
                //}

                // ����� ������ �߽����� ����մϴ�.
                var centers = new List<Point2f>();
                foreach (var contour in contours)
                {
                    var moments = Cv2.Moments(contour);

                    // ������ ����մϴ�.
                    var area = moments.M00;

                    // ��ȿ�� ���� ������ �����մϴ�.
                    var minArea = 1000; // �ּ� ���� ��
                    var maxArea = 100000000; // �ִ� ���� ��

                    if (area >= minArea && area <= maxArea)
                    {
                        
                        // ������ ��ȿ�� ���� ���� �ִ� ��쿡�� �߽��� ��� �� �����մϴ�.
                        var center = new Point2f((float)(moments.M10 / moments.M00), (float)(moments.M01 / moments.M00));
                        centers.Add(center);
                    }
                }

                // ����� Ȯ���ϰų� ó���� �� �ֽ��ϴ�.
                foreach (var center in centers)
                {
                    Cv2.Circle(center_img, (int)center.X, (int)center.Y, 5, Scalar.Blue, -1);
                }
            

            pictureBox7.Image = BitmapConverter.ToBitmap(center_img);
                #endregion

                #region Ư¡�� ����

                //var surf = SURF.Create(hessianThreshold: 400);

                ////Ű ����Ʈ�� ��ũ���͸� ����
                //KeyPoint[] keyPoints;
                //Mat descriptors = new Mat();

                //surf.DetectAndCompute(binary, null, out keyPoints, descriptors);




                ////Ư¡�� ���
                //Mat output = new Mat();
                //Cv2.DrawKeypoints(edge, keyPoints, output);

                //pictureBox7.Image = BitmapConverter.ToBitmap(output);

                #endregion

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
            mopology_iteration_value.Text = mopology_iteration_scr.Value.ToString();
            mopology_iteration = mopology_iteration_scr.Value;

            Mat mopology_img = new Mat();
            //dilate
            //Cv2.Dilate(binary, mopology_img, 3, iterations: mopology_iteration);
            Cv2.MorphologyEx(binary, mopology_img, MorphTypes.Open,3, iterations: mopology_iteration);

            pictureBox5.Image = BitmapConverter.ToBitmap(mopology_img);
        }
    }
}