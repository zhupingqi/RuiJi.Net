var content = "{0}";

if (content.EndsWith("小时前"))
{{
	var hour = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
	results.Add(DateTime.Now.AddHours(-hour));
}}

if (content.EndsWith("天前"))
{{
	var hour = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
	results.Add(DateTime.Now.AddDays(-hour));
}}

if (content.EndsWith("AddMinutes"))
{{
	var hour = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
	results.Add(DateTime.Now.AddDays(-hour));
}}