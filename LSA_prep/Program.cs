using LSA_Base;
using MathNet.Numerics.LinearAlgebra;

class Program
{//TODO: Refactor using the accurate terminology:
    // Включен ход - Linked Traverse
    // Затворен ход - Closed Traverse
    // TODO: UNIT TESTING -> Add automated testing mechanisms and prepare automated tests
    // TODO: Look to apply the SOLID principles - especially the S, I principles. Ensure the code is clean!
    //simo e gotin :) :)
    static void Main(string[] args)
    {
        //HashSet<int> set1 = new HashSet<int>();
        //HashSet<int> set2 = new HashSet<int>();
        //set1.Add(1);
        //set1.Add(2);
        //set2.Add(2);
        //set2.Add(1);
        //bool result = set1.SetEquals(set2);
        //Console.WriteLine(result);
        NewGravimetricPoint gp1 = new("1", 27629.2642, 340966.7408);
        NewGravimetricPoint gp2 = new("2", 27226.8512, 341323.5681);
        NewGravimetricPoint gp3 = new("3", 27201.8000, 341009.7822);
        //NewGravimetricPoint gp3 = new("3", 27039.5212, 341009.7822);
        NewGravimetricPoint gp7 = new("7", 27472.4619, 340382.2104);
        KnownGravimetricPoint gp4 = new("4", 27201.8739, 340834.8395, 9);
        KnownGravimetricPoint gp5 = new("5", 27389.2041, 340376.6567, 8);
        KnownGravimetricPoint gp8 = new("8", 27404.4680, 340280.8549, 8.5);
        KnownGravimetricPoint gp9 = new("9", 27133.3325, 341704.3286, 9.5);
        KnownGravimetricPoint gp10 = new("10", 27736.1762, 340572.0287, 10);
        NewGravimetricPoint gp11 = new("11", 27230.5212, 341000.7822);

        //RelativeMeasurement m1 = new(gp4, gp1, 0.052);
        //RelativeMeasurement m2 = new(gp1, gp2, -0.022);
        //RelativeMeasurement m3 = new(gp1, gp3, -0.011);
        //RelativeMeasurement m4 = new(gp2, gp3, 0.0097);
        //RelativeMeasurement m5 = new(gp3, gp4, -0.039);
        //RelativeMeasurement m6 = new(gp4, gp5, -1.02);
        //RelativeMeasurement m7 = new(gp5, gp7, 1);
        //RelativeMeasurement m8 = new(gp7, gp1, 0.07);
        //RelativeMeasurement m9 = new(gp5, gp8, 0.49);
        //RelativeMeasurement m10 = new(gp8, gp7, 0.49);
        //RelativeMeasurement m11 = new(gp2, gp9, 0.5);
        //RelativeMeasurement m12 = new(gp9, gp3, -0.5);
        //RelativeMeasurement m13 = new(gp4, gp2, 0.033);
        //RelativeMeasurement m14 = new(gp8, gp9, 1);
        //RelativeMeasurement m15 = new(gp3, gp5, -1.05);
        //RelativeMeasurement m16 = new(gp7, gp10, 1);
        //RelativeMeasurement m17 = new(gp1, gp10, 0.94);
        //RelativeMeasurement m18 = new(gp10, gp3, -1);
        //RelativeMeasurement m19 = new(gp3, gp11, -0.38);
        //RelativeMeasurement m20 = new(gp11, gp8, -0.52);

        RelativeMeasurement m1 = new(gp4, gp1, 0.052, 447.281);
        RelativeMeasurement m2 = new(gp1, gp2, -0.022, 537.831);
        RelativeMeasurement m3 = new(gp1, gp3, -0.011, 429.626);
        RelativeMeasurement m4 = new(gp2, gp3, 0.0097, 314.784);
        RelativeMeasurement m5 = new(gp3, gp4, -0.039, 174.943);
        RelativeMeasurement m6 = new(gp4, gp5, -1.02, 494.999);
        RelativeMeasurement m7 = new(gp5, gp7, 1, 83.443);
        RelativeMeasurement m8 = new(gp7, gp1, 0.07, 605.196);
        RelativeMeasurement m9 = new(gp5, gp8, 0.49, 97.01);
        RelativeMeasurement m10 = new(gp8, gp7, 0.49, 122.05);
        RelativeMeasurement m11 = new(gp2, gp9, 0.5, 392.077);
        RelativeMeasurement m12 = new(gp9, gp3, -0.5, 697.913);
        RelativeMeasurement m13 = new(gp4, gp2, 0.033, 489.366);
        RelativeMeasurement m14 = new(gp8, gp9, 1, 1449.066);
        RelativeMeasurement m15 = new(gp3, gp5, -1.05, 660.279);
        RelativeMeasurement m16 = new(gp7, gp10, 1, 324.925);
        RelativeMeasurement m17 = new(gp1, gp10, 0.94, 408.935);
        RelativeMeasurement m18 = new(gp10, gp3, -1, 690.787);
        RelativeMeasurement m19 = new(gp3, gp11, -0.38, 30.098);
        RelativeMeasurement m20 = new(gp11, gp8, -0.52, 740.644);

        //NewGravimetricPoint gp2 = new("2", 27448.6326, 340661.0996);
        //NewGravimetricPoint gp3 = new("3", 27343.8182, 340696.2238);
        //NewGravimetricPoint gp4 = new("4", 27400.675, 340781.3131);
        //NewGravimetricPoint gp6 = new("6", 27364.0889, 340867.3919);
        //NewGravimetricPoint gp7 = new("7", 27532.1874, 340761.525);
        //NewGravimetricPoint gp8 = new("8", 27533.1761, 340888.1693);

        //KnownGravimetricPoint gp1 = new("1", 27397.7086, 340571.5581, 9);
        //KnownGravimetricPoint gp5 = new("5", 27463.4648, 340833.7519, 8.5);

        //RelativeMeasurement m1 = new(gp1, gp2, 0.3);
        //RelativeMeasurement m2 = new(gp1, gp3, 0.1);
        //RelativeMeasurement m3 = new(gp2, gp3, -0.22);
        //RelativeMeasurement m4 = new(gp2, gp4, -0.7);
        //RelativeMeasurement m5 = new(gp2, gp7, -0.65);
        //RelativeMeasurement m6 = new(gp3, gp4, -0.47);
        //RelativeMeasurement m7 = new(gp3, gp6, -0.09);
        //RelativeMeasurement m8 = new(gp4, gp6, 0.38);
        //RelativeMeasurement m9 = new(gp4, gp5, -0.08);
        //RelativeMeasurement m10 = new(gp5, gp6, 0.53);
        //RelativeMeasurement m11 = new(gp5, gp7, 0.17);
        //RelativeMeasurement m12 = new(gp5, gp8, 0.24);
        //RelativeMeasurement m13 = new(gp6, gp8, -0.23);
        //RelativeMeasurement m14 = new(gp7, gp8, 0.11);
        //var graph = new NetworkAnalyzer<RelativeMeasurement>();
        //graph.MeasurementToEdge(m2);
        //graph.MeasurementToEdge(m3);
        //graph.MeasurementToEdge(m1);
        //graph.MeasurementToEdge(m4);
        //graph.MeasurementToEdge(m5);
        //graph.MeasurementToEdge(m6);
        //graph.MeasurementToEdge(m7);
        //graph.MeasurementToEdge(m8);
        //graph.MeasurementToEdge(m9);
        //graph.MeasurementToEdge(m10);
        //graph.MeasurementToEdge(m11);
        //graph.MeasurementToEdge(m12);
        //graph.MeasurementToEdge(m13);
        //graph.MeasurementToEdge(m14);
        //graph.MeasurementToEdge(m15);
        //graph.MeasurementToEdge(m16);
        //graph.MeasurementToEdge(m17);
        //graph.MeasurementToEdge(m18);
        //graph.MeasurementToEdge(m19);
        //graph.MeasurementToEdge(m20);

        //List<List<PointBase>> distinctPaths = graph.FindAllDistinctTraverses();
        //foreach(var key in distinctPaths.Keys)
        //{
        //    List<GravimetricPoint> path = distinctPaths[key];
        //    List<string> pathNumbers = new();

        //    foreach(var point in path)
        //    {
        //        pathNumbers.Add(point.Number);
        //    }
        //    distinctPathsList.Add(path);
        //    Console.WriteLine(String.Join(" -> ", pathNumbers));
        //}

        List<RelativeMeasurement> meas = new() { m1, m2, m13 };
        //GravimetricAdjustment adjustment = new(distinctPaths, meas);
        //Matrix<double> confM = adjustment.CreateConfigurationMatrix();
        //Vector<double> inaccuracies = adjustment.CalculateInnacuraciesVector(adjustment.AssignedApproxMeasurements);
        //Matrix<double> weightMatrix = adjustment.CreateWeightMatrix();
        //Matrix<double> normalMatrix = adjustment.CalculateNormalMatrix(confM, weightMatrix);
        //Vector<double> kMatrix = adjustment.CalculateK(normalMatrix, inaccuracies);
        //Vector<double> pvMatrix = adjustment.CalculatePV(kMatrix, confM);
        //Vector<double> correction = adjustment.CalculateCorrections(pvMatrix, weightMatrix);
        //List<RelativeMeasurement> adjustedMeas = adjustment.CalculateAdjustedMeasurements(correction);
        //List<List<RelativeMeasurement>> assignedAdjustedMeas = adjustment.AsignMeasurementsToTraverses(adjustedMeas);
        //Vector<double> adjustedInaccuracies = adjustment.CalculateInnacuraciesVector(assignedAdjustedMeas);
        //List<AdjustedPoint> adjustedPointValues = adjustment.CalculateUnknownPoints(adjustedMeas.Cast<RelativeMeasurement>().ToList());

        GravimetricAdjuster adjuster = new(meas);
        adjuster.PerformAdjustment();

        //AdjustmentResult<RelativeMeasurement, AdjustedPoint> result = adjustment.AdjustNetwork();
        //Console.WriteLine("---");
        //foreach(double v in confM.ToArray())
        //{
        //    Console.WriteLine(v);
        //}
        //Console.WriteLine("---");

        //Console.WriteLine("Configuration matrix");
        //for (int i = 0; i < confM.RowCount; i++)
        //{
        //    for (int j = 0; j < confM.ColumnCount; j++)
        //    {
        //        Console.Write($"{confM[i, j], 10} ");
        //    }
        //    Console.WriteLine();
        //}
        //Console.WriteLine("Weight matrix");
        //for (int i = 0; i < weightMatrix.RowCount; i++)
        //{
        //    Console.WriteLine(weightMatrix[i, i]);
        //}
        //Console.WriteLine("Inaccuracies");
        ////Console.WriteLine(inaccuracies);
        //for (int i = 0; i<inaccuracies.Count; i++)
        //{

        //    Console.Write($"{inaccuracies[i]} ");
        //    Console.WriteLine();
        //}
        //Console.WriteLine("Normal matrix");
        //for (int i = 0; i < normalMatrix.RowCount; i++)
        //{
        //    for (int j = 0; j < normalMatrix.ColumnCount; j++)
        //    {
        //        Console.Write($"{normalMatrix[i, j]} ");
        //    }
        //    Console.WriteLine();
        //}
        //Console.WriteLine(normalMatrix);
        //Console.WriteLine("k matrix");
        //Console.WriteLine(kMatrix);
        //Console.WriteLine("PV");
        //Console.WriteLine(pvMatrix);
        //Console.WriteLine("Corrections");
        //Console.WriteLine(correction);
        //Console.WriteLine("Крайна проверка");
        //Console.WriteLine(adjustedInaccuracies);
        //Console.WriteLine("Изравнени измервания");
        //foreach (RelativeMeasurement adjMeas in adjustedMeas)
        //{
        //    Console.WriteLine($"От {adjMeas.FromPoint.Number} до {adjMeas.ToPoint.Number} {adjMeas.Value}");
        //}
        //Console.WriteLine("Изравнени точки");
        //foreach (AdjustedPoint kp in adjustedPointValues)
        //{
        //    Console.WriteLine($"Изравнената точка {kp.Number} има потенциал {kp.Value}");
        //}

    }
}
