using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

public static class Globals
{
    public static List<(int, int)> final_list = new List<(int, int)>();
}


public class ThreadWork
{
    public static void ThreadSearch(List<string> list, int delta, string ToSearch, List<int> index)
    {
        List<(int, int)> result = new List<(int, int)>();
        List<(int, int)> result2 = new List<(int, int)>();

        //finding all the strings that contains ToSearch[0] and saving the original location
        int item_count = 0;
        foreach (string item in list)
        {

            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] == ToSearch[0])
                {
                    result.Add((index[item_count], i));
                }


            }
            item_count++;

        }
        //concatinate all the text into one string
        string text = "";

        foreach (string item in list)
        {
            text = text + item + " ";
        }
        //saving the new location in the concatinate string
        List<int> location = new List<int>();
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ToSearch[0])
            {
                location.Add(i);
            }
        }
        int num = (ToSearch.Length - 1) * delta + ToSearch.Length;
        string search = "";

        int string_counter = 0;
        //every string that contains ToSearch[0] we will search in the concatinate string with delta 
        //if we will get the origenal string we will add it to the resalt

        for (int i = 0; i < location.Count; i++)
        {


            for (int j = location[i]; j < location[i] + num; j = j + 1 + delta)
            {
                if (string_counter == 0)
                {
                    search = search + ToSearch[0];
                }

                if (string_counter != 0 && string_counter < ToSearch.Length && j < text.Length)
                {
                    if (text[j] == ToSearch[string_counter])
                    {
                        search = search + text[j];

                    }
                    else
                    {
                        break;
                    }

                }

                string_counter++;
            }
            if (String.Equals(ToSearch, search))
            {
                result2.Add(result[i]);


            }
            search = "";
            string_counter = 0;
        }

        //adding the result to the final result
        for (int j = 0; j < result2.Count; j++)
        {
            if (!Globals.final_list.Contains(result2[j]))
            {
                Globals.final_list.Add(result2[j]);
            }

        }




    }




}

namespace MultiThreadedSearch
{
    internal class Program
    {


        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                throw new Exception("need to enter 4 arguments");

            }
            //TODO - cheack if file exist
            var textfile = args[0];

            var StringToSearch = args[1];
            string regular_exp = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
            if (!regular_exp.Contains(StringToSearch))
            {
                throw new Exception("is not regular expresion");
            }
            int nThreads = 0;
            int Delta = 0;
            try
            {
                nThreads = int.Parse(args[2]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Third argument is not a number!");
                Console.WriteLine(ex.Message);
            }
            try
            {
                Delta = int.Parse(args[3]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fourth argument is not a number!");
                Console.WriteLine(ex.Message);
            }



            string[] lines = System.IO.File.ReadAllLines(@textfile);
            double val = (double)lines.Length / nThreads;
            int partsNum = (int)Math.Floor(val);
            int counter = 0;
            List<List<string>> data = new List<List<string>>();
            int worst_case = StringToSearch.Length + (StringToSearch.Length - 1) * Delta;
            List<List<int>> index = new List<List<int>>();

            //taking the text and divided into nThread parts that are partly overlaped whit the worst case of a string
            List<string> list = new List<string>();
            List<int> count = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                count.Add(i);
                list.Add(lines[i]);
                counter++;
                if (counter == partsNum)
                {
                    for (int j = i + 1; j <= i + worst_case; j++)
                    {
                        if (j > lines.Length - 1)
                        {
                            break;
                        }
                        list.Add(lines[j]);
                        count.Add(j);
                    }
                    data.Add(list);
                    index.Add(count);
                    List<string> temp = new List<string>();
                    List<int> temp2 = new List<int>();
                    list = temp;
                    count = temp2;

                    counter = 0;

                }

            }
            int x = 0;
            if (lines.Length > nThreads)
            {
                x = nThreads;
            }
            else
            {
                x = lines.Length;
            }



            //every thread is responsible for his part of the text

            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < x; i++)
            {
                Thread thread = new Thread(() => ThreadWork.ThreadSearch(data.ElementAt(i), Delta, StringToSearch, index.ElementAt(i)));

                threads.Add(thread);
                threads.ElementAt(i).Start();
                threads.ElementAt(i).Join();




            }

            for (int i = 0; i < Globals.final_list.Count; i++)
            {
                Console.WriteLine("[" + Globals.final_list[i].Item1 + "," + Globals.final_list[i].Item2 + "]");
            }
            if (Globals.final_list.Count == 0)
            {
                Console.WriteLine("not found");
            }



















        }
    }
}



