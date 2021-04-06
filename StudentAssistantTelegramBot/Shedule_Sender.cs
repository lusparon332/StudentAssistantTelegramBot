using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAssistantTelegramBot
{
    class Shedule_Sender
    {
        static int test_min = 8;
        static int test_hour = 20;
        
        public static void Bot_SendWarn()
        {
            Dictionary<long, string> NeedSend = new Dictionary<long, string>();//Нужно будет поменять bool на string с названием дисциплины. Чтоб избежать коллизии можно попробовать в стеки
            while (true)
            {
                DateTime now = DateTime.Now;
                if ((now.Hour == 8 && now.Minute == 0 && now.Second == 0) || (now.Hour == 12 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == 16 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == 20 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == test_hour && now.Minute == test_min && now.Second == 0))
                {
                    NeedSend = CreateNotSendedArr();
                }

                if ((now.Hour == 8 && now.Minute == 0 && now.Second == 3 && now.Millisecond == 0) || (now.Hour == 12 && now.Minute == 0 && now.Second == 3 && now.Millisecond == 0) || (now.Hour == 16 && now.Minute == 0 && now.Second == 3 && now.Millisecond == 0) || (now.Hour == 20 && now.Minute == 0 && now.Second == 3 && now.Millisecond == 0) || (now.Hour == test_hour && now.Minute == test_min && now.Second == 3 && now.Millisecond == 0))
                {
                    Bot_SendWarn_core(now, ref NeedSend);
                }
            }
        }



        public static void Bot_SendWarn_core(DateTime now, ref Dictionary<long, string> NeedSend)
        {
            foreach (var i in NeedSend.Keys)
            {
                
                    string answer = $"Время подготовки. Сейчас у вас {NeedSend[i]}.";
                    Student st;
                    if (Program.students.ContainsStudByID(i,out st))
                    {
                        st.prev_loc = st.users_loc;
                        st.users_loc = LevelOfCode.Question_1;
                        answer += " Вы планируете сейчас готовиться?";//да или нет
                        //Место для переключения кода доступа. Внедрить диалог
                        
                    }
                    
                    Program.Bot.SendTextMessageAsync(i, answer);
                    Console.WriteLine($"Оповещение отправлено юзеру {i}");
                    //NeedSend[i] = false; //Создаёт коллизию в потоке. Не трогать под страхом расстрела через повешание
                

            }
            
        }

        //tester id 790754149


        /// <summary>
        /// Содержит ли массив дат сегодня
        /// </summary>
        /// <param name="Shed"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool ContData(DateTime[] Shed)
        {
            bool res = false;
            DateTime now = DateTime.Now;
            for (int i =0; i< Shed.Length;i++)
            {
                if (Shed[i].Date == now.Date && Shed[i].Hour == now.Hour && Shed[i].Minute == now.Minute)
                    return true;
            }
            return res;
        }


        public static Dictionary<long,string> CreateNotSendedArr()
        {
            Dictionary<long, string> res = new Dictionary<long, string>();
            foreach (var i in Program.students.list)
            {
                if (i.Shedule.Keys.Count != 0)
                {
                    foreach (var k in i.Shedule.Keys)
                    {

                        if (ContData(i.Shedule[k]))//Заменить Today на какое-то время. Предположительно 10.00. А лучше реализовать проверку
                        {
                            res.Add(i.student_id, k);
                        }
                    }
                }
            }

            return res;
        }

        public static void SetTestShed(ref Students s, long uid)
        {
            Console.WriteLine($"Юзеру {uid} установлено тестовое расписание. Сообщите офицеру безопасности!");
            for (int i = 0; i< s.list.Count;i++)
            {
                DateTime td = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour, test_min, 0);
                if (s.list[i].student_id == uid)
                    s.list[i].Shedule.Add("Test shedule", new DateTime[] { td });
            }
        }

    }
}
