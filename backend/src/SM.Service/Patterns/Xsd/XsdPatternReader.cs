using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SM.Service.Messages;

namespace SM.Service.Patterns.Xsd
{
    public class XsdPatternReader
    {
        public Messages.Pattern Read(byte[] content)
        {
            using (var stream = new MemoryStream(content) {Position = 0})
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var vendors = new Dictionary<int, XsdVendor>();
                var array = new byte[1024];
                stream.Read(array, 0, 2);
                var num = BitConverter.ToUInt16(array, 0);
                if (num == 1296)
                {
                    var pattern = new Pattern();
                    stream.Seek(741L, SeekOrigin.Begin);
                    stream.Read(array, 0, 14);
                    pattern.Width = BitConverter.ToUInt16(array, 0);
                    pattern.Height = BitConverter.ToUInt16(array, 2);
                    var propCount = BitConverter.ToUInt32(array, 4);
                    var count = BitConverter.ToUInt16(array, 8);
                    pattern.ThreadCountX = BitConverter.ToUInt16(array, 10);
                    pattern.ThreadCountY = BitConverter.ToUInt16(array, 12);
                    stream.Seek(761L, SeekOrigin.Begin);
                    stream.Read(array, 0, 1);
                    int num2 = array[0];
                    var count2 = 123;
                    stream.Seek(763L, SeekOrigin.Begin);
                    for (var i = 0; i < num2; i++)
                    {
                        stream.Read(array, 0, count2);
                        var color = new Color(i);
                        int key = array[2];
                        if (vendors.ContainsKey(key))
                        {
                            color.VendorCode = vendors[key].Code;
                            color.VendorTitle = vendors[key].Title;
                        }
                        else
                        {
                            color.VendorCode = "ZZZ";
                            color.VendorTitle = "Unknown";
                        }
                        color.ColorCode = ReadStringTrim(array, 3, 11);
                        color.ColorTitle = ReadStringTrim(array, 14, 41);
                        color.Rgb = new Rgb(array[55], array[56], array[57]);
                        int num3 = BitConverter.ToUInt16(array, 59);
                        var num4 = 61;
                        for (var j = 0; j < 4; j++)
                        {
                            key = array[num4];
                            num4++;
                            var colorCode = ReadStringTrim(array, num4, 11);
                            num4 += 11;
                            if (j < num3)
                            {
                                string vendorCode;
                                string vendorTitle;
                                if (vendors.ContainsKey(key))
                                {
                                    vendorCode = vendors[key].Code;
                                    vendorTitle = vendors[key].Title;
                                }
                                else
                                {
                                    vendorCode = "ZZZ";
                                    vendorTitle = "Unknown";
                                }
                                var blendColor = new Color.BlendColor
                                {
                                    VendorCode = vendorCode,
                                    VendorTitle = vendorTitle,
                                    ColorCode = colorCode
                                };
                                color.Blends.Add(blendColor);
                            }
                        }
                        for (var k = 0; k < num3; k++)
                        {
                            color.Blends[k].Strands.Full = array[num4];
                            color.Blends[k].Strands.Half = array[num4];
                            color.Blends[k].Strands.Quarter = array[num4];
                            color.Blends[k].Strands.ThreeQuarter = array[num4];
                            color.Blends[k].Strands.Petit = array[num4];
                            color.Blends[k].Strands.FrenchKnot = array[num4];
                            color.Blends[k].Strands.BackStitch = array[num4];
                            num4++;
                        }
                        pattern.Colors.Add(color);
                    }
                    stream.Seek(num2 * 2, SeekOrigin.Current);
                    for (var l = 0; l < num2 * 9; l++)
                    {
                        stream.Read(array, 0, 2);
                        int num5 = BitConverter.ToUInt16(array, 0);
                        if (num5 > 0)
                        {
                            var array2 = new byte[num5];
                            stream.Read(array2, 0, num5);
                            ReadStringTrim(array2, 0, num5);
                        }
                    }
                    for (var m = 0; m < num2 * 8; m++)
                    {
                        var index = (int) Math.Floor(m / 8f);
                        var num6 = m % 8;
                        stream.Read(array, 0, 2);
                        int num7 = BitConverter.ToUInt16(array, 0);
                        switch (num6)
                        {
                            case 0:
                                pattern.Colors[index].Strands.Full = num7;
                                break;
                            case 1:
                                pattern.Colors[index].Strands.Half = num7;
                                break;
                            case 2:
                                pattern.Colors[index].Strands.Quarter = num7;
                                break;
                            case 3:
                                pattern.Colors[index].Strands.BackStitch = num7;
                                break;
                            case 4:
                                pattern.Colors[index].Strands.FrenchKnot = num7;
                                break;
                            case 5:
                                pattern.Colors[index].Strands.Petit = num7;
                                break;
                        }
                    }
                    stream.Seek(2400L, SeekOrigin.Current);
                    stream.Seek(2400L, SeekOrigin.Current);
                    stream.Seek(960L, SeekOrigin.Current);
                    stream.Seek(2400L, SeekOrigin.Current);
                    stream.Seek(2400L, SeekOrigin.Current);
                    stream.Seek(2400L, SeekOrigin.Current);
                    stream.Seek(2400L, SeekOrigin.Current);
                    for (var n = 0; n < 240; n++)
                    {
                        stream.Read(array, 0, 53);
                        if (n < num2)
                            pattern.Colors[n].FontFamily = ReadStringTrim(array, 0, 33);
                    }
                    for (var num8 = 0; num8 < num2 * 6; num8++)
                    {
                        stream.Read(array, 0, 2);
                        var index2 = (int) Math.Floor(num8 / 6f);
                        if (pattern.Colors[index2].Symbol == "" && array[0] != 255 && array[1] != 255 && array[0] >= 32 && array[0] != 127 && array[0] != 160 && array[0] != 173)
                            try
                            {
                                pattern.Colors[index2].Symbol = Encoding.GetEncoding(1252).GetString(array, 0, 1);
                            }
                            catch
                            {
                                //
                            }
                    }
                    stream.Read(array, 0, 53);
                    var text = ReadStringTrim(array, 0, 33);
                    //if (!Font.fonts.ContainsKey(text))
                    //{
                    //    text = "CrossStitchDim";
                    //}
                    stream.Seek(33L, SeekOrigin.Current);
                    stream.Seek(2L, SeekOrigin.Current);
                    stream.Seek(4L, SeekOrigin.Current);
                    stream.Seek(28L, SeekOrigin.Current);
                    stream.Seek(120L, SeekOrigin.Current);
                    stream.Seek(120L, SeekOrigin.Current);
                    stream.Seek(12L, SeekOrigin.Current);
                    stream.Seek(2L, SeekOrigin.Current);
                    stream.Seek(2L, SeekOrigin.Current);
                    stream.Seek(101L, SeekOrigin.Current);
                    stream.Read(array, 0, 3);
                    pattern.Canvas.DefaultRGB = new Rgb(array[0], array[1], array[2]);
                    stream.Seek(65L, SeekOrigin.Current);
                    stream.Read(array, 0, 41);
                    pattern.Info.Title = ReadStringTrim(array, 0, 41);
                    stream.Read(array, 0, 41);
                    pattern.Info.Author = ReadStringTrim(array, 0, 41);
                    stream.Read(array, 0, 41);
                    pattern.Info.Company = ReadStringTrim(array, 0, 41);
                    stream.Read(array, 0, 201);
                    pattern.Info.Copyright = ReadStringTrim(array, 0, 201);
                    stream.Seek(2049L, SeekOrigin.Current);
                    stream.Seek(6L, SeekOrigin.Current);
                    stream.Read(array, 0, 31);
                    pattern.Canvas.Title = ReadStringTrim(array, 0, 31);
                    stream.Seek(216L, SeekOrigin.Current);
                    stream.Read(array, 0, 14);
                    pattern.Strands.Full = BitConverter.ToUInt16(array, 0);
                    pattern.Strands.Half = BitConverter.ToUInt16(array, 2);
                    pattern.Strands.Quarter = BitConverter.ToUInt16(array, 4);
                    pattern.Strands.BackStitch = BitConverter.ToUInt16(array, 6);
                    pattern.Strands.Petit = BitConverter.ToUInt16(array, 8);
                    pattern.Strands.FrenchKnot = 2;
                    pattern.Strands.ThreeQuarter = 2;
                    foreach (var current in pattern.Colors)
                    {
                        if (current.FontFamily == "default")
                            current.FontFamily = text;
                        if (current.Strands.BackStitch == 0)
                            current.Strands.BackStitch = pattern.Strands.BackStitch;
                        if (current.Strands.FrenchKnot == 0)
                            current.Strands.FrenchKnot = pattern.Strands.FrenchKnot;
                        if (current.Strands.Full == 0)
                            current.Strands.Full = pattern.Strands.Full;
                        if (current.Strands.Half == 0)
                            current.Strands.Half = pattern.Strands.Half;
                        if (current.Strands.Petit == 0)
                            current.Strands.Petit = pattern.Strands.Petit;
                        if (current.Strands.Quarter == 0)
                            current.Strands.Quarter = pattern.Strands.Quarter;
                        if (current.Strands.ThreeQuarter == 0)
                            current.Strands.ThreeQuarter = pattern.Strands.ThreeQuarter;
                    }
                    stream.Seek(16994L, SeekOrigin.Current);
                    LoadStitches(pattern, stream, propCount);
                    stream.Seek(2L, SeekOrigin.Current);
                    stream.Read(array, 0, 2);
                    var num9 = BitConverter.ToUInt16(array, 0);
                    for (var num10 = 0; num10 < (int) num9; num10++)
                    {
                        stream.Read(array, 0, 2);
                        var num11 = BitConverter.ToUInt16(array, 0);
                        if (num11 == 4)
                        {
                            stream.Seek(2L, SeekOrigin.Current);
                            stream.Read(array, 0, 4);
                            var @string = Encoding.ASCII.GetString(array, 0, 4);
                            if (@string == "sps1")
                            {
                                stream.Seek(256L, SeekOrigin.Current);
                                stream.Seek(256L, SeekOrigin.Current);
                                stream.Seek(2L, SeekOrigin.Current);
                                for (var num12 = 0; num12 < 3; num12++)
                                {
                                    stream.Read(array, 0, 10);
                                    stream.Read(array, 0, 2);
                                    num11 = BitConverter.ToUInt16(array, 0);
                                    if (num11 != 1296)
                                        break;
                                    stream.Read(array, 0, 2);
                                    var num13 = BitConverter.ToUInt16(array, 0);
                                    if (num13 > 0)
                                    {
                                        var list = new List<Node>();
                                        var list2 = new List<Backstitch>();
                                        LoadJoints(pattern, stream, num13, ref list, ref list2);
                                    }
                                }
                            }
                        }
                    }
                    LoadJoints(pattern, stream, count, ref pattern.Nodes, ref pattern.Backstitches);
                    stream.Dispose();

                    return Convert(pattern);
                }
            }

