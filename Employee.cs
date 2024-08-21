public class Employee {
    public string? Name { get; set;}
    public int Id { get; set; }
    public bool IsManager { get; set; }
    public SocialSecurity? SSN { get; set;}
}

public class SocialSecurity {
    public string number() {
        return "XXX-XXX-XXXX";
    }

    public int id { get; set; }
}