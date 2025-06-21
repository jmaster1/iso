using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IsoNetTest.Iso.Serialize;

public class TestBean
{
    public string Property;
}

public class TestBeanConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var bean = (TestBean) value!;
        writer.WriteStartObject();
        writer.WritePropertyName("val");
        writer.WriteValue(bean.Property);
        writer.WriteEndObject();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TestBean);
    }
}

public class JsonPopTest
{
    [Test]
    public void Test()
    {
            var settings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>
                {
                    new TestBeanConverter(),
                },
                Formatting = Formatting.Indented, 
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            var serializer = JsonSerializer.CreateDefault(settings);
            var bean = new TestBean
            {
                Property = "?"
            };
            var w = new StringWriter();
            serializer.Serialize(w, bean);
            var bean2 = new TestBean();
            serializer.Populate(new StringReader(w.ToString()), bean2);
            Assert.AreEqual(bean.Property, bean2.Property); 
    }
}

