using SkiaSharp;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class ThumbnailDrawer
    {
        public byte[] Draw(PatternState state)
        {
            const SKAlphaType alphaType = SKAlphaType.Premul;
            var imageInfo = SKImageInfo.PlatformColorType;
            using (var surface = SKSurface.Create((int) state.Width, (int) state.Height, imageInfo, alphaType))
            {
                var canvas = surface.Canvas;
                var bitmap = new SKBitmap((int) state.Width, (int) state.Height, imageInfo, alphaType);
                bitmap.Erase(SKColor.Empty);

                foreach (var stitch in state.Stitches)
                {
                    var configuration = state.Configurations[stitch.ConfigurationIndex];
                    var color = SKColor.Parse(configuration.HexColor);
                    bitmap.SetPixel((int) stitch.Point.X, (int) stitch.Point.Y, color);
                }
                canvas.DrawBitmap(bitmap, new SKRect(0, 0, state.Width, state.Height));
                return surface.Snapshot().Encode().ToArray();
            }
        }
    }
}
