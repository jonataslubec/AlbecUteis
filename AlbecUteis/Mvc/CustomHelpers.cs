using Attributes;
using System.Linq.Expressions;

//      ,--._______,-. 
//         ,','  ,    .  ,_`-. 
//        / /  ,' , _` ``. |  )       `-.. 
//       (,';'""`/ '"`-._ ` \/ ______    \\ 
//         : ,o.-`- ,o.  )\` -'      `---.)) 
//         : , __  ^-.   '|   `.      `    `. 
//         |/ __:_     `. |  ,  `       `    \ 
//         | ( ,-.`-.    ;'  ;   `       :    ; 
//         | |  ,   `.      /     ;      :    \ 
//         ;-'`:::._,`.__),'             :     ; 
//        / ,  `-   `--                  ;     | 
//       /  \                   `       ,      | 
//      (    `     :              :    ,\      | 
//       \   `.    :     :        :  ,'  \    : 
//        \    `|-- `     \ ,'    ,-'     :-.-'; 
//        :     |`--.______;     |        :    : 
//         :    /           |    |         |   \ 
//         |    ;           ;    ;        /     ; 
//       _/--' | Black Dog :`-- /         \_:_:_| 
//     ,',','  |           |___ \ 
//     `^._,--'           / , , .) 
//                        `-._,-' 

namespace System.Web.Mvc.Html
{
    public enum typeButton
    {
        Adicionar = 1,
        Detalhes = 2,
        VoltarIndex = 3,
        VoltarDetails = 4,
        Editar = 5,
        NovoRegistro = 6,
        Excluir = 7,
        Salvar = 8,
        SalvarNotSubmit = 9
    }

    public enum typeHeader
    {
        Index = 0,
        Details = 1,
        Create = 2,
        Edit = 3
    }

    public enum typeboxChart
    {
        cols6 = 0,
        cols12 = 1,
    }

    



    public static class ValueExtensions
    {

        public static MvcHtmlString Error(this HtmlHelper htmlHelper, string dataCuston = "")
        {
            // <div class="validation-summary-valid" data-valmsg-summary="true" data-valmsg-endereco="true">
            //    <ul></ul>
            //</div>

            var builder = new TagBuilder("div");
            builder.MergeAttribute("class", "validation-summary-valid");
            builder.MergeAttribute("data-valmsg-summary", "true");

            if (dataCuston.Length > 0)
                builder.MergeAttribute(dataCuston, "true");

            var builderUl = new TagBuilder("ul");

            builder.InnerHtml += builderUl;

            return MvcHtmlString.Create(builder.ToString());


        }

        public static MvcHtmlString StatusFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            return MvcHtmlString.Create(((value.ToString().ToLower() == "true" || expression.ToString() == "1") ? "Ativo" : "Inativo"));
        }

        public static MvcHtmlString StatusBool(this HtmlHelper htmlHelper, bool expression)
        {
            return MvcHtmlString.Create((expression ? "Ativo" : "Inativo"));
        }


