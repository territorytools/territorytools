using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TerritoryTools.Web.MainSite;
using TerritoryTools.Web.MainSite.Services;

namespace Web.MainSite.Tests;

public class TerritoryAssignmentServiceTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PassCorrectUri()
    {
        Mock<ICombinedAssignmentService> combinedAssignmentService = new();
        Mock<IUserService> userService = new();
        userService.Setup(s => s.GetUsers(It.IsAny<string>()))
            .Returns(new List<Controllers.UseCases.User>());

        Mock<IAlbaAuthClientService> albaAuthClientService = new();
        bool downloadWasCalled = false;
        string actualUri = null!;
        string actualUser = null!;
        albaAuthClientService.Setup(x => x.DownloadString(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((uri, user) =>
            {
                downloadWasCalled = true;
                actualUri = uri;
                actualUser = user;
            });

        Mock<IOptions<WebUIOptions>> optionsAccessor = new();
        Mock<ILogger<TerritoryAssignmentService>> logger = new();
        ITerritoryAssignmentService service = new TerritoryAssignmentService(
            combinedAssignmentService.Object,
            userService.Object,
            albaAuthClientService.Object,
            optionsAccessor.Object,
            logger.Object);

        service.Assign(111, 222, "some_user");

        Assert.IsTrue(downloadWasCalled);
        Assert.AreEqual("/ts?mod=assigned&cmd=assign&id=111&date=2022-06-16&user=222", actualUri);
        Assert.AreEqual("some_user", actualUser);
    }
}
