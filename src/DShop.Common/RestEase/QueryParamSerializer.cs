using RestEase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DShop.Common.RestEase
{
    public class QueryParamSerializer : RequestQueryParamSerializer
    {
        public override IEnumerable<KeyValuePair<string, string>> SerializeQueryParam<T>(string name, T value, RequestQueryParamSerializerInfo info)
            => Serialize(name, value, info);

        public override IEnumerable<KeyValuePair<string, string>> SerializeQueryCollectionParam<T>(string name, IEnumerable<T> values, RequestQueryParamSerializerInfo info)
            => Serialize(name, values, info);

        private IEnumerable<KeyValuePair<string, string>> Serialize<T>(string name, T value, RequestQueryParamSerializerInfo info)
        {
            if (value == null)
                yield break;

            foreach (var prop in GetPropertiesDeepRecursive(value, name))
            {
                if (prop.Value == null)
                {
                    yield return new KeyValuePair<string, string>(prop.Key, string.Empty);
                }
                else if (prop.Value is DateTime dt)
                {
                    yield return new KeyValuePair<string, string>(prop.Key, dt.ToString(info.Format ?? "o"));
                }
                else
                {
                    yield return new KeyValuePair<string, string>(prop.Key, prop.Value.ToString());
                }
            }
        }

        private Dictionary<string, object> GetPropertiesDeepRecursive(object obj, string name)
        {
            var dict = new Dictionary<string, object>();

            if (obj == null)
            {
                dict.Add(name, null);
                return dict;
            }

            if (obj.GetType().IsValueType || obj is string)
            {
                dict.Add(name, obj);
                return dict;
            }

            if (obj is IEnumerable collection)
            {
                int i = 0;
                foreach (var item in collection)
                {
                    dict = dict.Concat(GetPropertiesDeepRecursive(item, $"{name}[{i++}]")).ToDictionary(e => e.Key, e => e.Value);
                }
                return dict;
            }

            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                dict = dict
                    .Concat(GetPropertiesDeepRecursive(prop.GetValue(obj, null), $"{name}.{prop.Name}"))
                    .ToDictionary(e => e.Key, e => e.Value);
            }
            return dict;
        }
    }
}
