using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    class Person
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime BirthDate { get; private set; }

        public List<PersonRelation> Relations { get; private set; }

        public Person(int id, string firstName, string lastName, DateTime birthDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public string GetRelation(Person person)
        {
            foreach(var relation in Relations)
            {
                if(relation.Person1 == person || relation.Person2 == person)
                {
                    if(relation.RelationType == RelationType.Parent)
                    {
                        if(relation.Person1 == person) return "parent";
                        else                           return "child";
                    }
                    else
                    {
                        return relation.RelationType.ToString("G");
                    }
                }
            }

            return string.Empty;
        }
    }

    class PersonRelation
    {
        public Person Person1;
        public Person Person2;
        public RelationType RelationType;
    }

    enum RelationType
    {
        Spouse, Sibling, Parent
    }


    class Program
    {
        public static void Main()
        {
            string[] info = File.ReadAllLines("input.txt");
            var list = ReadPeople(info);
        }
        
        public static List<Person> ReadPeople(string[] info)
        {
            var result = new List<Person>();

            int index = 0;
            string[] pattern = { "Id", "FirstName", "LastName", "BirthDate" };
            while (info[index] != string.Empty)
            {
                if(index == 0)
                {
                    pattern = info[index].Split(";");
                }

                result.Add(ReadPerson(info[index], pattern));
            }
            index++;

            for(int i = index; i < info.Length; i++)
            {
                FillPersonRelation(info[i], result);
            }

            return result;
        }

        private static Person ReadPerson(string personInfo, string[] pattern)
        {
            int id = -1;
            string firstName = string.Empty, lastName = string.Empty;
            DateTime birthDate = DateTime.MinValue;

            var parts = personInfo.Split(";");
            for(int i = 0; i < pattern.Length; i++)
            {
                
                if(parts[i] == "Id")
                {
                    id = int.Parse(parts[i]);
                }
                else if (parts[i] == "FirstName")
                {
                    firstName = parts[i];
                }
                else if(parts[i] == "LastName")
                {
                    lastName = parts[i];
                } 
                else if(parts[i] == "BirthDate")
                {
                    birthDate = DateTime.Parse(parts[i]);
                }
            }

            return new Person(id, firstName, lastName, birthDate);
        }

        private static void FillPersonRelation(string personRelationInfo, List<Person> persons)
        {
            // 3<->4=spouse
            string[] parts1 = personRelationInfo.Split("<->");
            // 3 , 4=spouse
            string[] parts2 = parts1[0].Split("=");
            // 4 , spouse

            int person1Id = int.Parse(parts1[0]);
            int person2Id = int.Parse(parts2[0]);
            string relationType = parts2[1];

            PersonRelation relation = new PersonRelation();
            relation.RelationType = GetRelationType(relationType);
            foreach(var person in persons)
            {
                if(person.Id == person1Id)
                {
                    relation.Person1 = person;
                    person.Relations.Add(relation);
                } 
                else if(person.Id == person2Id)
                {
                    relation.Person2 = person;
                    person.Relations.Add(relation);
                }
            }
        }

        private static RelationType GetRelationType(string relation)
        {
            return relation == "spouse"
                ? RelationType.Spouse
                : relation == "sibling"
                    ? RelationType.Sibling
                    : RelationType.Parent;
        }
    }
 }
