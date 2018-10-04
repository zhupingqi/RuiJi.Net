using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Expression
{
    public class Converter
    {
        public static ExtractBlock ToExtractBlock(string expression)
        {
            var parser = new RuiJiParser();

            return parser.ParseExtract(expression).Result;
        }

        public static string ToExpression(ExtractBlock block)
        {
            var expression = new List<string>();
            if(block.Selectors.Count > 0)
            {
                expression.Add("[block]");
                expression.AddRange(GetSelectorsExpression(block));    
            }               

            if(block.TileSelector.Selectors.Count > 0)
            {
                expression.Add("[tile]");
                expression.AddRange(GetSelectorsExpression(block.TileSelector));

                if(block.TileSelector.Metas.Count > 0)
                {
                    expression.Add("\t[meta]");
                    foreach (var meta in block.TileSelector.Metas)
                    {
                        expression.AddRange(GetSelectorsExpression(meta.Value, 1));
                    }
                }

                if(block.TileSelector.Paging!=null)
                {
                    expression.Add("\t[paging]");
                    expression.AddRange(GetSelectorsExpression(block.TileSelector.Paging.TileSelector, 1));
                }
            }

            if (block.Metas.Count > 0)
            {
                expression.Add("[meta]");
                foreach (var meta in block.Metas)
                {
                    expression.AddRange(GetSelectorsExpression(meta.Value));
                }                
            }

            if (block.Paging != null)
            {
                expression.Add("[paging]");
                expression.AddRange(GetSelectorsExpression(block.Paging.TileSelector));
            }

            return string.Join("\r\n",expression);
        }

        private static List<string> GetSelectorsExpression(ExtractBase extractBase, int t = 0)
        {
            var tab = "";
            for (int i = 0; i < t; i++)
            {
                tab += "\t";
            }

            var expression = new List<string>();

            if (!string.IsNullOrEmpty(extractBase.Name))
            {
                var dateType = "";
                switch (extractBase.DataType)
                {
                    case TypeCode.Boolean:
                        {
                            dateType = "_b";
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            dateType = "_i";
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            dateType = "_l";
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            dateType = "_f";
                            break;
                        }
                    case TypeCode.Double:
                        {
                            dateType = "_d";
                            break;
                        }
                    case TypeCode.DateTime:
                        {
                            dateType = "_dt";
                            break;
                        }
                }

                expression.Add(tab + "#" + extractBase.Name + dateType);
            }

            if (extractBase.Selectors.Count > 0)
            {
                expression.AddRange(GetSelectorsExpression(extractBase.Selectors,tab));
            }

            if (expression.Count > 0)
                expression[expression.Count - 1] += "\r\n";

            return expression;
        }

        private static List<string> GetSelectorsExpression(List<ISelector> selectors,string tab = "")
        {
            var expression = new List<string>() {  };

            foreach (var selector in selectors)
            {
                expression.Add(tab + selector.ToString().Trim());               
            }

            //expression.Add("\r\n");
            return expression;
        }
    }
}
