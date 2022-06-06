using AvaloniaCallCenter.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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

        private const string host = "https://0106-62-217-190-128.ngrok.io";

        private static HubConnection connection;
        private static string _token;
        private static User _user;

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


        public static List<Client> getAllClients()
        {
            var response = sendRequest("", $"{host}/api/data/GetAllClients", Method.POST);

            var clients = JsonConvert.DeserializeObject<List<Client>>(response);

            return clients;
        }
         
        public static bool addClient(Client client)
        {
            try
            {
                string json = JsonConvert.SerializeObject(client);
                var response = sendRequest(json, $"{host}/api/data/CreateClient",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        public static bool updateClient(Client client)
        {
            try
            {
                string json = JsonConvert.SerializeObject(client);
                var response = sendRequest(json, $"{host}/api/data/UpdateClient",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
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
                GetConnection();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        public static bool addEvent(int idClient, Event @event)
        {
            try
            {
                string json = JsonConvert.SerializeObject(@event);
                var response = sendRequest(json,
                    $"{host}/api/data/AddEvent?idClient={idClient}",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static bool sendRegistration(User model)
        {
            try
            {
                string json = JsonConvert.SerializeObject(model);
                var response = sendRequest(json, $"{host}/api/account/Registration",
                    Method.POST);
                JObject obj = JObject.Parse(response);
                _token = (string)obj["access_token"];
                GetConnection();
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


            var connectionList = JsonConvert.DeserializeObject<List<Connection>>(response);

            return connectionList;
        }

        public static User getUser()
        {
            if(_user == null && _token != null)
            {
                var response = sendRequest("", $"{host}/api/account/GetRegistrationModel",
                   Method.POST);

                _user = JsonConvert.DeserializeObject<User>(response);
            }
            return _user;
        }

        public static void setUserAndConnectionNull()
        {
            _user = null;
            connection.StopAsync();
            connection = null;
        }
    }
}
