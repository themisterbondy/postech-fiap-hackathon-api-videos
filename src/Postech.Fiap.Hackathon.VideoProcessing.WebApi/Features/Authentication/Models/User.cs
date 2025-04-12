using Microsoft.AspNetCore.Identity;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;

public class User : IdentityUser
{
    public string? Initials { get; set; }
}