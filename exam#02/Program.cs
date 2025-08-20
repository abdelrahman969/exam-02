using System;
using System.Collections.Generic;

// -------------------------- Answer Class --------------------------
public class Answer : ICloneable
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; }

    public Answer(int id, string text)
    {
        AnswerId = id;
        AnswerText = text;
    }

    public object Clone()
    {
        return new Answer(AnswerId, AnswerText);
    }

    public override string ToString()
    {
        return $"{AnswerId}. {AnswerText}";
    }
}

// ------------------------------ Base Question ------------------------------
public abstract class Question : ICloneable, IComparable<Question>
{
    public string Header { get; set; }
    public string Body { get; set; }
    public int Mark { get; set; }
    public List<Answer> AnswerList { get; set; }
    public Answer RightAnswer { get; set; }

    public Question(string header, string body, int mark)
    {
        Header = header;
        Body = body;
        Mark = mark;
        AnswerList = new List<Answer>();
    }

    public abstract void Show();

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public int CompareTo(Question other)
    {
        return this.Mark.CompareTo(other.Mark);
    }

    public override string ToString()
    {
        return $"{Header}: {Body} (Mark: {Mark})";
    }
}

// ----------------------------- True/False Question -----------------------------
public class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string body, int mark)
        : base("True/False", body, mark)
    {
        AnswerList.Add(new Answer(1, "True"));
        AnswerList.Add(new Answer(2, "False"));
    }

    public override void Show()
    {
        Console.WriteLine(ToString());
        foreach (var ans in AnswerList)
            Console.WriteLine(ans);
    }
}

//--------------------- MCQ Question --------------------- 
public class MCQQuestion : Question
{
    public MCQQuestion(string body, int mark, List<Answer> answers)
        : base("MCQ", body, mark)
    {
        AnswerList.AddRange(answers);
    }

    public override void Show()
    {
        Console.WriteLine(ToString());
        foreach (var ans in AnswerList)
            Console.WriteLine(ans);
    }
}

// ------------------------ Base Exam------------------------
public abstract class Exam
{
    public int Time { get; set; }
    public int NumberOfQuestions { get; set; }
    public List<Question> Questions { get; set; }

    public Exam(int time, int numberOfQuestions)
    {
        Time = time;
        NumberOfQuestions = numberOfQuestions;
        Questions = new List<Question>();
    }

    public abstract void ShowExam();
}

// ----------------------- Final Exam -----------------------
public class FinalExam : Exam
{
    public FinalExam(int time, int numberOfQuestions)
        : base(time, numberOfQuestions) { }

    public override void ShowExam()
    {
        Console.WriteLine("=== Final Exam ===");
        int grade = 0;

        foreach (var q in Questions)
        {
            q.Show();
            Console.Write("Enter your answer: ");
            int userAnswer = int.Parse(Console.ReadLine());

            if (q.RightAnswer != null && q.RightAnswer.AnswerId == userAnswer)
            {
                grade += q.Mark;
            }
        }

        Console.WriteLine($"Your grade is {grade}/{Questions.Count * 10}");
    }
}

// ------------------------ Practical Exam ------------------------
public class PracticalExam : Exam
{
    public PracticalExam(int time, int numberOfQuestions)
        : base(time, numberOfQuestions) { }

    public override void ShowExam()
    {
        Console.WriteLine("=== Practical Exam ===");

        foreach (var q in Questions)
        {
            q.Show();
            Console.Write("Enter your answer: ");
            int userAnswer = int.Parse(Console.ReadLine());

            Console.WriteLine($"Correct Answer: {q.RightAnswer.AnswerText}\n");
        }
    }
}

// ------------------- Subject -------------------
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
        return $"Subject: {SubjectName} (ID: {SubjectId})";
    }
}

// -------------------------- Program Main --------------------------
public class Program
{
    public static void Main(string[] args)
    {
        Subject subj = new Subject(1, "Programming");

        FinalExam exam = new FinalExam(60, 2);
        var q1 = new TrueFalseQuestion("C# is an object-oriented language?", 10);
        q1.RightAnswer = q1.AnswerList[0]; // True
        exam.Questions.Add(q1);

        var answers = new List<Answer>
        {
            new Answer(1, "1995"),
            new Answer(2, "2000"),
            new Answer(3, "2002")
        };
        var q2 = new MCQQuestion("C# was released in?", 10, answers);
        q2.RightAnswer = answers[2];
        exam.Questions.Add(q2);

        subj.CreateExam(exam);

        Console.WriteLine(subj);
        subj.Exam.ShowExam();
    }
}
