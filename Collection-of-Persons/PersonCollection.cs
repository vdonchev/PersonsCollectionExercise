using System;
using System.Collections.Generic;

namespace Collection_of_Persons
{
    using Extensions;
    using Interfaces;
    using Models;
    using Wintellect.PowerCollections;

    public class PersonCollection : IPersonCollection
    {
        // Persons by email
        private IDictionary<string, Person> personsByEmail =
            new Dictionary<string, Person>();

        // Persons by email domain
        private IDictionary<string, SortedSet<Person>> personsByEmailDomain =
           new Dictionary<string, SortedSet<Person>>();

        // Persons by name and town
        private IDictionary<string, SortedSet<Person>> personsByNameAndTown =
            new Dictionary<string, SortedSet<Person>>();

        // Persons by age range
        private OrderedDictionary<int, SortedSet<Person>> personsByAge =
            new OrderedDictionary<int, SortedSet<Person>>();

        // Persons by town and age range
        private Dictionary<string, OrderedDictionary<int, SortedSet<Person>>> personsByTownAndAge =
            new Dictionary<string, OrderedDictionary<int, SortedSet<Person>>>();

        public bool AddPerson(string email, string name, int age, string town)
        {
            if (this.personsByEmail.ContainsKey(email))
            {
                return false;
            }

            var person = new Person(email, name, age, town);
            this.personsByEmail.Add(email, person);

            var emailDomain = this.ExtractEmailDomain(email);
            this.personsByEmailDomain.AppendValueToKey(emailDomain, person);

            var combinedNameAndTown = this.CombineNameAndTown(name, town);
            this.personsByNameAndTown.AppendValueToKey(combinedNameAndTown, person);

            this.personsByAge.AppendValueToKey(age, person);

            this.personsByTownAndAge.EnsureKeyExists(town);
            this.personsByTownAndAge[town].AppendValueToKey(age, person);

            return true;
        }

        public int Count
        {
            get
            {
                return this.personsByEmail.Count;
            }
        }

        public Person FindPerson(string email)
        {
            Person person;
            this.personsByEmail.TryGetValue(email, out person);

            return person;
        }

        public bool DeletePerson(string email)
        {
            var person = this.FindPerson(email);
            if (person == null)
            {
                return false;
            }

            this.personsByEmail.Remove(email);

            var emailDomain = this.ExtractEmailDomain(email);
            this.personsByEmailDomain[emailDomain].Remove(person);

            var combinedNameAndTown = this.CombineNameAndTown(person.Name, person.Town);
            this.personsByNameAndTown[combinedNameAndTown].Remove(person);

            this.personsByAge[person.Age].Remove(person);

            this.personsByTownAndAge[person.Town][person.Age].Remove(person);

            return true;
        }

        public IEnumerable<Person> FindPersons(string emailDomain)
        {
            return this.personsByEmailDomain.GetValuesForKey(emailDomain);
        }

        public IEnumerable<Person> FindPersons(string name, string town)
        {
            var combinedNameAndTown = this.CombineNameAndTown(name, town);

            return this.personsByNameAndTown.GetValuesForKey(combinedNameAndTown);
        }

        public IEnumerable<Person> FindPersons(int startAge, int endAge)
        {
            var personsInRange = this.personsByAge.Range(startAge, true, endAge, true);
            foreach (var personsByAgeList in personsInRange)
            {
                foreach (var person in personsByAgeList.Value)
                {
                    yield return person;
                }
            }
        }

        public IEnumerable<Person> FindPersons(
            int startAge, int endAge, string town)
        {
            if (!this.personsByTownAndAge.ContainsKey(town))
            {
                yield break;
            }

            var personsInRange = this.personsByTownAndAge[town]
                .Range(startAge, true, endAge, true);

            foreach (var personsByAgeList in personsInRange)
            {
                foreach (var person in personsByAgeList.Value)
                {
                    yield return person;
                }
            }
        }

        private string ExtractEmailDomain(string email)
        {
            var emailDomain = email.Split('@')[1];

            return emailDomain;
        }

        private string CombineNameAndTown(string name, string town)
        {
            var combined = name + "|!|" + town;

            return combined;
        }
    }
}
