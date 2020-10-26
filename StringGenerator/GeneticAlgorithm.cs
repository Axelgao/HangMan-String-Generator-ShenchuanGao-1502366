using System;
using System.Collections.Generic;
using System.Linq;


namespace StringGenerator
{
    public class GeneticAlgorithm
    {
        Random random = new Random();
        public string CalculateBestSolution(int length, string geneSet, Func<string, int> fitness, 
            Action<string, int, int, string> showSolution, int popSize, string thread, int mutationSize, 
            ref string tempBestStringToShare, ref int tempBestFitnessToShare, 
            ref string tempBestStringFromOtherThread, ref int tempBestFitnessFromOtherThread)
        {

            // Initial current best slution
            string parentCurrentBest = "";
            int parentFitnessCurrentBest = 0;

            // Initial random solutions arrays
            string[] parentsString = new string[popSize];
            int[] parentsFitness = new int[popSize];
            for (int i = 0; i < popSize; i++)
            {
                string parentString = CalculateSequence(length, geneSet);
                int parentFitness = fitness(parentString);

                // Add solution into arrays
                parentsString[i] = parentString;
                parentsFitness[i] = parentFitness;

                if (i == 0)
                {
                    // Initial first current best solution
                    parentCurrentBest = parentString;
                    parentFitnessCurrentBest = parentFitness;
                }
                else 
                {
                    // Compare fitness to current best solution
                    if (parentFitness >= parentFitnessCurrentBest)
                    {
                        // Update current best solution
                        parentCurrentBest = parentString;
                        parentFitnessCurrentBest = parentFitness;
                    }
                }
            }

            // Show current best solution, fitness and generation
            int generation = 1;

            tempBestStringToShare = parentCurrentBest;
            tempBestFitnessToShare = parentFitnessCurrentBest;

            showSolution(parentCurrentBest, parentFitnessCurrentBest, generation, thread);

            // Flag to check this thread's best solution is better or the other thread's 
            bool tempLeadingInThreads = true;

            // Start calculation loop
            while (parentFitnessCurrentBest != length)
            {
                // Initial empty offspring arrays
                string[] offspringsString = new string[popSize];
                int[] offspringsFitness = new int[popSize];

                // Start recombination the offsprings
                for (int i = 0; i < popSize; i++)
                {
                    char[] offspringChars = new char[length];

                    for (int j = 0; j < length; j++)
                    {
                        // Randomly choose the parent to transmit the character
                        int parentChosenLocation = random.Next(0, popSize);
                        char[] parentChosenChars = parentsString[parentChosenLocation].ToCharArray();
                        offspringChars[j] = parentChosenChars[j];
                    }

                    string offspringGenerated = new String(offspringChars);

                    // Mutate the offspring 
                    string offspring = Mutate(offspringGenerated, geneSet, ref random, mutationSize);

                    // Check if offspring is identical from parents
                    bool identical = false;
                    while (!identical)
                    {
                        identical = true;

                        for (int j = 0; j < popSize; j++)
                        {
                            if (offspring == parentsString[j])
                            {
                                identical = false;
                                // Mutate the offspring 
                                offspring = Mutate(offspringGenerated, geneSet, ref random, mutationSize);
                            }
                        }
                    }

                    // Calculate offspring fitness
                    int offspringFitness = fitness(offspring);

                    offspringsString[i] = offspring;
                    offspringsFitness[i] = offspringFitness;


                    //// For S-String Display!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //// Share the temp best solution every generation
                    //if (offspringFitness > parentFitnessCurrentBest)
                    //{
                    //    parentCurrentBest = offspring;
                    //    parentFitnessCurrentBest = offspringFitness;

                    //    tempBestStringToShare = parentCurrentBest;
                    //    tempBestFitnessToShare = parentFitnessCurrentBest;

                    //    showSolution(parentCurrentBest, parentFitnessCurrentBest, generation, thread);

                    //    if (tempBestFitnessToShare < tempBestFitnessFromOtherThread)
                    //    {
                    //        // Flag to update the other thread's best solution into this thread
                    //        tempLeadingInThreads = false;
                    //    }
                    //}



                    // For M-String and L-String!!!!!!!!!!!!!!!!!!!!!!!!!
                    // Share the temp best solution every 10 generations
                    if (offspringFitness > parentFitnessCurrentBest)
                    {
                        parentCurrentBest = offspring;
                        parentFitnessCurrentBest = offspringFitness;
                    }

                }


                // For M-String and L-String Display!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // Share the temp best solution every 100 generations
                string genString = generation.ToString();
                if (genString.Length > 2)
                {
                    if (genString.Substring(genString.Length - 2) == "00")
                    {
                        tempBestStringToShare = parentCurrentBest;
                        tempBestFitnessToShare = parentFitnessCurrentBest;
                        showSolution(parentCurrentBest, parentFitnessCurrentBest, generation, thread);

                        if (tempBestFitnessToShare < tempBestFitnessFromOtherThread)
                        {
                            // Flag to update the other thread's best solution into this thread
                            tempLeadingInThreads = false;
                        }
                    }
                }




                // Selection in Plus strategy
                string[] allSolutionsString = new string[popSize * 2];
                int[] allSolutionsFitness = new int[popSize * 2];
 
                // Add parents into calculation arrays
                for (int i = 0; i < popSize; i++)
                {
                    allSolutionsString[i] = parentsString[i];
                    allSolutionsFitness[i] = parentsFitness[i];
                }

                // Add offsprings into calculation arrays
                for (int i = 0; i < popSize; i++)
                {
                    allSolutionsString[i + popSize] = offspringsString[i];
                    allSolutionsFitness[i + popSize] = offspringsFitness[i];
                }

                // Order all the solutions by fitness desending
                string tempString;
                int tempFitness;
                for (int i = 0; i < allSolutionsFitness.Length - 1; i++)
                {
                    for (int j = i + 1; j < allSolutionsFitness.Length; j++)
                    {
                        if (allSolutionsFitness[i] < allSolutionsFitness[j])
                        {
                            tempString = allSolutionsString[i];
                            tempFitness = allSolutionsFitness[i];

                            allSolutionsString[i] = allSolutionsString[j];
                            allSolutionsFitness[i] = allSolutionsFitness[j];

                            allSolutionsString[j] = tempString;
                            allSolutionsFitness[j] = tempFitness;
                        }
                    }
                }

                // Select best solutions
                for (int i = 0; i < popSize; i++)
                {
                    parentsString[i] = allSolutionsString[i];
                    parentsFitness[i] = allSolutionsFitness[i];
                }

                // If the leading flag is false,
                // replace the last chosen solution by the leading solution from the other thread
                if (!tempLeadingInThreads)
                {
                    parentsString[popSize - 1] = tempBestStringFromOtherThread;
                    parentsFitness[popSize - 1] = tempBestFitnessFromOtherThread;

                    Console.WriteLine(thread + " synchronize temp best solution from other thread!!!!!!!!!!!!!!!!!!!!!!!");

                    parentCurrentBest = tempBestStringFromOtherThread;
                    parentFitnessCurrentBest = tempBestFitnessFromOtherThread;

                    tempLeadingInThreads = true;
                }

                generation++;
            }

            showSolution(parentCurrentBest, parentFitnessCurrentBest, generation, thread);

            return parentCurrentBest;
        }

        public string Mutate(string oringinal, string geneSet, ref Random r, int mutationSize)
        {
            string res = oringinal;

            for (int i = 0; i < mutationSize; i++)
            {
                // Randomly choose the mutation location
                int mutateLocation = random.Next(0, oringinal.Length);

                if (mutateLocation == 0)
                {
                    r = new Random();
                }

                // operate the mutatiion
                char[] oringinalChars = oringinal.ToCharArray();
                oringinalChars[mutateLocation] = geneSet[random.Next(0, geneSet.Length)];

                res = new String(oringinalChars);
            }

            return res;
        }

        // Calculate solution sequence
        public string CalculateSequence(int length, string geneSet)
        {
            Func<char> next = () => geneSet[random.Next(0, geneSet.Length)];

            char ini = next();
            char[] sequenceArray = ini.EnumGenerate(next).Take(length).ToArray();
            string res = new String(sequenceArray);

            return res;
        }
    }

    public static class TExtensions
    {
        // Extensions for char type
        public static IEnumerable<T> EnumGenerate<T>(this T ini, Func<T> next)
        {
            var current = ini;

            while (1 == 1)
            {
                yield return current;
                current = next();
            }
        }
    }
}
