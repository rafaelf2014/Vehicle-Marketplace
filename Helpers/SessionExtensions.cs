using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CliCarProject.Helpers
{
    public static class SessionExtensions
    {
        //Método para guardar o objeto em JSON Serielizado na sessão
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        //Método para obter um objeto da sessão
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
