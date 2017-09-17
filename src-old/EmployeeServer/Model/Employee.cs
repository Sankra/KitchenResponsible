﻿namespace KitchenResponsible.Model
{
    public class Employee
    {
        public Employee(int id, ushort weekReponsible, string firstName, string lastName, string nick = null)
        {
            Id = id;
            WeekResponsible = weekReponsible;
            FirstName = firstName;
            LastName = lastName;
            Nick = nick ?? firstName;

            JobTitle = "UX Utvikler";
            Company = "Health Angels";
            Email = "roh@dips.no";
            Phone = "98817218";

            SmallPhotoUrl = "http://hjerpbakk.com/img/About/0Profilbilde.jpg";
        }

        public int Id { get; }
        public ushort WeekResponsible { get; set; }
        public string Nick { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Company { get; }
        public string JobTitle { get; }
        public string Email { get; }
        public string Phone { get; }
        public string SmallPhotoUrl { get; }
        
        public override string ToString() => $"{Nick} {WeekResponsible}";
    }
}