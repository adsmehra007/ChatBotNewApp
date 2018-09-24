using ChatBot.Business;
using ChatBot.Data;
using ChatBot.Helper;
using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ChatBot.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PatientController : ApiController
    {

        //[HttpGet]
        // [Route("api/Patient/TestAction")]

        //public string TestAction()
        //{
        //    try
        //    {
        //        DataAccess da = new DataAccess();
        //        da.executeDB2Statement("Select PANAME,PAZIP from QS36F.KMPAT Where PAPATID='63'").Tables[0].Rows[0]["PANAME"].ToString();
        //        return "conn done";
        //    }
        //    catch (Exception)
        //    {
        //        return "no conn";

        //    }


        //}

        [HttpGet]
        [Route("api/Patient/searchPatient")]
        public BOTResponse SearchPatient(string matchParameter,int sequence, string requestToken,List<Patient> patData)
        {
            BOTResponse response = new BOTResponse();
            if (string.IsNullOrEmpty(requestToken))
            {
                requestToken = Guid.NewGuid().ToString();
            }
            try
            {
                PatientSearch patientSearch = new PatientSearch();
                var responseCode = false;
                switch (sequence)
                {
                    case 1:
                        responseCode = patientSearch.MatchFirstName(sequence,matchParameter, requestToken, patData);
                        break;
                    case 2:
                        responseCode = patientSearch.MatchLastName(sequence,matchParameter, requestToken, patData);
                        break;
                    case 3:
                        responseCode = patientSearch.MatchDOB(sequence, matchParameter, requestToken, patData);
                        break;
                    case 4:
                        responseCode = patientSearch.MatchSSN(sequence, matchParameter, requestToken, patData);
                        break;
                    case 5:
                        responseCode = patientSearch.MatchZip(sequence, matchParameter, requestToken, patData);
                        break;
                    case 6:
                        responseCode = patientSearch.MatchPhone(sequence, matchParameter, requestToken, patData);
                        break;
                    default:
                        break;
                }
                if (responseCode)
                {
                    BotManager bM = new BotManager();
                    response.ResponseMessage = bM.getResponse(requestToken, sequence, responseCode);
                    response.RequestToken = requestToken;
                    response.status = responseCode;
                }
                else
                {
                    BotManager bM = new BotManager();
                    response.ResponseMessage = bM.getResponse(requestToken, sequence, responseCode);
                    response.RequestToken = requestToken;
                    response.status = responseCode;
                }
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        [HttpGet]
        // [Route("api/Patient/getMenuOption")]
        public void getMenuOption(string requestToken,string selectedOption)
        {
            try
            {
                switch (ConversionHelper.convertSelectedOption(selectedOption))
                {
                    case 1:
                        BookAppointment(1,"","");
                        break;
                    case 2:
                        getVisitHistory(1,1);
                        break;
                    case 3:
                        PayBill(1);
                        break;
                    case 4:
                        RefillRequest(1);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        [HttpGet]
        // [Route("api/Patient/getOption")]
        public void getOption(int PatientID)
        {


        }

        [HttpGet]
        // [Route("api/Patient/getVisitHistory")]
        public void getVisitHistory(int PatientID, int VisitID) {


        }

        [HttpGet]
        // [Route("api/Patient/BookAppointment")]
        public void BookAppointment(int PatientID, string Date,string Time) {
        }

        [HttpGet]
        //  [Route("api/Patient/PayBill")]
        public void PayBill(int PatientID)
        {

        }

        [HttpGet]
        // [Route("api/Patient/RefillRequest")]
        public void RefillRequest(int PatientID)
        {
        }
  

     

    }
}
