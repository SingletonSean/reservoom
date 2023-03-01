using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Reservoom.Test.E2E
{
    public class MakeReservationTests
    {
        [Test]
        public void ShouldMakeReservation()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("app", @"C:\Storage\VS Repos\YouTube\Reservoom\src\Reservoom\bin\Debug\net5.0-windows\win-x64\Reservoom.exe");
            options.AddAdditionalCapability("appWorkingDir", @"C:\Storage\VS Repos\YouTube\Reservoom\src\Reservoom\bin\Debug\net5.0-windows\win-x64\");

            WindowsDriver<WindowsElement> driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"),
                options);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            wait.IgnoreExceptionTypes(typeof(WebDriverException));

            try
            {
                driver.FindElementByAccessibilityId("MakeReservationButton").Click();

                driver.FindElementByAccessibilityId("ReservationUsernameTextBox").SendKeys("SingletonSean");

                driver.FindElementByAccessibilityId("ReservationFloorNumberTextBox").Clear();
                driver.FindElementByAccessibilityId("ReservationFloorNumberTextBox").SendKeys("2");

                driver.FindElementByAccessibilityId("ReservationRoomNumberTextBox").Clear();
                driver.FindElementByAccessibilityId("ReservationRoomNumberTextBox").SendKeys("10");

                driver.FindElementByAccessibilityId("ReservationStartDateTextBox").SendKeys("2/28/2023");
                driver.FindElementByAccessibilityId("ReservationEndDateTextBox").SendKeys("3/01/2023");

                driver.FindElementByAccessibilityId("ReservationSubmitButton").Click();
                driver.FindElementByAccessibilityId("ReservationSubmitButton").Click();

                WindowsElement reservationListingItem = wait.Until((_) =>
                {
                    Actions actions = new Actions(driver);
                    actions.SendKeys(Keys.Escape).Perform();

                    return driver.FindElementByAccessibilityId("210_ReservationListingItem");
                });

                Assert.That(reservationListingItem, Is.Not.Null);
            }
            finally
            {
                driver.Close();
            }
        }
    }
}