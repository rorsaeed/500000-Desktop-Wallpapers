using DevExpress.XtraEditors;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AiWallpapers.stableDb;
using DevExpress.Xpo;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace AiWallpapers
{
    public partial class UploadThump : DevExpress.XtraEditors.XtraForm
    {
        private List<List<tmpImage>> _allImages;

        public UploadThump()
        {
            InitializeComponent();
        }
        public  List<List<tmpImage>> SplitList(List<tmpImage> originalList, int numberOfLists)
        {
            // Calculate the size of each list
            int sizePerList = (int)Math.Ceiling(originalList.Count / (double)numberOfLists);

            // Create the list of lists
            var splitLists = new List<List<tmpImage>>();

            for (int i = 0; i < numberOfLists; i++)
            {
                // Get the range for the current list
                var currentList = originalList
                    .Skip(i * sizePerList)
                    .Take(sizePerList)
                    .ToList();

                // Add the current list to the list of lists
                splitLists.Add(currentList);
            }

            return splitLists;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var imges = unitOfWork1.Query<tmpImage>().Where(x=>x.thumpUri==null).ToList();
            _allImages= SplitList(imges, 8);
            this.Text = _allImages[0].Count.ToString();
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            backgroundWorker3.RunWorkerAsync();
            backgroundWorker4.RunWorkerAsync();
            backgroundWorker5.RunWorkerAsync();
            backgroundWorker6.RunWorkerAsync();
            backgroundWorker7.RunWorkerAsync();
            backgroundWorker8.RunWorkerAsync();
        }


        public static Image LoadImage(string filePath)
        {
            // Ensure the file exists before trying to load it
            if (System.IO.File.Exists(filePath))
            {
                // Load the image
                Image image = Image.FromFile(filePath);
                return image;
            }
            return null;
        }
        private Image ResizeImageWithShortestEdge(string imagePth, int shortestEdgeSize)
        {
            Image originalImage = LoadImage(imagePth);
            // Determine the ratio to resize the image
            float scale = shortestEdgeSize / (float)(originalImage.Width < originalImage.Height ? originalImage.Width : originalImage.Height);
            int newWidth = (int)(originalImage.Width * scale);
            int newHeight = (int)(originalImage.Height * scale);

            // Create a new Bitmap with the proper dimensions
            Bitmap resultImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resultImage))
            {
                // Set the resize quality modes
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                // Draw the resized image
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resultImage;
        }
        private ByteArrayContent ConvertImageToByteArrayContent(Image image)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save the image to the memory stream in a desired format
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    // Reset the position of the memory stream to the beginning
                    ms.Position = 0;

                    // Create ByteArrayContent from the memory stream
                    ByteArrayContent byteArrayContent = new ByteArrayContent(ms.ToArray());

                    // Optionally set the content type if needed
                    // For example, if it's an image, you might set it to "image/jpeg"
                    // byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    return byteArrayContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting image to ByteArrayContent: " + ex.Message);
                return null;
            }
        }
        private async void UploadAll(List<tmpImage> images)
        {
            int cnt1 = 0;
            int cnt2 = 0;
            foreach (var image in images)
            {
                cnt1++;
                cnt2++;
                if (cnt2>20)
                {
                    cnt2 = 0;
                    Console.WriteLine(cnt1);
                }
                if (cnt1 > 5000) break;
                if (!string.IsNullOrWhiteSpace(image.thumpUri))continue;
                string imagePath = image.imagePath;
                if(!File.Exists(imagePath)) { Console.WriteLine("nofile"); continue;}
                string apiEndpoint = "https://freeimage.host/api/1/upload";
                string apiKey = "6d207e02198a847aa98d0a2a901485a5";
                try
                {

                    using (var client = new HttpClient())
                    using (var formData = new MultipartFormDataContent())
                    {
                        // إضافة مفتاح API إلى الطلب
                        formData.Add(new StringContent(apiKey), "key");

                        // إضافة الصورة إلى الطلب
                        formData.Add(ConvertImageToByteArrayContent(ResizeImageWithShortestEdge(imagePath, 256)), "source", Path.GetFileName(imagePath));

                        // إرسال الطلب
                        var response = await client.PostAsync(apiEndpoint, formData);

                        // التحقق من نجاح الطلب
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("error.");
                            //return;
                        }

                        // قراءة الرد واستخراج رابط الصورة
                        string responseBody = await response.Content.ReadAsStringAsync();
                        //textBox2.Text = responseBody;
                        // يجب تحليل الرد هنا لاستخراج الرابط، يمكن استخدام JSON.NET أو System.Text.Json

                        //Console.WriteLine("تم رفع الصورة بنجاح. رابط الصورة: {رابط_الصورة}");

                        JsonDocument doc = JsonDocument.Parse(responseBody);
                        JsonElement root = doc.RootElement;
                        JsonElement imageUrlElement = root.GetProperty("image").GetProperty("url");

                        string imageUrl = imageUrlElement.GetString();
                        //Console.WriteLine("Image URL: " + imageUrl);
                        image.thumpUri= imageUrl;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);

                }
            
        }
            Console.WriteLine("finish.");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            unitOfWork1.CommitChanges();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[0]);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[1]);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[2]);
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[3]);
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[4]);
        }

        private void backgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[5]);
        }

        private void backgroundWorker7_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[6]);
        }

        private void backgroundWorker8_DoWork(object sender, DoWorkEventArgs e)
        {
            UploadAll(_allImages[7]);
        }
    }
}