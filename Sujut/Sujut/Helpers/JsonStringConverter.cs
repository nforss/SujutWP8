using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sujut.Helpers
{
    public class JsonStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is decimal)
            {
                var deci = (decimal) value;

                serializer.Serialize(writer, deci.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                serializer.Serialize(writer, value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Json string converter not intended for reading!");
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof (decimal).IsAssignableFrom(objectType) ||
                    typeof (long).IsAssignableFrom(objectType));
        }
    }
}
