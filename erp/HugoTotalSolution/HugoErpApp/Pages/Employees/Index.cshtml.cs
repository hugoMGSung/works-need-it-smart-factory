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
            // �α��ε� ����ڸ� ���� ����
        }
    }
}
