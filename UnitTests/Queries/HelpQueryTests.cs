﻿using System;
using System.Text;
using Moq;
using MotoBotCore.Classes;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;
using MotoBotCore.Queries;
using NUnit.Framework;

namespace UnitTests.Queries
{
    [TestFixture]
    public class HelpQueryTests
    {
        [Test]
        public void Ctor_Called_PropertieSetUp()
        {
            // * Arrange
            var fh = new Mock<IFileReader>(MockBehavior.Loose);

            // * Act
            var q = new HelpQuery { FileReader = fh.Object };

            // * Assert
            Assert.That(!String.IsNullOrWhiteSpace(q.Description));
            Assert.That(q.PrivilegeLevel, Is.Not.EqualTo(QueryPrivilegeLevel.Undefined));
        }

        [Test]
        public void CanExecute_ValidCall_True()
        {
            // * Arrange
            var fh = new Mock<IFileReader>(MockBehavior.Loose);
            
            // * Act
            var q = new HelpQuery { FileReader = fh.Object };
            var canExecute = q.CanExecute("help");

            // * Assert
            Assert.That(canExecute);
        }

        [Test]
        public void CanExecute_InvalidCall_False()
        {
            // * Arrange
            var fh = new Mock<IFileReader>(MockBehavior.Loose);

            // * Act
            var q = new HelpQuery { FileReader = fh.Object };
            var canExecute = q.CanExecute("foobar");

            // * Assert
            Assert.That(!canExecute);
        }

        [Test]
        public void Execute_ValidCall_MessagesUser()
        {
            // * Arrange
            var fh = new Mock<IFileReader>(MockBehavior.Loose);
            fh.Setup(x => x.ReadAll("help.txt", It.IsAny<Encoding>())).Returns("Test");

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var context = new QueryContext(bot.Object, user.Object);

            // * Act
            var q = new HelpQuery { FileReader = fh.Object };
            q.Execute("help", context);

            // * Assert
            bot.Verify(x => x.MessageUser(user.Object, "Test"), Times.Once);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Execute_InvalidCall_Throws()
        {
            // * Arrange
            var fh = new Mock<IFileReader>(MockBehavior.Loose);
            fh.Setup(x => x.ReadAll("help.txt", It.IsAny<Encoding>())).Returns("Test");

            var bot = new Mock<IBot>();
            var user = new Mock<IUser>();
            var context = new QueryContext(bot.Object, user.Object);

            // * Act
            var q = new HelpQuery { FileReader = fh.Object };
            q.Execute("foobar", context);
        }
    }
}
