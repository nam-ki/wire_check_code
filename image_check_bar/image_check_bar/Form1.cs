using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 추가 using
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



        #region event : 메인 폼 로드

        private void MainForm_Load(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = "D:\\과제\\삭도검사로봇\\삭도 코드\\삭도 코드";
        }

        #endregion

        #region event : 메인 폼 종료
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        #endregion

        private void image_load_button_Click(object sender, EventArgs e)
        {
            image = Cv2.ImRead(filePath);

            pictureBox8.Image = BitmapConverter.ToBitmap(image);

            //이미지 처리 부분
            Rect roi = new Rect(300, 0, image.Width - 300, image.Height);

            image = image.SubMat(roi);



            // 그레이스케일로 변환
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);



            // 가장자리 감지
            Mat edges = new Mat();
            Cv2.Canny(gray, edges, 250, 390);
            //Cv2.Canny(Sharpend_img, edges, 450,500);

            // 외곽선 찾기
            var contours = Cv2.FindContoursAsArray(edges, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 외곽선 그리기
            Mat contouriamge = image.Clone();
            Cv2.DrawContours(contouriamge, contours, -1, Scalar.Red, 2);

            //최소값 최대값 설정
            int minval = contours[0][0].X;
            int maxval = contours[0][0].X;


            //최소점, 최대점 추출
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

            // 바깥쪽 점들 시각화
            Mat outsidepointimage = image.Clone();
            foreach (OpenCvSharp.Point point in outerPoints)
            {
                Cv2.Circle(outsidepointimage, point, 5, Scalar.Red, -1);
                Cv2.PutText(outsidepointimage, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            }

            //사각형 그리기
            Mat rectangle_image = outsidepointimage.Clone();
            Cv2.Polylines(rectangle_image, new OpenCvSharp.Point[][] { outerPoints }, true, Scalar.Red, 2);

            //마스크 생성
            Mat mask = new Mat(image.Size(), MatType.CV_8UC1, Scalar.Black);
            Cv2.FillConvexPoly(mask, outerPoints, Scalar.White);

            // 마스크를 적용하여 객체만 분리된 이미지 생성
            Mat segmentedObject = new Mat();
            image.CopyTo(segmentedObject, mask);

            pictureBox3.Image = BitmapConverter.ToBitmap(segmentedObject);





            // 객체 탐지를 위한 적응형 이진화

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

                #region 윤곽선 그린 후 중심점 추출(진행중)
                //윤곽선 그리기
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                // 검출된 윤곽선 그리기

                Mat resultImage = Mat.Zeros(binary.Size(), MatType.CV_8UC3);
                Cv2.DrawContours(resultImage, contours, -1, Scalar.Red, 2);

                pictureBox6.Image = BitmapConverter.ToBitmap(resultImage);

                Mat center_img = resultImage.Clone();


                //// 검출된 컨투어 중심점을 계산합니다.
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

                // 검출된 컨투어 중심점을 계산합니다.
                var centers = new List<Point2f>();
                foreach (var contour in contours)
                {
                    var moments = Cv2.Moments(contour);

                    // 면적을 계산합니다.
                    var area = moments.M00;

                    // 유효한 면적 범위를 설정합니다.
                    var minArea = 1000; // 최소 면적 값
                    var maxArea = 100000000; // 최대 면적 값

                    if (area >= minArea && area <= maxArea)
                    {
                        
                        // 면적이 유효한 범위 내에 있는 경우에만 중심점 계산 및 저장합니다.
                        var center = new Point2f((float)(moments.M10 / moments.M00), (float)(moments.M01 / moments.M00));
                        centers.Add(center);
                    }
                }

                // 결과를 확인하거나 처리할 수 있습니다.
                foreach (var center in centers)
                {
                    Cv2.Circle(center_img, (int)center.X, (int)center.Y, 5, Scalar.Blue, -1);
                }
            

            pictureBox7.Image = BitmapConverter.ToBitmap(center_img);
                #endregion

                #region 특징점 추출

                //var surf = SURF.Create(hessianThreshold: 400);

                ////키 포인트와 디스크립터를 추출
                //KeyPoint[] keyPoints;
                //Mat descriptors = new Mat();

                //surf.DetectAndCompute(binary, null, out keyPoints, descriptors);




                ////특징점 출력
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