using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AutoFixture.Xunit2;
using Google.Protobuf;
using Newtonsoft.Json;
using SM.Service.Patterns;
using SM.Service.Patterns.Xsd;
using Xunit;
using Xunit.Abstractions;
using Stitch = SM.Service.Stitch;
using Pattern = SM.Service.Pattern;

namespace SM.Service.Tests
{
    public class ProtobufSerializerShould
    {
        private readonly ITestOutputHelper output;

        public ProtobufSerializerShould(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Theory]
        [AutoData]
        public void SerializeSimpleObject(XsdPatternReader reader)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var pattern = reader.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));
            
            output.WriteLine($"Pattern: {stopwatch.ElapsedMilliseconds} ms");
            
            var pattern1 = new Pattern();

            pattern1.Stitches.Add(pattern.Stitches.Select(stitch => new Stitch
            {
                X = stitch.X,
                Y = stitch.Y,
                Type = (StitchType) (int) stitch.Type,
                ConfigurationIndex = stitch.ConfigurationIndex
            }));

            stopwatch.Restart();

            var patternBytes = pattern1.ToByteArray();
            
            output.WriteLine($"Proto: {stopwatch.ElapsedMilliseconds} ms");
            
            File.WriteAllBytes("proto.pattern", patternBytes);
            
            stopwatch.Restart();

            var json = JsonConvert.SerializeObject(pattern.Stitches);
            
            output.WriteLine($"JSON: {stopwatch.ElapsedMilliseconds} ms");
            
            File.WriteAllBytes("json.pattern", Encoding.UTF8.GetBytes(json));
        }

        [Theory]
        [AutoData]
        public void SerializeStitches()
        {
        }
    }
}