            return null;
        }

        private static uint Rol4(uint val, uint iterations)
        {
            var num = 32u;
            var num2 = iterations % num;
            return (val << (int) num2) | (val >> (int) (num - num2));
        }

        private static void Decrypt(ref uint[] rand1, out uint rand2, ref uint[] arr)
        {
            var num = 0u;
            rand2 = (rand1[3] & 255u) | ((BitConverter.GetBytes(rand1[2])[2] | (((rand1[0] << 8) | BitConverter.GetBytes(rand1[1])[1]) << 8)) << 8);
            var array = new byte[16];
            for (var i = 0; i < 4; i++)
            {
                var bytes = BitConverter.GetBytes(rand1[i]);
                for (var j = 0; j < 4; j++)
                    array[i * 4 + j] = bytes[j];
            }
            do
            {
                arr[(int) (UIntPtr) num] = (BitConverter.ToUInt32(array, (int) (4u * (num / 4u))) >> (int) (num % 4u)) % 32u;
                num += 1u;
            } while (num < 16u);
        }

        private static string ReadStringTrim(byte[] data, int offset, int length)
        {
            var @string = Encoding.UTF8.GetString(data, offset, length);
            var regex = new Regex("\\0.*");
            return regex.Replace(@string, "");
        }

        private static void LoadJoints(Pattern pattern, Stream stream, ushort count, ref List<Node> nodes, ref List<Backstitch> backstitches)
        {
            var array = new byte[512];
            var i = 0;
            while (i < count)
            {
                stream.Read(array, 0, 2);
                switch (BitConverter.ToUInt16(array, 0))
                {
                    case 1:
                    {
                        var node = new Node {Type = Node.NodeType.FrenchKnot};
                        stream.Read(array, 0, 12);
                        node.X = BitConverter.ToUInt16(array, 2);
                        node.Y = BitConverter.ToUInt16(array, 4);
                        if (array[10] < pattern.Colors.Count)
                            node.Color = pattern.Colors[array[10]];
                        nodes.Add(node);
                        break;
                    }
                    case 2:
                    case 5:
                    {
                        stream.Read(array, 0, 12);
                        var backstitch = new Backstitch
                        {
                            X1 = BitConverter.ToUInt16(array, 2),
                            Y1 = BitConverter.ToUInt16(array, 4),
                            X2 = BitConverter.ToUInt16(array, 6),
                            Y2 = BitConverter.ToUInt16(array, 8)
                        };
                        if (array[10] < pattern.Colors.Count)
                            backstitch.Color = pattern.Colors[array[10]];
                        backstitches.Add(backstitch);
                        break;
                    }
                    case 3:
                        goto IL_1C2;
                    case 4:
                        stream.Seek(23L, SeekOrigin.Current);
                        break;
                    case 6:
                    {
                        var node = new Node {Type = Node.NodeType.Bead};
                        stream.Read(array, 0, 10);
                        node.X = BitConverter.ToUInt16(array, 2);
                        node.Y = BitConverter.ToUInt16(array, 4);
                        if (array[6] < pattern.Colors.Count)
                            node.Color = pattern.Colors[array[6]];
                        nodes.Add(node);
                        break;
                    }
                    default:
                        goto IL_1C2;
                }
                IL_1EC:
                i++;
                continue;
                IL_1C2:
                stream.Seek(3L, SeekOrigin.Current);
                stream.Read(array, 0, 2);
                var num = BitConverter.ToUInt16(array, 0);
                stream.Seek(num * 4, SeekOrigin.Current);
                goto IL_1EC;
            }
        }

