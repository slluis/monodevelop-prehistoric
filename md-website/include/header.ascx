<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3c.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
  <head>
    <link rel="stylesheet" media="screen" href="/styles.css" type="text/css" />
    <title>MonoDevelop</title>
<!--[if gte IE 5.5000]>
<script language="JavaScript">
function correctPNG() // correctly handle PNG transparency in Win IE 5.5 or higher.
   {
   for(var i=0; i<document.images.length; i++)
      {
	  var img = document.images[i]
	  var imgName = img.src.toUpperCase()
	  if (imgName.substring(imgName.length-3, imgName.length) == "PNG")
	     {
		 var imgID = (img.id) ? "id='" + img.id + "' " : ""
		 var imgClass = (img.className) ? "class='" + img.className + "' " : ""
		 var imgTitle = (img.title) ? "title='" + img.title + "' " : "title='" + img.alt + "' "
		 var imgStyle = "display:inline-block;" + img.style.cssText 
		 if (img.align == "left") imgStyle = "float:left;" + imgStyle
		 if (img.align == "right") imgStyle = "float:right;" + imgStyle
		 if (img.parentElement.href) imgStyle = "cursor:hand;" + imgStyle		
		 var strNewHTML = "<span " + imgID + imgClass + imgTitle
		 + " style=\"" + "width:" + img.width + "px; height:" + img.height + "px;" + imgStyle + ";"
	     + "filter:progid:DXImageTransform.Microsoft.AlphaImageLoader"
		 + "(src=\'" + img.src + "\', sizingMethod='scale');\"></span>" 
		 img.outerHTML = strNewHTML
		 i = i-1
	     }
      }
   }
window.attachEvent("onload", correctPNG);
</script>
<![endif]-->
  </head>
  <body>
    <div id="header">
      <img src="/images/mono-develop.png" alt="" height="62" width="357" />
    </div>
    <div id="flags">
      <a href="/"><img src="/images/flags/usa.png" /></a>
      <a href="/lang/fr/"><img src="/images/flags/france.png" /></a>
    </div>
    <div class="left-nav">
      <ul>
        <li><a href="/">Home</a></li>
        <li><a href="/news.aspx">News</a></li>
	<li><a href="/about.aspx">About</a></li>
	<li><a href="/screenshots.aspx">Screenshots</a></li>
	<li><a href="/release.aspx">Releases</a></li>
	<li><a href="/tutorials/package_install.aspx">Installation</a></li>
	<li><a href="/contribute.aspx">Contribute</a></li>
	<li><a href="/tutorial.aspx">Tutorials</a></li>
	<li><a href="/faq.aspx">FAQ</a></li>
        <li><a href="/wiki/">Wiki</a></li>
      </ul>
      <br />
      <a href="http://www.cityhost.ca/">
         <img src="http://www.cityhost.ca/images/cityhost-button.png" border='0' />
      </a>
    </div>
    <div id="content">
