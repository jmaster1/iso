using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web2.Pages;

public class IndexModel(
    // IConfigRepository configRepository, 
    // IGameRepository gameRepository, 
    // IPlayerTokenRepository playerTokenRepository
    ) : PageModel
{
    public SelectList ConfigSelectList { get; set; } = default!;

    [BindProperty]
    public string ConfigId { get; set; } = null!;
    
    // public PlayerToken? XPlayerToken { get; set; }
    //
    // public PlayerToken? OPlayerToken { get; set; }
    
    [BindProperty]
    public string GameSnapshotJson { get; set; } = null!;

    private void Load()
    {
        // var selectListData = configRepository.GetConfigurationNames()
        //     .Select(name => new {id = name, value = name})
        //     .ToList();
        // ConfigSelectList = new SelectList(selectListData, "id", "value");
    }
    
    public void OnGet()
    {
        Load();
    }
    
}
