using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Proto;
using SkiaSharp;
using SM.Service.Messages;

namespace SM.Service.Patterns
{
    public class PatternImageActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case GetThumbnail command:
                    var thumbnail = CreateThumbnail(command.Pattern, command.Width, command.Height);
                    context.Parent.Tell(new Thumbnail
                    {
                        Id = command.Id,
                        Image = ByteString.CopyFrom(thumbnail)
                    });
                    break;
            }
            return Actor.Done;
        }

        private static byte[] CreateThumbnail(Pattern pattern, int thumbnailWidth, int thumbnailHeight)
        {
            var size = GetStitchSize((int) pattern.Width, (int) pattern.Height, thumbnailWidth, thumbnailHeight);
            var width = pattern.Width * size;
            var height = pattern.Height * size;
            var bitmap = new SKBitmap((int) width, (int) height);
            var canvas = new SKCanvas(bitmap);

            canvas.Clear();

            foreach (var stitch in pattern.Stitches)
            {
                var configuration = pattern.Configurations[stitch.ConfigurationIndex];
                var paint = new SKPaint {Color = SKColor.Parse(configuration.HexColor)};
                var rect = new SKRect
                {
                    Left = stitch.Point.X * size,
                    Top = stitch.Point.Y * size,
                    Right = (stitch.Point.X + 1) * size,
                    Bottom = (stitch.Point.Y + 1) * size
                };
                canvas.DrawRect(rect, paint);
            }

            var x = (int) (width - thumbnailWidth) / 2;
            var y = (int) (height - thumbnailHeight) / 2;
            var image = SKImage.FromBitmap(bitmap).Subset(SKRectI.Create(x, y, thumbnailWidth, thumbnailHeight));

            return image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }

        private static float GetStitchSize(int patternWidth, int patternHeight, int thumbnailWidth, int thumbnailHeight)
        {
            if (patternHeight >= thumbnailHeight && patternWidth >= thumbnailWidth) return 1;

            var heightRatio = thumbnailHeight / (float) patternHeight;
            var widthRatio = thumbnailWidth / (float) patternWidth;

            return Math.Max(heightRatio, widthRatio);
        }
    }
}
