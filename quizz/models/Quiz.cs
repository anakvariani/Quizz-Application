using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models
{
    public class Quiz
    {
        public string Title { get; set; }
        public string CreatorUsername { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
