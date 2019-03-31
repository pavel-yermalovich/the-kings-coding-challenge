using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TheKings.Tests
{
    [TestFixture]
    public class KingStatsServiceTests
    {
        private KingStatsService _instance;
        private Mock<IKingApiService> _kingApiServiceMock;

        [SetUp]
        public void Setup()
        {
            _kingApiServiceMock = new Mock<IKingApiService>();
            _instance = new KingStatsService(_kingApiServiceMock.Object);
        }

        [TestCase(125)]
        [TestCase(0)]
        [TestCase(33)]
        public void Should_Get_Kings_Count(int count)
        {
            // Arrange
            SetupKings(KingsWithNameStartedWith("Elizabeth", count));
            
            // Act
            var result = _instance.GetKingsCount();

            // Assert
            Assert.That(result, Is.EqualTo(count));
        }

        [Test]
        public void Should_Return_Null_If_No_Kings()
        {
            // Arrange
            SetupKings(Enumerable.Empty<King>());

            // Assert
            Assert.That(_instance.GetHouseWhichRuledLongest(), Is.Null);
            Assert.That(_instance.GetKingWhoRuledLongest(), Is.Null);
            Assert.That(_instance.GetMostCommonFirstName(), Is.Null);
        }

        [Test]
        public void Should_Get_King_Who_Ruled_Longest()
        {
            // Arrange
            SetupKings(
                KingWithYearsRuled("Alexander", 1000, 2000),
                KingWithYearsRuled("Petr I", 1000, 1050),
                KingWithYearsRuled("Petr II", 1050, 1055)
            );

            // Act
            var result = _instance.GetKingWhoRuledLongest();

            // Assert
            Assert.That(result.First().Name, Is.EqualTo("Alexander"));
        }

        [Test]
        public void Should_Get_Kings_Who_Ruled_Longest()
        {
            // Arrange
            SetupKings(
                KingWithYearsRuled("Alexander", 1000, 2000),
                KingWithYearsRuled("Petr I", 1000, 2000),
                KingWithYearsRuled("Petr II", 1050, 1055),
                KingWithYearsRuled("Edward", 1050, 1065)
            );

            // Act
            var result = _instance.GetKingWhoRuledLongest().ToArray();

            // Assert
            Assert.That(result[0].Name, Is.EqualTo("Alexander"));
            Assert.That(result[1].Name, Is.EqualTo("Petr I"));
        }

        [Test]
        public void Should_Get_House_Which_Ruled_Longest()
        {
            // Arrange
            var house1 = CreateHouse("House 1")
                .WithKing("Antony 1", 1900, 1950)
                .WithKing("Antony 2", 1950, 1960)
                .WithKing("Antony 3", 1960, 1970);

            var house2 = CreateHouse("House 2")
                .WithKing("Pavel 1", 1900, 1901);

            var house3 = CreateHouse("House 3")
                .WithKing("Antony 1", 1900, 1901)
                .WithKing("Antony 2", 1901, 1921);

            SetupKings(house1.Kings.Union(house2.Kings.Union(house3.Kings)));

            // Act
            var result = _instance.GetHouseWhichRuledLongest();

            // Assert
            Assert.That(result.First().Name, Is.EqualTo(house1.Name));
        }

        [Test]
        public void Should_Get_Houses_Which_Ruled_Longest()
        {
            // Arrange
            var house1 = CreateHouse("House 1")
                .WithKing("Antony 1", 1700, 1900);

            var house2 = CreateHouse("House 2")
                .WithKing("Pavel 1", 1750, 1950);

            var house3 = CreateHouse("House 3")
                .WithKing("Antony 1", 1900, 1901)
                .WithKing("Antony 2", 1901, 1921);

            SetupKings(house1.Kings.Union(house2.Kings.Union(house3.Kings)));

            // Act
            var result = _instance.GetHouseWhichRuledLongest().ToArray();

            // Assert
            Assert.That(result[0].Name, Is.EqualTo(house1.Name));
            Assert.That(result[1].Name, Is.EqualTo(house2.Name));
        }

        private House CreateHouse(string name)
        {
            var house = new House { Name = name, Kings = new List<King>() };
            return house;
        }

        [Test]
        public void Should_Get_Most_Common_First_Name()
        {
            // Arrange
            SetupKings(KingsWithNameStartedWith("Andrew", 25)
                .Union(KingsWithNameStartedWith("Nicolas", 24))
                .Union(KingsWithNameStartedWith("Edward", 1)));

            // Act
            var name = _instance.GetMostCommonFirstName();

            // Assert
            Assert.That(name.First(), Is.EqualTo("Andrew"));
        }

        [Test]
        public void Should_Get_Most_Common_First_Names()
        {
            // Arrange
            SetupKings(KingsWithNameStartedWith("Andrew", 25)
                .Union(KingsWithNameStartedWith("Nicolas", 24))
                .Union(KingsWithNameStartedWith("Peter", 15))
                .Union(KingsWithNameStartedWith("Anna", 25))
                .Union(KingsWithNameStartedWith("Edward", 1)));

            // Act
            var name = _instance.GetMostCommonFirstName().ToArray();

            // Assert

            Assert.That(name, Does.Contain("Andrew").And.Contain("Anna"));
        }

        private IList<King> KingsWithNameStartedWith(string firstName, int number)
        {
            return Enumerable.Range(1, number)
                .Select(i => new King { Name = firstName + " " + number })
                .ToList();
        }

        private King KingWithYearsRuled(string name, int from, int to)
        {
            return new King { Name = name, Ruled = new YearRange(from, to) };
        }

        private void SetupKings(IEnumerable<King> kings)
        {
            _kingApiServiceMock
                .Setup(s => s.GetKings())
                .Returns(kings);
        }

        private void SetupKings(params King[] kings)
        {
            SetupKings(kings.AsEnumerable());
        }
    }
}
