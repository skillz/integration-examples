using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SkillzSDK.Internal.API.Android
{
    class ProgressionCallback : AndroidJavaProxy
    {
        Action<Dictionary<string, ProgressionValue>> successCallback;
        Action<string> failureCallback;
        public ProgressionCallback(Action<Dictionary<string, ProgressionValue>> success, Action<string> failure)
            : base("com.skillz.progression.ProgressionCallback")
        {
            successCallback = success;
            failureCallback = failure;
        }

        void success(AndroidJavaObject dataObj)
        {
            if (dataObj == null)
            {
                if (successCallback != null)
                {
                    successCallback(new Dictionary<string, ProgressionValue>());
                }
                return;
            }
            // The expected data is of type HashMap<string, ProgressionValue>
            // Because C# doesn't know about Java classes, everything has to be done with AndroidJavaObjects and the Call function
            // which means we have to know the data types and methods

            // Treat the element like a HashMap, get the keys, for each key get the value, for each value get all the fields and make a ProgressionValue
            AndroidJavaObject mapKeys = dataObj.Call<AndroidJavaObject>("keySet");
            AndroidJavaObject keysList = new AndroidJavaObject("java.util.ArrayList");
            keysList.Call<bool>("addAll", mapKeys);

            // Prepare objects needed to convert the Java Date object into a compativale ISO 8601 string
            AndroidJavaObject dateFormatObj = new AndroidJavaObject("java.text.SimpleDateFormat", "yyyy-MM-dd'T'HH:mm:ss.SSSZ");

            Dictionary<string, ProgressionValue> data = new Dictionary<string, ProgressionValue>();
            int keysCount = keysList.Call<int>("size");
            for (int i = 0; i < keysCount; i++)
            {
                string key = keysList.Call<string>("get", i);
                
                AndroidJavaObject progValueObj = dataObj.Call<AndroidJavaObject>("get", key);
                string developerValue = progValueObj.Get<string>("value");
                string developerValueType = progValueObj.Get<string>("valueType");
                string displayName = progValueObj.Get<string>("displayName");

                // ProgressionValue in the java side uses a Date object, so we have to get the string in the ISO 8601 format ("yyyy-MM-dd'T'HH:mm:ss.SSSZ")
                // using java.text.DateFormat. ProgressionValue can then convert into a DateTime object from the string
                AndroidJavaObject dateObj = progValueObj.Get<AndroidJavaObject>("dateUpdated");
                string dateUpdated = dateFormatObj.Call<string>("format", dateObj);
                
                // The metadata is another Java object that we must iterate through
                List<string> gameIds = new List<string>();
                AndroidJavaObject progMetaObject = progValueObj.Get<AndroidJavaObject>("meta");
                AndroidJavaObject gameIdsObj = progMetaObject.Get<AndroidJavaObject>("gameIds");
                int gameIdsCount = gameIdsObj.Call<int>("size");
                for (int j = 0; j < gameIdsCount; j++)
                {
                    string gameId = gameIdsObj.Call<string>("get", j);
                    gameIds.Add(gameId);
                }

                ProgressionMetadata metadata = new ProgressionMetadata(gameIds);

                ProgressionValue progValue = new ProgressionValue(developerValue, developerValueType, dateUpdated, displayName, metadata);
                data.Add(key, progValue);
            }

            if (successCallback != null)
            {
                successCallback(data);
            }
        }

        void failure(AndroidJavaObject errorObj)
        {
            // errorObject is of type "Exception"
            if (failureCallback != null)
            {
                string errorMessage = errorObj.Call<string>("getMessage");
                failureCallback(errorMessage);
            }
        }
    }
}
