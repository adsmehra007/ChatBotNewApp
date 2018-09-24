using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBotApplication.Models
{
    public class CommonQuestions
    {
        public long QuestionID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string ConversationState { get; set; }
        public string ConversationStateValue { get; set; }


    }
}