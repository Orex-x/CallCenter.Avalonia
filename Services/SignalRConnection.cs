using AvaloniaCallCenter.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaCallCenter.Services
{
    public class SignalRConnection
    {
        public struct Method
        {
            public static string POST = "POST";
            public static string GET = "GET";
            public static string DELETE = "DELETE";
            public static string PUT = "PUT";
        }

        private const string host = "https://948f-62-217-190-128.ngrok.io";

        private static HubConnection connection;
        private static string _token;

        public static object MessageBox { get; private set; }

        public static string sendRequest(string body, string uri, string method)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = method;
                
                if (_token != null)
                    httpWebRequest.Headers.Add("Authorization", _token);

                if (body.Length > 0)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(body);
                        streamWriter.Flush();
                    }
                }


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

               

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }

        public static bool authServer(string login, string password)
        {
            try
            {
                var response = sendRequest("",
                    $"{host}/api/account/token?login={login}&password={password}",
                    Method.POST);
                JObject obj = JObject.Parse(response);
                _token = (string)obj["access_token"];
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static bool sendRegistration(RegistrationModel model)
        {
            try
            {
                string json = JsonSerializer.Serialize(model);
                var response = sendRequest(json, $"{host}/api/account/Registration",
                    Method.POST);
                JObject obj = JObject.Parse(response);
                _token = (string)obj["access_token"];
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static List<Connection> getAllConnection()
        {
           
            var response = sendRequest("", $"{host}/api/account/GetAllConnection",
                    Method.POST);

            var connectionList = JsonSerializer.Deserialize<List<Connection>>(response);

            return connectionList;
        }

        public static HubConnection GetConnection()
        {
            if (connection == null)
            {
                connection = new HubConnectionBuilder()
               .WithUrl($"{host}/ChatHub", options =>
               {
                   options.AccessTokenProvider = () => Task.FromResult(_token);
               })
               .WithAutomaticReconnect()
               .Build();



                connection.Closed += error =>
                {
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                   // MessageBox.Show("connection has been closed or manually try to restart the connection");
                    // Notify users the connection has been closed or manually try to restart the connection.
                    return Task.CompletedTask;
                };

                connection.Reconnecting += error =>
                {
                    Debug.Assert(connection.State == HubConnectionState.Reconnecting);
                   // MessageBox.Show("connection was lost");
                    // Notify users the connection was lost and the client is reconnecting.
                    // Start queuing or dropping messages.

                    return Task.CompletedTask;
                };

                connection.Reconnected += connectionId =>
                {
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    //MessageBox.Show("connection was reestablished");
                    // Notify users the connection was reestablished.
                    // Start dequeuing messages queued while reconnecting if any.

                    return Task.CompletedTask;
                };
            }
            return connection;
        }
    }
}
