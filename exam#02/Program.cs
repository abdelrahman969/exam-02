using System;
using System.Collections.Generic;

namespace ExaminationSystem
{
    public class Answer : ICloneable
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }

        public Answer() { }
        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone()
        {
            return new Answer(this.AnswerId, this.AnswerText);
        }

        public override string ToString()
        {
            return $"{AnswerId}. {AnswerText}";
        }
    }

    public abstract class Question : ICloneable, IComparable<Question>
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public double Mark { get; set; }
        public List<Answer> AnswerList { get; set; }
        public Answer RightAnswer { get; set; }

        protected Question(string header, string body, double mark)
        {
            Header = header;
            Body = body;
            Mark = mark;
            AnswerList = new List<Answer>();
        }

        public abstract void ShowQuestion();

        public object Clone()
        {
            Question copy = (Question)this.MemberwiseClone();
            copy.AnswerList = new List<Answer>();
            foreach (var ans in AnswerList)
            {
                copy.AnswerList.Add((Answer)ans.Clone());
            }
            return copy;
        }

        public int CompareTo(Question other)
        {
            return Mark.CompareTo(other.Mark);
        }

        public override string ToString()
        {
            return $"{Header} - {Body} (Mark: {Mark})";
        }
    }

    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string body, double mark, Answer rightAnswer)
            : base("True/False Question", body, mark)
        {
            AnswerList.Add(new Answer(1, "True"));
            AnswerList.Add(new Answer(2, "False"));
            RightAnswer = rightAnswer;
        }

        public override void ShowQuestion()
        {
            Console.WriteLine(ToString());
            foreach (var ans in AnswerList)
            {
                Console.WriteLine(ans);
            }
        }
    }

    public class MCQQuestion : Question
    {
        public MCQQuestion(string body, double mark, List<Answer> answers, Answer rightAnswer)
            : base("MCQ Question", body, mark)
        {
            AnswerList = answers;
            RightAnswer = rightAnswer;
        }

        public override void ShowQuestion()
        {
            Console.WriteLine(ToString());
            foreach (var ans in AnswerList)
            {
                Console.WriteLine(ans);
            }
        }
    }

    public abstract class Exam
    {
        public int Time { get; set; }
        public int NumberOfQuestions { get; set; }
        public List<Question> Questions { get; set; }

        protected Exam(int time, int numberOfQuestions)
        {
            Time = time;
            NumberOfQuestions = numberOfQuestions;
            Questions = new List<Question>();
        }

        public abstract void ShowExam();
    }
    public class PracticalExam : Exam
    {
        public PracticalExam(int time, int numberOfQuestions)
            : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("---- Practical Exam ----");
            foreach (var q in Questions)
            {
                q.ShowQuestion();
                Console.WriteLine($"Correct Answer: {q.RightAnswer}\n");
            }
        }
    }
    public class FinalExam : Exam
    {
        public FinalExam(int time, int numberOfQuestions)
            : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("---- Final Exam ----");
            double totalGrade = 0;
            foreach (var q in Questions)
            {
                q.ShowQuestion();
                Console.Write("Enter your Answer Id: ");
                int userAns = int.Parse(Console.ReadLine());
                if (q.RightAnswer.AnswerId == userAns)
                {
                    totalGrade += q.Mark;
                }
            }
            Console.WriteLine($"Your Grade: {totalGrade} / {Questions.Count}");
        }
    }
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Exam Exam { get; set; }

        public Subject(int id, string name)
        {
            SubjectId = id;
            SubjectName = name;
        }

        public void CreateExam(Exam exam)
        {
            Exam = exam;
        }

        public override string ToString()
        {
            return $"Subject: {SubjectId} - {SubjectName}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Subject subj = new Subject(1, "OOP");

            Exam exam = new FinalExam(60, 2);

            exam.Questions.Add(new TrueFalseQuestion("C# is Object Oriented?", 5, new Answer(1, "True")));

            var mcqAnswers = new List<Answer>
            {
                new Answer(1, "Encapsulation"),
                new Answer(2, "Abstraction"),
                new Answer(3, "Polymorphism"),
                new Answer(4, "All of the above")
            };
            exam.Questions.Add(new MCQQuestion("Which is OOP concept?", 5, mcqAnswers, mcqAnswers[3]));

            subj.CreateExam(exam);

            Console.WriteLine(subj);
            subj.Exam.ShowExam();
        }
    }
}
