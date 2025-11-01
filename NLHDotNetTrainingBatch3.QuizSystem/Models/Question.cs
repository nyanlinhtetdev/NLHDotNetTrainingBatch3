using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.QuizSystem.Models
{
    internal class Question
    {
        public int QuestionId { get; set; }
        public int SubjectId { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public char CorrectOption { get; set; }
        public string Explanation { get; set; }
    }
}
