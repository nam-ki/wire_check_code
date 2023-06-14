using System;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading.Tasks;

namespace watershed_test_bar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //적응형 이진화 scroll바 및 text 설정
            hScrollBar1.Minimum = 3; // blocksize scroll 최소
            hScrollBar1.Maximum = 500;
            hScrollBar2.Minimum = 0; // c 값 scroll 최소
            hScrollBar2.Maximum = 500;
            textBox1.Text = 3.ToString(); // blocksize 표시 text박스
            textBox2.Text = 0.ToString(); // c 값 표시 text 박스

            //opening iteration 횟수 scroll 바 및 text 설정
            hScrollBar3.Minimum = 1;
            hScrollBar3.Maximum = 20;
            textBox3.Text = 1.ToString();

            //dilate iteration 횟수 scroll 바 및 text 설정
            hScrollBar4.Minimum = 1;
            hScrollBar4.Maximum = 20;
            textBox4.Text = 1.ToString();

            //thresholdvalue percent 값 scroll 바 및 text 설정
            hScrollBar5.Minimum = 1;
            hScrollBar5.Maximum = 500;
            textBox5.Text = (0.001 * 1).ToString();

        }

        Mat image;
        Mat gray_img = new Mat();
        int block_size = 3;
        int c_value = 0;
        Mat binary_img = new Mat();
        Mat sure_bg = new Mat();
        Mat distance = new Mat();
        Mat distance_img = new Mat();
        Mat marker = new Mat();
        Mat marker_img = new Mat();
        Mat water_img = new Mat();
        Mat sharpened_img = new Mat();

        Mat sure_fg = new Mat();
        Mat sure_fg_img = new Mat();
        Mat unknown = new Mat();
        double maxVal;
        double thresholdValue;

        int kernel_size = 3;
        int opening_iteration = 1;
        int dilate_teration = 1; // 배경 추출

        double thresholdvalue_percent = 1;

        Mat opening = new Mat();
        int count;
        Mat kernel;



        #region event : 메인 폼 로드

        private void MainForm_Load(object sender, EventArgs e)
        {

            count = 0;
        }

        #endregion

        #region event : 메인 폼 종료
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        #endregion

        private async void button1_Click(object sender, EventArgs e)
        {
            image = Cv2.ImRead("real_wire_test3.jpg");

            //이미지 그레이 스케일 변화
            Cv2.CvtColor(image, gray_img, ColorConversionCodes.BGR2GRAY);

            ////이미지 선명화(sharpening)

            sharpened_img = SharpenImage(gray_img);


            await Task.Run(() => call_algorithm(sharpened_img));
            //call_algorithm(sharpened_img);

            #region 예비 저장
            ////이미지 적응형 이진화
            //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////모폴로지(noise reduce)

            //int kernel_size = 3;
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            ////모폴로지(opening)
            //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: 1);

            ////배경 추출을 위한 모폴로지(확장)

            //Cv2.Dilate(opening, sure_bg, kernel, iterations: 10);

            ////거리변환

            //(distance, distance_img) = distacne_transform_fun(opening);

            //////전경 추출을 위한 이진화

            //(sure_fg, sure_fg_img) = fg_transform(distance);

            ////unknown 부분 추출

            //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            ////labeling을 통한 marker
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            ////watershed 알고리즘 적용
            //water_img = watershed_algorithm(image, marker);

            //List<PictureBox> PictureBoxList = new List<PictureBox>(8) { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

            //Mat[] images = { binary_img, opening, sure_bg, distance_img, sure_fg_img, unknown, marker_img, water_img };

            //for (int i = 0; i < images.Length; i++)
            //{
            //    PictureBoxList[i].Image = BitmapConverter.ToBitmap(images[i]);
            //}
            #endregion

        }

        //알고리즘 호출
        async void call_algorithm(Mat sharpened_img)
        {
            while (true)
            {

                if (count > 3)
                {
                    break;
                }

                switch (count)
                {
                    case 0:
                        //이미지 적응형 이진화
                        binary_img = await Task.Run(() => task1(sharpened_img));
                        //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);
                        break;

                    case 1:
                        opening = await Task.Run(() => task2(binary_img));

                        ////모폴로지(noise reduce)
                        //kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

                        ////모폴로지(opening)
                        //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: opening_iteration);

                        break;

                    case 2:
                        //배경 추출을 위한 모폴로지(확장)

                        sure_bg = await Task.Run(() => task3(opening));

                        //Cv2.Dilate(opening, sure_bg, kernel, iterations: dilate_teration);

                        break;

                    case 3:
                        (distance_img, sure_fg_img, unknown, marker_img, water_img) = await Task.Run(() => task4());

                        ////거리변환

                        //(distance, distance_img) = await Task.Run(() => distacne_transform_fun(opening));
                        ////(distance, distance_img) = distacne_transform_fun(opening);

                        //////전경 추출을 위한 이진화

                        //(sure_fg, sure_fg_img) = await Task.Run(() => fg_transform(distance));
                        ////(sure_fg, sure_fg_img) = fg_transform(distance);

                        ////unknown 부분 추출

                        //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

                        ////labeling을 통한 marker
                        //(marker, marker_img) = await Task.Run(() => labeling(sure_fg_img, unknown));
                        ////(marker, marker_img) = labeling(sure_fg_img, unknown);


                        ////watershed 알고리즘 적용
                        //water_img = await Task.Run(() => watershed_algorithm(image, marker));
                        ////water_img = watershed_algorithm(image, marker);

                        List<PictureBox> PictureBoxList = new List<PictureBox>(8) { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

                        Mat[] images = { binary_img, opening, sure_bg, distance_img, sure_fg_img, unknown, marker_img, water_img };

                        for (int i = 0; i < images.Length; i++)
                        {
                            PictureBoxList[i].Image = BitmapConverter.ToBitmap(images[i]);
                        }

                        break;

                    default:
                        break;
                }
                count++;

            }

            ////이미지 적응형 이진화
            //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////모폴로지(noise reduce)
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            ////모폴로지(opening)
            //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: 1);

            ////배경 추출을 위한 모폴로지(확장)

            //Cv2.Dilate(opening, sure_bg, kernel, iterations: 10);

            ////거리변환

            //(distance, distance_img) = distacne_transform_fun(opening);

            //////전경 추출을 위한 이진화

            //(sure_fg, sure_fg_img) = fg_transform(distance);

            ////unknown 부분 추출

            //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            ////labeling을 통한 marker
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            ////watershed 알고리즘 적용
            //water_img = watershed_algorithm(image, marker);

            //List<PictureBox> PictureBoxList = new List<PictureBox>(8) { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

            //Mat[] images = { binary_img, opening, sure_bg, distance_img, sure_fg_img, unknown, marker_img, water_img };

            //for (int i = 0; i < images.Length; i++)
            //{
            //    PictureBoxList[i].Image = BitmapConverter.ToBitmap(images[i]);
            //}
        }

        //gray to binary
        Mat task1(Mat colorimage)
        {
            Mat binary = new Mat();

            Cv2.AdaptiveThreshold(colorimage, binary, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            return binary;
        }

        //mopology(opening)
        Mat task2(Mat binary)
        {
            Mat opening_img = new Mat();

            //모폴로지(noise reduce)
            kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            //모폴로지(opening)
            Cv2.MorphologyEx(binary, opening_img, MorphTypes.Open, kernel, iterations: opening_iteration);

            return opening_img;
        }

        Mat task3(Mat opening_img)
        {
            Mat sure_bg_image = new Mat();

            Cv2.Dilate(opening_img, sure_bg_image, kernel, iterations: dilate_teration);

            return sure_bg_image;
        }

        async Task<(Mat,Mat,Mat,Mat,Mat)> task4()
        {
            //거리변환

            (distance, distance_img) = await Task.Run(() => distacne_transform_fun(opening));
            //(distance, distance_img) = distacne_transform_fun(opening);

            ////전경 추출을 위한 이진화

            (sure_fg, sure_fg_img) = await Task.Run(() => fg_transform(distance));
            //(sure_fg, sure_fg_img) = fg_transform(distance);

            //unknown 부분 추출

            Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            //labeling을 통한 marker
            (marker, marker_img) = await Task.Run(() => labeling(sure_fg_img, unknown));
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            //watershed 알고리즘 적용
            water_img = await Task.Run(() => watershed_algorithm(image, marker));
            //water_img = watershed_algorithm(image, marker);

            return (distance_img, sure_fg_img, unknown, marker_img, water_img);
        }

        //sharpening
        static Mat SharpenImage(Mat image)
        {
            Mat sharpenedImage = new Mat();

            //float b = (1 - strength) / 8;

            //커널 정의

            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //        b, b, b,
            //        b, strength, b,
            //        b, b, b
            //    });

            ////라플라시안 커널
            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //       0,  1,  0,
            //       1, -4,  1,
            //       0,  1,  0
            //    });

            //    //prewitt 커널
            //    Mat kernelX = new Mat(3, 3, MatType.CV_32F, new float[] {
            //   -1,  0,  1,
            //   -1,  0,  1,
            //   -1,  0,  1
            //});

            //    Mat kernelY = new Mat(3, 3, MatType.CV_32F, new float[] {
            //    1,  1,  1,
            //    0,  0,  0,
            //   -1, -1, -1
            //});

            //    Mat kernel = kernelX + kernelY;



            Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            -1, -1, -1,
            -1, 9, -1,
            -1, -1, -1
        });

            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //        0, -1, 0,
            //        -1, 5, -1,
            //        0, -1, 0
            //    });

            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //        1, 1, 1,
            //        1, -7, 1,
            //        1, 1, 1
            //    });

            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //        -1, -1, -1,
            //        -1, 7, -1,
            //        -1, -1, -1
            //    });

            //Mat kernel = new Mat(5, 5, MatType.CV_32F, new float[] {
            //        -1, -1, -1,-1,-1,
            //        -1, 2, 2,2,-1,
            //        -1, 2, 8,2,-1,
            //        -1,2,2,2,-1,
            //        -1,-1,-1,-1,-1
            //    })/8.0;


            //이미지 필터링
            Cv2.Filter2D(image, sharpenedImage, -1, kernel);





            return sharpenedImage;
        }

        //거리 변환
        static (Mat, Mat) distacne_transform_fun(Mat image)
        {
            ////거리변환
            Mat distance_trans = new Mat();
            Cv2.DistanceTransform(image, distance_trans, DistanceTypes.L2, DistanceTransformMasks.Mask5);
            Cv2.Normalize(distance_trans, distance_trans, 0, 255, NormTypes.MinMax);

            //거리변환 이미지화
            Mat distance_trans_img = new Mat();
            distance_trans.ConvertTo(distance_trans_img, MatType.CV_8U);

            return (distance_trans, distance_trans_img);
        }

        //전경 추출 이진화
        (Mat, Mat) fg_transform(Mat distance_data)
        {
            Mat surefg_trans = new Mat();

            OpenCvSharp.Cv2.MinMaxLoc(distance_data, out _, out maxVal);

            thresholdValue = thresholdvalue_percent * 0.001 * maxVal;

            Cv2.Threshold(distance_data, surefg_trans, thresholdValue, 255, ThresholdTypes.Binary);

            //전경추출 이미지화
            Mat surefg_trans_img = new Mat();

            surefg_trans.ConvertTo(surefg_trans_img, MatType.CV_8U);

            return (surefg_trans, surefg_trans_img);
        }

        //fg labeling
        static (Mat, Mat) labeling(Mat fg_data, Mat unknown_data)
        {
            Mat marker_fun_img = new Mat();
            Mat marker_fun = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();

            Cv2.ConnectedComponentsWithStats(fg_data, marker_fun, stats, centroids, PixelConnectivity.Connectivity8);
            marker_fun = marker_fun + 1;

            for (int j = 0; j < marker_fun.Rows; j++)
            {
                for (int k = 0; k < marker_fun.Cols; k++)
                {
                    if (unknown_data.Get<int>(j, k) == 255)
                    {
                        marker_fun.Set<int>(j, k, 0);
                    }
                }
            }

            //marker 이미지 화
            marker_fun.ConvertTo(marker_fun_img, MatType.CV_8U);

            return (marker_fun, marker_fun_img);
        }

        //watershed 알고리즘
        static Mat watershed_algorithm(Mat image, Mat marker_data)
        {
            Mat water_img_fun = image.Clone();

            Cv2.Watershed(water_img_fun, marker_data);

            for (int j = 0; j < marker_data.Rows; j++)
            {
                for (int k = 0; k < marker_data.Cols; k++)
                {
                    if (marker_data.Get<int>(j, k) == -1)
                    {

                        water_img_fun.Set<Vec3b>(j, k, new Vec3b(0, 0, 255));
                    }
                }
            }

            return water_img_fun;

        }

        //adaptive threshold blocksize
        private async void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (hScrollBar1.Value % 2 != 0)
            {
                textBox1.Text = hScrollBar1.Value.ToString();

                block_size = hScrollBar1.Value;


                count = 0;



                call_algorithm(sharpened_img);
            }
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            if (hScrollBar1.Value % 2 != 0)
            {
                textBox2.Text = hScrollBar2.Value.ToString();


                c_value = hScrollBar2.Value;

                count = 0;



                call_algorithm(sharpened_img);
            }
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            textBox3.Text = hScrollBar3.Value.ToString();


            opening_iteration = hScrollBar3.Value;

            count = 1;

            call_algorithm(sharpened_img);

        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {

            textBox4.Text = hScrollBar4.Value.ToString();


            dilate_teration = hScrollBar4.Value;

            count = 2;

            call_algorithm(sharpened_img);

        }

        private void hScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            textBox5.Text = (0.001*hScrollBar5.Value).ToString();


            thresholdvalue_percent = hScrollBar5.Value;

            count = 3;

            call_algorithm(sharpened_img);
        }
    }
}