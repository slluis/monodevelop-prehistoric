using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Xml;


using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.StartPage 
{
	public enum ColorScheme
	{
		blue,
		red,
		green,
		brown,
		orange
	}

	public class MenuItem
	{
		public string Caption, URL;

		public MenuItem(string strCaption, string strUrl)
		{
			Caption = strCaption;
			URL = strUrl;
		}
	}
	
	public class ICSharpCodePage
	{
		ColorScheme _ColorScheme;
		
		string startPageLocation;
		
		string m_strMainColColor, m_strSubColColor;
		int m_nLeftTopImageWidth, m_nRightTopImageWidth;
		bool m_bShowMilestoneContentImage;
		
		private int nTotalColumns = 0;

		string m_strTitle, m_strMetaDescription, m_strMetaKeywords, m_strMetaAuthor, m_strMetaCopyright;
		string m_strStaticStyleSheet, m_strRightBoxHtml;

		bool m_bShowLeftMenu, m_bShowRightBox, m_bShowContentBar;
		string m_strContentBarText, m_strTopMenuSelectedItem, m_strLeftMenuSelectedItem;
		string m_strVersionText, m_strVersionStatus;

		public string PrimaryColor
		{
			get { return m_strMainColColor; }
		}

		public string SecondaryColor
		{
			get { return m_strSubColColor; }
		}

		public string Title
		{
			get { return m_strTitle; }
			set { m_strTitle = value; }
		}

		public bool ShowMilestoneContentImage
		{
			get { return m_bShowMilestoneContentImage; }
			set { m_bShowMilestoneContentImage = value; }
		}
		
		public string MetaDescription
		{
			get { return m_strMetaDescription; }
			set { m_strMetaDescription = value; }
		}

		public string MetaKeywords
		{
			get { return m_strMetaKeywords; }
			set { m_strMetaKeywords = value; }
		}

		public string MetaAuthor
		{
			get { return m_strMetaAuthor; }
			set { m_strMetaAuthor = value; }
		}

		public string MetaCopyright
		{
			get { return m_strMetaCopyright; }
			set { m_strMetaCopyright = value; }
		}

		public string StaticStyleSheet
		{
			get { return m_strStaticStyleSheet; }
			set { m_strStaticStyleSheet = value; }
		}

		public string ContentBarText
		{
			get { return m_strContentBarText; }
			set { m_strContentBarText = value; }
		}

		public bool ShowLeftMenu
		{
			get { return m_bShowLeftMenu; }
			set { m_bShowLeftMenu = value; }
		}

		public bool ShowRightBox
		{
			get { return m_bShowRightBox; }
			set { m_bShowRightBox = value; }
		}

		public bool ShowContentBar
		{
			get { return m_bShowContentBar; }
			set { m_bShowContentBar = value; }
		}
  
		private ArrayList TopMenu;
		private ArrayList LeftMenu;

		public virtual void PopulateTopMenu()
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			TopMenu.Add(new MenuItem(stringParserService.Parse("${res:StartPage.StartMenu.Name}"),      "/Start/opensection"));
			TopMenu.Add(new MenuItem(stringParserService.Parse("${res:StartPage.ChangeLogMenu.Name}"),  "/ChangeLog/opensection"));
			TopMenu.Add(new MenuItem(stringParserService.Parse("${res:StartPage.AuthorsMenu.Name}"),    "/Authors/opensection"));
			TopMenu.Add(new MenuItem(stringParserService.Parse("${res:StartPage.HelpWantedMenu.Name}"), "/HelpWanted/opensection"));
		}
		
		public virtual void PopulateLeftMenu()
		{
//			LeftMenu.Add(new MenuItem("Start",       "/OpenSource/SD/AnnouncementList.asp"));
//			LeftMenu.Add(new MenuItem("ChangeLog",   "/OpenSource/SD/WhatsNew.asp"));
//			LeftMenu.Add(new MenuItem("Authors",     "/OpenSource/SD/NewsHistory.asp"));
//			LeftMenu.Add(new MenuItem("Readme",      "/OpenSource/SD/NewsHistory.asp"));
//			LeftMenu.Add(new MenuItem("Help Wanted", "/pub/relations/"));
		}
		
		public string TopMenuSelectedItem
		{
			get { return m_strTopMenuSelectedItem; }
			set { m_strTopMenuSelectedItem = value; }
		}

		public string LeftMenuSelectedItem
		{
			get { return m_strLeftMenuSelectedItem; }
			set { m_strLeftMenuSelectedItem = value; }
		}

		public string VersionText
		{
			get { return m_strVersionText; }
			set { m_strVersionText = value; }
		}

		public string VersionStatus
		{
			get { return m_strVersionStatus; }
			set { m_strVersionStatus = value; }
		}

		public string RightBoxHtml
		{
			get { return m_strRightBoxHtml; }
			set { m_strRightBoxHtml = value; }
		}

		public virtual void RenderRightBoxHtml(StringBuilder builder)
		{
			builder.Append(m_strRightBoxHtml);
		}

		public ICSharpCodePage()
		{
			ColorScheme = ICSharpCode.StartPage.ColorScheme.blue;
			
			TopMenu = new ArrayList();
			PopulateTopMenu();
			TopMenuSelectedItem = "Home";

			LeftMenu = new ArrayList();
			PopulateLeftMenu();
			LeftMenuSelectedItem = "";
			
			Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
			VersionText   = "milestone " + v.Major + "." + v.Minor;
			VersionStatus = "Beta";
			
			RightBoxHtml = "";
			
			StaticStyleSheet = startPageLocation + "/Layout/default.css";
			MetaAuthor = "Christoph Wille - christophw@alphasierrapapa.com";
			MetaCopyright = "(c) 2001-2002 AlphaSierraPapa";

			ShowLeftMenu = false;
			ShowRightBox = false;
			ShowContentBar = true;
		}
		
		public ColorScheme ColorScheme
		{
			get { return _ColorScheme; }
			set 
			{
				_ColorScheme = value;
				m_bShowMilestoneContentImage = false;

				switch (_ColorScheme)
				{
					case ColorScheme.blue:
						m_nLeftTopImageWidth = 292;//412;
						m_nRightTopImageWidth = 363;
						m_strSubColColor =  "#C2E0FB";
						m_strMainColColor = "#A8C6E3";
						m_bShowMilestoneContentImage = true;
						break;
					case ColorScheme.red:
						m_nLeftTopImageWidth = 214;//334;
						m_nRightTopImageWidth = 438;
						m_strSubColColor =  "#a7a9ac";
						m_strMainColColor = "#d7797d";
						break;
					case ColorScheme.brown:
						m_nLeftTopImageWidth = 294;//415;
						m_nRightTopImageWidth = 359;
						m_strSubColColor =  "#EEE9E2";
						m_strMainColColor = "#D5D0C9";
						break;
					case ColorScheme.green:
						m_nLeftTopImageWidth = 259;//450;
						m_nRightTopImageWidth = 325;
						m_strSubColColor =  "#E7EDBB";
						m_strMainColColor = "#CED4A2";
						break;
					case ColorScheme.orange:
						m_nLeftTopImageWidth = 191;//311;
						m_nRightTopImageWidth = 460;
						m_strSubColColor =  "#F4D97B";
						m_strMainColColor = "#E7CD6F";
						break;
				}
			}
		}

		public virtual void RenderHeaderSection(StringBuilder builder)
		{
			builder.Append("<html><head><title>");
			builder.Append(Title);
			builder.Append("</title>\r\n");
			builder.Append("<META HTTP-EQUIV=\"content-type: text/html; charset= ISO-8859-1\">\r\n");
			builder.Append("<META NAME=\"robots\" CONTENT=\"FOLLOW,INDEX\">\r\n");
			builder.Append("<meta name=\"Author\" content=\"");
			builder.Append(MetaAuthor);
			builder.Append("\">\r\n<META NAME=\"copyright\" CONTENT=\"");
			builder.Append(MetaCopyright);
			builder.Append("\">\r\n<meta http-equiv=\"Description\" name=\"Description\" content=\"");
			builder.Append(MetaDescription);
			builder.Append("\">\r\n<meta http-equiv=\"Keywords\" name=\"Keywords\" content=\"");
			builder.Append(MetaKeywords);
			builder.Append("\">\r\n<link rel=\"stylesheet\" href=\"");
			builder.Append(StaticStyleSheet);
			builder.Append("\">\r\n</head>\r\n<body bgcolor=\"#ffffff\">\r\n");
		}

		public virtual void RenderPageEndSection(StringBuilder builder)
		{
			builder.Append("</body>\r\n</html>\r\n");
		}

		public virtual void RenderPageTopSection(StringBuilder builder)
		{
			builder.Append("<div class=\"balken\" style=\"position:absolute;left:0px;top:0px\">");
			builder.Append("<table border=0 cellspacing=0 cellpadding=0><TR>\r\n");
			builder.Append("<td height=72 background=\""+ startPageLocation +"/Layout/");
			builder.Append(Enum.GetName(typeof(ColorScheme), _ColorScheme));
			builder.Append("/balken_links.gif\"><img src=\"" + startPageLocation + "/Layout/Common/blind.gif\" width=");
			builder.Append(m_nLeftTopImageWidth.ToString());
			builder.Append(" height=1></td>\r\n");
			builder.Append("<td width=\"100%\" background=\""+ startPageLocation + "/Layout/");
			builder.Append(Enum.GetName(typeof(ColorScheme), _ColorScheme));
			builder.Append("/balken_mitte.gif\">&nbsp;</td>\r\n");
			builder.Append("<td background=\""+ startPageLocation + "/Layout/");
			builder.Append(Enum.GetName(typeof(ColorScheme), _ColorScheme));
			builder.Append("/balken_rechts.gif\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=");
			builder.Append(m_nRightTopImageWidth.ToString());
			builder.Append(" height=1></td>\r\n</TR></table>\r\n");
			builder.Append("<table border=0 cellspacing=0 cellpadding=0><tr>");
			builder.Append("<td width=\"100%\" height=24 align=left bgcolor=\"#DCDDDE\">");
			builder.Append("<table border=0 cellspacing=0 cellpadding=0 width=\"100%\"><tr>\r\n");
			builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=15 height=1></td>\r\n");

			int nTopMenuElements = TopMenu.Count;
			foreach (MenuItem de in TopMenu)
			{
				--nTopMenuElements;

				builder.Append("<td class=\"navi");
				if (0 == String.Compare(de.Caption, m_strTopMenuSelectedItem, true))
				{
					builder.Append("Activ\">");
					builder.Append(de.Caption);
					builder.Append("</td>\r\n");
				}
				else
				{
					builder.Append("\"><a href=\"");
					builder.Append(de.URL);
					builder.Append("\">");
					builder.Append(de.Caption);
					builder.Append("</a></td>\r\n");
				}

				if (0 != nTopMenuElements)
				{
					builder.Append("<td width=13><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=5 height=1><img src=\""+ startPageLocation + "/Layout/Common/line_hor_black.gif\" width=1 height=15><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=5 height=1></td>\r\n");
				}
			}

			builder.Append("</tr></table></td></tr></table></DIV>\r\n");
		}

		public virtual void RenderLeftMenu(StringBuilder builder)
		{
			builder.Append("<td bgcolor=\"White\" valign=\"top\"><table border=0 cellspacing=0 cellpadding=0>");
			builder.Append("<tr><td width=20 heigth=10><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=20 height=10></td>");
			builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=1></td>");
			builder.Append("</tr><tr><td colspan=2><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=4></td></tr>");

			int nLeftMenuElements = LeftMenu.Count;
			foreach (MenuItem de in LeftMenu)
			{
				--nLeftMenuElements;
				builder.Append("<tr>");

				if (0 == String.Compare(de.Caption, m_strLeftMenuSelectedItem, true))
				{
					builder.Append("<td width=20><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=10 height=1><img src=\""+ startPageLocation + "/Layout/Common/dot_listing.gif\" width=8 height=8><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=2 height=1></td>\r\n");
					builder.Append("<td class=\"naviListDevelopActiv\">");
					builder.Append(de.Caption);
					builder.Append("</td></tr>\r\n");
				}
				else
				{
					builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/allgemein/blind.gif\" width=20 height=1></td>\r\n");
					builder.Append("<td class=\"naviListDevelop\"><a href=\"");
					builder.Append(de.URL);
					builder.Append("\">");
					builder.Append(de.Caption);
					builder.Append("</a></td></tr>\r\n");
				}

				if (0 != nLeftMenuElements)
				{
					builder.Append("<tr><td colspan=2><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=4></td></tr>\r\n");
				}
			}

			builder.Append("</table></td>");
			builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>");
		}

		public virtual void RenderFirstPageBodySection(StringBuilder builder)
		{
			builder.Append("<div class=\"text\" style=\"position:absolute;left:0px;top:120px\"><table border=0 cellspacing=0 cellpadding=0>\r\n");

			if (ShowContentBar)
			{
				nTotalColumns = 1;
				builder.Append("<tr>");

				if (ShowLeftMenu)
				{
					builder.Append("<td height=43 bgcolor=\"");
					builder.Append(m_strSubColColor);
					builder.Append("\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=135 height=1></td>");
					builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>");
					nTotalColumns += 2;
				}

				builder.Append("<td width=\"100%\" bgcolor=\"");
				builder.Append(m_strMainColColor);
				builder.Append("\" class=\"head\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=15 height=1>");
				builder.Append(ContentBarText);	// TODO for virtual
				builder.Append("</td>\r\n");

				if (ShowRightBox)
				{
					builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>");
					if (ShowMilestoneContentImage)
					{
						builder.Append("<td height=43 bgcolor=\"");
						builder.Append(m_strSubColColor);
						builder.Append("\"><img src=\""+ startPageLocation + "/Layout/Common/milestone_col_head.gif\" width=138 height=43></td>");
					}
					else
					{
						builder.Append("<td height=43 bgcolor=\"");
						builder.Append(m_strSubColColor);
						builder.Append("\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=138 height=43></td>");
					}
					nTotalColumns += 2;
				}

				builder.Append("</tr><tr><td colspan=");
				builder.Append(nTotalColumns.ToString());
				builder.Append("><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td></tr>");
			}

			builder.Append("<tr>");
			if (ShowLeftMenu)
			{
				RenderLeftMenu(builder);
			}

			builder.Append("<td bgcolor=\"#d6d7d8\" valign=\"top\" height=270><table border=0 cellspacing=0 cellpadding=0>\r\n");
			builder.Append("<tr><td width=15><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=15 height=15></td>");
			builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=1></td>");
			builder.Append("<td width=15><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=15 height=1></td>");
			builder.Append("</tr><tr><td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=1></td>");
			builder.Append("<td class=\"copy\">\r\n");

		
		}

		public virtual void RenderFinalPageBodySection(StringBuilder builder)
		{
			builder.Append("</td><td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=1></td></tr></table></td>\r\n");
			if (ShowRightBox)
			{
				RenderRightBox(builder);
			}

			builder.Append("</tr><tr><td colspan=");
			builder.Append(nTotalColumns.ToString());
			builder.Append("><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td></tr><tr>\r\n");

			string strSubColor2Use = "#ffffff";
			if (ShowLeftMenu)
			{
				builder.Append("<td height=20 bgcolor=\"");
				builder.Append(strSubColor2Use);
				builder.Append("\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=135 height=1></td>");
				builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>\r\n");
			}

			builder.Append( "<td width=\"100%\" bgcolor=\"");
			builder.Append(m_strMainColColor);
			builder.Append("\" class=\"copy\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=15 height=1>");
			builder.Append("<font size=\"-2\">");
			builder.Append("Copyright &copy;2000-2003 <A HREF=\"mailto:webmaster@icsharpcode.net\" title=\"Contact Us\">IC#SharpCode</a>. Released under the terms of the GNU General Public License. Projects sponsored by <a href=\"http://www.alphasierrapapa.com\">AlphaSierraPapa</a>.</font></td>\r\n");

			if (ShowRightBox)
			{
				builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>");
				builder.Append("<td height=20 bgcolor=\"");
				builder.Append(strSubColor2Use);
				builder.Append("\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=138 height=20></td>\r\n");
			}
			builder.Append("</tr><tr><td colspan=");
			builder.Append(nTotalColumns.ToString());
			builder.Append("><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td></tr>\r\n");
			builder.Append("</table></div>\r\n");
		}

		public virtual void RenderRightBox(StringBuilder builder)
		{
			builder.Append("<td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td>\r\n");
			builder.Append("<td valign=\"top\"><table border=0 cellspacing=0 cellpadding=0><tr>");
			builder.Append("<td valign=\"top\" background=\""+ startPageLocation + "/Layout/Common/klinker_milestone.gif\" width=138 height=113>");
			builder.Append("<table border=0 cellspacing=0 cellpadding=0><tr><td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=3></td></tr>");
			builder.Append("<tr><td class=\"milestoneText\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=10 height=1>");
			builder.Append(VersionText);
			builder.Append("</td></tr>");
			builder.Append("<tr><td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=52></td></tr>");
			builder.Append("<tr><td class=\"milestoneText\"><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=75 height=1><font size=\"+2\">");
			builder.Append(VersionStatus);
			builder.Append("</font></td></tr>");
			builder.Append("</table></td></tr>");
			builder.Append("<tr><td width=1><img src=\""+ startPageLocation + "/Layout/Common/pixel_weiss.gif\" width=1 height=1></td></tr>");
			builder.Append("<tr><td bgcolor=\"#d6d7d8\" valign=\"top\" height=49><table border=0 cellspacing=0 cellpadding=0>");
			builder.Append("<tr><td width=10><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=5></td>");
			builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=1 height=1></td></tr><tr>");
			builder.Append("<td><img src=\""+ startPageLocation + "/Layout/Common/blind.gif\" width=10 height=1></td><td class=\"copyUnderlineBig\">");
			RenderRightBoxHtml(builder);
			builder.Append("</td>");
			builder.Append("</tr></table></td></tr></table></td>");
		}
		public string[] projectFiles;
		
		StringBuilder projectSection = null;
		public void RenderSectionStartBody(StringBuilder builder)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			if (projectSection == null) {
				projectSection = new StringBuilder();
				projectSection.Append("<DIV class='tablediv'><TABLE CLASS='dtTABLE' CELLSPACING='0'>\n");
				projectSection.Append(String.Format("<TR><TH>{0}</TH><TH>{1}</TH><TH>{2}</TH></TR>\n",
				                                    stringParserService.Parse("${res:StartPage.StartMenu.NameTable}"),
				                                    stringParserService.Parse("${res:StartPage.StartMenu.ModifiedTable}"),
				                                    stringParserService.Parse("${res:StartPage.StartMenu.LocationTable}")
				                                    ));
				
				try {
					// Get the recent projects
					Core.Properties.DefaultProperties svc = (Core.Properties.DefaultProperties)Core.Services.ServiceManager.Services.GetService(typeof(Core.Services.PropertyService));
					object recentOpenObj = svc.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
					if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) {
						ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
						projectFiles = new string[recOpen.RecentProject.Count];
						for (int i = 0; i < recOpen.RecentProject.Count; ++i) {
							string fileName = recOpen.RecentProject[i].ToString();
							
							// if the file does not exist, goto next one
							if (!System.IO.File.Exists(fileName)) {
								continue;
							}
							projectFiles[i] = fileName;
							projectSection.Append("<TR><TD>");
							projectSection.Append("<a href=\"project://" + i + "\">");
							projectSection.Append(ParseCombineFile(fileName));
							projectSection.Append("</A>");
							projectSection.Append("</TD><TD>");
							System.IO.FileInfo fInfo = new System.IO.FileInfo(fileName);
							projectSection.Append(fInfo.LastWriteTime.ToShortDateString());
							projectSection.Append("</TD><TD>");
							projectSection.Append(fileName);
							projectSection.Append("</TD></TR>\n");
						}
					}
				} catch {}
				projectSection.Append("</TABLE></DIV><BR/><BR/>");
				projectSection.Append(String.Format("<input type=button value='{0}' onClick=\"location.href('/opencombine');\">\n",
				                      stringParserService.Parse("${res:StartPage.StartMenu.OpenCombineButton}")
				                      ));
				projectSection.Append(String.Format("<input type=button value='{0}'  onClick=\"location.href('/newcombine');\">\n",
				                      stringParserService.Parse("${res:StartPage.StartMenu.NewCombineButton}")
				                      ));
				projectSection.Append("<BR/><BR/><BR/>");
			}
			builder.Append(projectSection.ToString());
		}
		
		public void RenderSectionAuthorBody(StringBuilder builder)
		{
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string html = ConvertXml.ConvertToString(propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + ".." +
				                   Path.DirectorySeparatorChar + "doc" +
				                   Path.DirectorySeparatorChar + "AUTHORS.xml",
				                   
				                   propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   Path.DirectorySeparatorChar + "ShowAuthors.xsl");
				builder.Append(html);
			} catch (Exception e) {
				//MessageBox.Show(e.ToString());
				throw e;
			}
		}

		public void RenderSectionChangeLogBody(StringBuilder builder)
		{
			try {
				PropertyService ps = (PropertyService) ServiceManager.Services.GetService (typeof(PropertyService));
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				string html = ConvertXml.ConvertToString(ps.DataDirectory +
				                   Path.DirectorySeparatorChar + ".." +
				                   Path.DirectorySeparatorChar + "doc" +
				                   Path.DirectorySeparatorChar + "ChangeLog.xml",
				                   
				                   ps.DataDirectory + 
				                   Path.DirectorySeparatorChar + ".." + 
				                   Path.DirectorySeparatorChar + "data" + 
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   Path.DirectorySeparatorChar + "ShowChangeLog.xsl");
				builder.Append(html);
			} catch (Exception e) {
				//MessageBox.Show(e.ToString());
				throw e;
			}
		}
		
		public void RenderSectionHelpWantedBody(StringBuilder builder)
		{
			try {
				PropertyService ps = (PropertyService)ServiceManager.Services.GetService (typeof(PropertyService));
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				string html = ConvertXml.ConvertToString(ps.DataDirectory +
				                   Path.DirectorySeparatorChar + ".." +
				                   Path.DirectorySeparatorChar + "doc" +
				                   Path.DirectorySeparatorChar + "HowYouCanHelp.xml",
				                   
				                   ps.DataDirectory + 
				                   Path.DirectorySeparatorChar + ".." + 
				                   Path.DirectorySeparatorChar + "data" + 
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   Path.DirectorySeparatorChar + "ShowHowYouCanHelp.xsl");
				builder.Append(html);
			} catch (Exception e) {
				//MessageBox.Show(e.ToString());
				throw e;
			}
		}
		
		/// <summary>
		/// Extracts a combine name from the specified file; return fileName on error
		/// </summary>
		string ParseCombineFile(string fileName) 
		{
			XmlTextReader reader = new XmlTextReader(fileName);
			reader.MoveToContent();
			if (reader.MoveToAttribute("name")) {
				return reader.Value;
			}
			return fileName;
		}
		
		public string Render(string section) 
		{
			PropertyService ps = (PropertyService) ServiceManager.Services.GetService (typeof(PropertyService));
			startPageLocation = ps.DataDirectory + Path.DirectorySeparatorChar + 
			                    ".." + Path.DirectorySeparatorChar +
			                    "data" + Path.DirectorySeparatorChar +
			                    "resources" + Path.DirectorySeparatorChar +
			                    "startpage";
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			switch (section) {
				case "Start":
					ContentBarText = stringParserService.Parse("${res:StartPage.StartMenu.BarNameName}");
					break;
				case "ChangeLog":
					ContentBarText = stringParserService.Parse("${res:StartPage.ChangeLogMenu.BarNameName}");
					break;
				case "Authors":
					ContentBarText = stringParserService.Parse("${res:StartPage.AuthorsMenu.BarNameName}");
					break;
				case "HelpWanted":
					ContentBarText = stringParserService.Parse("${res:StartPage.HelpWantedMenu.BarNameName}");
					break;
			}
			
			StringBuilder builder = new StringBuilder(2048);
			RenderHeaderSection(builder);
			RenderPageTopSection(builder);
			RenderFirstPageBodySection(builder);
			
			switch (section) {
				case "Start":
					RenderSectionStartBody(builder);
					break;
				case "ChangeLog":
					RenderSectionChangeLogBody(builder);
					break;
				case "Authors":
					RenderSectionAuthorBody(builder);
					break;
				case "HelpWanted":
					RenderSectionHelpWantedBody(builder);
					break;
			}
			
			RenderFinalPageBodySection(builder);
			RenderPageEndSection(builder);
			return builder.ToString();
		}
	}
}
