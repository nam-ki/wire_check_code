using OpenCvSharp;
using OpenCvSharp.Features2D;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;




#region 특징점 검출
//Mat image1 = Cv2.ImRead("real_wire_test2.jpg", ImreadModes.Grayscale);
////SIFT 객체 생성
//SIFT sift = SIFT.Create();

////특징점 검출
//KeyPoint[] keypoints = sift.Detect(image1);

////특징점 출력
//Mat output = new Mat();
//Cv2.DrawKeypoints(image1, keypoints, output);

////결과 이미지 저장
//Cv2.Resize(image1, image1, new Size(1000, 600));
//Cv2.Resize(output, output,new Size(1000,600));
//Cv2.ImShow("image", image1);
//Cv2.ImShow("result",output);

//Cv2.WaitKey(0);
//Cv2.DestroyAllWindows();

#endregion

#region 윤곽선 추출

//Mat img1 = Cv2.ImRead("real_wire_test2.jpg");
//Mat img2 = Cv2.ImRead("real_wire_test6.jpg");

//Mat img_seg = new Mat();
//Mat img_seg2 = new Mat();
//Point[] perspective_point_Base = new Point[4];
//Point[] perspective_point = new Point[4];

////Mat img_seg = contour_segment(img1);
////Mat img_seg2 = contour_segment(img2);


//(img_seg,perspective_point_Base) = contour_segment(img1);
//(img_seg2,perspective_point) = contour_segment(img2);

//Mat test_wrap_img = wraping_img(perspective_point_Base, perspective_point, img_seg2);

//Mat merge = image_merge(img_seg, img_seg2);
//Mat test_merge = image_merge(img_seg, test_wrap_img);

//Mat[] images = { img_seg,img_seg2, merge,test_merge };
//for (int i = 0; i < images.Length; i++)
//{
//    Cv2.Resize(images[i], images[i], new Size(1000, 600));
//}
//string[] titles = { "segment1","segment2" ,"merge_img","test_merge_img"};

//for (int i = 0; i < images.Length; i++)
//{
//    Cv2.NamedWindow(titles[i], WindowFlags.AutoSize);
//    Cv2.ImShow(titles[i], images[i]);
//}

//Cv2.WaitKey();
//Cv2.DestroyAllWindows();

#endregion

#region 윤곽선 추출 후 투영변환 여러개 이미지 적용

//폴더 안의 이미지 파일 이름 얻어오기
string directoryPath = "C:\\Users\\김남기\\source\\repos\\merge wire test\\merge wire test\\bin\\Debug\\net6.0\\merge_image_file";

List<string> imageFileList = new List<string>();

foreach (string fileName in Directory.GetFiles(directoryPath))
{
    if (Regex.IsMatch(fileName, @".jpg$"))
    {
        imageFileList.Add(fileName);
    }
}

//가져온 폴더 안의 이미지 파일 불러오기
List<Mat> folder_image = new List<Mat>();

foreach (string filename in imageFileList)
{
    Mat image = Cv2.ImRead(filename);
    folder_image.Add(image);
}

//폴더 안의 이미지 파일 이미지 분할
List<Mat> segment_image_list = new List<Mat>();
List<Point[]> points = new List<Point[]>();

foreach (Mat image in folder_image)
{
    (Mat seg_img, Point[] perspective_point) = contour_segment(image);
    segment_image_list.Add(seg_img);
    points.Add(perspective_point);
}

Point[] perspective_point_base = points[0];

//1번 이미지를 기준으로 나머지 이미지 투영 변환하여 크기 맞춤.

List<Mat> wrap_img_list = new List<Mat>();

for (int i = 1; i < segment_image_list.Count; i++)
{
    Mat wrap_img = wraping_img(perspective_point_base, points[i], segment_image_list[i]);
    wrap_img_list.Add(wrap_img);
}

//크기 맞춘 이미지 세로 병합

