#pragma checksum "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "797c4133ce984274f08edd96ec190d8a2b1b88d6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(ClientIntegrator.Pages.Organization.Pages_Organization_Index), @"mvc.1.0.razor-page", @"/Pages/Organization/Index.cshtml")]
namespace ClientIntegrator.Pages.Organization
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\_ViewImports.cshtml"
using ClientIntegrator;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
using ClientIntegrator.DataAccess.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
using Microsoft.Extensions.Configuration;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"797c4133ce984274f08edd96ec190d8a2b1b88d6", @"/Pages/Organization/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3d3de6d9cb49267decf6f9712f83af3a6157e973", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Organization_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary py-1"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page", "./Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 9 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
  
    ViewData["Title"] = "Organization Setup";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    <h3 class=\"pb-2\">Organization Setup</h3>\r\n\r\n");
#nullable restore
#line 15 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
 if (SignInManager.IsSignedIn(User) && (User.IsInRole("Admin") || User.IsInRole("SuperUser")))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
     if (Model.Organizations != null && Model.Organizations.Count == 0)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"container\">\r\n    <div class=\"col-12 py-1\">\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "797c4133ce984274f08edd96ec190d8a2b1b88d65302", async() => {
                WriteLiteral("Create Organization");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Page = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n    </div>\r\n    <div class=\"p-5 bg-light my-5 rounded text-center\">\r\n        <p class=\"text-muted\">\r\n            There are no items available.\r\n        </p>\r\n    </div>\r\n</div>\r\n");
#nullable restore
#line 29 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
    }
    else
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"container\">\r\n            <div class=\"row\">\r\n                <div class=\"col-12 py-1\">\r\n                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "797c4133ce984274f08edd96ec190d8a2b1b88d67128", async() => {
                WriteLiteral("Create Organization");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Page = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                </div>
            </div>
            <table class=""table border"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>ClientId</th>
                        <th>ClientSecret</th>
                    </tr>
                </thead>
                <tbody>
");
#nullable restore
#line 48 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                     foreach (var item in Model.Organizations)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td>");
#nullable restore
#line 51 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                       Write(item.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "797c4133ce984274f08edd96ec190d8a2b1b88d69415", async() => {
#nullable restore
#line 52 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                                                                    Write(item.DisplayName);

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Page = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 52 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                                                   WriteLiteral(item.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 53 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                       Write(item.ClientId);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 54 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                       Write(item.ClientSecret);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n");
#nullable restore
#line 56 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tbody>\r\n            </table>\r\n        </div>\r\n");
#nullable restore
#line 60 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
#nullable restore
#line 60 "C:\Users\Ken\Desktop\CC\HU\Artchitecure\ClientIntegrator\ClientIntegrator\Pages\Organization\Index.cshtml"
     
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IConfiguration Configuration { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<PortalUser> UserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<PortalUser> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<OrganizationModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<OrganizationModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<OrganizationModel>)PageContext?.ViewData;
        public OrganizationModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
