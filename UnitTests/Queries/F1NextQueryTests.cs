using System;
using System.Text;
using Moq;
using MotoBotCore.Classes;
using MotoBotCore.Data;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;
using MotoBotCore.Queries;
using NUnit.Framework;

namespace UnitTests.Queries
{
    [TestFixture]
    public class F1NextQueryTests
    {
        [Test]
        public void Ctor_Called_PropertieSetUp()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };

            // * Assert
            Assert.That(!String.IsNullOrWhiteSpace(q.Description));
            Assert.That(q.PrivilegeLevel, Is.Not.EqualTo(QueryPrivilegeLevel.Undefined));
        }

        [Test]
        public void CanExecute_ValidCall_True()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            var canExecute = q.CanExecute("f1next");

            // * Assert
            Assert.That(canExecute);
        }

        [Test]
        public void CanExecute_InvalidCall_False()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            var canExecute = q.CanExecute("foobar");

            // * Assert
            Assert.That(!canExecute);
        }

        [Test]
        public void Execute_ValidCallNoArgFull_MessagesUser()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);
            fh
                .Setup(x => x.GetNextSession(Series.Formula1))
                .Returns(() =>
                    new Session
                    {
                        DateTimeUtc = DateTime.UtcNow.AddDays(1).AddHours(2).AddMinutes(3).AddSeconds(4),
                        Name = "R",
                        GrandPrix = new GrandPrix { Name = "Fantasy GP of Utopia", Number = 0 }
                    });

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var channel = new Mock<IChannel>();
            var context = new QueryContext(bot.Object, user.Object, channel.Object, false);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            q.Execute("f1next", context);

            // * Assert
            bot.Verify(x => x.MessageChannel(channel.Object, "Next GP: Race #0 - Fantasy GP of Utopia"), Times.Once);
            bot.Verify(x => x.MessageChannel(channel.Object, It.IsRegex("Next Session: .+ in 1 days, 2 hours, 3 minutes, [0-9] seconds")), Times.Once);
        }

        [Test]
        public void Execute_ValidCallNoArgPartial_MessagesUser()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);
            fh
                .Setup(x => x.GetNextSession(Series.Formula1))
                .Returns(() =>
                    new Session
                    {
                        DateTimeUtc = DateTime.UtcNow.AddMinutes(1).AddSeconds(2),
                        Name = "R",
                        GrandPrix = new GrandPrix { Name = "Fantasy GP of Utopia", Number = 0 }
                    });

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var channel = new Mock<IChannel>();
            var context = new QueryContext(bot.Object, user.Object, channel.Object, false);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            q.Execute("f1next", context);

            // * Assert
            bot.Verify(x => x.MessageChannel(channel.Object, "Next GP: Race #0 - Fantasy GP of Utopia"), Times.Once);
            bot.Verify(x => x.MessageChannel(channel.Object, It.IsRegex("Next Session: .+ in 1 minutes, 2 seconds")), Times.Once);
        }

        [Test, ExpectedException(typeof(NotImplementedException))]
        public void Execute_ValidCallArg_Throws()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);
            fh
                .Setup(x => x.GetNextSession(Series.Formula1))
                .Returns(() =>
                    new Session
                    {
                        DateTimeUtc = DateTime.UtcNow.AddDays(1),
                        Name = "R",
                        GrandPrix = new GrandPrix { Name = "Fantasy GP of Utopia", Number = 0 }
                    });

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var channel = new Mock<IChannel>();
            var context = new QueryContext(bot.Object, user.Object, channel.Object, false);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            q.Execute("f1next Europe/Zurich", context);

            // * Assert
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Execute_InvalidCall_Throws()
        {
            // * Arrange
            var fh = new Mock<IInformationRepository>(MockBehavior.Loose);
            fh.Setup(x => x.ReadAllFromFile("help.txt", It.IsAny<Encoding>())).Returns("Test");

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var channel = new Mock<IChannel>();
            var context = new QueryContext(bot.Object, user.Object, channel.Object, false);

            // * Act
            var q = new F1NextQuery { InformationRepository = fh.Object };
            q.Execute("foobar", context);
        }
    }
}
