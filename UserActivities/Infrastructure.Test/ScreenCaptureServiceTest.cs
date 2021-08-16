using Autofac.Extras.Moq;
using Infrastructure.ScreenCapture;
using Infrastructure.FileManager;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Infrastructure.Test
{
    [TestFixture]
    public class ScreenCaptureServiceTest
    {
        private AutoMock _mock;
        private Mock<IFileManagerService> _fileManagerServiceMoc;
        private Mock<IScreenCaptureAdapter> _screenCaptureAdapterMoc;
        private ScreenCaptureService _screenCaptureService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileManagerServiceMoc = _mock.Mock<IFileManagerService>();
            _screenCaptureAdapterMoc = _mock.Mock<IScreenCaptureAdapter>();
            _screenCaptureService = _mock.Create<ScreenCaptureService>();
        }

        [TearDown]
        public void Clean()
        {
            _fileManagerServiceMoc?.Reset();
            _screenCaptureAdapterMoc?.Reset();
        }

        [Test, Category("Unit Test")]
        public async Task CaptureScreenAsync_ForValidHeightWeightFilePath_StoreCapturedUserScreen()
        {
            //Arrange
            var filePath = "C:\\Test\\file.jpg";
            var width = 1920;
            var height = 1080;
            var bitmapImage = new Bitmap(width, height);

            _screenCaptureAdapterMoc.Setup(x => x.CaptureUserScreen(width, height)).Returns(bitmapImage).Verifiable();
            _fileManagerServiceMoc.Setup(x => x.SaveBitmapImage(filePath, bitmapImage)).Verifiable();

            //Act
            await _screenCaptureService.CaptureScreenAsync(width, height, filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _screenCaptureAdapterMoc.Verify(),
                () => _fileManagerServiceMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task CaptureScreenAsync_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            await Should.ThrowAsync<Exception>(_screenCaptureService.CaptureScreenAsync(1920, 1080, filePath));
        }

        [Test, Category("Unit Test")]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public async Task CaptureScreenAsync_IfWidthOrHeightIsZero_ThrowException(int width, int height)
        {
            //Arrange
            var filePath = "C:\\Test\\file.jpg";
            _screenCaptureAdapterMoc.Setup(x => x.CaptureUserScreen(width, height)).Returns((Bitmap)null).Verifiable();

            //Act & Assert
            await Should.ThrowAsync<Exception>(() => _screenCaptureService.CaptureScreenAsync(width, height, filePath));
        }
    }
}
