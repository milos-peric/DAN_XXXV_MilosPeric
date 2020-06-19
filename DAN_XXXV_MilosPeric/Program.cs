using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXV_MilosPeric
{
    class Program
    {
        private static readonly object obj = new object();
        private static Random random = new Random();
        private static List<Thread> threadListGlobal = new List<Thread>();
        private static int counter = 0;
        private static bool gameNotOver = true;
        private static List<Thread> globalThreads = new List<Thread>();
        private static int numberOfThreads = 0;
        private static int numberToGuess = 0;

        /// <summary>
        /// Generates random number 1 to 100
        /// </summary>
        /// <returns>Allows asignment to int variable.</returns>
        public static int GenerateRandomNumber()
        {
            int randomNumber = random.Next(1, 101);
            return randomNumber;
        }

        /// <summary>
        /// Logic of guessing a number that is Thread Safe.
        /// Using lock(object) to achieve this.
        /// </summary>
        /// <param name="numberObject"></param>
        public static void GuessANumber(object numberObject)
        {
            int timeMiliseconds = 100;
            int randomNumber = GenerateRandomNumber();
            int numberToGuess = (int)numberObject;
            bool numberToGuessParity = numberToGuess % 2 == 0;
            bool randomNumberParity = randomNumber % 2 == 0;
            while (gameNotOver)
            {
                lock (obj)
                {
                    Console.WriteLine(counter--);
                    if (counter <= 0)
                    {
                        Console.WriteLine($"Igra je zavrsena. Nema pobednika. Trazeni broj je bio {numberToGuess}");
                        gameNotOver = false;
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                    else
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name} je pokusao da pogodi parnost broja: {randomNumber}");
                        if (numberToGuessParity == randomNumberParity)
                        {
                            Console.WriteLine($"{Thread.CurrentThread.Name} je pogodio parnost broja.");
                            if (randomNumber == numberToGuess)
                            {
                                Console.WriteLine($"{Thread.CurrentThread.Name} je pobedio, a traženi broj je bio {numberToGuess}!");
                                gameNotOver = false;
                                Console.WriteLine("Igra je zavrsena.");
                                Console.ReadLine();
                                Main();
                            }
                        }
                        Thread.Sleep(timeMiliseconds);
                    }
                }
            }
        }

        /// <summary>
        /// Creates number of new threads equal to numberOfThreads variable
        /// </summary>
        /// <param name="numberOfThreads"></param>
        /// <returns></returns>
        public static List<Thread> CreateThreads(int numberOfThreads)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 1; i <= numberOfThreads; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(GuessANumber))
                {
                    Name = string.Format("Ucesnik_{0}", i)
                };
                threads.Add(t);
            }
            return threads;
        }

        /// <summary>
        /// Allows user to select numbers and starts threads.
        /// </summary>
        public static void SelectNumberOfContesters()
        {
            Console.WriteLine("How many contesters would you like");



            bool isNotCorrectNumberContesters = true;
            do
            {
                string Contesters = Console.ReadLine();
                bool isNumber = int.TryParse(Contesters, out int threadsNumberOutput);
                if (isNumber == false)
                {
                    Console.WriteLine("You must enter a number. ERROR: first entry is not a number.");
                }
                else
                {
                    numberOfThreads = threadsNumberOutput;
                    isNotCorrectNumberContesters = false;
                }
            } while (isNotCorrectNumberContesters);

            Console.WriteLine("Now write a number you would like contesters to guess (1-100)");
            bool isNotCorrectNumber = true;

            do
            {
                string numberString = Console.ReadLine();
                bool isNumber = int.TryParse(numberString, out int numberToGuessOutput);
                if (isNumber == false)
                {
                    Console.WriteLine("You must enter a number. ERROR: second entry is not a number.");
                }
                else
                {
                    if (numberToGuessOutput > 0 && numberToGuessOutput < 101)
                    {
                        numberToGuess = numberToGuessOutput;
                        isNotCorrectNumber = false;
                    }
                    else
                    {
                        Console.WriteLine("Wrong range");
                    }
                }
            } while (isNotCorrectNumber);

            List<Thread> myList = CreateThreads(numberOfThreads);
            threadListGlobal = myList;
            counter = threadListGlobal.Count;
            Console.WriteLine($"You have entered number of contesters: {numberOfThreads} and number to guess is: {numberToGuess}.");
            myList.ForEach(t => t.Start(numberToGuess));
        }

        static void Main()
        {
            Thread startingThread = new Thread(new ThreadStart(SelectNumberOfContesters));
            startingThread.Name = "First thread";
            startingThread.Start();
            startingThread.Join();
            Console.ReadKey();
        }
    }
}
