if (content.endWith("小时前"))
{{
	var hour = /[\d]*/.exec(content)[0];
	results.push(new Date().AddHour(-hour));
}}

if (content.endWith("天前"))
{{
	var days = /[\d]*/.exec(content)[0];
	results.push(new Date().addDay(-days));
}}

if (content.endWith("刚刚"))
{{
	results.push(new Date().format("yyyy-MM-dd hh:mm:ss"));
}}