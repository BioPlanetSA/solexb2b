//function handleFileUpload(files, obj, katalog,callback,deleteCallback) {
//    for (var i = 0; i < files.length; i++) {
//        var fd = new FormData();
//        fd.append('file', files[i]);
//        fd.append('katalog', katalog);

//        //czy rozmiar jest MNIEJSZY niz 10 MB = 10 000 000
//        if (files[i].size > 10500000) {
//            Informacja(this, "Zbyt duży rozmiar pliku", "Rozmiar pliku: <b>" + files[i].name + "</b> jest zbyt duży - pliki powinny być nie większe niż 10MiB.", null);
//        } else {
//            var status = new CreateStatusbar(obj, false, deleteCallback, 0); //Using this we can set progress.
//            sendFileToServer(fd, status, obj, callback, files[i].name, files[i].size);
//        }
//    }
//}

//function wyliczRozmiarLadny(fileSize) {
//    var sizeKB = fileSize / 1024;
//    if (parseInt(sizeKB) > 1024) {
//        var sizeMB = sizeKB / 1024;
//        fileSize = sizeMB.toFixed(2) + " MiB";
//    }
//    else {
//        fileSize = sizeKB.toFixed(2) + " KiB";
//    }
//    return fileSize;
//}

//function sendFileToServer(formData, status, obj, callback, fileName, fileSize) {
//    //obliczenie size z bajtów do normalnej postaci
//    fileSize = wyliczRozmiarLadny(fileSize);

//    status.setProgress(0, fileName, fileSize);
//    var uploadURL = "/Pliki/UploadAjax"; //Upload URL
//    var extraData = {}; //Extra Data.
//    var jqXHR = $.ajax({
//        xhr: function () {
//            var xhrobj = $.ajaxSettings.xhr();
//            if (xhrobj.upload) {
//                xhrobj.upload.addEventListener('progress', function (event) {
//                    var percent = 0;
//                    var position = event.loaded || event.position;
//                    var total = event.total;
//                    if (event.lengthComputable) {
//                        percent = Math.ceil(position / total * 100);
//                    }
//                    //Set progress
//                    status.setProgress(percent, fileName, fileSize);
//                }, false);
//            }
//            return xhrobj;
//        },
//        url: uploadURL,
//        type: "POST",
//        contentType: false,
//        processData: false,
//        cache: false,
//        data: formData,
//        success: function (data) {
//            status.setProgress(100, fileName, fileSize);
//            status.setFileId(parseInt(data));
//            if (callback != undefined) {
//                callback(obj,parseInt( data));
//            }
//        }
//    });

//    status.setAbort(jqXHR);
//}

//function CreateStatusbar(obj,noProgresBar,deleteCallback,fileid) {
//    var row = "parzyste";
//    var rowCount = obj.siblings('.pliki-lista').find('.statusbar').length;
//    if (rowCount % 2 == 0) row = "nieparzyste";
//    this.statusbar = $("<div class='statusbar " + row + "'></div>");
//    this.progressBar = $('<div class="progress"> <div class="progress-bar" style="width: 0%;"></div></div>');
//    this.abort = $("<span class='fa fa-remove close'></span>");
//    this.delete = $("<span class='fa fa-remove close'></span>");
//    if (noProgresBar == false) {
//        this.progressBar.appendTo(this.statusbar);
//        this.abort.appendTo(this.statusbar);
//    }
//    this.fileid = fileid;
//    var that = this;
//    this.delete.appendTo(this.statusbar).on('click', function () {
//        that.statusbar.hide();
//        deleteCallback(obj, that.fileid);
//    }).hide();

//    obj.siblings('.pliki-lista').append(this.statusbar);

//    this.setProgress = function (progress, fileName, fileSize, nieAnimujPaska) {
//        var progressBarWidth = progress * this.progressBar.width() / 100;
//        var rozmiarPliku = ' &nbsp;&nbsp;  (' + fileSize + ') ';
      

//        if (nieAnimujPaska) {
//            this.progressBar.find('div').hide();
//            this.progressBar.find('div').width(progressBarWidth);
//            this.progressBar.find('div').show();
//        }

//        this.progressBar.find('div').animate({ width: progressBarWidth }, 10).html(fileName +rozmiarPliku + '&nbsp;&nbsp;<b>' + progress + "% </b> ");
//        if (parseInt(progress) >= 100) {
//            this.abort.hide();
//            this.delete.show();
//        }
//    }

//    this.setFileNameSize = function (name, size) {
//        var sizeStr = wyliczRozmiarLadny(size);
//        this.setProgress(100, name, sizeStr);
//    }


//    this.setAbort = function (jqxhr) {
//        var sb = this.statusbar;
//        this.abort.click(function () {
//            jqxhr.abort();
//            sb.hide();
//        });
//    }
//    this.setFileId=function(id) {
//        this.fileid = id;
//    }
//    this.getFileId = function () {
//       return this.fileid;
//    }
//}