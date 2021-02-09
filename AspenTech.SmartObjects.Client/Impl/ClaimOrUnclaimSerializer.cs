using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models;
using Newtonsoft.Json;

namespace AspenTech.SmartObjects.Client.Impl
{
    /// <summary>
    /// Json Serilize or deserialize ClaimOrUnclaim instances
    /// </summary>
    public class ClaimOrUnclaimSerializer
    {
        private const string DeviceIdProperty = "x_device_id";
        private const string UsernameProperty = "username";

        /// <summary>
        /// Serialize a list of ClaimOrUnclaim to a Json string
        /// </summary>
        /// <param name="values">List of claims or unclaims</param>
        /// <returns>json string</returns>
        public static string SerializeClaimOrUnclaims(IEnumerable<ClaimOrUnclaim> values)
        {
            List<string> stringBuilder = new List<string>();
            foreach (ClaimOrUnclaim value in values)
            {
                stringBuilder.Add(SerializeClaimOrUnclaim(value));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        /// <summary>
        /// serialize a ClaimOrUnclaim instance to a JSON string
        /// </summary>
        /// <param name="claimOrUnclaim">ClaimOrUnclaim instance</param>
        /// <returns>json string</returns>
        public static string SerializeClaimOrUnclaim(ClaimOrUnclaim claimOrUnclaim)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();

            body.Add(UsernameProperty, claimOrUnclaim.Username);
            body.Add(DeviceIdProperty, claimOrUnclaim.DeviceId);

            foreach (KeyValuePair<string, object> attribute in claimOrUnclaim.Attributes)
            {
                body.Add(attribute.Key, attribute.Value);
            }

            return JsonConvert.SerializeObject(
                body,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = EventSerializer.DatetimeFormat
                });
        }
    }
}