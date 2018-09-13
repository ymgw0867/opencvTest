using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OpenCvSharp;

namespace opencvTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const float B_WIDTH = 0.25f;
        const float B_HEIGHT = 0.25f;

        const float A_WIDTH = 0.05f;
        const float A_HEIGHT = 0.05f;

        float n_width = 0f;
        float n_height = 0f;

        Mat mMat = new Mat();
        
        private void button1_Click(object sender, EventArgs e)
        {
            n_width = B_WIDTH;
            n_height = B_HEIGHT;

            imgShow(@"C:\CONYX_OCR\TIF\201808\201808_87654323_山田　哲人.tif", n_width, n_height);

            trackBar1.Value = 0;
        }

        private void imgShow(string filePath, float w, float h)
        {
            mMat = new Mat(filePath, ImreadModes.GrayScale);
            Bitmap bt = MatToBitmap(mMat);

            // Bitmap を生成
            Bitmap canvas = new Bitmap((int)(bt.Width * w), (int)(bt.Height * h));

            Graphics g = Graphics.FromImage(canvas);

            g.DrawImage(bt, 0, 0, bt.Width * w, bt.Height * h);

            //メモリクリア
            bt.Dispose();
            g.Dispose();

            pictureBox1.Image = canvas;
        }
        private void imgShow(Mat mImg, float w, float h)
        {
            int cWidth = 0;
            int cHeight = 0;

            Bitmap bt = MatToBitmap(mImg);

            // Bitmapサイズ
            if (panel1.Width < (bt.Width * w) || panel1.Height < (bt.Height * h))
            {
                cWidth = (int)(bt.Width * w);
                cHeight = (int)(bt.Height * h);
            }
            else
            {
                cWidth = panel1.Width;
                cHeight = panel1.Height;
            }

            // Bitmap を生成
            Bitmap canvas = new Bitmap(cWidth, cHeight);

            // ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);

            // 画像をcanvasの座標(0, 0)の位置に指定のサイズで描画する
            g.DrawImage(bt, 0, 0, bt.Width * w, bt.Height * h);

            //メモリクリア
            bt.Dispose();
            g.Dispose();

            // PictureBox1に表示する
            pictureBox1.Image = canvas;
        }

        // GUI上に画像を表示するには、OpenCV上で扱うMat形式をBitmap形式に変換する必要がある
        public static Bitmap MatToBitmap(Mat image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            n_width += A_WIDTH;
            n_height += A_HEIGHT;

            //imgShow(@"C:\CONYX_OCR\TIF\201808\201808_87654323_山田　哲人.tif", n_width, n_height);
            imgShow(mMat, n_width, n_height);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (n_width > A_WIDTH)
            {
                n_width -= A_WIDTH;
            }

            if (n_height > A_HEIGHT)
            {
                n_height -= A_HEIGHT;
            }

            //imgShow(@"C:\CONYX_OCR\TIF\201808\201808_87654323_山田　哲人.tif", n_width, n_height);
            imgShow(mMat, n_width, n_height);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Mat rMat = imgRotate(new Mat(@"C:\CONYX_OCR\TIF\201808\201808_87654323_山田　哲人.tif", ImreadModes.GrayScale), true);

            //imgShow(rMat, n_width, n_height);

            imgRotateBmp(pictureBox1.Image);

            //mMat = imgRotate(mMat, true);
            //imgShow(mMat, n_width, n_height);
        }

        private Mat imgRotate(Mat img, bool isRight)
        {
            Mat dest = new Mat();

            Point2f center = new Point2f(img.Cols / 2, img.Rows / 2);
            //Point2f center = new Point2f(pictureBox1.Width / 2, pictureBox1.Height / 2);
            var rMat = Cv2.GetRotationMatrix2D(center, 90, 1);
            Cv2.WarpAffine(img, dest, rMat, img.Size());

            Cv2.ImShow("img", dest);
            Cv2.WaitKey();



            //Cv2.Flip(img, dest, FlipMode.XY);




            return dest;
        }

        private void imgRotateBmp(Image img)
        {
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox1.Image = img;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            showMultiTiff(@"C:\CONYX_OCR\CCF_000023.tif");
        }

        private void showMultiTiff(string tiffFileName)
        {
            FileStream tifFS = new FileStream(tiffFileName, FileMode.Open, FileAccess.Read);
            Image gim = Image.FromStream(tifFS);
            FrameDimension gfd = new FrameDimension(gim.FrameDimensionsList[0]);
            int pageCount = gim.GetFrameCount(gfd);//全体のページ数を得る 

            //System.Diagnostics.Debug.WriteLine(pageCount);
            //Graphics g = pictureBox1.CreateGraphics();

            for (int i = 0; i < pageCount; i++)
            {
                gim.SelectActiveFrame(gfd, i);

                gim.Save(@"C:\multitif\0902" + i.ToString().PadLeft(3, '0') + ".tif", ImageFormat.Tiff);

                //g.DrawImage(gim, 0, 0, pictureBox1.Width, pictureBox1.Height);//PictureBoxに表示してます
                //System.Threading.Thread.Sleep(500);
            }

            MessageBox.Show("finish!!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 50;
            trackBar1.Value = 0;

            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 10;

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {            
            n_width = B_WIDTH + (float)trackBar1.Value * 0.05f;
            n_height = B_HEIGHT + (float)trackBar1.Value * 0.05f;

            //imgShow(@"C:\CONYX_OCR\TIF\201808\201808_87654323_山田　哲人.tif", n_width, n_height);
            imgShow(mMat, n_width, n_height);
        }
    }
}
