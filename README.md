[![Nuget](https://buildstats.info/nuget/RuiJi.Net.Core?includePreReleases=true)](https://www.nuget.org/packages/RuiJi.Net.Core)
[![Build status](https://ci.appveyor.com/api/projects/status/6hs9a47tftkv1yeo?svg=true)](https://ci.appveyor.com/project/zhupingqi/ruiji-net)
![CodeFactor](https://www.codefactor.io/repository/github/zhupingqi/ruiji.net/badge)

[![Build status](https://opencollective.com/ruijinet/tiers/backer/badge.svg?label=backer&color=brightgreen)](https://opencollective.com/ruijinet)
[![Build status](https://opencollective.com/ruijinet/tiers/sponsor/badge.svg?label=sponsor&color=brightgreen)](https://opencollective.com/ruijinet)

#### About RuiJi Scraper

RuiJi Scraper is a RuiJi expression based browser plug-in that uses visual rule editing and generates RuiJi expressions for RuiJi.Net.
[firefox](https://addons.mozilla.org/zh-CN/firefox/addon/ruiji-scraper/)

#### Contributors

This project exists thanks to all the people who contribute. 
<a href="graphs/contributors"><img src="https://opencollective.com/ruijinet/contributors.svg?width=890&button=false" /></a>


#### Backers

Thank you to all our backers! 🙏 [[Become a backer](https://opencollective.com/ruijinet#backer)]

<a href="https://opencollective.com/ruijinet#backers" target="_blank"><img src="https://opencollective.com/ruijinet/backers.svg?width=890"></a>

#### Sponsors

Support this project by becoming a sponsor. Your logo will show up here with a link to your website. [[Become a sponsor](https://opencollective.com/ruijinet#sponsor)]

<a href="https://opencollective.com/ruijinet/sponsor/0/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/0/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/1/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/1/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/2/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/2/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/3/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/3/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/4/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/4/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/5/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/5/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/6/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/6/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/7/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/7/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/8/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/8/avatar.svg"></a>
<a href="https://opencollective.com/ruijinet/sponsor/9/website" target="_blank"><img src="https://opencollective.com/ruijinet/sponsor/9/avatar.svg"></a>


#### Support us

https://www.scraperapi.com/?fp_ref=ruijinet

https://promotion.aliyun.com/ntms/yunparter/invite.html?userCode=r1bio67c&utm_source=r1bio67c

## About RuiJi.Net
RuiJi.Net is a distributed crawl framework written in netcore.

RuiJi.Net is a self host webapi written using Microsoft.AspNetCore.Owin. Major features include distribute crawler, distribute Extractor and managed cookie.

RuiJi.Net support ip polling that using the server public network address and proxy server.

## Documentation

Building
[http://doc.ruijihg.com/](http://doc.ruijihg.com/)

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
| proxy | http |

#### Selectors

|Type|
| :-: |
|CSS|
|REGEX|
|REGEXSPLIT|
|TEXTRANGE|
|EXCLUDE|
|REGEXREPLACE|
|JPATH|
|XPATH|
|CLEAR|
|EXPRESSION|
|SELECTORPROCESSOR|

#### Extract structure

 ![Image text](http://www.ruijihg.com/wp-content/uploads/2018/06/4-2.png)

## Examples

#### crawl use local ip automatic

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	var response = crawler.Request(request);


#### crawl with special ip

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	request.Ip = "192.168.31.196";
	var response = crawler.Request(request);

#### crawl with proxy

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	request.Proxy = new RequestProxy("223.93.172.248", 3128);

	var response = crawler.Request(request);

#### extract url

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.oschina.net/blog");

	var response = crawler.Request(request);
	var content = response.Data.ToString();

	var parser = new RuiJiParser();
	var eb = parser.ParseExtract("css a.blog-title-link[href]\nexp https://my.oschina.net/*/blog/*");
	var result = RuiJiExtractor.Extract(content, eb.Block);

#### extract tile

	var crawler = new RuiJiCrawler();
	var request = new Request("http://www.ruijihg.com/archives/category/tech/bigdata");

	var response = crawler.Request(request);
	var content = response.Data.ToString();

	var parser = new RuiJiParser();
	var eb = parser.ParseExtract(@"[tile]\ncss article:html

	[meta]
	#title
	css .entry-header:text

	#summary
	css .entry-header + p:text
	ex /Read more »/ -e");

	var result = RuiJiExtractor.Extract(content, eb.Block);


#### extract meta

	var crawler = new RuiJiCrawler();
	var request = new Request("https://my.oschina.net/zhupingqi/blog/1826317");

	var response = crawler.Request(request);
	var content = response.Data.ToString();

	var parser = new RuiJiParser();
	var eb = parser.ParseExtract(@"[meta]
	#title
	css h1.header:text

	#author
	css div.blog-meta .avatar + span:text

	#date
	css div.blog-meta > div.item:first:text
	regS /发布于/ 1

	#words_i
	css div.blog-meta > div.item:eq(1):text
	regS / / 1

	#content
	css #articleContent:html");

	var result = RuiJiExtractor.Extract(content, eb.Block);

detect mine

	var crawler = new RuiJiCrawler();
	var request = new Request("http://img10.jiuxian.com/2018/0111/cd51bb851410404388155b3ec2c505cf4.jpg");
	var response = crawler.Request(request);

	var ex = response.Extensions;


## RuiJi.Net Cluster 

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

##### The RuiJi.Net.Cmd.exe have to run as an administrator!


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

            var r = Extractor.Extract(new ExtractRequest {
                Block = block,
                Content = content
            });


## RuiJi Expression

RuiJi Expression is a way to quickly add the rules of page extraction. The ruiji expressions are as simple and understandable as possible.Before we start, we should first understand the rule model of RuiJi.Net.

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
## Admin Ui

![](http://www.ruijihg.com/wp-content/uploads/2018/07/3-2.png)

![](http://www.ruijihg.com/wp-content/uploads/2018/07/6.png)

![](http://www.ruijihg.com/wp-content/uploads/2018/07/13.png)

## Contact
Please contact me with any suggestion

416803633@qq.com

my website : www.ruijihg.com

QQ交流群: 545931923

https://github.com/zhupingqi/RuiJi.Net

https://gitee.com/zhupingqi/RuiJi.Net