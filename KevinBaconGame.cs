using System;
using MathGraph;
using System.IO;

namespace KevinBaconGame
{
    class Program
    {

        /* Some things to note about the project:
         * - There was a line in your graph library that printed a line 
         * $"Search completed in {count} steps" in line 301. I preferred to 
         * comment out this line because I didn't like how it looked. I'm not 
         * sure if your library keeps that in, and if so, I'm not sure if it makes 
         * the colors look odd. 
         * 
         * - Project is completed in a series of functions, but they're quite 
         * easily labled. 
         * 
         * - My "interesting fact" uses a Stack object. I'm not sure if that's 
         * very efficient for just two items in it, but I figured this was the 
         * easiest way to do it. 
         * 
         * Enjoy!
         */





        static void CheckFileName(string file_name)
        {
            if (File.Exists(file_name) == false)
            {
                Console.WriteLine("Could not find the file...");
                Environment.Exit(0);
            }
        }


        static MathGraph<string> CreateGraph(string file_name)
        {
            CheckFileName(file_name);

            MathGraph<string> movies = new MathGraph<string>();

            foreach (string line in System.IO.File.ReadLines(file_name))
            {
                string[] info = line.Split("|");
                string[] drop_nums = info[0].Split(" (");
                info[0] = drop_nums[0];

                if (movies.ContainsVertex(info[0]) == false)
                {
                    movies.AddVertex(info[0]);
                }
                if (movies.ContainsVertex(info[1]) == false)
                {
                    movies.AddVertex(info[1]);
                }

                movies.AddEdge(info[0], info[1]);
            }

            return movies;
        }







        // check if actors are in the graph 
        static bool CheckActors(MathGraph<string> movies, string actor1, string actor2)
        {
            bool result = true;
            if(movies.ContainsVertex(actor1)==false)
            {
                Console.WriteLine($"{actor1} not recognized.");
                result = false;
            }
            if (movies.ContainsVertex(actor2) == false)
            {
                Console.WriteLine($"{actor2} not recognized.");
                result = false;
            }
            return result;
        }


        // check if actors have a connectoin 
        static bool CheckLink(MathGraph<string> movies, string actor1, string actor2)
        {
            if(movies.TestConnectedTo(actor1, actor2) == false)
            {
                Console.WriteLine($"There is no link between {actor1} and {actor2}.");
                return false;
            }
            return true;
        }


        // combines above checks into one T/F bool 
        static bool Checks(MathGraph<string> movies, string actor1, string actor2)
        {
            return CheckActors(movies, actor1, actor2) && CheckLink(movies, actor1, actor2);
        }










        // returns sentences of actor link with colors + fun fact 
        static void PathReturn(MathGraph<string> movies, string actor1, string actor2)
        {
            string[] connection = movies.FindShortestPath(actor1, actor2).ToArray();

            decimal degrees = connection.Length / 2;

            //Math.Round always returns even int when halfway
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Math.Round(degrees)} degrees of seperation between {actor1} and {actor2}.");

            int i = 0;
            while (connection[i] != actor2)
            {
                string first_actor = connection[i];
                string film = connection[i + 1];
                string second_actor = connection[i + 2];

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(first_actor);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" was in ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(film);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" with ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(second_actor);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(".");


                Console.WriteLine();
                i += 2;
            }

            System.Collections.Generic.Stack<string> actor1_movies = new System.Collections.Generic.Stack<string>();
            foreach (string movie in movies.EnumAdjacent(actor1))
            {
                actor1_movies.Push(movie);


                // this stop is meant to not waste memory, but I'm not sure how
                // efficient this process is. 
                if (actor1_movies.Count == 2)
                {
                    break;
                }
            }

            if (actor1_movies.Count > 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Fun Fact: {actor1} has been in {movies.CountAdjacent(actor1)} movies, " +
                    $"including {actor1_movies.Pop()} and {actor1_movies.Pop()}.");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }







        // runs the program 
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string file_name = "";
            // file name must be passed as CLA
            try
            {
                file_name = args[0];
            }
            catch
            {
                Console.WriteLine("No file name argument passed.");
                Environment.Exit(0);
            }

            MathGraph<string> movies = CreateGraph(file_name);
            bool command = false;
            if (args.Length == 2)
            {
                Console.WriteLine("Not enough arguments passed.");
                Console.WriteLine("'{file_name}' '{Actor 1}' '{Actor 2}'");
                Environment.Exit(0);
            }
            else if (args.Length == 3)
            {
                string actor1 = args[1];
                string actor2 = args[2];

                if (Checks(movies, actor1, actor2))
                {
                    PathReturn(movies, actor1, actor2);
                }

                command = true;
            }


            // interactive looper

            if (command == false) { Console.WriteLine("What's up?"); }
                        
            while (command == false)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input.ToLower().Replace(" ","") == "quit" || input.ToLower().Replace(" ","") == "exit")
                {
                    Console.WriteLine("Okay see ya!");
                    break;
                }

                string[] actors = input.Split(" and ");

                if (actors.Length == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Incorrect input: '{actor} and {actor}'");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                try
                {
                    if (Checks(movies, actors[0], actors[1]))
                    {
                        PathReturn(movies, actors[0], actors[1]);
                    }
                }

                catch { }
                
                Console.WriteLine();
                Console.WriteLine("What else? ");
            }
        }
    }
}
