using System.Threading.Tasks;
using Proto;
using SkiaSharp;

namespace SM.Service.Classes
{
    public class ThumbnailDrawer : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case CreateThumbnail command:
                    var pattern = command.Pattern;
                    const int stitchSize = 2;
                    using (var surface = SKSurface.Create((int) pattern.Width * stitchSize, (int) pattern.Height * stitchSize,
                        SKImageInfo.PlatformColorType, SKAlphaType.Premul))
                    {
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
                            surface.Canvas.DrawRect(rect, paint);
                        }
                        context.Parent.Tell(new Thumbnail
                        {
                            Id = command.Id,
                            Image = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray()
                        });
                    }
                    break;
            }
            return Actor.Done;
        }
    }
}
