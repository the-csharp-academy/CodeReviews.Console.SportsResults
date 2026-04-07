using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using SportsResultsNotifier.TerrenceLGee.Data;
using SportsResultsNotifier.TerrenceLGee.Interfaces;
using SportsResultsNotifier.TerrenceLGee.Services;
using SportsResultsNotifier.TerrenceLGee.Tests.Resources;

namespace SportsResultsNotifier.TerrenceLGee.Tests;

public class EmailServiceTests
{
    private readonly OptionsWrapper<EmailConfiguration> _configWrapper;
    private readonly Mock<ISmtpClientFactory> _mockClientFactory;
    private readonly IEmailService _emailService;

    public EmailServiceTests()
    {
        _configWrapper = new OptionsWrapper<EmailConfiguration>(EmailResources.GetConfiguration());
        _mockClientFactory = new Mock<ISmtpClientFactory>();
        _emailService = new EmailService(_configWrapper, _mockClientFactory.Object);
    }

    [Fact]
    public async Task SendEmailAsync_Does_Not_Throw_ExceptionWhenSentSuccessfully()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        await _emailService.SendEmailAsync(EmailResources.GetEmailData(), token);

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_DoesNotSendEmail_WhenConnectionFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException());

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        await Assert.ThrowsAsync<IOException>(async () => await _emailService.SendEmailAsync(EmailResources.GetEmailData(), token));

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_DoesNotSendEmail_WhenAuthenticationFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException());

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        await Assert.ThrowsAsync<AuthenticationException>(async () => await _emailService.SendEmailAsync(EmailResources.GetEmailData(), token));

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_DoesNotSendEmail_WhenSendingFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _emailService.SendEmailAsync(EmailResources.GetEmailData(), token));

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
