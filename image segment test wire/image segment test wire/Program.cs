// See https://aka.ms/new-console-template for more information

using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


Mat gray_img1 = new Mat();
Mat binary_img1 = new Mat();
//Rect roi = new Rect(10, 0, 460, 300);
//matImage = matImage.SubMat(roi);
Mat image = Cv2.ImRead("C:\\Users\\김남기\\source\\repos\\image segment test wire\\image segment test wire\\real_wire_test4.jpg");

//roi
//Rect roi = new Rect(300,0,1400,image.Height);
Rect roi = new Rect(300, 0, image.Width-300, image.Height);
//roi 추출 시 테스트용
//Mat show_img = new Mat(); // 임시 테스트용
//show_img = image.Clone();
//Cv2.Rectangle(show_img, roi, Scalar.Red, 10, LineTypes.AntiAlias);
//Cv2.Resize(show_img, show_img, new Size(500, 300));
//Cv2.ImShow("roi", show_img);

image = image.SubMat(roi);


#region 객체의 외곽선 추출
// 그레이스케일로 변환
Mat gray = new Mat();
Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

//이미지 샤프닝
Mat Sharpend_img = SharpenImage(gray);


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
foreach(Point[] contour in contours)
{
    foreach(Point point in contour)
    {
        if (point.X < minval)
        {
            minval = point.X;

        }
        if (point.X > maxval) {
            maxval = point.X;
        }
    }
}

Point[] outerPoints = new Point[4]
{
    new Point(minval, 0),
    new Point(maxval, 0),
    new Point(maxval, image.Height),
    new Point(minval, image.Height)
};

// 바깥쪽 점들 시각화
Mat outsidepointimage = image.Clone();
foreach (Point point in outerPoints)
{
    Cv2.Circle(outsidepointimage, point, 5, Scalar.Red, -1);
    Cv2.PutText(outsidepointimage, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
}

//사각형 그리기
Mat rectangle_image = outsidepointimage.Clone();
Cv2.Polylines(rectangle_image, new Point[][] { outerPoints }, true, Scalar.Red, 2);

//마스크 생성
Mat mask = new Mat(image.Size(), MatType.CV_8UC1, Scalar.Black);
Cv2.FillConvexPoly(mask, outerPoints, Scalar.White);

// 마스크를 적용하여 객체만 분리된 이미지 생성
Mat segmentedObject = new Mat();
image.CopyTo(segmentedObject, mask);

// 결과 출력

Mat[] images = { outsidepointimage, rectangle_image, edges, contouriamge,segmentedObject };
for (int i = 0; i < images.Length; i++)
{
    Cv2.Resize(images[i], images[i], new Size(1000, 600));
}
string[] titles = { "outerpoint", "rectangle", "canny", "Contours","segment" };

for (int i = 0; i < images.Length; i++)
{
    Cv2.NamedWindow(titles[i], WindowFlags.AutoSize);
    Cv2.ImShow(titles[i], images[i]);
}

//Cv2.ImShow("outerpoint", outsidepointimage);
//Cv2.ImShow("rectangle", rectangle_image);
//Cv2.ImShow("canny", edges);
//Cv2.ImShow("Contours",contouriamge);


Cv2.WaitKey(0);
Cv2.DestroyAllWindows();

#endregion



#region watershed 이용
////이미지 그레이 스케일 변화
//Cv2.CvtColor(image, gray_img1, ColorConversionCodes.BGR2GRAY);

//////이미지 선명화(sharpening)
//Mat sharpened_img = new Mat();
//sharpened_img = SharpenImage(gray_img1);

////이미지 적응형 이진화
//Cv2.AdaptiveThreshold(sharpened_img, binary_img1, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize:99, c:30 );
////Cv2.AdaptiveThreshold(gray_img1, binary_img1, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: 99, c: 50);


////모폴로지(noise reduce)
//Mat opening = new Mat();
//int kernel_size = 3;
//Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernel_size, kernel_size));

//////모폴로지(opening)
////Cv2.MorphologyEx(binary_img1, opening, MorphTypes.Open, kernel, iterations: 1);

////모폴로지 축소(erode)(노이즈 제거)
////Cv2.Erode(opening, opening, kernel, iterations: 1);
//Cv2.Erode(binary_img1, opening, kernel, iterations: 5);

////배경 추출을 위한 모폴로지(확장)
//Mat sure_bg = new Mat();
//Cv2.Dilate(opening, sure_bg, kernel, iterations: 10);

////거리변환
//Mat distance = new Mat();
//Cv2.DistanceTransform(opening, distance, DistanceTypes.L2, DistanceTransformMasks.Mask5);
//Cv2.Normalize(distance, distance, 0, 255, NormTypes.MinMax);

////거리변환 이미지화
//Mat distance_img = new Mat();
//distance.ConvertTo(distance_img, MatType.CV_8U);

////전경 추출을 위한 이진화
//Mat sure_fg = new Mat();
//double maxVal;
//OpenCvSharp.Cv2.MinMaxLoc(distance, out _, out maxVal);

//double thresholdValue = 0.001 * maxVal;

//Cv2.Threshold(distance, sure_fg, thresholdValue, 255, ThresholdTypes.Binary);

////전경추출 이미지화
//Mat surefg_img = new Mat();

//sure_fg.ConvertTo(surefg_img, MatType.CV_8U);



////unknown 부분 추출
//Mat unknown = new Mat();
//Cv2.Subtract(sure_bg, surefg_img, unknown);



////sure_fg에 labelling
//Mat marker_img = new Mat();
//Mat marker = new Mat();
//Mat stats = new Mat();
//Mat centroids = new Mat();

//Cv2.ConnectedComponentsWithStats(surefg_img, marker, stats, centroids, PixelConnectivity.Connectivity8);
//marker = marker + 1;

//for (int j = 0; j < marker.Rows; j++)
//{
//    for (int k = 0; k < marker.Cols; k++)
//    {
//        if (unknown.Get<int>(j, k) == 255)
//        {
//            marker.Set<int>(j, k, 0);
//        }
//    }
//}

////marker 이미지 화
//marker.ConvertTo(marker_img, MatType.CV_8U);


////워터 쉐드 알고리즘
//Mat water_img = image.Clone();

//Cv2.Watershed(water_img, marker);

//for (int j = 0; j < marker.Rows; j++)
//{
//    for (int k = 0; k < marker.Cols; k++)
//    {
//        if (marker.Get<int>(j, k) == -1)
//        {

//            water_img.Set<Vec3b>(j, k, new Vec3b(0, 0, 255));
//        }
//    }
//}


//Mat[] images = { gray_img1, binary_img1, opening, sure_bg, distance_img, surefg_img, unknown, marker_img, water_img };
//for (int i = 0; i < images.Length; i++)
//{
//    Cv2.Resize(images[i], images[i], new Size(500, 300));
//}
//string[] titles = { "Gray", "Binary", "Opening", "Sure BG", "Distance", "Sure FG", "Unknown", "Markers", "Result" };

//for (int i = 0; i < images.Length; i++)
//{
//    Cv2.NamedWindow(titles[i], WindowFlags.AutoSize);
//    Cv2.ImShow(titles[i], images[i]);
//}

//Cv2.WaitKey(0);
//Cv2.DestroyAllWindows();

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

#endregion