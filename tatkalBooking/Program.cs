using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace tatkalBooking
{
    class Program
    {
        static string userName = ConfigurationManager.AppSettings["username"];
        static string computerName = ConfigurationManager.AppSettings["ComputerName"];
        static string password = ConfigurationManager.AppSettings["password"];
        static string FromStation = ConfigurationManager.AppSettings["FromStation"];
        static string ToStation = ConfigurationManager.AppSettings["ToStation"];
        static string JourneyDate = ConfigurationManager.AppSettings["JourneyDate"];
        static string Quota = ConfigurationManager.AppSettings["Quota"];
        static string TrainNumber = ConfigurationManager.AppSettings["TrainNumber"];
        static string ClassToBook = ConfigurationManager.AppSettings["Class"];
        static string PassengerName = ConfigurationManager.AppSettings["PassengerName"];
        static string PassengerAge = ConfigurationManager.AppSettings["PassengerAge"];
        static string PassengerGender = ConfigurationManager.AppSettings["PassengerGender"];
        static string PassengerBerthPreference = ConfigurationManager.AppSettings["PassengerBerthPreference"];
        static string FoodChoice = ConfigurationManager.AppSettings["FoodChoice"];
        static string BoardingStation = ConfigurationManager.AppSettings["BoardingStation"];
        static string PassengerMobileNumber = ConfigurationManager.AppSettings["PassengerMobileNumber"];
        static string debitCardNumber = ConfigurationManager.AppSettings["DebitCardNumber"];
        static string debitCardMonth = ConfigurationManager.AppSettings["DebitCardMonth"];
        static string debitCardYear = ConfigurationManager.AppSettings["DebitCardYear"];
        static string nameOnCard = ConfigurationManager.AppSettings["NameOnCard"];
        static string atmPin = ConfigurationManager.AppSettings["AtmPin"];

        static void Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("user-data-dir=C:/Users/" + computerName + "/AppData/Local/Google/Chrome/User Data/Default");
            options.AddExtension("adblockplus.crx");
            ChromeDriver driver = new ChromeDriver(options);
    //        driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            login(driver);
            EnterTrainDetails(driver);
            SelectTrain(driver);
            FillPassengerDetails(driver);
            MakePayment(driver);
            EnterCardDetails(driver);
        }
        static void EnterCardDetails(ChromeDriver driver)
        {
            while(true)
            {
                IWebElement captcha = null;
                try
                {
                    captcha = driver.FindElementById("passline");
                }
                catch(Exception e)
                {
                    break;
                }

                IWebElement CardNumber = driver.FindElementById("debitCardNumber");
                CardNumber.SendKeys(debitCardNumber);

                IWebElement CardExpiryMonth = driver.FindElementByName("debiMonth");
                CardExpiryMonth.SendKeys(debitCardMonth);

                IWebElement CardExpiryYear = driver.FindElementByName("debiYear");
                CardExpiryYear.SendKeys(debitCardYear);

                IWebElement NameOnCard = driver.FindElementByName("debitCardholderName");
                NameOnCard.SendKeys(nameOnCard);

                IWebElement AtmPin = driver.FindElementById("cardPin");
                AtmPin.SendKeys(atmPin);

                Console.WriteLine("Enter Captcha");
                string Captcha = Console.ReadLine();
                captcha.SendKeys(Captcha);

                IWebElement Pay = driver.FindElementByName("proceed");
                Pay.Click();
            }
        }
        static void MakePayment(ChromeDriver driver)
        {
            WaitForElementAvailable(driver, By.Id("DEBIT_CARD"));
            IWebElement debitCard = driver.FindElementById("DEBIT_CARD");
                
            debitCard.Click();

            IWebElement sbiBank = driver.FindElementByXPath("//*[@id=\"DEBIT_CARD\" and @value=\"3\" and @type=\"radio\"]");
            sbiBank.Click();

            IWebElement makePayment = driver.FindElementById("validate");
            makePayment.Click();
        }
        static void FillPassengerDetails(ChromeDriver driver)
        {
            // //*[@id="addPassengerForm:psdetail:0"]/td[2]/input
            IWebElement PassengerNameCell = driver.FindElementByXPath("//*[@id=\"addPassengerForm:psdetail:0\"]/td[2]/input");
            PassengerNameCell.SendKeys(PassengerName);

            IWebElement boardingStation = driver.FindElementById("addPassengerForm:boardingStation");
            boardingStation.SendKeys(BoardingStation);

            IWebElement PassengerAgeCell = driver.FindElementById("addPassengerForm:psdetail:0:psgnAge");
            PassengerAgeCell.SendKeys(PassengerAge);

            IWebElement PassengerGenderCell = driver.FindElementById("addPassengerForm:psdetail:0:psgnGender");
            PassengerGenderCell.SendKeys(PassengerGender);

            IWebElement PassengerBerthCell = driver.FindElementById("addPassengerForm:psdetail:0:berthChoice");
            PassengerBerthCell.SendKeys(PassengerBerthPreference);

            IWebElement PassengerMobileNumberInputTextbox = driver.FindElementById("addPassengerForm:mobileNo");
            PassengerMobileNumberInputTextbox.Clear();
            PassengerMobileNumberInputTextbox.SendKeys(PassengerMobileNumber);

            try
            {
                IWebElement PassengerFoodChoice = driver.FindElementById("addPassengerForm:psdetail:0:foodChoice");
                PassengerFoodChoice.SendKeys(FoodChoice);
            }
            catch (NoSuchElementException e)
            { }

            IWebElement autoUpgrade = driver.FindElementById("addPassengerForm:autoUpgrade");
            autoUpgrade.Click();

            try
            {
                IWebElement confirmBerths = driver.FindElementById("addPassengerForm:onlyConfirmBerths");
                confirmBerths.Click();
            }
            catch (Exception e)
            {
                Console.WriteLine("Confirm berths are not alloted !!! \n PRESS ANY KEY TO CONTINUE BOOKING OR CLOSE THE CONSOLE WINDOW TO STOP");
                Console.ReadKey();
            }

            while (true)
            {
                Console.WriteLine("Enter Captcha");
                string captcha = Console.ReadLine();

                IWebElement Captcha = driver.FindElementById("nlpAnswer");
                Captcha.SendKeys(captcha);

                IWebElement next = driver.FindElementById("validate");
                next.Click();

                try
                {
                    IWebElement popUp = driver.FindElementById("addPassengerForm:waitOnPPPopup");

                    while (!popUp.GetAttribute("style").Contains("hidden"))
                        Thread.Sleep(250);
                }
                catch (Exception e)
                { }

                try
                {
                    Captcha = driver.FindElementById("nlpAnswer");
                    continue;
                }
                catch(Exception e)
                {
                    break;
                }
                    
            }
        }
        static void SelectTrain(ChromeDriver driver)
        {
            string selectQuotaString = null;
            if (Quota.Equals("GN"))
                selectQuotaString = "//*[@id=\"qcbd\"]/table/tbody/tr/td[1]/input";
            else if (Quota.Equals("CK"))
                selectQuotaString = "//*[@id=\"qcbd\"]/table/tbody/tr/td[5]/input";

            IWebElement selectQuota = driver.FindElementByXPath(selectQuotaString);
            IWebElement trainTable = driver.FindElementById("avlAndFareForm:trainbtwnstns:tb");
            IList<IWebElement> allrows = trainTable.FindElements(By.TagName("tr"));
            selectQuota.Click();
            foreach (IWebElement row in allrows)
            {
                IWebElement anchorTag = row.FindElement(By.XPath("td[1]/a"));
                if(anchorTag.Text.Equals(TrainNumber))
                {
                    IList<IWebElement> columns = row.FindElements(By.TagName("td"));
                    IWebElement ClassCell = columns[columns.Count - 1];
                    IList<IWebElement> anchorTagList = ClassCell.FindElements(By.TagName("a"));
                    foreach(IWebElement Class in anchorTagList)
                    {
                        if(Class.Text.Equals(ClassToBook))
                        {
                            Class.Click();

                            IWebElement bookNowForm = driver.FindElementById("avlAndFareForm");

                            WaitForElementPresentInWebElement(bookNowForm , By.Id("c1"));
                    //        IWebElement bookNowColumn = bookNowForm.FindElement(By.Id("c1"));
                            IWebElement bookNowDivColumn = bookNowForm.FindElement(By.XPath("//*[@id=\"c1\"]/div[1]/div[2]"));

                            string bookNowIdXpath = "table/tbody/tr[2]/td[" + GetColumnNumberOfTrainAvailabilty(bookNowDivColumn) + "]/a";
                            
                            IWebElement bookNow = bookNowDivColumn.FindElement(By.XPath(bookNowIdXpath));
                            bookNow.Click();
                            return;
                        }
                    }
                }
            }
        }
        static void WaitForElementPresentInWebElement(IWebElement element , By by)
        {
            while(true)
            {
                try
                {
                    element.FindElement(by);
                    return;
                }
                catch(NoSuchElementException e)
                {
                    Thread.Sleep(150);
                    continue;
                }
            }
        }
        static void WaitForElementAvailable(ChromeDriver driver , By by)
        {
            while(true)
            {
                try
                {
                    driver.FindElement(by);
                    return ;
                }
                catch(NoSuchElementException e)
                {
                    Thread.Sleep(250);
                    continue;
                }
            }

        }
        static int GetColumnNumberOfTrainAvailabilty(IWebElement Avlform)
        {
            int num = 1;
            //*[@id="j_idt335_body"]/table/tbody/tr[1]
            string Xpath = "table/tbody/tr[1]";
            IWebElement row = Avlform.FindElement(By.XPath(Xpath));
            IList<IWebElement> columnList = row.FindElements(By.TagName("td"));
            foreach(IWebElement column in columnList)
            {
                string temp = column.GetAttribute("innerHTML");
                if (temp.Contains(JourneyDate))
                    break;
                num++;
            }
            return num;
        } 
        static void EnterTrainDetails(ChromeDriver driver)
        {
            IWebElement fromStation = driver.FindElement(By.Id("jpform:fromStation"));
            IWebElement toStation = driver.FindElement(By.Id("jpform:toStation"));
            IWebElement journeyDate = driver.FindElement(By.Id("jpform:journeyDateInputDate"));
            IWebElement submit = driver.FindElement(By.Id("jpform:jpsubmit"));

            fromStation.SendKeys(FromStation);
            toStation.SendKeys(ToStation);
            journeyDate.SendKeys(JourneyDate);
            submit.Click();
        }
        static void login(ChromeDriver driver)
        {
            driver.Url = "https://www.irctc.co.in";
            driver.Navigate();
            

            while (true)
            {
                IWebElement username = driver.FindElement(By.Id("usernameId"));
                IWebElement pass = driver.FindElement(By.Name("j_password"));
                IWebElement loginButton = driver.FindElement(By.Id("loginbutton"));
                IWebElement Captcha = driver.FindElement(By.Name("j_captcha"));
                username.SendKeys(userName);
                pass.SendKeys(password);
                
                Console.WriteLine("Enter the captcha for this page");
                string captcha = Console.ReadLine();
                Captcha.SendKeys(captcha);
                loginButton.Click();
                try
                {
                    IWebElement incorrectLogin = driver.FindElementById("loginerrorpanelok");
                    incorrectLogin.Click();
                    continue;
                }
                catch(NoSuchElementException e)
                {
                    break;
                }
            }
        }
    }
}