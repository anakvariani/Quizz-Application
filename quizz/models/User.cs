using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<QuizRecord> QuizRecords { get; set; }
        public int TotalScore { get; set; }  

        public User()
        {
            QuizRecords = new List<QuizRecord>();
            TotalScore = 0; 
        }
    }
}
