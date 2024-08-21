public class ReviewDTO {
    public int EmployeId { get; set; }
    public string? Date { get; set; }
    public int Id { get; set; }
    public bool IsComplete { get; set; }
    
    public ReviewDTO() {}
    public ReviewDTO(Review review) => 
    (Id, Date, IsComplete, EmployeId) = (review.Id, review.Date, review.IsComplete, review.EmployeId);
}