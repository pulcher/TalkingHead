using FluentAssertions;
using FluentAssertions.Extensions;
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
        private IOptions<CoolDownOptions> options 
            = Substitute.For<IOptions<CoolDownOptions>>();

        [TestMethod]
        public void CanInstance()
        {
            var cot = new CoolDownService(options);

            cot.Should().NotBeNull();
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExcutedSetsCoolDown()
        {
            options.Value.Returns(SetupOptions(2000));
            var cot = new CoolDownService(options);

            cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull();
            results.Should().HaveCount(1);
            results.Keys.Should().Contain("dad");
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExcutedMultipleTimesSetsCoolDown()
        {
            options.Value.Returns(SetupOptions(2000));
            var cot = new CoolDownService(options);

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
            options.Value.Returns(SetupOptions(2000));
            var cot = new CoolDownService(options);

            var expectedDateTime  = cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull()
                .And.ContainKey("dad");

            var diffDate = results["dad"] - expectedDateTime;
            diffDate.Should().BeLessThan(1.Seconds());
        }

        [TestMethod]
        public void GivenCommandExecutionWhenExecutedThenCoolDownDoesNotHaveUnexpectedKey()
        {
            options.Value.Returns(SetupOptions(2000));
            var cot = new CoolDownService(options);

            var expectedDateTime = cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotContainKey("dad2");
        }

        [TestMethod]
        public void GivenCommandExecutionTwiceWhenExecutedThenCoolDownsContainsCorrectEntry()
        {
            options.Value.Returns(SetupOptions(2000));
            var cot = new CoolDownService(options);

            var expectedDateTime = cot.Execute("dad");

            cot.Execute("dad");

            var results = cot.GetAllCoolDowns();

            results.Should().NotBeNull()
                .And.ContainKey("dad");

            var diffDate = results["dad"] - expectedDateTime;
            diffDate.Should().BeLessThan(1.Seconds());
        }

        private CoolDownOptions SetupOptions(int delay)
        {
            var newOptions = new Dictionary<string, int>
            {
                { "dad", delay}
            };

            return new CoolDownOptions
            {
                Options = newOptions
            };
        }
    }
}
