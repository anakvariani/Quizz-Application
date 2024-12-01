using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models

{
    public class QuizRecord
    {
        public string QuizTitle { get; set; }
        public int BestScore { get; set; } 
        public DateTime CompletionDate { get; set; }
    }
}
