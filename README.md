# RuiJi.Net
RuiJi.Net is a distributed crawl framework written in c#.

RuiJi.Net is a self host webapi written using Microsoft.Owin. Major features include distribute crawler, distribute extracter and managed cookie.

RuiJi.Net support ip polling that using the server public network address and proxy server.

## Document

Building, who can help me...

## Notice

The project is under development.

## Features

#### Crawler

| Feature | Support |
| :-: | :-:  |
| webheader |  custom  |
| method | get/post |
| auto redirection | support |
| cookie | managed/custom |
| service point ip | auto/custom Bind |
| encoding | auto detect/by specify |
| response | raw/string |
| proxy | future additions |

#### Extracter

| Feature | Support |
| :-: | :-:  |
| selector |  css/xpath/regex/json/text range/exclude text/clear  |
| extrac structure | block/tile/meta |
| jsonconvert | extractblock |

## About extract structure

 ![Image text](/RuiJi.ChromePlugin/extract_structure.png)


## Examples

#### Crawl and Extract with loacl libary 


            var crawler = new IPCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            block.Selectors = new List<ISelector>
            {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            };

            block.TileSelector = new ExtractTile
            {
                Selectors = new List<ISelector>
                {
                    new CssSelector(".pt-cv-content-item",CssTypeEnum.InnerHtml)
                }
            };

            block.TileSelector.Metas.AddMeta("title",new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var ext = new RuiJiExtracter();
            var r = ext.Extract(content, block);


#### Crawl and Extract width cluster 

1. downloaded ZooKeeper from Apache mirrors http://mirrors.hust.edu.cn/apache/zookeeper/zookeeper-3.4.12/

2. Add the same file as zoo_sample.cfg in folder conf and rename it to zoo.cfg. and change dataDir with your

3. Please confirm whether the Java runtime environment is installed

4. run bin/zkServer.cmd in you zookeepr folder

5. run RuiJi.cmd.exe 

if You see the following information

    Server Start At http://x.x.x.x:x
    proxy x.x.x.x:x ready to startup!
    try connect to zookeeper server : x.x.x.x:2181
    zookeeper server connected!

the service startup is complete!

##### Notice 
##### The RuiJi.Cmd.exe have to run as an administrator!


            Common.StartupNodes();

            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = Crawler.Request(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

            var content = response.Data.ToString();

            var block = new ExtractBlock();
            block.Selectors = new List<ISelector>
            {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            };

            block.TileSelector = new ExtractTile
            {
                Selectors = new List<ISelector>
                {
                    new CssSelector(".pt-cv-content-item",CssTypeEnum.InnerHtml)
                }
            };

            block.TileSelector.Metas.AddMeta("title", new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var r = Extracter.Extract(new ExtractRequest {
                Block = block,
                Content = content
            });


## Contact
Please contact me with any suggestion

416803633@qq.com

my website : www.ruijihg.com