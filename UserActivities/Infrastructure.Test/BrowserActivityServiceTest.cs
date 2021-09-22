using Autofac.Extras.Moq;
using Infrastructure.BrowserActivity;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreActivities.FileManager;
using CoreActivities.BrowserActivity;

namespace Infrastructure.Test
{
    [TestFixture]
    public class BrowserActivityServiceTest
    {
        private AutoMock _mock;
        private Mock<IFile> _fileAdapterMoc;
        private Mock<IBrowserActivity> _browserActivityAdapterMoc;
        private BrowserActivityService _browserActivityService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileAdapterMoc = _mock.Mock<IFile>();
            _browserActivityAdapterMoc = _mock.Mock<IBrowserActivity>();
            _browserActivityService = _mock.Create<BrowserActivityService>();
        }

        [TearDown]
        public void Clean()
        {
            _fileAdapterMoc?.Reset();
            _browserActivityAdapterMoc?.Reset();
        }

        [Test, Category("Unit Test")]
        public async Task EnlistAllOpenTabs_ForValidTextFilePathAndBrowserName_StoreTabInformation()
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";
            var tabs = new List<string>
            {
                "tab1",
                "tab2"
            };
            var serTabs = tabs.Select((x, index) => {
                return $"{index + 1}. {x}";
            }).ToList();

            _browserActivityAdapterMoc.Setup(x => x.EnlistAllOpenTabs(BrowserType.Chrome)).Returns(tabs).Verifiable();
            _fileAdapterMoc.Setup(x => x.AppendAllLineAsync(serTabs, filePath)).Returns(Task.CompletedTask).Verifiable();

            //Act
            await _browserActivityService.EnlistAllOpenTabs(BrowserType.Chrome, filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _browserActivityAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase(null, " ")]
        [TestCase("", null)]
        [TestCase("", "")]
        [TestCase("", " ")]
        [TestCase(" ", null)]
        [TestCase(" ", "")]
        [TestCase(" ", " ")]
        public void EnlistAllOpenTabs_ForInvalidFilePathAndBrowserName_ThrowException(string browserName, string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _browserActivityService.EnlistActiveTabUrl(BrowserType.Chrome, filePath));
        }

        [Test, Category("Unit Test")]
        public async Task EnlistActiveTabUrl_ForValidTextFilePathAndBrowserName_ReceiveValidTabUrl()
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";
            var browser = "chrome";
            var url = "www.google.com/test";

            _browserActivityAdapterMoc.Setup(x => x.EnlistActiveTabUrl(BrowserType.Chrome)).Returns(url).Verifiable();
            _fileAdapterMoc.Setup(x => x.AppendAllTextAsync("www.google.com", filePath)).Returns(Task.CompletedTask).Verifiable();

            //Act
            await _browserActivityService.EnlistActiveTabUrl(BrowserType.Chrome, filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _browserActivityAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase(null, " ")]
        [TestCase("", null)]
        [TestCase("", "")]
        [TestCase("", " ")]
        [TestCase(" ", null)]
        [TestCase(" ", "")]
        [TestCase(" ", " ")]
        public void EnlistActiveTabUrl_ForInvalidFilePathAndBrowserName_ThrowException(string browserName, string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _browserActivityService.EnlistActiveTabUrl(BrowserType.Chrome, filePath));
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void EnlistActiveTabUrl_ForInvalidFileUrl_ThrowException(string url)
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";

            _browserActivityAdapterMoc.Setup(x => x.EnlistActiveTabUrl(BrowserType.Chrome)).Returns(url).Verifiable();

            //Act & Assert
            Should.Throw<Exception>(() => _browserActivityService.EnlistActiveTabUrl(BrowserType.Chrome, filePath));
        }
    }
}
