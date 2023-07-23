using GamerVII.MinecraftLauncher.Core.SkinViewer.Helpers;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GamerVII.MinecraftLauncher.Core.SkinViewer;

public class SkinViewerManager
{

    public ImageSource ImageSource;
    public Size ImageSize;

    private int _scaleFactor;
    private string _imageUrl;


    public SkinViewerManager(string url)
    {
        _imageUrl = url;
    }


    public async Task LoadAsync()
    {
        ImageSource = await SkinViewerHelper.LoadImageAsync(_imageUrl);

        if (ImageSource != null)
        {
            ImageSize = new Size(Convert.ToInt32(ImageSource.Width), Convert.ToInt32(ImageSource.Height));

            _scaleFactor = Convert.ToInt32(ImageSource.Width / 64);
        }
    }

    internal ImageSource GetHead(int scale = 1)
    {

        if (ImageSource == null)
        {
            throw new ArgumentNullException(nameof(ImageSource));
        }

        var croppedImage = SkinViewerHelper.CropImage(ImageSource, 8 * _scaleFactor, 8 * _scaleFactor, 8 * _scaleFactor, 8 * _scaleFactor);

        return SkinViewerHelper.ResizeImage(croppedImage, Convert.ToInt32(croppedImage.Width) * scale, Convert.ToInt32(croppedImage.Height) * scale);

    }

    internal ImageSource GetBody(int scale = 1)
    {

        if (ImageSource == null)
        {
            throw new ArgumentNullException(nameof(ImageSource));
        }

        var croppedImage = SkinViewerHelper.CropImage(ImageSource, 20 * _scaleFactor, 20 * _scaleFactor, 8 * _scaleFactor, 12 * _scaleFactor);

        return SkinViewerHelper.ResizeImage(croppedImage, Convert.ToInt32(croppedImage.Width) * scale, Convert.ToInt32(croppedImage.Height) * scale);

    }

    internal ImageSource GetLeg(int scale = 1)
    {

        if (ImageSource == null)
        {
            throw new ArgumentNullException(nameof(ImageSource));
        }

        var croppedImage = SkinViewerHelper.CropImage(ImageSource, 4 * _scaleFactor, 20 * _scaleFactor, 4 * _scaleFactor, 12 * _scaleFactor);

        return SkinViewerHelper.ResizeImage(croppedImage, Convert.ToInt32(croppedImage.Width) * scale, Convert.ToInt32(croppedImage.Height) * scale);

    }

    internal ImageSource GetArm(int scale = 1)
    {

        if (ImageSource == null)
        {
            throw new ArgumentNullException(nameof(ImageSource));
        }

        var croppedImage = SkinViewerHelper.CropImage(ImageSource, 44 * _scaleFactor, 20 * _scaleFactor, 4 * _scaleFactor, 12 * _scaleFactor);

        return SkinViewerHelper.ResizeImage(croppedImage, Convert.ToInt32(croppedImage.Width) * scale, Convert.ToInt32(croppedImage.Height) * scale);
    }

    internal ImageSource GetFront(int scale = 1)
    {

        var head = GetHead(scale);
        var body = GetBody(scale);
        var leg = GetLeg(scale);
        var arm = GetArm(scale);

        return SkinViewerHelper.CombineImageSources(head, body, leg, arm, ImageSize.Width * scale, ImageSize.Height * scale);
    }
}
