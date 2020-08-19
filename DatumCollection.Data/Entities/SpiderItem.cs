﻿using DatumCollection.Data.Attributes;
using DatumCollection.Infrastructure.Selectors;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.Utility.HtmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.Entities
{
    [Schema("SpiderItem")]
    public class SpiderItem<T> : SystemBase, ISpiderItem where T: ISpider
    {
        [Column(Name = "Url", Type = "nvarchar")]
        public string Url { get; set; }

        [Column(Name = "Method", Type = "varchar", Length = 50)]
        public string Method { get; set; }

        [Column(Name = "ContentType", Type = "nvarchar")]
        public string ContentType { get; set; }

        [Column(Name = "Encoding", Type = "nvarchar")]
        public string Encoding { get; set; }

        [Column(Name = "FK_Channel_ID", Type = "uniqueidentifier")]
        public Guid FK_Channel_ID { get; set; }

        [JoinTable("FK_Channel_ID")]
        public Channel Channel { get; set; }

        public ISpiderConfig SpiderConfig { get; set; }

        public async Task<ISpider> Spider(SpiderAtom atom)
        {
            try
            {
                var result = System.Activator.CreateInstance<T>();
                var selectors = await SpiderConfig.GetSpiderSelectors();
                var props = typeof(T).GetProperties();

                foreach (var selector in selectors)
                {
                    IParser parser = null; object o = null;
                    switch (selector.Type)
                    {
                        case SelectorType.XPath:
                            {
                                parser = new XPathParser(selector.Path);
                                o = parser.SelectSingle(atom.Response.Content);
                            }
                            break;
                        case SelectorType.Html:
                            break;
                        case SelectorType.Json:
                            break;
                        default:
                            break;
                    }
                    props.FirstOrDefault(p => p.Name.ToLower() == selector.Key.ToLower())?.SetValue(result, o);
                }
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
            


            
        }


    }
}
