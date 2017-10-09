﻿using System;
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
                    var pattern = command.Pattern;
                    var stitchSize = 2;

                    using (var surface = SKSurface.Create((int) pattern.Width * stitchSize,
                        (int) pattern.Height * stitchSize,
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
                        var content = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();
                        context.Parent.Tell(new Thumbnail
                        {
                            Id = command.Id,
                            Image = ByteString.CopyFrom(content)
                        });
                    }
                    break;
            }
            return Actor.Done;
        }
    }
}