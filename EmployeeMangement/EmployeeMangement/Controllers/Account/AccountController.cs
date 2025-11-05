using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Service.EmployeeMangement.Executes;
using Service.EmployeeMangement.Executes.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeMangement.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountCommand _accountCommand;

        public AccountController(AccountCommand accountCommand)
        {
            _accountCommand = accountCommand;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(AccountModel.AccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return View(request);
            }

            var result = await _accountCommand.CheckAccount(request);
            if (!result)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View(request);
            }
            var account = await _accountCommand.GetAccountByEmail(request.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email ?? ""),
                new Claim(ClaimTypes.Name, account.Fullname ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "login");
            await HttpContext.SignInAsync(
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(2)
                });

            return RedirectToAction("List", "Employee");
        }

        // GET: view form API
        [HttpGet("api/account/sign-in-view")]
        public IActionResult ApiSignInView()
        {
            return View("ApiSignIn");
        }

        [HttpPost("api/account/sign-in-view")]
        public async Task<IActionResult> ApiSignInViewPost(AccountModel.AccountRequest request)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = "Thiếu thông tin đăng nhập." });

            var result = await _accountCommand.CheckAccount(request);
            if (!result)
                return Ok(new { success = false, message = "Sai email hoặc mật khẩu." });

            var account = await _accountCommand.GetAccountByEmail(request.Email);

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công",
                data = new
                {
                    Id = account.Id,
                    Email = account.Email,
                    Fullname = account.Fullname
                }
            });
        }


        // Logout
        public async Task<IActionResult> Logout()
        {
            await _accountCommand.Logout();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("SignIn", "Account");
        }
    }
}