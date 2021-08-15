using Autofac.Extras.Moq;
using Infrastructure.FileManager;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace Infrastructure.Test
{
    [TestFixture]
    public class FileManagerServiceTest
    {
        private AutoMock _mock;
        private Mock<IFileAdapter> _fileAdapterMoc;
        private Mock<IFileStreamAdapter> _fileStreamAdapterMoc;
        private FileManagerService _fileManagerService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileAdapterMoc = _mock.Mock<IFileAdapter>();
            _fileStreamAdapterMoc = _mock.Mock<IFileStreamAdapter>();
            _fileManagerService = _mock.Create<FileManagerService>();
        }

        [TearDown]
        public void Clean()
        {
            _fileAdapterMoc?.Reset();
            _fileStreamAdapterMoc?.Reset();
        }

        [Test, Category("Unit Test")]
        public void CreateFile_ForValidFilePath_CreatesTheFile()
        {
            //Arrange
            var filePath = "C:\\Test\\img.jpg";
            _fileAdapterMoc.Setup(x => x.DoesExists(filePath)).Returns(false).Verifiable();
            _fileAdapterMoc.Setup(x => x.CreateFile(filePath)).Verifiable();

            //Act
            _fileManagerService.CreateFile(filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CreateFile_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _fileManagerService.CreateFile(filePath));
        }

        [Test, Category("Unit Test")]
        public void ReadFileAsByte_ForValidFilePath_ReturnFileBytes()
        {
            //Arrange
            var filePath = "C:\\Test\\img.jpg";
            var fileBytes = new byte[64];
            _fileAdapterMoc.Setup(x => x.DoesExists(filePath)).Returns(true).Verifiable();
            _fileStreamAdapterMoc.Setup(x => x.ReadFileAsByte(filePath)).Returns(fileBytes).Verifiable();

            //Act
            var returnedfileBytes = _fileManagerService.ReadFileAsByte(filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _fileStreamAdapterMoc.Verify(),
                () => returnedfileBytes.ShouldBe(fileBytes)
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ReadFileAsByte_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _fileManagerService.ReadFileAsByte(filePath));
        }

        [Test, Category("Unit Test")]
        public void ReadFileAsByte_IfFileDoesNotExists_ThrowException()
        {
            //Arrange
            var filePath = "C:\\Test\\img.jpg";
            _fileAdapterMoc.Setup(x => x.DoesExists(filePath)).Returns(false).Verifiable();

            //Act & Assert
            Should.Throw<Exception>(() => _fileManagerService.ReadFileAsByte(filePath));
        }

        [Test, Category("Unit Test")]
        public void SaveByteStream_ForValidFilePathAndFile_WriteTheFile()
        {
            //Arrange
            var filePath = "C:\\Test\\img.jpg";
            var fileBytes = new byte[64];
            _fileStreamAdapterMoc.Setup(x => x.WriteFileBytes(filePath, fileBytes)).Verifiable();

            //Act
            _fileManagerService.SaveByteStream(filePath, fileBytes);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileStreamAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(" ", null)]
        [TestCase(" ", new byte[0])]
        public void SaveByteStream_ForInvalidFilePathOrFile_ThrowException(string filePath, byte[] file)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _fileManagerService.SaveByteStream(filePath, file));
        }
    }
}
