using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Models
{
    public class User
    {
        private int id;
        private String name;
        private String firstName;
        private String email;
        private String userName;
        private String birthplace;
        private DateTime birthday;
        private int addressID;
        private int userroleID; //noch nicht sicher wie wir das mit der Rollenzuweisung machen sollen... Evtl Vererbung

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string Email { get => email; set => email = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Birthplace { get => birthplace; set => birthplace = value; }
        public DateTime Birthday { get => birthday; set => birthday = value; }
        public int AddressID { get => addressID; set => addressID = value; }
        public int UserroleID { get => userroleID; set => userroleID = value; }

        // Constructors
        public User (String name, String firstName, String email, String userName, String birthplace, DateTime birthday, int addressID, int userroleID )
        {
            this.id = 0; //Get Id from DB after Creation
            this.name = name;
            this.firstName = firstName;
            this.email = email;
            this.userName = userName;
            this.birthplace = birthplace;
            this.birthday = birthday;
            this.addressID = addressID;
            this.userroleID = userroleID;
        }
        public User (int id)
        {
            this.id = id;
            // DB Abfrage für User!
        }


        //end Constructors
    }
}
