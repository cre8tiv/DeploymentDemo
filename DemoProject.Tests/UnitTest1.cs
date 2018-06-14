using DemoProject.Controllers;
using Xunit;

namespace DemoProject.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void GetEmployeeName()
        {
            HomeController home = new HomeController();
            string result = home.GetEmployeeName(1);
            Assert.Equal("Bob", result);
        }
    }
}