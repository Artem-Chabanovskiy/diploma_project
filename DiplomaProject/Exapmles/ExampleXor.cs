using System;
using DiplomaProject.Data;
using SharpML.Recurrent.DataStructs;
using SharpML.Recurrent.Models;
using SharpML.Recurrent.Networks;
using SharpML.Recurrent.Trainer;
using SharpML.Recurrent.Util;

namespace DiplomaProject.Exapmles
{
    internal class ExampleXor
    {
        public static void Run()
        {
            Random rng = new Random();
            DataSet data = new XorDataSetGenerator();

            const int inputDimension = 2;
            const int hiddenDimension = 3;
            const int outputDimension = 1;
            const int hiddenLayers = 1;
            const double learningRate = 0.001;
            const double initParamsStdDev = 0.08;

            INetwork nn = NetworkBuilder.MakeFeedForward(inputDimension,
                hiddenDimension,
                hiddenLayers,
                outputDimension,
                data.GetModelOutputUnitToUse(),
                data.GetModelOutputUnitToUse(),
                initParamsStdDev, rng);


            const int reportEveryNthEpoch = 10;
            const int trainingEpochs = 100000;

            Trainer.train<NeuralNetwork>(trainingEpochs, learningRate, nn, data, reportEveryNthEpoch, rng);

            Console.WriteLine("Training Completed.");
            Console.WriteLine("Test: 1,1");

            Matrix input = new Matrix(new double[] {1, 1});
            Graph g = new Graph(false);
            Matrix output = nn.Activate(input, g);

            Console.WriteLine("Test: 1,1. Output:" + output.W[0]);

            Matrix input1 = new Matrix(new double[] {0, 1});
            Graph g1 = new Graph(false);
            Matrix output1 = nn.Activate(input1, g1);

            Console.WriteLine("Test: 0,1. Output:" + output1.W[0]);

            Console.WriteLine("done.");
        }
    }
}