using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

Mat image = Cv2.ImRead("C:\\Users\\김남기\\source\\repos\\pixel_line_detecting\\pixel_line_detecting\\image1.jpg");



Mat binary_img = new Mat();

// 이미지 그레이스케일로 변환
Mat grayImage = new Mat();
Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

//이미지 선명화(sharpening 사용)
Mat sharpening_img = SharpenImage(grayImage);

// 이미지 이진화
Cv2.AdaptiveThreshold(sharpening_img, binary_img, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 7,3);

////케니 엣지 필터 사용
//Mat edge = new Mat();
//Cv2.Canny(grayImage, edge, 50,100 ,3,true);

#region 케니 엣지 후 허프 변환해서 직선인 선 검출해서 크기 측정

//int cellCountHorizontal = 3;
//int cellCountVertical = 3;

////케니 엣지 필터 사용
//Mat edge = new Mat();
//Cv2.Canny(sharpening_img, edge, 100, 250, 3, true);

//// 허프 변환을 사용하여 직선 검출
//Mat hough_image = image.Clone();
//LineSegmentPoint[] lines = Cv2.HoughLinesP(edge, 1, Cv2.PI / 180, 100, 10, 10);

//// 추출된 직선을 이미지에 그리기
//for (int i = 0; i < lines.Length; i++)
//{
//    Cv2.Line(hough_image, lines[i].P1, lines[i].P2, Scalar.Yellow, 2);
//}

//// 수평 선과 수직 선 필터링
//LineSegmentPoint[] horizontalLines = Array.FindAll(lines, line =>
//{
//    double angle = Math.Atan2(line.P2.Y - line.P1.Y, line.P2.X - line.P1.X) * 180 / Math.PI;
//    return (angle > -10 && angle < 10);
//});

//LineSegmentPoint[] verticalLines = Array.FindAll(lines, line =>
//{
//    double angle = Math.Atan2(line.P2.Y - line.P1.Y, line.P2.X - line.P1.X) * 180 / Math.PI;
//    return (angle > 80 && angle < 100);
//});

//// 수평 선과 수직 선의 평균 길이 계산
//double horizontalLength = horizontalLines.Average(line => line.Length());
//double verticalLength = verticalLines.Average(line => line.Length());

//// 체크보드 셀 가로와 세로 길이 계산
//double cellWidth = horizontalLength / (cellCountHorizontal - 1);
//double cellHeight = verticalLength / (cellCountVertical - 1);


//Console.WriteLine($"cellwidth:{cellWidth}");
//Console.WriteLine($"cellheight:{cellHeight}");
////결과 이미지 표시


//Cv2.NamedWindow("original_image");
//Cv2.ImShow("original_image", image);
//Cv2.NamedWindow("canny edge");
//Cv2.ImShow("canny edge", edge);
//Cv2.NamedWindow("hough line");
//Cv2.ImShow("hough line", hough_image);


//Cv2.WaitKey(0);
//Cv2.DestroyAllWindows();

#endregion

#region 허프변환 이용하여 직선 검출 후 교차점 이용하여 코너점 추출
//// 허프 변환을 사용하여 직선 검출
//Mat hough_image = image.Clone();
//LineSegmentPoint[] lines = Cv2.HoughLinesP(edge, 1, Cv2.PI / 180, 100, 50, 10);

//// 추출된 직선을 이미지에 그리기
//for (int i=0;i<lines.Length; i++)
//{
//    Cv2.Line(hough_image, lines[i].P1, lines[i].P2, Scalar.Yellow, 2);
//}

////교차점 추출
//Mat hough_corner_img = image.Clone();
//Point2f[] corners = ExtractIntersections(lines, image.Size());

////추출된 코너점들 이미지에 그리기
//foreach(Point2f corner in corners)
//{
//    Cv2.Circle(hough_corner_img, (int)corner.X,(int)corner.Y, 5, Scalar.Red, 2);
//}

#endregion

#region 케니 엣지 코너점 추출

////케니 엣지 필터 사용
//Mat edge = new Mat();
//Cv2.Canny(grayImage, edge, 50, 100, 3, true);

////코너점 추출
//Mat edge_corner = image.Clone();

