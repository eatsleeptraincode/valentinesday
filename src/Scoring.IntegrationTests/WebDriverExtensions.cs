using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using OpenQA.Selenium;

namespace Scoring.IntegrationTests
{
    public static class WebDriverExtensions
    {
        public static void ClickLink(this IWebDriver driver, string linkText)
        {
            var link = driver.FindElement(By.LinkText(linkText));
            link.Click();
        }

        public static void FillIn<T>(this IWebDriver driver, Expression<Func<T,object>> expr, object text)
        {
            var fieldName = ReflectionHelper.GetAccessor(expr).Name;
            var field = driver.FindElement(By.Name(fieldName));
            field.Clear();
            field.SendKeys(text.ToString());
        }

        public static void ClickButton(this IWebDriver driver)
        {
            var button = driver.FindElement(By.CssSelector("input[type=\"submit\"]"));
            button.Click();
        }
    }
}