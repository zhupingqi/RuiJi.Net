if (content.EndsWith("小时前"))
{{
	var hour = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
	results.Add(DateTime.Now.AddHours(-hour));
}}

if (content.EndsWith("天前"))
{{
	var days = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
	results.Add(DateTime.Now.AddDays(-days));
}}

if (content.EndsWith("刚刚"))
{{
	results.Add(DateTime.Now);
}}