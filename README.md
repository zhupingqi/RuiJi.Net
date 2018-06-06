# RuiJi.Net
RuiJi.Net is a distributed crawl framework written in c#.

RuiJi.Net is a self host webapi written using Microsoft.Owin. Major features include distribute crawler, distribute extracter and managed cookie.

RuiJi.Net support ip polling that using the server public network address and proxy server.

## Document

Building

[http://www.ruijihg.com/archives/ruijinet/getting-started](http://www.ruijihg.com/archives/ruijinet/getting-started)

以上链接带中文说明

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

 ![Image text](http://www.ruijihg.com/wp-content/uploads/2018/06/4-2.png)


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


#### Crawl and Extract with cluster 

1. downloaded ZooKeeper from Apache mirrors http://mirrors.hust.edu.cn/apache/zookeeper/zookeeper-3.4.12/

2. Add the same file as zoo_sample.cfg in folder conf and rename it to zoo.cfg. and change dataDir with your

3. Please confirm whether the Java runtime environment is installed

4. run bin/zkServer.cmd in you zookeepr folder

5. Start up zookeeper

6. Compile RuiJi.Net.Cmd and run RuiJi.Net.Cmd.exe

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


# About RuiJi Expression

RuiJi Expression is a way to quickly add the rules of page extraction and implement soft coding. The ruiji expressions are as simple and understandable as possible.Before we start, we should first understand the rule model of RuiJi.Net.

The RuiJi expression uses the structure described in the figure above to extract the pages that need to be extracted, and the extraction unit is Block, as shown in the following figure.

Selectors is a list of selector
Tiles is a region that needs to be repeatedly extracted
Metas is the metadata that needs to be extracted
Blocks is a subBlock that needs to be extracted within Block

 ![Image text](http://www.ruijihg.com/wp-content/uploads/2018/06/1-3.png)

If you need to extract http://www.ruijihg.com/开发, you need to observe the structure of the page first.You can use F12 to look at the structure of the page

 ![Image text](http://www.ruijihg.com/wp-content/uploads/2018/06/2-2.png)

First, make sure that the result of the Block selector is unique.

 ![Image text](http://www.ruijihg.com/wp-content/uploads/2018/06/3-2.png)

The definition of Block can be as follows

    #content
    css .pt-cv-view:ohtml

Continue adding tile

    [tile]
        #tiles
        css .pt-cv-content-item:ohtml

        [meta]
        #title
        css .pt-cv-title:text

        #content
        css .pt-cv-content:html
        ex 阅读更多... -e

You may notice \t, because both block and tile contain meta, so the tile selector part and tile meta are \t as the current tile flag.

The complete Block description structure is as follows

    [Block]
    #blockname
    selector

    [blocks]
        @subblockname1
        @subblockname2

    [tile]
        #tilename
        tile selector

        [meta]
        #meta1
        selector

        #meta2
        selector

    [meta]
        #blockmeta1
        selector

        #blockmeta2
        selector



#### I'm still improving RuiJi Expression

## Contact
Please contact me with any suggestion

416803633@qq.com

my website : www.ruijihg.com

QQ交流群: 545931923

https://gitee.com/zhupingqi/RuiJi.Net

https://github.com/zhupingqi/RuiJi.Net