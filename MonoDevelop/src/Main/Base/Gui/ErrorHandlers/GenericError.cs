// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using MonoDevelop.Core.Services;

namespace MonoDevelop.Gui.ErrorHandlers
{
	class GenericError
	{
		GenericError()
		{
			
		}
		
		public static void DisplayError(string message)
		{
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			messageService.ShowError(message);
		}
	}
}
