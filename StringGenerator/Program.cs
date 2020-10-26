using StringGenerator;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace StringGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Enter target string here

            //// S-string
            //string TargetString = "Metaheuristic Algorithms";

            //// M-string
            //string TargetString = "This paper explores common metaheuristic algorithms such as variable neighbourhood search and mimetic algorithms";

            // L-string
            string TargetString = "Paper Description: This paper explores common metaheuristic algorithms such as variable neighbourhood search and mimetic algorithms, and their application in areas such as science, engineering and health informatics.";

            // Target string length
            Console.WriteLine("Target string length: " + TargetString.Length.ToString());

            // Start generation
            new Generator().Generate(TargetString);

            // Waiting the final result reading
            Console.ReadLine();
        }
    }
}

public class Generator
{
    public string Generate(string target)
    {
        // Choose the genetic algorithm here
        GeneticAlgorithm calculatorGA = new GeneticAlgorithm();

        // Calculate parameter length
        int length = target.Length;

        // Calculate parameter geneSet
        char[] geneSetArray = target.Distinct().ToArray();
        string geneSet = new String(geneSetArray);

        // Calculating time
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Show solution action
        Action<string, int, int, string> showCurrentBestSolution = (solution, fit, generation, thread) =>
            Console.WriteLine("Current " + thread + " Best: \r\n" + solution + " \r\n--Fitness: " + fit.ToString() + " Generation: " + generation.ToString() + " Elapsed: " + sw.Elapsed);

        // Calculate parameter fitness
        Func<string, int> fitness = offspring =>
        {
            int res = Enumerable.Range(0, length).Count(x => offspring[x] == target[x]);

            return res;
        };

        // Set pop size parameters
        int popSizeA = 10;
        int popSizeB = 15;

        // Set mutation size parameters
        int mutationSizeA = 1;
        int mutationSizeB = 1;

        // Parallel calculation results
        string resA = "";
        string resB = "";

        // Thread label in result display
        string threadA = "Thread A";
        string threadB = "Thread B";

        // Temp best solution for sharing results between threads
        string tempBestSolutionStringA = "";
        int tempBestSolutionFitnessA = 0;

        string tempBestSolutionStringB = "";
        int tempBestSolutionFitnessB = 0;


        // Parallel parameter
        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 2
        };

        // Parallel calculation for two different popSize Genetic Algorithms
        Parallel.For(0, 2, options, i =>
        {
            if (i == 0)
            {
                // Thread A: Pop size 20
                resA = calculatorGA.CalculateBestSolution(length, geneSet, fitness, showCurrentBestSolution, popSizeA, threadA, mutationSizeA,
                    ref tempBestSolutionStringA, ref tempBestSolutionFitnessA, ref tempBestSolutionStringB, ref tempBestSolutionFitnessB);
                // Thread complete notification
                Console.WriteLine("==============================Thread A calculation done!");
            }
            else if (i == 1)
            {
                // Thread B: Pop size 10
                resB = calculatorGA.CalculateBestSolution(length, geneSet, fitness, showCurrentBestSolution, popSizeB, threadB, mutationSizeB,
                    ref tempBestSolutionStringB, ref tempBestSolutionFitnessB, ref tempBestSolutionStringA, ref tempBestSolutionFitnessA);
                // Thread complete notification
                Console.WriteLine("==============================Thread B calculation done!");
            }
        });

        return resA;
    }
}