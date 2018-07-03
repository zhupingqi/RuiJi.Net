
#### RuiJi.Net Documentation

http://www.ruijihg.com/archives/ruijinet/getting-started

#### Star on github

https://github.com/zhupingqi/RuiJi.Net

#### RuiJi.Net.Core Sample



crawl use local ip automatic

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	var response = crawler.Request(request);


crawl with special ip

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	request.Ip = "192.168.31.196";
	var response = crawler.Request(request);

crawl with proxy

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.baidu.com");
	request.Proxy = new RequestProxy("223.93.172.248", 3128);

	var response = crawler.Request(request);

extract url

	var crawler = new RuiJiCrawler();
	var request = new Request("https://www.oschina.net/blog");

	var response = crawler.Request(request);
	var content = response.Data.ToString();

	var parser = new RuiJiParser();
	var eb = parser.ParseExtract("css a.blog-title-link[href]\nexp https://my.oschina.net/*/blog/*");
	var result = RuiJiExtractor.Extract(content, eb.Block);

extract tile

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


extract meta

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

run js

	var request = new Request("https://gitee.com/zhupingqi/RuiJi.Net");
	request.RunJS = true;

	var crawler = new RuiJiCrawler();

	var response = crawler.Request(request);

cookie

	var request = new Request("https://gitee.com/zhupingqi/RuiJi.Net");
	request.Cookie = "xxxxxx";

	var crawler = new RuiJiCrawler();

	var response = crawler.Request(request);

#### More Feature

please visit my github

https://github.com/zhupingqi

or my website

http://www.ruijihg.com/archives/ruijinet/getting-started

#### RuiJi.Net Cluster is waitting for you

RuiJi.Net is a dotnet distributed crawler framework written in c#.Major features include distribute crawler, distribute Extractor and managed cookie, support ip polling that using the server public network address and proxy server.

RuiJi.Net has extract model called RuiJi Expression,It divides the web page into block,tile and meta. you can extract web page by RuiJi Expression and save the expression with text file or database.

RuiJi.Net have more features including extract rule match by url wildcard and page feature, paging extract, url function, cookie manager and cookie channel,much selecors to clear data, ...

If you like , please star my project, It will give me more motivation to improve this project.