Mat mergeImage = segment_image_list[0];

for (int i = 0; i < wrap_img_list.Count; i++)
{
    Mat currentImage = wrap_img_list[i];
    Cv2.VConcat(new Mat[]{ mergeImage, currentImage },mergeImage);
}


//병합 이미지 외에 검은 부분 제거

int merge_img_width = perspective_point_base[1].X - perspective_point_base[0].X; // 병합 부분 width
int merge_img_height = mergeImage.Height;

int merge_img_left_up = perspective_point_base[0].X;


Rect merge_img_rectangle = new Rect(merge_img_left_up,0,merge_img_width,merge_img_height);

//roi 추출 시 테스트용
//Mat show_img = new Mat(); // 임시 테스트용
//show_img = mergeImage.Clone();
//Cv2.Rectangle(show_img, merge_img_rectangle, Scalar.Red, 10, LineTypes.AntiAlias);
//Cv2.Resize(show_img, show_img, new Size(500, 300));
//Cv2.ImShow("roi", show_img);

mergeImage = mergeImage.SubMat(merge_img_rectangle);


//Mat test_img = mergeImage.Clone();
//test_img = test_img.SubMat(merge_img_rectangle);
//Cv2.ImWrite("merge_image.jpg", test_img);
//Cv2.Resize(test_img, test_img, new Size(500, 300));
//Cv2.ImShow("test", test_img);
Cv2.Resize(mergeImage, mergeImage, new Size(500, 300));
Cv2.ImShow("merge_img", mergeImage);
Cv2.WaitKey();
Cv2.DestroyAllWindows();



#endregion

//윤곽선 추출해서 segment후 segment 이미지와 특징점 반환
static (Mat, Point[]) contour_segment(Mat image)
{
    Rect roi = new Rect(300, 0, image.Width - 300, image.Height);
    image = image.SubMat(roi);

    // 그레이스케일로 변환
    Mat gray = new Mat();
    Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

    // 가장자리 감지
    Mat edges = new Mat();
    Cv2.Canny(gray, edges, 150, 200);

    // 외곽선 찾기
    var contours = Cv2.FindContoursAsArray(edges, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

    // 외곽선 그리기
    Mat contouriamge = image.Clone();
    Cv2.DrawContours(contouriamge, contours, -1, Scalar.Red, 2);

    //최소값 최대값 설정
    int minval = contours[0][0].X;
    int maxval = contours[0][0].X;


    //최소점, 최대점 추출
    foreach (Point[] contour in contours)
    {
        foreach (Point point in contour)
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

    return (segmentedObject, outerPoints);
}

//이미지 이어붙이기
static Mat image_merge(Mat image1, Mat image2)
{
    Mat merge_img = new Mat();
    try
    {
        if ((image1.Height == image2.Height))
        {


            Cv2.VConcat(new Mat[] { image1, image2 }, merge_img);



        }
    }
    catch (Exception ex) { throw; }

    return merge_img;
}


//투영 변환을 이용한 특징점 맞춤
static Mat wraping_img(Point[] outerPoint_Base, Point[] outerPoint, Mat img)
{
    Mat wrap = new Mat();

    Point2f[] srcpoint1 = new Point2f[4] {
               outerPoint[0],             // 좌상단 원점
               outerPoint[1],               // 우상단
               outerPoint[2],      // 우하단
               outerPoint[3]               // 좌하단
            };

    Point2f[] srcpoint2 = new Point2f[4] {
               outerPoint_Base[0],             // 좌상단 원점
               outerPoint_Base[1],               // 우상단
               outerPoint_Base[2],      // 우하단
               outerPoint_Base[3]               // 좌하단
            };// 기준점

    Mat perspectivematrix = Cv2.GetPerspectiveTransform(srcpoint1, srcpoint2);

    Cv2.WarpPerspective(img, wrap, perspectivematrix, img.Size());

    return wrap;
}