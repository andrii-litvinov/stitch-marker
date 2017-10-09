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
                    var thumbnail = CreateThumbnail(command.Pattern);
                    context.Parent.Tell(new Thumbnail
                    {
                        Id = command.Id,
                        Image = ByteString.CopyFrom(thumbnail)
                    });
                    break;
            }
            return Actor.Done;
        }

        private static byte[] CreateThumbnail(Pattern pattern)
        {
            const int stitchSize = 2;
            var width = (int) pattern.Width * stitchSize;
            var height = (int) pattern.Height * stitchSize;

            var bitmap = new SKBitmap(width, height);
            bitmap.Erase(SKColor.Empty);
            
            var canvas = new SKCanvas(bitmap);

            foreach (var stitch in pattern.Stitches)
            {
                var configuration = pattern.Configurations[stitch.ConfigurationIndex];
                var paint = new SKPaint {Color = SKColor.Parse(configuration.HexColor)};
                var rect = new SKRect
                {
                    Left = stitch.Point.X * stitchSize,
                    Top = stitch.Point.Y * stitchSize,
                    Right = (stitch.Point.X + 1) * stitchSize,
                    Bottom = (stitch.Point.Y + 1) * stitchSize
                };

                canvas.DrawRect(rect, paint);
            }
            var image = SKImage.FromBitmap(bitmap);
            var cropped = image.Subset(SKRectI.Create(width, height));
            var data = cropped.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }
    }
}
