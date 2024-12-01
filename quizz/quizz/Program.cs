using models;
using Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace quizz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string UserfilePath = @"../../../../Repository/Data/UsersData.json";
            string quizFilePath = @"../../../../Repository/Data/QuizzesData.json";

            var quizRepository = new QuizRepository(quizFilePath);
            var userRepository = new UserRepository(UserfilePath);

            // Top 10 Users
            DisplayTopUsers(userRepository);

            Console.WriteLine("Start QuizApp!");
            Console.WriteLine("Choose: 1. Register, 2. Login");

            int choice = int.Parse(Console.ReadLine());
            User currentUser = null;

            try
            {
                if (choice == 1)
                {
                    // Register User
                    Console.Write("Enter username: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();
                    currentUser = userRepository.Register(username, password);
                    Console.WriteLine("Registration successful!");
                }
                else if (choice == 2)
                {
                    // Login User
                    Console.Write("Enter username: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();
                    currentUser = userRepository.Login(username, password);
                    Console.WriteLine("Login successful!");
                }
                else
                {
                    Console.WriteLine("choose one of the options.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return;
            }

            // menu offers:
            while (true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Create a Quiz");
                Console.WriteLine("2. Solve a Quiz");
                Console.WriteLine("3. Manage My Quizzes");
                Console.WriteLine("4. Exit");

                choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    // Create a Quiz
                    CreateQuiz(currentUser, quizRepository);
                }
                else if (choice == 2)
                {
                    // Solve a Quiz
                    SolveQuiz(currentUser, quizRepository, userRepository);
                }
                else if (choice == 3)
                {
                    // Manage My Quizzes
                    ManageMyQuizzes(currentUser, quizRepository);
                }
                else if (choice == 4)
                {
                    // Exit the Application
                    Console.WriteLine(" Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("choose one  from the options.");
                }
            }
        }

        private static void DisplayTopUsers(UserRepository userRepository)
        {
            var topUsers = userRepository.GetAllUsers().Take(10).ToList();

            Console.WriteLine("\nTop 10 Users by Total Scores:");
            if (topUsers.Count == 0)
            {
                Console.WriteLine("No users available.");
                return;
            }

            foreach (var user in topUsers)
            {
                Console.WriteLine($"{user.Username} - Total Score: {user.TotalScore}");
            }
            Console.WriteLine();
        }

        private static void CreateQuiz(User currentUser, QuizRepository quizRepository)
        {
            Console.Write("Enter quiz title: ");
            string title = Console.ReadLine();
            List<Question> questions = new List<Question>();

            for (int i = 1; i <= 5; i++)
            {
                Console.Write($"Enter question {i}: ");
                string questionText = Console.ReadLine();
                List<string> options = new List<string>();

                for (int j = 1; j <= 4; j++)
                {
                    Console.Write($"Enter option {j}: ");
                    options.Add(Console.ReadLine());
                }

                int correctAnswerIndex;
                while (true)
                {
                    Console.Write("Enter the correct option number (1-4): ");
                    if (int.TryParse(Console.ReadLine(), out correctAnswerIndex) && correctAnswerIndex >= 1 && correctAnswerIndex <= 4)
                    {
                        correctAnswerIndex -= 1; 
                        break;
                    }
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                }

                questions.Add(new Question
                {
                    QuestionText = questionText,
                    Options = options,
                    CorrectAnswerIndex = correctAnswerIndex
                });
            }

            quizRepository.CreateQuiz(currentUser.Username, title, questions);
            Console.WriteLine("Quiz created successfully!");
        }



        private static void SolveQuiz(User currentUser, QuizRepository quizRepository, UserRepository userRepository)
        {
            while (true)
            {
               
                List<Quiz> quizzes = quizRepository.GetAllQuizzes()
                    .Where(q => q.CreatorUsername != currentUser.Username)
                    .ToList();

                if (quizzes.Count == 0)
                {
                    Console.WriteLine("\nNo quizzes available to solve for you.");
                    break;
                }

                
                Console.WriteLine("\nAvailable Quizzes:");
                for (int i = 0; i < quizzes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {quizzes[i].Title} by {quizzes[i].CreatorUsername}");
                }

                Console.Write("Choose a quiz (or go back to menu by entering 0): ");
                int quizIndex = int.Parse(Console.ReadLine()) - 1;

                if (quizIndex == -1)
                    break;

                if (quizIndex < 0 || quizIndex >= quizzes.Count)
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    continue;
                }

                Quiz selectedQuiz = quizzes[quizIndex];

                while (true)
                {
                    Console.WriteLine("\nYou have 2 minutes to complete this quiz!");
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    int score = 0;
                    bool timeUp = false;

                    foreach (var question in selectedQuiz.Questions)
                    {
                        if (stopwatch.Elapsed.TotalSeconds > 120)
                        {
                            Console.WriteLine("\nTime's up! You lost the quiz.");
                            timeUp = true;
                            break;
                        }

                        Console.WriteLine(question.QuestionText);
                        for (int i = 0; i < question.Options.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {question.Options[i]}");
                        }

                        int answer = -1; 
                        while (true)
                        {
                            Console.Write("Your answer (1-4): ");
                            string input = Console.ReadLine();
                            if (int.TryParse(input, out answer) && answer >= 1 && answer <= 4)
                            {
                                answer -= 1; 
                                break;
                            }
                            Console.WriteLine("invalid input,  Please enter a number between 1 and 4.");
                        }

                        if (answer == question.CorrectAnswerIndex)
                        {
                            score += 20;
                        }
                    }

                    stopwatch.Stop();

                    if (timeUp)
                    {
                        Console.WriteLine("\nTime's up, you earned no points. Choose one of the options:");
                        Console.WriteLine("1. Retry this quiz");
                        Console.WriteLine("2. Choose another quiz");
                        Console.WriteLine("3. Return to main menu");

                        int retryChoice = int.Parse(Console.ReadLine());

                        if (retryChoice == 1)
                        {
                            // Retry the same quiz
                            continue;
                        }
                        else if (retryChoice == 2)
                        {
                            // Choose another quiz
                            break;
                        }
                        else if (retryChoice == 3)
                        {
                            // Exit to main menu
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nQuiz completed! Your score: {score}");
                        UpdateQuizRecord(currentUser, selectedQuiz, score, userRepository);
                        break;
                    }
                }
            }
        }


        private static void UpdateQuizRecord(User user, Quiz quiz, int score, UserRepository userRepository)
        {
            var existingRecord = user.QuizRecords.FirstOrDefault(r => r.QuizTitle == quiz.Title);
            if (existingRecord == null)
            {
                user.QuizRecords.Add(new QuizRecord
                {
                    QuizTitle = quiz.Title,
                    BestScore = score,
                    CompletionDate = DateTime.Now
                });
            }
            else if (score > existingRecord.BestScore)
            {
                user.TotalScore -= existingRecord.BestScore;
                existingRecord.BestScore = score;
                existingRecord.CompletionDate = DateTime.Now;
            }

            user.TotalScore = user.QuizRecords.Sum(r => r.BestScore);
            userRepository.SaveUsers();
        }

        private static void ManageMyQuizzes(User currentUser, QuizRepository quizRepository)
        {
            while (true)
            {
                List<Quiz> quizzes = quizRepository.GetAllQuizzes()
                    .Where(q => q.CreatorUsername == currentUser.Username)
                    .ToList();

                if (quizzes.Count == 0)
                {
                    Console.WriteLine("\nNo quizzes created by you.");
                    break;
                }

                Console.WriteLine("\nYour Quizzes:");
                for (int i = 0; i < quizzes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {quizzes[i].Title}");
                }

                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Edit a Quiz");
                Console.WriteLine("2. Delete a Quiz");
                Console.WriteLine("3. Go back");

                int choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    Console.Write("which quiz you want to edit, choose number: ");
                    int quizIndex = int.Parse(Console.ReadLine()) - 1;

                    if (quizIndex >= 0 && quizIndex < quizzes.Count)
                    {
                        Quiz quizToEdit = quizzes[quizIndex];
                        EditQuiz(quizToEdit, quizRepository);
                    }
                    else
                    {
                        Console.WriteLine("Invalid quiz selection.");
                    }
                }
                else if (choice == 2)
                {
                    Console.Write("which quiz you want to delete, choose number:  ");
                    int quizIndex = int.Parse(Console.ReadLine()) - 1;

                    if (quizIndex >= 0 && quizIndex < quizzes.Count)
                    {
                        Quiz quizToDelete = quizzes[quizIndex];
                        quizRepository.DeleteQuiz(quizToDelete);
                        Console.WriteLine($"Quiz '{quizToDelete.Title}' deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid quiz selection.");
                    }
                }
                else if (choice == 3)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }


        private static void EditQuiz(Quiz quiz, QuizRepository quizRepository)
        {
            Console.WriteLine($"\nEditing Quiz: {quiz.Title}");

            // Display the questions in the quiz
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quiz.Questions[i].QuestionText}");
            }

            
            Console.Write("Select a question to edit (enter the number): ");
            int questionIndex = int.Parse(Console.ReadLine()) - 1;

            if (questionIndex < 0 || questionIndex >= quiz.Questions.Count)
            {
                Console.WriteLine("Invalid selection. Returning to the quiz menu.");
                return;
            }

            Question selectedQuestion = quiz.Questions[questionIndex];

            // Edit the question text
            Console.Write($"Enter new text for the question '{selectedQuestion.QuestionText}' (or press Enter to keep it): ");
            string newQuestionText = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newQuestionText))
            {
                selectedQuestion.QuestionText = newQuestionText;
            }

            // Edit the options for the selected question
            Console.WriteLine("\nCurrent options:");
            for (int i = 0; i < selectedQuestion.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {selectedQuestion.Options[i]}");
            }

            // Allow the user to edit or replace the options
            Console.WriteLine("\nWould you like to edit the options? yes or no");
            string editOptions = Console.ReadLine().ToLower();

            if (editOptions == "yes")
            {
                for (int i = 0; i < selectedQuestion.Options.Count; i++)
                {
                    Console.Write($"Enter new option {i + 1} (or press Enter to keep it): ");
                    string newOption = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newOption))
                    {
                        selectedQuestion.Options[i] = newOption;
                    }
                }

                //if correct option index changed
                Console.Write("Enter the correct option number : ");
                int newCorrectAnswerIndex = int.Parse(Console.ReadLine()) - 1;
                if (newCorrectAnswerIndex >= 0 && newCorrectAnswerIndex < 4)
                {
                    selectedQuestion.CorrectAnswerIndex = newCorrectAnswerIndex;
                }
                else
                {
                    Console.WriteLine("Invalid correct option. Keeping the original answer.");
                }
            }

            // Save the updated quiz
            quizRepository.UpdateQuiz(quiz);
            Console.WriteLine("\nQuiz updated successfully!");
        }

        private static void DeleteQuiz(Quiz quiz, QuizRepository quizRepository)
        {
            Console.WriteLine($"\nAre you sure you want to delete the quiz '{quiz.Title}'? (yes/no)");
            string confirmation = Console.ReadLine();

            if (confirmation.ToLower() == "yes")
            {
                quizRepository.DeleteQuiz(quiz);
                Console.WriteLine("Quiz deleted successfully!");
            }
            else
            {
                Console.WriteLine("Quiz deletion canceled.");
            }
        }
    }
}
