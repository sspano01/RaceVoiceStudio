function secsToTime(seconds) { 
    var mins = Math.floor(seconds / 60);
    mins = mins.toString();

    var secs = Math.floor(seconds - (mins*60));
    secs = strPadStart(secs.toString(), 2, '0');

    var ms = Math.floor((seconds - (mins*60) - secs) * 1000);
    ms = strPadStart(ms.toString(), 3, '0');

    return mins + "." + secs + "." + ms;
}

function pctToStr(pct) {
    return (Math.floor(pct*1000)/10).toString() + "%";
}

function strPadStart(str, n, pad) {
    for (var i = 0; i < n - str.length; i++) {
        str = pad + str;
    }

    return str;
}