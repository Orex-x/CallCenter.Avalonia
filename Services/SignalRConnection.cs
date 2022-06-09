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

        private const string host = "https://a160-62-217-190-128.ngrok.io";

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
                   options.Headers.Add("Hostname", Environment.MachineName);
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



        public static async Task<string> sendRequestAsync(string body, string uri, string method)
        {
            try
            {
                var request = WebRequest.Create(uri);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = method;
                if (_token != null)
                    request.Headers.Add("Authorization", _token);

                if (body.Length > 0)
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(body);
                        streamWriter.Flush();
                    }
                }

                var response = (HttpWebResponse)await Task.Factory
                    .FromAsync<WebResponse>(request.BeginGetResponse,
                                            request.EndGetResponse,
                                            null);

                using (var streamReader = new StreamReader(response.GetResponseStream()))
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

        public static  string sendRequest(string body, string uri, string method)
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


        public async static Task<List<Client>> getAllClientsAsync()
        {
            var response = await sendRequestAsync("", $"{host}/api/data/GetAllClients", Method.POST);

            var clients = JsonConvert.DeserializeObject<List<Client>>(response);

            return clients;
        }
         
        public async static Task<bool> addClientAsync(Client client)
        {
            try
            {
                string json = JsonConvert.SerializeObject(client);
                var response = await sendRequestAsync(json, $"{host}/api/data/CreateClient",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        public async static Task<bool> updateClientAsync(Client client)
        {
            try
            {
                string json = JsonConvert.SerializeObject(client);
                var response = await sendRequestAsync(json, $"{host}/api/data/UpdateClient",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async static Task<bool> updateUserCountFieldsAsync(User user)
        {
            try
            {
                string json = JsonConvert.SerializeObject(user);
                var response = await sendRequestAsync(json, $"{host}/api/data/UpdateUserCountFields",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        public async static Task<bool> deleteClientAsync(int idClient)
        {
            try
            {
                var response = await sendRequestAsync("",
                    $"{host}/api/data/DeleteClient?id={idClient}",
                    Method.POST);
              
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        public async static Task<bool> deleteEventAsync(int idClient, int idEvent)
        {
            try
            {
                var response = await sendRequestAsync("",
                    $"{host}/api/data/DeleteEvent?idClient={idClient}&idEvent={idEvent}",
                    Method.POST);

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async static Task<bool> authServerAsync(string login, string password)
        {
            try
            {
                var response = await sendRequestAsync("", 
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


        public async static Task<bool> addEventAsync(int idClient, Event @event)
        {
            try
            {
                string json = JsonConvert.SerializeObject(@event);
                var response = await sendRequestAsync(json,
                    $"{host}/api/data/AddEvent?idClient={idClient}",
                    Method.POST);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async static Task<bool> sendRegistrationAsync(User model)
        {
            try
            {
                string json = JsonConvert.SerializeObject(model);
                var response = await sendRequestAsync(json, $"{host}/api/account/Registration",
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

        public async static Task<List<Connection>> getAllConnectionAsync()
        {
           
            var response = await sendRequestAsync("", $"{host}/api/account/GetAllConnection",
                    Method.POST);

            var connectionList = JsonConvert.DeserializeObject<List<Connection>>(response);

            return connectionList;
        }

        public async static Task<User> getUserAsync()
        {
            if(_user == null && _token != null)
            {
                var response = await sendRequestAsync("", $"{host}/api/account/GetRegistrationModel",
                   Method.POST);

                _user = JsonConvert.DeserializeObject<User>(response);
            }
            return _user;
        }

        public async static Task<bool> UpdateUser(User user)
        {
            if (user != null)
            {
                string json = JsonConvert.SerializeObject(user);

                var response = await sendRequestAsync(json, $"{host}/api/data/UpdateUser",
                   Method.POST);
                return JsonConvert.DeserializeObject<bool>(response);

            }
            return false;
        }

        public static void setUserAndConnectionNull()
        {
            if(_user != null)
                _user = null;
            if (connection != null)
            {
                connection.StopAsync();
                connection = null;
            }
        }
    }
}
