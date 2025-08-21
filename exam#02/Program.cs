using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Answer
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; }

    public Answer(int answerId, string answerText)
    {
        AnswerId = answerId;
        AnswerText = answerText;
    }

    public override string ToString() => $"[{AnswerId}] {AnswerText}";
}

public abstract class Question : ICloneable, IComparable<Question>
{
    public string Header { get; set; } 
    public string Body { get; set; }
    public int Mark { get; set; }
    public Answer[] AnswerList { get; set; }
    public Answer CorrectAnswer { get; set; }

    protected Question(string header, string body, int mark, Answer[] answers, Answer correctAnswer)
    {
        Header = header;
        Body = body;
        Mark = mark;
        AnswerList = answers;
        CorrectAnswer = correctAnswer;
    }

    public object Clone() => MemberwiseClone();

    public int CompareTo(Question other) => Mark.CompareTo(other.Mark);

    public override string ToString() => $"{Body}\nMark: {Mark}";

    public abstract void DisplayQuestion();
}

public class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string header, string body, int mark, Answer[] answers, Answer correctAnswer)
        : base(header, body, mark, answers, correctAnswer)
    {
    }

    public override void DisplayQuestion()
    {
        Console.WriteLine($"{ToString()}\nAnswers:");
        foreach (var answer in AnswerList)
        {
            Console.WriteLine(answer);
        }
    }
}

public class MCQQuestion : Question
{
    public MCQQuestion(string header, string body, int mark, Answer[] answers, Answer correctAnswer)
        : base(header, body, mark, answers, correctAnswer)
    {
    }

    public override void DisplayQuestion()
    {
        Console.WriteLine($"{ToString()}\nAnswers:");
        foreach (var answer in AnswerList)
        {
            Console.WriteLine(answer);
        }
    }
}

public abstract class Exam
{
    public TimeSpan Time { get; set; }
    public int NumberOfQuestions { get; set; }
    public Question[] Questions { get; set; }

    protected Exam(TimeSpan time, int numberOfQuestions)
    {
        Time = time;
        NumberOfQuestions = numberOfQuestions;
        Questions = new Question[numberOfQuestions];
    }

    public abstract void ShowExam();
}

public class FinalExam : Exam
{
    public FinalExam(TimeSpan time, int numberOfQuestions) : base(time, numberOfQuestions)
    {
    }

    public override void ShowExam()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int totalMarks = 0;
        int score = 0;
        List<Answer> userAnswers = new List<Answer>();

        Console.WriteLine("\nFinal Exam:");
        for (int i = 0; i < NumberOfQuestions; i++)
        {
            Console.WriteLine($"\nQuestion {i + 1}:");
            Questions[i].DisplayQuestion();
            Console.Write("Enter answer ID: ");
            string input = Console.ReadLine();
            Answer userAnswer = null;

            if (int.TryParse(input, out int userAnswerId))
            {
                userAnswer = Questions[i].AnswerList.FirstOrDefault(a => a.AnswerId == userAnswerId);
                if (userAnswer != null && Questions[i].CorrectAnswer.AnswerId == userAnswerId)
                {
                    score += Questions[i].Mark;
                }
            }
            userAnswers.Add(userAnswer);
            totalMarks += Questions[i].Mark;
        }

        stopwatch.Stop();
        TimeSpan timeTaken = stopwatch.Elapsed;

        Console.WriteLine("\nFinal Exam Results:");
        Console.WriteLine($"Questions: {NumberOfQuestions}");
        Console.WriteLine($"Your Score: {score}/{totalMarks}");
        Console.WriteLine($"Time Taken: {timeTaken.Minutes} minutes and {timeTaken.Seconds} seconds");

        Console.WriteLine("\nAnswer Review:");
        for (int i = 0; i < NumberOfQuestions; i++)
        {
            Console.WriteLine($"\nQuestion {i + 1}: {Questions[i].Body}");
            Console.WriteLine($"Correct Answer: {Questions[i].CorrectAnswer}");
            Console.WriteLine($"Your Answer: {(userAnswers[i] != null ? userAnswers[i].ToString() : "Invalid or no answer provided")}");
        }
    }
}

public class PracticalExam : Exam
{
    public PracticalExam(TimeSpan time, int numberOfQuestions) : base(time, numberOfQuestions)
    {
    }

    public override void ShowExam()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Console.WriteLine("\nPractical Exam:");
        for (int i = 0; i < NumberOfQuestions; i++)
        {
            Console.WriteLine($"\nQuestion {i + 1}:");
            Questions[i].DisplayQuestion();
            Console.Write("Enter answer ID: ");
            Console.ReadLine();
            Console.WriteLine($"Correct Answer: {Questions[i].CorrectAnswer}\n");
        }

