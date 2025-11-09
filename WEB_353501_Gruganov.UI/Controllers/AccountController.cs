using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEB_353501_Gruganov.UI.HelperClasses;
using WEB_353501_Gruganov.UI.Models;
using WEB_353501_Gruganov.UI.Services.Authentication;
using WEB_353501_Gruganov.UI.Services.FileService;

namespace WEB_353501_Gruganov.UI.Controllers;

public class AccountController(
    HttpClient httpClient,
    IHttpContextAccessor contextAccessor,
    ITokenAccessor tokenAccessor,
    IOptions<KeycloakData> options,
    IFileService fileService) : Controller
{
    // GET
    public IActionResult Register()
    {
        return View(new RegisterUserViewModel());
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Register(RegisterUserViewModel? user)
    {
        if (ModelState.IsValid) {
            if (user == null) {
                return BadRequest();
            }

            try {
                await tokenAccessor.SetAuthorizationHeaderAsync(httpClient, true);
            }
            catch (Exception ex) {
                return Unauthorized();
            }

            var avatarUrl = "/Images/avatar.png";
            if (user.Avatar != null) {
                avatarUrl = await fileService.SaveFileAsync(user.Avatar);
            }

            var newUser = new CreateUserModel();
            newUser.Attributes.Add("avatar", avatarUrl);
            newUser.Email = user.Email;
            newUser.Username = user.Email;
            newUser.Credentials.Add(new UserCredentials(){Value = user.Password});

            var requestUri = $"{options.Value.Host}/admin/realms/{options.Value.Realm}/users";
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var userData = JsonSerializer.Serialize(newUser, serializerOptions);
            HttpContent content = new StringContent(userData, Encoding.UTF8,
                "application/json");
            
            var response = await httpClient.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode) {
                return Redirect(Url.Action("Index", "Home"));
            }
            else {
                return BadRequest(response.StatusCode);
            }
        }

        return View(user);
    }
}

class CreateUserModel
{
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string Username { get; set; }
    public string Email { get; set; }
    public bool Enabled { get; set; } = true;
    public bool EmailVerified { get; set; } = true;
    public List<UserCredentials> Credentials { get; set; } = new();
}

class UserCredentials
{
    public string Type { get; set; } = "password";
    public bool Temporary { get; set; } = false;
    public string Value { get; set; }
}