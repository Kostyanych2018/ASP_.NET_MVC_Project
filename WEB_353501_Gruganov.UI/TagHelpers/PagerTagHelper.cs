using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WEB_353501_Gruganov.UI.TagHelpers;

[HtmlTargetElement("pager")]
public class PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor): TagHelper
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? GenreName { get; set; }
    public bool Admin { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "nav";
        output.Attributes.SetAttribute("aria-label", "Page navigation");
        
        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");

        ul.InnerHtml.AppendHtml(GeneratePageLink(CurrentPage - 1, "Предыдущая", CurrentPage == 1));

        for (int i = 1; i <= TotalPages; i++) {
            ul.InnerHtml.AppendHtml(GeneratePageLink(i, i.ToString(), i == CurrentPage));
        }
        ul.InnerHtml.AppendHtml(GeneratePageLink(CurrentPage + 1, "Следующая", CurrentPage == TotalPages));
        output.Content.AppendHtml(ul);
    }

    private TagBuilder GeneratePageLink(int pageNo, string text, bool disabled = false, bool active = false)
    {
        var li = new TagBuilder("li");
        li.AddCssClass("page-item");
        if (disabled) li.AddCssClass("disabled");
        if (active) li.AddCssClass("active");   
        
        var a = new TagBuilder("a");
        a.AddCssClass("page-link");
        a.InnerHtml.Append(text);
        
        if (!disabled)
        {
            string? path;
            if (Admin)
            {
                path = linkGenerator.GetPathByPage(httpContextAccessor.HttpContext,
                    page: "/Games/Index",
                    values: new { pageNo });
            }
            else
            {
                path = linkGenerator.GetPathByAction("Index", "Game",
                    new { pageNo, genre = GenreName });
            }
            a.Attributes["href"] = path;
        }

        li.InnerHtml.AppendHtml(a);
        return li;
    }
}