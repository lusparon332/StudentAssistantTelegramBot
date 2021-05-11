using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace StudentAssistantTelegramBot
{
    public class questionnaire
    {
        public static string[] qss; 
        public static string Question = "no question";
        public static QuestionType qt;
        public static string FindMatch(string e)
        {
            Question = e;
            List<string> Mathes = new List<string>();
            string[] Questions = File.ReadAllLines("BD_Voprosov_1.txt");
            string[] MessageByWords = e.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int NeededMatches = 2;
            int CountMatches = 0;
            int NumberOfQuestion = 0;
            for (int i = 0; i < Questions.Length; i++)
            {
                for (int j = 0; j < MessageByWords.Length; j++)
                    if (Questions[i].ToLower().Contains(MessageByWords[j].ToLower()))
                        CountMatches++;
                if (CountMatches >= NeededMatches)
                {
                    NumberOfQuestion++;
                    Mathes.Add(Questions[i]);
                }
                CountMatches = 0;
            }
            qss = Mathes.ToArray();
            string res = string.Join('\n', Mathes.ToArray());
            return res;

        }
        public static void AddNewQuestion()
        {
            using (FileStream fs = new FileStream("BD_Voprosov_2.txt", FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs))
                sw.WriteLine(Question);
        }
        public static void SendQuestToModer()
        {
            Program.Bot.SendTextMessageAsync(584336025, $"ВАМ ВОПРОС: {qt.text}\n\nЕсли вопрос - спам, введите badq");
            foreach (var s in Program.students.list)
            {
                if (s.student_id == 584336025)
                {
                    s.prev_loc = s.users_loc;
                    s.users_loc = LevelOfCode.TO_ANSWER;
                }
            }
        }
    }

    public class QuestionType
    {
        public long from;
        public string text;
        public bool is_answered;

        public QuestionType(long f, string t, bool i)
        {
            this.from = f;
            this.text = t;
            this.is_answered = i;
        }
    }
}
