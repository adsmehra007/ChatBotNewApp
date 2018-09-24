using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using Microsoft.Bot.Connector;
using System.Reflection;
using ChatBot.Models;
using System.Collections.Generic;
using ChatBot.Business;
using System.Web.Http;
using System.Net.Http;
using System;
using System.Configuration;

namespace ChatBotApplication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static List<Patient> getPatData;
        public static List<Patient> getPatName;
        public static List<Patient> getPatFirstName;
        public static List<Patient> getPatLastName;
        public static List<Patient> getPatDOB;
        public static List<Patient> getPatSSN;
        public static List<Patient> getPatZIP;
        public static List<Patient> getPatPhone;
        public static string firstName = "Dear";
        public static string initState = string.Empty;


        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                    var responseTask = client.GetAsync("getallpat");
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Patient>>();
                        readTask.Wait();
                        getPatData = readTask.Result;
                    }
                }

            }
            catch (Exception ex)
            {


            }
            // getPatData = PatientSearch.GetAllPat();



            Conversation.UpdateContainer(
            builder =>
            {
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: [https://github.com/Microsoft/BotBuilder-Azure](https://github.com/Microsoft/BotBuilder-Azure)
                var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });
        }
    }
}
