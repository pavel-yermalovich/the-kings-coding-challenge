using NUnit.Framework;

namespace TheKings.Tests
{
    [TestFixture]
    public class YearRangeTests
    {
        [Test]
        public void Default_Ctor_Doesnt_Setup_Years()
        {
            // Act
            var years = new YearRange();

            // Assert
            Assert.That(years.Start, Is.EqualTo(0));
            Assert.That(years.End, Is.EqualTo(0));
        }

        [TestCase(123, 345)]
        public void Ctor_Should_Setup_Years(int start, int end)
        {
            // Act
            var years = new YearRange(start, end);

            // Assert
            Assert.That(years.Start, Is.EqualTo(start));
            Assert.That(years.End, Is.EqualTo(end));
        }

        [TestCase(2000, 3000, 1000)]
        public void TotalYears_Is_Calculated_Correct(int start, int end, int expected)
        {
            // Assert
            var instance = new YearRange(start, end);

            // Act
            var result = instance.TotalYears;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("960-984", 960, 984)]
        [TestCase("960", 960, 960)]
        [TestCase("960-", 960, 2019)]
        [TestCase("2021", 2021, 2021)]
        public void Can_Parse_String(string input, int expectedStart, int expectedEnd)
        {
            // Act
            var result = YearRange.Parse(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Start, Is.EqualTo(expectedStart));
            Assert.That(result.End, Is.EqualTo(expectedEnd));
        }

        [TestCase("abc-xyz")]
        [TestCase("abc-")]
        [TestCase("abc")]
        [TestCase("-984")]
        [TestCase(" 960-984")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("985-984")]
        [TestCase("2021-")]
        public void Cannot_Parse_Incorrect_Format(string input)
        {
            // Act
            var result = YearRange.Parse(input);

            // Assert
            Assert.IsNull(result);
        }
    }
}
