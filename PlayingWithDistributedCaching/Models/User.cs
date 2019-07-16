using System;

namespace PlayingWithDistributedCaching.Models
{
  [Serializable]
  public class User
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
  }
}
