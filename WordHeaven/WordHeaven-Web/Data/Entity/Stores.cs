namespace WordHeaven_Web.Data.Entity
{
    public class Stores
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        //Looking like that: Lisbon, Portugal. For now, later we can add a country and city class.
    }
}
