using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Data
{
    public class QueryBuilder
    {
        #region Patient

        public static string GetPatientByPatID(string id)
        {
            try
            {
                string queryStatement = @"Select * from QS36F.TRPAT Where PAPATID='" + id + "'";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public static string SearchPatientName(string name)
        public static string SearchPatientName( )
        {
            try
            {
                string queryStatement = @"Select * from QS36F.TRPAT";
                //string queryStatement = @"Select * from QS36F.TRPAT Where PANAME LIKE '%" + name +"%' ";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string SearchPatientDOB(string dob, string ID)
        {
            try
            {
                string queryStatement = @"Select * from QS36F.TRPAT Where PABRTHDT='" + dob + "' and PAPATID ="+ID+"";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string SearchPatientSSN(string SSN, string ID)
        {
            try
            {
                string queryStatement = @"Select * from QS36F.TRPAT Where PASSN='" + SSN + "' and PAPATID =" + ID + "";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string SearchZIP(string zip, string ID)
        {
            try
            {
                string queryStatement = @"Select * from QS36F.TRPAT Where PAZIP='" + zip + "' and PAPATID =" + ID + "";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region SQLQueries
       
        public static string InsertPatientSearchLog(string requestToken, int responseSequenceID, string matchParam,string matchStatus)
        {
            try
            {
                string queryStatement = @"INSERT INTO [dbo].[PatientSearchLog] (RequestToken, ResponseSequenceID, MatchParameter,IsMatch) VALUES ('" + requestToken + "'," + responseSequenceID + ", '" + matchParam + "',"+ matchStatus + ");";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GetPatientSearchLogID(string requestToken,int responseSequenceID)
        {
            try
            {
                string queryStatement = @"Select [PatientSearchLogID] from [dbo].[PatientSearchLog] where requesttoken = '"+ requestToken + "' and responsesequenceid="+ responseSequenceID + " and ismatch=1 ; ";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GetPatientSearchLogID(string requestToken)
        {
            try
            {
                string queryStatement = @"Select [PatientSearchLogID] from [dbo].[PatientSearchLog] where requesttoken = '" + requestToken + "' ; ";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string InsertPatientSearchLogDetail(string InsertPatientSearchLogID, string PatientID)
        {
            try
            {
                string queryStatement = @"INSERT INTO [dbo].[PatientSearchLogDetail] (PatientSearchLogID,PatientID ) VALUES (" + InsertPatientSearchLogID + "," + PatientID + ");";
                return queryStatement;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetPatientResponseSequence(int ResponseSequenceID)
        {
            try
            {
                string queryStatement = @"SELECT * FROM [dbo].[ResponseSequence] where responsesequenceid="+ResponseSequenceID;
                return queryStatement;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public static string GetPatientsBySearchLogID(string PatientSearchLogID)
        {
            try
            {
                string queryStatement = @"Select [PatientID] from [dbo].[PatientSearchLogDetail] where patientsearchlogid=" + PatientSearchLogID + " ; ";
                return queryStatement;
            }
            catch (Exception ex)
            {

                return null;
            }
        }



        public static string GetCommonQuestions()
        {
            try
            {
                string queryStatement = @"SELECT * FROM [ChatBot].[dbo].[Common.Questions]";
                return queryStatement;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static string GetAnswerByQuestionID(string questionID)
        {
            try
            {
                string queryStatement = @"SELECT Answer,ConversationState,ConversationStateValue FROM [ChatBot].[dbo].[Common.Questions] where QuestionID="+questionID+";";
                return queryStatement;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        #endregion
    }
}