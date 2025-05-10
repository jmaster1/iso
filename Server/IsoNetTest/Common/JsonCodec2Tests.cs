using System.Text;
using IsoNet.Core.IO.Codec;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IsoNetTest.Common;

public class JsonCodec2Tests : AbstractTests
{
    private class CustomClass
    {
        public string? Str;
        public int N;

        protected bool Equals(CustomClass other)
        {
            return Str == other.Str && N == other.N;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CustomClass)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Str, N);
        }
    }

    private class CustomClassJsonConverter : JsonConverter
    {
        public int WriteCount, ReadCount;
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is not CustomClass custom)
            {
                writer.WriteNull();
                return;
            }

            var val = custom.N + "_" + custom.Str;
            writer.WriteValue(val);
            WriteCount++;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value))
                return null;

            var parts = value.Split('_', 2);
            if (parts.Length < 2 || !int.TryParse(parts[0], out var n))
                throw new JsonSerializationException("Invalid format for CustomClass. Expected 'n_str'.");

            ReadCount++;
            return new CustomClass
            {
                N = n,
                Str = parts[1]
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(CustomClass));
        }
    }
        
    [Test]
    public void Test()
    {
        var codec = new JsonCodec2();
        var test = CreateTestAction(codec);
        test(new CustomClass
        {
            Str = "hello",
            N = 123
        });
        
        test(null!);
        test(123);
        test("123");
        test(new[]{1, 2, 3});
        test(new[]{"1", "2", "3"});
    }

    [Test]
    public void TestCustomSerializer()
    {
        var converter = new CustomClassJsonConverter();
        var codec = new JsonCodec2().AddConverter(converter);
        var test = CreateTestAction(codec);
        test(new CustomClass
        {
            Str = "hello",
            N = 123
        });
        Assert.That(converter.WriteCount, Is.EqualTo(1));
        Assert.That(converter.ReadCount, Is.EqualTo(1));
    }

    private Action<object> CreateTestAction(ICodec2 codec)
    {
        return msg =>
        {
            using var stream = new MemoryStream();
            codec.Write(msg, stream);
            
            var str = Encoding.UTF8.GetString(stream.ToArray());
            Logger.LogInformation("Written json: {str}", str);
            
            stream.Position = 0;
            var result = codec.Read(stream, msg == null ? null : msg.GetType());
            Assert.That(result, Is.EqualTo(msg));
        };
    }
}
