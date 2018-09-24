using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System;
using ChatBot.Business;
using ChatBot.Models;
using System.Collections.Generic;
using ChatBotApplication.Helper;

namespace ChatBotApplication
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        //private Activity HandleSystemMessage(Activity message)
        //{
        //    string messageType = message.GetActivityType();
        //    if (messageType == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (messageType == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (messageType == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (messageType == ActivityTypes.Typing)
        //    {
        //        // Handle knowing that the user is typing
        //    }
        //    else if (messageType == ActivityTypes.Ping)
        //    {
        //    }

        //    return null;
        //}
        private async Task HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    var greeting = Validator.checkDayGreeting();
                    var agentName = Validator.getAgentName();
                    if (string.IsNullOrEmpty(agentName))
                    {
                        agentName = "Cortana";
                    }

                    if (greeting!="Error")
                    {
                        string msg = greeting + ". Welcome to MedUSA. My name is " + agentName + ".";
                        Activity reply = message.CreateReply(msg);
                        //connector.Conversations.ReplyToActivityAsync(reply);
                        string options = Validator.getOptions();
                        string msg2 = "I can help with,";
                        msg2 += $"\n"+ options + $"\n" + "How can I help you today?";
                        reply.Text = msg;
                        Activity reply2 = message.CreateReply(msg2);
                        connector.Conversations.ReplyToActivity(reply);
                        connector.Conversations.ReplyToActivity(reply2);
                    }

                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }

    }
}