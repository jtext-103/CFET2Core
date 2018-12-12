using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Event;
using FluentAssertions;

namespace Jtext103.CFET2.Core.Test.Event
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EventFilterTest
    {
        public EventFilterTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestInitialize]
        public void init()
        {

        }

       
        [TestMethod]
        public void ResourceRegexShouldMatch()
        {
            //arrange
            //t for thing, s for status and etc.
            var fExactMatch = new EventFilter(@"/T/t2/s1","changed");
            var fStartEndMatch = new EventFilter(@"/T(/(\w)+)*/s1", "changed"); //"/t/any level/s1"
            var fPartsMatch = new EventFilter(@"(\/(\w)+)*\/T(\/(\w)+)*\/s1", "changed");//"/any level/t/any level/s1"

            //act
            var rfExactMatch = fExactMatch.Predicate(new EventArg(@"/t/t2/s1", "changed", null));
            var rfStartEndMatch = fStartEndMatch.Predicate(new EventArg(@"/t/t2/S1", "changed", null));
            var r2fStartEndMatch = fStartEndMatch.Predicate(new EventArg(@"/T/t/t/t2/ty/t/s1", "changed", null));
            var rfPartsMatch = fPartsMatch.Predicate(new EventArg(@"/aa3/Dfe3/T/t2/s1", "changed", null));
            var r2fPartsMatch = fPartsMatch.Predicate(new EventArg(@"/aa3/t/Dfe3/t/t2/t/S1", "changed", null));
            //not match
            var nrfExactMatch = fExactMatch.Predicate(new EventArg(@"/ts/t2/S1", "changed", null));

            var nrfStartEndMatch = fStartEndMatch.Predicate(new EventArg(@"/gh/t/t2/s1", "changed", null));
            var n2rfStartEndMatch = fStartEndMatch.Predicate(new EventArg(@"/tt/t2/s1", "changed", null));

            var nrfPartsMatch = fPartsMatch.Predicate(new EventArg(@"/aa3/tdd/Dfe3/td/t2/td/s1", "changed", null));
            var nr2fPartsMatch = fPartsMatch.Predicate(new EventArg(@"/aa3/t/Dfe3/t/t2/t/s1/s2", "changed", null));

            //assert
            rfExactMatch.Should().BeTrue();
            rfStartEndMatch.Should().BeTrue();
            r2fStartEndMatch.Should().BeTrue();
            rfPartsMatch.Should().BeTrue();
            r2fPartsMatch.Should().BeTrue();

            nrfExactMatch.Should().BeFalse();
            
            nrfStartEndMatch.Should().BeFalse();
            n2rfStartEndMatch.Should().BeFalse();

            nrfPartsMatch.Should().BeFalse();
            nr2fPartsMatch.Should().BeFalse();




        }


        [TestMethod]
        public void EventRegexShouldMatch()
        {
            //arrange
            //t for thing, s for status and etc.
            var fExactMatch = new EventFilter(@"/T/t2/s1", "changed");
            var fStartMatch = new EventFilter(@"/T/t2/s1", @"(\w)*changed"); //"/t/any level/s1"
            
            //act
            var rfExactMatch = fExactMatch.Predicate(new EventArg(@"/t/t2/s1", "changed", null));
            var rfStartMatch = fStartMatch.Predicate(new EventArg(@"/t/t2/s1", "ValueChanged", null));
            var r2fStartMatch = fStartMatch.Predicate(new EventArg(@"/t/t2/s1", "Changed", null));
            
            //not match
            var nrfExactMatch = fExactMatch.Predicate(new EventArg(@"/ts/t2/s1", "NotChanged", null));
            var nrfStartMatch = fStartMatch.Predicate(new EventArg(@"/t/t2/s1", "ChangedValue", null));


            //assert
            rfExactMatch.Should().BeTrue();
            rfStartMatch.Should().BeTrue();
            r2fStartMatch.Should().BeTrue();
            
            nrfExactMatch.Should().BeFalse();

            nrfStartMatch.Should().BeFalse();
            




        }
    }
}