//Point2f[] corners = Cv2.GoodFeaturesToTrack(
//    edge, maxCorners: 100,    // 검출할 최대 코너점 개수
//    qualityLevel: 0.01, // 코너점으로 간주할 품질 수준
//    minDistance: 1,    // 코너점 사이의 최소 거리
//    mask: null,         // 코너점 검출 영역 마스크 (전체 이미지에 대해 검출)
//    blockSize: 3,       // 코너점 검출을 위한 블록 크기
//    useHarrisDetector: true, // 코너점 검출 방법 (Harris 검출 사용 안 함)
//    k: 0.01            // 코너점 검출을 위한 상수 (Harris 검출 사용 안 함)
//     );

//// 코너점 출력
//int count = 0;
//foreach (Point corner in corners)
//{
//    Cv2.Circle(edge_corner, (int)corner.X, (int)corner.Y, 5, Scalar.Red, 4);
//    Cv2.PutText(edge_corner, $"corner {count}", corner, HersheyFonts.HersheySimplex, 1, Scalar.Red, 1);
//    count++;
//}

//Mat line_image = image.Clone();

//count = 0;
//foreach (Point corner in corners)
//{
//    if (count == 18 || count == 14 || count == 8 || count == 5 || count == 14 || count == 3)
//    {
//        Cv2.Circle(line_image, (int)corner.X, (int)corner.Y, 5, Scalar.Red, 4);
//        Cv2.PutText(line_image, $"corner {count}", corner, HersheyFonts.HersheySimplex, 1, Scalar.Red, 1);
//    }
//    count++;
//}

//Cv2.Line(line_image, (int)corners[18].X, (int)corners[18].Y, (int)corners[14].X, (int)corners[14].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[14].X, (int)corners[14].Y, (int)corners[8].X, (int)corners[8].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[5].X, (int)corners[5].Y, (int)corners[14].X, (int)corners[14].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[14].X, (int)corners[14].Y, (int)corners[3].X, (int)corners[3].Y, Scalar.Red, 4);

//int first_width_distance = CalculatePixelDistance((int)corners[18].X, (int)corners[18].Y, (int)corners[14].X, (int)corners[14].Y);
//int second_width_distance = CalculatePixelDistance((int)corners[14].X, (int)corners[14].Y, (int)corners[8].X, (int)corners[8].Y);
//int first_heigth_distance = CalculatePixelDistance((int)corners[5].X, (int)corners[5].Y, (int)corners[14].X, (int)corners[14].Y);
//int second_heigth_distance = CalculatePixelDistance((int)corners[14].X, (int)corners[14].Y, (int)corners[3].X, (int)corners[3].Y);

//Console.WriteLine($"first width distance:{first_width_distance}");
//Console.WriteLine($"second width distance:{second_width_distance}");
//Console.WriteLine($"first height distance:{first_heigth_distance}");
//Console.WriteLine($"second height distance:{second_heigth_distance}");



////이미지 조정
////Cv2.Resize(edge_corner, edge_corner, new Size(1200, 700));

//Cv2.ImWrite("canny edge2.jpg", edge);
//Cv2.ImWrite("corner_point3.jpg", edge_corner);
//Cv2.ImWrite("corner_line3.jpg", line_image);

////결과 출력
//Mat[] images = { grayImage, binary_img, edge, edge_corner, line_image };
//string[] titles = { "Gray", "binary", "edge", "edge corner", "line image" };

//for (int i = 0; i < images.Length; i++)
//{
//    Cv2.NamedWindow(titles[i], WindowFlags.AutoSize);
//    Cv2.ImShow(titles[i], images[i]);
//}

//Cv2.WaitKey(0);
//Cv2.DestroyAllWindows();
#endregion

#region 이미지의 코너점을 이용한 픽셀 거리 구하기
//Mat sharpening_img = SharpenImage(image);

//// 이미지 그레이스케일로 변환
//Mat grayImage = new Mat();
//Cv2.CvtColor(sharpening_img, grayImage, ColorConversionCodes.BGR2GRAY);

//// 코너점 검출
//Point2f[] corners = Cv2.GoodFeaturesToTrack(
//    grayImage,
//    maxCorners: 100,    // 검출할 최대 코너점 개수
//    qualityLevel: 0.05, // 코너점으로 간주할 품질 수준
//    minDistance: 200,    // 코너점 사이의 최소 거리
//    mask: null,         // 코너점 검출 영역 마스크 (전체 이미지에 대해 검출)
//    blockSize: 3,       // 코너점 검출을 위한 블록 크기
//    useHarrisDetector: false, // 코너점 검출 방법 (Harris 검출 사용 안 함)
//    k: 0.04            // 코너점 검출을 위한 상수 (Harris 검출 사용 안 함)
//);

