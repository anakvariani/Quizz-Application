using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using models;

namespace Repository
{
    public class QuizRepository
    {
        private readonly string _filePath;

        private List<Quiz> _quizzes;

        public QuizRepository(string filePath)
        {
            _filePath = filePath;
            _quizzes = LoadQuizzes();
        }

        public void CreateQuiz(string creatorUsername, string title, List<Question> questions)
        {
            if (questions.Count != 5)
                throw new Exception("Quiz must have exactly 5 questions.");

            _quizzes.Add(new Quiz
            {
                CreatorUsername = creatorUsername,
                Title = title,
                Questions = questions
            });

            SaveQuizzes();
        }

        
        public List<Quiz> GetAllQuizzes()
        {
            return _quizzes;
        }

       
        public void SaveQuizzes()
        {
            var json = JsonSerializer.Serialize(_quizzes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

      
        public List<Quiz> LoadQuizzes()
        {
            if (!File.Exists(_filePath))
                return new List<Quiz>();

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Quiz>(); /////////////////

            return JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
        }

        
        public void DeleteQuiz(Quiz quiz)
        {
            _quizzes.Remove(quiz);
            SaveQuizzes();
        }

        
        public void UpdateQuiz(Quiz updatedQuiz)
        {
           
            var quizIndex = _quizzes.FindIndex(q => q.Title == updatedQuiz.Title);

            if (quizIndex == -1)
            {
                throw new Exception("Quiz not found.");
            }

            _quizzes[quizIndex] = updatedQuiz;

            SaveQuizzes();
        }
    }
}
 