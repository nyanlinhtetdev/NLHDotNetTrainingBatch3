using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.QuizSystem.Models
{
    internal class Result
    {
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptedAt { get; set; }
    }
}