//// 코너점 그리기
//int count = 0;
//foreach (Point corner in corners)
//{

//    Cv2.Circle(image, corner, 5, Scalar.Red, 2);
//    //Cv2.PutText(image, $"corner {count}:({corner.X},{corner.Y})",corner,HersheyFonts.HersheySimplex,1,Scalar.Red,2);
//    count++;
//}

//Mat line_image = image.Clone();

//Cv2.Line(line_image, (int)corners[0].X, (int)corners[0].Y, (int)corners[7].X, (int)corners[7].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[7].X, (int)corners[7].Y, (int)corners[3].X, (int)corners[3].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[11].X, (int)corners[11].Y, (int)corners[7].X, (int)corners[7].Y, Scalar.Red, 4);
//Cv2.Line(line_image, (int)corners[7].X, (int)corners[7].Y, (int)corners[9].X, (int)corners[9].Y, Scalar.Red, 4);

//int first_width_distance = CalculatePixelDistance((int)corners[0].X, (int)corners[0].Y, (int)corners[7].X, (int)corners[7].Y);
//int second_width_distance = CalculatePixelDistance((int)corners[7].X, (int)corners[7].Y, (int)corners[3].X, (int)corners[3].Y);
//int first_heigth_distance = CalculatePixelDistance((int)corners[11].X, (int)corners[11].Y, (int)corners[7].X, (int)corners[7].Y);
//int second_heigth_distance = CalculatePixelDistance((int)corners[7].X, (int)corners[7].Y, (int)corners[9].X, (int)corners[9].Y);

//Console.WriteLine($"first width distance:{first_width_distance}");
//Console.WriteLine($"second width distance:{second_width_distance}");
//Console.WriteLine($"first height distance:{first_heigth_distance}");
//Console.WriteLine($"second height distance:{second_heigth_distance}");



//이미지 조정
//Cv2.Resize(image, image, new Size(1200, 700));

//Cv2.ImWrite("corner_point.jpg", image);
//Cv2.ImWrite("corner_line.jpg", line_image);
// 결과 이미지 출력
//Cv2.ImShow("Corner Detection", image);
////Cv2.ImShow("corner line", line_image);
//Cv2.WaitKey(0);
//Cv2.DestroyAllWindows();
#endregion



static Point2f[] ExtractIntersections(LineSegmentPoint[] lines, Size imageSize)
{
    // 교차점을 저장할 리스트 초기화
    List<Point2f> intersections = new List<Point2f>();

    // 모든 직선 쌍에 대해 교차점 찾기
    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = i + 1; j < lines.Length; j++)
        {
            Point2f intersection = FindIntersection(lines[i], lines[j], imageSize);
            intersections.Add(intersection);
        }
    }

    // 중복 제거 후 배열로 변환하여 반환
    return intersections.Distinct().ToArray();
}

static Point2f FindIntersection(LineSegmentPoint line1, LineSegmentPoint line2, Size imageSize)
{
    double x1 = line1.P1.X;
    double y1 = line1.P1.Y;
    double x2 = line1.P2.X;
    double y2 = line1.P2.Y;
    double x3 = line2.P1.X;
    double y3 = line2.P1.Y;
    double x4 = line2.P2.X;
    double y4 = line2.P2.Y;

    double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

    if (Math.Abs(d) < 1e-5)
    {
        // 두 선이 평행한 경우
        return new Point2f(float.MaxValue, float.MaxValue);
    }
    else
    {
        double x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d;
        double y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d;

        // 교차점이 이미지 영역 내에 있는지 확인
        if (x >= 0 && x < imageSize.Width && y >= 0 && y < imageSize.Height)
        {
            return new Point2f((float)x, (float)y);
        }
        else
        {
            // 이미지 영역 밖에 있는 경우
            return new Point2f(float.MaxValue, float.MaxValue);
        }
    }
}

static int CalculatePixelDistance(int x1, int y1, int x2, int y2)
{
    int deltaX = x2 - x1;
    int deltaY = y2 - y1;
    int deltaXSquare = deltaX * deltaX;
    int deltaYSquare = deltaY * deltaY;
    int distanceSquare = deltaXSquare + deltaYSquare;
    int distance = (int)Math.Sqrt(distanceSquare);

    return distance;
}

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