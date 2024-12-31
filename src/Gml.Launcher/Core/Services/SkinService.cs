using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace Gml.Launcher.Core.Services;

public abstract class SkinViewer
{
    public static byte[] GetHead(string skinPath, int size)
    {
        if (!File.Exists(skinPath))
        {
            return [];
        }

        using var inputImage = Image.Load(skinPath);

        var scaleFactor = inputImage.Width / 64;

        var croppedImage = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(8 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor)));

        var memoryStream = ResizeImage(size, inputImage, croppedImage);

        return memoryStream.ToArray();
    }

    private static int GetScaleSize(double size, int croppedImageWidth)
    {
        return Convert.ToInt32(Math.Round(size / croppedImageWidth));
    }

    public static byte[] GetCloak(string cloakPath, int size)
    {
        using var inputImage = Image.Load(cloakPath);

        var scaleFactor = inputImage.Width / 64;

        var croppedImage = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(0, 0, 11 * scaleFactor, 17 * scaleFactor)));

        var memoryStream = ResizeImage(size, inputImage, croppedImage);

        return memoryStream.ToArray();
    }

    public static byte[] GetFront(Stream skinStream, int size)
    {
        using var inputImage = Image.Load(skinStream);

        var scaleFactor = inputImage.Width / 64;
        var extendedSkin = inputImage.Height / scaleFactor >= 64;

        var croppedHead = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(8 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor)));
        var croppedBody = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(20 * scaleFactor, 20 * scaleFactor, 8 * scaleFactor, 12 * scaleFactor)));
        var leftCroppedLeg = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(4 * scaleFactor, 20 * scaleFactor, 4 * scaleFactor, 12 * scaleFactor)));
        var leftCroppedArm = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(44 * scaleFactor, 20 * scaleFactor, 4 * scaleFactor, 12 * scaleFactor)));

        var rightCroppedLeg = extendedSkin
            ? inputImage.Clone(ctx =>
                ctx.Crop(new Rectangle(20 * scaleFactor, 52 * scaleFactor, 4 * scaleFactor, 12 * scaleFactor)))
            : leftCroppedLeg.Clone(x => x.Flip(FlipMode.Horizontal));

        var rightCroppedArm = extendedSkin
            ? inputImage.Clone(ctx =>
                ctx.Crop(new Rectangle(36 * scaleFactor, 52 * scaleFactor, 4 * scaleFactor, 12 * scaleFactor)))
            : leftCroppedArm.Clone(x => x.Flip(FlipMode.Horizontal));

        var secondLayerHead = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(40 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor, 8 * scaleFactor)));

        // var secondLayerArm = inputImage.Clone(ctx =>
        //     ctx.Crop(new Rectangle(44 * scaleFactor, 52 * scaleFactor, 4 * scaleFactor, 12 * scaleFactor)));

        var newWidth = leftCroppedArm.Width * 2 + croppedBody.Width;
        var newHeight = croppedHead.Height + croppedBody.Height + leftCroppedArm.Height;

        var combinedImage = new Image<Rgba32>(newWidth, newHeight);

        combinedImage.Mutate(context =>
        {
            var headPosition = new Rectangle(leftCroppedArm.Width, 0, croppedHead.Width, croppedHead.Height);
            var bodyPosition = new Rectangle(headPosition.X, croppedHead.Height, croppedBody.Width, croppedBody.Height);

            var leftArmPosition = new Rectangle(0, bodyPosition.Y, leftCroppedArm.Width, leftCroppedArm.Height);
            var rightArmPosition = new Rectangle(leftArmPosition.Width + bodyPosition.Width, leftArmPosition.Y,
                leftArmPosition.Width, leftArmPosition.Height);

            var leftLegPosition = new Rectangle(leftArmPosition.Width, headPosition.Height + bodyPosition.Height,
                leftCroppedLeg.Width, leftCroppedLeg.Height);
            var rightLegPosition = new Rectangle(leftArmPosition.Width + leftLegPosition.Width,
                headPosition.Height + bodyPosition.Height, leftLegPosition.Width, leftLegPosition.Height);

            context.DrawImage(croppedHead, new Point(headPosition.X, headPosition.Y), 1f);
            context.DrawImage(croppedBody, new Point(bodyPosition.X, bodyPosition.Y), 1f);
            context.DrawImage(leftCroppedArm, new Point(leftArmPosition.X, leftArmPosition.Y), 1f);
            context.DrawImage(rightCroppedArm, new Point(rightArmPosition.X, rightArmPosition.Y), 1f);
            context.DrawImage(leftCroppedLeg, new Point(leftLegPosition.X, leftLegPosition.Y), 1f);
            context.DrawImage(rightCroppedLeg, new Point(rightLegPosition.X, rightLegPosition.Y), 1f);

            context.DrawImage(secondLayerHead, new Point(headPosition.X, headPosition.Y), 1f);
        });

        if (size != combinedImage.Width)
        {
            var scaleSize = GetScaleSize(size, combinedImage.Width);
            if (scaleSize != 0)
                combinedImage = combinedImage.Clone(ctx => ctx.Resize(combinedImage.Width * scaleSize,
                    combinedImage.Height * scaleSize, KnownResamplers.Box));
        }

        using var memoryStream = new MemoryStream();
        combinedImage.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }


    public static byte[] GetBack(string skinPath, int size, bool includeCloak = false)
    {
        using var inputImage = Image.Load(skinPath);
        Image? cloakImage = /*includeCloak && user.HasCloak ? Image.Load(user.CloakFullPath) :*/ default;

        var skinScaleFactor = inputImage.Width / 64;
        var cloakScaleFactor = cloakImage?.Width >= 64 ? cloakImage.Width / 64 : 1;
        var needSkinResizeToCloakSize = cloakScaleFactor > skinScaleFactor;
        var extendedSkin = inputImage.Height / skinScaleFactor >= 64;

        var croppedHead = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(24 * skinScaleFactor, 8 * skinScaleFactor, 8 * skinScaleFactor,
                8 * skinScaleFactor)));

        var croppedBody = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(32 * skinScaleFactor, 20 * skinScaleFactor, 8 * skinScaleFactor,
                12 * skinScaleFactor)));

        var croppedRightLeg = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(12 * skinScaleFactor, 20 * skinScaleFactor, 4 * skinScaleFactor,
                12 * skinScaleFactor)));

        var croppedRightArm = inputImage.Clone(ctx =>
            ctx.Crop(new Rectangle(52 * skinScaleFactor, 20 * skinScaleFactor, 4 * skinScaleFactor,
                12 * skinScaleFactor)));

        var croppedLeftLeg = extendedSkin
            ? inputImage.Clone(ctx =>
                ctx.Crop(new Rectangle(28 * skinScaleFactor, 52 * skinScaleFactor, 4 * skinScaleFactor,
                    12 * skinScaleFactor)))
            : croppedRightLeg.Clone(x => x.Flip(FlipMode.Horizontal));

        var croppedLeftArm = extendedSkin
            ? inputImage.Clone(ctx =>
                ctx.Crop(new Rectangle(44 * skinScaleFactor, 52 * skinScaleFactor, 4 * skinScaleFactor,
                    12 * skinScaleFactor)))
            : croppedRightArm.Clone(x => x.Flip(FlipMode.Horizontal));

        // var croppedRightArm = croppedLeftArm.Clone(x => x.Flip(FlipMode.Horizontal));

        if (needSkinResizeToCloakSize)
        {
            croppedHead = ResizeImage(croppedHead, cloakScaleFactor);
            croppedBody = ResizeImage(croppedBody, cloakScaleFactor);
            croppedLeftLeg = ResizeImage(croppedLeftLeg, cloakScaleFactor);
            croppedRightLeg = ResizeImage(croppedRightLeg, cloakScaleFactor);
            croppedLeftArm = ResizeImage(croppedLeftArm, cloakScaleFactor);
            croppedRightArm = ResizeImage(croppedRightArm, cloakScaleFactor);
        }

        Image croppedCloak = null;

        if (includeCloak && cloakImage != null)
            croppedCloak = cloakImage.Clone(ctx =>
                    ctx.Crop(new Rectangle(0, 0, 11 * cloakScaleFactor, 17 * cloakScaleFactor)))
                .Clone(ctx => ctx.Resize(croppedBody.Width, croppedBody.Height, KnownResamplers.Box));

        var newWidth = croppedLeftArm.Width * 2 + croppedBody.Width;
        var newHeight = croppedHead.Height + croppedBody.Height + croppedLeftArm.Height;

        var combinedImage = new Image<Rgba32>(newWidth, newHeight);


        combinedImage.Mutate(context =>
        {
            var headPosition = new Rectangle(croppedLeftArm.Width, 0, croppedHead.Width, croppedHead.Height);
            var bodyPosition = new Rectangle(headPosition.X, croppedHead.Height, croppedBody.Width, croppedBody.Height);

            var leftArmPosition = new Rectangle(0, bodyPosition.Y, croppedLeftArm.Width, croppedLeftArm.Height);
            var rightArmPosition = new Rectangle(leftArmPosition.Width + bodyPosition.Width, leftArmPosition.Y,
                leftArmPosition.Width, leftArmPosition.Height);

            var leftLegPosition = new Rectangle(leftArmPosition.Width, headPosition.Height + bodyPosition.Height,
                croppedLeftLeg.Width, croppedLeftLeg.Height);
            var rightLegPosition = new Rectangle(leftArmPosition.Width + leftLegPosition.Width,
                headPosition.Height + bodyPosition.Height, leftLegPosition.Width, leftLegPosition.Height);

            context.DrawImage(croppedHead, new Point(headPosition.X, headPosition.Y), 1f);
            context.DrawImage(croppedBody, new Point(bodyPosition.X, bodyPosition.Y), 1f);
            context.DrawImage(croppedLeftArm, new Point(leftArmPosition.X, leftArmPosition.Y), 1f);
            context.DrawImage(croppedRightArm, new Point(rightArmPosition.X, rightArmPosition.Y), 1f);
            context.DrawImage(croppedLeftLeg, new Point(leftLegPosition.X, leftLegPosition.Y), 1f);
            context.DrawImage(croppedRightLeg, new Point(rightLegPosition.X, rightLegPosition.Y), 1f);


            if (croppedCloak != null) context.DrawImage(croppedCloak, new Point(bodyPosition.X, bodyPosition.Y), 1f);


            // context.DrawImage(croppedArm, new Point(0, 8 * scaleFactor), 1f);
            // context.DrawImage(croppedLeg, new Point(200, 200), 1f);
        });

        using var memoryStream = new MemoryStream();
        if (inputImage.Metadata.DecodedImageFormat != null)
            combinedImage.Save(memoryStream, inputImage.Metadata.DecodedImageFormat);
        else
            combinedImage.Save(memoryStream, new PngEncoder());

        // var image = ResizeImage(size, inputImage, combinedImage);

        if (size != combinedImage.Width)
        {
            var scaleSize = GetScaleSize(size, combinedImage.Width);

            if (scaleSize != 0)
                combinedImage = combinedImage.Clone(ctx => ctx.Resize(combinedImage.Width * scaleSize,
                    combinedImage.Height * scaleSize, KnownResamplers.Box));
        }

        using var resizeMemoryStream = new MemoryStream();

        if (inputImage.Metadata.DecodedImageFormat != null)
            combinedImage.Save(resizeMemoryStream, inputImage.Metadata.DecodedImageFormat);
        else
            combinedImage.Save(resizeMemoryStream, new PngEncoder());

        return resizeMemoryStream.ToArray();
    }

    private static Image ResizeImage(Image croppedHead, int cloakScaleFactor)
    {
        var newSize = croppedHead.Size;
        croppedHead = croppedHead.Clone(ctx =>
            ctx.Resize(newSize.Width * cloakScaleFactor, newSize.Height * cloakScaleFactor, KnownResamplers.Box));
        return croppedHead;
    }


    private static MemoryStream ResizeImage(int size, Image inputImage, Image croppedImage)
    {
        if (size != croppedImage.Width)
        {
            var scaleSize = GetScaleSize(size, croppedImage.Width);
            croppedImage = croppedImage.Clone(ctx => ctx.Resize(croppedImage.Width * scaleSize,
                croppedImage.Height * scaleSize, KnownResamplers.Box));
        }

        using var memoryStream = new MemoryStream();

        if (inputImage.Metadata.DecodedImageFormat != null)
            croppedImage.Save(memoryStream, inputImage.Metadata.DecodedImageFormat);
        else
            croppedImage.Save(memoryStream, new PngEncoder());

        return memoryStream;
    }
}
