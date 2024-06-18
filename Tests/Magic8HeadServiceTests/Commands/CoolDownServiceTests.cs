using FluentAssertions;
using Magic8HeadService.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic8HeadServiceTests
{
    [TestClass]
    public class CoolDownServiceTests
    {
        private IOptionsSnapshot<CoolDownOptions> optionsSnapshot 
            = Substitute.For<IOptionsSnapshot<CoolDownOptions>>();

        [TestMethod]
        public void CanInstance()
        {
            var cot = new CoolDownService(optionsSnapshot);

            cot.Should().NotBeNull();
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExcutedSetsCoolDown()
        {
            var cot = new CoolDownService(optionsSnapshot);

            cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull();
            results.Should().HaveCount(1);
            results.Keys.Should().Contain("dad");
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExcutedMultipleTimesSetsCoolDown()
        {
            var cot = new CoolDownService(optionsSnapshot);

            cot.Execute("dad");
            cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull();
            results.Should().HaveCount(1);
            results.Keys.Should().Contain("dad");
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExecutedThenCoolDownHasExpectedTime()
        {
            optionsSnapshot.Value.Returns(new CoolDownOptions { /* set your properties here */ });
            var cot = new CoolDownService(optionsSnapshot);

            cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull();
            results.Keys.Should().Contain("dad");
            results["dad"].Should().Be(DateTime.Now);
        }
    }
}
