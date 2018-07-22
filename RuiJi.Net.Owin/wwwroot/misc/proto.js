//array

var linq = {
    EqualityComparer: function (a, b) {
        return a === b || a.valueOf() === b.valueOf();
    },
    SortComparer: function (a, b) {
        if (a === b) return 0;
        if (a === null) return -1;
        if (b === null) return 1;
        if (typeof a == 'string') return a.toString().localeCompare(b.toString());
        return a.valueOf() - b.valueOf();
    },
    Predicate: function () {
        return true;
    },
    Selector: function (t) {
        return t;
    }
};

Array.prototype.addRange = function (items) {
    var length = items.length;

    if (length != 0) {
        for (var index = 0; index < length; index++) {
            this.push(items[index]);
        }
    }
}

Array.prototype.unique = function () {
    var res = [];
    var json = {};
    for (var i = 0; i < this.length; i++) {
        if (!json[this[i]]) {
            res.push(this[i]);
            json[this[i]] = 1;
        }
    }
    return res;
};

Array.prototype.remove = function (dx) {
    var i = Number(dx);
    if (!isNaN(i)) {
        this.splice(dx, 1);
    } else {
        i = this.indexOf(dx);
        this.remove(i);
    }
}

Array.prototype.removeAll = function (predicate) {
    var item;
    var i = 0;
    while ((item = this.first(predicate)) != null) {
        i++;
        this.remove(item);
    }

    return i;
};

Array.prototype.select = Array.prototype.map || function (selector, context) {
    context = context || window;
    var arr = [];
    var l = this.length;
    for (var i = 0; i < l; i++)
        arr.push(Selector.call(context, this[i], i, this));
    return arr;
};

Array.prototype.selectMany = function (selector, resSelector) {
    resSelector = resSelector || function (i, res) {
        return res;
    };
    return this.aggregate(function (a, b) {
        return a.concat(selector(b).select(function (res) {
            return resSelector(b, res);
        }));
    }, []);
};

Array.prototype.take = function (c) {
    return this.slice(0, c);
};

Array.prototype.first = function (predicate, def) {
    var l = this.length;
    if (!predicate) return l ? this[0] : def == null ? null : def;
    for (var i = 0; i < l; i++) {
        if (predicate(this[i], i, this))
            return this[i];
    }

    return def == null ? null : def;
};

Array.prototype.last = function (predicate, def) {
    var l = this.length;
    if (!predicate) return l ? this[l - 1] : def == null ? null : def;
    while (l-- > 0) {
        if (predicate(this[l], l, this))
            return this[l];
    }

    return def == null ? null : def;
};

Array.prototype.union = function (arr) {
    return this.concat(arr).distinct();
};

Array.prototype.intersect = function (arr, comparer) {
    comparer = comparer || linq.EqualityComparer;
    return this.distinct(comparer).where(function (t) {
        return arr.contains(t, comparer);
    });
};

Array.prototype.except = function (arr, comparer) {
    if (!(arr instanceof Array)) arr = [arr];
    comparer = comparer || linq.EqualityComparer;
    var l = this.length;
    var res = [];
    for (var i = 0; i < l; i++) {
        var k = arr.length;
        var t = false;
        while (k-- > 0) {
            if (comparer(this[i], arr[k]) === true) {
                t = true;
                break;
            }
        }
        if (!t) res.push(this[i]);
    }
    return res;
};

Array.prototype.distinct = function (comparer) {
    var arr = [];
    var l = this.length;
    for (var i = 0; i < l; i++) {
        if (!arr.contains(this[i], comparer))
            arr.push(this[i]);
    }
    return arr;
};

Array.prototype.indexOf = Array.prototype.indexOf || function (o, index) {
    var l = this.length;
    for (var i = Math.max(Math.min(index, l), 0) || 0; i < l; i++)
        if (this[i] === o) return i;
    return -1;
};

Array.prototype.lastIndexOf = Array.prototype.lastIndexOf || function (o, index) {
    var l = Math.max(Math.min(index || this.length, this.length), 0);
    while (l-- > 0)
        if (this[l] === o) return l;
    return -1;
};

