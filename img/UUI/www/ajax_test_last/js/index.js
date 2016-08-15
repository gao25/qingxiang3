(function (doc, win) {
    var docEl = doc.documentElement,
        resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
        recalc = function () {
            var clientWidth = docEl.clientWidth;
            if (!clientWidth) return;
            docEl.style.fontSize = 100* (clientWidth / 640) + 'px';
        };

    if (!doc.addEventListener) return;
    win.addEventListener(resizeEvt, recalc, false);
    doc.addEventListener('DOMContentLoaded', recalc, false);
})(document, window);
var pageNum=1;
$(document).ready(function(){
    function getStart(){
        $.ajax({
            url:'index.json',
            type:'GET',
            dataType:'JSON',
            data:{
                pageNum:pageNum
            },
            success:function(data){
                if (data.state!="success") {
                    console.log(data);
                    return;
                }else{
                    getBanerImg(data);
                    getData(data);
                }
            },
            error:function(){},
        })
    }
    getStart();

    //getBannerImg函数
    function getBanerImg(data){
        var Baner="";
        var Topic="";
        for(var i=0;i<data.data_scroll.length;i++){
            if (data.data_scroll[i].topic.length>16) {
                data.data_scroll[i].topic=data.data_scroll[i].topic.slice(0,16)+"...";
            }else{
                data.data_scroll[i].topic=data.data_scroll[i].topic;
            }
            if (data.data_scroll[i].showtype=="3007") {
                Baner+="<div class='swiper-slide'>"+"<a href='"+data.data_scroll[i].detailurl+"'><img class='play' src='images/play.png'><img class='news-img' src='"+data.data_scroll[i].imglist[0]+"'></a></div>";
            }else{
                Baner+="<div class='swiper-slide'>"+"<a href='"+data.data_scroll[i].detailurl+"'><img class='news-img' src='"+data.data_scroll[i].imglist[0]+"'></a></div>";
            }
        }
        $('.swiper-wrapper').html(Baner);
        var mySwiper = new Swiper('.swiper-container',{
            autoplay:5000,
            loop:true,
            onSlideChangeEnd:function(mySwiper){
                if (mySwiper.activeIndex==0) {
                    Topic="<span class='title'>"+data.data_scroll[data.data_scroll.length-1].topic+"</span><span class='page-number'>"+parseInt(data.data_scroll.length)+"/"+data.data_scroll.length+"</span>";
                }else if(mySwiper.activeIndex==data.data_scroll.length+1){
                    Topic="<span class='title'>"+data.data_scroll[0].topic+"</span><span class='page-number'>"+"1"+"/"+data.data_scroll.length+"</span>";
                }else{
                Topic="<span class='title'>"+data.data_scroll[mySwiper.activeIndex-1].topic+"</span><span class='page-number'>"+parseInt(mySwiper.activeIndex)+"/"+data.data_scroll.length;
                }
                $('.topic').html(Topic);
            }
        })
    }
    function getData(data){
        var Data="";
        for(var i=0;i<data.data.length;i++){
            if (data.data[i].showtype=="3004") {
                Data+="<a href='"+data.data[i].detailurl+"'><div class='news-three-img clear'><div class='news-header clear'><div class='news-three-topic'>"+data.data[i].topic+"</div>"+"<div class='news-three-comment clear'><img src='images/comm.png'>"+"<span>"+data.data[i].comment+"</span></div></div>"+"<div class='news-pic-list clear'>"+"<img class='news-img-three' src='"+data.data[i].imglist[0]+"'><img class='news-img-three img2' src='"+data.data[i].imglist[1]+"'><img class='news-img-three' src='"+data.data[i].imglist[2]+"'>"+"</div></div></a>"
            }
            if (data.data[i].showtype=="3007") {
                Data+="<a href='"+data.data[i].detailurl+"'><div class='news clear'><img src='images/play.png' class='play1'><img class='news-img' src='"+data.data[i].imglist[0]+"'><div class='news-right'><div class='news-topic'>"+data.data[i].topic+"</div>"+"<div class='news-footer'><span class='news-time'>"+data.data[i].releasedate+"</span><span class='news-type'>#"+data.data[i].newschannelname+"</span><span class='news-comment'><img src='images/comm.png'><span>"+data.data[i].comment+"</span></span>"+"</div></div></div></a>"
            }
            if (data.data[i].showtype=="3002"||data.data[i].showtype=="3003") {
                if (data.data[i].newschannelname=="") {
                    Data+="<a href='"+data.data[i].imglist[0]+"'><div class='news clear'><img class='news-img' src='"+data.data[i].imglist[0]+"'>"+"<div class='news-right'><div class='news-topic'>"+data.data[i].topic+"</div>"+"<div class='news-footer'><span class='news-time'>"+data.data[i].releasedate+"</span><span class='news-comment'><img src='images/comm.png'><span>"+data.data[i].comment+"</span></span>"+"</div></div></div></a>"
                }else{
                    Data+="<a href='"+data.data[i].detailurl+"'><div class='news clear'><img class='news-img' src='"+data.data[i].imglist[0]+"'><div class='news-right'><div class='news-topic'>"+data.data[i].topic+"</div>"+"<div class='news-footer'><span class='news-time'>"+data.data[i].releasedate+"</span><span class='news-type'>#"+data.data[i].newschannelname+"</span><span class='news-comment'><img src='images/comm.png'><span>"+data.data[i].comment+"</span></span>"+"</div></div></div></a>"
                }
            }
        }
        $('.content').append(Data);
    }
        $('.btn').click(function(){
            pageNum++;
            getStart();
        })
})