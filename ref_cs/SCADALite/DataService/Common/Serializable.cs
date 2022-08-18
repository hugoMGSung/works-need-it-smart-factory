using Newtonsoft.Json;
using System;

namespace DataService
{
    /// <summary>
    /// 직렬화 처리
    /// </summary>
    public static class Serializable
    {
        #region JSON

        /// <summary>
        /// JSON 문자열로 객체 직렬화
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJsonString(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.None);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// JSON 문자열을 객체로 역직렬화
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonStringToObject<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return default; // default(T);
            }
        }

        #endregion
    }
}
