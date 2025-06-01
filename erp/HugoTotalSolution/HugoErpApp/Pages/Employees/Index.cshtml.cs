using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HugoErpApp.Pages.Employees
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // 로그인된 사용자만 접근 가능
        }
    }
}
