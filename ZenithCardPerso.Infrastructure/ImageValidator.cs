using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardPerso.Infrastructure
{
    public class ImageValidator
    {
        private static CascadeClassifier _cascadeClassifier;
        private static CascadeClassifier EyeClassifier;
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (System.IO.Stream bmpStream = File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        public static FaceObj FacialScan(string imgSaveLoc)
        {
            try
            {
                //Image<Gray, byte> inputImage = new Image<Gray, byte>(imgBitMap);
                //Mat image = new Mat(imgSaveLoc, LoadImageType.Color); //Read the files as an 8-bit Bgr image  reo
                using (Image<Bgr, byte> image = new Image<Bgr, byte>(imgSaveLoc))
                {

                    long detectionTime;
                    List<Rectangle> faces = new List<Rectangle>();
                    List<Rectangle> eyes = new List<Rectangle>();
                    List<Rectangle> eyesProperties = new List<Rectangle>();

                    //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
                    //disabling CUDA module for now
                    bool tryUseCuda = false;

                    DetectFace.Detect(
                      image.Mat, baseDirectory + "haarcascade_frontalface_alt_tree.xml", baseDirectory + "haarcascade_eye.xml",
                      faces, eyes, eyesProperties,
                      tryUseCuda,
                      out detectionTime);

                    int facesCount = faces.Count;
                    int eyesCount = eyes.Count;

                    FaceObj faceObj = new FaceObj() { NoofFaces = facesCount, NoofEyes = eyesCount };

                    return faceObj;
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public static List<Rectangle> TiltCheck(string imgSaveLoc)
        {

            using (Mat image = new Mat(imgSaveLoc, LoadImageType.Color))
            {

                long detectionTime;
                List<Rectangle> faces = new List<Rectangle>();
                List<Rectangle> eyes = new List<Rectangle>();
                List<Rectangle> eyesProperties = new List<Rectangle>();


                //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
                //disabling CUDA module for now
                bool tryUseCuda = false;

                DetectFace.Detect(
                  image, baseDirectory + "haarcascade_frontalface_alt_tree.xml", baseDirectory + "haarcascade_eye.xml",
                  faces, eyes, eyesProperties,
                  tryUseCuda,
                  out detectionTime);


                return eyesProperties;
            }

        }

        public static string EyesValidation(string imgURl)
        {
            try
            {
                FaceObj facialScan = FacialScan(imgURl);

                if (facialScan.NoofFaces == 1)
                {
                    if (facialScan.NoofEyes == 2)
                    {
                        return "00";
                    }
                    else
                    {
                        return "01";
                    }
                }
                else if (facialScan.NoofFaces == 2)
                {
                    return "03";
                }
                else if (facialScan.NoofFaces == 0)
                {
                    return "02";
                }
                else
                {
                    return "10";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string SingleFaceDetection(string imgURl)
        {
            try
            {
                FaceObj facialScan = FacialScan(imgURl);

                if (facialScan.NoofFaces > 1)
                {
                    return "03";
                }
                else
                {
                    if (facialScan.NoofFaces == 1)
                    {
                        return "00";
                    }
                    else
                    {
                        return "02";
                    }
                }
            }
            catch (Exception ex)
            {
                return "10";
            }
        }
        public static string MultipleFaceDetection(string imgURl, int noOfFaces)
        {
            FaceObj facialScan = FacialScan(imgURl);

            if (facialScan.NoofFaces > noOfFaces)
            {
                return "03";
            }
            else
            {
                if (facialScan.NoofFaces == noOfFaces)
                {
                    return "00";
                }
                else
                {
                    return "10";
                }
            }
        }
        public static string HeadTilt(string imgUrl, int degreeOfTilt)
        {
            var facialScan = FacialScan(imgUrl);
            var eyesResult = TiltCheck(imgUrl);

            if (facialScan.NoofEyes == 2)
            {
                var Tiltdiff = eyesResult[0].Y - eyesResult[1].Y;

                if (Math.Abs(Tiltdiff) > degreeOfTilt)
                {
                    return "09";
                }
                else
                {
                    return "00";
                }
            }
            else
            {
                return "01";
            }

        }
        public string ResolutionCheck()
        {
            return "";
        }

        public static string Dimension(int definedWidth, int definedHeight, string imageUrl)
        {
            using (Bitmap img = new Bitmap(imageUrl))
            {
                if (img.Width != definedWidth && img.Height != definedHeight)
                {
                    return "05";
                }
                else
                {
                    return "00";
                }
            }
        }

        public static string BlurDetection(string imageUrl)
        {
            using (Bitmap img = new Bitmap(imageUrl))
            {
                Image<Gray, byte> imgGray = new Image<Gray, byte>(img);
                Image<Gray, float> imgOut = new Image<Gray, float>(img.Width, img.Height, new Gray(0));

                imgOut = imgGray.Laplace(1);

                float[,] k = { {0, 1, 0},
                        {1, -4, 1},
                        {0, 1, 0}};

                ConvolutionKernelF kernel = new ConvolutionKernelF(k);
                Image<Gray, float> convoluted = imgOut * kernel;

                MCvScalar average = new MCvScalar();
                MCvScalar std = new MCvScalar();

                CvInvoke.MeanStdDev(convoluted.Mat, ref average, ref std);

                //Threshold can be re-defined
                if (std.V0 < 40)
                {
                    return "06";
                }
                else
                {
                    return "00";
                }
            }
        }

        public static string WhiteBackground(string imgURl)
        {
            using (Bitmap image = new Bitmap(imgURl))
            {
                var isWhite = IsWhiteBackgroundTest(image);
                if (!isWhite)
                {
                    return "00";
                }

                return "11";
            }
        }

        public static string InvalidImage(string imgURl)
        {

            FaceObj facialScan = FacialScan(imgURl);

            if (facialScan.NoofFaces == 0)
            {
                return "02";
            }
            else
            {
                return "00";
            }
        }

        public static string ImageSize(int definedSize, string imageUrl)
        {
            int fileSize = (int)new System.IO.FileInfo(imageUrl).Length;

            int sizeInKB = fileSize / 1024;

            if (sizeInKB > definedSize || definedSize == 0)
            {
                return "07";
            }
            else
            {
                return "00";
            }
        }

        public static string ImageFormatvalidator(string imgFormat, string imgUrl)
        {
            var format = GetImageFormat(imgUrl);
            if (imgFormat != format.ToString())
            {
                return "08";
            }
            else
            {
                return "00";
            }

        }

        //public static string DistanceFromPhotoTop(string imgUrl)
        //{
        //    Bitmap image = new Bitmap(imgUrl);
        //    var istopHeadCut = TopHeadCheck(image);

        //    if (istopHeadCut && isWhite)
        //    {
        //        errorList.Add("There is not enough distance between the image captured and the top of the photograph");
        //    }
        //}

        public static ImageFormat GetImageFormat(string imageUrl)
        {
            using (Image img = Image.FromFile(imageUrl))
            {

                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bytes = ms.ToArray();
                }
                //byte[] bytes

                // see http://www.mikekunz.com/image_file_header.html  
                var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
                var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
                var png = new byte[] { 137, 80, 78, 71 };    // PNG
                var tiff = new byte[] { 73, 73, 42 };         // TIFF
                var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
                var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
                var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

                if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                    return ImageFormat.bmp;

                if (gif.SequenceEqual(bytes.Take(gif.Length)))
                    return ImageFormat.gif;

                if (png.SequenceEqual(bytes.Take(png.Length)))
                    return ImageFormat.png;

                if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                    return ImageFormat.tiff;

                if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                    return ImageFormat.tiff;

                if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                    return ImageFormat.jpeg;

                if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                    return ImageFormat.jpeg;
                if (bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46)
                    return ImageFormat.pdf;


                return ImageFormat.unknown;
            }
        }

        public static bool IsWhiteBackgroundTest(Bitmap myBitmap)
        {
            myBitmap = Resize(myBitmap, 200, 200);
            // Draw myBitmap to the screen.
            int rightWhiteCountY = 0;
            int rightNotWhiteCount = 0;
            Color c = Color.White;
            // Set each pixel in myBitmap to black.
            //for (int x = 0; x < myBitmap.Width; x++)
            //{
            //    myBitmap.SetPixel(x, 0,Color.Red);
            //    myBitmap.SetPixel(x, myBitmap.Height - 1,Color.Red);
            //}
            var imageWidthMod = myBitmap.Width % 2;

            #region RightSideCheck
            var rightSideCheck = (imageWidthMod == 0) ? myBitmap.Width - 2 : myBitmap.Width;

            for (int Xcount = 0; Xcount < myBitmap.Width / 6; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height / 2; Ycount++)
                {
                    // myBitmap.SetPixel(rightSideCheck - Xcount, Ycount, Color.Red);

                    c = myBitmap.GetPixel(Xcount, Ycount);
                    if (c.R >= 170 && c.R <= 255)
                    {
                        rightWhiteCountY += 1;
                    }
                    else
                    {
                        rightNotWhiteCount += 1;
                    }

                    if (c.G > 170 && c.G <= 255)
                    {
                        rightWhiteCountY += 1;
                    }
                    else
                    {
                        rightNotWhiteCount += 1;
                    }

                    if (c.B > 170 && c.B <= 255)
                    {
                        rightWhiteCountY += 1;
                    }
                    else
                    {
                        rightNotWhiteCount += 1;
                    }
                }
            }

            //var vvv = rightWhiteCountY;
            //var bbb = rightNotWhiteCount; 
            #endregion


            int leftWhiteCountY = 0;
            int leftNotWhiteCount = 0;
            #region LeftCheck
            var leftSideCheck = (imageWidthMod == 0) ? myBitmap.Width - 2 : myBitmap.Width;

            for (int Xcount = 0; Xcount < myBitmap.Width / 6; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height / 2; Ycount++)
                {
                    //myBitmap.SetPixel(Xcount, Ycount, Color.Red);

                    c = myBitmap.GetPixel(Xcount, Ycount);
                    if (c.R >= 170 && c.R <= 255)
                    {
                        leftWhiteCountY += 1;
                    }
                    else
                    {
                        leftNotWhiteCount += 1;
                    }

                    if (c.G > 170 && c.G <= 255)
                    {
                        leftWhiteCountY += 1;
                    }
                    else
                    {
                        leftNotWhiteCount += 1;
                    }

                    if (c.B > 170 && c.B <= 255)
                    {
                        leftWhiteCountY += 1;
                    }
                    else
                    {
                        leftNotWhiteCount += 1;
                    }
                }
            }
            #endregion


            //Checking the percentage of not white pixels in white pixels of the selected rectangle
            var rightResult = ((double)rightNotWhiteCount / (double)rightWhiteCountY) * 100;
            var leftResult = ((double)leftNotWhiteCount / (double)leftWhiteCountY) * 100;
            if (rightResult > 30 && leftResult > 30)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Bitmap Resize(Bitmap _currentBitmap, int newWidth, int newHeight)
        {
            if (newWidth != 0 && newHeight != 0)
            {
                Bitmap temp = (Bitmap)_currentBitmap;
                Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);
                //using (Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat))
                //{



                    double nWidthFactor = (double)temp.Width / (double)newWidth;
                    double nHeightFactor = (double)temp.Height / (double)newHeight;

                    double fx, fy, nx, ny;
                    int cx, cy, fr_x, fr_y;
                    Color color1 = new Color();
                    Color color2 = new Color();
                    Color color3 = new Color();
                    Color color4 = new Color();
                    byte nRed, nGreen, nBlue;

                    byte bp1, bp2;

                    for (int x = 0; x < bmap.Width; ++x)
                    {
                        for (int y = 0; y < bmap.Height; ++y)
                        {

                            fr_x = (int)Math.Floor(x * nWidthFactor);
                            fr_y = (int)Math.Floor(y * nHeightFactor);
                            cx = fr_x + 1;
                            if (cx >= temp.Width) cx = fr_x;
                            cy = fr_y + 1;
                            if (cy >= temp.Height) cy = fr_y;
                            fx = x * nWidthFactor - fr_x;
                            fy = y * nHeightFactor - fr_y;
                            nx = 1.0 - fx;
                            ny = 1.0 - fy;

                            color1 = temp.GetPixel(fr_x, fr_y);
                            color2 = temp.GetPixel(cx, fr_y);
                            color3 = temp.GetPixel(fr_x, cy);
                            color4 = temp.GetPixel(cx, cy);

                            // Blue
                            bp1 = (byte)(nx * color1.B + fx * color2.B);

                            bp2 = (byte)(nx * color3.B + fx * color4.B);

                            nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                            // Green
                            bp1 = (byte)(nx * color1.G + fx * color2.G);

                            bp2 = (byte)(nx * color3.G + fx * color4.G);

                            nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                            // Red
                            bp1 = (byte)(nx * color1.R + fx * color2.R);

                            bp2 = (byte)(nx * color3.R + fx * color4.R);

                            nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                            bmap.SetPixel(x, y, System.Drawing.Color.FromArgb
                    (255, nRed, nGreen, nBlue));
                        }
                    }
                    _currentBitmap = (Bitmap)bmap.Clone();

                //}
            }

            return _currentBitmap;
        }

        public static bool TopHeadCheck(Bitmap myBitmap)
        {
            int TopWhiteCount = 0;
            int TOpNotWhiteCount = 0;

            var imageWidthMod = myBitmap.Width % 2;
            #region TopFaceCheck
            var TopFaceCheck = (imageWidthMod == 0) ? myBitmap.Width - 2 : myBitmap.Width;
            Color c = Color.White;

            for (int Xcount = 0; Xcount < myBitmap.Width / 4; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height / 40; Ycount++)
                {
                    //myBitmap.SetPixel(myBitmap.Width / 2 - Xcount, Ycount, Color.Red);

                    c = myBitmap.GetPixel(myBitmap.Width / 2 - Xcount, Ycount);
                    if (c.R >= 170 && c.R <= 255)
                    {
                        TopWhiteCount += 1;
                    }
                    else
                    {
                        TOpNotWhiteCount += 1;
                    }

                    if (c.G > 170 && c.G <= 255)
                    {
                        TopWhiteCount += 1;
                    }
                    else
                    {
                        TOpNotWhiteCount += 1;
                    }

                    if (c.B > 170 && c.B <= 255)
                    {
                        TopWhiteCount += 1;
                    }
                    else
                    {
                        TOpNotWhiteCount += 1;
                    }
                }
            }
            #endregion

            var TopResult = ((double)TOpNotWhiteCount / (double)TopWhiteCount) * 100;

            bool topcheck = (TopResult > 10) ? true : false;
            return topcheck;
        }

    }
}