        private static void LoadStitches(Pattern pattern, Stream stream, uint propCount)
        {
            var array = new byte[32768];
            var num = 1073741824u;
            var num2 = 2147483648u;
            var num3 = pattern.Width * pattern.Height;
            var list = new List<uint>();
            uint num4;
            var array2 = new uint[16];
            var array3 = new uint[4];
            for (var i = 0; i < 4; i++)
            {
                stream.Read(array, 0, 4);
                array3[i] = BitConverter.ToUInt32(array, 0);
            }
            Decrypt(ref array3, out num4, ref array2);
            var num5 = 0;
            while (list.Count < num3)
            {
                stream.Read(array, 0, 4);
                var num7 = BitConverter.ToUInt32(array, 0);
                if (num7 != 0u)
                {
                    stream.Read(array, 0, (int) (num7 * 4u));
                    var array4 = new uint[num7];
                    var num8 = 0;
                    while (num8 < num7)
                    {
                        array4[num8] = BitConverter.ToUInt32(array, num8 * 4);
                        array4[num8] = array4[num8] ^ num4 ^ array3[0];
                        num4 = Rol4(num4, array2[num5]);
                        array3[0] += array3[1];
                        num5 = (num5 + 1) % 16;
                        num8++;
                    }
                    var num9 = 0;
                    while (num9 < num7)
                    {
                        var j = 1;
                        if ((array4[num9] & num) != 0u && (array4[num9] & num2) == 0u)
                        {
                            j = (int) ((array4[num9] & 1073741823u) >> 16);
                            num9++;
                        }
                        while (j > 0)
                        {
                            list.Add(array4[num9]);
                            j--;
                        }
                        num9++;
                    }
                }
            }
            var list2 = new List<byte[]>();
            var num10 = 0;
            while (num10 < propCount)
            {
                var array5 = new byte[10];
                stream.Read(array5, 0, 10);
                list2.Add(array5);
                num10++;
            }
            for (var k = 0; k < num3; k++)
            {
                if (k >= list.Count)
                    return;
                var bytes = BitConverter.GetBytes(list[k]);
                if (bytes[3] != 15)
                {
                    var num11 = (int) Math.Floor(k / (float) pattern.Width);
                    var num12 = k % pattern.Width;
                    var offset = num12 * pattern.Height + num11;
                    if ((bytes[3] & 128) != 0)
                    {
                        var num13 = ((list[k] & 65535u) >> 11) << 15;
                        var index = ((list[k] >> 16) & 32767u) + num13;
                        var array6 = list2[(int) index];
                        if ((array6[0] & 1) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[2]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.HalfTop
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[0] & 2) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[3]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.HalfBottom
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[0] & 4) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[4]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.QuarterTL
                            };

                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[0] & 16) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[6]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.QuarterTR
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[0] & 32) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[7]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.QuarterBR
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[0] & 8) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[5]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.QuarterBL
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[1] & 1) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[4]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.PetitTL
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[1] & 4) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[6]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.PetitTR
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[1] & 8) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[7]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.PetitBR
                            };
                            pattern.Stitches.Add(stitch);
                        }
                        if ((array6[1] & 2) != 0)
                        {
                            var stitch = new Stitch
                            {
                                Color = pattern.Colors[array6[5]],
                                Offset = (uint) offset,
                                Count = 1,
                                Type = Stitch.StitchType.PetitBL
                            };
                            pattern.Stitches.Add(stitch);
                        }
                    }
                    else
                    {
                        var stitch = new Stitch
                        {
                            Color = pattern.Colors[bytes[2]],
                            Offset = (uint) offset,
                            Count = 1,
                            Type = Stitch.StitchType.Full
                        };
                        pattern.Stitches.Add(stitch);
                    }
                }
            }
        }

        private static Messages.Pattern Convert(Pattern pattern)
        {
            // TODO: Cover property mapping with tests.
            var result = new Messages.Pattern
            {
                Width = pattern.Width,
                Height = pattern.Height,
                Canvas = new Messages.Canvas
                {
                    Title = pattern.Canvas.Title
                },
                Info = new Messages.Info
                {
                    Title = pattern.Info.Title,
                    Author = pattern.Info.Author,
                    Company = pattern.Info.Company,
                    Copyright = pattern.Info.Copyright
                },
                StrandsCount = new StrandsCount
                {
                    Backstitch = pattern.Strands.BackStitch,
                    FrenchKnot = pattern.Strands.FrenchKnot,
                    Full = pattern.Strands.Full,
                    Half = pattern.Strands.Half,
                    Petit = pattern.Strands.Petit,
                    Quarter = pattern.Strands.Quarter,
                    ThreeQuarter = pattern.Strands.ThreeQuarter
                }
            };

            foreach (var color in pattern.Colors)
                result.Configurations.Add(new StitchConfiguration
                {
                    Symbol = color.Symbol,
                    HexColor = "#" + color.Rgb.R.ToString("x2") + color.Rgb.G.ToString("x2") +
                               color.Rgb.B.ToString("x2")
                });

            foreach (var stitch in pattern.Stitches)
                result.Stitches.Add(new Messages.Stitch
                {
                    Point = new Point {X = stitch.Offset / pattern.Height, Y = stitch.Offset % pattern.Height},
                    Type = (StitchType) Enum.Parse(typeof(StitchType), stitch.ItemType.ToString()),
                    ConfigurationIndex = stitch.Color.Index
                });

            foreach (var backstitch in pattern.Backstitches)
                result.Backstitches.Add(new Messages.Backstitch
                {
                    StartPoint = new Point {X = backstitch.X1, Y = backstitch.Y1},
                    EndPoint = new Point {X = backstitch.X2, Y = backstitch.Y2},
                    ConfigurationIndex = backstitch.Color.Index
                });

            foreach (var node in pattern.Nodes)
                result.Elements.Add(new Element
                {
                    Point = new Point {X = node.X, Y = node.Y},
                    Type = (ElementType) Enum.Parse(typeof(ElementType), node.Type.ToString()),
                    ConfigurationIndex = node.Color.Index
                });

            // TODO: Find out what to do with ThreadCountX and ThreadCountY.

            return result;
        }

        private struct XsdVendor
        {
            public XsdVendor(string code, string title)
            {
                Code = code;
                Title = title;
            }

            public string Code { get; }
            public string Title { get; }
        }
    }
}
