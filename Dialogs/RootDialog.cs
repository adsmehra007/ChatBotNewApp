using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ChatBotApplication.Models;
using Microsoft.Bot.Builder;
using ChatBot.Controllers;
using ChatBot.Models;
using ChatBotApplication.Helper;
using ChatBotApplication;
using ChatBot.Helper;
using System.Configuration;

namespace ChatBotApplication.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            MessagesController obj = new MessagesController();
            var activity = await result as Activity;
            PatientController pC = new PatientController();
            BotManager bM = new BotManager();
            BOTResponse bR = new BOTResponse();
            var state = string.Empty;
            var input = string.Empty;
            var responseToken = string.Empty;
            string botResponse = string.Empty;
            input = activity.Text.ToString().ToLower();
            bool questionResponded = false;
            bool isOption = false;

            try
            {
                state = context.ConversationData.GetValue<string>("state");
                responseToken = context.ConversationData.GetValue<string>("ResponseToken");
            }
            catch (Exception ex)
            {

            }
            if (Validator.getOptionSelected(input) > 0)
            {
                var optionNo = Validator.getOptionSelected(input);
                isOption = true;
            }
            if (input.Trim() == "1" || input.Trim() == "2" || input.Trim() == "3" || input.Trim() == "4" || input.Trim() == "5" || input.Trim() == "6" || input.Trim() == "7")
            {
                isOption = true;
            }
            if (state == "firstname")
            {
                WebApiApplication.firstName = input;
                //bR = pC.SearchPatient(input, 1, null, WebApiApplication.getPatData);
                bR = Validator.BotAPICall("SearchPatient", input, 1, responseToken, WebApiApplication.getPatData);

                if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "lastname");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "firstname");
                }
                await context.PostAsync($"" + bR.ResponseMessage);
                questionResponded = true;
                WebApiApplication.getPatFirstName = bR.FilteredPatList;
            }
            if (state == "lastname")
            {
                //bR = pC.SearchPatient(input, 2, null, WebApiApplication.getPatFirstName);
                bR = Validator.BotAPICall("SearchPatient", input, 2, responseToken, WebApiApplication.getPatFirstName);

                //if (WebApiApplication.getPatLastName.Count == 1)
                //{
                //    await context.PostAsync($"Thank you for Verifying your Details.");
                //    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                //    context.ConversationData.SetValue<string>("state", "options");
                //}
                //else
                if (bR.status == true)
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "dob");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "");
                }
                questionResponded = true;
                WebApiApplication.getPatLastName = bR.FilteredPatList;

            }
            else if (state == "dob")
            {
                input = ConversionHelper.dateFormat(input);
                if (Validator.checkDateFormat(input))
                {
                    bR = Validator.BotAPICall("SearchPatient", input, 3, responseToken, WebApiApplication.getPatLastName);

                    //bR = pC.SearchPatient(input, 3, responseToken, WebApiApplication.getPatLastName);
                    if (bR.status == true)
                    {

                        if (WebApiApplication.getPatDOB.Count == 1)
                        {
                            await context.PostAsync($"Thank you for Verifying your Details.");
                            //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                            //context.ConversationData.SetValue<string>("state", "options");
                        }
                        else
                        {
                            bool hasSSN = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.SSN));
                            bool hasHomePhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatHomePhone));
                            bool hasWorkPhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatWorkPhone));
                            if (hasHomePhone || hasWorkPhone)
                            {
                                context.ConversationData.SetValue<string>("state", "phone");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"Please enter the last 4 digits of your Phone number");
                            }
                            else if (hasSSN)
                            {
                                context.ConversationData.SetValue<string>("state", "ssn");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"" + bR.ResponseMessage);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "dob");
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "dob");
                    await context.PostAsync($"Please enter valid Date format, (MM/DD/YYYY)");
                }
                questionResponded = true;
                WebApiApplication.getPatDOB = bR.FilteredPatList;

            }
            else if (state == "phone")
            {
                if (Validator.isPhoneNumberValid(input))
                {
                    bR = Validator.BotAPICall("SearchPatient", input,6, responseToken, WebApiApplication.getPatDOB);
                    //bR = pC.SearchPatient(input, 6, responseToken, WebApiApplication.getPatDOB);
                    if (WebApiApplication.getPatPhone.Count == 1)
                    {
                        await context.PostAsync($"Thank you for Verifying your Details.");
                        //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                        //context.ConversationData.SetValue<string>("state", "options");
                    }
                    else if (bR.status == true)
                    {
                        context.ConversationData.SetValue<string>("state", "ssn");
                        context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "");
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "phone");
                    await context.PostAsync($"Please enter valid Phone format, (xxxxxxxxxx)");
                }
                questionResponded = true;
                WebApiApplication.getPatPhone = bR.FilteredPatList;

            }

            else if (state == "zip")
            {
                bR = Validator.BotAPICall("SearchPatient", input, 5, responseToken, WebApiApplication.getPatZIP);

                //bR = pC.SearchPatient(input, 5, responseToken, WebApiApplication.getPatZIP);
                if (WebApiApplication.getPatZIP.Count == 1)
                {
                    await context.PostAsync($"Thank you for Verifying your Details.");
                    //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    //context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    await context.PostAsync($"" + bR.ResponseMessage);
                    //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    //context.ConversationData.SetValue<string>("state", "options");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                questionResponded = true;
            }
            else if (state == "ssn")
            {
                bR = Validator.BotAPICall("SearchPatient", input, 4, responseToken, WebApiApplication.getPatDOB);

                //bR = pC.SearchPatient(input, 4, responseToken, WebApiApplication.getPatDOB);
                if (WebApiApplication.getPatSSN.Count == 1)
                {
                    await context.PostAsync($"Thank you for Verifying your Details.");
                    //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    //context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "ssn");
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                questionResponded = true;
                WebApiApplication.getPatSSN = bR.FilteredPatList;

            }
            else if (state == "reqdescription")
            {
                context.ConversationData.SetValue<string>("state", "reqdescriptionresp");
                await context.PostAsync($"I'm afraid I can't help with this at this time. Can I ask someone to call you?");
                questionResponded = true;
            }
            else if (state == "reqdescriptionresp")
            {
                if (input=="no")
                {
                    context.ConversationData.SetValue<string>("state", "");
                    await context.PostAsync($"My apologies I wasn't of any help.");
                    questionResponded = true;
                }
                else
                {
                    await context.PostAsync($"I'll need some more details from you so we can contact you at the right number.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");
                    questionResponded = true;
                }
            }
            else if (state == "options")
            {
                string keyName = "Options.option" + input;
                var keyVal = ConfigurationManager.AppSettings[keyName];
                if (input.Length > 1)
                {
                    input = Validator.getOptionSelected(input).ToString();
                    keyName = "Options.option" + input;
                    try
                    {
                        keyVal = ConfigurationManager.AppSettings[keyName];
                    }
                    catch (Exception)
                    {

                    }
                }
                if (input.ToLower() == "1" || Validator.sentenceComparison(keyVal, input) ==true)
                {
                    await context.PostAsync($"You chose to Book an Appointment.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "2" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"You chose to Request Refills.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "3" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"You chose to Update on Lab Orders.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "4"|| Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"You chose to pay your bills.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "5"|| Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"You chose to contact the Doctor.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "6"|| Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"You chose to inquire about your Medical History.");
                    context.ConversationData.SetValue<string>("state", "firstname");
                    await context.PostAsync($"Can I know your first name please ?");

                    questionResponded = true;
                }
                else if (input.ToLower() == "7" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Please type a brief description of your requirement.");
                    context.ConversationData.SetValue<string>("state", "reqdescription");
                    questionResponded = true;
                }
                else
                {
                    //await context.PostAsync($"Sorry, Cannot process this input at the moment.");
                    try
                    {
                        var commonQuestions = bM.getCommonQuestions();
                        foreach (var cQuestion in commonQuestions)
                        {
                            try
                            {
                                bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                                if (isMatch == true)
                                {
                                    if (cQuestion.ConversationStateValue == "initgreet")
                                    {
                                        await context.PostAsync($"How can i help you today ? {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                                        context.ConversationData.SetValue<string>("state", "options");
                                    }
                                    else if (cQuestion.ConversationStateValue == "greet")
                                    {
                                        botResponse = cQuestion.Answer;
                                        botResponse = botResponse.Replace("{greet}", Validator.checkDayGreeting());
                                        await context.PostAsync(botResponse);
                                        context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                    }
                                    else
                                    {
                                        botResponse = cQuestion.Answer;
                                        await context.PostAsync(botResponse);
                                        context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                    }
                                    questionResponded = true;
                                    break;
                                }
                                else
                                {
                                    questionResponded = false;
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
                }
            }
            else
            {
                try
                {
                    var commonQuestions = bM.getCommonQuestions();
                    foreach (var cQuestion in commonQuestions)
                    {
                        try
                        {
                            bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                            if (isMatch == true)
                            {
                                if (cQuestion.ConversationStateValue == "initgreet")
                                {
                                    await context.PostAsync($"How can i help you today ?, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                                    context.ConversationData.SetValue<string>("state", "options");

                                }
                                else if (cQuestion.ConversationStateValue == "greet")
                                {
                                    botResponse = cQuestion.Answer;
                                    botResponse = botResponse.Replace("{greet}", Validator.checkDayGreeting());
                                    await context.PostAsync(botResponse);
                                    context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                }
                                else
                                {
                                    botResponse = cQuestion.Answer;
                                    await context.PostAsync(botResponse);
                                    context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                }
                                questionResponded = true;

                                break;
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
            }
            if (questionResponded == false)
            {
                await context.PostAsync($"Sorry, I couldn't get you.");

            }

            //if (input.Contains("hey") || input.Contains("hello"))
            //{
            //    await context.PostAsync($"What is your name ?");
            //    context.ConversationData.SetValue<string>("State","name");
            //}


            //else if (ShortName == null)
            //{
            //    await context.PostAsync($"The ICDcodes is Invalid..Please Enter Valid ICDcode ");
            //}
            //else
            //{
            //    await context.PostAsync($"The ICDcodes is  {activity.Text} and its shortname is  {ShortName} ");
            //    await context.PostAsync($"Do you want to search something else ...press yes or no!!");

            //}
            // Return our reply to the user


            context.Wait(MessageReceivedAsync);
        }

    }
}