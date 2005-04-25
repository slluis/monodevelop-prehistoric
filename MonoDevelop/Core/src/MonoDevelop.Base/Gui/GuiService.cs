
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Gui
{
	public class GuiService
	{
		DisplayBindingService displayBindingService;
		MenuService menuService;
		IWorkbench workbench;
		ToolbarService toolbarService;
		IconService icons;
		ResourceService resourceService;
		IStatusBarService statusBarService;
		CommandService commandService;
		
		public IWorkbench Workbench {
			get { return MonoDevelop.Gui.WorkbenchSingleton.Workbench; }
		}
	
		public DisplayBindingService DisplayBindings {
			get {
				if (displayBindingService == null)
					displayBindingService = (DisplayBindingService) ServiceManager.GetService (typeof(DisplayBindingService));
				return displayBindingService;
			}
		}
	
		public MenuService Menus {
			get {
				if (menuService == null)
					menuService = (MenuService) ServiceManager.GetService (typeof(MenuService));
				return menuService;
			}
		}
	
		public IStatusBarService StatusBar {
			get {
				if (statusBarService == null)
					statusBarService = (IStatusBarService) ServiceManager.GetService (typeof(IStatusBarService));
				return statusBarService;
			}
		}
	
		public ToolbarService Toolbars {
			get {
				if (toolbarService == null)
					toolbarService = (ToolbarService) ServiceManager.GetService (typeof(ToolbarService));
				return toolbarService;
			}
		}

		public ResourceService Resources {
			get {
				if (resourceService == null)
					resourceService = (ResourceService) ServiceManager.GetService (typeof(ResourceService));
				return resourceService;
			}
		}
	
		public IconService Icons {
			get {
				if (icons == null)
					icons = (IconService) ServiceManager.GetService (typeof(IconService));
				return icons;
			}
		}
	
		public CommandService CommandService {
			get {
				if (commandService == null)
					commandService = (CommandService) ServiceManager.GetService (typeof(CommandService));
				return commandService;
			}
		}
	}
}
