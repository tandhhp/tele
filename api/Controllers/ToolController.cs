using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.Json;
using Waffle.Core.Constants;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Extensions;
using Waffle.ExternalAPI.Interfaces;
using Waffle.ExternalAPI.Models;
using Waffle.Models.Components;
using Waffle.Models.Files;
using Waffle.Models.Params.Tools;

namespace Waffle.Controllers;

public class ToolController : BaseController
{
    private readonly ICatalogService _catalogService;
    private readonly IWordPressService _wordPressService;
    private readonly IComponentService _componentService;
    private readonly IWorkService _workService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ToolController(ApplicationDbContext context, ICatalogService catalogService, IWordPressService wordPressService, IComponentService componentService, IWorkService workService, UserManager<ApplicationUser> userManager)
    {
        _catalogService = catalogService;
        _context = context;
        _wordPressService = wordPressService;
        _componentService = componentService;
        _workService = workService;
        _userManager = userManager;
    }

    [HttpPost("fetch-wordpress")]
    public async Task<IActionResult> FetchWordPressAsync([FromBody] FetchWordPressArgs args)
    {
        if (string.IsNullOrEmpty(args.Domain)) return BadRequest("No domain found!");
        if (!args.Domain.EndsWith("/"))
        {
            args.Domain += "/";
        }
        if (args.CatalogId != null)
        {
            var parent = await _catalogService.FindAsync(args.CatalogId ?? Guid.Empty);
            if (parent is null) return BadRequest("Catalog not found!");
        }
        var current = 1;
        var editor = await _componentService.GetByNameAsync(nameof(Editor));
        if (editor is null) return BadRequest("Editor not found!");
        while (true)
        {
            var posts = await _wordPressService.ListPostAsync(args.Domain, new Models.SearchFilterOptions
            {
                Current = current
            });
            if (posts is null) break;
            foreach (var post in posts)
            {
                var article = new Catalog
                {
                    Active = true,
                    CreatedBy = User.GetId(),
                    CreatedDate = DateTime.Now,
                    Description = post.Excerpt.Rendered,
                    ModifiedDate = DateTime.Now,
                    Name = post.Title.Rendered ?? string.Empty,
                    NormalizedName = post.Slug ?? string.Empty,
                    ParentId = args.CatalogId,
                    ViewCount = 0,
                    Type = CatalogType.Article,
                    Thumbnail = "/imgs/search-engines-amico.svg"
                };
                await _catalogService.AddAsync(article);
                var content = string.Empty;
                if (!string.IsNullOrEmpty(post.Content.Rendered))
                {
                    content = post.Content.Rendered.Replace("href=\"" + args.Domain, "href=\"" + "/");
                }
                var arguments = new Editor
                {
                    Blocks = new List<BlockEditorBlock>
                    {
                        new BlockEditorBlock
                        {
                            Type = BlockEditorType.RAW,
                            Data = new BlockEditorItemData
                            {
                                Html = post.Content.Rendered
                            }
                        }
                    }
                };
                var work = new WorkContent
                {
                    ComponentId = editor.Id,
                    Active = true,
                    Arguments = JsonSerializer.Serialize(arguments),
                    Name = "WordPress content"
                };
                await _workService.AddAsync(work);
                await _workService.AddItemAsync(work.Id, article.Id);
            }
            current++;
        }
        return Ok(IdentityResult.Success);
    }

    [HttpPost("lead-import"), AllowAnonymous]
    public async Task<IActionResult> ImportLeadAsync([FromForm] UploadFileArgs args)
    {
        if (args.File == null) return BadRequest("File not found!");
        using var pgk = new ExcelPackage(args.File.OpenReadStream());
        var ws = pgk.Workbook.Worksheets[0];
        if (ws == null) return BadRequest();

        var rowCount = ws.Dimension.End.Row;

        var data = await _context.Leads.AsNoTracking().ToListAsync();

        var leads = new List<Lead>();

        var sales = await _userManager.GetUsersInRoleAsync(RoleName.Sales);

        for (int row = 2; row <= rowCount; row++)
        {
            var phoneNumber = ws.Cells[row, 2].GetCellValue<string>();
            if (data.Any(x => x.PhoneNumber == phoneNumber))
            {
                continue;
            }
            var eventDate = ws.Cells[row, 4].GetCellValue<DateTime>();

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                EventDate = eventDate,
                EventTime = ws.Cells[row, 5].GetCellValue<string>(),
                Name = ws.Cells[row, 1].GetCellValue<string>(),
                CreatedDate = DateTime.Now,
                Branch = Branch1.North,
                PhoneNumber = phoneNumber,
                Note = ws.Cells[row, 14].GetCellValue<string>(),
                Status = LeadStatus.Checkin,
                IdentityNumber = ws.Cells[row, 11].GetCellValue<string>(),
                Address = ws.Cells[row, 12].GetCellValue<string>()
            };

            var names = ws.Cells[row, 9].GetCellValue<string>();
            if (!string.IsNullOrEmpty(names))
            {
                var sale = sales.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == names.ToLower());
                if (sale != null)
                {
                    lead.SalesId = sale.Id;
                }
            }

            leads.Add(lead);
        }
        await _context.Leads.AddRangeAsync(leads);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