Array.prototype.orderBy = function (selector, comparer) {
    comparer = comparer || linq.SortComparer;
    var arr = this.slice(0);
    var fn = function (a, b) {
        return comparer(selector(a), selector(b));
    };

    arr.thenBy = function (selector, comparer) {
        comparer = comparer || linq.SortComparer;
        return arr.orderBy(linq.Selector, function (a, b) {
            var res = fn(a, b);
            return res === 0 ? comparer(selector(a), selector(b)) : res;
        });
    };

    arr.thenByDescending = function (selector, comparer) {
        comparer = comparer || linq.SortComparer;
        return arr.orderBy(linq.Selector, function (a, b) {
            var res = fn(a, b);
            return res === 0 ? -comparer(selector(a), selector(b)) : res;
        });
    };

    return arr.sort(fn);
};

Array.prototype.orderByDescending = function (selector, comparer) {
    comparer = comparer || linq.SortComparer;
    return this.orderBy(linq.Selector, function (a, b) {
        return -comparer(a, b);
    });
};

Array.prototype.innerJoin = function (arr, outer, inner, result, comparer) {
    comparer = comparer || linq.EqualityComparer;
    var res = [];

    this.forEach(function (t) {
        arr.where(function (u) {
            return comparer(outer(t), inner(u));
        })
            .forEach(function (u) {
                res.push(result(t, u));
            });
    });

    return res;
};

Array.prototype.groupJoin = function (arr, outer, inner, result, comparer) {
    comparer = comparer || linq.EqualityComparer;
    return this
        .select(function (t) {
            var key = outer(t);
            return {
                outer: t,
                inner: arr.where(function (u) {
                    return comparer(key, inner(u));
                }),
                key: key
            };
        })
        .select(function (t) {
            t.inner.key = t.key;
            return result(t.outer, t.inner);
        });
};

Array.prototype.groupBy = function (selector, comparer) {
    var grp = [];
    var l = this.length;
    comparer = comparer || linq.EqualityComparer;
    selector = selector || linq.Selector;

    for (var i = 0; i < l; i++) {
        var k = selector(this[i]);
        var g = grp.first(function (u) {
            return comparer(u.key, k);
        });

        if (!g) {
            g = [];
            g.key = k;
            grp.push(g);
        }

        g.push(this[i]);
    }
    return grp;
};

Array.prototype.toDictionary = function (keySelector, valueSelector) {
    var o = {};
    var l = this.length;
    while (l-- > 0) {
        var key = keySelector(this[l]);
        if (key == null || key === '') continue;
        o[key] = valueSelector(this[l]);
    }
    return o;
};

Array.prototype.aggregate = Array.prototype.reduce || function (func, seed) {
    var arr = this.slice(0);
    var l = this.length;
    if (seed == null) seed = arr.shift();

    for (var i = 0; i < l; i++)
        seed = func(seed, arr[i], i, this);

    return seed;
};

Array.prototype.min = function (s) {
    s = s || linq.Selector;
    var l = this.length;
    var min = s(this[0]);
    while (l-- > 0)
        if (s(this[l]) < min) min = s(this[l]);
    return min;
};

Array.prototype.max = function (s) {
    s = s || linq.Selector;
    var l = this.length;
    var max = s(this[0]);
    while (l-- > 0)
        if (s(this[l]) > max) max = s(this[l]);
    return max;
};

Array.prototype.sum = function (s) {
    s = s || linq.Selector;
    var l = this.length;
    var sum = 0;
    while (l-- > 0) sum += s(this[l]);
    return sum;
};

Array.prototype.where = Array.prototype.filter || function (predicate, context) {
    context = context || window;
    var arr = [];
    var l = this.length;
    for (var i = 0; i < l; i++)
        if (Predicate.call(context, this[i], i, this) === true) arr.push(this[i]);
    return arr;
};

Array.prototype.any = function (predicate, context) {
    context = context || window;
    predicate = predicate || linq.Predicate;
    var f = this.some || function (p, c) {
        var l = this.length;
        if (!p) return l > 0;
        while (l-- > 0)
            if (p.call(c, this[l], l, this) === true) return true;
        return false;
    };
    return f.apply(this, [predicate, context]);
};

Array.prototype.all = function (predicate, context) {
    context = context || window;
    predicate = predicate || linq.Predicate;
    var f = this.every || function (p, c) {
        return this.length == this.where(p, c).length;
    };
    return f.apply(this, [predicate, context]);
};

Array.prototype.takeWhile = function (predicate) {
    predicate = predicate || linq.Predicate;
    var l = this.length;
    var arr = [];
    for (var i = 0; i < l && predicate(this[i], i) === true; i++)
        arr.push(this[i]);

    return arr;
};

