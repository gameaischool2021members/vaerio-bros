using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assets.DataLayer.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.DataLayer
{
    public class GenericRequester : IGenericRequester
    {
        static readonly HttpClient client = new HttpClient();
        public GenericRequester(string host)
        {
            Endpoint = host;
        }

        public string Endpoint { get; }

        public Tout GetObject<Tin, Tmid, Tout>(Tin obj)
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<Tout> GetObjects<Tin, Tmid, Tout>(Tin obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetObject<Tout>(string path, Action<Tout> responseHandler = null)
        {

            // parse response
            using (UnityWebRequest www = UnityWebRequest.Get($"{Endpoint}/{path}"))
            {
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                    Debug.Log(www.error);
                else
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));
            }

        }

        public IEnumerator PostObject<Tin, Tout>(Tin bodyObj, string path, Action<Tout> responseHandler = null)
        {
            var json = JsonConvert.SerializeObject(bodyObj);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            // parse response
            using (UnityWebRequest www = UnityWebRequest.Post($"{Endpoint}/{path}", json))
            {
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    //Debug.Log("Form upload complete!");

                    //Debug.Log("POST successful!");
                    //StringBuilder sb = new StringBuilder();
                    //foreach (System.Collections.Generic.KeyValuePair<string, string> d in www.GetResponseHeaders())
                    //{
                    //    sb.Append(d.Key).Append(": \t[").Append(d.Value).Append("]\n");
                    //}

                    //// Print Headers
                    //Debug.Log(sb.ToString());

                    //// Print Body
                    //Debug.Log(www.downloadHandler.text);
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));

                }
            }

        }

        public Tout PostObjects<Tin, Tmid, Tout>(IEnumerable<Tin> obj)
        {
            throw new NotImplementedException();
        }
    }
}
