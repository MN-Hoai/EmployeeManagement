using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Service.EmployeeMangement.Executes;
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

        // ----------------- GET: SignIn -----------------
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult SignIn(string returnUrl = null)
        {
            // Nếu người dùng đã đăng nhập → redirect sang Employee List
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("List", "Employee");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ----------------- POST: SignIn -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(AccountModel.AccountRequest request, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                ViewBag.ReturnUrl = returnUrl;
                return View(request);
            }

            var result = await _accountCommand.CheckAccount(request);
            if (!result)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                ViewBag.ReturnUrl = returnUrl;
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

            // Redirect về ReturnUrl nếu hợp lệ, nếu không → Employee List
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("List", "Employee");
        }

        // ----------------- API Login View -----------------
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

        // ----------------- Logout -----------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Sign out khỏi cookie authentication
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("SignIn", "Account");
        }
    }
}
