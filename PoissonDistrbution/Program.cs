using Spectre.Console;

namespace PoissonDistribution
{
    public class Program
    {
        const int MaximumX = 60; // Maximum number of x values to fit on screen
        const int NumberOfSteps = 40; // Number of steps on the y-axis
        static double MinProbability = 0.0001; // Minimum probability to be printed

        static void Main()
        {
            // Set the RGB background color
            AnsiConsole.Background = new Color(20, 0, 80); // RGB: 20, 0, 80
            AnsiConsole.Clear(); // Clear the console with the background color

            // Set the complementary text color
            var textColor = new Color(235, 255, 175); // Complementary RGB: 235, 255, 175

            while (true)
            {
                // Input Lambda
                AnsiConsole.MarkupLine("[rgb(235,255,175)]Please choose Lambda value greater or equal 1: [/]");
                double lambda;
                while (!double.TryParse(Console.ReadLine(), out lambda) || lambda < 1)
                {
                    AnsiConsole.MarkupLine("[rgb(235,255,175)]Invalid input. Please choose Lambda value greater or equal 1: [/]");
                }

                AnsiConsole.MarkupLine($"[rgb(235,255,175)]Lambda is {lambda}, and the area under the curve is 1:[/]");

                var poisson = new PoissonDistribution(lambda);
                int maxK = (int)(3 * lambda);

                // Adjust the resolution: determine step size for large Lambda
                int step = 1; // Default step size for smaller Lambda
                if (maxK > MaximumX)
                {
                    step = (int)Math.Ceiling((double)maxK / MaximumX); // Increase step size for larger Lambda
                }

                // Dictionary to store k and y-values (probabilities)
                Dictionary<int, double> valuePerK = new Dictionary<int, double>();

                // Calculate y-values (probabilities) for each k and store them in the dictionary
                double maxY = 0;
                for (int k = 0; k <= maxK; k += step)
                {
                    double probability = poisson.ProbabilityMassFunction(k);
                    valuePerK[k] = probability; // Store k and its corresponding y-value
                    if (probability > maxY)
                    {
                        maxY = probability; // Find the maximum y-value
                    }
                }

                // Calculate the y-axis step size
                double yStep = maxY / NumberOfSteps;

                MinProbability = maxY / NumberOfSteps;
                int startK = maxK;
                double minY = 1;
                bool foundMinY = false;

                foreach (var kvp in valuePerK)
                {
                    if (kvp.Value >= MinProbability && !foundMinY)
                    {
                        if (kvp.Value < minY)
                        {
                            minY = kvp.Value;
                            startK = kvp.Key;
                            foundMinY = true; // Ensure we update startK only once when minY is found
                        }
                    }
                }

                startK = startK / 2;

                // Print each y-level from maxY to 0, and check values from the dictionary for 'x' or blanks
                for (double y = maxY; y >= MinProbability; y -= yStep)
                {
                    AnsiConsole.Markup($"[rgb(235,255,175)]{y:F4} |[/]"); // Print the y-value on the left side
                    foreach (var kvp in valuePerK)
                    {
                        if (kvp.Key < startK) continue;

                        int k = kvp.Key;
                        double probability = kvp.Value;
                        if (Math.Abs(probability - y) < yStep / 2)
                        {
                            AnsiConsole.Markup("[rgb(235,255,175)]  x [/]");
                        }
                        else
                        {
                            AnsiConsole.Markup("[rgb(235,255,175)]    [/]");
                        }
                    }
                    AnsiConsole.WriteLine();
                }

                // Print footer (x-axis labels)
                AnsiConsole.Markup("[rgb(235,255,175)]       [/]");
                if (startK > 0)
                {
                    maxK = maxK - 2 * startK;
                }

                for (int k = startK; k <= maxK; k += step)
                {
                    AnsiConsole.Markup($"[rgb(235,255,175)]{k,4}[/]"); // Print the x-values (k-values), adjust for the step
                }
                AnsiConsole.WriteLine();
            }
        }
    }
}
