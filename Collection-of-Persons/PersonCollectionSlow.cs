using System;
using System.Collections.Generic;

namespace Collection_of_Persons
{
    using System.Linq;
    using Interfaces;
    using Models;

    public class PersonCollectionSlow : IPersonCollection
    {
        private IList<Person> persons = new List<Person>(); 

        public bool AddPerson(string email, string name, int age, string town)
        {
            var newPerson = new Person(email, name, age, town);
            if (this.persons.Any(p => p.Email == email))
            {
                return false;
            }

            this.persons.Add(newPerson);
            return true;
        }

        public int Count
        {
            get
            {
                return this.persons.Count;
            }
        }

        public Person FindPerson(string email)
        {
            var person = this.persons.FirstOrDefault(p => p.Email == email);

            return person;
        }

        public bool DeletePerson(string email)
        {
            var person = this.FindPerson(email);
            if (person != null)
            {
                this.persons.Remove(person);
                return true;
            }

            return false;
        }

        public IEnumerable<Person> FindPersons(string emailDomain)
        {
            var personsResult = this.persons
                .Where(p => p.Email.EndsWith("@" + emailDomain))
                .OrderBy(p => p.Email);

            return personsResult;
        }

        public IEnumerable<Person> FindPersons(string name, string town)
        {
            var personsResult = this.persons
                .Where(p => p.Name == name && p.Town == town)
                .OrderBy(p => p.Email);

            return personsResult;
        }

        public IEnumerable<Person> FindPersons(int startAge, int endAge)
        {
            var personsResult = this.persons
                .Where(p => p.Age >= startAge && p.Age <= endAge)
                .OrderBy(p => p.Age)
                .ThenBy(p => p.Email);

            return personsResult;
        }

        public IEnumerable<Person> FindPersons(
            int startAge, int endAge, string town)
        {
            var personsResult = this.persons
                .Where(p => p.Age >= startAge && p.Age <= endAge && p.Town == town)
                .OrderBy(p => p.Age)
                .ThenBy(p => p.Email);

            return personsResult;
        }
    }
}
