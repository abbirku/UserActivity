using Autofac.Extras.Moq;
using Infrastructure.DirectoryManager;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Test
{
    [TestFixture]
    public class DirectoryManagerServiceTest
    {
        private AutoMock _mock;
        private Mock<IDirectoryManagerAdapter> _directoryManagerAdapterMock;
        private IDirectoryManagerService _directoryManagerService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _directoryManagerAdapterMock = _mock.Mock<IDirectoryManagerAdapter>();
            _directoryManagerService = _mock.Create<DirectoryManagerService>();
        }

        [TearDown]
        public void Clean()
        {
            _directoryManagerAdapterMock?.Reset();
        }

        [OneTimeTearDown]
        public void ClassClean()
        {
            _mock?.Dispose();
        }

        [Test, Category("Unit Test")]
        public void ChecknCreateDirectory_ForEmptyString_ReturnsFalse()
        {
            //Act
            var result = _directoryManagerService.ChecknCreateDirectory(string.Empty);

            //Assert
            result.ShouldBe(false);
        }
    }
}
