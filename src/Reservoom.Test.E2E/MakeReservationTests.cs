using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Reservoom.E2E
{
    public class MakeReservationTests
    {
        [Test]
        public void MakeReservation_WhenRoomAvailable_MakesReservation()
        {
            WindowsDriver<WindowsElement> driver = CreateAppDriver();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            wait.IgnoreExceptionTypes(typeof(WebDriverException));

            try
            {
                driver.FindElementByAccessibilityId("MakeReservationButton").Click();

                SubmitMakeReservationForm(driver, "SingletonSean");

                WindowsElement reservationListingItem = wait.Until(_ =>
                {
                    Actions actions = new Actions(driver);
                    actions.SendKeys(Keys.Escape).Perform();

                    return driver.FindElementByAccessibilityId("2_10_ReservationListingItem");
                });

                Assert.That(reservationListingItem, Is.Not.Null);
            }
            finally
            {
                driver.Close();
            }
        }

        [Test]
        public void MakeReservation_WhenAlreadyBooked_DoesNotMakeReservation()
        {
            WindowsDriver<WindowsElement> driver = CreateAppDriver();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            wait.IgnoreExceptionTypes(typeof(WebDriverException));

            try
            {
                driver.FindElementByAccessibilityId("MakeReservationButton").Click();
                
                SubmitMakeReservationForm(driver, "SingletonSean");

                WindowsElement reservationListingItem = wait.Until(_ =>
                {
                    Actions actions = new Actions(driver);
                    actions.SendKeys(Keys.Escape).Perform();

                    return driver.FindElementByAccessibilityId("2_10_ReservationListingItem");
                });

                driver.FindElementByAccessibilityId("MakeReservationButton").Click();

                SubmitMakeReservationForm(driver, "SingletonSean2");

                WindowsElement reservationSubmitErrorMessage = wait.Until(_ =>
                {
                    return driver.FindElementByAccessibilityId("MakeReservationSubmitErrorMessage");
                });

                Assert.That(reservationSubmitErrorMessage.Text, Is.EqualTo("This room is already taken on those dates."));
            }
            finally
            {
                driver.Close();
            }
        }

        private static void SubmitMakeReservationForm(WindowsDriver<WindowsElement> driver, string username)
        {
            driver.FindElementByAccessibilityId("MakeReservationUsernameTextBox").SendKeys(username);

            driver.FindElementByAccessibilityId("MakeReservationFloorNumberTextBox").Clear();
            driver.FindElementByAccessibilityId("MakeReservationFloorNumberTextBox").SendKeys("2");

            driver.FindElementByAccessibilityId("MakeReservationRoomNumberTextBox").Clear();
            driver.FindElementByAccessibilityId("MakeReservationRoomNumberTextBox").SendKeys("10");

            driver.FindElementByAccessibilityId("MakeReservationStartDatePicker").SendKeys("3/1/2023");

            driver.FindElementByAccessibilityId("MakeReservationEndDatePicker").SendKeys("3/2/2023");

            driver.FindElementByAccessibilityId("MakeReservationSubmitButton").Click();
            driver.FindElementByAccessibilityId("MakeReservationSubmitButton").Click();
        }

        private static WindowsDriver<WindowsElement> CreateAppDriver()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("app", "C:\\Storage\\VS Repos\\YouTube\\Reservoom\\src\\Reservoom\\bin\\Debug\\net5.0-windows\\win-x64\\Reservoom.exe");
            options.AddAdditionalCapability("appWorkingDir", "C:\\Storage\\VS Repos\\YouTube\\Reservoom\\src\\Reservoom\\bin\\Debug\\net5.0-windows\\win-x64");
            options.AddAdditionalCapability("appArguments", "E2E");

            WindowsDriver<WindowsElement> driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723/"),
                options);
            
            Thread.Sleep(3000);

            return driver;
        }
    }
}