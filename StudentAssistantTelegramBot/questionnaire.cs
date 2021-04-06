using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace StudentAssistantTelegramBot
{
    class questionnaire
    {
        public static  string Question = "no question";
        public static string FindMatch( string e)
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
                    Mathes.Add(NumberOfQuestion.ToString() + '.'+ Questions[i]);
                }
                CountMatches = 0;
            }
            string res = string.Join('\n', Mathes.ToArray());
            return res;
                        
        }
        public static void AddNewQuestion()
        {
            using (FileStream fs = new FileStream("BD_Voprosov_2.txt", FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs))
                sw.WriteLine(Question);
        }
    }
}
