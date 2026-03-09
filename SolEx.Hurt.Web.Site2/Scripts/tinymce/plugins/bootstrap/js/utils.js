/* get custom Bootstrap styles */

function getBootstrapStyles()
{
    var bodyBackground;
    var previewBackground;
    var choiceTitleBorderTop;
    var choiceTitleBorderTopAfter;
    bodyBackground = previewBackground = $(context).css('background-color');
    // bodyBackground = previewBackground = $('body').css('background-color');console.log(bodyBackground);
    if(bodyBackground.match(/rgb(255, 255, 255)/)) {
        $('body', context).css('background-color', '#fafafa');
    } else {
        bodyBackground = shadeRGBColor(bodyBackground, 0.07);
        choiceTitleBorderTop = shadeRGBColor(bodyBackground, -0.2);
        choiceTitleBorderTopAfter = shadeRGBColor(bodyBackground, 0.3);
        $('body', context).css('background-color', bodyBackground);
        $('.choice-title, #preview-title, #code-title', context).css('border-top-color', choiceTitleBorderTop);
        $('<style>.choice-title:after, #preview-title:after, #code-title:after{border-top-color: '+choiceTitleBorderTopAfter+'}</style>').appendTo('head', context);
        $('#preview', context).css('background-color', previewBackground);
    }
}

function shadeRGBColor(color, percent) {
    var f=color.split(","),t=percent<0?0:255,p=percent<0?percent*-1:percent,R=parseInt(f[0].slice(4)),G=parseInt(f[1]),B=parseInt(f[2]);
    return "rgb("+(Math.round((t-R)*p)+R)+","+(Math.round((t-G)*p)+G)+","+(Math.round((t-B)*p)+B)+")";
}

/* btn-toggle */

function toggleBtn(toggleGroup) {
    $(toggleGroup, context).find('.btn').toggleClass('active');
    $(toggleGroup, context).find('.btn').toggleClass('btn-success');
    $(toggleGroup, context).find('.btn').toggleClass('btn-default');
    if($(toggleGroup, context).find('.active').attr("data-attr") == 'true') {
        return true;
    } else {
        return false;
    }
}

function toggleAllBtns(toggleGroup, trueFalse) {
    $(toggleGroup, context).each(function() {
        if(!trueFalse) { // set all buttons to true
            $(this).find('button[data-attr="true"]').addClass('active').removeClass('btn-default').addClass('btn-success');
            $(this).find('button[data-attr="false"]').removeClass('active').removeClass('btn-success').addClass('btn-default');
        } else { // set all buttons to false
            $(this).find('button[data-attr="false"]').addClass('active').removeClass('btn-default').addClass('btn-success');
            $(this).find('button[data-attr="true"]').removeClass('active').removeClass('btn-success').addClass('btn-default');
        }
    });
    $(toggleGroup, context).find('.btn').toggleClass('active');
    $(toggleGroup, context).find('.btn').toggleClass('btn-success');
    $(toggleGroup, context).find('.btn').toggleClass('btn-default');
    if($(toggleGroup, context).find('.active').attr("data-attr") == 'true') {
        return true;
    } else {
        return false;
    }
}

/* code update */

function updateCode()
{
    var code = $('#test-wrapper', context).html(); // get preview content

    /* remove table content only for new tables (with false-text content) */

    if($('<div>').html(code).find('table.table').length > 0) {
        if($('<div>').html(code).find('table.table').attr('data-attr') == "new-table") {
            code = $('<div>').html(code).find('th, td').text('').closest('div').html();
        }
    }

    var find = new Array(/\s?data-mce-[a-z]+="[^"]+"/g);
    var replace = new Array('', '', '', '', '');

    for (var i = find.length - 1; i >= 0; i--) {
        code = code.replace(find[i], replace[i]);
    }

    /* render code */

    /* remove unwanted classes, attrs, ... */

    var allowedAttrs = new Array(["class"]);
    var allowedEmpty   = new Array(["span"]);

    if($('#test-wrapper', context).hasClass('test-icon-wrapper')) {

    /* keep styles for icons and add beginning space
    (otherwise tinymce will not allow empty tag and will remove your icon) */

        allowedAttrs.push(["style"]);
        code = '&nbsp;' + $.trim(escapeHtml($.htmlClean(code, {format:true, allowedAttributes: allowedAttrs, allowEmpty: allowedEmpty})));
    } else {
        code = $.trim(escapeHtml($.htmlClean(code, {format:true, allowedAttributes: allowedAttrs, allowEmpty: allowedEmpty})));
    }
    $('#code-wrapper', context).html('<pre class="prettyprint">' + code + '</pre>');
    // PR.prettyPrint();
    prettyPrint();
    parent.document.getElementById("bs-code").value = code;
}

/* code escape html */

function escapeHtml(text)
{
  var map = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;'
  };

  return text.replace(/[&<>"']/g, function (m) { return map[m]; });
}

/* code toggle */

$('#code-title a', context).on('click', function () {
    $('#code-wrapper', context).slideToggle(400, function () {
        if ($('#code-wrapper', context).css('display') == 'block') {
            $('#code-title a i', context).removeClass('glyphicon-arrow-down').addClass('glyphicon-arrow-up');
        } else {
            $('#code-title a i', context).removeClass('glyphicon-arrow-up').addClass('glyphicon-arrow-down');
        }
        return false;
    });
});
