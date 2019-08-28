using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////

        //todo: allow any of the crud operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudoperation)
        {
            switch (crudoperation)
            {
                case "delete":
                    var employeeDelete = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber && e.LastName == employee.LastName).Single();
                    db.Employees.DeleteOnSubmit(employeeDelete);
                    db.SubmitChanges();
                    break;
                case "update":
                    var employeeUpdate = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).SingleOrDefault();

                    employeeUpdate.FirstName = employee.FirstName;
                    employeeUpdate.LastName = employee.LastName;
                    employeeUpdate.EmployeeNumber = employee.EmployeeNumber;
                    employeeUpdate.Email = employee.Email;

                    db.SubmitChanges();
                    break;
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    var employeeRead = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
                    List<string> employeeInformation = new List<string>() { employeeRead.FirstName, employeeRead.LastName, employeeRead.Email};
                    UserInterface.DisplayUserOptions(employeeInformation);
                    break;
                default:
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)//The result from the Method AddAnimal would come here and go to the table
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            var animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {

            var updateSelectedAnimal = db.Animals.Where(a => a.AnimalId == animalId).SingleOrDefault();
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        updateSelectedAnimal.CategoryId = GetCategoryId(update.Value);
                        break;
                    case 2:
                        updateSelectedAnimal.Name = update.Value;
                        break;
                    case 3:
                        updateSelectedAnimal.Age = int.Parse(update.Value);
                        break;
                    case 4:
                        updateSelectedAnimal.Demeanor = update.Value;
                        break;
                    case 5:
                        if (update.Value == "False" || update.Value == "0")
                        {
                            updateSelectedAnimal.KidFriendly = false;
                        }
                        else if (update.Value == "True" || update.Value == "1")
                        {
                            updateSelectedAnimal.KidFriendly = true;
                        }
                        break;
                    case 6:
                        if (update.Value == "False" || update.Value == "0")
                        {
                            updateSelectedAnimal.PetFriendly = false;
                        }
                        else if (update.Value == "True" || update.Value == "1")
                        {
                            updateSelectedAnimal.PetFriendly = true;
                        }
                        break;
                    case 7:
                        updateSelectedAnimal.Weight = int.Parse(update.Value);
                        break;
                    case 8:
                        updateSelectedAnimal.AnimalId = int.Parse(update.Value);
                        break;
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            var animalInDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            db.Animals.DeleteOnSubmit(animalInDb);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals;
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1: animals = db.Animals.Where(a => a.CategoryId == GetCategoryId(update.Value));
                        break;
                    case 2:
                        animals = db.Animals.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        animals = db.Animals.Where(a => a.Age == int.Parse(update.Value));
                        break;
                    case 4:
                        animals = db.Animals.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        animals = db.Animals.Where(a => a.KidFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 6:
                        animals = db.Animals.Where(a => a.PetFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 7:
                        animals = db.Animals.Where(a => a.Gender == update.Value);
                        break;
                    case 8:
                        animals = db.Animals.Where(a => a.Weight == int.Parse(update.Value)); ;
                        break;
                    default: Console.WriteLine("There were no animals that fir your search.");
                        break;
                }
            }
            return animals;
         
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryId = db.Categories.Where(c => c.Name.Equals(categoryName)).FirstOrDefault();
            return categoryId.CategoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(c => c.RoomId.Equals(animalId)).FirstOrDefault();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = db.DietPlans.Where(c => c.Name.Equals(dietPlanName)).FirstOrDefault();
            return dietPlanId.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            var adoptClient = db.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);
            var adoptAnimal = db.Animals.FirstOrDefault(a => a.AnimalId == animal.AnimalId);
            adoption.ClientId = adoptClient.ClientId;
            adoption.AnimalId = adoptAnimal.AnimalId;
            adoption.ApprovalStatus = "Waiting";
            adoption.AdoptionFee = null;
            adoption.PaymentCollected = false;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}