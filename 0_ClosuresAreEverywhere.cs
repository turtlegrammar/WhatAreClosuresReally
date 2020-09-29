using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Closures
{
    public class ClosuresAreEverywhereExample
    {

        public class Person
        {
            public string Name;
            public string PhoneNumber;
        }

        string PhoneNumberForPerson(string name, List<Person> people)
        {
            return people.Where(p => p.Name == name).FirstOrDefault()?.PhoneNumber;
        }

        string PhoneNumberForPerson2(string name, List<Person> people)
        {
            Func<Person, bool> IsRightPerson = p => p.Name == name;
            return people.Where(IsRightPerson).FirstOrDefault()?.PhoneNumber;
        }
    }
}
