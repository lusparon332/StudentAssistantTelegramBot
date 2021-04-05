using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAssistantTelegramBot
{
    class Shedule_Sender
    {
        // Класс объекта отправки оповещения
        public class SendObj
        {
            public DateTime SendTime;
            public long uid;
            public string name;

            public SendObj(DateTime DT, long id, string namen)
            {
                SendTime = DT;
                uid = id;
                name = namen;
            }
        }

        public static void Bot_SendWarn()
        {
            List<SendObj> NeedSend = new List<SendObj>();
            while (true)
            {
                DateTime now = DateTime.Now;
                if (now.Minute % 2 == 0 && now.Second == 0 && now.Millisecond == 0)
                {
                    NeedSend = CreateNotSendedArr();
                }

                Bot_SendWarn_core(now, ref NeedSend);
            }
        }



        public static void Bot_SendWarn_core(DateTime now, ref List<SendObj> NeedSend)
        {
            foreach (var i in NeedSend)
            {
                if (now.Hour == i.SendTime.Hour && now.Minute == i.SendTime.Minute && now.Second == i.SendTime.Second && now.Millisecond == i.SendTime.Millisecond)
                {
                    string answer = $"Время подготовки к экзамену по предмету: {i.name}.\n\n";
                    if (Program.students.ContainsStudByID(i.uid, out Student st))
                    {
                        st.prev_loc = st.users_loc;
                        st.users_loc = LevelOfCode.PREPARE_TIME;
                        answer += "Вы планируете сейчас готовиться?\nДа - /yes\nПеренести на 15 минут - /postpone_15_minutes" +
                            "\nПеренести на 30 минут - /postpone_30_minutes\n" +
                            "Перенести на 1 час - /postpone_1_hour\nПеренести на 2 часа - /postpone_2_hours";
                    }

                    Program.Bot.SendTextMessageAsync(i.uid, answer);
                    Console.WriteLine($"Оповещение отправлено пользователю: {i.uid}");
                }
            }

        }

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
        }

    }
}
