using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Telegram.Bot;

namespace StudentAssistantTelegramBot
{
    // для опознования, в каком меню находится пользователь
    public enum LevelOfCode { MAIN_MENU = 0, STUDY_MENU, FAN_MENU, MAKE_EXAM_NAME, MAKE_EXAM_CNT, MAKE_EXAM_DATE, PREPARE_TIME, FAN_JANR, QUESTIONS_MENU };

    public class Students
    {
        public List<Student> list; // список студентов

        // констурктор
        public Students()
        {
            this.list = new List<Student>();
        }

        // получить список студентов из файла
        public void ReadStudentsList(string path)
        {
            try
            {
                var lines = File.ReadAllLines(path, Encoding.UTF8);
                if (lines.Length == 0)
                {
                    Console.WriteLine($"Файл {path} пуст.");
                    return;
                }
                Console.WriteLine($"Идёт чтение студентов из {path}...");
                foreach (var line in lines)
                {
                    var sp_line = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var id = long.Parse(sp_line[0]);
                    var loc = int.Parse(sp_line[1]);
                    var st = new Student(id, (LevelOfCode)loc);

                    this.list.Add(st);
                }
                Console.WriteLine($"Чтение завершено успешно. Из файла {path} получено студентов: {this.list.Count}");
            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        // записать стундентов в файл
        public void WriteStudentsList(string path)
        {
            if (this.list.Count == 0)
            {
                Console.WriteLine("Студентов нет.");
                return;
            }
            try
            {
                Console.WriteLine($"Идёт запись студентов в {path}...");

                var to_file = new List<string>();
                foreach (var elem in list)
                    to_file.Add($"{elem.student_id} {(int)elem.users_loc}");
                File.WriteAllLines(path, to_file, Encoding.UTF8);
                Console.WriteLine($"Запись студентов в {path} успешно завершена. Записано студентов: {list.Count}");
            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        // проверить наличие студента в списке по id
        // если нет - добавить
        public bool ContainsStudByID(long id, out Student getStud)
        {
            foreach (var st in this.list)
                if (st.student_id == id)
                {
                    getStud = st;
                    return true;
                }
            getStud = new Student(id, LevelOfCode.MAIN_MENU);
            Program.students.list.Add(getStud);
            return false;
        }
    }

    // класс студента для корректного ответа на сообщения в дальнейшем
    public class Student
    {
        public long student_id; // id пользователя
        public LevelOfCode users_loc; // уровень вложенности кода для пользователя
        public Exam current_exam; // ближайший экзамен студента
        public Dictionary<string, DateTime[]> Shedule; // расписание, составленное для пользователя
        public LevelOfCode prev_loc;
        public int success;

        public Student(long id, LevelOfCode loc)
        {
            this.student_id = id;
            this.users_loc = loc;
            this.current_exam = new Exam();
            this.Shedule = new Dictionary<string, DateTime[]>();
            this.success = 0;
        }

    }

    // класс экзамен
    public class Exam
    {
        public string name; // название предмета
        public int question_cnt; // количество вопросов
        public DateTime date; // дата экзамена

        public Exam()
        {
            this.name = "name";
            this.question_cnt = 1;
            this.date = DateTime.Now.AddDays(77777);
        }
    }
}
