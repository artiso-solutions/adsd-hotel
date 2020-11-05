namespace artiso.AdsdHotel.Black.Contracts
{
    public class GuestInformation
    {
        public GuestInformation(string firstName, string lastName, string eMail)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new System.ArgumentException($"'{nameof(firstName)}' cannot be null or empty", nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new System.ArgumentException($"'{nameof(lastName)}' cannot be null or empty", nameof(lastName));
            }

            if (string.IsNullOrEmpty(eMail))
            {
                throw new System.ArgumentException($"'{nameof(eMail)}' cannot be null or empty", nameof(eMail));
            }

            FirstName = firstName;
            LastName = lastName;
            EMail = eMail;
        }

        public string FirstName { get;  }
        public string LastName { get;  }
        public string EMail { get;  }
    }
}