Array.prototype.skipWhile = function (predicate) {
    predicate = predicate || linq.Predicate;
    var l = this.length;
    var i = 0;
    for (i = 0; i < l; i++)
        if (predicate(this[i], i) === false) break;

    return this.skip(i);
};

Array.prototype.contains = function (o, comparer) {
    comparer = comparer || linq.EqualityComparer;
    var l = this.length;
    while (l-- > 0)
        if (comparer(this[l], o) === true) return true;
    return false;
};

Array.prototype.forEach = Array.prototype.forEach || function (callback, context) {
    context = context || window;
    var l = this.length;
    for (var i = 0; i < l; i++)
        callback.call(context, this[i], i, this);
};

Array.prototype.defaultIfEmpty = function (val) {
    return this.length === 0 ? [val === null ? null : val] : this;
};

Array.prototype.iFind = function (field, value, prop, def) {
    switch (arguments.length) {
        case 1:
            value = arguments[0];
            field = "id";
            break;
        case 2:
            value = arguments[0];
            prop = arguments[1];
            field = "id";
            break;
        default:
            break;
    }
    field = field || "id";
    prop = prop || "name";
    def = def || "";

    var item = this.first(function (m) {
        if (m[field] instanceof Array) {
            return m[field].contains(value);
        }
        else {
            return m[field] == value;
        }
    });
    return item == null ? def : item[prop];
};

Array.range = function (start, count) {
    var arr = [];
    while (count-- > 0) {
        arr.push(start++);
    }
    return arr;
};

//date

Date.prototype.format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1,                 //月份 
        "d+": this.getDate(),                    //日 
        "h+": this.getHours(),                   //小时 
        "m+": this.getMinutes(),                 //分 
        "s+": this.getSeconds(),                 //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds()             //毫秒 
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

Date.prototype.addDay = function (days) {
    this.setTime(this.getTime() + (days * 86400000));
    return this;
};

Date.prototype.addMonth = function (months) {
    this.setMonth(this.getMonth() + months);
    return this;
};

Date.prototype.AddMinute = function (minites) {
    this.setTime(this.getTime() + (minites * 60000));
    return this;
};

Date.prototype.AddSecond = function (second) {
    this.setTime(this.getTime() + (second * 1000));
    return this;
};

Date.prototype.AddHour = function (hours) {
    this.setTime(this.getTime() + (hours * 3600000));
    return this;
};

Date.prototype.timestamp = function () {
    return Math.floor(this.getTime() / 1000);
}

//object

Object.defineProperty(Object.prototype, 'pKeys', {
    get: function () {
        return Object.keys(this);
    }
});

//string

String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}

String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}

String.prototype.trim = function () {
    return this ? this.replace(/(^\s*)|(\s*$)/g, "") : "";
}

String.prototype.emptyDef = function (def) {
    if (this && this.trim().length > 0) {
        return this;
    }
    return def;
}

String.prototype.dateFormat = function (fmt) {
    var dateStr = this.replace("T", " ").replace("Z", "");
    if (dateStr.startWith("/Date\\(")) {
        var jsonDate = dateStr.replace("/Date(", "").replace(")/", "");
        if (jsonDate.indexOf("+") > 0) {
            jsonDate = jsonDate.substring(0, jsonDate.indexOf("+"));
        }
        else if (jsonDate.indexOf("-") > 0) {
            jsonDate = jsonDate.substring(0, jsonDate.indexOf("-"));
        }
        return new Date(parseInt(jsonDate, 10)).format(fmt);
    }
    else {
        var d = Date.parse(dateStr);
        if (isNaN(d)) {
            d = new Date(parseInt(dateStr, 10) * 1000);
            if (isNaN(d))
                return;
            return d.format(fmt);
        }

        return new Date(dateStr).format(fmt);
    }
};

String.prototype.highlight = function (key, tag) {
    var self = this;
    key = key.trim();
    var keys = key.split(" ");
    tag = tag || "em";
    keys.forEach(function (k) {
        k = k.trim();
        self = self.replace(new RegExp(k, "g"), "<" + tag + ">" + k + "</" + tag + ">");
    });
    //title.replace(/\s+/g, " ").replace(/&#013;/g, ""); 特殊行为 去_heler封装
    return self;
};

Number.prototype.dateFormat = function (format) {
    return new Date(this * 1000).format(format);
}