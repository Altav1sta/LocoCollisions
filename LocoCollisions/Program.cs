using System;
using System.IO;
using System.Collections.Generic;

namespace LocoCollision
{
    class Program
    {
        struct Passage
        {
            public int station1;
            public int station2;
            public int distance;

            public Passage(int s1, int s2, int d)
            {
                if (s1 < s2)
                {
                    station1 = s1;
                    station2 = s2;
                }
                else
                {
                    station1 = s2;
                    station2 = s1;
                }
                distance = d;
            }
        }

        static void Main(string[] args)
        {
            List<Passage> allPassages = new List<Passage>();
            // Заполнение "сети" начальными данными
            using (StreamReader file = new StreamReader(string.Format(@"{0}\..\..\input\Web.txt", Environment.CurrentDirectory)))
            {
                string s;
                while ((s = file.ReadLine()) != null)
                {
                    string[] arr = s.Trim().Split(' ');
                    if (arr.Length != 3)
                    {
                        Console.WriteLine("Ошибка формата входных данных файла Web.txt");
                        Console.ReadLine();
                        return;
                    }
                    int s1, s2, d;
                    if (Int32.TryParse(arr[0], out s1) && Int32.TryParse(arr[1], out s2) && Int32.TryParse(arr[2], out d))
                    {
                        Passage p = new Passage(s1, s2, d);
                        if (!allPassages.Contains(p)) allPassages.Add(p);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка формата входных данных файла Web.txt");
                        Console.ReadLine();
                        return;
                    }
                }
            }

            List<string> locos = new List<string>();
            //Заполнение массива маршрутов поездов, где каждый маршрут набор станций, и массива времен прибывания на них
            using (StreamReader file = new StreamReader(string.Format(@"{0}\..\..\input\Locos.txt", Environment.CurrentDirectory)))
            {
                string s;
                while ((s = file.ReadLine()) != null)
                {
                    locos.Add(s.Trim());
                }
            }
            int[][] stations = new int[locos.Count][];
            int[][] time = new int[locos.Count][];
            int counter = 0;
            foreach (string s in locos)
            {
                string[] tmp = s.Split(' ');
                stations[counter] = new int[tmp.Length];
                time[counter] = new int[tmp.Length];
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (!Int32.TryParse(tmp[i], out stations[counter][i]))
                    {
                        Console.WriteLine("Ошибка формата входных данных Locos.txt");
                        Console.ReadLine();
                        return;
                    }
                    if (i == 0) time[counter][i] = 0;
                    else
                    {
                        time[counter][i] = time[counter][i - 1] + allPassages.Find(item => (item.station1 == stations[counter][i - 1]) && (item.station2 == stations[counter][i]) ||
                            (item.station2 == stations[counter][i - 1]) && (item.station1 == stations[counter][i])).distance;
                    }
                }
                counter++;
            }

            // Выводим расписание движения паровозов
            Console.WriteLine("Ниже представлено расписание движения поездов по станциям. Первая цифра номер станции, цифра в скобках время прибытия. ");
            for (int i = 0; i < locos.Count; i++)
            {
                Console.Write("Паровоз " + (i + 1) + ":  ");
                for (int j = 0; j < stations[i].Length - 1; j++)
                {
                    Console.Write(stations[i][j] + "(" + time[i][j] + ") - ");
                }
                Console.WriteLine(stations[i][stations[i].Length - 1] + "(" + time[i][stations[i].Length - 1] + ")");
            }
            Console.WriteLine();

            // Проверяем все станции на предмет столкновения
            for (int i = 0; i < locos.Count - 1; i++)
            {
                for (int j = 0; j < stations[i].Length; j++)
                {
                    for (int k = i + 1; k < locos.Count; k++)
                    {
                        for (int l = 0; l < stations[k].Length; l++)
                        {
                            if ((stations[i][j] == stations[k][l]) && (time[i][j] == time[k][l]))
                                Console.WriteLine("Паровозы " + (i + 1) + " и " + (k + 1) + " cталкиваются на станции " + stations[i][j]);
                        }
                    }
                }
            }

            // Проверяем все переезды на предмет столкновения
            for (int i = 0; i < locos.Count - 1; i++)
            {
                for (int j = 0; j < stations[i].Length - 1; j++)
                {
                    for (int k = i + 1; k < locos.Count; k++)
                    {
                        for (int l = 0; l < stations[k].Length - 1; l++)
                        {
                            if ((stations[i][j] == stations[k][l + 1]) && (stations[i][j + 1] == stations[k][l]) &&
                                ((time[i][j] >= time[k][l]) && (time[i][j] <= time[k][l + 1]) || (time[i][j + 1] >= time[k][l]) && (time[i][j + 1] <= time[k][l + 1])))
                                Console.WriteLine("Паровозы " + (i + 1) + " и " + (k + 1) + " cталкиваются на переезде между станциями " + stations[i][j] + " и " + stations[i][j + 1]);
                        }
                    }
                }
            }

            Console.ReadLine();

        }
    }
}
