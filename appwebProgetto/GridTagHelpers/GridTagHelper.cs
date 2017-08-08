using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace appwebProgetto.GridTagHelpers
{
    public abstract class BaseTagHelper : TagHelper
    {
        // Per avere accesso a @Url.Action
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected HttpRequest Request => ViewContext.HttpContext.Request;

        protected HttpResponse Response => ViewContext.HttpContext.Response;

        private IUrlHelperFactory UrlHelper { get; }

        protected IUrlHelper Url => UrlHelper.GetUrlHelper(ViewContext);

        protected BaseTagHelper(IUrlHelperFactory urlHelper)
        {
            UrlHelper = urlHelper;
        }
    }

    [HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [RestrictChildren("column")]
    public class GridTagHelper : BaseTagHelper
    {
        public GridTagHelper(IUrlHelperFactory urlHelper) : base(urlHelper)
        { }

        public const string TagName = "grid";

        static class ActionName
        {
            public const string Read = "read";
            public const string Create = "create";
            public const string Update = "update";
            public const string Delete = "delete";
        }

        static class Format
        {
            public const string Date = "yyyy-MM-dd";
        }

        [HtmlAttributeName("id")]
        public string Id { get; set; } = "grid";

        [HtmlAttributeName("filterable")]
        public bool Filterable { get; set; } = true;

        [HtmlAttributeName("sortable")]
        public bool Sortable { get; set; } = true;

        [HtmlAttributeName("pageable")]
        public bool Pageable { get; set; } = false;

        [HtmlAttributeName("editable")]
        public bool Editable { get; set; } = true;

        [HtmlAttributeName("height")]
        public int? Height { get; set; }

        [HtmlAttributeName("controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("action-read")]
        public string ReadAction { get; set; } = ActionName.Read;

        [HtmlAttributeName("action-create")]
        public string CreateAction { get; set; } = ActionName.Create;

        [HtmlAttributeName("action-update")]
        public string UpdateAction { get; set; } = ActionName.Update;

        [HtmlAttributeName("action-delete")]
        public string DeleteAction { get; set; } = ActionName.Delete;

        [HtmlAttributeName("primary-key")]
        public string PrimaryKey { get; set; } = "id";

        void RenderDataSource(TagHelperContent content, GridContext gridContext)
        {
            var formatScript = new StringBuilder();
            foreach (var column in gridContext.Columns)
            {
                if (column.Type != GridColumnType.Date)
                    continue;

                formatScript.AppendLine($"data.{column.Field} = kendo.toString(data.{column.Field}, '{Format.Date}');");
            }

            content.AppendHtml(@"
                dataSource: {
                    transport:
                    {
            ");
            
            content.AppendHtml($@"
                        read: {{
                            url: '{Url.Action(ReadAction, Controller)}',
                            type: 'get',
                            dataType: 'json'
                        }},
            ");

            content.AppendHtml($@"
                        update: {{
                            url: '{Url.Action(UpdateAction, Controller)}',
                            type: 'post',
                            dataType: 'json',
                            data: function(data) {{
                                {formatScript}
                                return data;
                            }}
                        }},
            ");
            
            content.AppendHtml($@"
                        destroy: {{
                            url: '{Url.Action(DeleteAction, Controller)}',
                            type: 'post',
                            dataType: 'json'
                        }},
            ");

            content.AppendHtml($@"
                        create: {{
                            url: '{Url.Action(CreateAction, Controller)}',
                            type: 'post',
                            dataType: 'json',
                            data: function(data) {{
                                {formatScript}
                                return data;
                            }}
                        }}
            ");

            content.AppendHtml(@"
                },
            ");
        }

        void RenderSchema(TagHelperContent content, GridContext context)
        {
            content.AppendHtml($@"
                schema: {{
                    data: 'data',
                    model: {{
                        id: '{PrimaryKey}',
                        fields: {{
            ");

            foreach (var column in context.Columns)
            {
                content.AppendHtml($"{column.Field}: {{ ");
                content.AppendHtml($"type: '{ToJs(column.Type)}'");
                content.AppendHtml($", editable: {ToJs(column.Editable)}");

                if (!string.IsNullOrEmpty(column.Format))
                    content.AppendHtml($", format: '{column.Format}'");

                content.AppendHtml($", validation: {{ required: {ToJs(column.Required)} }}");

                content.AppendHtml(" }, ");
            }

            content.AppendHtmlLine(@"
                        }
                    }
                },
            ");
        }

        void RenderRequestEnd(TagHelperContent content)
        {
            content.AppendHtml($@"
                requestEnd: function(e) {{
                    var grid = $('#{Id}').data('kendoGrid');
                    var data = grid.dataSource;
                    if (e.type === 'create' || e.type === 'update') {{
                        if (!e.response.Errors)
                            data.read();
                        else console.log('I had some issues');
                    }}
                }}
            ");
        }

        void RenderColumns(TagHelperContent content, GridContext context)
        {
            content.AppendHtml(@"
                columns: [
            ");

            foreach (var column in context.Columns)
            {
                content.AppendHtml("{ ");
                content.AppendHtml($"field: '{column.Field}'");

                if (!string.IsNullOrEmpty(column.Title))
                    content.AppendHtml($", title: '{column.Title}'");

                if (!string.IsNullOrEmpty(column.Format))
                    content.AppendHtml($", format: '{column.Format}'");

                if (column.Width != null)
                    content.AppendHtml($", width: '{column.Width}px'");

                if (column.Values is SelectList values)
                    content.AppendHtml($", values: {values.ToJson()}");

                content.AppendHtml(" }, ");
            }

            content.AppendHtml("{ command: ['edit', 'destroy'], title: '&nbsp;', width: '180px' }");

            content.AppendHtml(@"
                ]
            ");
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var gridContext = new GridContext
            {
                Columns = new List<IGridColumn>()
            };

            context.Items.Add(typeof(GridColumnTagHelper), gridContext);
            var childContent = await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Id);

            // Rimuovo il contenuto altrimenti avrei gli elementi <column> all'interno di <div>.
            output.Content.Clear();

            // <script> deve seguire <div></div>
            var content = output.PostElement;
            content.AppendLine();
            content.AppendHtml("<script>");

            content.AppendHtml($@"
                $(document).ready(function () {{
                    $(""#{Id}"").kendoGrid({{
            ");

            RenderDataSource(content, gridContext);
            RenderSchema(content, gridContext);
            RenderRequestEnd(content);

            content.AppendHtml(@"
                },            
            ");

            if (Height != null)
            {
                content.AppendHtml($"height: {Height},");
            }

            content.AppendHtmlLine($"filterable: {ToJs(Filterable)},");
            content.AppendHtmlLine($"sortable: {ToJs(Sortable)},");
            content.AppendHtmlLine($"pageable: {ToJs(Pageable)},");
            content.AppendHtmlLine("toolbar: ['create'],");
            content.AppendHtmlLine("editable: 'inline',");

            RenderColumns(content, gridContext);

            content.AppendHtml(@"
                    });
                });
            ");

            content.AppendHtml("</script>");
        }

        public static string ToJs<T>(T value) => value?.ToString()?.ToLower() ?? "null";
    }

    [HtmlTargetElement(TagName, ParentTag = "grid")]
    public class GridColumnTagHelper : BaseTagHelper, IGridColumn
    {
        public GridColumnTagHelper(IUrlHelperFactory urlHelper) : base(urlHelper)
        { }

        public const string TagName = "column";

        public GridColumnType Type { get; set; }

        public string Field { get; set; }

        public bool Editable { get; set; } = true;

        public string Format { get; set; }

        public bool Required { get; set; } = true;

        public string Title { get; set; }

        public int? Width { get; set; }

        public object Values { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await output.GetChildContentAsync();

            var gridContext = context.Items[typeof(GridColumnTagHelper)] as GridContext;
            gridContext?.Columns?.Add(this);
            output.SuppressOutput();
        }
    }

    public enum GridColumnType
    {
        Number,
        String,
        Date
    }

    public interface IGridColumn
    {
        GridColumnType Type { get; set; }

        string Field { get; set; }

        bool Editable { get; set; }

        string Format { get; set; }

        bool Required { get; set; }

        string Title { get; set; }

        int? Width { get; set; }

        object Values { get; set; }
    }

    public class GridContext
    {
        public IList<IGridColumn> Columns { get; set; } = new List<IGridColumn>();
    }
}