        public static MvcHtmlString SexoFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            return MvcHtmlString.Create(((value.ToString().ToLower() == "true" || expression.ToString() == "1") ? "Masculino" : "Feminino"));
        }

        public static MvcHtmlString Sexo(this HtmlHelper htmlHelper, bool expression)
        {
            return MvcHtmlString.Create((expression ? "Masculino" : "Feminino"));
        }


        public static MvcHtmlString SimNaoFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            return MvcHtmlString.Create(((value.ToString().ToLower() == "true" || expression.ToString() == "1") ? "Sim" : "Não"));
        }

        public static MvcHtmlString SimNao(this HtmlHelper htmlHelper, bool expression)
        {
            return MvcHtmlString.Create((expression ? "Sim" : "Não"));
        }

        public static MvcHtmlString PlanoContasFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            return MvcHtmlString.Create(((value.ToString().ToLower() == "true" || expression.ToString() == "1") ? "Crédito" : "Débito"));
        }

        public static MvcHtmlString PlanoContas(this HtmlHelper htmlHelper, bool expression)
        {
            return MvcHtmlString.Create((expression ? "Crédito" : "Débito"));
        }

        public static MvcHtmlString CorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            var builder = new TagBuilder("canvas");
            builder.MergeAttribute("width", "20");
            builder.MergeAttribute("height", "20");
            builder.MergeAttribute("style", "border:1px solid " + value.ToString() + ";background:" + value.ToString() + ";");

            MemberExpression body = (MemberExpression)expression.Body;
            string propertyName = body.Member.Name;

            builder.MergeAttribute("id", propertyName);

            builder.SetInnerText(value.ToString());
            return MvcHtmlString.Create(builder.ToString());
        }


        public static MvcHtmlString TooltipFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;

            MemberExpression body = (MemberExpression)expression.Body;
            string propertyName = body.Member.Name;
            object[] attributes = body.Member.GetCustomAttributes(true);
            string message = "";


            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].GetType() == typeof(TooltipAttribute))
                {
                    TooltipAttribute tooltip = (TooltipAttribute)attributes[i];
                    message = tooltip.Message;
                }
            }

            var builder = new TagBuilder("span");
            builder.AddCssClass("fa fa-question-circle text-blue tooltipSisGPC");
            builder.MergeAttribute("data-toggle", "tooltip");
            builder.MergeAttribute("title", message);
            builder.MergeAttribute("id", propertyName + "_ToolTip");

            return MvcHtmlString.Create(builder.ToString());
        }


        public static MvcHtmlString TryFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var builder = new TagBuilder("textarea");
            builder.AddCssClass("ckeditor");
            builder.MergeAttribute("cols", "80");
            builder.MergeAttribute("name", "editor1");
            builder.MergeAttribute("id", expression.Name);
            var value = ModelMetadata.FromLambdaExpression(
                expression, htmlHelper.ViewData
            ).Model;
            builder.SetInnerText(value.ToString());
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, typeButton typeBtn)
        {
            return Button(htmlHelper, typeBtn, null, null);
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, typeButton typeBtn, string ControllerName)
        {
           return Button(htmlHelper, typeBtn, ControllerName,null);
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, typeButton typeBtn, string ControllerName, object routeValues)
        {
            var builder = new TagBuilder("a");
            var text = "";
            var actionName = "";
            var onclick = "";

            switch (typeBtn)
            {
                case typeButton.Adicionar:
                    builder.AddCssClass("btn btn-primary btn-sm btn-flat");
                    text = "Adicionar";
                    actionName = "Create";
                    builder.MergeAttribute("alt", "Ajax.ActionLink('" + actionName + "', '" + ControllerName + "');");
                    break;
                case typeButton.NovoRegistro:
                    builder.AddCssClass("btn btn-primary btn-sm btn-flat bg-purple");
                    text = "Novo Registro";
                    actionName = "Create";
                    builder.MergeAttribute("alt", "Ajax.ActionLink('" + actionName + "', '" + ControllerName + "');");
                    break;
                case typeButton.Detalhes:
                    builder.AddCssClass("btn btn-warning btn-sm btn-flat ");
                    text = "Detalhes";
                    actionName = "Details";
                    builder.MergeAttribute("alt", "detailsClick('" + actionName + "', '" + ControllerName + "');");
                    break;
                case typeButton.Editar:
                    builder.AddCssClass("btn btn-info btn-sm btn-flat");
                    text = "Editar";
                    actionName = "Edit";
                    builder.MergeAttribute("alt", "Ajax.ActionLink('" + actionName + "', '" + ControllerName + "', '" + routeValues + "');");
                    break;
                case typeButton.Excluir:
                    builder.AddCssClass("btn btn-danger btn-flat btn-sm");
                    builder.MergeAttribute("data-dismiss", "modal");
                    builder.MergeAttribute("id", "btnExcluir");
                    text = "Excluir";
                    break;
                case typeButton.Salvar:
                    builder = new TagBuilder("button");
                    builder.AddCssClass("btn btn-success btn-flat btn-sm");
                    builder.MergeAttribute("id", "btnSalvar");
                    text = "Salvar";
                    break;
                case typeButton.SalvarNotSubmit:
                    builder = new TagBuilder("button");
                    builder.AddCssClass("btn btn-success btn-flat btn-sm");
                    builder.MergeAttribute("id", "btnSalvar");
                    builder.MergeAttribute("type", "button");//Não submit
                    text = "Salvar";
                    break;
                case typeButton.VoltarDetails:
                    builder.AddCssClass("btn btn-default btn-sm btn-flat");
                    text = "Voltar";
                    actionName = "Details";
                    builder.MergeAttribute("alt", "Ajax.ActionLink('" + actionName + "', '" + ControllerName + "', '" + routeValues + "');");
                    break;
                case typeButton.VoltarIndex:
                    builder.AddCssClass("btn btn-default btn-sm btn-flat");
                    text = "Voltar";
                    actionName = "Index";
                    builder.MergeAttribute("alt", "Ajax.ActionLink('" + actionName + "', '" + ControllerName + "');");
                    break;
            }

            builder.SetInnerText(text);
            return MvcHtmlString.Create(builder.ToString());
        }



         public static MvcHtmlString Header(this HtmlHelper htmlHelper, typeHeader typeheader,string icon, string modulo, string subModulo, string msg = "")
        {
            var builder = new TagBuilder("section");
            builder.AddCssClass("content-header");
            builder.MergeAttribute("id", "content-headerAjax");

             var h1 = new TagBuilder("h1");
             var ol = new TagBuilder("ol");

            switch (typeheader)
            {
                case typeHeader.Create:

                    h1.InnerHtml += "<span>" + subModulo + "</span>";       
                    h1.InnerHtml += "<small>" + (msg.Length > 0 ? msg :  "Preencha os campos abaixo para cadastrar.") + "</small>";

                    ol.AddCssClass("breadcrumb");
                    ol.InnerHtml += "<li><i class='fa " + icon + "'></i> " + modulo + "</li>";
                    ol.InnerHtml += "<li>" + subModulo + "</li>";
                    ol.InnerHtml += "<li class='active'>Novo</li>";

                    break;
                case typeHeader.Edit:

                    h1.InnerHtml += "<span>" + subModulo + "</span>";
                    h1.InnerHtml += "<small>" + (msg.Length > 0 ? msg : "Preencha os campos abaixo para atualizar.") + "</small>";

                    ol.AddCssClass("breadcrumb");
                    ol.InnerHtml += "<li><i class='fa " + icon + "'></i> " + modulo + "</li>";
                    ol.InnerHtml += "<li>" + subModulo + "</li>";
                    ol.InnerHtml += "<li class='active'>Editar</li>";

                    break;
                case typeHeader.Details:

                    h1.InnerHtml += "<span>" + subModulo + "</span>";
                    h1.InnerHtml += "<small>" + (msg.Length > 0 ? msg : "Abaixo é listado os detalhes do registro.") + "</small>";

                    ol.AddCssClass("breadcrumb");
                    ol.InnerHtml += "<li><i class='fa " + icon + "'></i> " + modulo + "</li>";
                    ol.InnerHtml += "<li>" + subModulo + "</li>";
                    ol.InnerHtml += "<li class='active'>Detalhes</li>";

                    break;
                case typeHeader.Index:

                    h1.InnerHtml += "<span>" + subModulo + "</span>";
                    h1.InnerHtml += "<small>" + (msg.Length > 0 ? msg : "Abaixo são listados todos os registros cadastros.") + "</small>";

                    ol.AddCssClass("breadcrumb");
                    ol.InnerHtml += "<li><i class='fa " + icon + "'></i> " + modulo + "</li>";
                    ol.InnerHtml += "<li>" + subModulo + "</li>";

                    break;  
            }

            builder.InnerHtml += h1;
            builder.InnerHtml += ol;
            return MvcHtmlString.Create(builder.ToString());
        }


         public static MvcHtmlString boxChart(this HtmlHelper htmlHelper, typeboxChart typeboxChart, string icon, string title, string divChart)
         {
             var builder = new TagBuilder("div");

             var div = new TagBuilder("div");
             div.AddCssClass("box box-primary");

             string inner =  string.Format(@"<div class='box-header with-border'>
	                                                <i class='fa {0}'></i>
	                                                <h3 class='box-title'>{1}</h3>
	                                                <div class='box-tools pull-right'>
		                                                <div class='dropdown'>
			                                                <button class='btn btn-default btn-flat btn-sm dropdown-toggle' type='button' id='dropdownMenu1' data-toggle='dropdown' aria-haspopup='true' aria-expanded='true'>
				                                                Exportar&nbsp;<span class='caret'></span>
			                                                </button>
			                                                <ul class='dropdown-menu' aria-labelledby='dropdownMenu1'>
                                                                <li><a href='javascript:;' id='btnchartimprimir_" + divChart + @"' class='btnchartimprimir_'><i class='fa fa-print'></i><span class='file-format-name'>Imprimir</span></a></li>
                                                                <li role='separator' class='divider'></li>
				                                                <li><a href='javascript:void(0);' id='btnchartpng_" + divChart + @"' class='btnchartpng_'><i class='fa fa-file-image-o'></i> <span class='file-format-name'>Imagem PNG</span></a></li>
				                                                <li><a href='javascript:void(0);' id='btnchartjpg_" + divChart + @"' class='btnchartjpg_'><i class='fa fa-file-image-o'></i> <span class='file-format-name'>Imagem JPEG</span></a></li>
				                                                <li role='separator' class='divider'></li>
				                                                <li><a href='javascript:void(0);' id='btnchartsvg_" + divChart + @"' class='btnchartsvg_'><i class='fa fa-file-code-o'></i> <span class='file-format-name'>Imagem Vetor SVG</span></a></li>
				                                                <li role='separator' class='divider'></li>
				                                                <li><a href='javascript:void(0);' id='btnchartpdf_" + divChart + @"' class='btnchartpdf_'><i class='fa fa-file-pdf-o'></i> <span class='file-format-name'>Documento PDF</span></a></li>
			                                                </ul>
		                                                </div>
	                                                </div>
                                                </div>
                                                <div class='box-body'>
	                                                <div id='{2}'></div>
                                                </div>",icon, title, divChart);

             switch (typeboxChart)
             {
                 case typeboxChart.cols6:
                     builder.AddCssClass("col-md-6");
                     break;
                 case typeboxChart.cols12:
                      builder.AddCssClass("col-md-12");
                     break;
             }

             div.InnerHtml += inner;
             builder.InnerHtml += div;

             return MvcHtmlString.Create(builder.ToString());
         }


        public static MvcHtmlString MyHelperFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var propertyName = expression.Body.ToString();
            propertyName = propertyName.Substring(propertyName.IndexOf(".") + 1);
            if (!string.IsNullOrEmpty(helper.ViewData.TemplateInfo.HtmlFieldPrefix))
                propertyName = string.Format("{0}.{1}", helper.ViewData.TemplateInfo.HtmlFieldPrefix, propertyName);

            TagBuilder span = new TagBuilder("span");
            span.Attributes.Add("name", propertyName);
            span.Attributes.Add("data-something", propertyName);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                span.MergeAttributes(attributes);
            }

            return new MvcHtmlString(span.ToString());
        }
    }

}