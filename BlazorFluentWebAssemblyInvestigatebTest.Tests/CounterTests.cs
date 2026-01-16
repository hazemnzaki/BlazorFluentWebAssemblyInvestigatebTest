using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BlazorFluentWebAssemblyInvestigatebTest.Pages;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorFluentWebAssemblyInvestigatebTest.Tests
{
    [TestClass]
    public class CounterTests
    {
        [TestMethod]
        public void Counter_ClickMe_Button_IncrementsCount()
        {
            // Arrange
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddFluentUIComponents();
            ctx.Services.AddSingleton<SharedDataPerApp>();
            var cut = ctx.RenderComponent<Counter>();

            // Get all buttons and find the "Click me" button
            var buttons = cut.FindAll("fluent-button");
            var clickMeButton = buttons.First(b => b.TextContent.Contains("Click me"));

            // Act - Click the "Click me" button
            clickMeButton.Click();

            // Assert - Verify the count is incremented to 1
            var badge = cut.Find("fluent-badge");
            Assert.AreEqual("1", badge.TextContent);

            // Act - Click the "Click me" button again
            clickMeButton.Click();

            // Assert - Verify the count is incremented to 2
            badge = cut.Find("fluent-badge");
            Assert.AreEqual("2", badge.TextContent);
        }

        [TestMethod]
        public void Counter_InitialCount_IsZero()
        {
            // Arrange
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddFluentUIComponents();
            ctx.Services.AddSingleton<SharedDataPerApp>();
            var cut = ctx.RenderComponent<Counter>();

            // Assert - Verify initial count is 0
            var badge = cut.Find("fluent-badge");
            Assert.AreEqual("0", badge.TextContent);
        }

        [TestMethod]
        public void Counter_ClickMe_UpdatesSharedData()
        {
            // Arrange
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddFluentUIComponents();
            var sharedData = new SharedDataPerApp();
            ctx.Services.AddSingleton(sharedData);
            var cut = ctx.RenderComponent<Counter>();

            // Get all buttons and find the "Click me" button
            var buttons = cut.FindAll("fluent-button");
            var clickMeButton = buttons.First(b => b.TextContent.Contains("Click me"));

            // Act - Click the "Click me" button
            clickMeButton.Click();

            // Assert - Verify the shared data is updated
            Assert.AreEqual(1, sharedData.sharedData);
        }

        [TestMethod]
        public void Counter_MultiClick_ThenNext_ShowsCountOnHome()
        {
            // Arrange
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddFluentUIComponents();
            var sharedData = new SharedDataPerApp();
            ctx.Services.AddSingleton(sharedData);
            var counterComponent = ctx.RenderComponent<Counter>();

            // Get all buttons and find the "Click me" and "Next" buttons
            var buttons = counterComponent.FindAll("fluent-button");
            var clickMeButton = buttons.First(b => b.TextContent.Contains("Click me"));
            var nextButton = buttons.First(b => b.TextContent.Contains("Next"));

            // Act - Click the "Click me" button 5 times
            for (int i = 0; i < 5; i++)
            {
                clickMeButton.Click();
            }

            // Verify the count is 5 on Counter page
            var badge = counterComponent.Find("fluent-badge");
            Assert.AreEqual("5", badge.TextContent);

            // Verify shared data is updated
            Assert.AreEqual(5, sharedData.sharedData);

            // Act - Click the "Next" button (navigates to Home)
            // Note: In bUnit, navigation doesn't actually change components,
            // so we need to render the Home component separately
            var homeComponent = ctx.RenderComponent<Home>();

            // Assert - Verify the Home page shows the correct count
            var homeText = homeComponent.Markup;
            StringAssert.Contains(homeText, "You have clicked the button");
            StringAssert.Contains(homeText, "5");
            StringAssert.Contains(homeText, "time(s) on the Counter page");
        }
    }
}
