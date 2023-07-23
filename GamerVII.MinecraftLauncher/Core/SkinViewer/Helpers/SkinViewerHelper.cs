using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GamerVII.MinecraftLauncher.Core.SkinViewer.Helpers
{
    public class SkinViewerHelper
    {

        /// <summary>
        /// Async download file
        /// </summary>
        /// <param name="url">Url file to load</param>
        /// <returns>ImageSource</returns>
        public static async Task<ImageSource> LoadImageAsync(string url)
        {

            try
            {
                using (var httpClient = new HttpClient())
                {

                    using (var response = await httpClient.GetAsync(url))
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();

                            return bitmap;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load image from {url}: {ex.Message}");
                return null;
            }

        }


        /// <summary>
        /// Crop Image
        /// </summary>
        /// <param name="imageSource">Image Source</param>
        /// <param name="x">X point</param>
        /// <param name="y">Y point</param>
        /// <param name="width">Crop width</param>
        /// <param name="height">Crop height</param>
        /// <returns></returns>
        public static ImageSource CropImage(ImageSource imageSource, int x, int y, int width, int height)
        {
            var croppedBitmap = new CroppedBitmap((BitmapSource)imageSource, new Int32Rect(x, y, width, height));
            croppedBitmap.Freeze();
            return croppedBitmap;

        }



        public static ImageSource ResizeImage(byte[] imageBytes, int newWidth, int newHeight)
        {
            using (var image = Image.Load<Rgba32>(imageBytes))
            {
                image.Mutate(x => x.Resize(newWidth, newHeight, KnownResamplers.NearestNeighbor));

                using (var stream = new MemoryStream())
                {
                    image.Save(stream, new PngEncoder());

                    stream.Seek(0, SeekOrigin.Begin);
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }

            }
        }

        internal static ImageSource ResizeImage(ImageSource imageSource, int newWidth, int newHeight)
        {
            byte[] bytes = ImageSourceToBytes(imageSource);
            return ResizeImage(bytes, newWidth, newHeight);
        }

        private static byte[] ImageSourceToBytes(ImageSource imageSource)
        {
            var encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        internal static ImageSource CombineImageSources(ImageSource head, ImageSource body, ImageSource leg, ImageSource arm, int width, int height)
        {

            var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                var yPosition = 0.0;

                // head
                var rectangle = new Rect(arm.Width, yPosition, head.Width, head.Height);
                context.DrawImage(head, rectangle);
                yPosition += head.Height;

                // body
                rectangle = new Rect(arm.Width, yPosition, body.Width, body.Height);
                context.DrawImage(body, rectangle);

                // left arm
                rectangle = new Rect(0, yPosition, arm.Width, arm.Height);
                context.DrawImage(arm, rectangle);

                // right arm
                rectangle = new Rect(arm.Width + body.Width, yPosition, arm.Width, arm.Height);
                context.DrawImage(FlipImageFromHorizontally(arm), rectangle);
                yPosition += body.Height;

                // left leg
                rectangle = new Rect(arm.Width, yPosition, leg.Width, leg.Height);
                context.DrawImage(leg, rectangle);

                // right leg
                rectangle = new Rect(arm.Width + leg.Width, yPosition, leg.Width, leg.Height);
                context.DrawImage(FlipImageFromHorizontally(leg), rectangle);


            }

            renderBitmap.Render(visual);

            return renderBitmap;
        }

        private static ImageSource FlipImageFromHorizontally(ImageSource imageSource)
        {

            var transformedBitmap = new TransformedBitmap();
            transformedBitmap.BeginInit();
            transformedBitmap.Source = (BitmapSource)imageSource;
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(-1, 1));
            transformGroup.Children.Add(new TranslateTransform(-imageSource.Width, 0));
            transformedBitmap.Transform = transformGroup;
            transformedBitmap.EndInit();

            return transformedBitmap;
        }
    }
}
