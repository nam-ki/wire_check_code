using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Features2D;
using OpenCvSharp.XFeatures2D;
using System.Numerics;
using System.Threading.Tasks;

namespace merge_test_program_keypoint
{
    public partial class Form1 : Form
    {

        //�������� ����
        string filePath;
        List<string> list_file = new List<string>();
        Mat origin_image = new Mat();
        Mat origin_image2 = new Mat();
        Mat origin_image3 = new Mat();
        int block_size = 3;
        int c_value = 0;
        Mat grayscale_img = new Mat();
        Mat grayscale_img2 = new Mat();
        int surf_threshold = 0;

        KeyPoint[] keypoint1;
        Mat descriptors1 = new Mat();
        KeyPoint[] keypoint2;
        Mat descriptors2 = new Mat();

        List<Point2f> goodmatch_keypoint1 = new List<Point2f>();
        List<Point2f> goodmatch_keypoint2 = new List<Point2f>();

        public Form1()
        {
            InitializeComponent();

            openFileDialog1.InitialDirectory = "D:\\����\\�赵�˻�κ�\\�赵 �ڵ�\\wire_check_code\\wire_check_code\\merge_test_program_keypoint\\merge_test_program_keypoint";

            blocksize_text.Text = 3.ToString();
            c_text.Text = 0.ToString();
            surf_threshold_value.Text = 0.ToString();

            blocksize_scr.Maximum = 500;
            blocksize_scr.Minimum = 3;

            c_scr.Maximum = 500;
            c_scr.Minimum = 0;

            surf_threshold_scr.Maximum = 800;
            surf_threshold_scr.Minimum = 0;


        }

        #region event : ���� �� �ε�

        private void MainForm_Load(object sender, EventArgs e)
        {


        }

        #endregion

        #region event : ���� �� ����
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        #endregion

