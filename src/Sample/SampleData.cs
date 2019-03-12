// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.StructureEditor.Sample.Model;

namespace NanoByte.StructureEditor.Sample
{
    public static class SampleData
    {
        public static AddressBook AddressBook => new AddressBook
        {
            Name = "My Address Book",
            Groups =
            {
                new Group
                {
                    Name = "Scientists",
                    Contacts =
                    {
                        new Contact
                        {
                            FirstName = "Marie",
                            LastName = "Curie",
                            WorkAddress = new Address
                            {
                                Street = "1 Rue Victor Cousin",
                                City = "Paris",
                                Country = "France"
                            }
                        },
                        new Contact
                        {
                            FirstName = "Albert",
                            LastName = "Einstein",
                            HomeAddress = new Address
                            {
                                Street = "112 Mercer Street",
                                City = "New Jersey",
                                Country = "United States"
                            },
                            PhoneNumbers =
                            {
                                new LandlineNumber {CountryCode = "+1", AreaCode = "555", LocalNumber = "0200"}
                            }
                        }
                    }
                }
            },
            Contacts =
            {
                new Contact
                {
                    FirstName = "John",
                    LastName = "Doe",
                    HomeAddress = new Address
                    {
                        Street = "123 Fake Street",
                        City = "New York",
                        Country = "United States"
                    },
                    WorkAddress = new Address
                    {
                        Street = "456 Fake Street",
                        City = "New York",
                        Country = "United States"
                    },
                    PhoneNumbers =
                    {
                        new LandlineNumber {CountryCode = "+1", AreaCode = "555", LocalNumber = "0101"},
                        new MobileNumber {CountryCode = "+1", AreaCode = "555", LocalNumber = "0102"},
                        new MobileNumber {CountryCode = "+1", AreaCode = "555", LocalNumber = "0103"}
                    }
                }
            }
        };
    }
}
