using ChatBot.Data;
using ChatBotApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace ChatBot.Helper
{
    public class BotManager
    {
        DataAccess dA = new DataAccess();
        internal string getResponse(string requestToken, int inputSequence, bool status)
        {

            try
            {
                var query = QueryBuilder.GetPatientResponseSequence(inputSequence);
                var queryResult = dA.executeSqlStatement(query);
                if (queryResult.Tables[0].Rows.Count > 0)
                {

                    if (status)
                    {
                        var querySuccess = QueryBuilder.GetPatientResponseSequence(Convert.ToInt32(queryResult.Tables[0].Rows[0]["SuccessSequenceID"].ToString()));
                        var queryResultSuccess = dA.executeSqlStatement(querySuccess);
                        return queryResultSuccess.Tables[0].Rows[0]["ResponseMessage"].ToString();
                    }
                    else
                    {
                        var queryFailure = QueryBuilder.GetPatientResponseSequence(Convert.ToInt32(queryResult.Tables[0].Rows[0]["FailureSequenceID"].ToString()));
                        var queryResultFailure = dA.executeSqlStatement(queryFailure);
                        return queryResultFailure.Tables[0].Rows[0]["ResponseMessage"].ToString();
                    }
                }
                else
                {

                    return "No response found";
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        internal List<CommonQuestions> getCommonQuestions()
        {
            List<CommonQuestions> commonQuestions = new List<CommonQuestions>();
            try
            {
                string getQuestionQry = QueryBuilder.GetCommonQuestions();
                //DataSet commonQuestionsList = dA.executeSqlStatement(getQuestionQry);
                DataSet commonQuestionsList = new DataSet();
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ChatBot.Service.URL"].ToString());
                        var responseTask = client.GetAsync("executeSqlStatement?sqlstatement=" + getQuestionQry);
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<DataSet>();
                            readTask.Wait();
                            commonQuestionsList = readTask.Result;
                        }
                    }

                }
                catch (Exception ex)
                {


                }
                if (commonQuestionsList.Tables[0].Rows.Count>0)
                {
                    foreach (DataRow question in commonQuestionsList.Tables[0].Rows)
                    {
                        CommonQuestions cQues = new CommonQuestions();
                        cQues.QuestionID = Convert.ToInt64(question["QuestionID"]);
                        cQues.Question = Convert.ToString(question["Question"]);
                        cQues.Answer = Convert.ToString(question["Answer"]);
                        cQues.ConversationState = Convert.ToString(question["ConversationState"]);
                        cQues.ConversationStateValue = Convert.ToString(question["COnversationStateValue"]);
                        commonQuestions.Add(cQues);
                    }
                }
            }
            catch (Exception)
            {

            }
            return commonQuestions;
        }


    }

}