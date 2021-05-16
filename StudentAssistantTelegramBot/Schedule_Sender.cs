using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAssistantTelegramBot
{
    class Shedule_Sender
    {
        static List<SendObj> NeedSend; //Лист для рассылки
        static List<SendObj> Sended; //Лист, того, что уже было разослано и в повторной рассылке не нуждается

        //Время, на которое ставится тестовое расписание (нужно для отладки)
        static int test_min = 51;
        static int test_hour = 2;

        // Класс объекта отправки оповещения
        public class SendObj
        {
            public DateTime SendTime;
            public long uid;
            public string name;

            /// <summary>
            /// Очевидно, конструктор класса объекта отправки оповещения
            /// </summary>
            /// <param name="DT"></param>
            /// <param name="id"></param>
            /// <param name="namen"></param>
            public SendObj(DateTime DT, long id, string namen)
            {
                SendTime = DT;
                uid = id;
                name = namen;
            }
        }

        /// <summary>
        /// Оболочка потока с рассылкой
        /// </summary>
        public static void Bot_SendWarn()
        {
            //List<SendObj> NeedSend = new List<SendObj>();
            NeedSend = new List<SendObj>();
            Sended = new List<SendObj>();
            while (!Program.exit)
            {
                DateTime now = DateTime.Now;
                if (now.Minute % 2 == 0 && now.Second == 0 && now.Millisecond == 0)
                {
                    //NeedSend = CreateNotSendedArr();//!!
                    AddNotSended(ref NeedSend);
                }

                Bot_SendWarn_core(now, ref NeedSend);
            }

            Console.WriteLine("\nБот выключен. Поток 2\n");
        }


        /// <summary>
        /// Модуль, отвечающий за непосредственно рассылку
        /// </summary>
        /// <param name="now"></param>
        /// <param name="NeedSend"></param>
        public static void Bot_SendWarn_core(DateTime now, ref List<SendObj> NeedSend)
        {
            foreach (var i in NeedSend)
            {
                if (now.Hour == i.SendTime.Hour && now.Minute == i.SendTime.Minute)// && now.Minute == i.SendTime.Minute && now.Second == i.SendTime.Second)
                {
                    //if (!Sended.Contains(i))
                    if (!MyContains(ref Sended, i))
                    {
                        string answer = $"Время подготовки к экзамену по предмету: {i.name}.\n\n";
                        if (Program.students.ContainsStudByID(i.uid, out Student st))
                        {
                            st.prev_loc = st.users_loc;
                            st.users_loc = LevelOfCode.PREPARE_TIME;
                            answer += "Вы планируете сейчас готовиться?";
                        }
                        //if (!Sended.Contains(i))
                        //{

                        var rkm = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();

                        rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                        {
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Да ✏"),
                            },
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Перенести на 15 минут ⏰"),
                            },
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Перенести на 30 минут ⏰"),
                            },
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Перенести на 1 час ⏰"),
                            },
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Перенести на 2 часа ⏰"),
                            },
                            new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                            {
                                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Нет, сегодня я не буду готовиться 😪"),
                            }
                        };

                        Program.Bot.SendTextMessageAsync(i.uid, answer, replyMarkup: rkm);
                        Sended.Add(i);
                        Console.WriteLine($"Оповещение отправлено пользователю: {i.uid}");
                    }
                    //if (!Sended.Contains(i))
                    //{
                        Program.Bot.SendTextMessageAsync(i.uid, answer);
                        Sended.Add(i);
                        Console.WriteLine($"Оповещение отправлено пользователю: {i.uid}");
                    }

                    //if (!Sended.Contains(i))
                    if (!MyContains(ref Sended, i))
                        Sended.Add(i);
                }
            }

            foreach (var i in Sended)
            {
                NeedSend.Remove(i);
            }

        }


        /// <summary>
        /// Самописный компаратор дат, ныне вроде не применяется
        /// </summary>
        /// <param name="DT"></param>
        /// <returns></returns>
        public static bool MyDateLessNow(DateTime DT)
        {
            bool res = false;
            DateTime Now = DateTime.Now;
            if (DT.Hour < Now.Hour)
                return true;
            else if (DT.Hour == Now.Hour)
                if (DT.Minute < Now.Minute)
                    return true;
                else if (DT.Minute == Now.Minute)
                    if (DT.Second < Now.Second - 1)
                        return true;
                    else if (DT.Second == Now.Second)
                        if (DT.Millisecond < Now.Millisecond)
                            return true;

            return res;
        }

        /// <summary>
        /// Фильтр сегодняшнего числа
        /// </summary>
        /// <param name="Shed"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool ContData(DateTime Shed)
        {
            DateTime now = DateTime.Now;
            //return (Shed.Date == now.Date && Shed.Hour == now.Hour && Shed.Minute == now.Minute);
            return (Shed.Date == now.Date);
        }

        /*        /// <summary>
                /// Устаревший кусок кода. Делал лист для рассылки
                /// </summary>
                /// <returns></returns>
                public static List<SendObj> CreateNotSendedArr()
                {
                    //Dictionary<long, string> res = new Dictionary<long, string>();
                    List<SendObj> res = new List<SendObj>();
                    foreach (var i in Program.students.list)
                    {
                        foreach (var k in i.Shedule.Keys)
                        {
                            foreach (var j in i.Shedule[k])
                            {
                                if (ContData(j) && !MyDateLessNow(j))
                                    res.Add(new SendObj(j, i.student_id, k));
                            }
                        }
                    }
                    return res;
                }*/


        /// <summary>
        /// Актуальный код. Дополняет лист рассылки. По умолчанию вызывается раз в 2 минуты
        /// </summary>
        /// <param name="L"></param>
        public static void AddNotSended(ref List<SendObj> L)
        {
            foreach (var i in Program.students.list)
            {
                foreach (var k in i.Shedule.Keys)
                {
                    foreach (var j in i.Shedule[k])
                    {
                        SendObj ths = new SendObj(j, i.student_id, k);
                        //if (ContData(j) && !Sended.Contains(ths))
                        if (ContData(j) && !MyContains(ref Sended, ths))
                            L.Add(ths);
                    }
                }
            }
        }

        /// <summary>
        /// Моя версия Contains для листа. Стандартный работает не так, как мне надо (сам в шоке)
        /// </summary>
        /// <param name="L"></param>
        /// <param name="O"></param>
        /// <returns></returns>
        public static bool MyContains(ref List<SendObj> L, SendObj O)
        {
            bool res = false;
            foreach (var i in L)
            {
                if (i.name == O.name && i.uid == O.uid && i.SendTime.Date == O.SendTime.Date && i.SendTime.Hour == O.SendTime.Hour && i.SendTime.Minute == O.SendTime.Minute)
                    return true;
            }
            return res;
        }


        /// <summary>
        /// Создать мне тестовое расписание
        /// </summary>
        /// <param name="s"></param>
        /// <param name="uid"></param>
        public static void SetTestShed(ref Students s, long uid)
        {
            Console.WriteLine($"Юзеру {uid} установлено тестовое расписание. Сообщите офицеру безопасности!");
            for (int i = 0; i < s.list.Count; i++)
            {
                DateTime td = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour, test_min, 0);
                DateTime td2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour, test_min + 3, 0);
                DateTime td3 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour + 1, test_min + 3, 0);
                if (s.list[i].student_id == uid)
                {
                    s.list[i].Shedule.Add("Test shedule", new DateTime[] { td, td2, td3 });
                    /*s.list[i].Shedule.Add("Ts2", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts3", new DateTime[] { td});
                    s.list[i].Shedule.Add("Ts4", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts5", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts6", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts7", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts8", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts9", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts10", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts11", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts12", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts13", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts14", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts15", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts16", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts17", new DateTime[] { td });
                    s.list[i].Shedule.Add("Ts18", new DateTime[] { td });*/
                }

            }
        }

    }

}
