
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;

Mat image = Cv2.ImRead("wire6.jpg");
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

// 객체 탐지를 위한 적응형 이진화
Mat binary_gray = new Mat();
Cv2.CvtColor(segmentedObject, binary_gray, ColorConversionCodes.BGR2GRAY);
Mat binary = new Mat();
Cv2.AdaptiveThreshold(binary_gray, binary, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, blockSize: 27, c: 0);

////통계적 방법을 이용한 손상 탐지 알고리즘
//Mat detect_image = segmentedObject.Clone();

////이미지 그레이 스케일 변환
//Mat detect_image_gray = new Mat();

//Point[] damagedIndices = DetectDamage(binary);

//// 손상 부분의 좌표 출력
//Console.WriteLine("Damaged Indices:");
//foreach (var point in damagedIndices)
//{
//    Cv2.Circle(detect_image, point, 5, Scalar.Red, -1);
//    Cv2.PutText(detect_image, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
//    Console.WriteLine($"X: {point.X}, Y: {point.Y}");
//}

//픽셀 확인을 위한 코드

Mat point_image = segmentedObject.Clone();
Cv2.Resize(point_image, point_image, new OpenCvSharp.Size(1000, 600));
OpenCvSharp.MouseCallback _clickonvision = new OpenCvSharp.MouseCallback(clickOnVision);

OpenCvSharp.Window win = new OpenCvSharp.Window("vision",point_image);
Cv2.SetMouseCallback(win.Name, _clickonvision);

// 결과 출력

Mat[] images = { outsidepointimage, rectangle_image, edges, contouriamge, segmentedObject,binary };
for (int i = 0; i < images.Length; i++)
{
    Cv2.Resize(images[i], images[i], new OpenCvSharp.Size(1000, 600));
}
string[] titles = { "outerpoint", "rectangle", "canny", "Contours", "segment" ,"binary"};

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

static OpenCvSharp.Point[] DetectDamage(Mat image)
{


    // 이미지를 픽셀 단위로 분석
    byte[] pixels = new byte[image.Total()];
    Marshal.Copy(image.Data, pixels, 0, pixels.Length);

    // 픽셀 값의 평균과 표준편차 계산
    Cv2.MeanStdDev(image, out Scalar mean, out Scalar stdDev);

    // 픽셀 값의 분포를 분석하여 이상점 탐지
    //double threshold = mean.Val0 - (stdDev.Val0 * 3);  // 임계치 설정
    double threshold = mean.Val0 - (stdDev.Val0 * 1);  // 임계치 설정
    OpenCvSharp.Point[] damagedIndices = Enumerable.Range(0, pixels.Length)
        .Where(index => pixels[index] < threshold)
        .Select(index => new OpenCvSharp.Point(index % image.Width, index / image.Width))
        .ToArray();

    // 손상 부분의 좌표 반환
    return damagedIndices;
}

//마우스 클릭
void clickOnVision(OpenCvSharp.MouseEventTypes eve, int x, int y ,OpenCvSharp.MouseEventFlags flag, System.IntPtr _ptr)
{
    if(eve == MouseEventTypes.LButtonDown)
    {
        

        string text = "X: " + x.ToString() + "Y: " + y.ToString();
        Debug.WriteLine(text);

        OpenCvSharp.Point point = new OpenCvSharp.Point(x,y);
        Cv2.Circle(point_image, point, 5, Scalar.Red, -1);
        Cv2.PutText(point_image, $"({point.X}, {point.Y})", point, HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);

        Cv2.ImShow("point image",point_image);
    }
}