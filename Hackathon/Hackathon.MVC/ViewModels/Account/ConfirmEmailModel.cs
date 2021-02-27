using Microsoft.AspNetCore.Mvc;

namespace Hackathon.MVC.ViewModels.Account
{
    public class ConfirmEmailModel
    {
        [TempData] public string StatusMessage { get; set; }
    }
}