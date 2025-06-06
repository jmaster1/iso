using IsoNet.Iso.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web2.Pages;

public class IndexModel(
    IsoServer isoServer
    ) : PageModel
{
    public SelectList ConfigSelectList { get; set; } = default!;

    [BindProperty]
    public string ConfigId { get; set; } = null!;
    
    public IsoServer IsoServer => isoServer;
    
    [BindProperty]
    public string GameSnapshotJson { get; set; } = null!;
    
    public void OnGet()
    {
        Console.WriteLine(isoServer);
    }
    
}
