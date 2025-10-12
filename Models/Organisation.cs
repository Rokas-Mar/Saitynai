public class Organisation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string IBAN { get; set; }
    public string Number { get; set; }
    public string CompanyCode { get; set; }

    public List<User> Users { get; set; }
}