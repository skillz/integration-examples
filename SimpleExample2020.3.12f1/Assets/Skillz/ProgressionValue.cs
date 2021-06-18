using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using SkillzSDK.Settings;
using SkillzSDK.Extensions;

using JSONDict = System.Collections.Generic.Dictionary<string, object>;
using System.Linq;


namespace SkillzSDK
{
    /// <summary>
    /// A piece of progression data maintained by Skillz.
    /// </summary>
    public class ProgressionValue
    {
        /// <summary>
        /// The stored value for this progresison value.
        /// </summary>
        public readonly string Value;
        
        /// <summary>
        /// The timestamp of when the data was last updated on Skillz servers in UTC.
        /// </summary>
        public readonly DateTime LastUpdatedTime;
        
        /// <summary>
        /// The data type for the stored value.
        /// </summary>
        public readonly string DataType;

        public ProgressionValue(string value, string dataType, string updateTime)
        {
            Value = value;
            DataType = dataType;
            DateTime localTime;
            if (DateTime.TryParse(updateTime, out localTime))
            {
                LastUpdatedTime = localTime.ToUniversalTime();
            }
            else
            {
                Debug.Log("ProgressionValue: DateTime failed to parse from empty string.");
            }
        }

        public string ToString()
        {
            return String.Format("ProgressionValue: {0}, {1}, {2}", Value, DataType, LastUpdatedTime.ToString("s"));
        }

        /// <summary>
        /// Returns a Dictionary of ProgressionValues parsed from the given json string
        /// </summary>
        public static Dictionary<string, ProgressionValue> GetProgressionValuesFromJSON(JSONDict jsonDict)
        {
            Dictionary<string, ProgressionValue> data = new Dictionary<string, ProgressionValue>();

            foreach (string key in jsonDict.Keys) {
                Dictionary<string, object> keyValuePairs = (Dictionary<string, object>)jsonDict.SafeGetValue(key);
                string dataValue = keyValuePairs.SafeGetStringValue("developer_value");
                string valueType = keyValuePairs.SafeGetStringValue("developer_value_type");
                string updateTime = keyValuePairs.SafeGetStringValue("date_updated");

                data.Add(key, new ProgressionValue(dataValue, valueType, updateTime));
                Debug.Log("ProgressionValue created: " + data[key].ToString());
            }

            return data;
        }
    }
}