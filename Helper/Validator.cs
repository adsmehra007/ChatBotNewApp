using ChatBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace ChatBotApplication.Helper
{
    public class Validator
    {
        /// <summary>
        /// Check date is in MM/DD/YYYY format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool checkDateFormat(string date)
        {

            try
            {
                DateTime Test;
                if (DateTime.TryParseExact(date, "MM/dd/yyyy", null, DateTimeStyles.None, out Test) == true)
                    return true;

            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public static string checkDayGreeting()
        {
            try
            {
                //   var systemTime = DateTime.Now.TimeOfDay;
                var systemTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).TimeOfDay;
                if (systemTime.Hours < 12)
                {
                    return "Good Morning";
                }
                else if (systemTime.Hours < 17||systemTime.Hours==12)
                {
                    return "Good Afternoon";
                }
                else
                {
                    return "Good Evening";
                }
            }
            catch (Exception ex)
            {
                return "Error";

            }
        }
        public static string getAgentName()

        {
            string randomAgentName = string.Empty;
            try
            {
                Random random = new Random();
                List<string> agentNames = new List<string>();
                string[] lstNames = ConfigurationManager.AppSettings["ChatBot.Agent.Names"].Split(':');
                agentNames.AddRange(lstNames);
                int namePos = random.Next(agentNames.Count);
                randomAgentName = (string)lstNames[namePos];
            }
            catch (Exception ex)
            {


            }
            return randomAgentName;
        }
        public static bool sentenceComparison(string s1, string s2)
        {
            bool stringEquals = false;
            try
            {
                string normalized1 = Regex.Replace(s1, @"\s", "");
                string normalized2 = Regex.Replace(s2, @"\s", "");
                normalized1 = Regex.Replace(normalized1, @"[^0-9a-zA-Z]+", "");
                normalized2 = Regex.Replace(normalized2, @"[^0-9a-zA-Z]+", "");
                //Regex.Replace(Your String, @"[^0-9a-zA-Z:,]+", "") if you do not want to remove : and , use this and add as many you want
                stringEquals = String.Equals(
                    normalized1,
                    normalized2,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {

            }
            return stringEquals;
        }
        public static bool isPhoneNumberValid(string phoneNumber)
        {
            bool isValid = false;
            try
            {
                const string matchPattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
                Regex regx = new Regex(matchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Match match = regx.Match(phoneNumber);
                if (match.Success)
                {
                    isValid = true;
                }
            }
            catch (Exception ex)
            {


            }
            return isValid;
        }
        public static string getOptions()
        {
            int index = 1;
            string options = string.Empty;
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                try
                {
                    if (key.Contains("Options."))
                    {
                        string _value = ConfigurationManager.AppSettings[key].ToString();
                        options = string.Concat(options, index + ". " + _value + System.Environment.NewLine);
                        index++;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return options;
        }
        public static int getOptionSelected(string option)
        {
            int optionSequence = 1;
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                {
                    try
                    {
                        if (key.Contains("Options."))
                        {
                            string _value = ConfigurationManager.AppSettings[key].ToString();
                            if (sentenceComparison(_value.Trim(), option.Trim()))
                            {
                                return optionSequence;
                            }
                            optionSequence++;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {


            }
            return 0;
        }
        public static BOTResponse BotAPICall(string methodName, string matchParameter, int sequence, string requestToken, List<Patient> patList)
        {
            BOTResponse bR = new BOTResponse();
            try
            {
                var jsonPatList = JsonConvert.SerializeObject(patList);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                    var responseTask = client.GetAsync(methodName.Trim() + "?matchParameter=" + matchParameter + "&sequence=" + sequence + "&requestToken=" + requestToken + "&patList=" + jsonPatList);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<BOTResponse>();
                        readTask.Wait();
                        bR= readTask.Result;
                        
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return bR;
        }
    }
}