        stopwatch.Stop();
        TimeSpan timeTaken = stopwatch.Elapsed;
        Console.WriteLine($"\nTime Taken: {timeTaken.Minutes} minutes and {timeTaken.Seconds} seconds");
    }
}
public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Exam Exam { get; set; }

    public Subject(int subjectId, string subjectName)
    {
        SubjectId = subjectId;
        SubjectName = subjectName;
    }

    public void CreateExam(Exam exam)
    {
        Exam = exam;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Subject programming = new Subject(1, "Programming Fundamentals");

        Console.WriteLine("Please select the type of exam:");
        Console.WriteLine("[1] Final");
        Console.WriteLine("[2] Practical");
        Console.Write("Enter the number (1 or 2): ");

        int examTypeChoice;
        while (!int.TryParse(Console.ReadLine(), out examTypeChoice) || (examTypeChoice != 1 && examTypeChoice != 2))
        {
            Console.WriteLine("Invalid choice. Please enter 1 for Final or 2 for Practical.");
            Console.Write("Enter the number (1 or 2): ");
        }

        bool isFinal = examTypeChoice == 1;

        int examMinutes;
        do
        {
            Console.Write("Please enter the exam time in minutes (30-180): ");
            if (!int.TryParse(Console.ReadLine(), out examMinutes) || examMinutes < 30 || examMinutes > 180)
            {
                Console.WriteLine("Invalid time. Must be between 30 and 180 minutes.");
            }
        } while (examMinutes < 30 || examMinutes > 180);

        TimeSpan examTime = TimeSpan.FromMinutes(examMinutes);

        int numQuestions;
        do
        {
            Console.Write("Please enter the number of questions: ");
            if (!int.TryParse(Console.ReadLine(), out numQuestions) || numQuestions <= 0)
            {
                Console.WriteLine("Invalid number. Must be positive integer.");
            }
        } while (numQuestions <= 0);

        Exam exam = isFinal ? new FinalExam(examTime, numQuestions) : new PracticalExam(examTime, numQuestions);

        for (int i = 0; i < numQuestions; i++)
        {
            Console.WriteLine($"\nQuestion {i + 1}:");

            string qType;
            if (!isFinal)
            {
                qType = "mcq";
                Console.WriteLine("Question type: MCQ (only for Practical)");
            }
            else
            {
                Console.WriteLine("Please select question type:");
                Console.WriteLine("[1] True/False");
                Console.WriteLine("[2] MCQ");
                Console.Write("Enter the number (1 or 2): ");

                int qTypeChoice;
                while (!int.TryParse(Console.ReadLine(), out qTypeChoice) || (qTypeChoice != 1 && qTypeChoice != 2))
                {
                    Console.WriteLine("Invalid choice. Please enter 1 for True/False or 2 for MCQ.");
                    Console.Write("Enter the number (1 or 2): ");
                }
                qType = qTypeChoice == 1 ? "true/false" : "mcq";
            }


            Console.Write("Please enter the body of the question: ");
            string body = Console.ReadLine();

            int mark;
            do
            {
                Console.Write("How many marks for this question: ");
                if (!int.TryParse(Console.ReadLine(), out mark) || mark <= 0)
                {
                    Console.WriteLine("Invalid mark. Must be positive integer.");
                }
            } while (mark <= 0);

            Answer[] answers;
            if (qType == "true/false")
            {
                answers = new Answer[]
                {
                    new Answer(1, "True"),
                    new Answer(2, "False")
                };
            }
            else 
            {
                List<Answer> optionList = new List<Answer>();
                for (int j = 1; j <= 3; j++)
                {
                    Console.Write($"Please enter option {j}: ");
                    string optionText = Console.ReadLine();
                    optionList.Add(new Answer(j, optionText));
                }
                answers = optionList.ToArray();
            }

            Console.WriteLine("Options:");
            foreach (var ans in answers)
            {
                Console.WriteLine(ans);
            }

            int correctId;
            do
            {
                Console.Write("Please enter the correct answer ID: ");
                if (!int.TryParse(Console.ReadLine(), out correctId) || !answers.Any(a => a.AnswerId == correctId))
                {
                    Console.WriteLine("Invalid ID. Must be one of the option IDs.");
                }
            } while (!answers.Any(a => a.AnswerId == correctId));

            Answer correctAnswer = answers.First(a => a.AnswerId == correctId);

            Question question = qType == "true/false"
                ? new TrueFalseQuestion(header, body, mark, answers, correctAnswer)
                : new MCQQuestion(header, body, mark, answers, correctAnswer);

            exam.Questions[i] = question;
        }

        programming.CreateExam(exam);

        Console.WriteLine("\nDo you want to start the exam?");
        Console.WriteLine("[1] Yes");
        Console.WriteLine("[2] No");
        Console.Write("Enter the number (1 or 2): ");

        int startExamChoice;
        while (!int.TryParse(Console.ReadLine(), out startExamChoice) || (startExamChoice != 1 && startExamChoice != 2))
        {
            Console.WriteLine("Invalid choice. Please enter 1 for Yes or 2 for No.");
            Console.Write("Enter the number (1 or 2): ");
        }

        if (startExamChoice == 1)
        {
            programming.Exam.ShowExam();
        }
        else
        {
            Console.WriteLine("Exiting the program.");
            return;
        }
    }
}