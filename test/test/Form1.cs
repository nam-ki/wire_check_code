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
using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
using System.Collections;
using OpenCvSharp.Extensions;
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

namespace test
{
    public partial class Form1 : Form
    {

        #region 생성자
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region 전역변수
        Mat frame = new Mat();
        // 열화상 카메라 초기 이미지 크기 설정
        int mWidth = 500;
        int mHeight = 300;

        int width;
        int height;
        static int d = 4; // 원통의 지름
        int r = d / 2; //원통의 반지름 

        static Mat image1 = new Mat();
        static Mat image2 = new Mat();
        static Mat image3 = new Mat();
        static Mat merge_img = new Mat();

        bool calibration = false;

        private CStApiAutoInit api;
        private CStDeviceArray deviceList = null;
        private CStSystem system = null;
        private CStDataStreamArray dataStreamList = null;
        private CStImageBuffer imageBuffer = null;
        private CStStillImageFiler stillImageFiler = null;
        private CStPixelFormatConverter pixelFormatConverter = null;
        //private CStStreamBuffer streamBuffer = null;

        private Thread mThread = null;
        private bool misStopping = false;

        private int mStep = 1;
        Mat cameraMatrix;
        Mat distCoeffs;
        Mat newcameramatrix;
        Rect rect;
        static Mat cameraMatrix_1 = null;
        static Mat distCoeffs_1 = null;
        static Mat newcameramatrix_1 = null;
        static Mat cameraMatrix_2 = null;
        static Mat distCoeffs_2 = null;
        static Mat newcameramatrix_2 = null;
        static Mat cameraMatrix_3 = null;
        static Mat distCoeffs_3 = null;
        static Mat newcameramatrix_3 = null;
        Mat[] calibrationmatrix;
        string device_name = null;

        Mat gray_img1 = new Mat();
        Mat binary_img1 = new Mat();
        Mat Morphology_img1 = new Mat();
        Mat label_img1 = new Mat();

        string[] xml_name_list;

        #endregion

        INodeMap nodeMapRemote;
        // Feature names
        string EXPOSURE_AUTO = "ExposureAuto";            //Standard
        string GAIN_AUTO = "GainAuto";                    //Standard
        int GAIN_AUTO_VALUE = 0;                          //0:OFF 1:AUTO(CONTINUOUS)
        string BALANCE_WHITE_AUTO = "BalanceWhiteAuto";    //Standard

        string AUTO_LIGHT_TARGET = "AutoLightTarget";        //Custom
        string GAIN = "Gain";                                //Standard

        string GAIN_RAW = "GainRaw";                        //Custom
        double Gain_value = 100;

        string EXPOSURE_MODE = "ExposureMode";            //Standard
        string EXPOSURE_TIME = "ExposureTime";            //Standard
        string EXPOSURE_TIME_RAW = "ExposureTimeRaw";        //Custom

        string BALANCE_RATIO_SELECTOR = "BalanceRatioSelector";    //Standard
        string BALANCE_RATIO = "BalanceRatio";            //Standard

        #region event : 메인 폼 로드

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region event : 메인 폼 종료
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (deviceList == null || dataStreamList == null)
            {
                return;
            }

            if (dataStreamList.IsGrabbingAny)
            {
                camera_connect_step5();
            }

        }
        #endregion



        #region event : 카메라 연결 버튼 클릭
        private void connect_camera_Click(object sender, EventArgs e)
        {

            if (camstartbtn.Text == "ON")
            {
                camstartbtn.BackColor = Color.Green;
                timerCameraSequence.Start();
                camstartbtn.Text = "OFF";

            }
            else
            {
                camera_connect_step5();
                timerCameraSequence.Stop();
                //takepicture_timesecond.Stop();
                camstate.Text = "timercamsequence stop";
                camstartbtn.Text = "ON";
                camstartbtn.BackColor = Color.Gray;

                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image = null;
                }

                camstate.Text = "";

                mStep = 1;
            }

