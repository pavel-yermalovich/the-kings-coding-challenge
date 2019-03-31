using NUnit.Framework;
using System.Text;

namespace TheKings.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private Program _instance;

        [SetUp]
        public void Setup()
        {
            _instance = new Program();
        }

        [Test]
        public void Should_Show_Question_And_Answer()
        {
            // Arrange
            StringBuilder logOutput = new StringBuilder();
            Program.LogFunc = i => logOutput.AppendLine(i);

            // Act
            string question = "How old are you?";
            string answer = "I'm 23 years old";
            Program.ShowQuestionAndAnswer(question, 23, () => answer);

            // Assert
            Assert.That(logOutput.ToString(), Is.EqualTo($"{question} {answer}\n\r\n"));
        }
    }
}
