using NUnit.Framework;

namespace TheKings.Tests
{
    [TestFixture]
    public class KingTests
    {
        [TestCase("Elizabeth II", "Elizabeth")]
        [TestCase("Elizabeth", "Elizabeth")]
        [TestCase("", "")]
        public void Should_Get_First_Name_From_Name(string name, string expectedFirstName)
        {
            // Act
            var king = new King() { Name = name };

            // Assert
            Assert.That(king.FirstName, Is.EqualTo(expectedFirstName));
        }

        [TestCase("1", "Pavel", "Belarus", "House Of Yermalovich", "2000-2010")]
        public void Should_Be_Created_From_Api_King(
            string id, string name, string country, string house, string years)
        {
            // Arrange
            var apiKing = new ApiKing { id = id, nm = name, hse = house, cty = country, yrs = years };

            // Act
            var king = new King(apiKing);

            // Assert
            Assert.That(king.Id, Is.EqualTo(int.Parse(id)));
            Assert.That(king.Name, Is.EqualTo(name));
            Assert.That(king.House, Is.EqualTo(house));
            Assert.That(king.Country, Is.EqualTo(country));
            var yearsParsed = YearRange.Parse(years);
            Assert.That(king.Ruled.Start, Is.EqualTo(yearsParsed.Start));
            Assert.That(king.Ruled.End, Is.EqualTo(yearsParsed.End));
        }
    }
}
