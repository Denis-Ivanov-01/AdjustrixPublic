using LSA_Base;
using NUnit.Framework;

namespace AdjustmentTest
{
    public class Tests
    {
        readonly NewGravimetricPoint gp1 = new("1", 27629.2642, 340966.7408);
        readonly NewGravimetricPoint gp2 = new("2", 27226.8512, 341323.5681);
        readonly NewGravimetricPoint gp3 = new("3", 27201.8000, 341009.7822);

        //NewGravimetricPoint gp3 = new("3", 27039.5212, 341009.7822);
        readonly NewGravimetricPoint gp7 = new("7", 27472.4619, 340382.2104);
        readonly KnownGravimetricPoint gp4 = new("4", 27201.8739, 340834.8395, 9);
        readonly KnownGravimetricPoint gp5 = new("5", 27389.2041, 340376.6567, 8);
        readonly KnownGravimetricPoint gp8 = new("8", 27404.4680, 340280.8549, 8.5);
        readonly KnownGravimetricPoint gp9 = new("9", 27133.3325, 341704.3286, 9.5);
        readonly KnownGravimetricPoint gp10 = new("10", 27736.1762, 340572.0287, 10);
        readonly NewGravimetricPoint gp11 = new("11", 27230.5212, 341000.7822);

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNetworkMultipleComplexDiagonals()
        {
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
            System.Collections.Generic.List<RelativeMeasurement> meas = new() { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m14, m15, m16, m17, m18, m19, m20 };
            GravimetricAdjuster adjuster = new(meas);
            adjuster.PerformAdjustment();
            Assert.Pass();
        }

        [Test]
        public void TestNetworkOneComplexDiagonal()
        {
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
            RelativeMeasurement m15 = new(gp3, gp5, -1.05, 660.279);
            RelativeMeasurement m16 = new(gp7, gp10, 1, 324.925);
            RelativeMeasurement m17 = new(gp1, gp10, 0.94, 408.935);
            RelativeMeasurement m18 = new(gp10, gp3, -1, 690.787);
            RelativeMeasurement m19 = new(gp3, gp11, -0.38, 30.098);
            RelativeMeasurement m20 = new(gp11, gp8, -0.52, 740.644);
            System.Collections.Generic.List<RelativeMeasurement> meas = new() { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m15, m16, m17, m18, m19, m20 };
            GravimetricAdjuster adjuster = new(meas);
            adjuster.PerformAdjustment();
            Assert.Pass();
        }

        [Test]
        public void TestNetworkNoComplexDiagonals()
        {
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
            RelativeMeasurement m15 = new(gp3, gp5, -1.05, 660.279);
            RelativeMeasurement m16 = new(gp7, gp10, 1, 324.925);
            RelativeMeasurement m17 = new(gp1, gp10, 0.94, 408.935);
            RelativeMeasurement m19 = new(gp3, gp11, -0.38, 30.098);
            RelativeMeasurement m20 = new(gp11, gp8, -0.52, 740.644);
            System.Collections.Generic.List<RelativeMeasurement> meas = new() { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m15, m16, m17, m19, m20 };
            GravimetricAdjuster adjuster = new(meas);
            adjuster.PerformAdjustment();
            Assert.Pass();
        }

        [Test]
        public void TestNetworkOneTriangle()
        {
            RelativeMeasurement m1 = new(gp4, gp1, 0.052, 447.281);
            RelativeMeasurement m2 = new(gp1, gp2, -0.022, 537.831);
            RelativeMeasurement m13 = new(gp4, gp2, 0.033, 489.366);
            System.Collections.Generic.List<RelativeMeasurement> meas = new() { m1, m2, m13 };
            GravimetricAdjuster adjuster = new(meas);
            adjuster.PerformAdjustment();
            Assert.Pass();
        }

        [Test]
        public void TestNetworkRectangleWithDiagonals()
        {
            RelativeMeasurement m1 = new(gp4, gp1, 0.052, 447.281);
            RelativeMeasurement m2 = new(gp1, gp2, -0.022, 537.831);
            RelativeMeasurement m3 = new(gp1, gp3, -0.011, 429.626);
            RelativeMeasurement m4 = new(gp2, gp3, 0.0097, 314.784);
            RelativeMeasurement m5 = new(gp3, gp4, -0.039, 174.943);
            RelativeMeasurement m13 = new(gp4, gp2, 0.033, 489.366);
            System.Collections.Generic.List<RelativeMeasurement> meas = new() { m1, m2, m3, m4, m5, m13 };
            GravimetricAdjuster adjuster = new(meas);
            adjuster.PerformAdjustment();
            Assert.Pass();
        }
    }
}