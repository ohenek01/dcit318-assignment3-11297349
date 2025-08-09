using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection.Metadata.Ecma335;
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80)
            return "A";
        else if (Score >= 70)
            return "B";
        else if (Score >= 60)
            return "C";
        else if (Score >= 50)
            return "D";
        else
            return "F";
    }
}
public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }
public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base (message) { }
    }

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        int lineNumber = 0;

        try
        {
            using (var reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    var parts = line.Split(',');

                    if (parts.Length != 3)
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields but found {parts.Length}");
                    }

                    if (!int.TryParse(parts[0].Trim(), out int id))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format");
                    }

                    string fullName = parts[1].Trim();

                    if (!int.TryParse(parts[2].Trim(), out int score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format");
                    }
                    students.Add(new Student(id, fullName, score));
                }
            }
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"Input file not found: {inputFilePath}");
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName}, {student.Id}, {student.Score}, Grade : {student.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var app = new StudentResultProcessor();

        try
        {
            var students = app.ReadStudentsFromFile("students.txt");

            app.WriteReportToFile(students, "report.txt");
            Console.WriteLine("Report Generated Successfully");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}