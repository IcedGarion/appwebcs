using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace School
{
    public static class Extensions
    {
        public static SelectList ToSelectList<T>(this DbSet<T> entities, string dataValueField, string dataTextField)
            where T : class =>
            new SelectList(entities.ToList(), dataValueField, dataTextField);

        public static string ToJson(this SelectList selectList)
        {
            var q =
                from i in selectList.Cast<SelectListItem>()
                select new
                {
                    value = i.Value,
                    text = i.Text
                };

            return JsonConvert.SerializeObject(q);
        }


        //serializza e deserializza oggetti in array di byte per salvarli in session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
