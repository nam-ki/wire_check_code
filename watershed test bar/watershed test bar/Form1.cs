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

            //������ ����ȭ scroll�� �� text ����
            hScrollBar1.Minimum = 3; // blocksize scroll �ּ�
            hScrollBar1.Maximum = 500;
            hScrollBar2.Minimum = 0; // c �� scroll �ּ�
            hScrollBar2.Maximum = 500;
            textBox1.Text = 3.ToString(); // blocksize ǥ�� text�ڽ�
            textBox2.Text = 0.ToString(); // c �� ǥ�� text �ڽ�

            //opening iteration Ƚ�� scroll �� �� text ����
            hScrollBar3.Minimum = 1;
            hScrollBar3.Maximum = 20;
            textBox3.Text = 1.ToString();

            //dilate iteration Ƚ�� scroll �� �� text ����
            hScrollBar4.Minimum = 1;
            hScrollBar4.Maximum = 20;
            textBox4.Text = 1.ToString();

            //thresholdvalue percent �� scroll �� �� text ����
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
        int dilate_teration = 1; // ��� ����

        double thresholdvalue_percent = 1;

        Mat opening = new Mat();
        int count;
        Mat kernel;



        #region event : ���� �� �ε�

        private void MainForm_Load(object sender, EventArgs e)
        {

            count = 0;
        }

        #endregion

        #region event : ���� �� ����
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        #endregion

        private async void button1_Click(object sender, EventArgs e)
        {
            image = Cv2.ImRead("real_wire_test3.jpg");

            //�̹��� �׷��� ������ ��ȭ
            Cv2.CvtColor(image, gray_img, ColorConversionCodes.BGR2GRAY);

            ////�̹��� ����ȭ(sharpening)

            sharpened_img = SharpenImage(gray_img);


            await Task.Run(() => call_algorithm(sharpened_img));
            //call_algorithm(sharpened_img);

            #region ���� ����
            ////�̹��� ������ ����ȭ
            //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////��������(noise reduce)

            //int kernel_size = 3;
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            ////��������(opening)
            //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: 1);

            ////��� ������ ���� ��������(Ȯ��)

            //Cv2.Dilate(opening, sure_bg, kernel, iterations: 10);

            ////�Ÿ���ȯ

            //(distance, distance_img) = distacne_transform_fun(opening);

            //////���� ������ ���� ����ȭ

            //(sure_fg, sure_fg_img) = fg_transform(distance);

            ////unknown �κ� ����

            //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            ////labeling�� ���� marker
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            ////watershed �˰��� ����
            //water_img = watershed_algorithm(image, marker);

            //List<PictureBox> PictureBoxList = new List<PictureBox>(8) { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

            //Mat[] images = { binary_img, opening, sure_bg, distance_img, sure_fg_img, unknown, marker_img, water_img };

            //for (int i = 0; i < images.Length; i++)
            //{
            //    PictureBoxList[i].Image = BitmapConverter.ToBitmap(images[i]);
            //}
            #endregion

        }

        //�˰��� ȣ��
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
                        //�̹��� ������ ����ȭ
                        binary_img = await Task.Run(() => task1(sharpened_img));
                        //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);
                        break;

                    case 1:
                        opening = await Task.Run(() => task2(binary_img));

                        ////��������(noise reduce)
                        //kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

                        ////��������(opening)
                        //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: opening_iteration);

                        break;

                    case 2:
                        //��� ������ ���� ��������(Ȯ��)

                        sure_bg = await Task.Run(() => task3(opening));

                        //Cv2.Dilate(opening, sure_bg, kernel, iterations: dilate_teration);

                        break;

                    case 3:
                        (distance_img, sure_fg_img, unknown, marker_img, water_img) = await Task.Run(() => task4());

                        ////�Ÿ���ȯ

                        //(distance, distance_img) = await Task.Run(() => distacne_transform_fun(opening));
                        ////(distance, distance_img) = distacne_transform_fun(opening);

                        //////���� ������ ���� ����ȭ

                        //(sure_fg, sure_fg_img) = await Task.Run(() => fg_transform(distance));
                        ////(sure_fg, sure_fg_img) = fg_transform(distance);

                        ////unknown �κ� ����

                        //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

                        ////labeling�� ���� marker
                        //(marker, marker_img) = await Task.Run(() => labeling(sure_fg_img, unknown));
                        ////(marker, marker_img) = labeling(sure_fg_img, unknown);


                        ////watershed �˰��� ����
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

            ////�̹��� ������ ����ȭ
            //Cv2.AdaptiveThreshold(sharpened_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////��������(noise reduce)
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            ////��������(opening)
            //Cv2.MorphologyEx(binary_img, opening, MorphTypes.Open, kernel, iterations: 1);

            ////��� ������ ���� ��������(Ȯ��)

            //Cv2.Dilate(opening, sure_bg, kernel, iterations: 10);

            ////�Ÿ���ȯ

            //(distance, distance_img) = distacne_transform_fun(opening);

            //////���� ������ ���� ����ȭ

            //(sure_fg, sure_fg_img) = fg_transform(distance);

            ////unknown �κ� ����

            //Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            ////labeling�� ���� marker
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            ////watershed �˰��� ����
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

            //��������(noise reduce)
            kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

            //��������(opening)
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
            //�Ÿ���ȯ

            (distance, distance_img) = await Task.Run(() => distacne_transform_fun(opening));
            //(distance, distance_img) = distacne_transform_fun(opening);

            ////���� ������ ���� ����ȭ

            (sure_fg, sure_fg_img) = await Task.Run(() => fg_transform(distance));
            //(sure_fg, sure_fg_img) = fg_transform(distance);

            //unknown �κ� ����

            Cv2.Subtract(sure_bg, sure_fg_img, unknown);

            //labeling�� ���� marker
            (marker, marker_img) = await Task.Run(() => labeling(sure_fg_img, unknown));
            //(marker, marker_img) = labeling(sure_fg_img, unknown);


            //watershed �˰��� ����
            water_img = await Task.Run(() => watershed_algorithm(image, marker));
            //water_img = watershed_algorithm(image, marker);

            return (distance_img, sure_fg_img, unknown, marker_img, water_img);
        }

        //sharpening
        static Mat SharpenImage(Mat image)
        {
            Mat sharpenedImage = new Mat();

            //float b = (1 - strength) / 8;

            //Ŀ�� ����

            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //        b, b, b,
            //        b, strength, b,
            //        b, b, b
            //    });

            ////���ö�þ� Ŀ��
            //Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            //       0,  1,  0,
            //       1, -4,  1,
            //       0,  1,  0
            //    });

            //    //prewitt Ŀ��
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


            //�̹��� ���͸�
            Cv2.Filter2D(image, sharpenedImage, -1, kernel);





            return sharpenedImage;
        }

        //�Ÿ� ��ȯ
        static (Mat, Mat) distacne_transform_fun(Mat image)
        {
            ////�Ÿ���ȯ
            Mat distance_trans = new Mat();
            Cv2.DistanceTransform(image, distance_trans, DistanceTypes.L2, DistanceTransformMasks.Mask5);
            Cv2.Normalize(distance_trans, distance_trans, 0, 255, NormTypes.MinMax);

            //�Ÿ���ȯ �̹���ȭ
            Mat distance_trans_img = new Mat();
            distance_trans.ConvertTo(distance_trans_img, MatType.CV_8U);

            return (distance_trans, distance_trans_img);
        }

        //���� ���� ����ȭ
        (Mat, Mat) fg_transform(Mat distance_data)
        {
            Mat surefg_trans = new Mat();

            OpenCvSharp.Cv2.MinMaxLoc(distance_data, out _, out maxVal);

            thresholdValue = thresholdvalue_percent * 0.001 * maxVal;

            Cv2.Threshold(distance_data, surefg_trans, thresholdValue, 255, ThresholdTypes.Binary);

            //�������� �̹���ȭ
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

            //marker �̹��� ȭ
            marker_fun.ConvertTo(marker_fun_img, MatType.CV_8U);

            return (marker_fun, marker_fun_img);
        }

        //watershed �˰���
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