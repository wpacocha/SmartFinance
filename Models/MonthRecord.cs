using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartFinance.API.Models;

public class Month
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int MonthNumber { get; set; }
    public int UserId { get; set; }  // <-- to musi być tutaj
    [JsonIgnore]
    [ValidateNever]
    public User? User { get; set; }   // <-- możesz dodać virtual, ale niekonieczne
}