            //await Task.Run(() => camera_connecting());
        }
        #endregion

        #region event : 캘리브레이션 버튼 클릭
        private void calibration_click(object sender, EventArgs e)
        {


            camera_calibration();
        }
        #endregion

        #region event : 픽처박스 스크린캡쳐

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Mat writeimg = new Mat();
            if (image1.Rows != 0 && image1.Cols != 0)
            {
                writeimg = image1.Clone();
                string dt = DateTime.Now.ToString("MMddHHmmss");
                //string path = System.Windows.Forms.Application.StartupPath;
                //MessageBox.Show(path);
                //string device_name = streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName;
                string folderPath = $"Capture\\{device_name}";
                DirectoryInfo di = new DirectoryInfo(folderPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                Cv2.ImWrite(folderPath + $"\\origin_{dt}.jpg", writeimg);
                MessageBox.Show("현재 영상 캡쳐됨",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                                );
            }
        }
        #endregion

        #region method : 텍스트박스 표시
        private void WriteTextBox(TextBox tb, string txt)
        {
            try
            {
                tb.Invoke(new MethodInvoker(delegate ()
                {
                    tb.AppendText(txt);
                    tb.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 현재 설정으로 현재 열거형 노드의 내용을 나열합니다.

        private void Enumeration(INodeMap nodeMap, string nodeName)
        {
            IEnum enumNode = nodeMap.GetNode<IEnum>(GAIN_AUTO);

            string strvalue = enumNode.Entries[GAIN_AUTO_VALUE];
            enumNode.FromString(strvalue);
        }

        private void Numeric<NODE_TYPE>(INodeMap nodeMap, string nodeName) where NODE_TYPE : IInteger
        {
            // Get the IInteger interface.
            NODE_TYPE node = (NODE_TYPE)nodeMap[nodeName];

            if (node.IsWritable)
            {
                node.Value = (long)Gain_value;
            }


        }

        private void NumericF<NODE_TYPE>(INodeMap nodeMap, string nodeName) where NODE_TYPE : IFloat
        {
            // Get the IFloat interface.
            NODE_TYPE node = (NODE_TYPE)nodeMap[nodeName];

            if (node.IsWritable)
            {
                while (true)
                {
                    // Display the feature name, the range, the current value and the incremental value.
                    WriteTextBox(textBox1, "nodename:" + nodeName + "\r\n");
                    WriteTextBox(textBox1, $" Minimum={node.Minimum}" + "\r\n");
                    WriteTextBox(textBox1, $" Maximum={node.Maximum}" + "\r\n");
                    WriteTextBox(textBox1, $" Current={node.Value}" + "\r\n");

                    // Reflect the value entered.
                    if (node.Minimum <= (long)Gain_value && (long)Gain_value <= node.Maximum)
                    {
                        node.Value = (long)Gain_value;
                        break;
                    }
                }


            }


        }

        #endregion

        #region 카메라 gain 설정

        private void GainAuto(INodeMap nodeMap)
        {
            //configure the GainAuto
            Enumeration(nodeMap, GAIN_AUTO);

            //configure the autolighttarget
            Numeric<IntegerNode>(nodeMap, AUTO_LIGHT_TARGET);

            if (nodeMap.GetNode<FloatNode>(GAIN) != null)
            {
                // Configure the Gain.
                NumericF<FloatNode>(nodeMap, GAIN);
            }
            else
            {
                // Configure the GainRaw if the Gain function does not exist.
                Numeric<IntegerNode>(nodeMap, GAIN_RAW);
            }
        }

        #endregion

        #region method : 이미지 병합

        private void merge_image(Mat img1, Mat img2, Mat img3)
        {
            Mat merge = new Mat();
            try
            {
                if ((img1.Height == img2.Height) && (img2.Height == img3.Height))
                {


                    Cv2.HConcat(new Mat[] { img1, img2, img3 }, merge_img);

                    Cv2.Resize(merge, merge, new OpenCvSharp.Size(mWidth, mHeight));

                    merge_img = merge.Clone();
                }
            }
            catch (Exception ex) { throw ex; }


        }

        private void merge_image_2(Mat img1, Mat img2)
        {
            Mat merge = new Mat();
            try
            {
                if ((img1.Height == img2.Height))
                {


                    Cv2.HConcat(new Mat[] { img1, img2 }, merge);

                    Cv2.Resize(merge, merge, new OpenCvSharp.Size(mWidth, mHeight));

                    merge_img = merge.Clone();



                }
            }
            catch (Exception ex) { throw; }


        }

        #endregion

        #region method : 카메라 시퀀스 순서

        // Callback method for image display.



        #region method : step 1(카메라 초기화 및 설정)
        private void camera_connect_step1()
        {

            camstate.Text = "카메라 초기화";

            try
            {
                // Initialize StApi before using.
                api = new CStApiAutoInit();

                // Create a system object for device scan and connection.
                system = new CStSystem();

                // Create a camera device list object to store all the cameras.
                deviceList = new CStDeviceArray();

                // Create a DataStream list object to store all the data stream object related to the cameras.
                dataStreamList = new CStDataStreamArray();


                xml_name_list = Directory.GetFiles(".\\", "*.xml");
            }

            catch (Exception ex)
            {

                //연결 끊고 종료 부분 추가 예정
                camera_connect_step5();
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        #endregion

        #region method : step 2(카메라 연결 준비)
        private void camera_connect_step2()
        {
            CStDevice device = null;

            camstate.Text = "카메라 연결 준비";

            try
            {
                while (true)
                {
                    try
                    {
                        // Create a camera device object and connect to first detected device.
                        device = system.CreateFirstStDevice();
                    }
                    catch
                    {
                        if (deviceList.GetSize() == 0)
                        {
                            throw;
                        }

                        break;
                    }

                    // Add the camera into device object list for later usage.
                    deviceList.Register(device);




                    // Create a DataStream object for handling image stream data then add into DataStream list for later usage.
                    dataStreamList.Register(device.CreateStDataStream(0));





                    IStDataStream dataStream = dataStreamList[dataStreamList.GetSize() - 1];

                    // Create INodeMap object to access current setting of the camera.
                    nodeMapRemote = device.GetRemoteIStPort().GetINodeMap();

                    // Check if the camera has AE, AWB and AWB functions.
                    string[] autoFunctionNames = { EXPOSURE_AUTO, GAIN_AUTO, BALANCE_WHITE_AUTO };
                    bool[] isWritable = { false, false, false };

                    for (int i = 0; i < isWritable.Length; i++)
                    {
                        // User INode interface to acquire the setting of camera with function name.
                        INode node = nodeMapRemote.GetNode<INode>(autoFunctionNames[i]);
                        if (node.IsWritable)
                        {
                            isWritable[i] = true;
                        }
                    }

                }

                //if (calibration == true)
                //{
                //    for (uint i = 0; i < dataStreamList.GetSize(); i++)
                //    {

                //        string display_name = ".\\" + deviceList[i].GetIStDeviceInfo().DisplayName + ".xml";
                //        if (i == 0 && xml_name_list.Contains(display_name))
                //        {

                //            calibrationmatrix = load_xml(display_name);
                //            cameraMatrix_1 = calibrationmatrix[0];
                //            distCoeffs_1 = calibrationmatrix[1];
                //            newcameramatrix_1 = calibrationmatrix[2];



                //        }
                //        else if (i == 1 && xml_name_list.Contains(display_name + ".xml"))
                //        {
                //            calibrationmatrix = load_xml(display_name + ".xml");
                //            cameraMatrix_2 = calibrationmatrix[0];
                //            distCoeffs_2 = calibrationmatrix[1];
                //            newcameramatrix_2 = calibrationmatrix[2];
                //        }
                //        else if (i == 2 && xml_name_list.Contains(display_name + ".xml"))
                //        {
                //            calibrationmatrix = load_xml(display_name + ".xml");
                //            cameraMatrix_3 = calibrationmatrix[0];
                //            distCoeffs_3 = calibrationmatrix[1];
                //            newcameramatrix_3 = calibrationmatrix[2];
                //        }
                //    }
                //}
                check_calibration();

            }
            catch (Exception ex)
            {
                //카메라 연결 중지 부분 추가.
                camera_connect_step5();
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }

        }

        #endregion

        #region method : step 3(카메라 연결 후 이미지 획득)
        private void camera_connect_step3()
        {

            camstate.Text = "카메라 작동";

            // Start the image acquisition of the host side.
            dataStreamList.StartAcquisition();

            // Start the image acquisition of the camera side.
            deviceList.AcquisitionStart();

            mThread = new Thread(new ParameterizedThreadStart(camera_img_acquire)); // 스레드에 IParameters라는 파라미터 전달을 위해 ParameterizedThreadStart 사용

            GainAuto(nodeMapRemote);

            Form1 IP1 = this;

            object[] IParameters = new object[] { IP1 };
            misStopping = false;
            mThread.Start(IParameters);



        }

        #endregion

        #region method : step 4(카메라 연결 종료)
        private void camera_connect_step4()
        {
            camstate.Text = "카메라 이미지 출력";
            timerCameraviewer.Start();

        }

        #endregion

        #region method : step 5
        private void camera_connect_step5()
        {
            timerCameraviewer.Stop();
            camstate.Text = "카메라 작동 종료";

            try
            {



                misStopping = true;
                camstate.Text = "misstopping = true로 변경";

                if (mThread != null)
                {
                    camstate.Text = "mthread.join 작동 시작";

                    mThread.Join();
                    camstate.Text = "mthread.join 작동 끝";
                    //mThread = null;
                }

                mThread = null;

                // Stop the image acquisition of the camera side.
                deviceList.AcquisitionStop();

                // Stop the image acquisition of the host side.
                dataStreamList.StopAcquisition();


                camstate.Text = "카메라 연결 종료중";


                dataStreamList.Dispose();

                dataStreamList = null;

                deviceList.Dispose();

                deviceList = null;

                system.Dispose();

                api.Dispose();



                camstate.Text = "카메라 연결 종료 완료";








            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text);
            }

        }

        #endregion

        //#region method : step 6
        //private void camera_connect_step6()
        //{

        //}

        //#endregion

        #endregion

        #region method : 카메라 영상 획득
        private void camera_img_acquire(object aParameters)
        {
            Mat matImage;

            object[] IParameters = (object[])aParameters;

            Form1 IThis = (Form1)IParameters[0];


            for (; ; )
            {


                try
                {
                    if (IThis.misStopping)
                    {
                        //signaled to terminate thread, return.
                        MessageBox.Show("영상 획득 부분 misstopping 작동");
                        return;
                    }
                }

                catch (Exception e)
                {
                    MessageBox.Show("misstopping 작동 불능.");
                }

                // Retrieve data buffer of image data from any camera with a timeout of 5000ms.
                for (uint i = 0; i < deviceList.GetSize(); i++)
                {

                    using (CStStreamBuffer streamBuffer = dataStreamList[i].RetrieveBuffer(5000))
                    {

                        device_name = streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName;


                        // Check if the acquired data contains image data.
                        if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                        {

                            Mat distimg = new Mat();

                            IStImage stImage = streamBuffer.GetIStImage();




                            imageBuffer = CStApiDotNet.CreateStImageBuffer();


                            stillImageFiler = new CStStillImageFiler();



                            pixelFormatConverter = new CStPixelFormatConverter();


                            pixelFormatConverter.DestinationPixelFormat = eStPixelFormatNamingConvention.BGR8;

                            pixelFormatConverter.Convert(streamBuffer.GetIStImage(), imageBuffer);



                            stImage = imageBuffer.GetIStImage();//2448 x 2048;



                            width = (int)stImage.ImageWidth;
                            height = (int)stImage.ImageHeight;

                            byte[] imageData = stImage.GetByteArray();

                            matImage = new Mat(height, width, MatType.CV_8UC3);


                            Marshal.Copy(imageData, 0, matImage.Data, imageData.Length);


                            //roi 설정

                            //if (device_name == "FS-MCS500POE(22GD394)")
                            //{
                            //    rect = new Rect(720, 0, 1100, 2000);
                            //    //완료
                            //}
                            //else if (device_name == "FS-MCS500POE(22GD400)")
                            //{
                            //    rect = new Rect(680, 0, 1100, 2000);
                            //    //완료
                            //}
                            //else
                            //{
                            //    rect = new Rect(720, 0, 1050, 2000);


                            //}

                            //roi 추출 시 테스트용
                            //Mat show_img = new Mat(); // 임시 테스트용
                            //show_img = matImage.Clone();
                            //Cv2.Rectangle(show_img, rect, Scalar.Red, 10, LineTypes.AntiAlias);
                            //Cv2.Resize(show_img, show_img, new OpenCvSharp.Size(mWidth, mHeight));
                            //pictureBox2.Image = BitmapConverter.ToBitmap(show_img);
                            //matImage = matImage.SubMat(rect);
                            //rect = new Rect(500, 100, 1500, 1800);
                            //Cv2.Rectangle(matImage, rect, Scalar.Red, 10, LineTypes.AntiAlias);
                            //matImage = matImage.SubMat(rect);

                            //Cv2.Resize(matImage, matImage, new OpenCvSharp.Size(mWidth, mHeight));

                            //gray scale 변화 테스트용
                            //gray_img1 = changetogray(matImage);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(gray_img1);
                            //pictureBox1.Image = BitmapConverter.ToBitmap(gray_img1);
                            ////적응형 이진화 테스트용
                            //binary_img1 = Adaptive_biniarzation(gray_img1);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(binary_img1);
                            ////Morphology 테스트용
                            //Morphology_img1 = Morphology(binary_img1);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(Morphology_img1);


                            //gray_img1 = changetogray(matImage);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(gray_img1);

                            //Cv2.Threshold(gray_img1, binary_img1, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.BinaryInv);
                            //for (int check_block = 3; check_block < 500; check_block++)
                            //{
                            //    for (int limit_value = 0; limit_value < 500; limit_value++) 
                            //    {
                            //        if (check_block % 2 != 0)
                            //        {
                            //            Cv2.AdaptiveThreshold(gray_img1, binary_img1, 255, AdaptiveThresholdTypes.GaussianC | AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, check_block, limit_value);
                            //            Cv2.ImWrite("C:\\Users\\김남기\\source\\repos\\test\\test\\bin\\Debug\\net6.0-windows\\check_image\\" + check_block + "_" + limit_value + ".jpg", binary_img1);
                            //        }
                            //    }

                            //}

                            //Cv2.AdaptiveThreshold(gray_img1, binary_img1, 255, AdaptiveThresholdTypes.GaussianC | AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 175,0);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(binary_img1);

                            //Mat opening = new Mat();
                            //int kernel_size = 5;
                            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(kernel_size, kernel_size));
                            //Cv2.MorphologyEx(binary_img1, opening, MorphTypes.Open, kernel, iterations: 3);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(opening);

                            //Mat surebg = new Mat();
                            //Cv2.Dilate(opening, surebg, kernel, iterations: 3);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(surebg);

                            //Mat distance = new Mat();
                            //Cv2.DistanceTransform(opening, distance, DistanceTypes.L2, DistanceTransformMasks.Mask5);
                            //Cv2.Normalize(distance, distance, 0, 255, NormTypes.MinMax);

                            //Mat distance_img = new Mat();
                            //distance.ConvertTo(distance_img, MatType.CV_8U);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(distance_img);

                            //Mat surefg = new Mat();
                            //distance.ConvertTo(distance, MatType.CV_8UC1);


                            //double maxVal;
                            //OpenCvSharp.Cv2.MinMaxLoc(distance, out _, out maxVal);

                            //double thresholdValue = 1 * maxVal;

                            //Cv2.Threshold(distance, surefg, thresholdValue, 255, ThresholdTypes.Binary);


                            ////Cv2.AdaptiveThreshold(distance, surefg, 255, AdaptiveThresholdTypes.GaussianC | AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 9, 1);
                            //Mat surfg_img = new Mat();

                            //surefg.ConvertTo(surfg_img, MatType.CV_8U);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(surfg_img);


                            //Mat unknown = new Mat();
                            //Cv2.Subtract(surebg, surefg, unknown, mask: null, MatType.CV_8U);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(unknown);

                            //Mat labels = new Mat();
                            //int numLabels = Cv2.ConnectedComponents(surefg, labels);

                            //labels = labels + 1;
                            //Mat markers = new Mat();
                            //labels.ConvertTo(markers, MatType.CV_32S);

                            //Mat unknownMask = new Mat();
                            //Cv2.Compare(unknown, new Scalar(0), unknownMask, CmpType.EQ);

                            //markers.SetTo(255, unknownMask);

                            ////Mat markers = new Mat();
                            ////surefg.ConvertTo(surefg, MatType.CV_8UC1);
                            ////Cv2.ConnectedComponents(surefg, markers);
                            ////markers = markers + 1;
                            ////Mat unknownMask = new Mat();
                            ////Cv2.Compare(unknown, new Scalar(255), unknownMask, CmpType.EQ);

                            ////for (int j = 0; j < markers.Rows; j++)
                            ////{
                            ////    for (int k = 0; k < markers.Cols; k++)
                            ////    {
                            ////        if (unknownMask.Get<byte>(j, k) == 255)
                            ////        {
                            ////            markers.Set<int>(j, k, 0);
                            ////        }
                            ////    }
                            ////}

                            //Mat marker_img = new Mat();
                            //markers.ConvertTo(marker_img, MatType.CV_8U);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(marker_img);

                            //Mat water_img = new Mat();
                            //water_img = matImage.Clone();

                            //Cv2.Watershed(water_img, markers);

                            //for (int j = 0; j < markers.Rows; j++)
                            //{
                            //    for (int k = 0; k < markers.Cols; k++)
                            //    {
                            //        if (markers.Get<int>(j, k) == -1)
                            //        {
                            //            water_img.Set<Vec3b>(j, k, new Vec3b(0, 0, 255));
                            //        }
                            //    }
                            //}


                            //pictureBox2.Image = BitmapConverter.ToBitmap(water_img);

                            //Mat segmented_img = matImage.Clone();
                            //segmented_img = segment_object(segmented_img);

                            //pictureBox2.Image = BitmapConverter.ToBitmap(segmented_img);

                            //wraping image 테스트
                            //Mat test = new Mat();


                            //test = wraping_img(matImage);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(test);

                            if (i == 0)
                            {
                                image1 = matImage.Clone();
                                //image1 = binary_img1.Clone();

                            }
                            else if (i == 1)
                            {
                                image2 = matImage.Clone();
                            }
                            else
                            {
                                image3 = matImage.Clone();
                            }

                            //Rect rect = new Rect(500, 100, 1500, 1800);

                            //matImage = matImage.SubMat(rect);

                            ////영상 이미지에 그림 그리기(ROI 설정)
                            ////Cv2.Rectangle(matImage, rect, Scalar.Red, 10, LineTypes.AntiAlias);                        

                            //Cv2.Resize(matImage, matImage, new OpenCvSharp.Size(mWidth, mHeight));

                            //if (calibration == true)
                            //{
                            //    if (i == 0)
                            //    {

                            //        //Cv2.Undistort(matImage, matImage, cameraMatrix_1, distCoeffs_1, newcameramatrix_1);

                            //        Cv2.Undistort(matImage, distimg, cameraMatrix, distCoeffs, newcameramatrix);

                            //        Cv2.Resize(distimg, distimg, new OpenCvSharp.Size(mWidth, mHeight));

                            //        image1 = distimg.Clone();

                            //    }
                            //    else if (i == 1)
                            //    {

                            //        //Cv2.Undistort(matImage, matImage, cameraMatrix_2, distCoeffs_2, newcameramatrix_2);
                            //        Cv2.Undistort(matImage, distimg, cameraMatrix, distCoeffs, newcameramatrix);

                            //        Cv2.Resize(distimg, distimg, new OpenCvSharp.Size(mWidth, mHeight));

                            //        image2 = distimg.Clone();
                            //    }
                            //    else
                            //    {
                            //        //Cv2.Undistort(matImage, matImage, cameraMatrix_3, distCoeffs_3, newcameramatrix_3);
                            //        Cv2.Undistort(matImage, distimg, cameraMatrix, distCoeffs, newcameramatrix);

                            //        Cv2.Resize(distimg, distimg, new OpenCvSharp.Size(mWidth, mHeight));
                            //        image3 = distimg.Clone();
                            //    }
                            //}
                            //else
                            //{
                            //    pictureBox1.Image = BitmapConverter.ToBitmap(matImage);
                            //}


                            //WriteTextBox(textBox1, "BlockId=" + streamBuffer.GetIStStreamBufferInfo().FrameID + "\r\n");
                            //WriteTextBox(textBox1, " Size:" + stImage.ImageWidth + " x " + stImage.ImageHeight + "\r\n");
                            //WriteTextBox(textBox1, " First byte =" + imageData[0] + Environment.NewLine + "\r\n");



                            ////MessageBox.Show("CHECK_image_accept");
                            //image1 = matImage.Clone();

                            ////Cv2.Undistort(matImage, image2, cameraMatrix, distCoeffs, newcameramatrix);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(image2);
                        }
                        else
                        {
                            // If the acquired data contains no image data.
                            //WriteTextBox(textBox1, "Image data does not exist \r\n");

                        }


                    }


                    //merge_image_2(image1, image2);

                    //Cv2.Resize(merge, merge, new OpenCvSharp.Size(mWidth, mHeight));

                    //pictureBox1.Image = BitmapConverter.ToBitmap(merge);
                }
            }
        }

        #endregion

        #region 이미지 분할(전경 추출)

        //와이어 이미지 분할v1(윤곽선)
        private Mat segment_object_wire(Mat image)
        {
            Rect roi = new Rect(300, 0, image.Width - 300, image.Height);
            image = image.SubMat(roi);

            // 그레이스케일로 변환
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            


            // 가장자리 감지
            Mat edges = new Mat();
            Cv2.Canny(gray, edges, 150, 200);
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

            //// 결과 출력

            //Mat[] images = { outsidepointimage, rectangle_image, edges, contouriamge, segmentedObject };
            //for (int i = 0; i < images.Length; i++)
            //{
            //    Cv2.Resize(images[i], images[i], new Size(1000, 600));
            //}
            //string[] titles = { "outerpoint", "rectangle", "canny", "Contours", "segment" };

            //for (int i = 0; i < images.Length; i++)
            //{
            //    Cv2.NamedWindow(titles[i], WindowFlags.AutoSize);
            //    Cv2.ImShow(titles[i], images[i]);
            //}

            //Cv2.ImShow("outerpoint", outsidepointimage);
            //Cv2.ImShow("rectangle", rectangle_image);
            //Cv2.ImShow("canny", edges);
            //Cv2.ImShow("Contours",contouriamge);


            //Cv2.WaitKey(0);
            //Cv2.DestroyAllWindows();

            return segmentedObject;

        }

        private Mat segment_object(Mat image)
        {
            Mat segmentObject = new Mat();


            //컬러 이미지 샤프닝
            Mat kernel = new Mat(3, 3, MatType.CV_32F, new float[] {
            -1, -1, -1,
            -1, 9, -1,
            -1, -1, -1
        });

            // 컬러 이미지를 BGR 채널로 분리
            Mat[] channels = Cv2.Split(image);

            // 각 채널에 필터 커널 적용
            for (int i = 0; i < channels.Length; i++)
            {
                Cv2.Filter2D(channels[i], channels[i], -1, kernel);
            }

            // 채널을 다시 합쳐서 컬러 이미지로 복원
            Mat sharpenedImage = new Mat();
            Cv2.Merge(channels, sharpenedImage);

            // GrabCut 알고리즘을 사용하여 이미지에서 전경 추출
            Rect rectangle = new Rect(5, 0, image.Cols - 15, image.Rows); // 전경 추출을 위한 사각형 영역 설정
            Mat result = new Mat();
            Mat bgModel = new Mat();
            Mat fgModel = new Mat();
            Cv2.GrabCut(sharpenedImage, result, rectangle, bgModel, fgModel, 1, GrabCutModes.InitWithRect); // GrabCut 알고리즘을 사용하여 전경 추출

            // 전경 추출 결과를 이진화
            Mat foregroundMask = new Mat();
            Cv2.Compare(result, new Scalar(3), foregroundMask, CmpType.EQ); // 전경으로 판단되는 부분을 이진화

            // 전경 추출 결과 적용
            Mat foregroundImage = new Mat(image.Size(), MatType.CV_8UC3, Scalar.Black); // 검은색 배경 이미지 생성
            image.CopyTo(foregroundImage, foregroundMask); // 전경으로 판단되는 부분만 복사

            // 객체의 외곽선 추출

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(foregroundMask, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 외곽선 그리기
            Mat contourImage = new Mat(image.Size(), MatType.CV_8UC3, Scalar.Black);
            Cv2.DrawContours(contourImage, contours, -1, Scalar.White, thickness: 2, hierarchy: hierarchy);

            // 가장 큰 외곽선 찾기
            int largestContourIndex = -1;
            double largestContourArea = 0;

            for (int i = 0; i < contours.Length; i++)
            {
                double contourArea = Cv2.ContourArea(contours[i]);
                if (contourArea > largestContourArea)
                {
                    largestContourArea = contourArea;
                    largestContourIndex = i;
                }
            }

            // 외곽선 근사화
            OpenCvSharp.Point[] approxCurve = Cv2.ApproxPolyDP(contours[largestContourIndex], Cv2.ArcLength(contours[largestContourIndex], true) * 0.02, true);

            // 네 꼭지점 좌표 출력 및 시각화
            Mat visualizationImage = image.Clone();
            foreach (OpenCvSharp.Point point in approxCurve)
            {
                Cv2.Circle(visualizationImage, point, 5, Scalar.Red, -1);
                Cv2.PutText(visualizationImage, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            }

            //가장 바깥쪽 점들 추출
            OpenCvSharp.Point[] outerPoints = GetOuterPoints(approxCurve, image);

            int outpoint_count = outerPoints.Length;

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

            image.CopyTo(segmentObject, mask);

            WriteTextBox(textBox1, $"outerpoint_num={outpoint_count}");

            //return outsidepointimage;
            return segmentObject;
        }

        #endregion

        #region 가장 바깥쪽의 점 추출 함수
        //// 가장 바깥쪽에 있는 점들 추출
        //static OpenCvSharp.Point[] GetOuterPoints(OpenCvSharp.Point[] points,Mat image)
        //{
        //    // 좌상단, 우상단, 우하단, 좌하단 점을 저장할 변수 초기화
        //    OpenCvSharp.Point topLeft = new OpenCvSharp.Point(int.MaxValue, int.MaxValue);
        //    OpenCvSharp.Point topRight = new OpenCvSharp.Point(int.MinValue, int.MaxValue);
        //    OpenCvSharp.Point bottomRight = new OpenCvSharp.Point(int.MinValue, int.MinValue);
        //    OpenCvSharp.Point bottomLeft = new OpenCvSharp.Point(int.MaxValue, int.MinValue);

        //    // 좌상단, 우상단, 우하단, 좌하단 점 찾기

        //    int centerX = image.Width / 2;
        //    int centerY = image.Height / 2;
        //    foreach (OpenCvSharp.Point point in points)
        //    {
        //        // 좌상단
        //        if ((point.X <= topLeft.X && point.Y <= topLeft.Y) && (point.X <= centerX && point.Y <= centerY))
        //            topLeft = point;
        //        // 우상단
        //        else if ((point.X >= topRight.X && point.Y <= topRight.Y) && (point.X >= centerX && point.Y <= centerY))
        //            topRight = point;
        //        // 우하단
        //        else if ((point.X >= bottomRight.X && point.Y >= bottomRight.Y) && (point.X >= centerX && point.Y >= centerY))
        //            bottomRight = point;
        //        // 좌하단
        //        else if ((point.X <= bottomLeft.X && point.Y >= bottomLeft.Y) && (point.X <= centerX && point.Y >= centerY))
        //            bottomLeft = point;
        //    }

        //    if (topLeft == null)
        //    {
        //        topLeft = bottomLeft;
        //        topLeft.Y = 0;
        //    }

        //    else if (topRight == null)
        //    {
        //        topRight = bottomRight;
        //        topRight.Y = 0;
        //    }
        //    else if (bottomRight == null)
        //    {
        //        bottomRight = topRight;
        //        bottomRight.Y = image.Height;
        //    }
        //    else if (bottomLeft == null)
        //    {
        //        bottomLeft = topLeft;
        //        bottomLeft.Y = image.Height;
        //    }


        //    // 추출된 네 개의 점을 배열로 반환
        //    return new OpenCvSharp.Point[] { topLeft, topRight, bottomRight, bottomLeft };
        //}

        //좌상단,우상단 잡고 같은 x좌표상의 점 찍기
        static OpenCvSharp.Point[] GetOuterPoints(OpenCvSharp.Point[] points, Mat image)
        {
            // 좌상단, 우상단, 우하단, 좌하단 점을 저장할 변수 초기화
            OpenCvSharp.Point topLeft = new OpenCvSharp.Point(int.MaxValue, int.MaxValue);
            OpenCvSharp.Point topRight = new OpenCvSharp.Point(int.MinValue, int.MaxValue);
            OpenCvSharp.Point bottomRight = new OpenCvSharp.Point(int.MinValue, int.MinValue);
            OpenCvSharp.Point bottomLeft = new OpenCvSharp.Point(int.MaxValue, int.MinValue);

            // 좌상단, 우상단, 우하단, 좌하단 점 찾기

            int centerX = image.Width / 2;
            int centerY = image.Height / 2;
            foreach (OpenCvSharp.Point point in points)
            {
                // 좌상단
                if ((point.X <= topLeft.X && point.Y <= topLeft.Y) && (point.X <= centerX && point.Y <= centerY))
                    topLeft = point;
                // 우상단
                else if ((point.X >= topRight.X && point.Y <= topRight.Y) && (point.X >= centerX && point.Y <= centerY))
                    topRight = point;
            }

            bottomLeft = topLeft;
            bottomRight = topRight;

            //같은 x좌표상의 맨끝 맨 아래로 점을 지정.
            topLeft.Y = 0;
            topRight.Y = 0;
            bottomLeft.Y = image.Height;
            bottomRight.Y = image.Height;



            // 추출된 네 개의 점을 배열로 반환
            return new OpenCvSharp.Point[] { topLeft, topRight, bottomRight, bottomLeft };
        }

        #endregion

        #region method : 카메라 캘리브레이션 확인 및 값 획득

        void check_calibration()
        {
            int check_count = 0;
            bool xml_check = false;
            string display_name;
            for (uint i = 0; i < deviceList.GetSize(); i++)
            {
                display_name = ".\\" + deviceList[i].GetIStDeviceInfo().DisplayName + ".xml";
                xml_check = xml_name_list.Contains(display_name);
                if (i == 0 && xml_check == true)
                {

                    calibrationmatrix = load_xml(display_name);
                    cameraMatrix_1 = calibrationmatrix[0];
                    distCoeffs_1 = calibrationmatrix[1];
                    newcameramatrix_1 = calibrationmatrix[2];
                    check_count += 1;
                    WriteTextBox(textBox1, "camera_1 calibration done \r\n");

                }
                else if (i == 1 && xml_check == true)
                {
                    calibrationmatrix = load_xml(display_name);
                    cameraMatrix_2 = calibrationmatrix[0];
                    distCoeffs_2 = calibrationmatrix[1];
                    newcameramatrix_2 = calibrationmatrix[2];
                    check_count += 1;
                    WriteTextBox(textBox1, "camera_2 calibration done \r\n");
                }
                else if (i == 2 && xml_check == true)
                {
                    calibrationmatrix = load_xml(display_name);
                    cameraMatrix_3 = calibrationmatrix[0];
                    distCoeffs_3 = calibrationmatrix[1];
                    newcameramatrix_3 = calibrationmatrix[2];
                    check_count += 1;
                    WriteTextBox(textBox1, "camera_3 calibration done \r\n");
                }

                if (xml_check != true)
                {
                    WriteTextBox(textBox1, $"camera_{i} calibration need \r\n");
                }



            }
            if (check_count == deviceList.GetSize())
            {
                calibration = true;
                WriteTextBox(textBox1, "All camera calibration finished \r\n");
            }
            else
            {
                WriteTextBox(textBox1, "All camera calibration unfinished \r\n");
            }
        }


        #endregion

        #region method : 카메라 캘리브레이션
        private void camera_calibration()
        {
            //Termination criteria
            var criteria = new TermCriteria(CriteriaTypes.MaxIter | CriteriaTypes.Eps, 30, 0.001);

            // 일반적인 체크 보드 캘리브레이션
            int wc = 3; // 체스 보드 가로 개수
            int hc = 4; // 체스 보드 세로 개수

            Mat img;

            Mat gray = new Mat();

            Mat objp = new Mat(wc * hc, 3, MatType.CV_32FC1); // object point를 3차원으로 cv_32fc1 형식으로 만든다.

            objp.SetTo(0); // objp 내부의 값들을 0으로 초기화한다.

            // 반복문으로 3차원 좌표인 object point를 구한다.(체크무늬의 거리가 동일하게 구성되어 있으므로 한 코너를 0,0으로 잡았을 때 object point를 구할 수 있다.)
            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < wc; j++)
                {
                    objp.At<float>(i * wc + j, 0) = j;
                    objp.At<float>(i * wc + j, 1) = i;
                    objp.At<float>(i * wc + j, 2) = 0;
                }
            }

            //Console.WriteLine("Object Points (OpenCvSharp):");
            for (int i = 0; i < objp.Rows; i++)
            {
                var point = new Point3f(objp.At<float>(i, 0), objp.At<float>(i, 1), objp.At<float>(i, 2));
                Console.WriteLine($"Point {i}: {point}");
            }

            //object point랑 image point가 들어갈 수 있는 Mat 형식의 List를 선언한다.
            List<Mat> objpoints = new List<Mat>();
            List<Mat> imgpoints = new List<Mat>();


            string dirpath = "Capture\\" + device_name + "\\";
            //string dirpath = "C:\\Users\\김남기\\Downloads\\옮길거\\옮길거\\first\\first";
            string[] imagePaths = Directory.GetFiles(dirpath, "*.jpg"); // .jpg 확장자로 되어 있는 모든 이미지들의 경로를 iamgepaths에 집어넣는다.

            Console.WriteLine("ImagePaths:");
            for (int i = 0; i < imagePaths.Length; i++)
            {
                WriteTextBox(textBox1, imagePaths[i]);
            }

            foreach (var imagePath in imagePaths)
            {
                img = new Mat(imagePath, ImreadModes.Color);


                WriteTextBox(textBox1, imagePath + "convert gray start...");
                Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);//그레이 스케일로 변환

                // Find the chess board corners(코너 점 찾기)
                bool ret = Cv2.FindChessboardCorners(gray, new OpenCvSharp.Size(wc, hc), out Point2f[] corners);

                // If found, add object points, image points (after refining them)
                if (ret)
                {
                    WriteTextBox(textBox1, "find corner and calculating...");


                    Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1), criteria); // 코너 점 검출
                    objpoints.Add(objp);
                    //Mat Corners = new Mat(corners.Length, 1, MatType.CV_32FC2);
                    imgpoints.Add(new Mat(corners.Length, 1, MatType.CV_32FC2, corners));

                    // Draw and display the corners
                    //Cv2.NamedWindow("img", WindowFlags.AutoSize);
                    Cv2.DrawChessboardCorners(img, new OpenCvSharp.Size(wc, hc), corners, ret);
                    pictureBox1.Image = BitmapConverter.ToBitmap(img);

                    Cv2.WaitKey(500);
                }
                else
                {
                    WriteTextBox(textBox1, "failed find board corners. please retry");
                }
            }

            OpenCvSharp.Size graySize = gray.Size();
            cameraMatrix = new Mat();
            distCoeffs = new Mat();
            Mat[] rvecs;
            Mat[] tvecs;

            Cv2.CalibrateCamera(objpoints, imgpoints, graySize, cameraMatrix, distCoeffs, out rvecs, out tvecs);

            img = Cv2.ImRead(imagePaths[0]);
            OpenCvSharp.Size imageSize = new OpenCvSharp.Size(img.Width, img.Height);
            newcameramatrix = Cv2.GetOptimalNewCameraMatrix(cameraMatrix, distCoeffs, imageSize, 0, imageSize, out var validPixROI);
            Mat dst = new Mat();
            Mat dst2 = new Mat();

            Cv2.Undistort(img, dst, cameraMatrix, distCoeffs);
            Cv2.Undistort(img, dst2, cameraMatrix, distCoeffs, newcameramatrix);

            pictureBox1.Image = BitmapConverter.ToBitmap(dst);
            Cv2.WaitKey(1000);
            pictureBox2.Image = BitmapConverter.ToBitmap(dst2);

            Cv2.WaitKey();

            double mean_error = 0;
            for (int i = 0; i < objpoints.Count; i++)
            {
                Mat imgpoints2 = new Mat();
                Cv2.ProjectPoints(objpoints[i], rvecs[i], tvecs[i], cameraMatrix, distCoeffs, imgpoints2);
                double error = Cv2.Norm(imgpoints[i], imgpoints2, NormTypes.L2) / imgpoints2.Rows;
                mean_error += error;
            }
            MessageBox.Show("Total error: {0}" + mean_error / objpoints.Count);

            MessageBox.Show("exit want to enter");

            string xml_save_name = device_name + ".xml";
            //string xml_save_name = streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName + ".xml";

            save_xml(cameraMatrix, distCoeffs, newcameramatrix, xml_save_name);

        }
        #endregion

        #region method : xml 저장 부분

        private void save_xml(Mat cameraMatrix, Mat distortioncoeffs, Mat newcameramatrix, string save_name)
        {
            XDocument xmlDoc = new XDocument();

            // 루트 엘리먼트 생성
            XElement rootElement = new XElement("CameraCalibration");
            xmlDoc.Add(rootElement);

            // 카메라 매트릭스 저장
            XElement cameraMatrixElement = new XElement("CameraMatrix");
            rootElement.Add(cameraMatrixElement);

            // 카메라 매트릭스의 행과 열 수 저장
            cameraMatrixElement.Add(new XAttribute("rows", cameraMatrix.Rows));
            cameraMatrixElement.Add(new XAttribute("cols", cameraMatrix.Cols));

            string cameramatrixdata = "";



            // 카메라 매트릭스 값 저장
            for (int row = 0; row < cameraMatrix.Rows; row++)
            {


                for (int col = 0; col < cameraMatrix.Cols; col++)
                {
                    double value = cameraMatrix.Get<double>(row, col);
                    if (col < cameraMatrix.Cols - 1)
                    {
                        cameramatrixdata += value + "\t";
                    }
                    else
                    {
                        cameramatrixdata += value;
                    }

                }

                cameramatrixdata += "\n";
            }

            cameraMatrixElement.Add(new XElement("data",
                new XCData(cameramatrixdata)));

            //왜곡 계수 매트릭스 저장
            XElement distcoeffselement = new XElement("distcoeffsMatrix");
            rootElement.Add(distcoeffselement);

            // 왜곡계수 매트릭스의 행과 열 수 저장
            distcoeffselement.Add(new XAttribute("rows", distCoeffs.Rows));
            distcoeffselement.Add(new XAttribute("cols", distCoeffs.Cols));

            string distcoeffsdata = "";


            // 왜곡 계수 매트릭스 값 저장
            for (int row = 0; row < distortioncoeffs.Rows; row++)
            {


                for (int col = 0; col < distortioncoeffs.Cols; col++)
                {
                    double value = distortioncoeffs.Get<double>(row, col);
                    if (col < distortioncoeffs.Cols - 1)
                    {
                        distcoeffsdata += value + "\t";
                    }
                    else
                    {
                        distcoeffsdata += value;
                    }

                }

                distcoeffsdata += "\n";
            }

            distcoeffselement.Add(new XElement("data",
                new XCData(distcoeffsdata)));

            //new camera 매트릭스 저장
            XElement newcameramatrixelement = new XElement("newcameramatrix");
            rootElement.Add(newcameramatrixelement);

            //new camera 매트릭스 행과 열 수 저장
            newcameramatrixelement.Add(new XAttribute("rows", newcameramatrix.Rows));
            newcameramatrixelement.Add(new XAttribute("cols", newcameramatrix.Cols));

            string newcameramatrixdata = "";


            //new camera 매트릭스 값 저장
            for (int row = 0; row < newcameramatrix.Rows; row++)
            {


                for (int col = 0; col < newcameramatrix.Cols; col++)
                {
                    double value = newcameramatrix.Get<double>(row, col);
                    if (col < newcameramatrix.Cols - 1)
                    {
                        newcameramatrixdata += value + "\t";
                    }
                    else
                    {
                        newcameramatrixdata += value;
                    }

                }

                newcameramatrixdata += "\n";
            }

            newcameramatrixelement.Add(new XElement("data",
                new XCData(newcameramatrixdata)));


            xmlDoc.Save(save_name);

        }

        #endregion

        #region method : xml 불러오기


        private Mat[] load_xml(string load_name)
        {

            Mat[] matrix = new Mat[3];

            // xml 문서 로드
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(load_name);

            //cameramatrix 데이터 읽어오기
            XmlNode cameraMatrixNode = xmlDoc.SelectSingleNode("//CameraMatrix/data");
            string cameraMatrixData = cameraMatrixNode.InnerText;

            //CameraMatrix 문자열을 Mat 형식으로 변환
            string[] cameraMatrixRows = cameraMatrixData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int rows = cameraMatrixRows.Length;
            int cols = cameraMatrixRows[0].Split('\t', StringSplitOptions.RemoveEmptyEntries).Length;

            cameraMatrix = new Mat(rows, cols, MatType.CV_64FC1);


            for (int i = 0; i < rows; i++)
            {
                string[] values = cameraMatrixRows[i].Split('\t', StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < cols; j++)
                {
                    double value;
                    if (double.TryParse(values[j], out value))
                    {
                        cameraMatrix.Set<double>(i, j, value);
                    }
                    else
                    {

                    }
                }
            }

            //distcoeffs 데이터 읽어오기
            XmlNode distcoeffsNode = xmlDoc.SelectSingleNode("//distcoeffsMatrix/data");
            string distcoeffsData = distcoeffsNode.InnerText;

            //CameraMatrix 문자열을 Mat 형식으로 변환
            string[] distcoeffsRows = distcoeffsData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            rows = distcoeffsRows.Length;
            cols = distcoeffsRows[0].Split('\t', StringSplitOptions.RemoveEmptyEntries).Length;

            distCoeffs = new Mat(rows, cols, MatType.CV_64FC1);


            for (int i = 0; i < rows; i++)
            {
                string[] values = distcoeffsRows[i].Split('\t', StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < cols; j++)
                {
                    double value;
                    if (double.TryParse(values[j], out value))
                    {
                        distCoeffs.Set<double>(i, j, value);
                    }
                    else
                    {

                    }
                }
            }

            //newcameramatrix 데이터 읽어오기
            XmlNode newcameramatrixNode = xmlDoc.SelectSingleNode("//newcameramatrix/data");
            string newcameramatrixData = newcameramatrixNode.InnerText;

            //CameraMatrix 문자열을 Mat 형식으로 변환
            string[] newcameramatrixRows = newcameramatrixData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            rows = newcameramatrixRows.Length;
            cols = newcameramatrixRows[0].Split('\t', StringSplitOptions.RemoveEmptyEntries).Length;

            newcameramatrix = new Mat(rows, cols, MatType.CV_64FC1);


            for (int i = 0; i < rows; i++)
            {
                string[] values = newcameramatrixRows[i].Split('\t', StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < cols; j++)
                {
                    double value;
                    if (double.TryParse(values[j], out value))
                    {
                        newcameramatrix.Set<double>(i, j, value);
                    }
                    else
                    {

                    }
                }
            }
            matrix[0] = cameraMatrix;
            matrix[1] = distCoeffs;
            matrix[2] = newcameramatrix;

            return matrix;
        }
        #endregion

        #region method : color to gray

        private Mat changetogray(Mat img)
        {
            Mat gray = new Mat();

            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }


        #endregion

        #region method : 적응형 이진화 알고리즘(thresholdtypes.binary 뒤의 blocksize와 임계값 수정 예약)

        private Mat Adaptive_biniarzation(Mat gray_image)
        {
            Mat binary = new Mat();

            //적응형이 아닌 이진화 테스트
            //Cv2.Threshold(gray_image, binary, 127, 255, ThresholdTypes.Otsu);

            Cv2.AdaptiveThreshold(gray_image, binary, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 25, 5);

            return binary;
        }

        #endregion

        #region method : Morphology(노이즈 제거) - 추후 조정 필요

        private Mat Morphology(Mat binary_img)
        {
            Mat Mor_img = new Mat();
            Mat opening = new Mat();
            Mat bg = new Mat();
            Mat fg = new Mat();
            Mat border = new Mat();
            Mat border2 = new Mat();

            int kernelsize = 5;
            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(kernelsize, kernelsize));

            OpenCvSharp.Point anchor = new OpenCvSharp.Point(-1, -1);
            // element - 모폴로지를 적용하는 커널
            // point - 커널 중심점 지정
            // iteration(맨 마지막) - 모폴로지를 적용하는 횟수(기본적으로 1)
            //Cv2.Dilate(binary_img, dilate, Kernel, new OpenCvSharp.Point(3, 3), 3);
            //Cv2.Erode(binary_img, erode, Kernel, new OpenCvSharp.Point(3, 3), 3);

            //Cv2.Dilate(binary_img, dilate, Kernel, anchor);

            //Cv2.Erode(dilate, Mor_img, Kernel, anchor);

            //Cv2.MorphologyEx(binary_img, Mor_img, MorphTypes.Open, Kernel);

            Cv2.MorphologyEx(binary_img, Mor_img, MorphTypes.Close, Kernel, anchor, 1);


            //Cv2.HConcat(new Mat[] { dilate, erode }, Mor_img);

            //Mat test = new Mat();



            return Mor_img;
        }


        #endregion

        #region method : 거리변환
        private Mat dist_trans(Mat Mor_image)
        {
            Mat distance = new Mat();

            Cv2.DistanceTransform(Mor_image, distance, DistanceTypes.L2, DistanceTransformMasks.Mask3);

            //Cv2.Normalize(distance, distance, 0, 1.0, NormTypes.MinMax);

            return distance;
        }
        #endregion

        #region method : 변환 행렬 이용 원기둥 평면 변환

        private Mat wraping_img(Mat img, int r)
        {

            Mat wrap = new Mat();
            //Point2f[] srcpoint1 = new Point2f[4] {
            //   new Point2f(365,0),                       // 좌상단 원점
            //   new Point2f(500,0),               // 우상단
            //   new Point2f(500,300),      // 우하단
            //   new Point2f(365,300)               // 좌하단
            //};
            //Point2f[] dstpoint1 = new Point2f[4] {
            //    new Point2f(0,0),                       // 좌상단 평면 좌표
            //    new Point2f(205,-25),          // 우상단
            //    new Point2f(205,320),          // 우하단
            //    new Point2f(0,300)          // 좌하단
            //};

            Point2f[] srcpoint1 = new Point2f[4] {
               new Point2f(15,10),                       // 좌상단 원점
               new Point2f(450,10),               // 우상단
               new Point2f(450,300),      // 우하단
               new Point2f(15,300)               // 좌하단
            };
            Point2f[] dstpoint1 = new Point2f[4] {
                new Point2f(0,0),                       // 좌상단 평면 좌표
                new Point2f(500,0),          // 우상단
                new Point2f(500,300),          // 우하단
                new Point2f(0,300)          // 좌하단
            };


            //// 변환 전 좌표에 원 그리기
            //foreach (Point2f point in srcpoint1)
            //{
            //    Cv2.Circle(image1, (int)point.X, (int)point.Y, 15, Scalar.Blue, -1);
            //}

            //// 변환 후 좌표에 원 그리기
            //foreach (Point2f point in dstpoint1)
            //{
            //    Cv2.Circle(image1, (int)point.X, (int)point.Y, 15, Scalar.Red, -1);
            //}

            Mat perspectivematrix = Cv2.GetPerspectiveTransform(srcpoint1, dstpoint1);

            //Cv2.WarpPerspective(img, wrap, perspectivematrix, img.Size(), flags: InterpolationFlags.Linear,
            //borderMode: BorderTypes.Replicate, borderValue: new Scalar(0, 0, 0));

            Cv2.WarpPerspective(img, wrap, perspectivematrix, img.Size());

            int dx = 0;
            int dy = 0;

            Mat translationMatrix = new Mat(2, 3, MatType.CV_32FC1, new float[]
            {
                1, 0, dx,
            0, 1, dy
        });

            Cv2.WarpAffine(wrap, wrap, translationMatrix, new OpenCvSharp.Size(img.Width, img.Height));

            return wrap;

        }

        #endregion

        #region method : image labeling

        private Mat object_labeling(Mat img_mor)
        {
            Mat labels = new Mat();

            int objectcount = Cv2.ConnectedComponents(img_mor, labels);

            return labels;
        }

        #endregion

        #region method : image segmentation

        private Mat segmentation(Mat img)
        {
            Mat seg_img = new Mat();


            return seg_img;
        }

        #endregion

        #region timer : 카메라 시퀀스
        private void timerCameraSequence_Tick(object sender, EventArgs e)
        {
            timerCameraSequence.Stop();
            switch (mStep)
            {
                case 1:
                    camera_connect_step1();
                    break;

                case 2:
                    camera_connect_step2();
                    break;

                case 3:
                    camera_connect_step3();
                    break;

                case 4:
                    camera_connect_step4();
                    return;

                case 5:
                    camera_connect_step5();
                    break;

                default:
                    Close();
                    return;
            }

            mStep++;
            timerCameraSequence.Start();
        }
        #endregion

        #region timer:카메라 이미지 출력
        private void timerCameraviewer_Tick(object sender, EventArgs e)
        {
            Mat undistimg1 = new Mat();
            Mat undistimg2 = new Mat();
            Mat undistimg3 = new Mat();
            Rect distRect = new Rect();


            if (calibration == false)
            {
                if (image1.Rows != 0 && image1.Cols != 0)
                {
                    //분할 시험
                    Mat segmentimg = segment_object_wire(image1);

                    pictureBox2.Image = BitmapConverter.ToBitmap(segmentimg);


                    //takepicture_timesecond.Interval = 500;
                    //takepicture_timesecond.Start();
                    pictureBox1.Image = BitmapConverter.ToBitmap(image1);
                    ////gray scale 변화 테스트용
                    //gray_img1 = changetogray(image1);
                    ////pictureBox2.Image = BitmapConverter.ToBitmap(gray_img1);
                    ////적응형 이진화 테스트용
                    //binary_img1 = Adaptive_biniarzation(gray_img1);
                    ////pictureBox2.Image = BitmapConverter.ToBitmap(binary_img1);
                    ////Morphology 테스트용
                    //Morphology_img1 = Morphology(binary_img1);
                    //pictureBox2.Image = BitmapConverter.ToBitmap(Morphology_img1);
                    ////wraping image 테스트
                    //Mat test = new Mat();
                    //test = wraping_img(image1);
                    //pictureBox2.Image = BitmapConverter.ToBitmap(test);
                    

                }

            }

            else
            {
                for (uint i = 0; i < deviceList.GetSize(); i++)
                {
                    string display_name = deviceList[i].GetIStDeviceInfo().DisplayName;

                    if (display_name == "FS-MCS500POE(22GD394)")
                    {
                        //distRect = new Rect(185, 35, 175, 250);
                        //rect = new Rect(100, 100, 200,200);

                    }
                    else if (display_name == "FS-MCS500POE(22GD400)")
                    {
                        //distRect = new Rect(145, 40, 280, 255);

                    }
                    else
                    {
                        //distRect = new Rect(0, 0, 10, 10);


                    }

                    if (i == 0)
                    {
                        if (image1.Rows != 0 && image1.Cols != 0)
                        {

                            Cv2.Undistort(image1, undistimg1, cameraMatrix_1, distCoeffs_1, newcameramatrix_1);

                            //pictureBox2.Image = BitmapConverter.ToBitmap(undistimg1);
                            //Cv2.Rectangle(undistimg1, rect, Scalar.Red, 10, LineTypes.AntiAlias);
                            //undistimg1 = undistimg1.SubMat(distRect);

                            Cv2.Resize(undistimg1, undistimg1, new OpenCvSharp.Size(mWidth, mHeight));

                            //gray scale 변화 테스트용
                            //gray_img1 = changetogray(undistimg1);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(gray_img1);
                            ////적응형 이진화 테스트용
                            //binary_img1 = Adaptive_biniarzation(gray_img1);
                            ////pictureBox2.Image = BitmapConverter.ToBitmap(binary_img1);
                            ////Morphology 테스트용
                            //Morphology_img1 = Morphology(binary_img1);
                            //pictureBox2.Image = BitmapConverter.ToBitmap(Morphology_img1);
                        }

                    }
                    else if (i == 1)
                    {
                        if (image2.Rows != 0 && image2.Cols != 0)
                        {

                            Cv2.Undistort(image2, undistimg2, cameraMatrix_2, distCoeffs_2, newcameramatrix_2);


                            //Cv2.Rectangle(undistimg2, rect, Scalar.Red, 10, LineTypes.AntiAlias);
                            //undistimg2 = undistimg2.SubMat(distRect);

                            Cv2.Resize(undistimg2, undistimg2, new OpenCvSharp.Size(mWidth, mHeight));


                        }

                    }
                    else
                    {
                        if (image3.Rows != 0 && image3.Cols != 0)
                        {

                            Cv2.Undistort(image3, undistimg3, cameraMatrix_3, distCoeffs_3, newcameramatrix_3);


                            //Cv2.Rectangle(undistimg3, rect, Scalar.Red, 10, LineTypes.AntiAlias);
                            undistimg3 = undistimg3.SubMat(distRect);

                            Cv2.Resize(undistimg3, undistimg3, new OpenCvSharp.Size(mWidth, mHeight));


                        }
                    }
                }

                if (deviceList.GetSize() == 2)
                {
                    merge_image_2(undistimg1, undistimg2);
                    pictureBox1.Image = BitmapConverter.ToBitmap(merge_img);
                }

                else if (deviceList.GetSize() == 3)
                {
                    merge_image(undistimg1, undistimg2, undistimg3);
                    pictureBox1.Image = BitmapConverter.ToBitmap(merge_img);
                }
                else
                {
                    pictureBox1.Image = BitmapConverter.ToBitmap(undistimg1);

                }
            }
        }




        #endregion





        #region timer:카메라 영상 이미지 얻기(5초마다)
        private void takepicture_timesecond_Tick(object sender, EventArgs e)
        {
            takepicture_timesecond.Stop();
            Mat writeimg = new Mat();
            if (image1.Rows != 0 && image1.Cols != 0)
            {
                writeimg = image1.Clone();
                string dt = DateTime.Now.ToString("MMddHHmmss");
                //string path = System.Windows.Forms.Application.StartupPath;
                //MessageBox.Show(path);
                //string device_name = streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName;
                string folderPath = $"Capture\\{device_name}";
                DirectoryInfo di = new DirectoryInfo(folderPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                Cv2.ImWrite(folderPath + $"\\origin_{dt}.jpg", writeimg);
                WriteTextBox(textBox1, "현재 이미지 저장됨\r\n");
            }
            takepicture_timesecond.Start();
        }
        #endregion
    }
}
