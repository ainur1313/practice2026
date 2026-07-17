using System;
using System.Threading;

namespace task14
{
    public static class OneThread
    {
        public static double Solve(double a, double b, Func<double, double> function, double step)
        {
            double segmentLength = b - a;
            if (segmentLength <= 0) return 0.0;

            int stepsCount = Math.Max(1, (int)Math.Ceiling(segmentLength / step));
            double actualStep = segmentLength / stepsCount;
            double result = 0.0;

            for (int i = 0; i < stepsCount; i++)
            {
                double xLeft = a + i * actualStep;
                double xRight = xLeft + actualStep;
                double fLeft = function(xLeft);
                double fRight = function(xRight);
                result += (fLeft + fRight) / 2.0 * actualStep;
            }

            return result;
        }
    }
    public class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsCount)
        {
            if (threadsCount < 1)
                throw new ArgumentException("Количество потоков должно быть положительным", nameof(threadsCount));

            double totalSum = 0.0;
            int activeThreads = threadsCount;
            double segmentWidth = (b - a) / threadsCount;
            Thread[] workerThreads = new Thread[threadsCount];
            double[] partialSums = new double[threadsCount];

            using (var threadBarrier = new Barrier(threadsCount + 1))
            {
                for (int i = 0; i < threadsCount; i++)
                {
                    int threadIndex = i;
                    workerThreads[i] = new Thread(() =>
                    {
                        double leftBound = a + threadIndex * segmentWidth;
                        double rightBound = (threadIndex == threadsCount - 1) ? b : leftBound + segmentWidth;

                        double localResult = ComputeSegment(leftBound, rightBound, function, step);

                        partialSums[threadIndex] = localResult;
                        threadBarrier.SignalAndWait();
                    });

                    workerThreads[i].Start();
                }

                threadBarrier.SignalAndWait();
            }

            foreach (var value in partialSums)
                totalSum += value;

            return totalSum;
        }

        private static double ComputeSegment(double start, double end, Func<double, double> function, double step)
        {
            double segmentLength = end - start;

            if (segmentLength <= 0)
                return 0.0;

            int stepsCount = Math.Max(1, (int)Math.Ceiling(segmentLength / step));
            double actualStep = segmentLength / stepsCount;

            double result = 0.0;
            for (int i = 0; i < stepsCount; i++)
            {
                double xLeft = start + i * actualStep;
                double xRight = xLeft + actualStep;
                double fLeft = function(xLeft);
                double fRight = function(xRight);
                result += (fLeft + fRight) / 2.0 * actualStep;
            }

            return result;
        }
    }
}
