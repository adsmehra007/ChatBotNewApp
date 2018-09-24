using ChatBot.Data;
using ChatBot.Helper;
using ChatBot.Models;
using ChatBotApplication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ChatBot.Business
{
    public class PatientSearch
    {
        DataAccess dA = new DataAccess();

        internal static List<Patient> GetAllPat()
        {
            List<Patient> patList = new List<Patient>();
            try
            {
                DataAccess dA = new DataAccess();
                var query = QueryBuilder.SearchPatientName();
                var queryResult = dA.executeDB2Statement(query);
                if (queryResult.Tables.Count > 0 && queryResult.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow pat in queryResult.Tables[0].Rows)
                    {
                        try
                        {
                            Patient patient = new Patient();
                            patient.Name = pat["PANAME"].ToString().ToLower();
                            try
                            {
                                patient.FirstName = pat["PANAME"].ToString().Split(',')[1];
                                patient.LastName = pat["PANAME"].ToString().Split(',')[0];
                            }
                            catch (Exception ex)
                            {
                            }
                            patient.ID = Convert.ToInt64(pat["PACLT#"].ToString().Trim() + pat["PAACT#"].ToString().Trim() + pat["PAPATID"].ToString().Trim());
                            //patient.ID = Convert.ToInt64(pat["PAPATID"]);
                            patient.birthDate = pat["PABRTHDT"].ToString();
                            patient.birthCC = pat["PABIRTHCC"].ToString();
                            patient.SSN = pat["PASSN"].ToString();
                            patient.ZipCode = pat["PAZIP"].ToString();
                            patient.PatWorkPhone = pat["PAWPHONE"].ToString();
                            patient.PatHomePhone = pat["PAHPHONE"].ToString();
                            patList.Add(patient);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return patList;
        }
        internal bool MatchFirstName(int sequence, string input, string requestToken, List<Patient> patList)
        {
            bool status = false;
            try
            {
                int queryStatusPSLQ = 0;
                List<Patient> matchedSearches = new List<Patient>();
                matchedSearches = FuzzySearch.Search(input.ToLower(), patList, 0.29);
                if (matchedSearches.Count > 0)
                {
                    var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                            var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<int>();
                                readTask.Wait();
                                queryStatusPSLQ = readTask.Result;
                            }
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                    // queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                    if (queryStatusPSLQ == 1)
                    {
                        var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                        //var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        var patientSearchLogIDResult = string.Empty;
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                    readTask.Wait();
                                    patientSearchLogIDResult = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString(); ;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                        //foreach (DataRow patient in queryResult.Tables[0].Rows)
                        foreach (var patient in matchedSearches)
                        {
                            try
                            {
                                //var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient["PAPATID"].ToString());
                                var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient.ID.ToString());
                                try
                                {
                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                        var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                        responseTask.Wait();
                                        var result = responseTask.Result;
                                        if (result.IsSuccessStatusCode)
                                        {
                                            var readTask = result.Content.ReadAsAsync<int>();
                                            readTask.Wait();
                                            int s = readTask.Result;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {


                                }
                                // dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    WebApiApplication.getPatFirstName = matchedSearches;
                    status = true;
                }
                else
                {
                    var sqlQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                            var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + sqlQuery);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<int>();
                                readTask.Wait();
                                int queryStatus = readTask.Result;
                            }
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                    //int queryStatus = dA.executeInsertSqlStatement(sqlQuery);
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        internal bool MatchLastName(int sequence, string input, string requestToken, List<Patient> patList)
        {
            int queryStatusPSLQ = 0;
            bool status = false;
            try
            {
                List<Patient> matchedSearches = new List<Patient>();
                matchedSearches = FuzzySearch.LNSearch(input.ToLower(), patList, 0.29);
                if (matchedSearches.Count > 0)
                {
                    var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                    //int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                            var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<int>();
                                readTask.Wait();
                                 queryStatusPSLQ = readTask.Result;
                            }
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                    if (queryStatusPSLQ == 1)
                    {
                        var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                        var patientSearchLogIDResult = string.Empty;
                        // patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                    readTask.Wait();
                                    patientSearchLogIDResult = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString(); ;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                        //foreach (DataRow patient in queryResult.Tables[0].Rows)
                        foreach (var patient in matchedSearches)
                        {
                            try
                            {
                                //var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient["PAPATID"].ToString());
                                var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient.ID.ToString());
                             //   dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
                                try
                                {
                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                        var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                        responseTask.Wait();
                                        var result = responseTask.Result;
                                        if (result.IsSuccessStatusCode)
                                        {
                                            var readTask = result.Content.ReadAsAsync<int>();
                                            readTask.Wait();
                                            int queryStatus = readTask.Result;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {


                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    WebApiApplication.getPatLastName = matchedSearches;
                    status = true;
                }
                else
                {
                    var sqlQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                    //int queryStatus = dA.executeInsertSqlStatement(sqlQuery);
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                            var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + sqlQuery);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<int>();
                                readTask.Wait();
                                int queryStatus = readTask.Result;
                            }
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
            return status;
        }

        internal bool MatchDOB(int sequence, string input, string requestToken, List<Patient> patList)
        {
            bool status = false;
            DataSet patientSearchLogIDSet = new DataSet();
            try
            {
                List<Patient> matchedSearches = new List<Patient>();
                //var dateOFBirth = ConversionHelper.ToJulianDate(Convert.ToDateTime(input));
                if (input != null)
                {
                    var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);
                  //  var patientSearchLogIDSet = dA.executeSqlStatement(patientSearchLogID);
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                            var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<DataSet>();
                                readTask.Wait();
                                 patientSearchLogIDSet = readTask.Result;
                            }
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                    string logIDs = string.Empty;
                    if (patientSearchLogIDSet.Tables.Count > 0 && patientSearchLogIDSet.Tables[0].Rows.Count > 0)
                    {
                        DataSet preMatchedPats = new DataSet();
                        logIDs = patientSearchLogIDSet.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(logIDs);
                        // preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + getAllPatQuery);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                    readTask.Wait();
                                    preMatchedPats = readTask.Result;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                        if (preMatchedPats.Tables.Count > 0 && preMatchedPats.Tables[0].Rows.Count > 0)
                        {

                            //requestToken = Guid.NewGuid().ToString();
                            foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
                            {
                                try
                                {
                                    foreach (var pat in patList)
                                    {
                                        try
                                        {
                                            if (Convert.ToInt64(patient["PatientID"]) == pat.ID)
                                            {
                                                string dob = pat.birthDate;
                                                string century = pat.birthCC;
                                                if (dob != null && century != null)
                                                {
                                                    ConversionHelper cH = new ConversionHelper();
                                                    var patDateOfBirth = cH.julianTOCalender(dob, century);
                                                    if (Convert.ToDateTime(input).Date == patDateOfBirth.Date)
                                                    {
                                                        DataSet existingPatientSearchLogIDResult = new DataSet();
                                                        matchedSearches.Add(pat);
                                                        status = true;
                                                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                                       // var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
                                                        try
                                                        {
                                                            using (var client = new HttpClient())
                                                            {
                                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + existingPatientSearchLogID);
                                                                responseTask.Wait();
                                                                var result = responseTask.Result;
                                                                if (result.IsSuccessStatusCode)
                                                                {
                                                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                                                    readTask.Wait();
                                                                    existingPatientSearchLogIDResult = readTask.Result;
                                                                }
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {


                                                        }
                                                        string patSearchLogID = null;
                                                        if (existingPatientSearchLogIDResult.Tables.Count > 0 && existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
                                                        {
                                                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                                        }
                                                        else
                                                        {
                                                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                                                           // int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                                                            try
                                                            {
                                                                using (var client = new HttpClient())
                                                                {
                                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                                    var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                                                    responseTask.Wait();
                                                                    var result = responseTask.Result;
                                                                    if (result.IsSuccessStatusCode)
                                                                    {
                                                                        var readTask = result.Content.ReadAsAsync<int>();
                                                                        readTask.Wait();
                                                                        int queryStatus = readTask.Result;
                                                                    }
                                                                }

                                                            }
                                                            catch (Exception ex)
                                                            {


                                                            }
                                                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                                            try
                                                            {
                                                                using (var client = new HttpClient())
                                                                {
                                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                                    var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + newPatientSearchLogID);
                                                                    responseTask.Wait();
                                                                    var result = responseTask.Result;
                                                                    if (result.IsSuccessStatusCode)
                                                                    {
                                                                        var readTask = result.Content.ReadAsAsync<DataSet>();
                                                                        readTask.Wait();
                                                                        patSearchLogID = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString(); ;
                                                                    }
                                                                }

                                                            }
                                                            catch (Exception ex)
                                                            {


                                                            }
                                                            //patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                                        }
                                                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
                                                        try
                                                        {
                                                            using (var client = new HttpClient())
                                                            {
                                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                                                responseTask.Wait();
                                                                var result = responseTask.Result;
                                                                if (result.IsSuccessStatusCode)
                                                                {
                                                                    var readTask = result.Content.ReadAsAsync<int>();
                                                                    readTask.Wait();
                                                                    int querystatus = readTask.Result ;
                                                                }
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {


                                                        }
                                                     //   dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }
                                }

                                catch (Exception ex)
                                {

                                }
                            }
                            WebApiApplication.getPatDOB = matchedSearches;
                        }
                        if (status == false)
                        {
                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                            //int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                    var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                    responseTask.Wait();
                                    var result = responseTask.Result;
                                    if (result.IsSuccessStatusCode)
                                    {
                                        var readTask = result.Content.ReadAsAsync<int>();
                                        readTask.Wait();
                                        int querystatus = readTask.Result;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return status;
        }
        internal bool MatchZip(int sequence, string input, string requestToken, List<Patient> patList)
        {
            bool status = false;

            try
            {
                string patientSearchLogIDResult = string.Empty;
                //var query = QueryBuilder.SearchZIP(input,"");
                //var queryResult = dA.executeDB2Statement(query);
                List<Patient> matchedSearches = new List<Patient>();
                var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);

                //var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            patientSearchLogIDResult = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(patientSearchLogIDResult);
                //var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
                DataSet preMatchedPats = new DataSet();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + getAllPatQuery);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            preMatchedPats = readTask.Result;
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                //requestToken = Guid.NewGuid().ToString();
                foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
                {
                    try
                    {
                        foreach (var pat in patList)
                        {
                            try
                            {
                                if (Convert.ToInt64(patient["PatientID"]) == pat.ID)
                                {
                                    if (pat.ZipCode == input)
                                    {
                                        matchedSearches.Add(pat);
                                        status = true;
                                        DataSet existingPatientSearchLogIDResult = new DataSet();
                                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                      // existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + existingPatientSearchLogID);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                                    readTask.Wait();
                                                    existingPatientSearchLogIDResult = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        string patSearchLogID = null;
                                        if (existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
                                        {
                                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                        }
                                        else
                                        {
                                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                                            try
                                            {
                                                using (var client = new HttpClient())
                                                {
                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                    var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                                    responseTask.Wait();
                                                    var result = responseTask.Result;
                                                    if (result.IsSuccessStatusCode)
                                                    {
                                                        var readTask = result.Content.ReadAsAsync<int>();
                                                        readTask.Wait();
                                                        int queryStatus = readTask.Result;
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                           // int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                            try
                                            {
                                                using (var client = new HttpClient())
                                                {
                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                    var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + newPatientSearchLogID);
                                                    responseTask.Wait();
                                                    var result = responseTask.Result;
                                                    if (result.IsSuccessStatusCode)
                                                    {
                                                        var readTask = result.Content.ReadAsAsync<DataSet>();
                                                        readTask.Wait();
                                                        patSearchLogID = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                          //  patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                        }
                                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<int>();
                                                    readTask.Wait();
                                                    int queryStatus = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                       // dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
                                    }
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
                    if (status == false)
                    {
                        var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<int>();
                                    readTask.Wait();
                                    int queryStatus = readTask.Result;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                      //  int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                    }
                }

            }
            catch (Exception)
            {

            }
            return status;

        }
        internal bool MatchSSN(int sequence, string input, string requestToken, List<Patient> patList)
        {
            bool status = false;

            try
            {
                //var query = QueryBuilder.SearchPatientSSN(input,"");
                //var queryResult = dA.executeDB2Statement(query);
                var patientSearchLogIDResult = string.Empty;
                List<Patient> matchedSearches = new List<Patient>();
                var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);

               // var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            patientSearchLogIDResult = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(patientSearchLogIDResult);
                var preMatchedPats = new DataSet();
               // var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + getAllPatQuery);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            preMatchedPats = readTask.Result;
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                //requestToken = Guid.NewGuid().ToString();
                foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
                {
                    try
                    {
                        foreach (var pat in patList)
                        {
                            try
                            {
                                if (Convert.ToInt64(patient["PatientID"]) == pat.ID)
                                {
                                    if (pat.SSN == input)
                                    {
                                        matchedSearches.Add(pat);
                                        status = true;
                                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                        //  var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
                                        var existingPatientSearchLogIDResult = new DataSet();
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + existingPatientSearchLogID);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                                    readTask.Wait();
                                                    existingPatientSearchLogIDResult = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        string patSearchLogID = null;
                                        if (existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
                                        {
                                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                        }
                                        else
                                        {
                                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                                           // int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                                            try
                                            {
                                                using (var client = new HttpClient())
                                                {
                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                    var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                                    responseTask.Wait();
                                                    var result = responseTask.Result;
                                                    if (result.IsSuccessStatusCode)
                                                    {
                                                        var readTask = result.Content.ReadAsAsync<int>();
                                                        readTask.Wait();
                                                        int queryStatus = readTask.Result;
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                            patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                        }
                                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
                                      //  dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<int>();
                                                    readTask.Wait();
                                                    int queryStatus = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        WebApiApplication.getPatSSN = matchedSearches;
                                    }
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
                    if (status == false)
                    {
                        var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                      //  int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<int>();
                                    readTask.Wait();
                                    int queryStatus = readTask.Result;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return status;
        }
        internal bool MatchPhone(int sequence, string input, string requestToken, List<Patient> patList)
        {
            bool status = false;

            try
            {
                //var query = QueryBuilder.SearchPatientSSN(input,"");
                //var queryResult = dA.executeDB2Statement(query);
                List<Patient> matchedSearches = new List<Patient>();
                var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 3);
                //var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                var patientSearchLogIDResult = string.Empty;
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + patientSearchLogID);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            patientSearchLogIDResult = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(patientSearchLogIDResult);
                //var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
                var preMatchedPats = new DataSet();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + getAllPatQuery);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            preMatchedPats = readTask.Result;
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                //requestToken = Guid.NewGuid().ToString();
                foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
                {
                    try
                    {
                        foreach (var pat in patList)
                        {
                            try
                            {
                                if (Convert.ToInt64(patient["PatientID"]) == pat.ID)
                                {
                                    if (pat.PatHomePhone == input || pat.PatWorkPhone == input)
                                    {
                                        matchedSearches.Add(pat);
                                        status = true;
                                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                        //var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
                                        var existingPatientSearchLogIDResult = new DataSet();
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + existingPatientSearchLogID);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<DataSet>();
                                                    readTask.Wait();
                                                    existingPatientSearchLogIDResult = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        string patSearchLogID = null;
                                        if (existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
                                        {
                                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                        }
                                        else
                                        {
                                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
                                           // int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                                            try
                                            {
                                                using (var client = new HttpClient())
                                                {
                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                    var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                                    responseTask.Wait();
                                                    var result = responseTask.Result;
                                                    if (result.IsSuccessStatusCode)
                                                    {
                                                        var readTask = result.Content.ReadAsAsync<int>();
                                                        readTask.Wait();
                                                        int queryStatus = readTask.Result;
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
                                           // patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                            try
                                            {
                                                using (var client = new HttpClient())
                                                {
                                                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                    var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + newPatientSearchLogID);
                                                    responseTask.Wait();
                                                    var result = responseTask.Result;
                                                    if (result.IsSuccessStatusCode)
                                                    {
                                                        var readTask = result.Content.ReadAsAsync<DataSet>();
                                                        readTask.Wait();
                                                        patSearchLogID = readTask.Result.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                        }
                                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
                                      //  dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogDetailQuery);
                                                responseTask.Wait();
                                                var result = responseTask.Result;
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    var readTask = result.Content.ReadAsAsync<int>();
                                                    readTask.Wait();
                                                    int queryStatus = readTask.Result;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        WebApiApplication.getPatPhone = matchedSearches;
                                    }
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
                    if (status == false)
                    {
                        var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
                      //  int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                                var responseTask = client.GetAsync("executeInsertSqlStatement?sqlstatement=" + insertPatientSearchLogQuery);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<int>();
                                    readTask.Wait();
                                    int queryStatus = readTask.Result;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }
            catch (Exception)
            {

            }
            return status;
        }
        #region Commented Code
        //internal bool MatchDOB(int sequence, string input, string requestToken)
        //{
        //    bool status = false;
        //    try
        //    {
        //        //var dateOFBirth = ConversionHelper.ToJulianDate(Convert.ToDateTime(input));
        //        if (input != null)
        //        {
        //            var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);
        //            var patientSearchLogIDSet = dA.executeSqlStatement(patientSearchLogID);
        //            string logIDs = string.Empty;
        //            if (patientSearchLogIDSet.Tables.Count > 0 && patientSearchLogIDSet.Tables[0].Rows.Count > 0)
        //            {
        //                logIDs = patientSearchLogIDSet.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(logIDs);
        //                var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
        //                if (preMatchedPats.Tables.Count > 0 && preMatchedPats.Tables[0].Rows.Count > 0)
        //                {
        //                    //requestToken = Guid.NewGuid().ToString();
        //                    foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
        //                    {
        //                        try
        //                        {
        //                            string getPatDOB = QueryBuilder.GetPatientByPatID(patient["PatientID"].ToString());
        //                            var PatDOB = dA.executeDB2Statement(getPatDOB);
        //                            if (PatDOB.Tables.Count > 0 && PatDOB.Tables[0].Rows.Count > 0)
        //                            {
        //                                string dob = PatDOB.Tables[0].Rows[0]["PABRTHDT"].ToString();
        //                                string century = PatDOB.Tables[0].Rows[0]["PABIRTHCC"].ToString();
        //                                if (dob != null && century != null)
        //                                {
        //                                    var patDateOfBirth = ConversionHelper.julianTOCalender(dob, century);
        //                                    if (Convert.ToDateTime(input).Date == patDateOfBirth.Date)
        //                                    {
        //                                        status = true;
        //                                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                                        var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
        //                                        string patSearchLogID = null;
        //                                        if (existingPatientSearchLogIDResult.Tables.Count > 0 && existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
        //                                        {
        //                                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                                        }
        //                                        else
        //                                        {
        //                                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
        //                                            int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //                                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                                            patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                                        }
        //                                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
        //                                        dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
        //                                    }
        //                                }
        //                            }

        //                        }

        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }
        //                }
        //                if (status == false)
        //                {
        //                    var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
        //                    int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return status;
        //}
        //internal bool MatchZip(int sequence, string input, string requestToken)
        //{
        //    bool status = false;

        //    try
        //    {
        //        //var query = QueryBuilder.SearchZIP(input,"");
        //        //var queryResult = dA.executeDB2Statement(query);

        //        var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);
        //        var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //        var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(patientSearchLogIDResult);
        //        var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
        //        //requestToken = Guid.NewGuid().ToString();
        //        foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
        //        {
        //            try
        //            {
        //                string getPatDOB = QueryBuilder.GetPatientByPatID(patient["PatientID"].ToString());
        //                var PatDOB = dA.executeDB2Statement(getPatDOB);
        //                if (PatDOB.Tables[0].Rows.Count > 0)
        //                {
        //                    if (PatDOB.Tables[0].Rows[0]["PAZIP"].ToString() == input)
        //                    {
        //                        status = true;
        //                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                        var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
        //                        string patSearchLogID = null;
        //                        if (existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
        //                        {
        //                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                        }
        //                        else
        //                        {
        //                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
        //                            int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                            patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                        }
        //                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
        //                        dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            if (status == false)
        //            {
        //                var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
        //                int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return status;

        //}

        //internal bool MatchSSN(int sequence, string input, string requestToken)
        //{
        //    bool status = false;

        //    try
        //    {
        //        //var query = QueryBuilder.SearchPatientSSN(input,"");
        //        //var queryResult = dA.executeDB2Statement(query);
        //        var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence - 1);
        //        var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //        var getAllPatQuery = QueryBuilder.GetPatientsBySearchLogID(patientSearchLogIDResult);
        //        var preMatchedPats = dA.executeSqlStatement(getAllPatQuery);
        //        //requestToken = Guid.NewGuid().ToString();
        //        foreach (DataRow patient in preMatchedPats.Tables[0].Rows)
        //        {
        //            try
        //            {
        //                string getPatDOB = QueryBuilder.GetPatientByPatID(patient["PatientID"].ToString());
        //                var PatDOB = dA.executeDB2Statement(getPatDOB);
        //                if (PatDOB.Tables[0].Rows.Count > 0)
        //                {
        //                    if (PatDOB.Tables[0].Rows[0]["PASSN"].ToString() == input)
        //                    {
        //                        status = true;
        //                        var existingPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                        var existingPatientSearchLogIDResult = dA.executeSqlStatement(existingPatientSearchLogID);
        //                        string patSearchLogID = null;
        //                        if (existingPatientSearchLogIDResult.Tables[0].Rows.Count > 0)
        //                        {
        //                            patSearchLogID = existingPatientSearchLogIDResult.Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                        }
        //                        else
        //                        {
        //                            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
        //                            int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //                            var newPatientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                            patSearchLogID = dA.executeSqlStatement(newPatientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                        }
        //                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patSearchLogID, patient["PatientID"].ToString());
        //                        dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            if (status == false)
        //            {
        //                var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
        //                int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return status;
        //}
        //internal bool MatchName(int sequence, string input, string requestToken)
        //{
        //    bool status = false;
        //    try
        //    {
        //        var query = QueryBuilder.SearchPatientName(input);
        //        var queryResult = dA.executeDB2Statement(query);
        //        List<Patient> matchedSearches = new List<Patient>();
        //        if (queryResult.Tables.Count > 0 && queryResult.Tables[0].Rows.Count > 0)
        //        {
        //            List<Patient> patList = new List<Patient>();
        //            foreach (DataRow pat in queryResult.Tables[0].Rows)
        //            {
        //                try
        //                {
        //                    Patient patient = new Patient();
        //                    patient.Name = pat["PANAME"].ToString().ToLower();
        //                    patient.ID = Convert.ToInt64(pat["PAPATID"]);
        //                    patList.Add(patient);
        //                }
        //                catch (Exception ex)
        //                {
        //                }
        //            }
        //            matchedSearches = FuzzySearch.Search(input.ToLower(), patList,0.29);
        //            var insertPatientSearchLogQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "1");
        //            int queryStatus = dA.executeInsertSqlStatement(insertPatientSearchLogQuery);
        //            if (queryStatus == 1)
        //            {
        //                var patientSearchLogID = QueryBuilder.GetPatientSearchLogID(requestToken, sequence);
        //                var patientSearchLogIDResult = dA.executeSqlStatement(patientSearchLogID).Tables[0].Rows[0]["PatientSearchLogID"].ToString();
        //                //foreach (DataRow patient in queryResult.Tables[0].Rows)
        //                    foreach (var patient in matchedSearches)
        //                    {
        //                        try
        //                    {
        //                        //var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient["PAPATID"].ToString());
        //                        var insertPatientSearchLogDetailQuery = QueryBuilder.InsertPatientSearchLogDetail(patientSearchLogIDResult, patient.ID.ToString());
        //                        dA.executeInsertSqlStatement(insertPatientSearchLogDetailQuery);
        //                    }
        //                    catch (Exception)
        //                    {
        //                    }
        //                }
        //            }
        //            status = true;
        //        }
        //        else
        //        {
        //            var sqlQuery = QueryBuilder.InsertPatientSearchLog(requestToken, sequence, input, "0");
        //            int queryStatus = dA.executeInsertSqlStatement(sqlQuery);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return status;
        //}
        #endregion


    }
}