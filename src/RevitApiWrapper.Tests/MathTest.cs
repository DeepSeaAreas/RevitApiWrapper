using NUnit.Framework;
using RevitApiWrapper.DB;
using System;

namespace RevitApiWrapper.Tests
{
    public class MathTest
    {
        [Test]
        public void IsGreaterThanTest()
        {
            var source = 3d;
            var target = 2e-5;
            var flag = source.IsGreaterThan(target);
            Assert.True(flag, "{source} is greater than {target}", source, target);
        }

        [Test]
        public void IsGreaterThanOrEqualTest()
        {
            var source = 3d;
            var target = 3d;
            var flag = source.IsGreaterThanOrEqualWith(target);
            Assert.IsTrue(flag, "{source} is greater than or equal {target}", source, target);
        }

        [Test]
        public void IsAlmostEqualZeroTest()
        {
            var source = 1e-6;
            var flag = source.IsAlmostEqualZero();
            Assert.True(flag);
        }

        [Test]
        public void IsAlmostEqualTest()
        {
            var source = 3d;
            var target = 2.9999999;
            var flag = source.IsAlmostEqual(target);
            Assert.True(flag);
        }

        [Test]
        public void IsLessThanTest()
        {
            var source = 2d;
            var target = 3d;
            var flag = source.IsLessThan(target);
            Assert.IsTrue(flag);
        }

        [Test]
        public void IsLessThanOrEqualTest()
        {
            var source = 3d;
            var target = 3d;
            var flag = source.IsLessThanOrEqualWith(target);
            Assert.IsTrue(flag);
        }

        [Test]
        public void RadianToAngleTest()
        {
            var source = Math.PI;
            var result = source.RadianToAngle();
            Assert.AreEqual(180, result);
        }

        [Test]
        public void AngelToRadianTest()
        {
            var source = 180d;
            var result = source.AngelToRadian();
            Assert.AreEqual(Math.PI, result);
        }

        [Test]
        public void MillimeterToFeetTest()
        {
            var source = 300d;
            var target = source.MillimeterToFeet();
            Assert.NotZero(target);
        }

        [Test]
        public void FeetToMillimeterTest()
        {
            var source = 10d;
            var target = source.FeetToMillimeter();
            Assert.NotZero(target);
        }
    }
}