        //folder open button click
        private void button1_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Multiselect = false;

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    //string fileName = Path.GetFileName(openFileDialog1.FileName);
            //    filePath = openFileDialog1.FileName;
            //    //image_path = filePath + "\\" + fileName;
            //    MessageBox.Show(filePath);
            //}

            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String filepath in openFileDialog1.FileNames)
                {
                    list_file.Add(filepath.ToString());
                }
            }
        }

        //image show button click
        private async void button2_Click(object sender, EventArgs e)
        {
            //origin_image = Cv2.ImRead(filePath);
            string current_path = Directory.GetCurrentDirectory();
            keypoint_name1.Text = list_file[0].Replace(current_path, "").ToString();
            keypoint_name2.Text = list_file[1].Replace(current_path, "").ToString();
            origin_image = Cv2.ImRead(list_file[0]);
            origin_image2 = Cv2.ImRead(list_file[1]);
            origin_image3 = Cv2.ImRead(list_file[2]);

            //�̹��� �׷��� ������ ��ȯ


            Cv2.CvtColor(origin_image, grayscale_img, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(origin_image2, grayscale_img2, ColorConversionCodes.BGR2GRAY);

            //await Task.Run(() => shift_keypoint(grayscale_img));
            //await Task.Run(() => shift_keypoint2(grayscale_img2));
            (keypoint1, descriptors1) = await Task.Run(() => shift_keypoint(grayscale_img));
            //Task.Run(() => shift_keypoint(grayscale_img));
            (keypoint2, descriptors2) = await Task.Run(() => shift_keypoint2(grayscale_img2));
            //Task.Run(() => shift_keypoint2(grayscale_img2));

            (goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));

            await Task.Run(() => RANSAC(goodmatch_keypoint1, goodmatch_keypoint2));
            //shift_keypoint(grayscale_img);

            Stitcher stitcher = Stitcher.Create();

            // �Է� �̹��� ����Ʈ ����
            List<Mat> images = new List<Mat> { origin_image, origin_image2,origin_image3 };

            // ��ƼĪ ����
            Mat resultImage = new Mat();

            Stitcher.Status status = stitcher.Stitch(images, resultImage);


            // ��ƼĪ ���� ���� Ȯ��
            if (status == Stitcher.Status.OK)
            {
                // ��� �̹��� ���
                pictureBox12.Image = BitmapConverter.ToBitmap(resultImage);
            }
            else
            {
                // ��ƼĪ ���� �� ���� �޽��� ���
                MessageBox.Show("Stitching failed: " + status);
            }


        }

        //Ư¡�� ����
        private (KeyPoint[], Mat) shift_keypoint(Mat gray_img)
        {

            //�̹��� ���̳ʸ� ȭ
            Mat binary_img = new Mat();

            Cv2.AdaptiveThreshold(gray_img, binary_img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////sift �˰����� �ʱ�ȭ
            //var sift = SIFT.Create();

            //surt �˰����ʱ�ȭ
            //var surf = SURF.Create(hessianThreshold: 400);
            var surf = SURF.Create(hessianThreshold: surf_threshold);

            //Ű ����Ʈ�� ��ũ���͸� ����
            KeyPoint[] keyPoints;
            Mat descriptors = new Mat();

            surf.DetectAndCompute(binary_img, null, out keyPoints, descriptors);
            //surf.DetectAndCompute(gray_img, null, out keyPoints, descriptors);
            //sift.DetectAndCompute(binary_img, null, out keyPoints, descriptors);

            //keyPoints = sift.Detect(binary_img);

            //Ư¡�� ���
            Mat output = new Mat();
            Cv2.DrawKeypoints(origin_image, keyPoints, output);

            //��� �̹��� ���
            List<Mat> image_list = new List<Mat>() { origin_image, gray_img, binary_img, output };
            List<PictureBox> picturebox_list = new List<PictureBox>() { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };

            for (int i = 0; i < image_list.Count; i++)
            {
                picturebox_list[i].Image = BitmapConverter.ToBitmap(image_list[i]);
            }

            return (keyPoints, descriptors);
        }

        private (KeyPoint[], Mat) shift_keypoint2(Mat gray_img)
        {

            //�̹��� ���̳ʸ� ȭ
            Mat binary_img = new Mat();

            Cv2.AdaptiveThreshold(gray_img, binary_img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: block_size, c: c_value);

            ////sift �˰����� �ʱ�ȭ
            //var sift = SIFT.Create();

            //surt �˰����ʱ�ȭ
            var surf = SURF.Create(hessianThreshold: surf_threshold);

            //Ű ����Ʈ�� ��ũ���͸� ����
            KeyPoint[] keyPoints;
            Mat descriptors = new Mat();

            surf.DetectAndCompute(binary_img, null, out keyPoints, descriptors);
            //surf.DetectAndCompute(gray_img, null, out keyPoints, descriptors);
            //sift.DetectAndCompute(binary_img, null, out keyPoints, descriptors);

            //keyPoints = sift.Detect(binary_img);

            //Ư¡�� ���
            Mat output = new Mat();
            Cv2.DrawKeypoints(origin_image, keyPoints, output);

            //��� �̹��� ���
            List<Mat> image_list = new List<Mat>() { origin_image2, gray_img, binary_img, output };
            List<PictureBox> picturebox_list = new List<PictureBox>() { pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

            for (int i = 0; i < image_list.Count; i++)
            {
                picturebox_list[i].Image = BitmapConverter.ToBitmap(image_list[i]);
            }

            return (keyPoints, descriptors);
        }


        async private void blocksize_scr_Scroll(object sender, ScrollEventArgs e)
        {

            if (blocksize_scr.Value % 2 != 0)
            {
                block_size = blocksize_scr.Value;
                blocksize_text.Text = block_size.ToString();

                (keypoint1, descriptors1) = await Task.Run(() => shift_keypoint(grayscale_img));
                //Task.Run(() => shift_keypoint(grayscale_img));
                (keypoint2, descriptors2) = await Task.Run(() => shift_keypoint2(grayscale_img2));
                (goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));
                //Task.Run(() => shift_keypoint2(grayscale_img2));
                //await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));
                //(goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));

                await Task.Run(() => RANSAC(goodmatch_keypoint1, goodmatch_keypoint2));
            }
        }

        async private void c_scr_Scroll(object sender, ScrollEventArgs e)
        {
            c_value = c_scr.Value;
            c_text.Text = c_value.ToString();
            (keypoint1, descriptors1) = await Task.Run(() => shift_keypoint(grayscale_img));
            (keypoint2, descriptors2) = await Task.Run(() => shift_keypoint2(grayscale_img2));
            (goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));
            //Task.Run(() => shift_keypoint(grayscale_img));
            //Task.Run(() => shift_keypoint2(grayscale_img2));
            //await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));
            //(goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));

            await Task.Run(() => RANSAC(goodmatch_keypoint1, goodmatch_keypoint2));
        }



        //Ư¡�� ��Ī
        private (List<Point2f>, List<Point2f>) keypoint_matching(Mat des1, Mat des2, OpenCvSharp.KeyPoint[] key1, OpenCvSharp.KeyPoint[] key2)
        {
            try
            {
                var matcher = new BFMatcher(NormTypes.L2);

                var matches = matcher.Match(des1, des2);

                double dMaxDist = matches[0].Distance;
                double dMinDist = matches[1].Distance;
                double dDistance;

                //�� ���� keypoint ���̿��� min-max �� ���(min�� ���)
                foreach (DMatch match in matches)
                {
                    dDistance = match.Distance;

                    if (dDistance < dMinDist)
                    {
                        dMinDist = dDistance;
                    }
                    if (dDistance > dMaxDist)
                    {
                        dMaxDist = dDistance;
                    }
                }

                //for (int i = 0; i<des1.Rows; i++)
                //{
                //    dDistance = matches[i].Distance;

                //    if (dDistance < dMinDist)
                //    {
                //        dMinDist = dDistance;
                //    }
                //    if (dDistance > dMaxDist)
                //    {
                //        dMaxDist = dDistance;
                //    }
                //}

                //���� ��Ī ����(match�� distance ���� �������� matching�� �� �� ��)
                //min�� ���� 3�� �Ǵ� good_matches.size() > 60������ goodmatch�� ����.
                List<DMatch> good_matches = new List<DMatch>();
                int distance = 5; //10
                do
                {
                    List<DMatch> good_matches2 = new List<DMatch>();
                    foreach (DMatch match in matches)
                    {
                        if (match.Distance < distance * dMinDist)
                        {
                            good_matches2.Add(match);
                        }
                    }
                    //for (int i=0;i<descriptors1.Rows ; i++)
                    //{
                    //    if (matches[i].Distance < distance * dMinDist)
                    //    {
                    //        good_matches2.
                    //    }
                    //}
                    good_matches = good_matches2;
                    distance -= 1;
                } while (distance != 2 && good_matches.Count > 60);

                //drawing the results
                var imagematches = new Mat();

                Mat combinedimage = new Mat();

                Cv2.VConcat(new Mat[] { origin_image, origin_image2 }, combinedimage);

                //Cv2.DrawMatches(origin_image, key1, origin_image2, key2, matches, imagematches);

                Cv2.DrawMatches(origin_image, key1, origin_image2, key2, good_matches.ToArray(), imagematches, Scalar.All(-1), Scalar.All(-1), null, DrawMatchesFlags.NotDrawSinglePoints);



                pictureBox9.Image = BitmapConverter.ToBitmap(imagematches);



                //GOODMATCH������ keypoint�� ����.
                List<Point2f> goodmatch_key1 = new List<Point2f>();
                List<Point2f> goodmatch_key2 = new List<Point2f>();

                //goodmatches������ keypoint�� ����.
                foreach (DMatch match in good_matches)
                {
                    goodmatch_key1.Add(key1[match.QueryIdx].Pt);
                    goodmatch_key2.Add(key2[match.TrainIdx].Pt);
                }

                return (goodmatch_key1, goodmatch_key2);
            }
            catch (Exception ex)
            {
                return (null, null);
            }



        }

        //RANSAC ����� �̿��Ͽ� ù��°�� �ι�° ��ǥ ������ ������ĺ�ȯ H�� ���Ѵ�.
        private void RANSAC(List<Point2f> goodmatch_key1, List<Point2f> goodmatch_key2)
        {
            //InputArray inputKey1 = InputArray.Create(goodmatch_key1.ToArray());
            //InputArray inputKey2 = InputArray.Create(goodmatch_key2.ToArray());
            try
            {
                Point2f[] inputPointsArray1 = goodmatch_key1.ToArray();
                Point2f[] inputPointsArray2 = goodmatch_key2.ToArray();

                Mat inputkey1 = new Mat(inputPointsArray1.Length, 1, MatType.CV_32FC2, inputPointsArray1);
                Mat inputkey2 = new Mat(inputPointsArray2.Length, 1, MatType.CV_32FC2, inputPointsArray2);

                Mat HomoMatrix = Cv2.FindHomography(inputkey2, inputkey1, HomographyMethods.Ransac);

                Mat matResult = new Mat();
                Cv2.WarpPerspective(origin_image2, matResult, HomoMatrix, new OpenCvSharp.Size((origin_image.Cols * 2), (int)(origin_image.Rows * 1.2)), InterpolationFlags.Cubic);

                pictureBox10.Image = BitmapConverter.ToBitmap(matResult);

                //copy left image to the panorama
                var matPanorama = matResult.Clone();
                var matROI = new Mat(matPanorama, new Rect(0, 0, origin_image.Cols, origin_image.Rows));
                Mat first_image = origin_image.Clone();

                first_image.CopyTo(matROI);

                pictureBox11.Image = BitmapConverter.ToBitmap(matPanorama);
                Cv2.ImWrite("test_merge.jpg", matPanorama);
            }
            catch (Exception ex) { }

        }

        //surf Ư¡�� ���� �˰��� threshold ����
        async private void surf_threshold_scr_Scroll(object sender, ScrollEventArgs e)
        {
            surf_threshold_value.Text = surf_threshold_scr.Value.ToString();

            surf_threshold = surf_threshold_scr.Value;

            (keypoint1, descriptors1) = await Task.Run(() => shift_keypoint(grayscale_img));
            (keypoint2, descriptors2) = await Task.Run(() => shift_keypoint2(grayscale_img2));
            (goodmatch_keypoint1, goodmatch_keypoint2) = await Task.Run(() => keypoint_matching(descriptors1, descriptors2, keypoint1, keypoint2));
            await Task.Run(() => RANSAC(goodmatch_keypoint1, goodmatch_keypoint2));
        }
    }
}
