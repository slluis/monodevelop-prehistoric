/*
 * Copyright (C) 2004 Jorn Baayen <jorn@nl.linux.org>
 * 
 * Modified by Todd Berman <tberman@sevenl.net> to fit with MonoDevelop.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 *
 * !!! Note that this class has to have the same API as the one
 *     from GNU.Gettext.dll, because otherwise the strings won't
 *     be picked up by update-po.
 */

using System.Runtime.InteropServices;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;

namespace MonoDevelop.Services
{

	public class GettextCatalog
	{

		[DllImport ("libmonodevelop")]
		private static extern void intl_init (string package);
	
		static GettextCatalog ()
		{
			intl_init ("monodevelop");
		}
	
		[DllImport ("libmonodevelop")]
		private static extern string intl_get_string (string str);
	
		public static string GetString (string str)
		{
			return intl_get_string (str);
		}
	
		[DllImport ("libmonodevelop")]
		private static extern string intl_get_plural_string (string singular,
								     string plural,
								     int n);
	
		public static string GetPluralString (string singular,
			    	 	              string plural,
					              int n)
		{
			return intl_get_plural_string (singular, plural, n);
		}
	}
}
